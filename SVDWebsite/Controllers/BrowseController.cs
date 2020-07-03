using System.Linq;
using System.Web.Mvc;
using SVD;
using SVD.Models;
using SVDWebsite.Code;
using SVDWebsite.Models;
using Webdiyer.WebControls.Mvc;
using Controller = System.Web.Mvc.Controller;

namespace SVDWebsite.Controllers
{
    public class BrowseController : Controller
    {
        // GET: /Browse/
        public ActionResult Index()
        {
            var m = new BrowseIndexModel
            {
                VehicleTypes = SVD.Controller.Instance.MakesAndModelsController.VehicleTypes
            };
            return View(m);
        }

        // GET: /Browse/Motorcycle
        public ActionResult VehicleType(string typeName, int? page)
        {
            // we add on an 's' for correct gramatical presentation.
            typeName = typeName.TrimEnd('s');

            var type = SVD.Controller.Instance.MakesAndModelsController.VehicleTypes.SingleOrDefault(q => q.Name.ToLower() == typeName.ToLower());
            if (type == null)
            {
                Helpers.AddPageMessage("Oops, no vehicle type like that found.", PageMessageType.Information);
                RedirectToAction("Index", "Home");
            }

            var currentPage = page ?? 1;
            var status = new[] { VehicleStatus.Active, VehicleStatus.Retrieved };
            var vehicles = SVD.Controller.Instance.VehicleController.GetVehiclesForType(type, status, 25, currentPage);
            var pagedList = new PagedList<Vehicle>(vehicles, currentPage, 25, vehicles.TotalCount);

            var m = new BrowseByVehicleTypeModel
            {
                VehicleType = type,
                Vehicles = pagedList
            };

            return View(m);
        }

        // GET: /Browse/Manufacturers/Honda
        public ActionResult VehicleManufacturer(string manufacturer, int? page)
        {
            manufacturer = Helpers.FromUrlPart(manufacturer);
            var currentPage = page ?? 1;
            var vehicleManufacturerWrapper = SVD.Controller.Instance.MakesAndModelsController.VehicleManufacturers.SingleOrDefault(q => q.Manufacturer.Name.ToLower() == manufacturer);
            if (vehicleManufacturerWrapper == null)
            {
                Helpers.AddPageMessage("Oops, no such manufacturer found!", PageMessageType.Error);
                RedirectToAction("Index", "Browse");
            }

            var status = new[] { VehicleStatus.Active, VehicleStatus.Retrieved };
            var vehicles = SVD.Controller.Instance.VehicleController.GetVehiclesForManufacturer(vehicleManufacturerWrapper.Manufacturer, status, 25, currentPage);
            var pagedList = new PagedList<Vehicle>(vehicles, currentPage, 25, vehicles.TotalCount);
            var m = new BrowseByVehicleManufacturerModel
            {
                ManufacturerWrapper = vehicleManufacturerWrapper,
                Vehicles = pagedList
            };

            return View(m);
        }

        // GET: /Browse/Manufacturers/Honda/CBR1000RR/2008 
        public ActionResult VehicleModel(string manufacturer, int id, string model, int? year, int? page)
        {
            manufacturer = Helpers.FromUrlPart(manufacturer);
            var currentPage = page ?? 1;
            var vehicleManufacturer = SVD.Controller.Instance.MakesAndModelsController.VehicleManufacturers.SingleOrDefault(q => q.Manufacturer.Name.ToLower() == manufacturer).Manufacturer;
            if (vehicleManufacturer == null)
            {
                Helpers.AddPageMessage("Oops, no such manufacturer found!", PageMessageType.Error);
                RedirectToAction("Index", "Browse");
            }

            var modelWrapper = SVD.Controller.Instance.MakesAndModelsController.VehicleModels.SingleOrDefault(q => q.Model.Id == id);
            if (modelWrapper == null)
            {
                Helpers.AddPageMessage("Oops, no such model found!", PageMessageType.Error);
                RedirectToAction("Index", "Browse");
            }
            
            var status = new[] { VehicleStatus.Active, VehicleStatus.Retrieved };
            var vehicles = SVD.Controller.Instance.VehicleController.GetVehiclesForModel(modelWrapper.Model, status, 25, currentPage, page);
            var pagedList = new PagedList<Vehicle>(vehicles, currentPage, 25, vehicles.TotalCount);
            var m = new BrowseByVehicleModelModel
            {
                ModelWrapper = modelWrapper,
                Manufacturer = vehicleManufacturer,
                Vehicles = pagedList
            };

            return View(m);
        }
    }
}