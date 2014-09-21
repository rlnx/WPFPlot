using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFPlot.Data;
using WPFPlot.Helpers;

namespace WPFPlot.Controls
{
	public class GraphItem : Animatable
	{

		private const double BOUND_POINT_RADOUS = 3d;

		internal event EventHandler<GraphItemUpdatedEventArgs> Updated;
		internal event EventHandler SegmentChanged;

		private bool mIsTracingBoxAdded;

		public GraphItem() { }


		#region Dependency Properties

		#region PlotData Property

		public static readonly DependencyProperty PlotDataProperty = 
			DependencyProperty.Register(
			"PlotData",
			typeof(IPlotDataSource),
			typeof(GraphItem),
			new FrameworkPropertyMetadata(null, OnPlotDataPropertyChanged));

		private static void OnPlotDataPropertyChanged(DependencyObject dObj,
			DependencyPropertyChangedEventArgs e)
		{
			var oldValue = e.OldValue as IPlotDataSource;
			var newValue = e.NewValue as IPlotDataSource;
			var owner = (GraphItem)dObj;
			owner.OnPlotDataChanged(oldValue, newValue);
		}

		public IPlotDataSource PlotData
		{
			get { return (IPlotDataSource)GetValue(PlotDataProperty); }
			set { SetValue(PlotDataProperty, value); }
		}

		#endregion

		#region StrokeBrush Property

		public static readonly DependencyProperty StrokeBrushProperty = 
			DependencyProperty.Register(
			"StrokeBrush",
			typeof(Brush),
			typeof(GraphItem),
			new FrameworkPropertyMetadata(null, RaiseUpdateEventOnPropertyChanged));

		public Brush StrokeBrush
		{
			get { return (Brush)GetValue(StrokeBrushProperty); }
			set { SetValue(StrokeBrushProperty, value); }
		}

		#endregion

		#region StrokeThickness Property

		public static readonly DependencyProperty StrokeThicknessProperty = 
			DependencyProperty.Register(
			"StrokeThickness",
			typeof(double),
			typeof(GraphItem),
			new FrameworkPropertyMetadata(1.0d, RaiseUpdateEventOnPropertyChanged));

		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		#endregion

		#region IsTracingEnabled Property

		public static readonly DependencyProperty IsTracingEnabledProperty = 
			DependencyProperty.Register(
			"IsTracingEnabled",
			typeof(bool),
			typeof(GraphItem),
			new FrameworkPropertyMetadata(false, OnIsTracingEnabledPropertyChanged));

		private static void OnIsTracingEnabledPropertyChanged(DependencyObject dObj,
			DependencyPropertyChangedEventArgs e)
		{
			var oldValue = (bool)e.OldValue;
			var newValue = (bool)e.NewValue;
			var owner = (GraphItem)dObj;
			owner.OnIsTracingEnabledChanged(oldValue, newValue);
		}

		public bool IsTracingEnabled
		{
			get { return (bool)GetValue(IsTracingEnabledProperty); }
			set { SetValue(IsTracingEnabledProperty, value); }
		}

		#endregion

		#region SegmentBegin Property

		public static readonly DependencyProperty SegmentBeginProperty = 
			DependencyProperty.Register(
			"SegmentBegin",
			typeof(double),
			typeof(GraphItem),
			new FrameworkPropertyMetadata(Double.NegativeInfinity,
				OnSegmentBeginPropertyChanged));

		private static void OnSegmentBeginPropertyChanged(DependencyObject dObj,
			DependencyPropertyChangedEventArgs e)
		{
			var owner = (GraphItem)dObj;
			owner.RaiseUpdatedEvent(GraphItemUpdateOptions.PropertiesChanged);
			owner.RaiseSegmentChangedEvent();
		}

		public double SegmentBegin
		{
			get { return (double)GetValue(SegmentBeginProperty); }
			set { SetValue(SegmentBeginProperty, value); }
		}

		#endregion

		#region SegmentEnd Property

		public static readonly DependencyProperty SegmentEndProperty = 
			DependencyProperty.Register(
			"SegmentEnd",
			typeof(double),
			typeof(GraphItem),
			new FrameworkPropertyMetadata(Double.PositiveInfinity,
				OnSegmentEndPropertyChanged));

		private static void OnSegmentEndPropertyChanged(DependencyObject dObj,
			DependencyPropertyChangedEventArgs e)
		{
			var owner = (GraphItem)dObj;
			owner.RaiseUpdatedEvent(GraphItemUpdateOptions.PropertiesChanged);
			owner.RaiseSegmentChangedEvent();
		}

		public double SegmentEnd
		{
			get { return (double)GetValue(SegmentEndProperty); }
			set { SetValue(SegmentEndProperty, value); }
		}

