using System;
using System.Diagnostics.Contracts;
using WPFPlot.Data;

namespace WPFPlot.Example.ViewModel
{
	public class FunctionWrapper : IEvaluateable
	{
		private Func<double, double> mFunc;

		public FunctionWrapper(Func<double, double> func)
		{
			Contract.Requires(func != null);
			mFunc = func;
		}

		public double Evaluate(double x)
		{ return mFunc(x); }

	}
}
