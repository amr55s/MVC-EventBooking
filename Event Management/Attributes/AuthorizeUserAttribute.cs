using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Event_Management.Attributes
{
    /// <summary>
    /// Requires a logged-in user with UserType == "User" (ticket booking flows).
    /// Unauthenticated → Login; wrong role → AccessDenied.
    /// </summary>
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            if (httpContext.Session.GetInt32("UserId") == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new
                {
                    returnUrl = httpContext.Request.Path + httpContext.Request.QueryString,
                });
                return;
            }

            if (httpContext.Session.GetString("UserType") != "User")
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
