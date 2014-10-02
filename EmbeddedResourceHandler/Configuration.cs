using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace EmbeddedResourceHandler
{
    public static class Configuration
    {
        public static void ConfigureEmbeddedResources(this RouteCollection routes, Assembly assembly, string prefix)
        {
            routes.Add(new Route (
                prefix + "/{*path}",
                new EmbeddedResourceHandler(assembly)
            ));
        }
    }
}
