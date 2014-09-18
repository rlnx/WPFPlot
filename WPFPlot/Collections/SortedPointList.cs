using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFPlot.Collections
{
	internal class SortedPointList : List<Point>
	{


		public int FindRightNearestPointIndex(double x)
		{
			int li = 0;
			int ri = Count - 1;
			int bestIndex = -1;

			while (li <= ri)
			{
				int m = (li + ri) / 2;
				double xMVal = this[m].X;
				
				if (x > xMVal)
				{ li = m + 1; }
				else if (x < xMVal)
				{
					ri = m - 1;
					bestIndex = m;
				}
				else
				{ return m; }
			}

			return bestIndex;
		}


		public int FindLeftNearestPointIndex(double x)
		{
			int li = 0;
			int ri = Count - 1;
			int bestIndex = -1;

			while (li <= ri)
			{
				int m = (li + ri) / 2;
				double xMVal = this[m].X;

				if (x > xMVal)
				{ 
					li = m + 1;
					bestIndex = m;
				}
				else if (x < xMVal)
				{ ri = m - 1; }
				else
				{ return m; }
			}

			return bestIndex;
		}


	}
}
