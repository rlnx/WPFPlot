using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPlot.Controls
{
	public class GraphItemUpdatedEventArgs : EventArgs
	{

		public GraphItemUpdatedEventArgs(GraphItemUpdateOptions options)
		{ Options = options; }

		public GraphItemUpdateOptions Options { get; private set; }

	}
}
