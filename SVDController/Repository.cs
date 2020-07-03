using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SVD
{
	public class Repository : IRepository
	{
		#region members
	    readonly SVDDataContext _db;
		#endregion

		#region constructors
		internal Repository()
		{
		    _db = new SVDDataContext(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
		}
		#endregion

		#region vehicles
        public IEnumerable<int> GetVehiclesForUser(Guid userId)
        {
            var ids = (from vehicle in _db.DB_Vehicles
                       where vehicle.MemberUID == userId
                       orderby vehicle.DateCreated descending
                       select vehicle.ID).AsEnumerable();
            return ids;
        }

        public IEnumerable<int> GetVehicles(int pageNumber, int pageSize, out int totalCount)
        {
            // get the total-count. we could make this an overload option or something to avoid having to do it if we have a need.
            totalCount = _db.DB_Vehicles.Count();

            var ids = (from vehicle in _db.DB_Vehicles
                       orderby vehicle.DateCreated descending
                       select vehicle.ID).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();

            return ids;
        }

	    public IEnumerable<int> GetVehiclesForType(int typeId, int pageNumber, int pageSize, out int totalCount, byte[] vehicleStatus)
	    {
            // get the total-count. we could make this an overload option or something to avoid having to do it if we have a need.
	        totalCount = _db.DB_Vehicles.Count(q => q.VehicleTypeId == typeId && vehicleStatus.Contains(q.Status));

	        var ids = (from vehicle in _db.DB_Vehicles
	                   where vehicle.VehicleTypeId == typeId && vehicleStatus.Contains(vehicle.Status)
	                   orderby vehicle.DateCreated descending
	                   select vehicle.ID).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();

            return ids;
	    }

        public IEnumerable<int> GetVehiclesForModel(int modelId, int pageNumber, int pageSize, out int totalCount, byte[] vehicleStatus, int? year = null)
        {
            // get the total-count. we could make this an overload option or something to avoid having to do it if we have a need.
            totalCount = _db.DB_Vehicles.Count(q => q.ModelID == modelId && vehicleStatus.Contains(q.Status));

            IEnumerable<int> ids;
            if (year != null)
            {
                ids = (from vehicle in _db.DB_Vehicles
                           where vehicle.ModelID == modelId && vehicleStatus.Contains(vehicle.Status) && vehicle.Year == year
                           orderby vehicle.DateCreated descending
                           select vehicle.ID).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();
            }
            else
            {
                ids = (from vehicle in _db.DB_Vehicles
                       where vehicle.ModelID == modelId && vehicleStatus.Contains(vehicle.Status)
                       orderby vehicle.DateCreated descending
                       select vehicle.ID).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();
            }

            return ids;
        }

        public IEnumerable<int> GetVehiclesForManufacturer(int manufacturerId, int pageNumber, int pageSize, out int totalCount, byte[] vehicleStatus)
        {
            // get the total-count. we could make this an overload option or something to avoid having to do it if we have a need.
            totalCount = _db.DB_Vehicles.Count(q => q.ManufacturerID == manufacturerId && vehicleStatus.Contains(q.Status));

            var ids = (from vehicle in _db.DB_Vehicles
                        where vehicle.ManufacturerID == manufacturerId && vehicleStatus.Contains(vehicle.Status)
                        orderby vehicle.DateCreated descending
                        select vehicle.ID).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();

            return ids;
        }

        public IEnumerable<int> GetVehiclesForPlace(int placeId, int pageNumber, int pageSize, out int totalCount, byte[] vehicleStatus)
        {
            // get the total-count. we could make this an overload option or something to avoid having to do it if we have a need.
            totalCount = _db.DB_VehicleTheftLocations.Count(q => q.LocationID == placeId && vehicleStatus.Contains(q.DB_Vehicle.Status));

            var ids = (from vtl in _db.DB_VehicleTheftLocations
                       where vtl.LocationID == placeId && vehicleStatus.Contains(vtl.DB_Vehicle.Status)
                       orderby vtl.DB_Vehicle.DateCreated descending
                       select vtl.VehicleID).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();

            return ids;
        }

	    public IEnumerable<int> GetLatestVehicles(int maxVehicles)
		{
			var ids = (from vehicle in _db.DB_Vehicles
									orderby vehicle.DateCreated descending
									select vehicle.ID).Take(maxVehicles).AsEnumerable();
			return ids;
		}

		public DB_Vehicle GetVehicle(int id)
		{
			return _db.DB_Vehicles.FirstOrDefault(q => q.ID == id);
		}

		public void UpdateVehicle(DB_Vehicle vehicle)
		{
			if (vehicle == null)
				throw new ArgumentNullException(nameof(vehicle));

			if (vehicle.ID < 1)
				_db.DB_Vehicles.InsertOnSubmit(vehicle);

			_db.SubmitChanges();
		}

		public void DeleteVehicle(int id)
		{
            if (id < 1)
                throw new ArgumentException("id is less than 1!");

		    var v = _db.DB_Vehicles.SingleOrDefault(q => q.ID == id);
            if (v == null)
                throw new Exception("No such vehicle found!");

            foreach (var vp in v.DB_VehiclePhotos)
                _db.DB_VehiclePhotos.DeleteOnSubmit(vp);

            foreach (var vv in v.DB_VehicleVideos)
                _db.DB_VehicleVideos.DeleteOnSubmit(vv);

            foreach (var vs in v.DB_VehicleSecurities)
                _db.DB_VehicleSecurities.DeleteOnSubmit(vs);

            foreach (var vl in v.DB_VehicleTheftLocations)
                _db.DB_VehicleTheftLocations.DeleteOnSubmit(vl);

            _db.DB_Vehicles.DeleteOnSubmit(v);
            _db.SubmitChanges();
		}

        public void CreateVehicleLocationPlace(int vehicleId, int placeId)
        {
            var dbVehicleTheftLocationPlace = new DB_VehicleTheftLocation
            {
                LocationID = placeId,
                VehicleID = vehicleId
            };

            _db.DB_VehicleTheftLocations.InsertOnSubmit(dbVehicleTheftLocationPlace);
            _db.SubmitChanges();
        }

        public IEnumerable<DB_VehicleTheftLocation> GetVehicleTheftLocationPlaces(int vehicleId)
        {
            return _db.DB_VehicleTheftLocations.Where(q => q.VehicleID == vehicleId).AsEnumerable();
        }

		public IEnumerable<DB_VehiclePhoto> GetVehiclePhotos(int vehicleId)
		{
			return _db.DB_VehiclePhotos.Where(q => q.VehicleID == vehicleId).AsEnumerable();
		}

		public IEnumerable<DB_VehicleVideo> GetVehicleVideos(int vehicleId)
		{
			return _db.DB_VehicleVideos.Where(q => q.VehicleID == vehicleId).AsEnumerable();
		}

		public IEnumerable<DB_VehicleSecurity> GetVehicleSecurities(int vehicleId)
		{
			return _db.DB_VehicleSecurities.Where(q => q.VehicleID == vehicleId).AsEnumerable();
		}

		public DB_VehiclePhoto GetVehiclePhoto(int photoId)
		{
			return _db.DB_VehiclePhotos.SingleOrDefault(q => q.ID == photoId);
		}

		public DB_VehicleVideo GetVehicleVideo(int videoId)
		{
			return _db.DB_VehicleVideos.SingleOrDefault(q => q.ID == videoId);
		}

		public DB_VehicleSecurity GetVehicleSecurity(int vehicleId, int typeId)
		{
			return _db.DB_VehicleSecurities.SingleOrDefault(q => q.VehicleID == vehicleId && q.DB_VehicleSecurityType.ID == typeId);
		}

		public void UpdateVehiclePhoto(DB_VehiclePhoto vehiclePhoto)
		{
			if (vehiclePhoto == null)
				throw new ArgumentNullException(nameof(vehiclePhoto));

            if (vehiclePhoto.ID < 1)
                _db.DB_VehiclePhotos.InsertOnSubmit(vehiclePhoto);

            _db.SubmitChanges();
		}

		public void UpdateVehicleVideo(DB_VehicleVideo vehicleVideo)
		{
			if (vehicleVideo == null)
				throw new ArgumentNullException(nameof(vehicleVideo));

            if (vehicleVideo.ID < 1)
                _db.DB_VehicleVideos.InsertOnSubmit(vehicleVideo);

            _db.SubmitChanges();
		}

		public void UpdateVehicleSecurity(DB_VehicleSecurity vehicleSecurity)
		{
			if (vehicleSecurity == null)
				throw new ArgumentNullException(nameof(vehicleSecurity));

            if (vehicleSecurity.ID < 1)
                _db.DB_VehicleSecurities.InsertOnSubmit(vehicleSecurity);

            _db.SubmitChanges();
		}

		public void DeleteVehiclePhoto(int vehiclePhotoId)
		{
		    var vehiclePhoto = _db.DB_VehiclePhotos.SingleOrDefault(q => q.ID == vehiclePhotoId);
			if (vehiclePhoto == null)
                throw new ArgumentException("vehiclePhotoId doesn't match any known object.");

            _db.DB_VehiclePhotos.DeleteOnSubmit(vehiclePhoto);
            _db.SubmitChanges();
		}

		public void DeleteVehicleVideo(int vehicleVideoId)
		{
		    var vehicleVideo = _db.DB_VehicleVideos.SingleOrDefault(q => q.ID == vehicleVideoId);
			if (vehicleVideo == null)
                throw new ArgumentException("vehicleVideoId doesn't match any known object.");

            _db.DB_VehicleVideos.DeleteOnSubmit(vehicleVideo);
            _db.SubmitChanges();
		}

        public void DeleteVehicleTheftLocationPlace(int vehicleId, int locationId)
        {
            var vehicleTheftLocationPlace = _db.DB_VehicleTheftLocations.SingleOrDefault(q => q.LocationID == locationId && q.VehicleID == vehicleId);
            if (vehicleTheftLocationPlace == null)
                throw new ArgumentException("no matching object found.");

            _db.DB_VehicleTheftLocations.DeleteOnSubmit(vehicleTheftLocationPlace);
            _db.SubmitChanges();
        }

		public void DeleteVehicleSecurity(int vehicleSecurityId)
		{
		    var vehicleSecurity = _db.DB_VehicleSecurities.SingleOrDefault(q => q.ID == vehicleSecurityId);
			if (vehicleSecurity == null)
                throw new ArgumentException("vehicleSecurityId doesn't match any known object.");

            _db.DB_VehicleSecurities.DeleteOnSubmit(vehicleSecurity);
            _db.SubmitChanges();
		}
		#endregion

		#region theft-methods
		public IEnumerable<DB_TheftMethod> GetTheftMethods()
		{
			return _db.DB_TheftMethods.OrderBy(q=>q.Name).AsEnumerable();
		}

		public DB_TheftMethod GetTheftMethod(int id)
		{
			return _db.DB_TheftMethods.FirstOrDefault(q => q.ID == id);
		}

		public void UpdateTheftMethod(DB_TheftMethod theftMethod)
		{
			if (theftMethod == null)
				throw new ArgumentNullException(nameof(theftMethod));

            if (theftMethod.ID < 1)
                _db.DB_TheftMethods.InsertOnSubmit(theftMethod);

            _db.SubmitChanges();
		}

		public void DeleteTheftMethod(int theftMethodId)
		{
		    var theftMethod = _db.DB_TheftMethods.SingleOrDefault(q => q.ID == theftMethodId);
			if (theftMethod == null)
                throw new ArgumentException("theftMethodId doesn't match any known object.");

			// remove any references to it from the vehicles.
            foreach (var v in _db.DB_Vehicles.Where(q => q.TheftMethodID == theftMethod.ID))
                v.TheftMethodID = null;

            _db.DB_TheftMethods.DeleteOnSubmit(theftMethod);
            _db.SubmitChanges();
		}
		#endregion

		#region vehicle-security-types
		public IEnumerable<DB_VehicleSecurityType> GetVehicleSecurityTypes()
		{
			return _db.DB_VehicleSecurityTypes.OrderBy(q=>q.Name).AsEnumerable();
		}

		public DB_VehicleSecurityType GetVehicleSecurityType(int id)
		{
			return _db.DB_VehicleSecurityTypes.FirstOrDefault(q => q.ID == id);
		}

		public void UpdateVehicleSecurityType(DB_VehicleSecurityType vehicleSecurityType)
		{
			if (vehicleSecurityType == null)
				throw new ArgumentNullException(nameof(vehicleSecurityType));

            if (vehicleSecurityType.ID < 1)
                _db.DB_VehicleSecurityTypes.InsertOnSubmit(vehicleSecurityType);

            _db.SubmitChanges();
		}

		public void DeleteVehicleSecurityType(int vehicleSecurityTypeId)
		{
		    var vehicleSecurityType = _db.DB_VehicleSecurityTypes.SingleOrDefault(q => q.ID == vehicleSecurityTypeId);
			if (vehicleSecurityType == null)
                throw new ArgumentException("vehicleSecurityTypeId doesn't match any known object.");

            _db.DB_VehicleSecurities.DeleteAllOnSubmit(_db.DB_VehicleSecurities.Where(q => q.DB_VehicleSecurityType.ID == vehicleSecurityType.ID));
            _db.DB_VehicleSecurityTypes.DeleteOnSubmit(vehicleSecurityType);
            _db.SubmitChanges();
		}
		#endregion

		#region colours
		public IEnumerable<DB_Colour> GetColours()
		{
			return _db.DB_Colours.OrderBy(q=>q.Name).AsEnumerable();
		}

		public DB_Colour GetColour(int id)
		{
			return _db.DB_Colours.FirstOrDefault(q => q.ID == id);
		}

		public void UpdateColour(DB_Colour colour)
		{
			if (colour == null)
				throw new ArgumentNullException(nameof(colour));

            if (colour.ID < 1)
                _db.DB_Colours.InsertOnSubmit(colour);

            _db.SubmitChanges();
		}

		public void DeleteColour(int colourId)
		{
			// set all vehicles using this colour to have no colour.
			// remove the colour.
			// -- might hit a performance ceiling with a large update-set, so could be a good candidate
			// -- for refactoring to an ef-sproc call.

		    var colour = _db.DB_Colours.SingleOrDefault(q => q.ID == colourId);
            if (colour == null)
                throw new ArgumentException("colourId doesn't match any known object.");

			foreach (var v in colour.DB_Vehicles)
				v.ColourID = null;

            _db.DB_Colours.DeleteOnSubmit(colour);
            _db.SubmitChanges();
		}
		#endregion

        #region places
        public DB_Location GetLocation(int id)
        {
            return _db.DB_Locations.FirstOrDefault(q => q.ID == id);
        }

        public Hashtable GetLocationSimpleStats(int id)
        {
            var stats = new Hashtable();
            foreach (var vt in Controller.Instance.MakesAndModelsController.VehicleTypes)
                stats.Add(vt.Name + "s", _db.DB_VehicleTheftLocations.Count(q => q.LocationID == id && q.DB_Vehicle.VehicleTypeId == vt.Id));

            stats.Add("Total Vehicles", _db.DB_VehicleTheftLocations.Count(q => q.LocationID == id));

            return stats;
        }

        /// <summary>
        /// Attempts to find the ID of a Location.
        /// </summary>
        /// <returns>0 if none found, otherwise the location id is returned.</returns>
	    public int FindLocation(byte type, string name)
	    {
	        var location = _db.DB_Locations.FirstOrDefault(q => q.Name == name && q.Type == type);
	        return location?.ID ?? 0;
	    }

        /// <summary>
        /// Creates a new Location record in the database.
        /// </summary>
        public void CreateLocation(DB_Location location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            // don't create duplicates.
            if (location.ID > 0)
                return;

            _db.DB_Locations.InsertOnSubmit(location);
            _db.SubmitChanges();
        }
        #endregion

        #region statistics
        public Dictionary<string, int> GetCountryStatistic(int id)
        {
            var result = _db.GetCountryStatistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }

        public Dictionary<string, int> GetArea1Statistic(int id)
        {
            var result = _db.GetArea1Statistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }

        public Dictionary<string, int> GetArea2Statistic(int id)
        {
            var result = _db.GetArea2Statistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }

        public Dictionary<string, int> GetLocalityStatistic(int id)
        {
            var result = _db.GetLocalityStatistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }

        public Dictionary<string, int> GetSubLocalityStatistic(int id)
        {
            var result = _db.GetSubLocalityStatistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }

        public Dictionary<string, int> GetRouteStatistic(int id)
        {
            var result = _db.GetRouteStatistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }

        public Dictionary<string, int> GetManufacturerStatistic(int id)
        {
            var result = _db.GetManufacturerStatistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }

        public Dictionary<string, int> GetModelStatistic(int id)
        {
            var result = _db.GetModelStatistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }

        public Dictionary<string, int> GetVehicleSecurityTypeStatistic(int id)
        {
            var result = _db.GetVehicleSecurityTypeStatistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }

        public Dictionary<string, int> GetTheftMethodStatistic(int id)
        {
            var result = _db.GetTheftMethodStatistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }

        public Dictionary<string, int> GetColourStatistic(int id)
        {
            var result = _db.GetColourStatistic(id).FirstOrDefault();
            if (result == null)
                return null;

            var stat = new Dictionary<string, int>();
            if (result.Instances != null) stat["Instances"] = result.Instances.Value;
            if (result.Rank != null) stat["Rank"] = Convert.ToInt32(result.Rank.Value);
            return stat;
        }
        #endregion
    }
}