using System;
using System.Collections.Generic;
using System.DrawingCore;

namespace WordCloudSharp
{
	internal class OccupancyMap : IntegralImage
	{
        private Random Rand { get; set; }

        public OccupancyMap(int outputImgWidth, int outputImgHeight) : base(outputImgWidth, outputImgHeight)
		{
		    this.Rand = new Random(Environment.TickCount);
		}

		public OccupancyMap(Image mask) : base(new FastImage(mask))
		{
            this.Rand = new Random(Environment.TickCount);
        }

        /// <summary>
        /// get the first availible postion from occupancy map
        /// </summary>
        /// <param name="strSizeX"></param>
        /// <param name="strSizeY"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <remarks>Obsoleted because this methods not brief enough</remarks>
        [Obsolete("TryFindUnoccupiedPosition is deprecated, please use GetRandomUnoccupiedPosition instead.")]
        public bool TryFindUnoccupiedPosition(int strSizeX, int strSizeY, out Point p)
        {
            var startPosX = this.Rand.Next(1, this.OutputImgWidth);
			var startPosY = this.Rand.Next(1, this.OutputImgHeight);

			int x, y, dx, dy;
			x = y = dx = 0;
			dy = -1;
			var width = this.OutputImgWidth - strSizeX;
			var height = this.OutputImgHeight - strSizeY;

			var t = Math.Max(width, height);
			var maxI = t * t;

			for (var i = 0; i < maxI; i++)
			{
				if ((-width / 2 <= x) && (x <= width / 2) && (-height / 2 <= y) && (y <= height / 2))
				{
					var posX = x + startPosX;
					var posY = y + startPosY;
					if (posY > 0 && posY < this.OutputImgHeight - strSizeY && posX > 0 && posX < this.OutputImgWidth - strSizeX)
					{
						if (GetArea(posX, posY, strSizeX, strSizeY) == 0)
						{
						    p = new Point(posX, posY);
							return true;
						}
					}	
				}
				if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
				{
					t = dx;
					dx = -dy;
					dy = t;
				}
				x += dx;
				y += dy;
			}
            p = new Point();
			return false;
		}

        /// <summary>
        /// get the a random postion from all availible positions from occupancy map
        /// </summary>
        /// <param name="strSizeX"></param>
        /// <param name="strSizeY"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <remarks>Obsoleted because this methods not brief enough</remarks>
        [Obsolete("TryFindRandomUnoccupiedPosition is deprecated, please use GetRandomUnoccupiedPosition instead.")]
        public bool TryFindRandomUnoccupiedPosition(int strSizeX, int strSizeY, out Point p)
        {
            var startPosX = this.Rand.Next(1, this.OutputImgWidth);
            var startPosY = this.Rand.Next(1, this.OutputImgHeight);

            int x, y, dx, dy;
            x = y = dx = 0;
            dy = -1;
            var width = this.OutputImgWidth - strSizeX;
            var height = this.OutputImgHeight - strSizeY;

            var t = Math.Max(width, height);
            var maxI = t * t;

            var posList = new List<Point>();

            for (var i = 0; i < maxI; i++)
            {
                if ((-width / 2 <= x) && (x <= width / 2) && (-height / 2 <= y) && (y <= height / 2))
                {
                    var posX = x + startPosX;
                    var posY = y + startPosY;
                    if (posY > 0 && posY < this.OutputImgHeight - strSizeY && posX > 0 && posX < this.OutputImgWidth - strSizeX)
                    {
                        if (GetArea(posX, posY, strSizeX, strSizeY) == 0)
                        {
                            posList.Add(new Point(posX, posY));                            
                        }
                    }
                }
                if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
                {
                    t = dx;
                    dx = -dy;
                    dy = t;
                }
                x += dx;
                y += dy;
            }
            if (posList.Count == 0)
            {
                p = new Point();
                return false;
            }
            else
            {
                p = posList[this.Rand.Next(0, posList.Count-1)];
                return true;
            }
        }

	    public bool GetRandomUnoccupiedPosition(int strSizeX, int strSizeY, out Point p)
	    {
            var posList = new List<Point>();
            for (var i = 1; i < this.OutputImgWidth - strSizeX; i++)
	        {
	            for (var j = 1; j < this.OutputImgHeight - strSizeY; j++)
	            {
	                if (GetArea(i, j, strSizeX, strSizeY) == 0)
	                {
                        posList.Add(new Point(i, j));
                    }
                }
	        }
            if (posList.Count == 0)
            {
                p = new Point();
                return false;
            }
            else
            {
                var randomIndex = this.Rand.Next(0, posList.Count - 1);
                p = posList[randomIndex];
                return true;
            }
        }
	}
}
