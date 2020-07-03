using System.Web.Mvc;
using SVD.Models;
using SVDWebsite.Areas.Admin.Models;
using Webdiyer.WebControls.Mvc;

namespace SVDWebsite.Areas.Admin.Controllers
{
    public class LandingController : Controller
    {
        // GET: /Admin/
        public ActionResult Index(int? page)
        {
            var currentPage = page ?? 1;
            var vehicles = SVD.Controller.Instance.VehicleController.GetVehicles(25, currentPage);
            var pagedList = new PagedList<Vehicle>(vehicles, currentPage, 25, vehicles.TotalCount);
            var m = new LandingModel {Vehicles = pagedList};
            return View(m);
        }
    }
}