using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFPlot.Data;

namespace WPFPlot.Controls
{
	public class GraphItem : Animatable
	{
		internal event EventHandler Updated;

		public GraphItem() { }


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
			new FrameworkPropertyMetadata(null, OnStrokeBrushPropertyChanged));

		private static void OnStrokeBrushPropertyChanged(DependencyObject dObj,
			DependencyPropertyChangedEventArgs e)
		{
			var oldValue = e.OldValue as Brush;
			var newValue = e.NewValue as Brush;
			var owner = (GraphItem)dObj;
			owner.OnStrokeBrushChanged(oldValue, newValue);
		}

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
			new FrameworkPropertyMetadata(1.0d, OnStrokeThicknessPropertyChanged));

		private static void OnStrokeThicknessPropertyChanged(DependencyObject dObj,
			DependencyPropertyChangedEventArgs e)
		{
			var oldValue = (double)e.OldValue;
			var newValue = (double)e.NewValue;
			var owner = (GraphItem)dObj;
			owner.OnStrokeThickenssChanged(oldValue, newValue);
		}

		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		#endregion


		protected virtual void OnPlotDataChanged(
			IPlotDataSource oldValue, IPlotDataSource newValue)
		{
			UnsubscribeFromPlotDataChanging(oldValue);
			SubscribeToPlotDataUpdating(newValue);
			RaiseUpdatedEvent();
		}

		protected virtual void OnStrokeBrushChanged(
			Brush oldValue, Brush newValue)
		{ RaiseUpdatedEvent(); }

		protected virtual void OnStrokeThickenssChanged(
			double oldValue, double newValue)
		{ RaiseUpdatedEvent(); }



		protected override Freezable CreateInstanceCore()
		{ return new GraphItem(); }


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
		{ RaiseUpdatedEvent(); }

		private void RaiseUpdatedEvent()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}


	}
}
