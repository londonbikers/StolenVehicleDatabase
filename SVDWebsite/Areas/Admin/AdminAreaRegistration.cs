using System.Web.Mvc;

namespace SVDWebsite.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "admin/{controller}/{action}/{id}",
                new { controller = "landing", action = "index", id = UrlParameter.Optional },
                new[] { "SVDWebsite.Areas.Admin.Controllers" }
            );
        }
    }
}
