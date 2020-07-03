using System;

namespace SVD.Models
{
	[Serializable]
	public class Video : BaseModel
	{
		#region accessors
		public Vehicle Vehicle { get; set; }
		public string Url { get; set; }
		public string Comment { get; set; }
		#endregion

		#region constructors
		public Video(ObjectMode mode)
			: base(typeof(Video), mode)
		{
		}
		#endregion

		#region public methods
		public override bool IsValid()
		{
			if (string.IsNullOrEmpty(Url))
				return false;

			return Vehicle != null;
		}
		#endregion
	}
}