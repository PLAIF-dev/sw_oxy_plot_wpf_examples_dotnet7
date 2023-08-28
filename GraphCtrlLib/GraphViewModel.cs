using CommunityToolkit.Mvvm.Input;
using GraphCtrlLib.CustomTrackerManipulator;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.Manipulations;

namespace GraphCtrlLib
{
    public class GraphViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string info = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            var propertChanged = this.PropertyChanged;
            if (propertChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //Oxy Plot
        private PlotModel model;

        public PlotModel Model
        {
            get { return model; }
            set 
            { 
                model = value; 
            }
        }

        private PlotController controller;

        public PlotController Controller
        {
            get { return controller; }
            set 
            { 
                controller = value; 
            }
        }

        private StaysOpenTrackerManipulator manipulator { get; set; }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Dictionary<string, LineSeries> dicLineGraph = new Dictionary<string, LineSeries>();

        //private List<OxyPlot.Series.LineSeries> listSeries;


        //public OxyPlot.Series.LineSeries LineSeries = new OxyPlot.Series.LineSeries();
        //public OxyPlot.Series.ScatterSeries ScatterSeries = new OxyPlot.Series.ScatterSeries();

        public ICommand PlotDrop { get; set; }

        public ICommand PlotDragOver { get; set; }
        public ICommand PlotLoadedCommand { get; set; }

        public GraphViewModel(string strGraphTitle = "Graph") 
        {
            model = new PlotModel();
            controller = new PlotController();
            //controller.BindMouseDown(OxyMouseButton.Left, PlotCommands.PointsOnlyTrack);

            controller.BindMouseDown(OxyMouseButton.Left, new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
            {
                manipulator = new StaysOpenTrackerManipulator(view);
                controller.AddMouseManipulator(view, manipulator, args);           
            }));
     
            name = strGraphTitle;
            model.Title = name;

            PlotDrop = new RelayCommand<DragEventArgs>(PlotView_Drop);
            PlotLoadedCommand = new RelayCommand(OnPlotLoaded);

            InitGraph();
        }

        private void OnPlotLoaded()
        {
            if (manipulator == null)
            {
                //한번 임시로 누른 것으로 하여 manipulator 객체를 생성시킨다.
                var simulatorArgs = new OxyMouseDownEventArgs()
                {
                    Position = new ScreenPoint(100, 100),
                    ChangedButton = OxyMouseButton.Left,
                    ClickCount = 1
                };

                controller.HandleMouseDown(model.PlotView, simulatorArgs);
            }
        }

        public void SyncTracker(double dataX, double dataY, double sPointX, double sPointY, object obj, int Index)
        {
            var series = model.Series[0] as LineSeries;

            if (manipulator == null)
            {
                //한번 임시로 누른 것으로 하여 manipulator 객체를 생성시킨다.
                var simulatorArgs = new OxyMouseDownEventArgs()
                {
                    Position = new ScreenPoint(sPointX, 0),
                    ChangedButton = OxyMouseButton.Left,
                    ClickCount = 1
                };

                controller.HandleMouseDown(model.PlotView, simulatorArgs);
            }

            if (manipulator != null && series != null)
            {
                manipulator.ShowTracker(series, new DataPoint(dataX, dataY), new ScreenPoint(sPointX, sPointY), obj, Index);
            }  
        }

        private void PlotView_Drop(DragEventArgs? e)
        {
            if(e != null)
            {
                if (e.Data.GetData(typeof(GraphModel.GraphDataSet)) is GraphModel.GraphDataSet graphDataSet)
                {
                    //Point position = e.GetPosition(graph);
                    //AddDataToGraph(graphDataSet, position);

                    AddData(graphDataSet.Id, graphDataSet.xData, graphDataSet.yData);
                    ReDraw();
                }
            }
        }

        ~GraphViewModel() 
        {

        }

        #region Init

        public void InitGraph()
        {
            #region Axis

            AddAxis("Time", AxisPosition.Bottom);

            model.Axes[0].Minimum = 0;
            model.Axes[0].Maximum = 5000;

            AddAxis("Value", AxisPosition.Left);

            #endregion

            #region Lengend

            AddLegend("Legend");

            #endregion

            //Design 변경 시도
            //Model.Background = OxyColor.FromRgb(0, 0, 0);
            //Model.PlotAreaBorderColor = OxyColor.FromRgb(128, 128, 128);
            //Model.TextColor = OxyColor.FromRgb(128, 128, 128);

            #region Line Setting
            AddLine("LineA");
            AddLine("LineB");
            #endregion

            ReDraw();
        }
        #endregion

        #region Example
        public void ExampleCode()
        {
            var graph = new GraphViewModel("Test");

            graph.AddAxis("X", AxisPosition.Bottom);
            graph.AddAxis("Y", AxisPosition.Left);

            graph.AddLine("LineA", OxyColors.SkyBlue, 2);
            graph.AddLine("LineB", OxyColors.SeaShell, 2);
            graph.AddLine("LineC", OxyColors.AliceBlue, 2);
            graph.AddLine("LineD", OxyColors.DeepSkyBlue, 2);

            double[] dataX = new double[1000];
            double[] dataY = new double[1000];

            // Graph Name으로 Data 추가
            graph.AddData("LineA", 1, 1);
            graph.AddData("LineB", dataX, dataY);

            // Index로 Data 추가
            graph.AddData(2, dataX, dataY);
            graph.AddData(3, dataX, dataY);
        }
        #endregion

        #region GUI
        public void Clear()
        {
            //listSeries.Clear();

            foreach(LineSeries ls in dicLineGraph.Values)
            {
                ls.Points.Clear();
            }

            ReDraw();
        }
        #endregion

        #region Funtion

