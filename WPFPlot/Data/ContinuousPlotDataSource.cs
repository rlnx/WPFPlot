using System;
using System.Collections.Generic;
using System.Windows;
using WPFPlot.Collections;

namespace WPFPlot.Data
{
   public class ContinuousPlotDataSource : IPlotDataSource
   {

      public event EventHandler PointsChanged;

      private SortedPointList mEvaluatedPoints;
      private double mMinSegmentStart;
      private double mMaxSegmentEnd;
      private double mSegmentStart;
      private double mSegmentEnd;


      public ContinuousPlotDataSource()
      { mEvaluatedPoints = new SortedPointList(); }


      private IEvaluateable mFunction;
      public IEvaluateable Function
      {
         get { return mFunction; }
         set
         {
            mFunction = value;
            UpdatePoints();
            NotifyPointsChanged();
         }
      }

      private double mStep = 0.05;
      public double Step
      {
         get { return mStep; }
         set
         {
            mStep = value;
            UpdatePoints();
            NotifyPointsChanged();
         }
      }


      public void SetSegment(double from, double to)
      {
         if (mFunction == null)
            return;

         if (Double.IsInfinity(from) || Double.IsInfinity(to))
            return;

         mSegmentStart = from;
         mSegmentEnd = to;

         if (mEvaluatedPoints.Count == 0)
         {
            InitSegment(from, to);
            return;
         }

         FillSegmentLeft(from);
         FillSegmentRight(to);
      }


      public void SetZoom(double zoom) { }

      public Point Interpolate(double x)
      {
         if (mFunction == null)
            return default(Point);

         return new Point(x, mFunction.Evaluate(x));
      }

      public IList<Point> GetPoints()
      { return mEvaluatedPoints; }

      public int GetStartIndex()
      { return mEvaluatedPoints.FindRightNearestPointIndex(mSegmentStart); }

      public int GetEndIndex()
      { return mEvaluatedPoints.FindLeftNearestPointIndex(mSegmentEnd); }



      private void NotifyPointsChanged()
      {
         if (PointsChanged != null)
            PointsChanged(this, EventArgs.Empty);
      }

      private void UpdatePoints()
      {
         mEvaluatedPoints.Clear();
         SetSegment(mSegmentStart, mSegmentEnd);
      }


      private void InitSegment(double from, double to)
      {
         mMinSegmentStart = from;
         mMaxSegmentEnd = to;
         for (double x = from; x < to; x += mStep)
         {
            double y = mFunction.Evaluate(x);
            mEvaluatedPoints.Add(new Point(x, y));
         }
      }

      private void FillSegmentLeft(double from)
      {
         double startOffset = mMinSegmentStart - mStep;
         if (startOffset > from)
         {
            for (double x = startOffset; x > from; x -= mStep)
            {
               double y = mFunction.Evaluate(x);
               mEvaluatedPoints.Insert(0, new Point(x, y));
            }
            mMinSegmentStart = from;
         }
      }

      private void FillSegmentRight(double to)
      {
         double startOffset = mMaxSegmentEnd + mStep;
         if (startOffset < to)
         {
            for (double x = startOffset; x < to; x += mStep)
            {
               double y = mFunction.Evaluate(x);
               mEvaluatedPoints.Add(new Point(x, y));
            }
            mMaxSegmentEnd = to;
         }
      }


      public double GetSegmentBegin()
      { return Double.NegativeInfinity; }

      public double GetSegmentEnd()
      { return Double.PositiveInfinity; }

   }
}
