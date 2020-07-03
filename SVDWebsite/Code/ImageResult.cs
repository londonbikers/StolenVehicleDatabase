using System.IO;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace SVDWebsite.Code
{
    public class ImageResult : FileStreamResult
    {
        #region constructors
        public ImageResult(Image input) : base(GetMemoryStream(input), "image/jpeg") {}
        #endregion

        static MemoryStream GetMemoryStream(Image input)
        {
            var ms = new MemoryStream();
            var info = ImageCodecInfo.GetImageEncoders();
            var encParams = new EncoderParameters(1);
            encParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)95);
            input.Save(ms, info[1], encParams);
            ms.Position = 0;
            return ms;
        }
    }
}