using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace WordCloudSharp
{
    internal class Util
    {
        internal static FastImage CropImage(FastImage img)
        {
            var cropRect = new Rectangle(1, 1, img.Width - 1, img.Height - 1);
            var src = img.Bitmap;
            var target = new Bitmap(cropRect.Width, cropRect.Height);

            using (var g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }
            return new FastImage(target);
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        internal static Image ResizeImage(Image image, int width, int height)
        {
            if (image.Width == width && image.Height == height)
                return image;
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            var bmpdata = destImage.LockBits(new Rectangle(0, 0, destImage.Width, destImage.Height), ImageLockMode.ReadOnly, destImage.PixelFormat);
            var len = bmpdata.Height * bmpdata.Stride;
            var buf = new byte[len];
            Marshal.Copy(bmpdata.Scan0, buf, 0, len);
            for (var i = 0; i < width*height*4; i++)
            {
                if (buf[i] == 255 || buf[i] == 0)
                    continue;
                if (i % 4 == 3)
                    continue;
                if (buf[i] > 0)
                    buf[i] = 255;
                else
                    buf[i] = 0;
            }
            Marshal.Copy(buf, 0, bmpdata.Scan0, len);
            destImage.UnlockBits(bmpdata);

            return destImage;
        }

        internal static bool CheckMaskValid(Image mask)
        {
            bool valid;
            using (var bmp = new Bitmap(mask))
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                var len = bmpdata.Height * bmpdata.Stride;
                var buf = new byte[len];
                Marshal.Copy(bmpdata.Scan0, buf, 0, len);
                valid = buf.Count(b => b != 0 && b != 255) == 0;
                bmp.UnlockBits(bmpdata);
            }
            return valid;
        }
    }
}