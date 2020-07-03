using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SVD.Models;
using SVD.VDWS;

namespace SVD.Controllers
{
	public class VehicleController
	{
		#region constructors
		internal VehicleController()
		{
		}
		#endregion

		#region colours
		public List<Colour> GetColours()
		{
			var colours = Controller.Instance.CacheManager.Get("Colours") as List<Colour>;
		    if (colours != null)
                return colours;

		    colours = Controller.Instance.Repository.GetColours().Select(dbColour => new Colour(ObjectMode.Existing) {Id = dbColour.ID, Name = dbColour.Name}).ToList();
		    Controller.Instance.CacheManager.Add("Colours", colours);

		    return colours;
		}

		public Colour GetColour(int id)
		{
			return GetColours().SingleOrDefault(q => q.Id == id);
		}

		public Colour NewColour()
		{
			return new Colour(ObjectMode.New);
		}

		public void UpdateColour(Colour colour)
		{
			if (colour == null)
				throw new ArgumentNullException(nameof(colour));

			if (!colour.IsValid())
				throw new ArgumentException("colour is invalid!");

            lock (colour)
            {

                var isNew = false;
                DB_Colour dbColour;
                if (colour.IsPersisted)
                {
                    dbColour = Controller.Instance.Repository.GetColour(colour.Id);
                }
                else
                {
                    dbColour = new DB_Colour();
                    isNew = true;
                }

                dbColour.Name = colour.Name;
                Controller.Instance.Repository.UpdateColour(dbColour);

                if (isNew)
                {
                    colour.Id = dbColour.ID;
                    colour.IsPersisted = true;
                    Controller.Instance.CacheManager.Remove("Colours");
                }
            }
		}

		public void DeleteColour(Colour colour)
		{
			if (colour == null)
				throw new ArgumentNullException(nameof(colour));

			if (!colour.IsPersisted)
				throw new ArgumentException("colour is not persisted, so cannot delete!");

            lock (colour)
            {
                Controller.Instance.Repository.DeleteColour(colour.Id);

                // colours are cached in a single collection, so just drop the whole lot from the cache.
                Controller.Instance.CacheManager.Remove("Colours");
            }
		}
		#endregion

		#region theft-methods
		public List<TheftMethod> GetTheftMethods()
		{
			var theftMethods = Controller.Instance.CacheManager.Get("TheftMethods") as List<TheftMethod>;
		    if (theftMethods != null)
                return theftMethods;

		    theftMethods = Controller.Instance.Repository.GetTheftMethods().Select(dbTheftMethod => new TheftMethod(ObjectMode.Existing) {Id = dbTheftMethod.ID, Name = dbTheftMethod.Name}).ToList();
		    Controller.Instance.CacheManager.Add("TheftMethods", theftMethods);

		    return theftMethods;
		}

		public TheftMethod GetTheftMethod(int id)
		{
			return GetTheftMethods().SingleOrDefault(q => q.Id == id);
		}

		public TheftMethod NewTheftMethod()
		{
			return new TheftMethod(ObjectMode.New);
		}

		public void UpdateTheftMethod(TheftMethod theftMethod)
		{
			if (theftMethod == null)
				throw new ArgumentNullException(nameof(theftMethod));

			if (!theftMethod.IsValid())
				throw new ArgumentException("theftMethod is invalid!");

            lock (theftMethod)
            {
                var isNew = false;
                DB_TheftMethod dbTheftMethod;
                if (theftMethod.IsPersisted)
                {
                    dbTheftMethod = Controller.Instance.Repository.GetTheftMethod(theftMethod.Id);
                }
                else
                {
                    dbTheftMethod = new DB_TheftMethod();
                    isNew = true;
                }

                dbTheftMethod.Name = theftMethod.Name;
                Controller.Instance.Repository.UpdateTheftMethod(dbTheftMethod);

                if (!isNew) return;
                theftMethod.Id = dbTheftMethod.ID;
                theftMethod.IsPersisted = true;
                Controller.Instance.CacheManager.Remove("TheftMethods");
            }
		}