		#endregion


		private static void RaiseUpdateEventOnPropertyChanged(
			DependencyObject dObj, DependencyPropertyChangedEventArgs e)
		{
			var owner = (GraphItem)dObj;
			owner.RaiseUpdatedEvent(GraphItemUpdateOptions.PropertiesChanged);
		}

		#endregion

		private GraphControl mParentGraph;
		internal GraphControl ParentGraph
		{
			get { return mParentGraph; }
			set
			{
				GraphControl oldParent = mParentGraph;
				mParentGraph = value;
				if (mParentGraph == null)
					return;

				if (mIsTracingBoxAdded && oldParent != null)
					oldParent.RemoveTracingBox(TracingBox);

				if (IsTracingEnabled)
					mParentGraph.AddTracingBox(TracingBox);
			}
		}

		private TracingBox mTracingBox;
		private TracingBox TracingBox
		{
			get
			{
				if (mTracingBox == null)
					mTracingBox = new TracingBox(this);

				return mTracingBox;
			}
		}


		protected virtual void OnPlotDataChanged(
			IPlotDataSource oldValue, IPlotDataSource newValue)
		{
			UnsubscribeFromPlotDataChanging(oldValue);
			SubscribeToPlotDataUpdating(newValue);
			RaiseUpdatedEvent(GraphItemUpdateOptions.DataSourceChanged);
		}

		protected virtual void OnIsTracingEnabledChanged(
			bool oldValue, bool newValue)
		{
			if (ParentGraph == null)
				return;

			mIsTracingBoxAdded = newValue;

			if (oldValue)
				ParentGraph.RemoveTracingBox(TracingBox);

			if (newValue)
				ParentGraph.AddTracingBox(TracingBox);
		}

		protected override Freezable CreateInstanceCore()
		{ return new GraphItem(); }



		internal void Draw(DrawingContext dc, PointTranslater pt,
			GraphDrawingOptions op)
		{
			IPlotDataSource plotData = PlotData;
			if (plotData == null)
				return;

			double from = Math.Max(op.From, SegmentBegin);
			double to = Math.Min(op.To, SegmentEnd);
			if (from >= to)
				return;

			plotData.SetZoom(pt.Zoom);
			plotData.SetSegment(from, to);
			var pointsToDraw = plotData.GetPoints();
			if (pointsToDraw.Count <= 1)
				return;

			var graphPen = new Pen(StrokeBrush, StrokeThickness);
			int startIndex = plotData.GetStartIndex();
			if (startIndex < 0)
				return;

			Rect rect = op.GraphRect;
			rect.X = pt.TransalteX(from);
			rect.Width = pt.TransalteX(to) - rect.X;
			Point next = pt.Translate(pointsToDraw[startIndex]);

			var boundPoint = plotData.Interpolate(from);
			boundPoint = pt.Translate(boundPoint);
			dc.DrawLine(graphPen, boundPoint, next);
			if (rect.Left > op.GraphRect.Left)
			{
				dc.DrawEllipse(StrokeBrush, null, boundPoint,
					BOUND_POINT_RADOUS, BOUND_POINT_RADOUS);
			}

			int endIndex = plotData.GetEndIndex();
			for (int i = startIndex; i <= endIndex; i++)
			{
				Point prev = next;
				next = pt.Translate(pointsToDraw[i]);
				if (!(rect.Contains(prev) || rect.Contains(next)))
					continue;

				dc.DrawLine(graphPen, prev, next);
			}

			boundPoint = plotData.Interpolate(to);
			boundPoint = pt.Translate(boundPoint);
			dc.DrawLine(graphPen, next, boundPoint);
			if (rect.Right < op.GraphRect.Right)
			{
				dc.DrawEllipse(StrokeBrush, null, boundPoint,
					BOUND_POINT_RADOUS, BOUND_POINT_RADOUS);
			}
		}




		private void SubscribeToPlotDataUpdating(IPlotDataSource source)
		{
			if (source == null)
				return;

			source.PointsChanged += PlotDataChanged;
		}

		private void UnsubscribeFromPlotDataChanging(IPlotDataSource source)
		{
			if (source == null)
				return;

			source.PointsChanged -= PlotDataChanged;
		}

		private void PlotDataChanged(object sender, EventArgs e)
		{ RaiseUpdatedEvent(GraphItemUpdateOptions.DataSourceUpdated); }


		private void RaiseUpdatedEvent(GraphItemUpdateOptions options)
		{
			if (Updated != null)
			{
				var args = new GraphItemUpdatedEventArgs(options);
				Updated(this, args);
			}
		}

		private void RaiseSegmentChangedEvent()
		{
			if (SegmentChanged != null)
				SegmentChanged(this, EventArgs.Empty);
		}

	}
}
