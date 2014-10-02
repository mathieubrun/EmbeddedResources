using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;
using EmbeddedResourceHandler;
using EmbeddedResourceMessageHandler;
using EmbeddedResourceHandler.Sample.Data;
using System.Web.Routing;

namespace EmbeddedResourceHandler.Sample
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configuration.ConfigureEmbeddedResources(typeof(Global).Assembly, "FromSame");

            GlobalConfiguration.Configuration.ConfigureEmbeddedResources(typeof(Class1).Assembly, "FromExternal");
        }
    }
}