		public void DeleteTheftMethod(TheftMethod theftMethod)
		{
			if (theftMethod == null)
				throw new ArgumentNullException(nameof(theftMethod));

			if (!theftMethod.IsPersisted)
				throw new ArgumentException("theftMethod is not persisted, so cannot delete!");

            lock (theftMethod)
            {
                Controller.Instance.Repository.DeleteTheftMethod(theftMethod.Id);

                // theft-methods are cached in a single collection, so just drop the whole lot from the cache.
                Controller.Instance.CacheManager.Remove("TheftMethods");
            }
		}
		#endregion

		#region vehicle-security-types
		public List<VehicleSecurityType> GetVehicleSecurityTypes()
		{
			var types = Controller.Instance.CacheManager.Get("VehicleSecurityTypes") as List<VehicleSecurityType>;
		    if (types != null)
                return types;

		    types = Controller.Instance.Repository.GetVehicleSecurityTypes().Select(dbType => new VehicleSecurityType(ObjectMode.Existing) {Id = dbType.ID, Name = dbType.Name}).ToList();
		    Controller.Instance.CacheManager.Add("VehicleSecurityTypes", types);

		    return types;
		}

		public VehicleSecurityType GetVehicleSecurityType(int id)
		{
			return GetVehicleSecurityTypes().SingleOrDefault(q => q.Id == id);
		}

		public VehicleSecurityType NewVehicleSecurityType()
		{
			return new VehicleSecurityType(ObjectMode.New);
		}

		public void UpdateVehicleSecurityType(VehicleSecurityType vehicleSecurityType)
		{
			if (vehicleSecurityType == null)
				throw new ArgumentNullException(nameof(vehicleSecurityType));

			if (!vehicleSecurityType.IsValid())
				throw new ArgumentException("vehicleSecurityType is invalid!");

            lock (vehicleSecurityType)
            {
                var isNew = false;
                DB_VehicleSecurityType dbVst;
                if (vehicleSecurityType.IsPersisted)
                {
                    dbVst = Controller.Instance.Repository.GetVehicleSecurityType(vehicleSecurityType.Id);
                }
                else
                {
                    dbVst = new DB_VehicleSecurityType();
                    isNew = true;
                }

                dbVst.Name = vehicleSecurityType.Name;
                Controller.Instance.Repository.UpdateVehicleSecurityType(dbVst);

                if (isNew)
                {
                    vehicleSecurityType.Id = dbVst.ID;
                    vehicleSecurityType.IsPersisted = true;
                    Controller.Instance.CacheManager.Remove("VehicleSecurityTypes");
                }
            }
		}

		public void DeleteVehicleSecurityType(VehicleSecurityType vehicleSecurityType)
		{
			if (vehicleSecurityType == null)
				throw new ArgumentNullException(nameof(vehicleSecurityType));

			if (!vehicleSecurityType.IsPersisted)
				throw new ArgumentException("vehicleSecurityType is not persisted, so cannot delete!");

            lock (vehicleSecurityType)
            {
                Controller.Instance.Repository.DeleteVehicleSecurityType(vehicleSecurityType.Id);

                // vehicle-security-types are cached in a single collection, so just drop the whole lot from the cache.
                Controller.Instance.CacheManager.Remove("VehicleSecurityTypes");
            }
		}
		#endregion

		#region vehicles
		public Vehicle NewVehicle()
		{
			return new Vehicle(ObjectMode.New);
		}

