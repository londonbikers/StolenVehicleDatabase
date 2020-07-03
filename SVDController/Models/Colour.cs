using System;
using SVD.Interfaces;

namespace SVD.Models
{
	[Serializable]
	public class Colour : BaseModel, INamable
	{
		#region accessors
		public string Name { get; set; }
		#endregion

		#region constructors
		public Colour(ObjectMode mode)
			: base(typeof(Colour), mode)
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