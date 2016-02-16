using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Web.Models;
using WebMatrix.WebData;
using System.Security.Principal;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web;
using System.Net;

namespace Web.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]

    public sealed class InitializeMembershipAttribute : ActionFilterAttribute//, IAuthorizationFilter
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Обеспечение однократной инициализации ASP.NET Simple Membership при каждом запуске приложения
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }





        //private void ValidateRequestHeader(HttpRequestBase request)
        //{
        //    string cookieToken = String.Empty;
        //    string formToken = String.Empty;
        //    string tokenValue = request.Headers["RequestVerificationToken"];
        //    if (!String.IsNullOrEmpty(tokenValue))
        //    {
        //        string[] tokens = tokenValue.Split(':');
        //        if (tokens.Length == 2)
        //        {
        //            cookieToken = tokens[0].Trim();
        //            formToken = tokens[1].Trim();
        //        }
        //    }
        //    AntiForgery.Validate(cookieToken, formToken);
        //}



        public void OnAuthenticationChallenge(AuthenticationChallengeContext context)
        {
            if (context.Result == null || context.Result is HttpUnauthorizedResult)
            {
                // ...
            }
            else
            {
                //Вам сюда нельзя
            }
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
                  //  throw new InvalidOperationException("Не удалось инициализировать базу данных ASP.NET Simple Membership. Чтобы получить дополнительные сведения, перейдите по адресу: http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }


    }

    public class ValidateJsonAntiForgeryToken : AuthorizeAttribute
    {
        public JsonResult deniedResult = new JsonResult()
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            Data = new { StatusCode = HttpStatusCode.Forbidden, Error = "Access Denied" }
        };

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            System.Diagnostics.Debug.WriteLine("ValidateJsonAntiForgeryToken");
            var request = filterContext.HttpContext.Request;

            if (request.HttpMethod == WebRequestMethods.Http.Post && request.IsAjaxRequest() && request.Headers["__RequestVerificationToken"] != null)
            {
                AntiForgery.Validate(CookieValue(request), request.Headers["__RequestVerificationToken"]);
            }
            else
            {
                filterContext.Result = deniedResult;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            System.Diagnostics.Debug.WriteLine("ValidateJsonAntiForgeryToken HandleUnauthorizedRequest ");
            filterContext.Result = deniedResult;
        }

        private string CookieValue(HttpRequestBase request)
        {
            var cookie = request.Cookies[AntiForgeryConfig.CookieName];
            return cookie != null ? cookie.Value : null;
        }
    }

}