        /// <summary>
        /// Retrieves a vehicle object from the repository.
        /// </summary>
        /// <param name="id">The identifier for the vehicle to retrieve.</param>
		public Vehicle GetVehicle(int id)
		{
			var vehicle = Controller.Instance.CacheManager.Get(Vehicle.GetCacheKey(id)) as Vehicle;
		    if (vehicle != null)
                return vehicle;

		    var dbVehicle = Controller.Instance.Repository.GetVehicle(id);
		    if (dbVehicle == null)
		        return null;

		    vehicle = new Vehicle(ObjectMode.Existing)
		    {
		        Id = dbVehicle.ID,
		        DateCreated = dbVehicle.DateCreated,
		        DateModified = dbVehicle.DateModified,
		        Description = dbVehicle.Description,
		        MemberUid = dbVehicle.MemberUID,
		        PoliceForce = dbVehicle.PoliceForce,
		        PolicePhoneNumber = dbVehicle.PolicePhoneNo,
		        PoliceReference = dbVehicle.PoliceReference,
		        Registration = dbVehicle.Registration,
		        Vin = dbVehicle.VIN,
		        EngineNumber = dbVehicle.EngineNumber,
		        Status = (VehicleStatus)dbVehicle.Status,
		        TheftDescription = dbVehicle.TheftDescription
		    };

		    var modelWrapper = Controller.Instance.MakesAndModelsController.GetModel(dbVehicle.ModelID);
		    vehicle.Model = modelWrapper.Model;

		    if (dbVehicle.ColourID.HasValue)
		        vehicle.Colour = GetColour(dbVehicle.ColourID.Value);

		    if (dbVehicle.TheftDate.HasValue)
		        vehicle.TheftDate = dbVehicle.TheftDate.Value;

		    if (dbVehicle.TheftLatitude.HasValue)
		        vehicle.TheftLatitude = dbVehicle.TheftLatitude.Value;

		    if (dbVehicle.TheftLongitude.HasValue)
		        vehicle.TheftLongitude = dbVehicle.TheftLongitude.Value;

		    if (dbVehicle.TheftMethodID.HasValue)
		        vehicle.TheftMethod = GetTheftMethod(dbVehicle.TheftMethodID.Value);
				
		    if (dbVehicle.IsLocationApprox.HasValue)
		        vehicle.IsLocationApproximate = dbVehicle.IsLocationApprox.Value;

		    if (dbVehicle.Year.HasValue)
		        vehicle.Year = dbVehicle.Year.Value;

		    foreach (var vl in dbVehicle.DB_VehicleTheftLocations.OrderBy(q=>q.DB_Location.Type))
		        vehicle.TheftLocationPlaces.Add(Controller.Instance.PlacesController.GetPlace(vl.LocationID));

		    Controller.Instance.CacheManager.Add(vehicle.CacheKey, vehicle);
		    return vehicle;
		}

        public List<Vehicle> GetVehiclesForUser(Guid userId)
        {
            var ids = Controller.Instance.Repository.GetVehiclesForUser(userId);
            return ids.Select(GetVehicle).ToList();
        }

        public LightVehicleCollection GetVehicles(int pageSize = 50, int pageNumber = 1)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 1;

            int totalCount;
            var source = Controller.Instance.Repository.GetVehicles(pageNumber, pageSize, out totalCount);
            var collection = new LightVehicleCollection { TotalCount = totalCount };
            foreach (var id in source)
                collection.Add(id);

            return collection;
        }

        public LightVehicleCollection GetVehiclesForType(VehicleType type, VehicleStatus[] status, int pageSize = 50, int pageNumber = 1)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 1;

            int totalCount;
            var statusValues = status.Select(s => (byte) s).ToArray();
            var source = Controller.Instance.Repository.GetVehiclesForType(type.Id, pageNumber, pageSize, out totalCount, statusValues);
            var collection = new LightVehicleCollection {TotalCount = totalCount};
            foreach (var id in source)
                collection.Add(id);

