using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPFPlot.Data;
using WPFPlot.Helpers;

namespace WPFPlot.Controls
{
	public class GraphControl : GraphControlBase
	{

		static GraphControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphControl),
				new FrameworkPropertyMetadata(typeof(GraphControl)));
		}

		private const double ZOOM_MIN = 20d;
		private const double ZOOM_MAX = 500d;
		private const double ZOOM_DEFAULT = (ZOOM_MAX + ZOOM_MIN) / 2d;
		private const double AXIS_DIGITS_MARGIN = 3d;
		private const int    AXIS_DIGITS_NUMBERS = 4;

		private PointTranslater mPointTrans;
		private List<TracingBox> mTraceBoxes;

		private Point mCurrentCursorPos;
		private Point mCapturePoint;
		private Point mCaptureCenter;
		private Vector mLastCenterTrans;


		public GraphControl()
		{
			mTraceBoxes = new List<TracingBox>();
			mPointTrans = new PointTranslater() { Zoom = ZOOM_DEFAULT };
		}


		#region Dependency Properties

		#region InternalMargins Property

		public static readonly DependencyProperty InternalMarginsProperty =
			DependencyProperty.Register(
			"InternalMargins",
			typeof(Thickness),
			typeof(GraphControl),
			new FrameworkPropertyMetadata(new Thickness(0),
				FrameworkPropertyMetadataOptions.AffectsRender));

		public Thickness InternalMargins
		{
			get { return (Thickness)GetValue(InternalMarginsProperty); }
			set { SetValue(InternalMarginsProperty, value); }
		}

		#endregion

		#region GridMinStep Property

		public static readonly DependencyProperty GridMinStepProperty =
			DependencyProperty.Register(
			"GridMinStep",
			typeof(double),
			typeof(GraphControl),
				new FrameworkPropertyMetadata(50d,
					FrameworkPropertyMetadataOptions.AffectsRender));

		public double GridMinStep
		{
			get { return (double)GetValue(GridMinStepProperty); }
			set { SetValue(GridMinStepProperty, value); }
		}

		#endregion

		#region SegmentBegin Property

		public static readonly DependencyProperty SegmentBeginProperty =
			DependencyProperty.Register(
			"SegmentBegin",
			typeof(double),
			typeof(GraphControl),
			new FrameworkPropertyMetadata(Double.NegativeInfinity,
				FrameworkPropertyMetadataOptions.AffectsRender));

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
			typeof(GraphControl),
			new FrameworkPropertyMetadata(Double.PositiveInfinity,
				FrameworkPropertyMetadataOptions.AffectsRender));

		public double SegmentEnd
		{
			get { return (double)GetValue(SegmentEndProperty); }
			set { SetValue(SegmentEndProperty, value); }
		}

		#endregion

		#region GridBrush Property

		public static readonly DependencyProperty GridBrushProperty =
			DependencyProperty.Register(
			"GridBrush",
			typeof(Brush),
			typeof(GraphControl),
			new FrameworkPropertyMetadata(null,
				FrameworkPropertyMetadataOptions.AffectsRender));

		public Brush GridBrush
		{
			get { return (Brush)GetValue(GridBrushProperty); }
			set { SetValue(GridBrushProperty, value); }
		}
		#endregion

		#region GridThickness Property

		public static readonly DependencyProperty GridThicknessProperty =
			DependencyProperty.Register(
			"GridThickness",
			typeof(double),
			typeof(GraphControl),
			new FrameworkPropertyMetadata(1d,
				FrameworkPropertyMetadataOptions.AffectsRender));

		public double GridThickness
		{
			get { return (double)GetValue(GridThicknessProperty); }
			set { SetValue(GridThicknessProperty, value); }
		}

		#endregion

		#region AxisBrush Property

		public static readonly DependencyProperty AxisBrushProperty =
			DependencyProperty.Register(
			"AxisBrush",
			typeof(Brush),
			typeof(GraphControl),
			new FrameworkPropertyMetadata(null,
				FrameworkPropertyMetadataOptions.AffectsRender));

		public Brush AxisBrush
		{
			get { return (Brush)GetValue(AxisBrushProperty); }
			set { SetValue(AxisBrushProperty, value); }
		}

		#endregion

		#region AxisThickness Property

		public static readonly DependencyProperty AxisThicknessProperty =
			DependencyProperty.Register(
			"AxisThickness",
			typeof(double),
			typeof(GraphControl),
			new FrameworkPropertyMetadata(1d,
				FrameworkPropertyMetadataOptions.AffectsRender));

		public double AxisThickness
		{
			get { return (double)GetValue(AxisThicknessProperty); }
			set { SetValue(AxisThicknessProperty, value); }
		}

		#endregion

		#region IsGridEnabled Property

		public static readonly DependencyProperty IsGridEnabledProperty =
			DependencyProperty.Register(
			"IsGridEnabled",
			typeof(bool),
			typeof(GraphControl),
			new FrameworkPropertyMetadata(true,
				FrameworkPropertyMetadataOptions.AffectsRender));

		public bool IsGridEnabled
		{
			get { return (bool)GetValue(IsGridEnabledProperty); }
			set { SetValue(IsGridEnabledProperty, value); }
		}

		#endregion

		#region IsDragingEnabled Property

		public static readonly DependencyProperty IsDraggingEnabledProperty =
			DependencyProperty.Register(
			"IsDraggingEnabled",
			typeof(bool),
			typeof(GraphControl),
			new FrameworkPropertyMetadata(true));

		public bool IsDraggingEnabled
		{
			get { return (bool)GetValue(IsDraggingEnabledProperty); }
			set { SetValue(IsDraggingEnabledProperty, value); }
		}

		#endregion


		#region Zoom Property

		public static readonly DependencyProperty ZoomProperty =
			DependencyProperty.Register(
			"Zoom",
			typeof(double),
			typeof(GraphControl),
				new FrameworkPropertyMetadata(ZOOM_DEFAULT, OnZoomPropertyChanged));

		private static void OnZoomPropertyChanged(DependencyObject dObj,
			DependencyPropertyChangedEventArgs e)
		{
			var oldValue = (double)e.OldValue;
			var newValue = (double)e.NewValue;
			var owner = (GraphControl)dObj;
			owner.OnZoomChanged(oldValue, newValue);
		}
		public double Zoom
		{
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}

		#endregion

		#region Center Property

		private static readonly DependencyProperty CenterProperty = 
			DependencyProperty.Register(
			"Center",
			typeof(Point),
			typeof(GraphControl),
			new FrameworkPropertyMetadata(new Point(), OnCenterPropertyChanged));

		private static void OnCenterPropertyChanged(DependencyObject dObj,
			DependencyPropertyChangedEventArgs e)
		{
			var oldValue = (Point)e.OldValue;
			var newValue = (Point)e.NewValue;
			var owner = (GraphControl)dObj;
			owner.OnCenterChanged(oldValue, newValue);
		}

		private Point Center
		{
			get { return (Point)GetValue(CenterProperty); }
			set { SetValue(CenterProperty, value); }
		}

		#endregion

		#endregion


		protected virtual void OnZoomChanged(double oldValue, double newValue)
		{
			Contract.Requires(newValue >= 0d);
			Contract.Requires(newValue <= 1d);
			mPointTrans.Zoom = ZOOM_MIN + newValue * (ZOOM_MAX - ZOOM_MIN);
			InvalidateVisual();
		}

		protected virtual void OnCenterChanged(Point oldValue, Point newValue)
		{
			mPointTrans.Center = newValue;
			InvalidateVisual();
		}

		protected override void PrepareItemOverride(GraphItem item)
		{
			base.PrepareItemOverride(item);
			item.ParentGraph = this;
		}

		protected override Visual GetVisualChild(int index)
		{
			int count = base.VisualChildrenCount;
			if (index < count)
				return base.GetVisualChild(index);

			return mTraceBoxes[index - count];
		}

		protected override int VisualChildrenCount
		{ get { return base.VisualChildrenCount + mTraceBoxes.Count; } }


		protected override Size MeasureOverride(Size constraint)
		{
			IsInvalidateVisualEnabled = false;
			Point notModifiedCenter = GetNotModifiedCenter(constraint);
			Center = notModifiedCenter + mLastCenterTrans;

			foreach (UIElement item in Items)
			{
				var fe = item as FrameworkElement;
				if (fe != null)
				{
					fe.VerticalAlignment = System.Windows.VerticalAlignment.Top;
					fe.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
				}
				item.Measure(constraint);
			}

			foreach (var box in mTraceBoxes)
				box.Measure(constraint);

			IsInvalidateVisualEnabled = true;
			base.MeasureOverride(constraint);
			return constraint;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var rect = new Rect(new Point(), arrangeBounds);

			foreach (UIElement item in Items)
				ArrangeElement(item, ref rect);

			foreach (var box in mTraceBoxes)
				ArrangeElement(box, ref rect);

			return arrangeBounds;
		}

		protected override void OnGraphItemsUpdated(
			GraphItemUpdateOptions options)
		{
			base.OnGraphItemsUpdated(options);

			switch (options)
			{
				case GraphItemUpdateOptions.DataSourceChanged:
					InvalidateVisual();
					break;
				case GraphItemUpdateOptions.DataSourceUpdated:
					RearrangeTracingBoxes();
					InvalidateVisual();
					break;
				case GraphItemUpdateOptions.PropertiesChanged:
					InvalidateVisual();
					break;
			}
		}

		protected override void OnRender(DrawingContext dc)
		{
			if (!IsInvalidateVisualEnabled)
				return;

			base.OnRender(dc);

			Rect recnderRect = new Rect(0, 0, ActualWidth, ActualHeight);
			dc.DrawRectangle(Background ?? Brushes.Transparent, null, recnderRect);

			Rect clipRect = GetGraphRectangle();
			var clipGeometry= new RectangleGeometry(clipRect);
			dc.PushClip(clipGeometry);

			RenderGrid(dc);
			RenderAxis(dc);
			RenderPlotData(dc);
		}


		#region Mouse Handling

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (IsDraggingEnabled == false)
				return;

			if (CaptureMouse())
			{
				mCapturePoint = e.GetPosition(this);
				mCaptureCenter = Center;
				Cursor = Cursors.SizeAll;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			mCurrentCursorPos = e.GetPosition(this);

			if (IsMouseCaptured)
			{
				Vector realitiveTrans = mCurrentCursorPos - mCapturePoint;
				Center = mCaptureCenter + realitiveTrans;
			}
			else
			{ RearrangeTracingBoxes(); }
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);
			if (IsMouseCaptured)
			{
				ReleaseMouseCapture();
				Point notModCenter = GetNotModifiedCenter(RenderSize);
				mLastCenterTrans = Center - notModCenter;
				Cursor = Cursors.Arrow;
			}
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			HideTracingBoxes();
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			ShowTracingBoxes();
		}

		#endregion

		#region Graph Rendering

		private void RenderGrid(DrawingContext dc)
		{
			if (IsGridEnabled == false)
				return;

			double step = CalculateGridStep();
			RenderGridX(dc, step);
			RenderGridY(dc, step);
		}

		private void RenderGridX(DrawingContext dc, double step)
		{
			double left = GetLeftGraphBound();
			double right = GetRightGraphBound();
			var typeface = new Typeface(FontFamily, FontStyle,
				FontWeight, FontStretch);

			var gridPen = new Pen(GridBrush, GridThickness);
			for (double x = Center.X - step; x > left; x -= step)
			{
				RenderVerticalLine(dc, gridPen, x);
				RenderDigitX(dc, x, typeface);
			}

			for (double x = Center.X + step; x < right; x += step)
			{
				RenderVerticalLine(dc, gridPen, x);
				RenderDigitX(dc, x, typeface);
			}

			RenderDigitX(dc, Center.X, typeface);
		}


		private void RenderGridY(DrawingContext dc, double step)
		{
			double top = GetTopGraphBound();
			double bottom = GetBottomGraphBound();
			var typeface = new Typeface(FontFamily, FontStyle,
				FontWeight, FontStretch);

			var gridPen = new Pen(GridBrush, GridThickness);
			for (double y = Center.Y - step; y > top; y -= step)
			{
				RenderHorizontalLine(dc, gridPen, y);
				RenderDigitY(dc, y, typeface);

			}
			for (double y = Center.Y + step; y < bottom; y += step)
			{
				RenderHorizontalLine(dc, gridPen, y);
				RenderDigitY(dc, y, typeface);
			}
		}

		private void RenderAxis(DrawingContext dc)
		{
			var axisPen = new Pen(AxisBrush, AxisThickness);
			RenderHorizontalLine(dc, axisPen, Center.Y);
			RenderVerticalLine(dc, axisPen, Center.X);
		}

		private void RenderVerticalLine(DrawingContext dc, Pen pen, double x)
		{
			double startY = GetTopGraphBound();
			double endY = GetBottomGraphBound();
			Point startPoint = new Point(x, startY);
			Point endPoint = new Point(x, endY);
			dc.DrawLine(pen, startPoint, endPoint);
		}

		private void RenderHorizontalLine(DrawingContext dc, Pen pen, double y)
		{
			double startX = GetLeftGraphBound();
			double endX = GetRightGraphBound();
			Point startPoint = new Point(startX, y);
			Point endPoint = new Point(endX, y);
			dc.DrawLine(pen, startPoint, endPoint);
		}

		private void RenderDigitX(DrawingContext dc, double x, Typeface typeface)
		{
			double realX = mPointTrans.TranslateXBack(x);
			realX = Math.Round(realX, AXIS_DIGITS_NUMBERS);

			CultureInfo uiCulture = CultureInfo.CurrentUICulture;
			var formattedText = new FormattedText(realX.ToString(),
				uiCulture, FlowDirection, typeface, FontSize, Foreground);

			dc.DrawText(formattedText, new Point(x + AXIS_DIGITS_MARGIN,
				Center.Y + AXIS_DIGITS_MARGIN));
		}

		private void RenderDigitY(DrawingContext dc, double y, Typeface typeface)
		{
			double realY = mPointTrans.TranslateYBack(y);
			realY = Math.Round(realY, AXIS_DIGITS_NUMBERS);

			CultureInfo uiCulture = CultureInfo.CurrentUICulture;
			var formattedText = new FormattedText(realY.ToString(),
				uiCulture, FlowDirection, typeface, FontSize, Foreground);

			dc.DrawText(formattedText, new Point(Center.X + AXIS_DIGITS_MARGIN,
				y + AXIS_DIGITS_MARGIN));
		}

		private void RenderPlotData(DrawingContext dc)
		{
			if (GraphItems == null)
				return;

			double left = GetLeftGraphBound();
			double right = GetRightGraphBound();
			GraphDrawingOptions options = new GraphDrawingOptions()
			{
				From = mPointTrans.TranslateXBack(left),
				To = mPointTrans.TranslateXBack(right),
				GraphRect = GetGraphRectangle()
			};

			foreach (var item in GraphItems)
				item.Draw(dc, mPointTrans, options);
		}

		#endregion

		#region Tracing Boxes

		private void HideTracingBoxes()
		{
			foreach (var box in mTraceBoxes)
				box.Hide();
		}

		private void ShowTracingBoxes()
		{
			foreach (var box in mTraceBoxes)
				box.Show();
		}

		private void RearrangeTracingBoxes()
		{
			double realX = mPointTrans.TranslateXBack(mCurrentCursorPos.X);
			foreach (var box in mTraceBoxes)
				box.UpdatePosition(realX);

			InvalidateArrange();
		}

		#endregion

		internal void AddTracingBox(TracingBox box)
		{
			mTraceBoxes.Add(box);
			AddVisualChild(box);
			AddLogicalChild(box);

			double realX = mPointTrans.TranslateXBack(mCurrentCursorPos.X);
			box.UpdatePosition(realX);
			InvalidateMeasure();
		}

		internal void RemoveTracingBox(TracingBox box)
		{
			mTraceBoxes.Remove(box);
			RemoveVisualChild(box);
			RemoveLogicalChild(box);
			InvalidateArrange();
		}


		private void ArrangeElement(UIElement element, ref Rect rect)
		{
			double x = GraphControl.GetBindPointX(element);
			double y = GraphControl.GetBindPointY(element);
			var bindPoint = new Point(x, y);
			rect.Location = mPointTrans.Translate(bindPoint);
			element.Arrange(rect);
		}

		private Point GetNotModifiedCenter(Size constraint)
		{
			double width = constraint.Width -
								InternalMargins.Left -
								InternalMargins.Right;
			double height = constraint.Height -
								 InternalMargins.Top -
								 InternalMargins.Bottom;
			double centerX = InternalMargins.Left + width / 2d;
			double centerY = InternalMargins.Top + height / 2d;
			return new Point(centerX, centerY);
		}

		private double GetLeftGraphBound()
		{ return InternalMargins.Left; }

		private double GetRightGraphBound()
		{ return ActualWidth - InternalMargins.Right; }

		private double GetTopGraphBound()
		{ return InternalMargins.Top; }

		private double GetBottomGraphBound()
		{ return ActualHeight - InternalMargins.Bottom; }

		private Rect GetGraphRectangle()
		{
			var rectRect = new Rect();
			rectRect.X = GetLeftGraphBound();
			rectRect.Y = GetTopGraphBound();
			rectRect.Width = GetRightGraphBound() - rectRect.X;
			rectRect.Height = GetBottomGraphBound() - rectRect.Y;
			return rectRect;
		}

		private double CalculateGridStep()
		{
			double oneLength = mPointTrans.TranslateDist(1d);
			if (oneLength > GridMinStep)
			{
				while (oneLength > GridMinStep)
					oneLength /= 2d;

				return oneLength * 2d;
			}
			else if (oneLength < GridMinStep)
			{
				while (oneLength < GridMinStep)
					oneLength *= 2d;

				return oneLength;
			}
			return oneLength;
		}



	}

}
