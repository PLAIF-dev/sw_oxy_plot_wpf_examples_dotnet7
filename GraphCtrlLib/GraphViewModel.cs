﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GraphCtrlLib.CustomController;
using GraphCtrlLib.CustomTrackerManipulator;
using GraphCtrlLib.Message;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using HorizontalAlignment = OxyPlot.HorizontalAlignment;
using VerticalAlignment = OxyPlot.VerticalAlignment;

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

        private StaysOpenTrackerManipulator? manipulator { get; set; }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int id;
        public int ID
        {
            get => id; set => id = value;
        }

        private Dictionary<string, LineSeries> dicLineGraph = new Dictionary<string, LineSeries>();

        //private List<OxyPlot.Series.LineSeries> listSeries;


        //public OxyPlot.Series.LineSeries LineSeries = new OxyPlot.Series.LineSeries();
        //public OxyPlot.Series.ScatterSeries ScatterSeries = new OxyPlot.Series.ScatterSeries();

        public void SetverticalLineTrackerX(double x, string text="")
        {
            verticalLineTracker.X = x;
            verticalLineTracker.Text = text;
            model.InvalidatePlot(false);
        }

        private LineAnnotation verticalLineTracker = new LineAnnotation()
        {
            Type = LineAnnotationType.Vertical,
            Color = OxyColors.Red,
            LineStyle = LineStyle.Solid,
            TextHorizontalAlignment = HorizontalAlignment.Right,
            TextVerticalAlignment = VerticalAlignment.Bottom,
        };

        public ICommand PlotDrop { get; set; }
        public ICommand? PlotDragOver { get; set; }
        public ICommand PlotLoaded { get; set; }
        public ICommand ResetClick { get; set; }
        public ICommand SplitClick { get; set; }
        public ICommand DeleteClick { get; set; }
        public ICommand ViewInNewWindowClick { get; set; }

        public GraphViewModel(int _id, string strGraphTitle = "Graph") 
        {
            model = new PlotModel();
            controller = new CustomPlotController();

            ID = _id;
            name = strGraphTitle;
            model.Title = name;

            model.Annotations.Add(verticalLineTracker);
            PlotDrop = new RelayCommand<DragEventArgs>(PlotViewDropCommand);
            PlotLoaded = new RelayCommand(PlotLoadedCommand);
            ResetClick = new RelayCommand(ResetPlotCommand);
            SplitClick = new RelayCommand(SplitLineCommand);
            DeleteClick = new RelayCommand(DeleteGraphCommand);
            ViewInNewWindowClick = new RelayCommand(ViewInNewWindowCommand);

            InitGraph();

            #region Messenger

            var Messeenger = WeakReferenceMessenger.Default;
            Messeenger.Register<SharedAddLineMessage>(this, OnAddLineMessageReceived);
            #endregion
        }

        private void OnAddLineMessageReceived(object obj, SharedAddLineMessage message)
        {
            List<GraphModel.GraphDataSet>? listItem = message.GraphDataSets.Cast<GraphModel.GraphDataSet>().ToList();

            if (listItem != null)
            {
                foreach (var graphdata in listItem)
                {
                    AddData(graphdata.LineName, graphdata.DataX, graphdata.DataY);
                }

                ReDraw();
            }        
        }

        private void ViewInNewWindowCommand()
        {
            WeakReferenceMessenger.Default.Send(new SharedNewWindowMessage
            {
                GraphID = ID,
                GraphName = Name,
            });
        }

        private void DeleteGraphCommand()
        {
            WeakReferenceMessenger.Default.Send(new SharedDeleteMessage
            {
                GraphID = ID,
                GraphName = Name,
            });
        }

        private void SplitLineCommand()
        {
            WeakReferenceMessenger.Default.Send(new SharedSplitMessage
            {
                GraphID = ID,
                LineName = dicLineGraph.Keys.ToList(),
            });
        }

        private void ResetPlotCommand()
        {
            this.Model.ResetAllAxes();
            ReDraw();
        }

        private void PlotLoadedCommand()
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

        private void PlotViewDropCommand(DragEventArgs? e)
        {
            if(e != null)
            {
                var items = e.Data.GetData(typeof(List<object>));
                List<GraphModel.GraphDataSet>? listItem = ((IEnumerable)items).Cast<GraphModel.GraphDataSet>().ToList();

                if (listItem != null)
                {
                    foreach (var graphdata in listItem)
                    {
                        //AddData(graphdata.Id, graphdata.xData, graphdata.yData);
                        AddData(graphdata.LineName, graphdata.DataX, graphdata.DataY);
                    }

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
            model.Axes[0].Maximum = 50;

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
            //AddLine("LineA");
            //AddLine("LineB");
            #endregion

            ReDraw();
        }
        #endregion

        #region Example
        public void ExampleCode()
        {
            var graph = new GraphViewModel(0, "Test");

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
            model.Series.Clear();
            model.Axes.Clear();
            foreach (LineSeries ls in dicLineGraph.Values)
            {
                ls.Points.Clear();
            }
            dicLineGraph.Clear();

            InitGraph();
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

        public void AddAxis(string strTitle, AxisPosition position = AxisPosition.None, bool _PositionAtZeroCrossing = false)
        {
            LinearAxis Axis = new LinearAxis()
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
        public ElementCollection<Series> GetSeries()
        {
            return model.Series;
        }

        public Dictionary<string, LineSeries> GetDicLineGraph()
        {
            return dicLineGraph;
        }

        //New Line

        private void AddData(LineSeries ls, double data1, double data2)
        {
            try
            {
                ls.Points.Add(new DataPoint(data1, data2));

                model.Axes[0].Minimum = ls.Points[0].X;
                model.Axes[0].Maximum = 50 + ls.Points[0].X;
            }
            catch
            {

            }
        }

        private void AddData(LineSeries ls, double[] data1, double[] data2)
        {
            try
            {
                //data1과 data2의 크기 확인
                for (int i = 0; i < data1.Count(); i++)
                {
                    AddData(ls, data1[i], data2[i]);
                }
            }
            catch
            {

            }
        }

        private void AddData(LineSeries ls, DataPoint[] data)
        {
            try
            {
                //data1과 data2의 크기 확인
                for (int i = 0; i < data.Count(); i++)
                {
                    AddData(ls, data[i].X, data[i].Y);
                }
            }
            catch
            {

            }
        }

        public void AddData(int idx, double data1, double data2)
        {
            try
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
            catch
            {
                throw new Exception("올바르지 않은 Index");
            }
        }

        public void AddData(int idx, double[] data1, double[] data2)
        {
            try
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
            catch 
            {
                throw new Exception("올바르지 않은 Index");
            }
        }

        public void AddData(int idx, List<double> data1, List<double> data2)
        {
            try
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
            catch
            {
                throw new Exception("올바르지 않은 Index");
            }
        }

        public void AddData(string strLineName, double data1, double data2)
        {
            try
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
            catch 
            {
                throw new Exception("올바르지 않은 LineName");
            }
        }

        public void AddData(string strLineName, double[] data1, double[] data2)
        {
            try
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
            catch
            {
                throw new Exception("올바르지 않은 LineName");
            }
        }

        public void AddData(string strLineName, List<double> data1, List<double> data2)
        {
            try
            {
                LineSeries? ls = new LineSeries();
                if (dicLineGraph.TryGetValue(strLineName, out ls) == false)
                {
                    AddLine(strLineName);

                    if (dicLineGraph.TryGetValue(strLineName, out ls))
                    {
                        AddData(ls, data1.ToArray(), data2.ToArray());
                    }                
                }

                ReDraw();
            }
            catch
            {
                throw new Exception("올바르지 않은 LineName");
            }
        }

        public void AddData(string strLineName, List<DataPoint> data)
        {
            try 
            {
                LineSeries? ls = new LineSeries();
                if (dicLineGraph.TryGetValue(strLineName, out ls) == false)
                {
                    AddLine(strLineName);

                    if (dicLineGraph.TryGetValue(strLineName, out ls))
                    {
                        AddData(ls, data.ToArray());
                    }
                }

                ReDraw();
            }
            catch 
            {
                throw new Exception("올바르지 않은 LineName");
            }
        }

        //Delete
        public void RemoveLine(string lineName) 
        {
            try
            {
                foreach(var line in model.Series.Where(s => s.Title == lineName).ToList())
                {
                    model.Series.Remove(line);
                }

                dicLineGraph.Remove(lineName);
            }
            catch 
            {

            }
        }

        #endregion
    }
}