        public void AddLine(string strLineTitle, OxyColor color, int LineThickness = 2)
        {
            LineSeries ls = new LineSeries();
            dicLineGraph.Add(strLineTitle, ls);

            //Line 설정
            ls.Title = strLineTitle;
            //LineSeries.InterpolationAlgorithm = InterpolationAlgorithms.UniformCatmullRomSpline;
            //ls.Color = OxyColor.FromAColor(200, OxyColors.Automatic);
            ls.Color = color;
            ls.StrokeThickness = LineThickness;
            ls.CanTrackerInterpolatePoints = false;

            Model.Series.Add(ls);

            //Marker 설정
            //LineSeries.MarkerFill = OxyColor.FromAColor(200, OxyColors.SkyBlue);
            //LineSeries.MarkerSize= 3;
            //LineSeries.MarkerStrokeThickness = 2;
            //LineSeries.MarkerType = MarkerType.Circle;

            //ScatterSeries.Title = "Scatter";
            //ScatterSeries.MarkerType = MarkerType.Circle;
            //ScatterSeries.MarkerStrokeThickness = 0;
            //ScatterSeries.BinSize= 2;
            //Model.Series.Add(ScatterSeries);   
        }

        public void AddLine(string strLineTitle, int LineThickness = 2)
        {
            AddLine(strLineTitle, OxyColors.Automatic, LineThickness);
        }

        public void AddAxis(string strTitle, OxyPlot.Axes.AxisPosition position = AxisPosition.None, bool _PositionAtZeroCrossing = false)
        {
            LinearAxis Axis = new OxyPlot.Axes.LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,

                IntervalLength = 80,
                Title = strTitle,

                PositionAtZeroCrossing = _PositionAtZeroCrossing,
            };

            Axis.Position = position;

            Model.Axes.Add(Axis);
        }

        public void AddLegend(string strLegendTitle)
        {
            Legend legend = new Legend();
            legend.LegendTitle = strLegendTitle;
            legend.LegendOrientation = LegendOrientation.Horizontal;
            legend.LegendPlacement = LegendPlacement.Inside;
            legend.LegendPosition = LegendPosition.RightTop;
            legend.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            legend.LegendBorder = OxyColors.Black;

            Model.Legends.Add(legend);
        }

        public void ReDraw()
        {
            Model.InvalidatePlot(true);
        }

        #endregion

        #region Data

        //Utility
        public int GetCount(int idx)
        {
            return dicLineGraph.Values.ToList()[idx].Points.Count;
        }

        //New Line

        private void AddData(LineSeries ls, double data1, double data2)
        {
            ls.Points.Add(new DataPoint(data1, data2));

            model.Axes[0].Minimum = ls.Points[0].X;
            model.Axes[0].Maximum = 5000 + ls.Points[0].X;
        }

        private void AddData(LineSeries ls, double[] data1, double[] data2)
        {
            //data1과 data2의 크기 확인
            for (int i = 0; i < data1.Count(); i++)
            {
                AddData(ls, data1[i], data2[i]);
            }
        }

        public void AddData(int idx, double data1, double data2)
        {
            LineSeries? ls = new LineSeries();

            if(dicLineGraph.Count > 0 && dicLineGraph.Count >= idx + 1)
            {
                ls = dicLineGraph.Values.ToList()[idx];
                AddData(ls, data1, data2);
            }
            else
            {
                throw new Exception("올바르지 않은 Index");
            }

            ReDraw();
        }

        public void AddData(int idx, double[] data1, double[] data2)
        {
            LineSeries? ls = new LineSeries();

            if (dicLineGraph.Count > 0 && dicLineGraph.Count >= idx + 1)
            {
                ls = dicLineGraph.Values.ToList()[idx];
                AddData(ls, data1, data2);
            }
            else
            {
                throw new Exception("올바르지 않은 Index");
            }

            ReDraw();
        }

        public void AddData(int idx, List<double> data1, List<double> data2)
        {
            LineSeries? ls = new LineSeries();

            if (dicLineGraph.Count > 0 && dicLineGraph.Count >= idx + 1)
            {
                ls = dicLineGraph.Values.ToList()[idx];
                AddData(ls, data1.ToArray(), data2.ToArray());
            }
            else
            {
                throw new Exception("올바르지 않은 Index");
            }

            ReDraw();
        }

        public void AddData(string strLineName, double data1, double data2)
        {
            LineSeries? ls = new LineSeries();
            if(dicLineGraph.TryGetValue(strLineName, out ls))
            {
                AddData(ls, data1, data2);
            }
            else
            {
                throw new Exception("올바르지 않은 LineName");
            }

            ReDraw();
        }

        public void AddData(string strLineName, double[] data1, double[] data2)
        {
            LineSeries? ls = new LineSeries();
            if (dicLineGraph.TryGetValue(strLineName, out ls))
            {
                AddData(ls, data1, data2);
            }
            else
            {
                throw new Exception("올바르지 않은 LineName");
            }

            ReDraw();
        }

        public void AddData(string strLineName, List<double> data1, List<double> data2)
        {
            LineSeries? ls = new LineSeries();
            if (dicLineGraph.TryGetValue(strLineName, out ls))
            {
                AddData(ls, data1.ToArray(), data2.ToArray());
            }
            else
            {
                throw new Exception("올바르지 않은 LineName");
            }

            ReDraw();
        }

        //Delete
        public void RemoveAtFirst(int idx) 
        {
            LineSeries? ls = new LineSeries();

            if (dicLineGraph.Count > 0 && dicLineGraph.Count >= idx + 1)
            {
                ls = dicLineGraph.Values.ToList()[idx];
                ls.Points.RemoveAt(0);
            }
            else
            {
                throw new Exception("올바르지 않은 Index");
            }
        }

        #endregion

    }
}
