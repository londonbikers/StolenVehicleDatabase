using SVD.Models;
using Webdiyer.WebControls.Mvc;

namespace SVDWebsite.Areas.Admin.Models
{
    public class LandingModel
    {
        public PagedList<Vehicle> Vehicles { get; set; }
    }
}