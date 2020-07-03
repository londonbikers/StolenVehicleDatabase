using System;
using System.Web.Mvc;
using System.Web.Security;
using SVDWebsite.Code;
using SVDWebsite.Models;

namespace SVDWebsite.Controllers
{
    public class ProfilesController : Controller
    {
        // GET: /Profiles/2005-Suzuki-GSX-R-1000/1564
        public ActionResult Show(string name, int id)
        {
            if (id < 1)
                return RedirectToAction("Index", "Home");

            var vehicle = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (vehicle == null)
            {
                Helpers.AddPageMessage("Sorry, no such vehicle found!", PageMessageType.Error);
                return RedirectToAction("Index", "Home");
            }

            // only the vehicle owner or staff can view archived profiles.
            if (vehicle.Status == SVD.VehicleStatus.Archived && !Roles.IsUserInRole("Staff"))
            {
                var user = Membership.GetUser();
                if (user != null && user.ProviderUserKey != null && vehicle.MemberUid != (Guid)user.ProviderUserKey)
                {
                    Helpers.AddPageMessage("Sorry, that vehicle has been archived.", PageMessageType.Error);
                    return RedirectToAction("Index", "Home");
                }
            }

            var manufacturerName = SVD.Controller.Instance.MakesAndModelsController.GetManufacturer(vehicle.Model.ManufacturerId).Manufacturer.Name;
            var m = new ShowProfileModel { Vehicle = vehicle, ManufacturerName = manufacturerName };
            return View(m);
        }
    }
}