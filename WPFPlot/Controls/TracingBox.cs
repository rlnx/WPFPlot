using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFPlot.Data;

namespace WPFPlot.Controls
{
	public class TracingBox : Control
	{

		private const int TEXT_DOUBLE_PRECISION = 4;

		static TracingBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TracingBox),
				new FrameworkPropertyMetadata(typeof(TracingBox)));
		}

		private double mXCoordinate;
		private bool mIsHidden;

		internal TracingBox(GraphItem item)
		{
			Contract.Requires(item != null);

			mXCoordinate = Double.NegativeInfinity;
			VerticalAlignment = System.Windows.VerticalAlignment.Top;
			HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
			IsHitTestVisible = false;
			GraphItem = item;

			Hide();
		}


		#region XValue Property

		public static readonly DependencyProperty XValueProperty = 
			DependencyProperty.Register(
			"XValue",
			typeof(string),
			typeof(TracingBox));

		public string XValue
		{
			get { return (string)GetValue(XValueProperty); }
			set { SetValue(XValueProperty, value); }
		}

		#endregion

		#region YValue Property

		public static readonly DependencyProperty YValueProperty = 
			DependencyProperty.Register(
			"YValue",
			typeof(string),
			typeof(TracingBox));

		public string YValue
		{
			get { return (string)GetValue(YValueProperty); }
			set { SetValue(YValueProperty, value); }
		}

		#endregion


		private GraphItem mGraphItem;
		public GraphItem GraphItem 
		{
			get { return mGraphItem; }
			private set
			{
				mGraphItem = value;
				mGraphItem.SegmentChanged += GraphItem_SegmentChanged;
			}
		}

		private void GraphItem_SegmentChanged(object sender, EventArgs e)
		{ UpdateVisibility(); }

		private bool IsValueInSegment(double x)
		{ return mGraphItem.SegmentBegin < x && x < mGraphItem.SegmentEnd; }

		private bool UpdateVisibility()
		{
			bool isVisible = IsValueInSegment(mXCoordinate) && !mIsHidden;
			Visibility = (isVisible) ?
				System.Windows.Visibility.Visible :
				System.Windows.Visibility.Hidden;
			return isVisible;
		}


		internal void Hide()
		{
			mIsHidden = true;
			UpdateVisibility();
		}

		internal void Show()
		{
			if (mGraphItem.PlotData == null)
				return;

			if (mGraphItem.PlotData.GetPoints().Count < 2)
				return;

			mIsHidden = false;
			UpdateVisibility();
		}

		internal void UpdatePosition(double realX)
		{
			mXCoordinate = realX;
			IPlotDataSource plotSource = mGraphItem.PlotData;
			if (plotSource == null)
				return;

			if (!UpdateVisibility())
				return;

			var realPnt = plotSource.Interpolate(realX);
			XValue = Math.Round(realPnt.X, TEXT_DOUBLE_PRECISION).ToString();
			YValue = Math.Round(realPnt.Y, TEXT_DOUBLE_PRECISION).ToString();

			GraphControlBase.SetBindPointX(this, realPnt.X);
			GraphControlBase.SetBindPointY(this, realPnt.Y);
		}

	}
}
