using System;
using WPFPlot.Data;

namespace WPFPlot.Example.ViewModel
{
	public class MainModel
	{

		public MainModel()
		{
			var f1 = new FunctionWrapper(x => Math.Sin(x));
			var f2 = new FunctionWrapper(x => 2 * Math.Cos(x));
			var f3 = new FunctionWrapper(x => -Math.Sin(x));
			var f4 = new FunctionWrapper(x => -2 * Math.Cos(x));

			Test1 = new ContinuousPlotDataSource() { Function = f1 };
			Test2 = new ContinuousPlotDataSource() { Function = f2 };
			Test3 = new ContinuousPlotDataSource() { Function = f3 };
			Test4 = new ContinuousPlotDataSource() { Function = f4 };
		}

		public IPlotDataSource Test1 { get; private set; }
		public IPlotDataSource Test2 { get; private set; }
		public IPlotDataSource Test3 { get; private set; }
		public IPlotDataSource Test4 { get; private set; }

	}
}
