using System;
using SVD.Interfaces;

namespace SVD.Models
{
	[Serializable]
	public class VehicleSecurityType : BaseModel, INamable
	{
		#region accessors
		public string Name { get; set; }
		#endregion

		#region constructors
		public VehicleSecurityType(ObjectMode mode)
			: base(typeof(VehicleSecurityType), mode)
		{
		}
		#endregion
	}
}