using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace EmbeddedResourceMessageHandler
{
    public static class Configuration
    {
        public static void ConfigureEmbeddedResources(this HttpConfiguration config, Assembly assembly, string prefix)
        {
            var handlers = new DelegatingHandler[] {
                new EmbeddedResourceMessageHandler(assembly)
            };

            var routeHandlers = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config), handlers);

            config.Routes.MapHttpRoute(
                name: prefix + "Route",
                routeTemplate: prefix + "/{*path}",
                defaults: null,
                constraints: null,
                handler: routeHandlers);
        }
    }
}
