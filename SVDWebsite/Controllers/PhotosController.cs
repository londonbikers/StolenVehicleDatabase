using System.Configuration;
using System.Drawing;
using System.Web.Mvc;
using System.Web.SessionState;
using SVDWebsite.Code;

namespace SVDWebsite.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class PhotosController : Controller
    {
        [OutputCache(Duration = 31536000, VaryByParam = "*")]
        public ActionResult Show(int year, int month, int day, string filename, int length, char mode)
        {
            // mode param:
            // - n = normal
            // - s = square

            // ** COMMENTS
            // Two ideals here...
            // 1: Resized images to be created so they don't have to be resized on subsequent request.
            // 2: This to be moved out into a separate domain all-together to have cookie-less requests, speeding up delivery.

            var root = ConfigurationManager.AppSettings["FileStorePath"];
            var path = string.Format("{0}\\{1}\\{2}\\{3}\\{4}.jpg", root, year, month, day, filename);
            using (var source = Image.FromFile(path))
            {
                // if no size specified, treat as high-res view and return source image.
                if (length == 0)
                    return new ImageResult(source);

                Image resized = null;
                try
                {
                    resized = mode == 's' ? ImageHelpers.ResizeToSquare(source, length) : ImageHelpers.ResizeImage(source, length);
                    return new ImageResult(resized);
                }
                finally
                {
                    if (resized != null)
                        resized.Dispose();
                }
            }
        }
    }
}