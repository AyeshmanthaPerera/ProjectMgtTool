using SEP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SEP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string time = DateTime.Now.ToString("HH:mm:ss tt");

        public const int Pr_Zero = 0;
        public const int Pr_one = 1;
        public const int Pr_two = 2;
        public const int Pr_three = 3;
        public const int Pr_five = 3;
        public const int Pr_minusone = -1;

        protected void Application_Start()
        { 
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
          
        }
    }
}
