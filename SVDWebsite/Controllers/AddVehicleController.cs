using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using System.Drawing;
using SVD;
using SVD.Models;
using SVDWebsite.Code;
using SVDWebsite.Models;
using Controller = System.Web.Mvc.Controller;

namespace SVDWebsite.Controllers
{
    public class AddVehicleController : Controller
    {
        // GET: /AddVehicle/
        public ActionResult Index()
        {
            return View(new AddVehicleModel());
        }

        // POST: /AddVehicle/
        [HttpPost]
        public ActionResult Index(AddVehicleModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = Membership.GetUser();
            if (user == null)
                RedirectToAction("logon", "account");

            // build a new Vehicle object.
            var v = SVD.Controller.Instance.VehicleController.NewVehicle();
            v.Description = model.Description;
            v.IsLocationApproximate = false;
            if (user != null && user.ProviderUserKey != null) v.MemberUid = (Guid)user.ProviderUserKey;
            v.Model = SVD.Controller.Instance.MakesAndModelsController.GetModel(int.Parse(model.VehicleModelId)).Model;

            if (!string.IsNullOrEmpty(model.ColourId))
                v.Colour = SVD.Controller.Instance.VehicleController.GetColour(int.Parse(model.ColourId));

            v.Status = (VehicleStatus)byte.Parse(model.StatusId);
            v.PoliceForce = model.PoliceForce;
            v.PolicePhoneNumber = model.PolicePhoneNumber;
            v.PoliceReference = model.PoliceReference;
            v.Registration = model.Registration;
            v.SecurityTypes = model.VehicleSecurityTypesChosen;
            v.TheftMethod = SVD.Controller.Instance.VehicleController.GetTheftMethod(int.Parse(model.TheftMethodId));
            v.Vin = model.Vin;
            v.EngineNumber = model.EngineNumber;
            v.Year = int.Parse(model.Year);
            v.TheftDescription = model.TheftDescription;

            DateTime date;
            if (DateTime.TryParse(model.TheftDate, out date))
                v.TheftDate = date;

            #region location
            if (!string.IsNullOrEmpty(model.TheftLocationLat) && !string.IsNullOrEmpty(model.TheftLocationLong))
            {
                v.TheftLatitude = decimal.Parse(model.TheftLocationLat);
                v.TheftLongitude = decimal.Parse(model.TheftLocationLong);
            }

            // create a place object for each location type we receive from Google Maps.
            if (!string.IsNullOrEmpty(model.TheftLocationCountry))
            {
                var p = new Place(ObjectMode.New)
                {
                    Type = PlaceType.Country,
                    Code = model.TheftLocationCountryCode,
                    Name = model.TheftLocationCountry
                };

                if (!string.IsNullOrEmpty(model.TheftLocationCountryPos))
                {
                    var pos = model.TheftLocationCountryPos.Split(',');
                    p.Latitude = double.Parse(pos[0]);
                    p.Longitude = double.Parse(pos[1]);
                }

                v.TheftLocationPlaces.Add(p);
            }
            if (!string.IsNullOrEmpty(model.TheftLocationAdministrativeAreaLevel1))
            {
                var p = new Place(ObjectMode.New)
                {
                    Type = PlaceType.AdministrativeAreaLevel1,
                    Name = model.TheftLocationAdministrativeAreaLevel1
                };

                if (!string.IsNullOrEmpty(model.TheftLocationAdministrativeAreaLevel1Pos))
                {
                    var pos = model.TheftLocationAdministrativeAreaLevel1Pos.Split(',');
                    p.Latitude = double.Parse(pos[0]);
                    p.Longitude = double.Parse(pos[1]);
                }

                v.TheftLocationPlaces.Add(p);
            }
            if (!string.IsNullOrEmpty(model.TheftLocationAdministrativeAreaLevel2))
            {
                var p = new Place(ObjectMode.New)
                {
                    Type = PlaceType.AdministrativeAreaLevel2,
                    Name = model.TheftLocationAdministrativeAreaLevel2
                };

                if (!string.IsNullOrEmpty(model.TheftLocationAdministrativeAreaLevel2Pos))
                {
                    var pos = model.TheftLocationAdministrativeAreaLevel2Pos.Split(',');
                    p.Latitude = double.Parse(pos[0]);
                    p.Longitude = double.Parse(pos[1]);
                }

                v.TheftLocationPlaces.Add(p);
            }
            if (!string.IsNullOrEmpty(model.TheftLocationRoute))
            {
                var p = new Place(ObjectMode.New)
                {
                    Type = PlaceType.Route, 
                    Name = model.TheftLocationRoute
                };

                if (!string.IsNullOrEmpty(model.TheftLocationRoutePos))
                {
                    var pos = model.TheftLocationRoutePos.Split(',');
                    p.Latitude = double.Parse(pos[0]);
                    p.Longitude = double.Parse(pos[1]);
                }

                v.TheftLocationPlaces.Add(p);
            }
            if (!string.IsNullOrEmpty(model.TheftLocationSubLocality))
            {
                var p = new Place(ObjectMode.New)
                {
                    Type = PlaceType.SubLocality,
                    Name = model.TheftLocationSubLocality
                };

                if (!string.IsNullOrEmpty(model.TheftLocationSubLocalityPos))
                {
                    var pos = model.TheftLocationSubLocalityPos.Split(',');
                    p.Latitude = double.Parse(pos[0]);
                    p.Longitude = double.Parse(pos[1]);
                }

                v.TheftLocationPlaces.Add(p);
            }
            if (!string.IsNullOrEmpty(model.TheftLocationLocality))
            {
                var p = new Place(ObjectMode.New)
                {
                    Type = PlaceType.Locality,
                    Name = model.TheftLocationLocality
                };

                if (!string.IsNullOrEmpty(model.TheftLocationLocalityPos))
                {
                    var pos = model.TheftLocationLocalityPos.Split(',');
                    p.Latitude = double.Parse(pos[0]);
                    p.Longitude = double.Parse(pos[1]);
                }

                v.TheftLocationPlaces.Add(p);
            }

            Helpers.ResolvePlaceParents(v.TheftLocationPlaces);
            #endregion

            SVD.Controller.Instance.VehicleController.UpdateVehicle(v);
            return RedirectToAction("stagetwo", new { id = v.Id });
        }

