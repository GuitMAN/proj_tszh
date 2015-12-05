using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using Web.Models;
//using System.Web.Mvc.Filters;
using System.Web.Routing;
using System.Security.Principal;


namespace Web.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Обеспечение однократной инициализации ASP.NET Simple Membership при каждом запуске приложения
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

     //   public void OnAuthentication(AuthenticationContext context)
     //   {
     //       IIdentity ident = context.Principal.Identity;
     //       if (!ident.IsAuthenticated)
     //       {
     ////           context.Result = new HttpUnauthorizedResult();
     //       }

     //   }

     //   public void OnAuthenticationChallenge(AuthenticationChallengeContext context)
     //   {
     //       if (context.Result == null|| context.Result is HttpUnauthorizedResult)
     //       {
     //           context.Result = new RedirectToRouteResult(new RouteValueDictionary {
     //               {"controller", "Login"}, 
     //               {"action",  "Index"}, 
     //               {"returnUrl", context.HttpContext.Request.RawUrl}
     //           });
     //       }                  
       
     //   }

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
                  //  throw new InvalidOperationException("Не удалось инициализировать базу данных ASP.NET Simple Membership. Чтобы получить дополнительные сведения, перейдите по адресу: http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}
