using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using SVD;
using SVD.Models;
using SVD.VDWS;

namespace SVDWebsite.Code
{
	#region helper types
	public enum PageMessageType
	{
		Success,
		Information,
		Error
	}

	public class PageMessage
	{
		public string Value { get; set; }
		public PageMessageType MessageType { get; set; }
	}
	#endregion

	public static class Helpers
	{
        /// <summary>
        /// Checks if the current logged-in user has the right to make changes to, or delete a specific vehicle.
        /// </summary>
        public static bool UserHasVehicleControlRights(Vehicle vehicle)
        {
            var member = Membership.GetUser();
            if (member == null)
                return false;

            var isStaff = (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Staff"));
            if (isStaff)
                return true;

            return vehicle.MemberUid == (Guid)member.ProviderUserKey;
        }

        public static double RoundUp(double valueToRound)
        {
            return (Math.Floor(valueToRound + 0.5));
        }

        public static bool IsIe()
        {
            return HttpContext.Current.Request.Browser.Id.StartsWith("ie");
        }

        public static string GetUserRegion()
        {
            var region = "GB";
            if (HttpContext.Current.Request.Browser != null && (HttpContext.Current.Request.Browser.Browser == "Safari" || IsIe()))
                return region;

            if (HttpContext.Current.Request.UserLanguages != null && HttpContext.Current.Request.UserLanguages.Length > 0)
            {
                var splits = HttpContext.Current.Request.UserLanguages[0].Split('-');
                if (splits.Length > 1)
                    region = splits[1];
            }

            return region;
        }

		public static void AddPageMessage(string message, PageMessageType messageType)
		{
			HttpContext.Current.Session["PageMessage"] = message;
			HttpContext.Current.Session["PageMessageType"] = messageType.ToString();
		}

		public static PageMessage RenderPageMessage()
		{
			if (HttpContext.Current.Session["PageMessage"] != null)
			{
				var message = new PageMessage
				{
				    Value = HttpContext.Current.Session["PageMessage"] as string,
				    MessageType = (PageMessageType)Enum.Parse(typeof (PageMessageType), HttpContext.Current.Session["PageMessageType"] as string)
				};
			    HttpContext.Current.Session.Remove("PageMessage");
				HttpContext.Current.Session.Remove("PageMessageType");
				return message;
			}

			return null;
		}

	    /// <summary>
	    /// Saves an uploaded file into an appropriate place within the file-store. Ensures for filename uniqueness.
	    /// </summary>
	    /// <param name="image">The posted file from the Request.Files collection.</param>
        /// <param name="filename">The name of the image file.</param>
	    /// <returns>A file-store specific path to the file, i.e. "2009/10/08/gsxr1000.jpg"</returns>
	    public static string SaveUploadedImage(Image image, string filename)
		{
			if (image == null)
				throw new ArgumentNullException();

			var date = DateTime.Now;
			var root = ConfigurationManager.AppSettings["FileStorePath"].TrimEnd('\\');
			var fullPath = string.Format("{0}\\{1}\\{2}\\{3}\\", root, date.Year, date.Month, date.Day);

            // strip the extension.
	        filename = Path.GetFileNameWithoutExtension(filename);
	        filename = GetSafeFilename(filename);
	        filename += ".jpg";

			if (!Directory.Exists(root))
				throw new DirectoryNotFoundException("FileStorePath directory not found or is not accessible!");
			if (!Directory.Exists(fullPath))
				Directory.CreateDirectory(fullPath);

            // ensure filename is unique here.
            if (File.Exists(fullPath + filename))
            {
                while (File.Exists(fullPath + filename))
                    filename = string.Format("{0}-{1}{2}", Path.GetFileNameWithoutExtension(filename), date.Ticks, Path.GetExtension(filename));
            }

            ImageHelpers.SaveJpeg(fullPath + filename, image);
			return string.Format("{0}\\{1}\\{2}\\{3}", date.Year, date.Month, date.Day, filename);
		}

        /// <summary>
        /// Transforms a filename so that it doesn't contain any illegal characters.
        /// </summary>
        /// <param name="filename">The filename to transform.</param>
        public static string GetSafeFilename(string filename)
        {
            var chars = Path.GetInvalidFileNameChars().ToList();
            chars.Add('%');
            chars.Add('&');
            return chars.Aggregate(filename, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

	    /// <summary>
	    /// Does a cursory check to see if an uploaded file is valid or not, i.e. of the right type, not empty, etc.
	    /// </summary>
	    /// <param name="fileStream">The Stream for the image..</param>
	    /// <param name="originalFilename">The original file-name, helps with file-type validation.</param>
	    public static bool IsFileUploadValid(Stream fileStream, string originalFilename)
		{
            if (fileStream.Length == 0)
				return false;

			#warning refactor to check the mime types.
			var goodExtensions = new[] { ".jpg", ".jpeg", ".bmp", ".png", ".gif" };
            return goodExtensions.Contains(Path.GetExtension(originalFilename).ToLower());
		}

        /// <summary>
        /// Converts a collection of BaseModel-derived SVD objects into a list of id/name pairs for UI use. 
        /// Objects must support INamable!
        /// </summary>
        /// <remarks>Can't get it to take in a generic argument, argh!</remarks>
        public static List<KeyValuePair<string, string>> ToUiList(List<VehicleManufacturer> list)
        {
            return list.Select(o => new KeyValuePair<string, string>(o.Id.ToString(), o.Name)).ToList();
        }

        /// <summary>
        /// Converts a collection of BaseModel-derived SVD objects into a list of id/name pairs for UI use. 
        /// Objects must support INamable!
        /// </summary>
        /// <remarks>Can't get it to take in a generic argument, argh!</remarks>
        public static List<KeyValuePair<string, string>> ToUiList(List<VehicleModel> list)
        {
            return list.Select(o => new KeyValuePair<string, string>(o.Id.ToString(), o.Name)).ToList();
        }

        /// <summary>
        /// Renders out the html needed to show a dynamic image.
        /// </summary>
        /// <param name="photo">The Photo object to render.</param>
        /// <param name="length">The primary dimension length (longest side if not square).</param>
        /// <param name="square">Crop to a square? False by default.</param>
        public static string DynamicImageUrl(Photo photo, int length, bool square = false)
        {
            var mode = square ? "s" : "n";
            var url = string.Format("/photos/{0}/{1}/{2}/{3}-{4}-{5}.jpg", photo.DateCreated.Year, photo.DateCreated.Month,
                photo.DateCreated.Day, Path.GetFileNameWithoutExtension(photo.Filename), length, mode);
            return url;
        }

        /// <summary>
        /// Converts a string into one that can be embedded within a url.
        /// </summary>
        public static string ToUrlPart(string text)
        {
            text = text.ToLower();
            text = text.Replace(" ", "_");
            //text = text.Replace("", "-");
            //text = text.Replace("'", string.Empty);
            return text;
        }

        public static string FromUrlPart(string text)
        {
            text = text.Replace("_", " ");
            return text;
        }

        public static void ResolvePlaceParents(List<Place> places)
        {
            // order by type so countries first, through to routes.
            places.Sort((p1, p2) => ((byte)p1.Type).CompareTo((byte)p2.Type));

            // not all types may be present, so we need to try and find the first higher-level place.
            foreach (var p in places)
            {
                switch (p.Type)
                {
                    case SVD.PlaceType.Route:
                        p.ParentPlace =
                            places.Where(
                                q =>
                                q.Type == SVD.PlaceType.SubLocality || 
                                q.Type == SVD.PlaceType.Locality ||
                                q.Type == SVD.PlaceType.AdministrativeAreaLevel2 ||
                                q.Type == SVD.PlaceType.AdministrativeAreaLevel1 || 
                                q.Type == SVD.PlaceType.Country).OrderByDescending
                                (q => (byte) q.Type).First();
                        break;
                    case SVD.PlaceType.SubLocality:
                        p.ParentPlace =
                            places.Where(
                                q =>
                                q.Type == SVD.PlaceType.Locality ||
                                q.Type == SVD.PlaceType.AdministrativeAreaLevel2 ||
                                q.Type == SVD.PlaceType.AdministrativeAreaLevel1 ||
                                q.Type == SVD.PlaceType.Country).OrderByDescending
                                (q => (byte)q.Type).First();
                        break;
                    case SVD.PlaceType.Locality:
                        p.ParentPlace =
                            places.Where(
                                q =>
                                q.Type == SVD.PlaceType.AdministrativeAreaLevel2 ||
                                q.Type == SVD.PlaceType.AdministrativeAreaLevel1 ||
                                q.Type == SVD.PlaceType.Country).OrderByDescending
                                (q => (byte)q.Type).First();
                        break;
                    case SVD.PlaceType.AdministrativeAreaLevel2:
                        p.ParentPlace =
                            places.Where(
                                q =>
                                q.Type == SVD.PlaceType.AdministrativeAreaLevel1 ||
                                q.Type == SVD.PlaceType.Country).OrderByDescending
                                (q => (byte)q.Type).First();
                        break;
                    case SVD.PlaceType.AdministrativeAreaLevel1:
                        p.ParentPlace = places.Single(q => q.Type == SVD.PlaceType.Country);
                        break;
                }
            }
        }

        #region Html Helpers
        /// <summary>
        /// Converts a YouTube URL into the html fragment for an embedded video.
        /// </summary>
        public static void YouTubeVideo(this HtmlHelper htmlHelper, Video video, int width = 480)
        {
            const int baseWidth = 480;
            const int baseHeight = 385;
            const decimal aspectRatio = (decimal)baseWidth / (decimal)baseHeight;
            var frameWidth = width;
            var frameHeight = Convert.ToInt32(frameWidth / aspectRatio);

            var match = Regex.Match(video.Url, @"^[^v]+v.(.{11}).*", RegexOptions.IgnoreCase);
            var id = match.Groups[1].Value;

            var template = "<div class=\"yvideo\"><object width=\"{0}\" height=\"{1}\">";
            template += "<param name=\"movie\" value=\"http://www.youtube.com/v/{2}?fs=1&amp;hl=en_GB\"></param>";
            template += "<param name=\"allowFullScreen\" value=\"true\"></param>";
            template += "<param name=\"allowscriptaccess\" value=\"always\"></param>";
            template += "<embed src=\"http://www.youtube.com/v/{2}?fs=1&amp;hl=en_GB\" type=\"application/x-shockwave-flash\" allowscriptaccess=\"always\" allowfullscreen=\"true\" width=\"{0}\" height=\"{1}\"></embed>";
            template += "</object></div>";

            var embedCode = string.Format(template, frameWidth, frameHeight, id);
            htmlHelper.ViewContext.Writer.Write(embedCode);
        }

        /// <summary>
        /// Renders out the html needed to show a dynamic image.
        /// </summary>
        /// <param name="htmlHelper">The MVC Html object to extend.</param>
        /// <param name="photo">The Photo object to render.</param>
        /// <param name="length">The primary dimension length (longest side if not square).</param>
        /// <param name="square">Crop to a square? False by default.</param>
        public static void DynamicImage(this HtmlHelper htmlHelper, Photo photo, int length, bool square = false)
        {
            var url = DynamicImageUrl(photo, length, square);
            var html = string.Format("<img src=\"{0}\" alt=\"\" />", url);
            htmlHelper.ViewContext.Writer.Write(html);
        }

        public static void WikipediaLink(this HtmlHelper htmlHelper, VehicleModel model)
        {
            var link = string.Format("<a href=\"http://en.wikipedia.org/wiki/{0}\" target=\"_blank\">Read more on Wikipedia</a>", model.WikipediaId);
            htmlHelper.ViewContext.Writer.Write(link);
        }

        public static void WikipediaLink(this HtmlHelper htmlHelper, VehicleManufacturer manufacturer)
        {
            var link = string.Format("<a href=\"http://en.wikipedia.org/wiki/{0}\" target=\"_blank\">Read more on Wikipedia</a>", manufacturer.WikipediaId);
            htmlHelper.ViewContext.Writer.Write(link);
        }

        /// <summary>
        /// Renders out the html needed to show a dynamic image.
        /// </summary>
        /// <param name="htmlHelper">The MVC Html object to extend.</param>
        /// <param name="photo">The Photo object to render.</param>
        /// <param name="length">The primary dimension length (longest side if not square).</param>
        /// <param name="square">Crop to a square? False by default.</param>
        /// <param name="bigImageLength">The length of the big image to show.</param>
        public static void DynamicImageWithZoom(this HtmlHelper htmlHelper, Photo photo, int length, bool square = false, int bigImageLength = 1600)
        {
            var url = DynamicImageUrl(photo, length, square);
            var bigImageUrl = DynamicImageUrl(photo, bigImageLength);
            var html = string.Format("<a href=\"{0}\"><img src=\"{1}\" rel=\"prettyPhoto\" alt=\"{2}\" /></a>", bigImageUrl, url, photo.Filename);
            htmlHelper.ViewContext.Writer.Write(html);
        }

        /// <summary>
        /// Renders out the html needed to show a dynamic image.
        /// </summary>
        /// <param name="htmlHelper">The MVC Html object to extend.</param>
        /// <param name="href">The url to go to.</param>
        /// <param name="photo">The Photo object to render.</param>
        /// <param name="length">The primary dimension length (longest side if not square).</param>
        /// <param name="square">Crop to a square? False by default.</param>
        public static void DynamicImageLink(this HtmlHelper htmlHelper, string href, Photo photo, int length, bool square = false)
        {
            var imageUrl = DynamicImageUrl(photo, length, square);
            var html = string.Format("<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" /></a>", href, imageUrl, photo.Filename);
            htmlHelper.ViewContext.Writer.Write(html);
        }

        /// <summary>
        /// Converts a location type name into something more readable.
        /// </summary>
        public static void ConvertPlaceType(this HtmlHelper htmlHelper, PlaceType placeType)
        {
            switch (placeType)
            {
                case PlaceType.Country:
                    htmlHelper.ViewContext.Writer.Write("Country");
                    break;
                case PlaceType.AdministrativeAreaLevel1:
                    htmlHelper.ViewContext.Writer.Write("State");
                    break;
                case PlaceType.AdministrativeAreaLevel2:
                    htmlHelper.ViewContext.Writer.Write("City / County");
                    break;
                case PlaceType.Route:
                    htmlHelper.ViewContext.Writer.Write("Road");
                    break;
                default:
                    htmlHelper.ViewContext.Writer.Write(placeType.ToString());
                    break;

            }
        }
        #endregion
    }
}