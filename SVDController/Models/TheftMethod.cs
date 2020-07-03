using System;
using SVD.Interfaces;

namespace SVD.Models
{
	[Serializable]
	public class TheftMethod : BaseModel, INamable
	{
		#region accessors
		public string Name { get; set; }
		#endregion

		#region constructors
		public TheftMethod(ObjectMode mode) : base(typeof(TheftMethod), mode)
		{
		}
		#endregion

		#region public methods
		public override bool IsValid()
		{
			return !string.IsNullOrEmpty(Name);
		}
		#endregion
	}
}