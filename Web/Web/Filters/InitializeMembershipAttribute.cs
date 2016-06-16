using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Web.Models;
using WebMatrix.WebData;
using System.Web.Helpers;
using System.Web;
using System.Net;
using System.Linq;

namespace Web.Filter
{
   // [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]

    public class InitializeMembershipAttribute : AuthorizeAttribute
    {

        private string[] allowedUsers;
        private string[] allowedRoles;

        public void MyAuthorizeAttribute(string[] users, string[] roles)
        {
            allowedUsers = users;
            allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Request.IsAuthenticated &&
                allowedUsers.Contains(httpContext.User.Identity.Name) &&
                Role(httpContext);
        }

        private bool Role(HttpContextBase httpContext)
        {
            if (allowedRoles.Length > 0)
            {
                for (int i = 0; i < allowedRoles.Length; i++)
                {
                    if (httpContext.User.IsInRole(allowedRoles[i]))
                        return true;
                }
                return false;
            }
            return true;
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
