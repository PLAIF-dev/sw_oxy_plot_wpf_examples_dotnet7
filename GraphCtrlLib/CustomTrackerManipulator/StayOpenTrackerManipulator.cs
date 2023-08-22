using OxyPlot;
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
            // Do nothing
        }
    }
}
