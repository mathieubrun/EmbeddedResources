using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using TestsContracts;
using System.Collections.Specialized;
using System.Web.Routing;
using System.IO;

namespace EmbeddedResourceHandler.Test
{
    [TestClass]
    public class EmbeddedResourceHandlerTest : IIntegrationTests
    {
        private Mock<HttpContextBase> _httpContext;
        private Mock<HttpResponseBase> _httpResponse;
        private Mock<HttpRequestBase> _httpRequest;
        private Mock<HttpCachePolicyBase> _httpCachePolicyBase;
        private NameValueCollection nvc;

        private EmbeddedResourceHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _httpContext = new Mock<HttpContextBase>();
            _httpResponse = new Mock<HttpResponseBase>();
            _httpRequest = new Mock<HttpRequestBase>();
            _httpCachePolicyBase = new Mock<HttpCachePolicyBase>();

            nvc = new NameValueCollection();
            _httpRequest.SetupGet(request => request.Headers).Returns(nvc);

            var routeData = new RouteData();
            routeData.Values.Add("path", "Data/Hello.txt");

            _httpRequest.SetupGet(request => request.RequestContext).Returns(new RequestContext(_httpContext.Object, routeData));

            _httpResponse.SetupGet(response => response.OutputStream).Returns(new MemoryStream());
            _httpResponse.SetupGet(response => response.Cache).Returns(_httpCachePolicyBase.Object);

            _httpContext.SetupGet(context => context.Response).Returns(_httpResponse.Object);
            _httpContext.SetupGet(context => context.Request).Returns(_httpRequest.Object);

            _handler = new EmbeddedResourceHandler(typeof(EmbeddedResourceHandlerTest).Assembly);
        }

        [TestMethod]
        public void Response_must_contain_correct_data()
        {
            _handler.ProcessRequest(_httpContext.Object);

            _httpResponse.VerifySet(response => response.StatusCode = 200);
        }

        [TestMethod]
        public void Response_must_have_expires_header()
        {
            _handler.ProcessRequest(_httpContext.Object);

            _httpCachePolicyBase.Verify(x => x.SetExpires(It.IsAny<DateTime>()));
        }

        [TestMethod]
        public void Response_must_have_private_cache_header()
        {
            _handler.ProcessRequest(_httpContext.Object);

            _httpCachePolicyBase.Verify(x => x.SetCacheability(HttpCacheability.Private));
        }

        [TestMethod]
        public void Response_must_set_not_modified_header()
        {
            nvc.Add("If-Modified-Since", DateTime.Now.ToString());

            _handler.ProcessRequest(_httpContext.Object);

            _httpResponse.VerifySet(response => response.StatusCode = 304);
        }
    }
}
