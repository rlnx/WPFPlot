using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace WPFPlot.Controls
{
	[ContentProperty("GraphItems")]
	public abstract class GraphControlBase : Control
	{

		private GraphControlItems mInternalItems;


		public GraphControlBase()
		{
			Items = new UIElementCollection(this, this);

			mInternalItems = new GraphControlItems();
			AddLogicalChild(mInternalItems);
			AddVisualChild(mInternalItems);

			GraphItems = new GraphItemCollection();
			SubscribeToItemsUpdating(GraphItems);
			mInternalItems.ItemsSource = GraphItems;

			DataContextChanged += (s, e) =>
			{ mInternalItems.DataContext = e.NewValue; };
		}


		#region Attached Properties

		public static readonly DependencyProperty BindPointXProperty = 
			DependencyProperty.RegisterAttached(
			"BindPointX", 
			typeof(double), 
			typeof(GraphControlBase),
			new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange));

		public static double GetBindPointX(UIElement element)
		{ return (double)element.GetValue(BindPointXProperty); }

		public static void SetBindPointX(UIElement element, double x)
		{
			element.SetValue(BindPointXProperty, x);
			InvalidateParentArrange(element);
		}

		public static readonly DependencyProperty BindPointYProperty = 
			DependencyProperty.RegisterAttached(
			"BindPointY",
			typeof(double),
			typeof(GraphControlBase),
			new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange));

		public static double GetBindPointY(UIElement element)
		{ return (double)element.GetValue(BindPointYProperty); }

		public static void SetBindPointY(UIElement element, double y)
		{
			element.SetValue(BindPointYProperty, y);
			InvalidateParentArrange(element);
		}

		private static void InvalidateParentArrange(UIElement element)
		{
			var fElement = element as FrameworkElement;
			if (fElement == null)
				return;

			var parentGraph = fElement.Parent as GraphControlBase;
			if (parentGraph == null)
				return;

			parentGraph.InvalidateArrange();
		}

		#endregion


		public GraphItemCollection GraphItems { get; private set; }
		public UIElementCollection Items { get; private set; }

		protected bool IsInvalidateVisualEnabled { get; set; }
		
		protected virtual void OnGraphItemsChanged(
			GraphItemCollection oldValue,
			GraphItemCollection newValue)
		{
			UnsubscribeFromItemsUpdating(oldValue);
			SubscribeToItemsUpdating(newValue);
			OnGraphItemsUpdated();
		}


		protected virtual void OnGraphItemsUpdated() { }


		protected override int VisualChildrenCount
		{ get { return Items.Count + 1; } }

		protected override Visual GetVisualChild(int index)
		{
			if (index == 0)
				return mInternalItems;

			return Items[index - 1]; 
		}






		private void SubscribeToItemsUpdating(GraphItemCollection collection)
		{
			if (collection == null)
				return;

			collection.CollectionChanged += GraphItemsCollectionChanged;
			foreach (var item in collection)
				item.Updated += ItemUpdated;
		}

		private void UnsubscribeFromItemsUpdating(GraphItemCollection collection)
		{
			if (collection == null)
				return;

			collection.CollectionChanged -= GraphItemsCollectionChanged;
			foreach (var item in collection)
				item.Updated -= ItemUpdated;
		}

		private void GraphItemsCollectionChanged(object sender,
			NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (var o in e.NewItems)
						((GraphItem)o).Updated += ItemUpdated;
					break;

				case NotifyCollectionChangedAction.Replace:
					foreach (var o in e.NewItems)
						((GraphItem)o).Updated += ItemUpdated;
					foreach (var o in e.OldItems)
						((GraphItem)o).Updated -= ItemUpdated;
					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (var o in e.OldItems)
						((GraphItem)o).Updated -= ItemUpdated;
					break;

				case NotifyCollectionChangedAction.Reset:
					var items = sender as GraphItemCollection;
					foreach (var o in items)
					{
						((GraphItem)o).Updated -= ItemUpdated;
						((GraphItem)o).Updated += ItemUpdated;
					}
					break;
			}
		}

		private void ItemUpdated(object sender, EventArgs e)
		{ OnGraphItemsUpdated(); }

	}
}