        // GET: /AddVehicle/StageTwo/5
        public ActionResult StageTwo(int id)
        {
            var vehicle = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (vehicle != null)
            {
                var videos = new StringBuilder();
                if (vehicle.Videos.Count > 0)
                {
                    videos.Append("[");
                    foreach (var video in vehicle.Videos)
                        videos.AppendFormat("[\"{0}\",{1}],", video.Url, video.Id);
                    videos.Remove(videos.Length - 1, 1);
                    videos.Append("]");
                }
                else
                {
                    videos.Append("null");
                }

                var model = new StageTwo
                {
                    Id = id,
                    Type = vehicle.Model.Type.Name,
                    Title = vehicle.Title,
                    Videos = vehicle.Videos,
                    Photos = vehicle.Photos,
                    VideoArray = videos.ToString()
                };
                return View(model);
            }

            return RedirectToAction("Index", "home");
        }

        #region AJAX Actions
        // GET: /AddVehicle/GetMakes
        // ReSharper disable InconsistentNaming
        public ActionResult GetMakes(string VehicleTypeId)
        // ReSharper restore InconsistentNaming
        {
            var makesJson = SVD.Controller.Instance.MakesAndModelsController.VehicleManufacturers.Where(q => q.Manufacturer.Types.Any(q2 => q2.Id == int.Parse(VehicleTypeId))).Select(m => m.Manufacturer);
            return Json(makesJson);
        }

