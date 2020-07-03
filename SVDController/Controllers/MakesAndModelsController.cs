using System;
using System.Collections.Generic;
using System.Linq;
using SVD.Models;
using SVD.VDWS;

namespace SVD.Controllers
{
    public class MakesAndModelsController
    {
        #region accessors
        public List<VehicleType> VehicleTypes { get; private set; }
        public List<VehicleManufacturerWrapper> VehicleManufacturers { get; private set; }
        public List<VehicleModelWrapper> VehicleModels { get; private set; }
        #endregion

        #region constructors
        internal MakesAndModelsController()
        {
        }
        #endregion

        #region public methods
        public VehicleModelWrapper GetModel(int id)
        {
            return VehicleModels.SingleOrDefault(q => q.Model.Id == id);
        }

        public VehicleManufacturerWrapper GetManufacturer(int id)
        {
            return VehicleManufacturers.SingleOrDefault(q => q.Manufacturer.Id == id);
        }

        public VehicleType GetVehicleType(int id)
        {
            return VehicleTypes.SingleOrDefault(q => q.Id == id);
        }
        #endregion

        #region internal methods
        internal void RetrieveHierarchy()
        {
            using (var vdws = new VdServiceClient())
            {
                var typesResponse = vdws.GetVehicleTypes();
                if (typesResponse.ResultType != GetOperationResultType.Success)
                    throw new Exception("Unsuccessful response from VDWS on GetVehicleTypes! - " + typesResponse.ErrorMessage);
                    VehicleTypes = typesResponse.Content;

                var manufacturersResponse = vdws.GetVehicleManufacturers();
                if (manufacturersResponse.ResultType != GetOperationResultType.Success)
                    throw new Exception("Unsuccessful response from VDWS on GetVehicleManufacturers! - " + manufacturersResponse.ErrorMessage);

                VehicleManufacturers = new List<VehicleManufacturerWrapper>();
                foreach (var vm in manufacturersResponse.Content)
                    VehicleManufacturers.Add(new VehicleManufacturerWrapper(ObjectMode.New) { Manufacturer = vm });

                VehicleModels = new List<VehicleModelWrapper>();
                lock (VehicleModels)
                {
                    foreach (var modelsResponse in VehicleManufacturers.Select(manufacturer => vdws.GetVehicleModelsByManufacturerId(manufacturer.Manufacturer.Id)))
                    {
                        if (modelsResponse.ResultType != GetOperationResultType.Success)
                            throw new Exception("Unsuccessful response from VDWS on GetVehicleModelsByManufacturerId! - " + modelsResponse.ErrorMessage);

                        foreach (var vm in modelsResponse.Content)
                            VehicleModels.Add(new VehicleModelWrapper(ObjectMode.New) {Model = vm});
                    }
                }
            }
        }
        #endregion
    }
}