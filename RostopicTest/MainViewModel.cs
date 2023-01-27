using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Rosbridge.Client;
using System;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Threading;
using System.Windows.Input;

namespace RostopicTest
{
    public class MainViewModel : ViewModelBase
    {
        private ICommand CmdClickChk1 { get; }
        private ICommand CmdClickChk2 { get; }
        private ICommand CmdClickChk3 { get; }
        private ICommand CmdClickChk4 { get; }
        private ICommand CmdClickChk5 { get; }
        private ICommand CmdClickChk6 { get; }

        private readonly Timer timer;
        private readonly Stopwatch watch = new Stopwatch();
        // try to change might be lower or higher than the rendering interval
        private const int UpdateInterval = 200;
        public PlotModel PlotModel { get; private set; }
        internal MainViewModel()
        {
            var rosmgr = RosbridgeMgr.Instance;
            rosmgr.SetMainModel(this);

            rosmgr.Connect("ws://192.168.1.36:9090");
            rosmgr.SubscribeMsg("/joint_states", "sensor_msgs/JointState", _subscriber_MessageReceived);
            PlotModel = new();
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left/*, Minimum = -2, Maximum = 2*/ });
            PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            
            timer = new Timer(OnTimerElapsed);
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            this.watch.Start();
            this.OnPropertyChanged(nameof(PlotModel));
            this.timer.Change(1000, UpdateInterval);

            CmdClickChk1 = new RelayCommand(OnCmdClickChk1);
            CmdClickChk2 = new RelayCommand(OnCmdClickChk2);
            CmdClickChk3 = new RelayCommand(OnCmdClickChk3);
            CmdClickChk4 = new RelayCommand(OnCmdClickChk4);
            CmdClickChk5 = new RelayCommand(OnCmdClickChk5);
            CmdClickChk6 = new RelayCommand(OnCmdClickChk6);
        }

        private bool isChecked1;
        public bool IsChecked1
        {
            get { return isChecked1; }
            set {  isChecked1 = value; OnPropertyChanged(nameof(isChecked1)); }
        }
        private bool isChecked2;
        public bool IsChecked2
        {
            get { return isChecked2; }
            set { isChecked2 = value; OnPropertyChanged(nameof(isChecked2)); }
        }
        private bool isChecked3;
        public bool IsChecked3
        {
            get { return isChecked3; }
            set { isChecked3 = value; OnPropertyChanged(nameof(isChecked3)); }
        }
        private bool isChecked4;
        public bool IsChecked4
        {
            get { return isChecked4; }
            set { isChecked4 = value; OnPropertyChanged(nameof(isChecked4)); }
        }
        private bool isChecked5;
        public bool IsChecked5
        {
            get { return isChecked5; }
            set { isChecked5 = value; OnPropertyChanged(nameof(isChecked5)); }
        }
        private bool isChecked6;
        public bool IsChecked6
        {
            get { return isChecked6; }
            set { isChecked6 = value; OnPropertyChanged(nameof(isChecked6)); }
        }

        private void OnCmdClickChk1()
        {
        }

        private void OnCmdClickChk2()
        {
        }

        private void OnCmdClickChk3()
        {
        }

        private void OnCmdClickChk4()
        {
        }

        private void OnCmdClickChk5()
        {
        }

        private void OnCmdClickChk6()
        {
        }


        private int y = 1;
        private int x = 0;

        public void _subscriber_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.Message["topic"]!.ToString())
            {
                case "/joint_states":
                    var msg = e.Message["msg"]!;
                    var pos = msg!["position"]!;
                    var sec = Int32.Parse(msg["header"]!["stamp"]!["secs"]!.ToString());
                    var nsec = Int32.Parse(msg["header"]!["stamp"]!["nsecs"]!.ToString());
                    var seq = Int32.Parse(msg["header"]!["seq"]!.ToString());
                    var pos1 = Double.Parse(pos[0]!.ToString());
                    var pos2 = Double.Parse(pos[1]!.ToString());
                    var pos3 = Double.Parse(pos[2]!.ToString());
                    var pos4 = Double.Parse(pos[3]!.ToString());
                    var pos5 = Double.Parse(pos[4]!.ToString());
                    var pos6 = Double.Parse(pos[5]!.ToString());

                    //((LineSeries)PlotModel.Series[0]).Points.Add(new DataPoint(x, y));
                    //((LineSeries)PlotModel.Series[1]).Points.Add(new DataPoint(x, y*y/10));

                    double time = sec + nsec*0.00000001;

                    if (isChecked1) ((LineSeries)PlotModel.Series[0]).Points.Add(new DataPoint(nsec, pos1));
                    if (isChecked2) ((LineSeries)PlotModel.Series[1]).Points.Add(new DataPoint(nsec, pos2));
                    if (isChecked3) ((LineSeries)PlotModel.Series[2]).Points.Add(new DataPoint(nsec, pos3));
                    if (isChecked4) ((LineSeries)PlotModel.Series[3]).Points.Add(new DataPoint(nsec, pos4));
                    if (isChecked5) ((LineSeries)PlotModel.Series[4]).Points.Add(new DataPoint(nsec, pos5));
                    if (isChecked6) ((LineSeries)PlotModel.Series[5]).Points.Add(new DataPoint(nsec, pos6));

                    for(int i=0; i<6; i++)
                    {
                        if (((LineSeries)PlotModel.Series[i]).Points.Count > 1000)
                            ((LineSeries)PlotModel.Series[i]).Points.RemoveAt(0);
                    }
                    
                    x++;
                    y++;
                    // 매번 업데이트 하려니 너무 빡세다. 그냥 타이머로 invalidate 하자
                    //PlotModel.InvalidatePlot(true);
                    break;
            }
        }

        private void OnTimerElapsed(object state)
        {
            this.PlotModel.InvalidatePlot(true);
        }
    }
}
