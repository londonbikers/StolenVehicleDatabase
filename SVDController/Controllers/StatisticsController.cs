using System;
using System.Collections.Generic;
using SVD.Models;

namespace SVD.Controllers
{
    public class StatisticsController
    {
        #region constructors
        internal StatisticsController()
        {
        }
        #endregion

        #region public methods
        /// <summary>
        /// Builds a statistic object for an instance of a domain object.
        /// </summary>
        /// <remarks>
        /// This method should be broken out into separate methods when the complexity of stats increases.
        /// </remarks>
        internal Statistic BuildStatistic(BaseModel model)
        {
            Dictionary<string, int> data = null;
            if (model.DerivedType == typeof(Place))
            {
                var place = model as Place;
                if (place != null)
                    switch (place.Type)
                    {
                        case PlaceType.Country:
                            data = Controller.Instance.Repository.GetCountryStatistic(model.Id);
                            break; 
                        case PlaceType.AdministrativeAreaLevel1:
                            data = Controller.Instance.Repository.GetArea1Statistic(model.Id);
                            break;
                        case PlaceType.AdministrativeAreaLevel2:
                            data = Controller.Instance.Repository.GetArea2Statistic(model.Id);
                            break;
                        case PlaceType.Locality:
                            data = Controller.Instance.Repository.GetLocalityStatistic(model.Id);
                            break;
                        case PlaceType.SubLocality:
                            data = Controller.Instance.Repository.GetSubLocalityStatistic(model.Id);
                            break;
                        case PlaceType.Route:
                            data = Controller.Instance.Repository.GetRouteStatistic(model.Id);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
            }
            else if (model.DerivedType == typeof(Colour))
            {
                data = Controller.Instance.Repository.GetColourStatistic(model.Id);
            }
            else if (model.DerivedType == typeof(TheftMethod))
            {
                data = Controller.Instance.Repository.GetTheftMethodStatistic(model.Id);
            }
            else if (model.DerivedType == typeof(VehicleSecurityType))
            {
                data = Controller.Instance.Repository.GetVehicleSecurityTypeStatistic(model.Id);
            }
            else if (model.DerivedType == typeof(VehicleManufacturerWrapper))
            {
                var manufacturer = ((VehicleManufacturerWrapper) model).Manufacturer;
                data = Controller.Instance.Repository.GetManufacturerStatistic(manufacturer.Id);
            }
            else if (model.DerivedType == typeof(VehicleModelWrapper))
            {
                var vm = ((VehicleModelWrapper)model).Model;
                data = Controller.Instance.Repository.GetModelStatistic(vm.Id);
            }

            if (data == null)
                return null;

            var stat = new Statistic {Rank = data["Rank"], Instances = data["Instances"]};
            return stat;
        }
        #endregion
    }
}
