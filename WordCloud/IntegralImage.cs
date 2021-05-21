using System;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace WordCloudSharp
{
    internal class IntegralImage
	{
        #region attributes & constructors
        public int OutputImgWidth { get; set; }

        public int OutputImgHeight { get; set; }

        protected uint[,] Integral { get; set; }

        public IntegralImage(int outputImgWidth, int outputImgHeight)
		{
		    this.Integral = new uint[outputImgWidth,outputImgHeight];
		    this.OutputImgWidth = outputImgWidth;
		    this.OutputImgHeight = outputImgHeight;
        }

        public IntegralImage(FastImage image)
        {
            this.Integral = new uint[image.Width, image.Height];
            this.OutputImgWidth = image.Width;
            this.OutputImgHeight = image.Height;
            InitMask(image);
        }

        #endregion

        private void InitMask(FastImage image)
        {
            Update(Util.CropImage(image), 1 ,1);
        }

        public void Update(FastImage image, int posX, int posY)
		{          
			if (posX < 1) posX = 1;
			if (posY < 1) posY = 1;
		    var pixelSize = Math.Min(3, image.PixelFormatSize);

            for (var i = posY; i < image.Height; ++i)
            {
                for (var j = posX; j < image.Width; ++j)
                {
                    byte pixel = 0;
                    for (var p = 0; p < pixelSize; ++p)
                    {
                        pixel |= image.Data[i * image.Stride + j * image.PixelFormatSize + p];
                    }
                    this.Integral[j, i] = pixel + this.Integral[j - 1, i] + this.Integral[j, i - 1] - this.Integral[j - 1, i - 1];
                }
            }
        }

        public ulong GetArea(int xPos, int yPos, int sizeX, int sizeY)
		{
			ulong area = this.Integral[xPos, yPos] + this.Integral[xPos + sizeX, yPos + sizeY];
			area -= this.Integral[xPos + sizeX, yPos] + this.Integral[xPos, yPos + sizeY];
			return area;
		}

#if DEBUG

	    public Bitmap IntegralImageToBitmap()
	    {
            var bmp = new Bitmap(this.OutputImgWidth, this.OutputImgHeight, PixelFormat.Format8bppIndexed);
	        var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
	            ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
	        var len = bmpdata.Stride*bmp.Height;
	        var buffer = new byte[len];
	        var bufIndex = 0;
	        for (var i = 0; i < this.OutputImgHeight; ++i)
	        {
	            for (var j = 0; j < this.OutputImgWidth; ++j)
	            {
                    if (i == 0 || j == 0)
                    {
                        buffer[bufIndex++] = 255;
                        continue;
                    }
                    if (this.Integral[j, i] - this.Integral[j - 1, i] - this.Integral[j, i - 1] + this.Integral[j - 1, i - 1] == 0)
                        buffer[bufIndex++] = 255;
                    else
	                    buffer[bufIndex++] = 0;
	            }
	        }
	        Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
	        bmp.UnlockBits(bmpdata);
            return bmp;
	    }

	    public void SaveIntegralImageToBitmap(string filename = "")
	    {
            if(filename == "")
                IntegralImageToBitmap().Save(Path.GetRandomFileName() + ".bmp");
            else
                IntegralImageToBitmap().Save(filename + DateTime.Now.ToString("_hhmmss_fff")+".bmp");
        }



	    public static string GetSha(byte[] array)
	    {
	        using (var sha1 = new SHA1CryptoServiceProvider())
	        {
	            return Convert.ToBase64String(sha1.ComputeHash(array));
	        }
        }

#endif
    }
}
