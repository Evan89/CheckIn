using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CheckInWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public const string DOMAIN_URL = "http://safteycheckin.cloudapp.net";


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Prevents this app to be embedded in other sites
            HttpContext.Current.Response.AddHeader("x-frame-options", "ALLOW-FROM=" + DOMAIN_URL);
        }
    }
}
