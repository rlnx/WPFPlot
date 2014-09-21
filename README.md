WPF Plot
=======
WPF library for plotting one dimensional mathematical functions including control for interactive plot.

Features
--------
* Dynamic data plotting.
* Scaling and moving coordinate system.
* Bind UIElements to points in cartesian coordinate system.

Screenshot
----------
![WPFPlot screenshot](http://s28.postimg.org/ydgkcmvhp/wpfplot.png)

Quickstart
----------
Import WPF Plot namespace:
~~~xml
<Window xmlns:wpfplot="clr-namespace:WPFPlot.Controls;assembly=WPFPlot" />
~~~
Create `GraphControl` and set `PlotData`. Data context object must be instance of `IPlotDataSource`. 
~~~xml
<wpfplot:GraphControl SegmentBegin="-3.14" SegmentEnd="3.14">
    <wpfplot:GraphItem StrokeBrush="Blue" PlotData="{Binding}" />
</wpfplot:GraphControl>
~~~

License
-------
MIT License.
