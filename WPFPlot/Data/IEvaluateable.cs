using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFPlot.Data
{
	public interface IEvaluateable
	{
		double Evaluate(double x);
	}
}
