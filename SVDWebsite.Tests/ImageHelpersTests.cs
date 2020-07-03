using Microsoft.VisualStudio.TestTools.UnitTesting;
using SVDWebsite.Code;

namespace SVDWebsite.Tests
{
    [TestClass]
    public class ImageHelpersTests
    {
        [TestMethod]
        public void ResizeImageCorrectSizeTest()
        {
            // source image is 3750 x 2500 pixels.
            using (var image = Resources.bmw1)
            {
                using (var newImage = ImageHelpers.ResizeImage(image, 800))
                {
                    Assert.IsNotNull(newImage);

                    // this is the correct size (see Photoshop for comparison).
                    Assert.IsTrue(newImage.Width == 800);
                    Assert.IsTrue(newImage.Height == 533);
                }
            }
        }
    }
}