            return collection;
        }

        public LightVehicleCollection GetVehiclesForModel(VehicleModel model, VehicleStatus[] status, int pageSize = 50, int pageNumber = 1, int? year = null)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 1;

            int totalCount;
            var statusValues = status.Select(s => (byte)s).ToArray();
            var source = Controller.Instance.Repository.GetVehiclesForModel(model.Id, pageNumber, pageSize, out totalCount, statusValues, year);
            var collection = new LightVehicleCollection { TotalCount = totalCount };

            foreach (var id in source)
                collection.Add(id);

            return collection;
        }

        public LightVehicleCollection GetVehiclesForManufacturer(VehicleManufacturer manufacturer, VehicleStatus[] status, int pageSize = 50, int pageNumber = 1)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 1;

            int totalCount;
            var statusValues = status.Select(s => (byte)s).ToArray();
            var source = Controller.Instance.Repository.GetVehiclesForManufacturer(manufacturer.Id, pageNumber, pageSize, out totalCount, statusValues);
            var collection = new LightVehicleCollection { TotalCount = totalCount };

            foreach (var id in source)
                collection.Add(id);

            return collection;
        }

        public LightVehicleCollection GetVehiclesForPlace(Place place, VehicleStatus[] status, int pageSize = 50, int pageNumber = 1)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 1;

            int totalCount;
            var statusValues = status.Select(s => (byte)s).ToArray();
            var source = Controller.Instance.Repository.GetVehiclesForPlace(place.Id, pageNumber, pageSize, out totalCount, statusValues);
            var collection = new LightVehicleCollection { TotalCount = totalCount };
            foreach (var id in source)
                collection.Add(id);

            return collection;
        }

		public void UpdateVehicle(Vehicle vehicle)
		{
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            if (!vehicle.IsValid())
                throw new ArgumentException("vehicle is invalid!");

            lock (vehicle)
            {
                var isNew = false;
                DB_Vehicle dbVehicle;
                if (vehicle.IsPersisted)
                {
                    dbVehicle = Controller.Instance.Repository.GetVehicle(vehicle.Id);
                }
                else
                {
                    dbVehicle = new DB_Vehicle();
                    isNew = true;
                }

                #region basic properties
                dbVehicle.DateCreated = vehicle.DateCreated;
                dbVehicle.DateModified = DateTime.Now;
                dbVehicle.Description = (!string.IsNullOrEmpty(vehicle.Description)) ? vehicle.Description : null;
                dbVehicle.MemberUID = vehicle.MemberUid;
                dbVehicle.ModelID = vehicle.Model.Id;
                dbVehicle.ManufacturerID = vehicle.Model.ManufacturerId;
                dbVehicle.VehicleTypeId = vehicle.Model.Type.Id;
                dbVehicle.PoliceForce = (!string.IsNullOrEmpty(vehicle.PoliceForce)) ? vehicle.PoliceForce : null;
                dbVehicle.PolicePhoneNo = (!string.IsNullOrEmpty(vehicle.PolicePhoneNumber)) ? vehicle.PolicePhoneNumber : null;
                dbVehicle.PoliceReference = (!string.IsNullOrEmpty(vehicle.PoliceReference)) ? vehicle.PoliceReference : null;
                dbVehicle.Registration = (!string.IsNullOrEmpty(vehicle.Registration)) ? vehicle.Registration : null;
                dbVehicle.TheftDescription = (!string.IsNullOrEmpty(vehicle.TheftDescription)) ? vehicle.TheftDescription : null;
                dbVehicle.VIN = (!string.IsNullOrEmpty(vehicle.Vin)) ? vehicle.Vin : null;
                dbVehicle.EngineNumber = (!string.IsNullOrEmpty(vehicle.EngineNumber)) ? vehicle.EngineNumber : null;
                dbVehicle.Status = (byte) vehicle.Status;

                if (vehicle.Colour != null)
                    dbVehicle.ColourID = vehicle.Colour.Id;
                else
                    dbVehicle.ColourID = null;

                if (vehicle.TheftDate.HasValue)
                    dbVehicle.TheftDate = vehicle.TheftDate.Value;
                else
                    dbVehicle.TheftDate = null;

                if (vehicle.TheftLatitude.HasValue)
                    dbVehicle.TheftLatitude = vehicle.TheftLatitude.Value;
                else
                    dbVehicle.TheftLatitude = null;

                if (vehicle.TheftLongitude.HasValue)
                    dbVehicle.TheftLongitude = vehicle.TheftLongitude.Value;
                else
                    dbVehicle.TheftLongitude = null;

                if (vehicle.TheftLatitude.HasValue && vehicle.TheftLongitude.HasValue)
                    dbVehicle.IsLocationApprox = vehicle.IsLocationApproximate;
                else
                    dbVehicle.IsLocationApprox = null;

                if (vehicle.TheftMethod != null)
                    dbVehicle.TheftMethodID = vehicle.TheftMethod.Id;
                else
                    dbVehicle.TheftMethodID = null;

                if (vehicle.Year.HasValue)
                    dbVehicle.Year = vehicle.Year.Value;
                else
                    dbVehicle.Year = null;
                #endregion

                Controller.Instance.Repository.UpdateVehicle(dbVehicle);
                if (isNew)
                {
                    vehicle.Id = dbVehicle.ID;
                    vehicle.IsPersisted = true;
                    Controller.Instance.CacheManager.Add(vehicle.CacheKey, vehicle);
                }

                #region locations
                for (var i = 0; i < vehicle.TheftLocationPlaces.Count; i++)
                    vehicle.TheftLocationPlaces[i] = Controller.Instance.PlacesController.UpdatePlace(vehicle.TheftLocationPlaces[i]);

                var dbPlaces = Controller.Instance.Repository.GetVehicleTheftLocationPlaces(vehicle.Id);

                // look for deletions by comparing against the current db state.
                var dbVehicleTheftLocations = dbPlaces as DB_VehicleTheftLocation[] ?? dbPlaces.ToArray();
                foreach (var originalPlace in dbVehicleTheftLocations.Where(originalPlace => !vehicle.TheftLocationPlaces.Exists(q => q.Id == originalPlace.LocationID)))
                    Controller.Instance.Repository.DeleteVehicleTheftLocationPlace(originalPlace.VehicleID, originalPlace.LocationID);

                // persist new relationships.
                foreach (var place in vehicle.TheftLocationPlaces.Where(place => dbVehicleTheftLocations.Count(q => q.LocationID == place.Id) <= 0))
                    Controller.Instance.Repository.CreateVehicleLocationPlace(vehicle.Id, place.Id);
                #endregion

                #region photos
                lock (vehicle.Photos)
                {
                    // look for deletions by comparing against the current db state.
                    foreach (var originalDbPhoto in Controller.Instance.Repository.GetVehiclePhotos(vehicle.Id).Where(originalDbPhoto => !vehicle.Photos.Exists(q => q.Id == originalDbPhoto.ID)))
                        Controller.Instance.Repository.DeleteVehiclePhoto(originalDbPhoto.ID);

                    foreach (var p in vehicle.Photos)
                        UpdateVehiclePhoto(p);
                }
                #endregion

                #region videos
                // look for deletions by comparing against the current db state.
                foreach (var originalDbVideo in Controller.Instance.Repository.GetVehicleVideos(vehicle.Id).Where(originalDbVideo => !vehicle.Videos.Exists(q => q.Id == originalDbVideo.ID)))
                    Controller.Instance.Repository.DeleteVehicleVideo(originalDbVideo.ID);

                foreach (var v in vehicle.Videos)
                    UpdateVehicleVideo(v);
                #endregion

                #region security
                // look for deletions by comparing against the current db state.
                foreach (var originalDbVehicleSecurity in Controller.Instance.Repository.GetVehicleSecurities(vehicle.Id).Where(
                    originalDbVehicleSecurity => !vehicle.SecurityTypes.Exists(q => q.Id == originalDbVehicleSecurity.DB_VehicleSecurityType.ID)))
                    Controller.Instance.Repository.DeleteVehicleSecurity(originalDbVehicleSecurity.ID);

                foreach (var vst in vehicle.SecurityTypes)
                    UpdateVehicleSecurity(vehicle, vst);
                #endregion
            }
		}

		public void DeleteVehicle(Vehicle vehicle)
		{
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            if (!vehicle.IsPersisted)
                throw new ArgumentException("vehicle is not persisted, cannot delete!");
            
            try
            {
                // delete any image files.
                Parallel.ForEach(vehicle.Photos, q => File.Delete(q.GetFilePath()));
            }
            catch (Exception ex)
            {
                Controller.Instance.Log(ex);
            }

		    Controller.Instance.Repository.DeleteVehicle(vehicle.Id);
		}

        public void DeleteVehiclePhoto(Photo photo)
        {
            if (photo == null)
                throw new ArgumentNullException(nameof(photo));

            if (!photo.IsPersisted)
                throw new ArgumentException("photo is not persisted, cannot delete!");

            var path = photo.GetFilePath();
            File.Delete(path);
            photo.Vehicle.Photos.RemoveAll(q => q.Id == photo.Id);
            Controller.Instance.Repository.DeleteVehiclePhoto(photo.Id);
        }

		private static void UpdateVehiclePhoto(Photo photo)
		{
			if (photo == null)
				throw new ArgumentNullException(nameof(photo));

			if (!photo.IsValid())
				throw new ArgumentException("photo is invalid!");

            lock (photo)
            {

                var isNew = false;
                DB_VehiclePhoto dbPhoto;
                if (photo.Id > 0)
                {
                    dbPhoto = Controller.Instance.Repository.GetVehiclePhoto(photo.Id);
                }
                else
                {
                    dbPhoto = new DB_VehiclePhoto();
                    isNew = true;
                }

                dbPhoto.Filename = photo.Filename;
                dbPhoto.Comment = (!string.IsNullOrEmpty(photo.Comment)) ? photo.Comment : null;
                dbPhoto.DateCreated = photo.DateCreated;
                dbPhoto.DateModified = DateTime.Now;
                dbPhoto.VehicleID = photo.Vehicle.Id;
                dbPhoto.PhotoType = (byte) photo.Type;

                Controller.Instance.Repository.UpdateVehiclePhoto(dbPhoto);

                if (isNew)
                {
                    photo.Id = dbPhoto.ID;
                    photo.IsPersisted = true;
                }
            }
		}

		private static void UpdateVehicleVideo(Video video)
		{
			if (video == null)
				throw new ArgumentNullException(nameof(video));

			if (!video.IsValid())
				throw new ArgumentException("video is invalid!");

            lock (video)
            {
                var isNew = false;
                DB_VehicleVideo dbVideo;
                if (video.Id > 0)
                {
                    dbVideo = Controller.Instance.Repository.GetVehicleVideo(video.Id);
                }
                else
                {
                    dbVideo = new DB_VehicleVideo();
                    isNew = true;
                }

                dbVideo.Url = video.Url;
                dbVideo.Comment = (!string.IsNullOrEmpty(video.Comment)) ? video.Comment : null;
                dbVideo.VehicleID = video.Vehicle.Id;

                Controller.Instance.Repository.UpdateVehicleVideo(dbVideo);

                if (!isNew) return;
                video.Id = dbVideo.ID;
                video.IsPersisted = true;
            }
		}

        public void DeleteVehicleVideo(Video video)
        {
            if (video == null)
                throw new ArgumentNullException(nameof(video));

            if (!video.IsPersisted)
                throw new ArgumentException("video is not persisted, cannot delete!");

            video.Vehicle.Videos.RemoveAll(q => q.Id == video.Id);
            Controller.Instance.Repository.DeleteVehicleVideo(video.Id);
        }

		private static void UpdateVehicleSecurity(BaseModel vehicle, VehicleSecurityType securityType)
		{
			if (vehicle == null)
				throw new ArgumentNullException(nameof(vehicle));

			if (securityType == null)
				throw new ArgumentNullException(nameof(securityType));

            lock (securityType)
            {
                var dbVehicleSecurity = Controller.Instance.Repository.GetVehicleSecurity(vehicle.Id, securityType.Id) ?? new DB_VehicleSecurity();
                dbVehicleSecurity.VehicleID = vehicle.Id;
                dbVehicleSecurity.DB_VehicleSecurityType = Controller.Instance.Repository.GetVehicleSecurityType(securityType.Id);
                Controller.Instance.Repository.UpdateVehicleSecurity(dbVehicleSecurity);
            }
		}
		#endregion
    }
}