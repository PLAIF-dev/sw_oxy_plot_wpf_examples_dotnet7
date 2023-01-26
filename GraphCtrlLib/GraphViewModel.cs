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

        private List<OxyPlot.Series.LineSeries> listSeries;


        //public OxyPlot.Series.LineSeries LineSeries = new OxyPlot.Series.LineSeries();
        //public OxyPlot.Series.ScatterSeries ScatterSeries = new OxyPlot.Series.ScatterSeries();

        public GraphViewModel() 
        {
            model = new PlotModel();
            controller = new PlotController();
            listSeries = new List<OxyPlot.Series.LineSeries>();

            InitGraph();
        }

        ~GraphViewModel() 
        {

        }

        #region Init
        public void InitGraph()
        {
            LinearAxis AxisX = new OxyPlot.Axes.LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,

                IntervalLength = 80,
                Title = "Time",

                //PositionAtZeroCrossing = true,
            };
            LinearAxis AxisY = new OxyPlot.Axes.LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,

                Title = "Value",

                //PositionAtZeroCrossing = true,
            };

            AxisX.Position = OxyPlot.Axes.AxisPosition.Bottom;
            AxisY.Position = OxyPlot.Axes.AxisPosition.Left;

            //Controller
            //Controller.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack);
            //Controller.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.None, null);

            //AxisX.MajorStep = 1;
            //AxisX.MajorTickSize = 10;
            //AxisX.TickStyle = OxyPlot.Axes.TickStyle.Crossing;
            //AxisX.MinorStep = 0.5;
            //AxisX.Maximum = 10;
            //AxisY.Minimum= 0;

            Model.Title = "OxyPlot";
            Model.Axes.Add(AxisX);
            Model.Axes.Add(AxisY);

            Legend legend = new Legend();
            legend.LegendTitle = "Legend";
            legend.LegendOrientation = LegendOrientation.Horizontal;
            legend.LegendPlacement = LegendPlacement.Inside;
            legend.LegendPosition = LegendPosition.RightTop;
            legend.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            legend.LegendBorder = OxyColors.Black;

            Model.Legends.Add(legend);

            //Design 변경 시도
            //Model.Background = OxyColor.FromRgb(0,0,0);
            //Model.PlotAreaBorderColor = OxyColor.FromRgb(128,128,128);
            //Model.TextColor = OxyColor.FromRgb(128, 128, 128);

            OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries();
            listSeries.Add(lineSeries);

            //Line 설정
            lineSeries.Title = "LineA";
            //LineSeries.InterpolationAlgorithm = InterpolationAlgorithms.UniformCatmullRomSpline;
            lineSeries.Color = OxyColor.FromAColor(200, OxyColors.SkyBlue);
            lineSeries.StrokeThickness = 2;
            lineSeries.CanTrackerInterpolatePoints = false;

            //Marker 설정
            //LineSeries.MarkerFill = OxyColor.FromAColor(200, OxyColors.SkyBlue);
            //LineSeries.MarkerSize= 3;
            //LineSeries.MarkerStrokeThickness = 2;
            //LineSeries.MarkerType = MarkerType.Circle;

            Model.Series.Add(lineSeries);

            //ScatterSeries.Title = "Scatter";
            //ScatterSeries.MarkerType = MarkerType.Circle;
            //ScatterSeries.MarkerStrokeThickness = 0;
            //ScatterSeries.BinSize= 2;
            //Model.Series.Add(ScatterSeries);

            Model.InvalidatePlot(true);
        }
        #endregion

        #region GUI
        public void Clear()
        {
            //listSeries.Clear();

            foreach(LineSeries ls in listSeries)
            {
                ls.Points.Clear();
            }

            Model.InvalidatePlot(true);
        }
        #endregion

        #region Funtion

        #endregion

        #region Data

        //New Line
        public void AddSeries(double data1, double data2)
        {      
            LineSeries lineSeries = new LineSeries();
            listSeries.Add(lineSeries);

            lineSeries.Points.Add(new DataPoint(data1, data2));
            //ScatterSeries.Points.Add(new OxyPlot.Series.ScatterPoint(nVal, dSin));

            Model.InvalidatePlot(true);
        }

        public void AddSeries(double[] data1, double[] data2)
        {
            LineSeries lineSeries = new LineSeries();
            listSeries.Add(lineSeries);

            //data1과 data2의 크기 확인
            for(int i = 0; i < data1.Count(); i++)
            {
                lineSeries.Points.Add(new DataPoint(data1[i], data2[i]));
            }         
            //ScatterSeries.Points.Add(new OxyPlot.Series.ScatterPoint(nVal, dSin));

            Model.InvalidatePlot(true);
        }

        //Append
        public void AppendSeries(int idx, double data1, double data2)
        {
            if(listSeries.Count > 0 && listSeries.Count > idx)
            {
                LineSeries lineSeries = listSeries[idx];

                lineSeries.Points.Add(new DataPoint(data1, data2));
                //ScatterSeries.Points.Add(new OxyPlot.Series.ScatterPoint(nVal, dSin));

                Model.InvalidatePlot(true);
            }
            else if(listSeries.Count <= 0)
            {
                LineSeries lineSeries = new LineSeries();

                lineSeries.Points.Add(new DataPoint(data1, data2));
                
                listSeries.Add(lineSeries);

                Model.InvalidatePlot(true);
            }
            else
            {
                throw new Exception("올바르지 않은 graph idx");
            }
        }
        public void AppendSeries(int idx, double[] data1, double[] data2)
        {
            if (listSeries.Count > 0 && listSeries.Count > idx)
            {
                LineSeries lineSeries = listSeries[idx];

                //data1과 data2의 크기 확인
                for (int i = 0; i < data1.Count(); i++)
                {
                    lineSeries.Points.Add(new DataPoint(data1[i], data2[i]));
                }

                //ScatterSeries.Points.Add(new OxyPlot.Series.ScatterPoint(nVal, dSin));

                Model.InvalidatePlot(true);
            }
            else if (listSeries.Count <= 0)
            {
                LineSeries lineSeries = new LineSeries();

                for (int i = 0; i < data1.Count(); i++)
                {
                    lineSeries.Points.Add(new DataPoint(data1[i], data2[i]));
                }

                listSeries.Add(lineSeries);

                Model.InvalidatePlot(true);
            }
            else
            {
                throw new Exception("올바르지 않은 graph idx");
            }
        }

        #endregion

    }
}
