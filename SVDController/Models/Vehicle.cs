using System;
using System.Collections.Generic;
using SVD.VDWS;

namespace SVD.Models
{
	[Serializable]
	public class Vehicle : BaseModel
	{
		#region members
		private List<Photo> _photos;
		private List<Video> _videos;
		private List<VehicleSecurityType> _securityTypes;
		private DateTime? _theftDate = DateTime.MinValue;
		#endregion

		#region accessors
		public Guid MemberUid { get; set; }
		public VehicleModel Model { get; set; }
		public Colour Colour { get; set; }
        public VehicleStatus Status { get; set; }
		/// <summary>
		/// The year of manufacture.
		/// </summary>
		public int? Year { get; set; }
		/// <summary>
		/// The vehicle registration, i.e. "LF05 BJY".
		/// </summary>
		public string Registration { get; set; }
		/// <summary>
		/// Vehicle Identification Number. 17 characters long exactly. Identifies the make, model, year and location of manufacture.
		/// </summary>
		public string Vin { get; set; }
        /// <summary>
        /// Engine number. 17 characters long exactly (or 11 chars if very old).
        /// </summary>
        public string EngineNumber { get; set; }
		/// <summary>
		/// A general description of the vehicle.
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Which Police force was it reported to? I.E. Hammersmith, Metropolitan Police, UK.
		/// </summary>
		public string PoliceForce { get; set; }
		/// <summary>
		/// A phone number people can use to provide information on the theft to the Police.
		/// </summary>
		public string PolicePhoneNumber { get; set; }
		/// <summary>
		/// The Police-supplied reference for the vehicle theft crime.
		/// </summary>
		public string PoliceReference { get; set; }
		/// <summary>
		/// A description of the theft of the vehicle.
		/// </summary>
		public string TheftDescription { get; set; }
		/// <summary>
		/// Where was the vehicle stolen from?
		/// </summary>
		public decimal? TheftLatitude { get; set; }
		/// <summary>
		/// Where was the vehicle stolen from?
		/// </summary>
        public decimal? TheftLongitude { get; set; }
		/// <summary>
		/// Indicates whether the latitude/longitude values (if present) are approximate or specific.
		/// For instance, a user can protect their privacy by making a location as approximate and we record the location to the nearest post-code.
		/// </summary>
		public bool IsLocationApproximate { get; set; }
        /// <summary>
        /// Provides a collection of places that describe the theft-location as set by the lat/lng values.
        /// </summary>
        public List<Place> TheftLocationPlaces { get; set; }
		/// <summary>
		/// When was the vehicle stolen?
		/// </summary>
		public DateTime? TheftDate { get { return _theftDate; } set { _theftDate = value; } }
		/// <summary>
		/// How was the vehicle stolen?
		/// </summary>
		public TheftMethod TheftMethod { get; set; }
		public List<Photo> Photos
		{
			get
			{
				if (_photos == null)
					GetPhotos();

				return _photos;
			}
		}
		public List<Video> Videos
		{
			get
			{
				if (_videos == null)
					GetVideos();

				return _videos;
			}
		}
		public List<VehicleSecurityType> SecurityTypes
		{
			get
			{
				if (_securityTypes == null)
					GetSecurityTypes();

				return _securityTypes;
			}
            set
            {
                _securityTypes = value;
            }
		}
	    /// <summary>
	    /// A friendly title for the vehicle, i.e. "2005 Honda Fireblade".
	    /// </summary>
	    /// <remarks>This could well end up needing to be changed, perhaps cache the make here... Increases complexity though at the benefit of performance.</remarks>
	    public string Title
	    {
	        get
	        {
	            var m = Controller.Instance.MakesAndModelsController.GetManufacturer(Model.ManufacturerId);
	            return $"{Year} {m.Manufacturer.Name} {Model.Name}";
	        }
	    }
		#endregion

		#region constructors
		public Vehicle(ObjectMode mode)
			: base(typeof(Vehicle), mode)
		{
		    Status = VehicleStatus.Active;
		    TheftLocationPlaces = new List<Place>();
		}
		#endregion

		#region static methods
		/// <summary>
		/// Gets the cache-key for a specific Vehicle instance.
		/// </summary>
		/// <remarks>
		/// This is a convention as you can't have a static abstract method on the base, or a static method on an interface.
		/// Know of a better way to ensure this method stays consistent across domain-objects?
		/// </remarks>
		public static string GetCacheKey(int id)
		{
			return GenerateCacheKey(typeof(Vehicle), id);
		}
		#endregion

		#region public methods
		public override bool IsValid()
		{
			if (MemberUid == Guid.Empty)
				return false;

			if (Model == null)
				return false;

			// at least one of two identifiers is needed.
			if (string.IsNullOrEmpty(Registration) && string.IsNullOrEmpty(Vin))
				return false;

			if (TheftDate == DateTime.MinValue)
				return false;

			if (TheftMethod == null)
				return false;

			return true;
		}

		public Video AddVideo(string url)
		{
			return AddVideo(url, string.Empty);
		}

		public Video AddVideo(string url, string comment)
		{
            lock (Videos)
            {
                var v = new Video(ObjectMode.New) {Vehicle = this, Url = url, Comment = comment};
                Videos.Add(v);
                return v;
            }
		}

        public Photo AddPhoto(string filename, PhotoType type)
		{
            return AddPhoto(filename, string.Empty, type);
		}

		public Photo AddPhoto(string filename, string comment, PhotoType type)
		{
            lock (Photos)
            {
                var p = new Photo(ObjectMode.New) { Vehicle = this, Filename = filename, Comment = comment, Type = type };
                Photos.Add(p);
                return p;
            }
		}
		#endregion

		#region private methods
		private void GetPhotos()
		{
			_photos = new List<Photo>();
            lock (_photos)
            {
                foreach (var vp in Controller.Instance.Repository.GetVehiclePhotos(Id))
                {
                    // candidate for putting this into a build-photos methods on the vehicle-controller, but as this
                    // is the only place it's being done for now, let's keep it simple and self-contained.

                    var p = new Photo(ObjectMode.Existing)
                    {
                        Id = vp.ID,
                        Vehicle = this,
                        DateCreated = vp.DateCreated,
                        DateModified = vp.DateModified,
                        Filename = vp.Filename,
                        Comment = vp.Comment,
                        Type = (PhotoType) vp.PhotoType
                    };

                    _photos.Add(p);
                }
            }
		}

		private void GetVideos()
		{
			_videos = new List<Video>();
            lock (_videos)
            {
                foreach (var vv in Controller.Instance.Repository.GetVehicleVideos(Id))
                {
                    // candidate for putting this into a build-videos methods on the vehicle-controller, but as this
                    // is the only place it's being done for now, let's keep it simple and self-contained.

                    var v = new Video(ObjectMode.Existing)
                    {
                        Id = vv.ID,
                        Vehicle = this,
                        Url = vv.Url,
                        Comment = vv.Comment
                    };

                    _videos.Add(v);
                }
            }
		}

		private void GetSecurityTypes()
		{
			_securityTypes = new List<VehicleSecurityType>();
            lock (_securityTypes)
            {
                foreach (var vs in Controller.Instance.Repository.GetVehicleSecurities(Id))
                {
                    // candidate for putting this into a build-securities method on the vehicle-controller, but as this
                    // is the only place it's being done for now, let's keep it simple and self-contained.

                    var vst = new VehicleSecurityType(ObjectMode.New) { Id = vs.DB_VehicleSecurityType.ID, Name = vs.DB_VehicleSecurityType.Name };
                    _securityTypes.Add(vst);
                }
            }
		}
		#endregion
	}
}