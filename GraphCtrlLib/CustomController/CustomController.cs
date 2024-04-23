using OxyPlot;

namespace GraphCtrlLib.CustomController
{
    public class CustomPlotController : PlotController
    {
        public CustomPlotController() : base()
        {
            //Binding
            this.BindMouseDown(OxyMouseButton.Right, PlotCommands.ZoomRectangle);
            this.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PanAt);

            //UnBinding
            this.UnbindMouseDown(OxyMouseButton.Left, OxyModifierKeys.None, 1);

            //StayOpenManiporator 주석상태로 관리
            //this.BindMouseDown(OxyMouseButton.Left, new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
            //{
            //    manipulator = new StaysOpenTrackerManipulator(view);
            //    controller.AddMouseManipulator(view, manipulator, args);
            //}));
        }
    }
}
