using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.Models;
using WebMatrix.WebData;

namespace Web
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }


        protected void Application_BeginRequest()
        {
            var allowedOrigins = new[] { Request.Headers["host"] };
            var request = HttpContext.Current.Request;
            var response = HttpContext.Current.Response;
            var origin = request.Headers["Origin"];

            if (origin != null && allowedOrigins.Any(x => x == origin))
            {
                response.AddHeader("Access-Control-Allow-Origin", origin);
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, withCredentials, Authorization");
                response.AddHeader("Access-Control-Allow-Credentials", "true");
                if (request.HttpMethod == "OPTIONS")
                {
                    response.End();
                }
            }
            //if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            //{
            //    Response.Flush();
            //}
        }
        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<EFDbContext>(null);

                try
                {
                    using (var context = new EFDbContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Создание базы данных SimpleMembership без схемы миграции Entity Framework
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("EFDbContext", "UserAccount", "id", "Login", autoCreateTables: true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        } 
    }
}