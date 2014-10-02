using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Owin.Hosting;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using Owin;
using System.Web.Http.Dispatcher;
using System.Net.Http.Headers;
using EmbeddedResourceMessageHandler;
using TestsContracts;

namespace EmbeddedResourceHandler.WebApi.Tests
{
    [TestClass]
    public class IntegrationTests : IIntegrationTests
    {
        protected static readonly string BaseAddress = "http://localhost:9000/";
        private static IDisposable app;

        [TestMethod]
        public void Response_must_contain_correct_data()
        {
            var content = GetString("EmbeddedResourceDemoData/Data/Hello.txt");

            Assert.AreEqual("Hello from root", content);
        }

        [TestMethod]
        public void Response_must_have_expires_header()
        {
            using (var client = new HttpClient())
            {
                var content = client.GetAsync(BaseAddress + "EmbeddedResourceDemoData/Data/Hello.txt").Result;

                Assert.IsTrue(content.Headers.CacheControl.MaxAge.HasValue);
                Assert.AreEqual(1, content.Headers.CacheControl.MaxAge.Value.Days);
            }
        }

        [TestMethod]
        public void Response_must_have_private_cache_header()
        {
            using (var client = new HttpClient())
            {
                var content = client.GetAsync(BaseAddress + "EmbeddedResourceDemoData/Data/Hello.txt").Result;

                Assert.IsTrue(content.Headers.CacheControl.Private);
            }
        }

        [TestMethod]
        public void Response_must_set_not_modified_header()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.IfModifiedSince = DateTime.Now;

                var content = client.GetAsync(BaseAddress + "EmbeddedResourceDemoData/Data/Hello.txt").Result;

                Assert.AreEqual(HttpStatusCode.NotModified, content.StatusCode);
            }
        }

        /// <summary>
        /// Start the self hosted server once per assembly
        /// </summary>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            app = WebApp.Start<Startup>(BaseAddress);
        }

        /// <summary>
        /// Do not forget to clean up !
        /// </summary>
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (app != null)
            {
                app.Dispose();
            }
        }

        protected string GetString(string url)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(BaseAddress + url).Result;

                var str = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(response.StatusCode + " " + str);
                }

                return str;
            }
        }

        public class Startup
        {
            public void Configuration(IAppBuilder appBuilder)
            {
                var config = new HttpConfiguration();

                config.ConfigureEmbeddedResources(typeof(Startup).Assembly, "EmbeddedResourceDemoData");

                config.EnsureInitialized();

                // register WebAPI with OWIN
                appBuilder.UseWebApi(config);
            }
        }
    }
}
