using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFPlot.Data
{
	public interface IPlotDataSource
	{
		event EventHandler PointsChanged;

		void SetSegment(double from, double to);
		void SetZoom(double zoom);

		Point Interpolate(double x);
		IList<Point> GetPoints();

		int GetStartIndex();
		int GetEndIndex();
	}
}
