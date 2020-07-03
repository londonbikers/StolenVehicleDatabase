using System.Collections;
using System.Configuration;

namespace SVD.Models
{
    /// <summary>
    /// A place is used to identity an area, whether it's a country, city, bay, etc.
    /// </summary>
    public class Place : BaseModel
    {
        #region members
        private Hashtable _simpleStats;
        private Place _parentPlace;
        #endregion

        #region accessors
        public PlaceType Type { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Place ParentPlace
        {
            get
            {
                if (_parentPlace == null && ParentPlaceId.HasValue)
                    _parentPlace = Controller.Instance.PlacesController.GetPlace(ParentPlaceId.Value);

                return _parentPlace;
            }
            set
            {
                _parentPlace = value;
                ParentPlaceId = _parentPlace.Id;
            }
        }
        /// <summary>
        /// Used to store the parent-place id for lazy-loading as required.
        /// </summary>
        internal int? ParentPlaceId { get; set; }
        /// <summary>
        /// A Google Maps zoom level that should be roughly appropriate for the type of place.
        /// </summary>
        public int DefaultMapZoomLevel
        {
            get
            {
                switch (Type)
                {
                    case PlaceType.Route:
                        return 17;
                    case PlaceType.SubLocality:
                        return 16;
                    case PlaceType.Locality:
                        return 15;
                    case PlaceType.AdministrativeAreaLevel2:
                        return 10;
                    case PlaceType.AdministrativeAreaLevel1:
                        return 8;
                    case PlaceType.Country:
                        return 6;
                    default:
                        return 16;
                }
            }
        }

        /// <summary>
        /// Holds a simple bag of statistic names and values. Good candidate for further development...
        /// </summary>
        public Hashtable SimpleStats
        {
            get
            {
                if (_simpleStats == null)
                    RetrieveSimpleStats();

                return _simpleStats;
            }
            internal set
            {
                _simpleStats = value;
            }
        }

        public int SimpleStatsUpdateIntervalMins => int.Parse(ConfigurationManager.AppSettings["BackgroundTasks.PlaceStatsProcessIntervalMins"]);
        #endregion

        #region constructors
        public Place(ObjectMode mode) : base(typeof(Place), mode)
		{
		}
        #endregion

        #region static methods
        /// <summary>
        /// Gets the cache-key for a specific Place instance.
        /// </summary>
        /// <remarks>
        /// This is a convention as you can't have a static abstract method on the base, or a static method on an interface.
        /// Know of a better way to ensure this method stays consistent across domain-objects?
        /// </remarks>
        public static string GetCacheKey(int id)
        {
            return GenerateCacheKey(typeof(Place), id);
        }
        #endregion

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Name);
        }

        #region private methods
        private void RetrieveSimpleStats()
        {
            _simpleStats = Controller.Instance.Repository.GetLocationSimpleStats(Id);
        }
        #endregion
    }
}