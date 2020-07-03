using System;
using System.Collections;
using System.Collections.Generic;

namespace SVD
{
	public interface IRepository
	{
		#region vehicles
		DB_Vehicle GetVehicle(int id);
	    IEnumerable<int> GetVehiclesForUser(Guid userId);
        IEnumerable<int> GetVehiclesForType(int typeId, int pageNumber, int pageSize, out int totalCount, byte[] vehicleStatus);
	    IEnumerable<int> GetVehiclesForModel(int modelId, int pageNumber, int pageSize, out int totalCount, byte[] vehicleStatus, int? year = null);
        IEnumerable<int> GetVehiclesForManufacturer(int manufacturerId, int pageNumber, int pageSize, out int totalCount, byte[] vehicleStatus);
		IEnumerable<int> GetLatestVehicles(int maxVehicles);
		void UpdateVehicle(DB_Vehicle vehicle);
		void DeleteVehicle(int id);
	    void CreateVehicleLocationPlace(int vehicleId, int placeId);
	    IEnumerable<DB_VehicleTheftLocation> GetVehicleTheftLocationPlaces(int vehicleId);
		IEnumerable<DB_VehiclePhoto> GetVehiclePhotos(int vehicleId);
		IEnumerable<DB_VehicleVideo> GetVehicleVideos(int vehicleId);
		IEnumerable<DB_VehicleSecurity> GetVehicleSecurities(int vehicleId);
		DB_VehiclePhoto GetVehiclePhoto(int photoId);
		DB_VehicleVideo GetVehicleVideo(int videoId);
		DB_VehicleSecurity GetVehicleSecurity(int vehicleId, int typeId);
		void UpdateVehiclePhoto(DB_VehiclePhoto vehiclePhoto);
		void UpdateVehicleVideo(DB_VehicleVideo vehicleVideo);
		void UpdateVehicleSecurity(DB_VehicleSecurity vehicleSecurity);
        void DeleteVehiclePhoto(int vehiclePhotoId);
        void DeleteVehicleVideo(int vehicleVideoId);
		void DeleteVehicleSecurity(int vehicleSecurityId);
        void DeleteVehicleTheftLocationPlace(int vehicleId, int locationId);
		#endregion

		#region colours
		IEnumerable<DB_Colour> GetColours();
		DB_Colour GetColour(int id);
		void UpdateColour(DB_Colour colour);
	    void DeleteColour(int colourId);
		#endregion

		#region theft-methods
		IEnumerable<DB_TheftMethod> GetTheftMethods();
		DB_TheftMethod GetTheftMethod(int id);
		void UpdateTheftMethod(DB_TheftMethod theftMethod);
	    void DeleteTheftMethod(int theftMethodId);
		#endregion

        #region vehicle-security-types
        IEnumerable<DB_VehicleSecurityType> GetVehicleSecurityTypes();
		DB_VehicleSecurityType GetVehicleSecurityType(int id);
		void UpdateVehicleSecurityType(DB_VehicleSecurityType vehicleSecurityType);
	    void DeleteVehicleSecurityType(int vehicleSecurityTypeId);
		#endregion

        #region locations
	    DB_Location GetLocation(int id);
	    int FindLocation(byte type, string name);
	    void CreateLocation(DB_Location location);
	    Hashtable GetLocationSimpleStats(int id);
	    #endregion

        #region statistics
        Dictionary<string, int> GetCountryStatistic(int id);
        Dictionary<string, int> GetArea1Statistic(int id);
        Dictionary<string, int> GetArea2Statistic(int id);
        Dictionary<string, int> GetLocalityStatistic(int id);
        Dictionary<string, int> GetSubLocalityStatistic(int id);
        Dictionary<string, int> GetRouteStatistic(int id);
	    Dictionary<string, int> GetManufacturerStatistic(int id);
	    Dictionary<string, int> GetModelStatistic(int id);
	    Dictionary<string, int> GetVehicleSecurityTypeStatistic(int id);
	    Dictionary<string, int> GetTheftMethodStatistic(int id);
	    Dictionary<string, int> GetColourStatistic(int id);
	    #endregion
	}
}