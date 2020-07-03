using System.Collections.Generic;
using System.Web.Mvc;
using SVD;
using SVD.Models;
using SVDWebsite.Code;
using SVDWebsite.Models;
using Webdiyer.WebControls.Mvc;
using Controller = System.Web.Mvc.Controller;

namespace SVDWebsite.Controllers
{
    public class PlacesController : Controller
    {
        // GET: /places/12/united-kingdom/2
        public ActionResult Show(int id, int? page)
        {
            var m = new PlaceModel();
            var p = SVD.Controller.Instance.PlacesController.GetPlace(id);

            if (p == null)
            {
                Helpers.AddPageMessage("No such place found, sorry!", PageMessageType.Error);
                return RedirectToAction("Index", "Home");
            }

            var currentPage = page ?? 1;
            var status = new[] { VehicleStatus.Active, VehicleStatus.Retrieved };
            var vehicles = SVD.Controller.Instance.VehicleController.GetVehiclesForPlace(p, status, 25, currentPage);
            var pagedList = new PagedList<Vehicle>(vehicles, currentPage, 25, vehicles.TotalCount);

            // build a list of all parent-places, high to low-level.
            if (p.ParentPlace != null)
            {
                m.ParentPlaces = new List<Place>();
                var parent = p.ParentPlace;
                while (parent != null)
                {
                    var newPlace = parent;
                    m.ParentPlaces.Add(newPlace);
                    parent = parent.ParentPlace;
                }

                m.ParentPlaces.Reverse();
            }

            m.Place = p;
            m.Vehicles = pagedList;
            return View(m);
        }
    }
}