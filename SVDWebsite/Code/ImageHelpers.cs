using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace SVDWebsite.Code
{
    /// <summary>
    /// Originally donated by londonbikers.com
    /// </summary>
    public class ImageHelpers
    {
        /// <summary>
        /// Saves an Image object out to disk.
        /// </summary>
        /// <param name="path">The physical or UNC path to save the image out to.</param>
        /// <param name="image">The source image to save to disk.</param>
        /// <param name="quality">0-100, representing the quality, i.e. 100 is best quality</param>
        public static void SaveJpeg(string path, Image image, long quality = 95)
        {
            var qualityParam = new EncoderParameter(Encoder.Quality, quality);
            var jpegCodec = GetEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            image.Save(path, jpegCodec, encoderParams);
        }

        public static Image CropImage(Image img, Rectangle cropArea)
        {
            var bmpImage = new Bitmap(img);
            var bmpCrop = bmpImage.Clone(cropArea,
            bmpImage.PixelFormat);
            return bmpCrop;
        }

        public static Image ResizeImage(Image imageToResize, int longestSideLength)
        {
            // don't up-scale images, just return the original.
            var primaryDimension = imageToResize.Width >= imageToResize.Height ? imageToResize.Width : imageToResize.Height;
            if (primaryDimension <= longestSideLength)
                return imageToResize;

            int scaledWidth;
            int scaledHeight;

            if (imageToResize.Width >= imageToResize.Height)
            {
                // landscape
                scaledWidth = longestSideLength;

                // don't remove castings, will break the math.
                // ReSharper disable RedundantCast
                scaledHeight = Convert.ToInt32(Helpers.RoundUp((double)scaledWidth * (double)imageToResize.Height / (double)imageToResize.Width));
                // ReSharper restore RedundantCast
            }
            else
            {
                // portrait
                // don't remove castings, will break the math.
                // ReSharper disable RedundantCast
                scaledWidth = Convert.ToInt32(Helpers.RoundUp((double)longestSideLength * (double)imageToResize.Width / (double)imageToResize.Height));
                // ReSharper restore RedundantCast
                scaledHeight = longestSideLength;
            }

            return PlaceImageOnNewCanvas(imageToResize, scaledWidth, scaledHeight, scaledWidth, scaledHeight);
        }

        public static Image ResizeToSquare(Image imageToResize, int length)
        {
            var xPlacement = 0;
            var yPlacement = 0;
            var scaledWidth = 0;
            var scaledHeight = 0;

            // determine floating image size.
            if (imageToResize.Width >= imageToResize.Height)
            {
                // landscape
                if (imageToResize.Height < length)
                {
                    // do not enlarge the picture.
                    length = imageToResize.Height;
                }
                else
                {
                    // get new size based on new height.
                    scaledWidth = Convert.ToInt32(Helpers.RoundUp((double)length * (double)imageToResize.Width / (double)imageToResize.Height));
                    scaledHeight = length;
                }

                xPlacement = 0 - ((scaledWidth - length) / 2);
            }
            else
            {
                // portrait
                if (imageToResize.Width < length)
                {
                    // do not enlarge the picture.
                    length = imageToResize.Width;
                }
                else
                {
                    // get new size based on new width.
                    scaledWidth = length;
                    scaledHeight = Convert.ToInt32(Helpers.RoundUp((double)scaledWidth * (double)imageToResize.Height / (double)imageToResize.Width));
                }

                yPlacement = 0 - ((scaledHeight - length) / 2);
            }

            return PlaceImageOnNewCanvas(imageToResize, length, length, scaledWidth, scaledHeight, xPlacement, yPlacement);
        }

        #region private methods
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.FirstOrDefault(t => t.MimeType == mimeType);
        }

        private static Bitmap PlaceImageOnNewCanvas(Image image, int canvasWidth, int canvasHeight, int sourceWidth, int sourceHeight, int xPlacement = 0, int yPlacement = 0)
        {
            var b = new Bitmap(canvasWidth, canvasHeight);
            using (var g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(image, xPlacement, yPlacement, sourceWidth, sourceHeight);
                g.Dispose();
            }

            return b;
        }
        #endregion
    }
}