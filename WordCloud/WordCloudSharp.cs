using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.DrawingCore.Text;
using System.Threading;

namespace WordCloudSharp
{
	/// <summary>
	/// Class to draw word clouds.
	/// </summary>
	public class WordCloud
	{
        public event Action<double> OnProgress;
#if DEBUG
        public event Action<Image> OnStepDrawResultImg;

        public event Action<Image> OnStepDrawIntegralImg;

	    private AutoResetEvent DrawWaitHandle;

	    public bool StepDrawMode { get; set; }
#endif
        #region attributes
        /// <summary>
        /// Gets font colour or random if font wasn't set
        /// </summary>
        private Color FontColor
        {
            get { return this._mFontColor ?? GetRandomColor(); }
            set { this._mFontColor = value; }
        }


        private Color? _mFontColor;


        /// <summary>
        /// Used to select random colors.
        /// </summary>
        private Random Random { get; set; }


        /// <summary>
        /// Working image.
        /// </summary>
        private FastImage WorkImage { get; set; }


        /// <summary>
        /// Keeps track of word positions using integral image.
        /// </summary>
        private OccupancyMap Map { get; set; }


        /// <summary>
        /// Gets or sets the maximum size of the font.
        /// </summary>
        private float MaxFontSize { get; set; }


        /// <summary>
        /// User input order instead of frequency
        /// </summary>
        private bool UseRank { get; set; }

        /// <summary>
        /// Amount to decrement font size each time a word won't fit.
        /// </summary>
        private int FontStep { get; set; }

        /// <summary>
        /// If allow vertical drawing 
        /// </summary>
	    private bool AllowVertical { get; set; }

        private string Fontname
        {
            get { return this._fontname ?? "Microsoft Sans Serif"; }
            set
            {
                this._fontname = value;
                if (value == null) return;
                using (var f = new Font(value, 12,  FontStyle.Regular))
                {
                    this._fontname = f.FontFamily.Name;
                }
            }
        }

	    private string _fontname;

	    #endregion

	    /// <summary>
	    /// Initializes a new instance of the <see cref="WordCloud"/> class.
	    /// </summary>
	    /// <param name="width">The width of word cloud.</param>
	    /// <param name="height">The height of word cloud.</param>
	    /// <param name="useRank">if set to <c>true</c> will ignore frequencies for best fit.</param>
	    /// <param name="fontColor">Color of the font.</param>
	    /// <param name="maxFontSize">Maximum size of the font.</param>
	    /// <param name="fontStep">The font step to use.</param>
	    /// <param name="mask">mask image</param>
	    /// <param name="allowVerical">allow vertical text</param>
	    public WordCloud(int width, int height, bool useRank = false, Color? fontColor = null, float maxFontSize = -1, int fontStep = 1, Image mask = null, bool allowVerical = false, string fontname = null)
		{
	        if (mask == null)
	        {
	            this.Map = new OccupancyMap(width, height);
	            this.WorkImage = new FastImage(width, height, PixelFormat.Format32bppArgb);
	        }
	        else
	        {
	            mask = Util.ResizeImage(mask, width, height);
                if (!Util.CheckMaskValid(mask))
	                throw new Exception("Mask is not a valid black-white image");
                this.Map = new OccupancyMap(mask);
                this.WorkImage = new FastImage(mask);
            }

            this.MaxFontSize = maxFontSize < 0 ? (float)height : maxFontSize;
		    this.FontStep = fontStep;
		    this._mFontColor = fontColor;
		    this.UseRank = useRank;
		    this.Random = new Random(Environment.TickCount);
	        this.AllowVertical = allowVerical;
            this.Fontname = fontname;
#if DEBUG
            this.DrawWaitHandle = new AutoResetEvent(false);
            this.StepDrawMode = false;
#endif
        }

        /// <summary>
        /// Gets a random color.
        /// </summary>
        /// <returns>Color</returns>
        private Color GetRandomColor()
        {
            return Color.FromArgb(this.Random.Next(0, 255), this.Random.Next(0, 255), this.Random.Next(0, 255));
        }

