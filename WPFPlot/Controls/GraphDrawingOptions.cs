using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFPlot.Controls
{
	internal class GraphDrawingOptions
	{

		public GraphDrawingOptions() { }

		public Rect GraphRect { get; set; }
		public double From { get; set; }
		public double To { get; set; }

	}
}
