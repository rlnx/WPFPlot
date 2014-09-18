using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace WPFPlot.Controls
{
	[ContentProperty("Items")]
	public abstract class GraphControlBase : Control
	{

		private GraphControlItems mInternalItems;


		public GraphControlBase()
		{
			mInternalItems = new GraphControlItems();
			AddVisualChild(mInternalItems);
			AddLogicalChild(mInternalItems);

			Items = new GraphItemCollection();
			mInternalItems.ItemsSource = Items;
			DataContextChanged += (s, e) =>
			{ mInternalItems.DataContext = e.NewValue; };
		}


		#region Items Property

		public static readonly DependencyProperty ItemsProperty = 
			DependencyProperty.Register(
			"Items",
			typeof(GraphItemCollection),
			typeof(GraphControlBase),
			new FrameworkPropertyMetadata(null, OnItemsPropertyChanged));

		private static void OnItemsPropertyChanged(DependencyObject dObj,
			DependencyPropertyChangedEventArgs e)
		{
			var oldValue = e.OldValue as GraphItemCollection;
			var newValue = e.NewValue as GraphItemCollection;
			var owner = (GraphControlBase)dObj;
			owner.OnItemsChanged(oldValue, newValue);
		}

		public GraphItemCollection Items
		{
			get { return (GraphItemCollection)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		#endregion


		protected virtual void OnItemsChanged(
			GraphItemCollection oldValue,
			GraphItemCollection newValue)
		{
			UnsubscribeFromItemsUpdating(oldValue);
			SubscribeToItemsUpdating(newValue);
			OnGraphItemsUpdated();
		}


		protected virtual void OnGraphItemsUpdated() { }


		protected override int VisualChildrenCount
		{ get { return 1; } }

		protected override Visual GetVisualChild(int index)
		{ return mInternalItems; }


		protected void EnableVisualInvalidation()
		{ mIsInvalidateVisualEnabled = true; }

		protected void DisableVisualInvalidation()
		{ mIsInvalidateVisualEnabled = false; }

		private bool mIsInvalidateVisualEnabled;
		protected void InvalidateVisualInternal()
		{
			if (mIsInvalidateVisualEnabled)
				InvalidateVisual();
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
