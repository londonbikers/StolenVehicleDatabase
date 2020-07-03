using System;
using System.Configuration;

namespace SVD.Models
{
	[Serializable]
	public class Photo : BaseModel
	{
		#region accessors
		public Vehicle Vehicle { get; set; }
		public string Filename { get; set; }
		public string Comment { get; set; }
        public PhotoType Type { get; set; }
		#endregion

		#region constructors
		public Photo(ObjectMode mode)
			: base(typeof(Photo), mode)
		{
		    Type = PhotoType.HighResolution;
		}
		#endregion

		#region public methods
		public override bool IsValid()
		{
			if (string.IsNullOrEmpty(Filename))
				return false;

			return Vehicle != null;
		}

        /// <summary>
        /// Returns the full file path for a Photo.
        /// </summary>
        public string GetFilePath()
        {
            var root = ConfigurationManager.AppSettings["FileStorePath"];
            var path = $"{root}\\{DateCreated.Year}\\{DateCreated.Month}\\{DateCreated.Day}\\{Filename}";
            return path;
        }
		#endregion
	}
}