using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SVD;
using SVD.Models;
using SVD.VDWS;

namespace SVDWebsite.Models
{
    public class EditVehicleModel
    {
        // source data
        public SelectList Colours
        {
            get
            {
                var list = new SelectList(SVD.Controller.Instance.VehicleController.GetColours(), "Id", "Name");
                return list;
            }
        }

        public SelectList VehicleTypes
        {
            get
            {
                var types = SVD.Controller.Instance.MakesAndModelsController.VehicleTypes;
                var list = new SelectList(types, "Id", "Name");
                return list;
            }
        }

        public SelectList VehicleManufacturers
        {
            get
            {
                var types = SVD.Controller.Instance.MakesAndModelsController.VehicleTypes;
                var type = types[0];
                if (!string.IsNullOrEmpty(VehicleTypeId))
                    type = types.Single(q => q.Id == int.Parse(VehicleTypeId));

                var makes =
                    SVD.Controller.Instance.MakesAndModelsController.VehicleManufacturers.Where(
                        q => q.Manufacturer.Types.Any(q2 => q2.Id == type.Id)).Select(q => q.Manufacturer);
                var list = new SelectList(makes, "Id", "Name");
                return list;
            }
        }

        public SelectList VehicleModels
        {
            get
            {
                var types = SVD.Controller.Instance.MakesAndModelsController.VehicleTypes;
                var type = types[0];
                if (!string.IsNullOrEmpty(VehicleTypeId))
                    type = types.Single(q => q.Id == int.Parse(VehicleTypeId));

                VehicleManufacturer make;
                if (!string.IsNullOrEmpty(VehicleManufacturerId))
                    make = SVD.Controller.Instance.MakesAndModelsController.GetManufacturer(int.Parse(VehicleManufacturerId)).Manufacturer;
                else
                    make = SVD.Controller.Instance.MakesAndModelsController.VehicleManufacturers.Single(q => q.Manufacturer.Types.Any(q2 => q2.Id == type.Id)).Manufacturer;

                var models =
                    SVD.Controller.Instance.MakesAndModelsController.VehicleModels.Where(
                        q => q.Model.ManufacturerId == make.Id).Select(q => q.Model);
                var list = new SelectList(models, "Id", "Name");
                return list;
            }
        }

        public SelectList VehicleSecurityTypes
        {
            get 
            { 
                var types = SVD.Controller.Instance.VehicleController.GetVehicleSecurityTypes();
                var list = new SelectList(types, "Id", "Name");
                return list;
            }
        }

        public SelectList TheftMethods
        {
            get
            {
                var methods = SVD.Controller.Instance.VehicleController.GetTheftMethods();
                var list = new SelectList(methods, "Id", "Name");
                return list;
            }
        }

        public SelectList Years
        {
            get 
            { 
                var items = new List<SelectListItem>();
                for (var i = DateTime.Now.Year; i >= DateTime.Now.Year - 100; i--)
                    items.Add(new SelectListItem {Text = i.ToString(), Value = i.ToString()});
                var list = new SelectList(items, "Value", "Text");
                return list;
            }
        }

        public SelectList Status
        {
            get
            {
                var items = new List<SelectListItem>
                {
                    new SelectListItem {Text = "Missing", Value = ((byte) VehicleStatus.Active).ToString()},
                    new SelectListItem {Text = "Retrieved", Value = ((byte) VehicleStatus.Retrieved).ToString()}
                };
                var list = new SelectList(items, "Value", "Text");
                return list;
            }
        }

        public string Title { get; set; }

        // core details
        [Required]
        [Display(Name = "Status")]
        public string StatusId { get; set; }
        
        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleTypeId { get; set; }

        [Required]
        [Display(Name = "Vehicle Manufacturer")]
        public string VehicleManufacturerId { get; set; }

        [Required]
        [Display(Name = "Vehicle Model")]
        public string VehicleModelId { get; set; }

        [Display(Name = "Colour")]
        public string ColourId { get; set; }

        [Required]
        [Display(Name = "Year")]
        [DataType(DataType.Text)]
        public string Year { get; set; }

        [Display(Name = "Registration / Plate No")]
        [DataType(DataType.Text)]
        public string Registration { get; set; }

        [Display(Name = "VIN / Chassis number")]
        [DataType(DataType.Text)]
        public string Vin { get; set; }

        [Display(Name = "Engine number")]
        [DataType(DataType.Text)]
        public string EngineNumber { get; set; }

        [Display(Name = "Vehicle description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        // police details
        [Display(Name = "Police force")]
        [DataType(DataType.Text)]
        public string PoliceForce { get; set; }

        [Display(Name = "Police telephone number")]
        [DataType(DataType.PhoneNumber)]
        public string PolicePhoneNumber { get; set; }

        [Display(Name = "Police reference")]
        [DataType(DataType.Text)]
        public string PoliceReference { get; set; }

        // theft details
        [Required]
        [Display(Name = "Date of theft")]
        [DataType(DataType.Date)]
        public string TheftDate { get; set; }

        [Display(Name = "How did it happen?")]
        [DataType(DataType.MultilineText)]
        public string TheftDescription { get; set; }

        [Display(Name = "Don't be too precise, this is my home, etc...", Description = "We will record a town if sensitive, not anything more specific.")]
        public bool IsLocationSensitive { get; set; }

        [Display(Name = "Theft location")]
        [DataType(DataType.Text)]
        public string TheftLocation { get; set; }

        public string TheftLocationLat { get; set; }
        public string TheftLocationLong { get; set; }
        public string TheftLocationCountry { get; set; }
        public string TheftLocationCountryCode { get; set; }
        public string TheftLocationCountryPos { get; set; }
        public string TheftLocationAdministrativeAreaLevel1 { get; set; }
        public string TheftLocationAdministrativeAreaLevel1Pos { get; set; }
        public string TheftLocationAdministrativeAreaLevel2 { get; set; }
        public string TheftLocationAdministrativeAreaLevel2Pos { get; set; }
        public string TheftLocationLocality { get; set; }
        public string TheftLocationLocalityPos { get; set; }
        public string TheftLocationSubLocality { get; set; }
        public string TheftLocationSubLocalityPos { get; set; }
        public string TheftLocationRoute { get; set; }
        public string TheftLocationRoutePos { get; set; }
        
        // theft method
        [Required]
        [Display(Name = "Method of theft")]
        public string TheftMethodId { get; set; }

        // security details
        public List<VehicleSecurityType> VehicleSecurityTypesChosen
        {
            get
            {
                // build the list from the csv.
                if (string.IsNullOrEmpty(VehicleSecurityTypeCsv))
                    return new List<VehicleSecurityType>();

                var csv = VehicleSecurityTypeCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var list = csv.Select(id => SVD.Controller.Instance.VehicleController.GetVehicleSecurityType(int.Parse(id))).ToList();
                return list;
            }
        }

        /// <summary>
        /// Meant for hidden field
        /// </summary>
        public string VehicleSecurityTypeCsv { get; set; }
    }

    public class EditVehicleMediaModel
    {
        #region source data
        public string Title { get; set; }
        public string Type { get; set; }
        public List<Photo> Photos { get; set; }
        public List<Video> Videos { get; set; }
        public string VideoArray { get; set; }
        #endregion

        #region form
        [Required]
        public int Id { get; set; }

        [Display(Name = "YouTube video address (Url)")]
        [DataType(DataType.Text)]
        public string YouTubeVideoUrl { get; set; }
        #endregion
    }
}