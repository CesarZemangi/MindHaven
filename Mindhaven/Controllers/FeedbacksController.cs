using System.Web.Mvc;

namespace Mindhaven.Helpers
{
    public static class RoleAuthorizationHelper
    {
        public static ActionResult AuthorizeUserRole(Controller controller, string allowedRoles)
        {
            if (controller == null)
                return new RedirectResult("~/Account/Login");

            var sessionRole = controller.Session["Role"] != null
                ? controller.Session["Role"].ToString()
                : string.Empty;

            if (string.IsNullOrEmpty(sessionRole))
            {
                controller.TempData["ErrorMessage"] = "Your session has expired. Please log in again.";
                return new RedirectResult("~/Account/Login");
            }

            var allowed = allowedRoles.Split(',');
            bool authorized = false;

            foreach (var role in allowed)
            {
                if (sessionRole.Equals(role.Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    authorized = true;
                    break;
                }
            }

            if (!authorized)
            {
                controller.TempData["ErrorMessage"] = "You do not have permission to access this section.";
                return new RedirectResult("~/Home/AccessDenied");
            }

            return null;
        }
    }
}
