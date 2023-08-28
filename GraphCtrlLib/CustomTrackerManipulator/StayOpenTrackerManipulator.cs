using CommunityToolkit.Mvvm.Messaging;
using GraphCtrlLib.Message;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCtrlLib.CustomTrackerManipulator
{
    public class StaysOpenTrackerManipulator : TrackerManipulator
    {
        public StaysOpenTrackerManipulator(IPlotView plotView) : base(plotView)
        {
            Snap = true;
            PointsOnly = true;
        }
        public override void Completed(OxyMouseEventArgs e)
        {
            // Message 전달
            if(this.PlotView != null && this.PlotView.ActualModel != null)
            {
                Series currentSeries = this.PlotView.ActualModel.GetSeriesFromPoint(e.Position, FiresDistance);
                if (currentSeries != null)
                {
                    var hitTestResult = currentSeries.HitTest(new HitTestArguments(e.Position, FiresDistance));

                    if (hitTestResult != null && hitTestResult.Item != null)
                    {
                        // 해당 데이터 포인트
                        var dataPoint = (DataPoint)hitTestResult.Item;

                        WeakReferenceMessenger.Default.Send(new SharedMessge 
                        { 
                            DataX = dataPoint.X, 
                            DataY = dataPoint.Y,
                            sDataX = e.Position.X, 
                            sDataY = e.Position.Y,
                            DataIndex = Convert.ToInt32(hitTestResult.Index),
                            e = e
                        });
                    }
                }
            }
        }
        public void ShowTracker( Series series, DataPoint point, ScreenPoint sPoint, object obj, int Index)
        {         
            //DataPoint를 가지고 현재 객체의 ScreenPoint를 얻는다.
            var xAxis = this.PlotView.ActualModel.Axes.FirstOrDefault(a => a.Position == OxyPlot.Axes.AxisPosition.Bottom);

            if(xAxis != null) 
            {
                //Index를 가지고 Series를 가져오는 방법
                //LineSeries ls = series as LineSeries;
                //var DataPoints = ls.Points[Index];

                double screenX = xAxis.Transform(point.X);

                //Screen Point가 자신의 PlotArea 범위 내에 있는 지 확인
                if (!this.PlotView.ActualModel.PlotArea.Contains(screenX, this.PlotView.ActualModel.PlotArea.Top))
                {
                    return;
                }

                var nearestPoint = series.GetNearestPoint(new ScreenPoint(screenX, 0), false);

                if (nearestPoint != null)
                {
                    nearestPoint.PlotModel = this.PlotView.ActualModel;
                    this.PlotView.ShowTracker(nearestPoint);
                    this.PlotView.ActualModel.RaiseTrackerChanged(nearestPoint);

                    //PlotView.ShowTracker(nearestPoint);
                }
            }
        }
    }
}
