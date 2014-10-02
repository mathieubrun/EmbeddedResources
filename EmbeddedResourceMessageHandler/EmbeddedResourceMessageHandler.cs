using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EmbeddedResourceMessageHandler
{
    public class EmbeddedResourceMessageHandler : DelegatingHandler
    {
        private readonly Assembly assembly;

        public EmbeddedResourceMessageHandler(Assembly assembly)
        {
            this.assembly = assembly;
        }

        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                HttpResponseMessage response = task.Result;

                var dateTime = GetModificationDate(assembly);

                var rawIfModifiedSince = request.Headers.IfModifiedSince;

                if (HasNotBeenModified(rawIfModifiedSince, dateTime))
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotModified;

                    return response;
                }

                var ctx = request.GetRequestContext();

                var path = ctx.RouteData.Values["path"] as string;

                var str = GetStream(assembly, path);

                response.Content = new StreamContent(str);

                response.StatusCode = System.Net.HttpStatusCode.OK;

                response.Headers.CacheControl = new CacheControlHeaderValue { Private = true, MaxAge = TimeSpan.FromDays(1) };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(GetMimeType(path));
                response.Content.Headers.LastModified = dateTime;

                return response;
            });
        }

        public static DateTime GetModificationDate(Assembly assembly)
        {
            var dateTime = File.GetLastWriteTimeUtc(assembly.CodeBase.Replace("file:///", ""));

            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);

            return dateTime;

        }

        public static bool HasNotBeenModified(DateTimeOffset? offset, DateTime date)
        {
            if (offset.HasValue && date <= offset.Value)
            {
                return true;
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