        /// <summary>
        /// Draws the specified word cloud given list of words and frequecies
        /// </summary>
        /// <param name="words">List of words ordered by occurance.</param>
        /// <param name="freqs">List of frequecies.</param>
        /// <param name="bgcolor">Backgroud color of the output image</param>
        /// <param name="img">Backgroud image of the output image</param>
        /// <returns>Image of word cloud.</returns>
        /// <exception cref="System.ArgumentException">
        /// Arguments null.
        /// or
        /// Must have the same number of words as frequencies.
        /// </exception>
        private Image Draw(IList<string> words, IList<int> freqs, Color bgcolor, Image img)
		{
#if DEBUG
            ShowIntegralImgStepDraw(this.Map.IntegralImageToBitmap());
            this.DrawWaitHandle.Reset();
#endif
            var fontSize = this.MaxFontSize;
			if (words == null || freqs == null)
			{
				throw new ArgumentException("Arguments null.");
			}
			if (words.Count != freqs.Count)
			{
				throw new ArgumentException("Must have the same number of words as frequencies.");
			}

            Bitmap result ;
            if (img == null)
                result = new Bitmap(this.WorkImage.Width, this.WorkImage.Height);
            else
            {
                if (img.Size != this.WorkImage.Bitmap.Size)
                    throw new Exception("The backgroud img should be with the same size with the mask");
                result = new Bitmap(img);
            }

            using (var gworking = Graphics.FromImage(this.WorkImage.Bitmap))
            using (var gresult = Graphics.FromImage(result))
            {
                if(img == null)
                    gresult.Clear(bgcolor);
                gresult.TextRenderingHint = TextRenderingHint.AntiAlias;
                gworking.TextRenderingHint = TextRenderingHint.AntiAlias;
			    var lastProgress = 0.0d;
				for (var i = 0; i < words.Count; ++i)
				{
				    var progress = (double) i/words.Count;
				    if (progress - lastProgress > 0.01)
				    {
				        ShowProgress(progress);
                        lastProgress = progress;
                    }
                    if (!this.UseRank)
					{
						fontSize =  (float) Math.Min(fontSize, 100*Math.Log10(freqs[i] + 100));
					}

                    var format = new StringFormat();
				    if (this.AllowVertical)
				    {
                        if (this.Random.Next(0, 2) == 1)
                            format.FormatFlags = StringFormatFlags.DirectionVertical;
                    }

                    Point p;
                    var foundPosition = false;
					Font font;
				    var size = new SizeF();
				    Debug.WriteLine("Word: " + words[i]);
					do
					{
						font = new Font(this.Fontname, fontSize, GraphicsUnit.Pixel);
						size = gworking.MeasureString(words[i], font, new PointF(0, 0), format);
					    Debug.WriteLine("Search with font size: " + fontSize);
						foundPosition = this.Map.GetRandomUnoccupiedPosition((int) size.Width, (int) size.Height, out p);
						if (!foundPosition) fontSize -= this.FontStep;
					} while (fontSize > 0 && !foundPosition);
				    Debug.WriteLine("Found pos: " + p);
					if (fontSize <= 0) break;
                    gworking.DrawString(words[i], font, new SolidBrush(this.FontColor), p.X, p.Y, format);
                    gresult.DrawString(words[i], font, new SolidBrush(this.FontColor), p.X, p.Y, format);
                    this.Map.Update(this.WorkImage, p.X, p.Y);
#if DEBUG
				    if (this.StepDrawMode)
				    {
                        ShowResultStepDraw(new Bitmap(this.WorkImage.Bitmap));
                        ShowIntegralImgStepDraw(this.Map.IntegralImageToBitmap());
                        this.DrawWaitHandle.WaitOne();
				    }
#endif
				}
			}
            this.WorkImage.Dispose();
            return result;
		}

        /// <summary>
        /// Draws the specified word cloud with background color spicified given list of words and frequecies
        /// </summary>
        /// <param name="words">List of words ordered by occurance.</param>
        /// <param name="freqs">List of frequecies.</param>
        /// <param name="bgcolor">Specified background color</param>
        /// <returns>Image of word cloud.</returns>
        public Image Draw(IList<string> words, IList<int> freqs, Color bgcolor)
        {
            return Draw(words, freqs, bgcolor, null);
        }

        /// <summary>
        /// Draws the specified word cloud with background spicified given list of words and frequecies
        /// </summary>
        /// <param name="words">List of words ordered by occurance.</param>
        /// <param name="freqs">List of frequecies.</param>
        /// <param name="img">Specified background image</param>
        /// <returns>Image of word cloud.</returns>
        public Image Draw(IList<string> words, IList<int> freqs, Image img)
        {
            return Draw(words, freqs, Color.White, img);
        }

        /// <summary>
        /// Draws the specified word cloud with given list of words and frequecies
        /// </summary>
        /// <param name="words">List of words ordered by occurance.</param>
        /// <param name="freqs">List of frequecies.</param>
        /// <returns>Image of word cloud.</returns>
        public Image Draw(IList<string> words, IList<int> freqs)
        {
            return Draw(words, freqs, Color.White, null);
        }

        private void ShowProgress(double progress)
	    {
	        OnProgress?.Invoke(progress);
	    }
#if DEBUG
        private void ShowResultStepDraw(Image bmp)
        {
            OnStepDrawResultImg?.Invoke(bmp);
        }

        private void ShowIntegralImgStepDraw(Image bmp)
        {
            OnStepDrawIntegralImg?.Invoke(bmp);
        }

        public void ContinueDrawing()
	    {
            this.DrawWaitHandle.Set();
	    }
#endif
    }
}
