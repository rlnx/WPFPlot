using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFPlot.Controls
{
	internal class GraphControlItems: ItemsControl
	{

		private List<FrameworkElement> mGeneratedContainers;

		public GraphControlItems()
		{
			mGeneratedContainers = new List<FrameworkElement>();
			DataContextChanged += GraphControlItemsDataContextChanged;
		}

		private void GraphControlItemsDataContextChanged(object sender, 
			DependencyPropertyChangedEventArgs e)
		{
			foreach (var fe in mGeneratedContainers)
				fe.DataContext = e.NewValue;
		}

		protected sealed override void PrepareContainerForItemOverride(
			DependencyObject element, object item)
		{
			if (item is GraphItem == false)
				throw new InvalidOperationException("Item has invalid type.");

			var fe = element as FrameworkElement;
			if (fe != null)
			{
				fe.DataContext = DataContext;
				mGeneratedContainers.Add(fe);
			}
		}

		protected sealed override DependencyObject GetContainerForItemOverride()
		{ return new FrameworkElement(); }

		protected sealed override void ClearContainerForItemOverride(
			DependencyObject element, object item)
		{
			var fe = element as FrameworkElement;
			if (fe != null)
				mGeneratedContainers.Remove(fe);
		}
	
		protected sealed override bool IsItemItsOwnContainerOverride(object item)
		{ return false; }
	


	}
}
