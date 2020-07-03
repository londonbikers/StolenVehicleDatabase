using System.Collections.Generic;
using System.Text;
using System.Linq;
using SVD.Models;
using Webdiyer.WebControls.Mvc;

namespace SVDWebsite.Models
{
    public class PlaceModel
    {
        public Place Place { get; set; }
        public List<Place> ParentPlaces { get; set; }
        public PagedList<Vehicle> Vehicles { get; set; }

        // build a list of vehicle coordinates for js use.
        public string GetVehicleLocations()
        {
            var js = new StringBuilder();
            var vehicles = Vehicles.Where(q => q.TheftLongitude.HasValue && q.TheftLatitude.HasValue).ToList();
            var count = vehicles.Count;
            for (var i = 0; i < count; i++)
            {
                js.AppendFormat("['{0}','{1}', '{2}']", vehicles[i].TheftLatitude.Value, vehicles[i].TheftLongitude.Value, vehicles[i].Title);
                if (i < count - 1)
                    js.Append(",");

            }
            return js.ToString();
        }
    }
}