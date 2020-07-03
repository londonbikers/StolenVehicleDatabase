using System.Collections.Generic;
using SVD.Models;
using SVD.VDWS;
using Webdiyer.WebControls.Mvc;

namespace SVDWebsite.Models
{
    public class BrowseIndexModel
    {
        public List<VehicleType> VehicleTypes { get; set; }
    }

    public class BrowseByVehicleTypeModel
    {
        public VehicleType VehicleType { get; set; }
        public PagedList<Vehicle> Vehicles { get; set; }
    }

    public class BrowseByVehicleModelModel
    {
        public VehicleManufacturer Manufacturer { get; set; }
        public VehicleModelWrapper ModelWrapper { get; set; }
        public PagedList<Vehicle> Vehicles { get; set; }
    }

    public class BrowseByVehicleManufacturerModel
    {
        public VehicleManufacturerWrapper ManufacturerWrapper { get; set; }
        public PagedList<Vehicle> Vehicles { get; set; }
    }
}