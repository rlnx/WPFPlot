using System.Windows;

namespace WPFPlot.Helpers
{
	internal class PointTranslater
	{
		public PointTranslater() { }

		public Point Center { get; set; }
		public double Zoom { get; set; }


		public double TransalteX(double x)
		{ return Center.X + Zoom * x; }

		public double TranslateY(double y)
		{ return Center.Y - Zoom * y; }


		public Point Translate(Point point)
		{
			point.X = TransalteX(point.X);
			point.Y = TranslateY(point.Y);
			return point;
		}

		public double TranslateXBack(double x)
		{ return (x - Center.X) / Zoom; }

		public double TranslateYBack(double y)
		{ return (Center.Y - y) / Zoom; }

		public Point TranslateBack(Point point)
		{
			point.X = TranslateXBack(point.X);
			point.Y = TranslateYBack(point.Y);
			return point;
		}

		public double TranslateDist(double dist)
		{ return dist * Zoom; }

		public double TranslateDistBack(double dist)
		{ return dist / Zoom; }

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
