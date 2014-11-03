using System.Windows;

namespace WPFPlot.Helpers
{
	internal class PointTranslater
	{
		public PointTranslater() { }

		public Point Center { get; set; }
		public double ZoomX { get; set; }
      public double ZoomY { get; set; }

		public double TransalteX(double x)
		{ return Center.X + ZoomX * x; }

		public double TranslateY(double y)
		{ return Center.Y - ZoomY * y; }


		public Point Translate(Point point)
		{
			point.X = TransalteX(point.X);
			point.Y = TranslateY(point.Y);
			return point;
		}

		public double TranslateXBack(double x)
		{ return (x - Center.X) / ZoomX; }

		public double TranslateYBack(double y)
		{ return (Center.Y - y) / ZoomY; }

		public Point TranslateBack(Point point)
		{
			point.X = TranslateXBack(point.X);
			point.Y = TranslateYBack(point.Y);
			return point;
		}

		public double TranslateDistX(double dist)
		{ return dist * ZoomX; }

		public double TranslateDistXBack(double dist)
		{ return dist / ZoomX; }

      public double TranslateDistY(double dist)
      { return dist * ZoomY; }

      public double TranslateDistYBack(double dist)
      { return dist / ZoomY; }


		public Rect TranslateBack(Rect rect)
		{
			Point topLeft = rect.TopLeft;
			Point bottomRight = rect.BottomRight;
			topLeft = TranslateBack(topLeft);
			bottomRight = TranslateBack(bottomRight);
			return new Rect(topLeft, bottomRight);
		}
	}

}
