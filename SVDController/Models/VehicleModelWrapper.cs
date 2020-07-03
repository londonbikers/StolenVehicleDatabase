using SVD.VDWS;

namespace SVD.Models
{
    public class VehicleModelWrapper : BaseModel
    {
        public VehicleModel Model { get; set; }

        #region constructors
        public VehicleModelWrapper(ObjectMode mode)
            : base(typeof(VehicleModelWrapper), mode)
		{
		}
		#endregion
    }
}