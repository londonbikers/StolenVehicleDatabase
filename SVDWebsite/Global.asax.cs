using System.Web.Mvc;
using System.Web.Routing;

namespace SVDWebsite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("ImageHandler", "photos/{year}/{month}/{day}/{filename}-{length}-{mode}.jpg", new {Controller = "Photos", Action = "Show"});
            routes.MapRoute("Profiles", "profiles/{name}/{id}", new {Controller = "Profiles", Action = "Show"});
            routes.MapRoute("BrowseVehicleTypes", "browse/{typeName}", new { Controller = "Browse", Action = "VehicleType" });
            routes.MapRoute("BrowseModels", "browse/makes/{manufacturer}/{id}/{model}/{year}", new {Controller = "Browse", Action = "VehicleModel", year = UrlParameter.Optional});
            routes.MapRoute("BrowseManufacturers", "browse/makes/{manufacturer}", new { Controller = "Browse", Action = "VehicleManufacturer" });
            routes.MapRoute("Places", "places/{id}/{name}", new {Controller = "Places", Action = "Show"});

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "home", action = "index", id = UrlParameter.Optional },
                new[] { "SVDWebsite.Controllers" }
            );
        }

        // ReSharper disable InconsistentNaming
        protected void Application_Start()
        // ReSharper restore InconsistentNaming
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            SVD.Controller.Instance.Initialise();
        }
    }
}