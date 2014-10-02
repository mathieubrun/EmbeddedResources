using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace EmbeddedResourceHandler
{
    public class EmbeddedResourceHandler : IHttpHandler, IRouteHandler
    {
        private readonly Assembly assembly;

        public EmbeddedResourceHandler(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new HttpContextWrapper(context));
        }

        public void ProcessRequest(HttpContextBase context)
        {
            var dateTime = GetModificationDate(assembly);

            if (HasBeenModified(context.Request, dateTime))
            {
                context.Response.StatusCode = 304;
                return;
            }

            var ctx = context.Request.RequestContext;

            var path = ctx.RouteData.Values["path"] as string;

            var str = GetStream(assembly, path);

            str.CopyTo(context.Response.OutputStream);

            context.Response.ContentType = GetMimeType(path);

            context.Response.StatusCode = 200;

            context.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(1));
            context.Response.Cache.SetCacheability(HttpCacheability.Private);
            context.Response.Cache.SetLastModified(dateTime);
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        public static DateTime GetModificationDate(Assembly assembly)
        {
            var dateTime = File.GetLastWriteTimeUtc(assembly.CodeBase.Replace("file:///", ""));

            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);

            return dateTime;

        }

        public static bool HasBeenModified(HttpRequestBase request, DateTime date)
        {
            string rawIfModifiedSince = request.Headers.Get("If-Modified-Since");

            if (!string.IsNullOrEmpty(rawIfModifiedSince))
            {
                DateTime ifModifiedSince = DateTime.Parse(rawIfModifiedSince).ToUniversalTime();

                if (date <= ifModifiedSince)
                {
                    return true;
                }
            }

            return false;
        }

        public static Stream GetStream(Assembly assembly, string path)
        {
            return assembly.GetManifestResourceStream(assembly.GetName().Name + "." + path.Replace("/", "."));
        }

        public static string GetMimeType(string path)
        {
            return MimeMapping.GetMimeMapping(Path.GetFileName(path));
        }
    }
}
