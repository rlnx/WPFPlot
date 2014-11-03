using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WPFPlot.Data;

namespace WPFPlot.Example.ViewModel
{
	public class DynamicPlotDataSource : IPlotDataSource
	{
		public event EventHandler PointsChanged;

		private List<Point> mComputedPoints;
		private Func<double, double, double> mFunc;
		private DispatcherTimer mTimer;
		private double mTParam;

		public DynamicPlotDataSource()
		{
			mTimer = new DispatcherTimer();
			mTimer.Interval = TimeSpan.FromMilliseconds(10);
			mTimer.Tick += mTimer_Tick;
			mTimer.Start();

			mFunc = (x, p) => Math.Sin(x - p);
		}

		private void mTimer_Tick(object sender, EventArgs e)
		{
			mTParam += 0.05;
			if (mTParam > 2 * Math.PI)
				mTParam = 0;

			if (PointsChanged != null)
				PointsChanged(this, EventArgs.Empty);
		}

		public void SetSegment(double from, double to)
		{
			mComputedPoints = new List<Point>();
			for (double x = from; x < to; x += 0.1)
				mComputedPoints.Add(Interpolate(x));

			mComputedPoints.Add(Interpolate(to));
		}

		public void SetZoom(double zoom) { }

		public Point Interpolate(double x)
		{ return new Point(x, GetFValue(x)); }

		public IList<Point> GetPoints()
		{ return mComputedPoints; }

		public int GetStartIndex()
		{ return 0; }

		public int GetEndIndex()
		{ return mComputedPoints.Count - 1; }


		private double GetFValue(double x)
		{ return mFunc(x, mTParam); }



      public double GetSegmentBegin()
      { return Double.NegativeInfinity; }

      public double GetSegmentEnd()
      { return Double.PositiveInfinity; }

   }
}
