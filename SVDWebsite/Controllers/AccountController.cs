using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SVD;
using SVD.Models;
using SVDWebsite.Code;
using SVDWebsite.Models;
using Controller = System.Web.Mvc.Controller;

namespace SVDWebsite.Controllers
{
    public class AccountController : Controller
    {
        #region accessors
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        #endregion

        #region constructors
        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }
            base.Initialize(requestContext);
        }
        #endregion

        #region ASP.NET Membership

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("index", "home");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("index", "home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("index", "home");
                }
                ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("changepasswordsuccess");
                }
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
        #endregion

        // GET: /Account
        public ActionResult Index()
        {
            var member = Membership.GetUser();
            if (!User.Identity.IsAuthenticated)
            {
                Helpers.AddPageMessage("Please login first!", PageMessageType.Information);
                return RedirectToAction("logon");
            }
            
            var v = SVD.Controller.Instance.VehicleController.GetVehiclesForUser((Guid)member.ProviderUserKey);
            var m = new AccountIndexModel
            {
                Vehicles = v
            };

            return View(m);
        }

        // GET: /Account/Edit/45
        [Authorize]
        public ActionResult Edit(int id)
        {
            var v = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (v == null)
            {
                Helpers.AddPageMessage("No such vehicle found!", PageMessageType.Error);
                RedirectToAction("Index");
            }

            if (!Helpers.UserHasVehicleControlRights(v))
            {
                Helpers.AddPageMessage("You shouldn't be there!", PageMessageType.Error);
                RedirectToAction("Index", "Home");
            }
            
            var m = new EditVehicleModel
            {
                Title = v.Title,
                Description = v.Description,
                IsLocationSensitive = v.IsLocationApproximate,
                PoliceForce = v.PoliceForce,
                PolicePhoneNumber = v.PolicePhoneNumber,
                PoliceReference = v.PoliceReference,
                Registration = v.Registration,
                TheftDescription = v.TheftDescription,
                TheftMethodId = v.TheftMethod.Id.ToString(),
                VehicleManufacturerId = v.Model.ManufacturerId.ToString(),
                VehicleModelId = v.Model.Id.ToString(),
                VehicleTypeId = v.Model.Type.Id.ToString(),
                Vin = v.Vin,
                EngineNumber = v.EngineNumber,
                Year = v.Year.ToString()
            };

            if (v.Colour != null)
                m.ColourId = v.Colour.Id.ToString();

            if (v.TheftDate.HasValue)
                m.TheftDate = v.TheftDate.Value.ToString("dd MMMM yyyy");

            m.VehicleSecurityTypeCsv = ",";
            foreach (var vst in v.SecurityTypes)
                m.VehicleSecurityTypeCsv += vst.Id + ",";

            if (v.TheftLatitude.HasValue && v.TheftLongitude.HasValue)
            {
                m.TheftLocationLat = v.TheftLatitude.Value.ToString();
                m.TheftLocationLong = v.TheftLongitude.Value.ToString();
            }
            
            return View(m);
        }

        // POST: /Account/Edit/45
        [HttpPost]
        [Authorize]
        public ActionResult Edit(int id, EditVehicleModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var v = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (v == null)
            {
                Helpers.AddPageMessage("No such vehicle found!", PageMessageType.Error);
                RedirectToAction("Index");
            }

            if (!Helpers.UserHasVehicleControlRights(v))
            {
                Helpers.AddPageMessage("You shouldn't be there!", PageMessageType.Error);
                RedirectToAction("Index", "Home");
            }

            // do the updates.
            v.Status = (VehicleStatus) byte.Parse(model.StatusId);
            v.Description = model.Description;
            v.IsLocationApproximate = false;
            v.Model = SVD.Controller.Instance.MakesAndModelsController.GetModel(int.Parse(model.VehicleModelId)).Model;
            v.PoliceForce = model.PoliceForce;
            v.PolicePhoneNumber = model.PolicePhoneNumber;
            v.PoliceReference = model.PoliceReference;
            v.Registration = model.Registration;
            v.SecurityTypes = model.VehicleSecurityTypesChosen;
            v.TheftDescription = model.TheftDescription;
            v.TheftMethod = SVD.Controller.Instance.VehicleController.GetTheftMethod(int.Parse(model.TheftMethodId));
            v.Vin = model.Vin;
            v.EngineNumber = model.EngineNumber;
            v.Year = int.Parse(model.Year);
            v.Colour = !string.IsNullOrEmpty(model.ColourId) ? SVD.Controller.Instance.VehicleController.GetColour(int.Parse(model.ColourId)) : null;

            DateTime date;
            if (DateTime.TryParse(model.TheftDate, out date))
                v.TheftDate = date;

            #region location
            if (!string.IsNullOrEmpty(model.TheftLocationLat) && !string.IsNullOrEmpty(model.TheftLocationLong))
            {
                var newLat = decimal.Parse(model.TheftLocationLat);
                var newLong = decimal.Parse(model.TheftLocationLong);

                if (v.TheftLatitude != newLat || v.TheftLongitude != newLong)
                {
                    v.TheftLatitude = newLat;
                    v.TheftLongitude = newLong;

                    // clear the places.
                    v.TheftLocationPlaces.Clear();

                    // create a place object for each location type we're aware of from Google.
                    if (!string.IsNullOrEmpty(model.TheftLocationCountry))
                    {
                        var pos = model.TheftLocationCountryPos.Split(',');
                        v.TheftLocationPlaces.Add(new Place(ObjectMode.New)
                        {
                            Type = PlaceType.Country,
                            Code = model.TheftLocationCountryCode,
                            Name = model.TheftLocationCountry,
                            Latitude = double.Parse(pos[0]),
                            Longitude = double.Parse(pos[1])
                        });
                    }
                    if (!string.IsNullOrEmpty(model.TheftLocationAdministrativeAreaLevel1))
                    {
                        var pos = model.TheftLocationAdministrativeAreaLevel1Pos.Split(',');
                        v.TheftLocationPlaces.Add(new Place(ObjectMode.New)
                        {
                            Type = PlaceType.AdministrativeAreaLevel1,
                            Name = model.TheftLocationAdministrativeAreaLevel1,
                            Latitude = double.Parse(pos[0]),
                            Longitude = double.Parse(pos[1])
                        });
                    }
                    if (!string.IsNullOrEmpty(model.TheftLocationAdministrativeAreaLevel2))
                    {
                        var pos = model.TheftLocationAdministrativeAreaLevel2Pos.Split(',');
                        v.TheftLocationPlaces.Add(new Place(ObjectMode.New)
                        {
                            Type = PlaceType.AdministrativeAreaLevel2,
                            Name = model.TheftLocationAdministrativeAreaLevel2,
                            Latitude = double.Parse(pos[0]),
                            Longitude = double.Parse(pos[1])
                        });
                    }
                    if (!string.IsNullOrEmpty(model.TheftLocationRoute))
                    {
                        var pos = model.TheftLocationRoutePos.Split(',');
                        v.TheftLocationPlaces.Add(new Place(ObjectMode.New)
                        {
                            Type = PlaceType.Route,
                            Name = model.TheftLocationRoute,
                            Latitude = double.Parse(pos[0]),
                            Longitude = double.Parse(pos[1])
                        });
                    }
                    if (!string.IsNullOrEmpty(model.TheftLocationSubLocality))
                    {
                        var pos = model.TheftLocationSubLocalityPos.Split(',');
                        v.TheftLocationPlaces.Add(new Place(ObjectMode.New)
                        {
                            Type = PlaceType.SubLocality,
                            Name = model.TheftLocationSubLocality,
                            Latitude = double.Parse(pos[0]),
                            Longitude = double.Parse(pos[1])
                        });
                    }
                    if (!string.IsNullOrEmpty(model.TheftLocationLocality))
                    {
                        var pos = model.TheftLocationLocalityPos.Split(',');
                        v.TheftLocationPlaces.Add(new Place(ObjectMode.New)
                        {
                            Type = PlaceType.Locality,
                            Name = model.TheftLocationLocality,
                            Latitude = double.Parse(pos[0]),
                            Longitude = double.Parse(pos[1])
                        });
                    }

                    Helpers.ResolvePlaceParents(v.TheftLocationPlaces);
                }
            }
            #endregion

            SVD.Controller.Instance.VehicleController.UpdateVehicle(v);
            Helpers.AddPageMessage("Vehicle updated!", PageMessageType.Success);
            model.Title = v.Title;
            return View(model);
        }

        // GET: /AddVehicle/StageTwo/5
        [Authorize]
        public ActionResult EditMedia(int id)
        {
            var v = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (v == null)
            {
                Helpers.AddPageMessage("No such vehicle found!", PageMessageType.Error);
                RedirectToAction("Index");
            }

            if (!Helpers.UserHasVehicleControlRights(v))
            {
                Helpers.AddPageMessage("You shouldn't be there!", PageMessageType.Error);
                RedirectToAction("Index", "Home");
            }

            if (v != null)
            {
                var videos = new StringBuilder();
                if (v.Videos.Count > 0)
                {
                    videos.Append("[");
                    foreach (var video in v.Videos)
                        videos.AppendFormat("[\"{0}\",{1}],", video.Url, video.Id);
                    videos.Remove(videos.Length - 1, 1);
                    videos.Append("]");
                }
                else
                {
                    videos.Append("null");
                }

                var model = new EditVehicleMediaModel
                {
                    Id = id,
                    Type = v.Model.Type.Name,
                    Title = v.Title,
                    Videos = v.Videos,
                    Photos = v.Photos,
                    VideoArray = videos.ToString()
                };
                return View(model);
            }

            return RedirectToAction("Index", "home");
        }

        // GET: /Account/Delete/45
        [Authorize]
        public ActionResult Delete(int id)
        {
            var v = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (v == null)
            {
                Helpers.AddPageMessage("No such vehicle found!", PageMessageType.Error);
                RedirectToAction("Index");
            }

            if (!Helpers.UserHasVehicleControlRights(v))
            {
                Helpers.AddPageMessage("You shouldn't be there!", PageMessageType.Error);
                RedirectToAction("Index", "Home");
            }

            var m = new AccountDeleteModel {Vehicle = v};
            return View(m);
        }

        // POST: /Account/Delete/45
        [HttpPost]
        [Authorize]
        public ActionResult DeleteHandler(int id, string mode)
        {
            var v = SVD.Controller.Instance.VehicleController.GetVehicle(id);
            if (v == null)
            {
                Helpers.AddPageMessage("No such vehicle found!", PageMessageType.Error);
                RedirectToAction("Index");
            }

            if (!Helpers.UserHasVehicleControlRights(v))
            {
                Helpers.AddPageMessage("You shouldn't be there!", PageMessageType.Error);
                RedirectToAction("Index", "Home");
            }

            var action = "deleted";
            if (v == null)
            {
                Helpers.AddPageMessage("Sorry, no such vehicle found!", PageMessageType.Error);
                return RedirectToAction("Index");
            }

            if (mode == "delete")
            {
                SVD.Controller.Instance.VehicleController.DeleteVehicle(v);
            }
            else
            {
                action = "archived";
                v.Status = VehicleStatus.Archived;
                SVD.Controller.Instance.VehicleController.UpdateVehicle(v);
            }

            Helpers.AddPageMessage(string.Format("Vehicle {0}!", action), PageMessageType.Success);
            return RedirectToAction("index");
        }
    }
}
