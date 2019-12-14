using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ReportesCLIN2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
      name: "Home",
      url: "{controller}/{action}/{reporte}",
      defaults: new { controller = "Home", action = "ViewReport", reporte = UrlParameter.Optional },
      constraints: new { reporte = @"\w+" }
           );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "api",
               url: "api/{action}/{id}",
               defaults: new { controller = "app", action = "Index", id = UrlParameter.Optional }
           );
        }
    }
}
