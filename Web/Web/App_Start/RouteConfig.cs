using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            string[] subhosts = {   "xn----itbevkug.xn--p1ai", 
                                     "xn----gtbhwjug.xn--p1ai",
                                     "dom-tszh.ru",
                                     "mytszh.ru",
                                     "mytsn.ru",
                                     "moe-tszh.ru",
                                     "localhost:53574"
                                 };


            routes.MapRoute(
               name: "def",
               url: "",
               defaults: new { controller = "Home", action = "Index" }
            );
            //routes.MapRoute(
            //    name: "",
            //    url: "Home/"+'#'+"/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
            routes.MapRoute(
                    name: "",
                    url: "{controller}/{action}/{id}",
                    defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional }
            );


            int i = 0;
            foreach (string p in subhosts)
            {
                i++;
                string i_str = i.ToString();
                routes.Add("default" + i_str, new SubDomainRoute(
                        p,
                        "",
                        new { controller = "Home", action = "Index", id = "Главная" },
                        new[] { "Web.Controllers" } // Namespaces defaults
                        ));
                routes.Add("default_param" + i_str, new SubDomainRoute(
                        p,
                        "{controller}/{action}/{id}",
                        new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                        new[] { "Web.Controllers" } // Namespaces defaults
                        ));

                routes.Add("host_default" + i_str, new SubDomainRoute(
                        "{host}." + p,
                        "",
                        new { controller = "Home", action = "Index", id = "Главная" },
                        new[] { "Web.Controllers" } // Namespaces defaults
                        ));

                routes.Add("host_param" + i_str, new SubDomainRoute(
                        "{host}." + p,
                        "{controller}/{action}/{id}",
                        new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                        new[] { typeof(Controllers.UserController).Namespace } // Namespaces defaults
                        ));
            }

        }
    }
}