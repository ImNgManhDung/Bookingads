using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using BookingAds.Common.Repository.Account;
using BookingAds.Constants;
using BookingAds.Entities;
using BookingAds.Modules;

namespace BookingAds.Attributes.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RoleFilterAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var accountRepo = new AccountRepository();
            if (filterContext == null)
            {
                return;
            }

            var currentUser = ConvertUtils<Account>.Deserialize(filterContext.HttpContext.User.Identity.Name);

            if (!string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name) && currentUser != null)
            {
                var isAdmin = accountRepo.IsAdmin(currentUser.UserName);
                var isEmployee = accountRepo.IsEmployee(currentUser.UserName);
                var isLockedEmployee = accountRepo.IsLockedEmployee(currentUser.UserName);

                if (isLockedEmployee)
                {
                    FormsAuthentication.SignOut();
                    filterContext.Result =
                        new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                    return;
                }

                var isForbiden = (Roles.Contains(RoleConstant.ADMIN) && !isAdmin) || (Roles.Contains(RoleConstant.EMPLOYEE) && !isEmployee);

                if (isForbiden)
                {
                    var viewResult = new ViewResult();
                    var viewData = new ViewDataDictionary
                    {
                        { RoleConstant.ADMIN, isAdmin },
                    };
                    if (isAdmin)
                    {
                        viewResult.ViewData = viewData;
                    }

                    viewResult.ViewName = "~/Areas/Admin/Views/Shared/_Forbiden.cshtml";
                    filterContext.Result = viewResult;
                    return;
                }
            }

            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var currentUser = ConvertUtils<Account>.Deserialize(httpContext.User.Identity.Name);
            return currentUser != null;
        }
    }
}