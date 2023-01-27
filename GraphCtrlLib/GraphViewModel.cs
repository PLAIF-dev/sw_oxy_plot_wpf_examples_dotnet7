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

        public GraphViewModel(string strGraphTitle)
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
            #region Axis

            AddAxis("Time", false, AxisPosition.Bottom);
            AddAxis("Value", false, AxisPosition.Left);

            #endregion

            Model.Title = "OxyPlot";

            #region Lengend

            AddLegend("Legend");

            #endregion

            //Design 변경 시도
            //Model.Background = OxyColor.FromRgb(0,0,0);
            //Model.PlotAreaBorderColor = OxyColor.FromRgb(128,128,128);
            //Model.TextColor = OxyColor.FromRgb(128, 128, 128);
            #region Line Setting
            AddLine("LineA");
            AddLine("LineB");
            #endregion

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

        private void AddLine(string strLineTitle)
        {
            LineSeries ls = new LineSeries();
            listSeries.Add(ls);

            //Line 설정
            ls.Title = strLineTitle;
            //LineSeries.InterpolationAlgorithm = InterpolationAlgorithms.UniformCatmullRomSpline;
            //ls.Color = OxyColor.FromAColor(200, OxyColors.Automatic);
            ls.StrokeThickness = 2;
            ls.CanTrackerInterpolatePoints = false;

            Model.Series.Add(ls);
        }

        private void AddLine(int LineCount)
        {
            if(LineCount > 0)
            {
                for(int i = 0; i < LineCount; i++)
                {
                    listSeries.Add(new OxyPlot.Series.LineSeries());
                }
            }
            else
            {
                listSeries.Add(new OxyPlot.Series.LineSeries());
            }

            foreach(LineSeries ls in listSeries)
            {
                //Line 설정
                ls.Title = "LineA";
                //LineSeries.InterpolationAlgorithm = InterpolationAlgorithms.UniformCatmullRomSpline;
                ls.Color = OxyColor.FromAColor(200, OxyColors.SkyBlue);
                ls.StrokeThickness = 2;
                ls.CanTrackerInterpolatePoints = false;

                Model.Series.Add(ls);
            }

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

        private void AddAxis(string strTitle, bool _PositionAtZeroCrossing = false, OxyPlot.Axes.AxisPosition position = AxisPosition.None)
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

        private void AddLegend(string strLegendTitle)
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
