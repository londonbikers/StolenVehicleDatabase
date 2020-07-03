using SVD.VDWS;

namespace SVD.Models
{
    public class VehicleManufacturerWrapper : BaseModel
    {
        public VehicleManufacturer Manufacturer { get; set; }
        
        #region constructors
        public VehicleManufacturerWrapper(ObjectMode mode)
            : base(typeof(VehicleManufacturerWrapper), mode)
		{
		}
		#endregion
    }
}