        // GET: /AddVehicle/GetModels
        // ReSharper disable InconsistentNaming 
        public ActionResult GetModels(string VehicleManufacturerId)
        // ReSharper restore InconsistentNaming
        {
            var models = SVD.Controller.Instance.MakesAndModelsController.VehicleModels.Where(q => q.Model.ManufacturerId == int.Parse(VehicleManufacturerId)).Select(q => q.Model);
            var modelsJson = models.Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), });
            return Json(modelsJson);
        }
        
		// POST: /AddVehicle/AddVideo/1
		[HttpPost]
		public ActionResult AddVideo(int id)
		{
			var v = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (v == null)
                return Json(new { success = false, message = "Whoops, no such vehicle found." });

            var url = Request.Form["url"];
            var video = v.AddVideo(url);
            SVD.Controller.Instance.VehicleController.UpdateVehicle(v);
		    return Json(new {success = true, message = "Video added!", videoid = video.Id});
		}
		
		// POST: /AddVehicle/UploadPhoto/1
        [HttpPost]
        public ActionResult UploadPhoto(int id)
        {
            var v = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (v == null)
                return Json(new { error = "Whoops, no such vehicle found." });

            var result = false;
            Image image = null;
            var filename = string.Empty;
            Photo photo = null;

            #region resolve image between upload methods
            if (Request.Files != null && Request.Files.Count > 0)
            {
                // *** old-fashioned internet-explorer file-upload. ***
                var file = Request.Files[0];
                if (file != null)
                {
                    filename = Path.GetFileName(file.FileName);
                    if (Helpers.IsFileUploadValid(file.InputStream, file.FileName))
                    {
                        image = Image.FromStream(file.InputStream);
                        result = true;
                    }
                }
            }
            else
            {
                // *** new-style raw upload. ***
                filename = Path.GetFileName(Request.Headers["X-File-Name"]);
                if (!string.IsNullOrEmpty(filename) && Helpers.IsFileUploadValid(Request.InputStream, filename))
                {
                    image = Image.FromStream(Request.InputStream);
                    result = true; 
                }
            }
            #endregion

            if (result)
            {
                // set a 1600px max image size.
                using (var highResImage = ImageHelpers.ResizeImage(image, 1600))
                {
                    var path = Helpers.SaveUploadedImage(highResImage, filename);
                    filename = Path.GetFileName(path);
                    photo = v.AddPhoto(filename, PhotoType.HighResolution);
                    SVD.Controller.Instance.VehicleController.UpdateVehicle(v);
                }
            }

            var photoUrl = Helpers.DynamicImageUrl(photo, 207, true);

            if (image != null)
                image.Dispose();

            return result ? Json(new { success = true, url = photoUrl, pid = photo.Id }) : Json(new { error = "whoops, something went wrong." });
        }

        // POST: /AddVehicle/RemovePhoto/1
        [HttpPost]
        public ActionResult RemovePhoto(int id)
        {
            var v = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (v == null)
                return Json(new { success = false, message = "Whoops, no such vehicle found." });

            var photoId = int.Parse(Request.Form["pid"]);
            var photo = v.Photos.SingleOrDefault(q => q.Id == photoId);
            if (photo == null)
                return Json(new { success = false, message = "Whoops, no such photo found." });

            SVD.Controller.Instance.VehicleController.DeleteVehiclePhoto(photo);
            return Json(new { success = true, message = "Photo removed!" });
        }

        // POST: /AddVehicle/RemovePhoto/1
        [HttpPost]
        public ActionResult RemoveVideo(int id)
        {
            var v = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (v == null)
                return Json(new { success = false, message = "Whoops, no such vehicle found." });

            var videoId = int.Parse(Request.Form["videoId"]);
            var video = v.Videos.SingleOrDefault(q => q.Id == videoId);
            if (video == null)
                return Json(new { success = false, message = "Whoops, no such video found." });

            SVD.Controller.Instance.VehicleController.DeleteVehicleVideo(video);
            return Json(new { success = true, message = "Video removed!" });
        }
        #endregion
    }
}