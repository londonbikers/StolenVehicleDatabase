using System;
using SVD.Models;

namespace SVD.Controllers
{
    public class PlacesController
    {
        #region constructors
        internal PlacesController()
        {
        }
        #endregion

        #region retrieve
        public Place GetPlace(int id)
        {
            if (id < 1)
                return null;

            var place = Controller.Instance.CacheManager.Get(Place.GetCacheKey(id)) as Place;
            if (place != null)
                return place;

            var dbPlace = Controller.Instance.Repository.GetLocation(id);
            if (dbPlace == null)
                return null;

            place = new Place(ObjectMode.Existing)
            {
                Id = dbPlace.ID,
                Code = dbPlace.Code,
                Name = dbPlace.Name,
                Type = (PlaceType) dbPlace.Type,
                Latitude = dbPlace.Latitude,
                Longitude = dbPlace.Longitude,
                ParentPlaceId = dbPlace.ParentLocationId
            };

            Controller.Instance.CacheManager.Add(place.CacheKey, place);
            return place;
        }
        #endregion

        #region create/update
        public Place UpdatePlace(Place place)
        {
            if (place == null)
                throw new ArgumentNullException(nameof(place));

            if (!place.IsValid())
                throw new ArgumentException("place is invalid!");

            // for now we are not offering update functionality, we just want to focus on creating.
            // we can add in full crud support later if required.
            if (place.Id > 0)
                return place;

            // work out if this is a new place.
            var existingLocation = GetPlace(Controller.Instance.Repository.FindLocation((byte) place.Type, place.Name));
            if (existingLocation != null)
                return existingLocation;

            lock (place)
            {
                var dbLocation = new DB_Location
                {
                    Name = place.Name, 
                    Type = (byte) place.Type,
                };

                if (place.Latitude.HasValue && place.Longitude.HasValue)
                {
                    dbLocation.Latitude = place.Latitude.Value;
                    dbLocation.Longitude = place.Longitude.Value;
                }

                if (place.ParentPlace != null)
                {
                    // the parent might be a reference to an non-persisted one...
                    if (!place.ParentPlace.IsPersisted)
                        place.ParentPlace = GetPlace(Controller.Instance.Repository.FindLocation((byte) place.ParentPlace.Type, place.ParentPlace.Name));

                    dbLocation.ParentLocationId = place.ParentPlace.Id;
                }

                if (!string.IsNullOrEmpty(place.Code)) 
                    dbLocation.Code = place.Code;

                Controller.Instance.Repository.CreateLocation(dbLocation);
                place.Id = dbLocation.ID;
                place.IsPersisted = true;
                Controller.Instance.CacheManager.Add(place.CacheKey, place);
            }

            return place;
        }
        #endregion
    }
}
