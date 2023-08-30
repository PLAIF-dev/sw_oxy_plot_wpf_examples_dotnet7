using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GraphCtrlLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static GraphCtrlLib.GraphModel;
using System.Windows.Threading;
using System.Windows.Input;

namespace GraphResearch
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region NotifyProperty
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string info = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region 변수
        public ICommand ListBoxChangedCommand { get; set; }

        public ICommand BtnNewClickCommand { get; set; }

        public ICommand BtnDeleteClickCommand { get; set; }

        public ICommand BtnStartClick { get; set; }
        public ICommand BtnStopClick { get; set; }
        public ICommand BtnClearClick { get; set; }


        private ObservableCollection<GraphViewModel> _viewModels = new ObservableCollection<GraphViewModel>();

        public ObservableCollection<GraphViewModel> ViewModels
        {
            get { return _viewModels; }
        }

        //private object? _contentView;
        //public object ContentView
        //{
        //    get { return this._contentView; }
        //    set
        //    {
        //        this._contentView = value;
        //        NotifyPropertyChanged(nameof(ContentView));
        //    }
        //}

        private int _selectedInd;

        public int SelectedInd
        {
            get { return _selectedInd; }
            set
            {
                _selectedInd = value;
                NotifyPropertyChanged(nameof(_selectedInd));
            }
        }

        public ObservableCollection<GraphModel.GraphDataSet> GraphDataSets { get; set; }

        private DispatcherTimer timer;
        #endregion

        public const double MaxGraphNum = 50;

        public MainViewModel()
        {
            //this.ContentView = (object)new GraphCtrlLib.GraphViewModel();

            ListBoxChangedCommand = new RelayCommand(ListBoxSeletedChanged);
            BtnNewClickCommand = new RelayCommand(BtnNewClick);
            BtnDeleteClickCommand = new RelayCommand(BtnDeleteClick);
            BtnStartClick = new RelayCommand(BtnStartCommand);
            BtnStopClick = new RelayCommand(BtnStopCommand);
            BtnClearClick = new RelayCommand(BtnClearCommand);

            List<double> xData = new List<double>();
            List<double> yData = new List<double>();

            List<double> xData2 = new List<double>();
            List<double> yData2 = new List<double>();

            for (float i = 0; i < MaxGraphNum; i += 0.1f)
            {
                xData.Add(Math.Sin(i) * 10 + 25);
                yData.Add(Math.Cos(i) * 10);

                xData2.Add(i);
                yData2.Add(Math.Cos(i) * 10);
            }

            GraphDataSets = new ObservableCollection<GraphModel.GraphDataSet>();
            GraphDataSets.Add(new GraphModel.GraphDataSet()
            {
                Id = 0,
                LineName = "lineA",
                xData = xData,
                yData = yData
            });
            GraphDataSets.Add(new GraphModel.GraphDataSet()
            {
                Id = 1,
                LineName = "lineB",
                xData = xData2,
                yData = yData2
            });
            GraphDataSets.Add(new GraphModel.GraphDataSet()
            {
                Id = 2,
                LineName = "lineC",
                xData = xData2,
                yData = yData2
            });

            //Timer 초기화
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5);
            timer.Tick += CallBackTimer;

            #region Messenger

            var Messageee = WeakReferenceMessenger.Default;
            Messageee.Register<GraphCtrlLib.Message.SharedMessge>(this, OnMessageReceived);

            #endregion

        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param> 객체 전송자
        /// <param name="message"></param> 메세지
        private void OnMessageReceived(object obj, GraphCtrlLib.Message.SharedMessge message)
        {
            double DataX = message.DataX;
            double DataY = message.DataY;

            double sDataX = message.sDataX;
            double sDataY = message.sDataY;

            var e = message.e;
            if (e != null)
            {
                int index = message.DataIndex;
                foreach (GraphViewModel _graph in _viewModels)
                {
                    _graph.SyncTracker(DataX, DataY, sDataX, sDataY, e, index);
                }
            }
        }

        public double xData = new double();
        public double yData = new double();
        double count = 0f;

        private void CallBackTimer(object? sender, EventArgs e)
        {
            xData = count;
            yData = Math.Sin(count) * 1000f;

            foreach (GraphViewModel _graph in _viewModels)
            {
                //if(_graph.GetCount(0) > 1000)
                //{
                //    _graph.RemoveAtFirst(0);
                //}
                _graph.AddData(0, xData, yData);
            }

            count += 0.1;

            //if(count > 1000)
            //{
            //    count= 0;
            //}
        }

        private void BtnStartCommand()
        {
            if (_viewModels != null)
            {
                if (_viewModels.Count > 0)
                {
                    ClearGraph();
                    count = 0;
                    timer.Start();
                }
            }
        }

        private void BtnStopCommand()
        {
            if (_viewModels != null)
            {
                if (_viewModels.Count > 0)
                {
                    timer.Stop();
                }
            }
        }

        private void BtnClearCommand()
        {
            if (_viewModels != null)
            {
                if (_viewModels.Count > 0)
                {
                    ClearGraph();
                }
            }
        }

        private void ClearGraph()
        {
            foreach (GraphViewModel _graph in _viewModels)
            {
                _graph.Clear();
            }
        }

        private void BtnNewClick()
        {
            if (_viewModels != null)
            {
                if (_viewModels.Count < 8)
                {
                    _viewModels.Add(new GraphViewModel());
                }
            }
        }

        private void BtnDeleteClick()
        {
            _viewModels.Clear();
        }

        private void ListBoxSeletedChanged()
        {
            //double[] xData = new double[Convert.ToInt32(MaxGraphNum)];
            //double[] yData = new double[Convert.ToInt32(MaxGraphNum)];

            //double[] xData2 = new double[Convert.ToInt32(MaxGraphNum)];
            //double[] yData2 = new double[Convert.ToInt32(MaxGraphNum)];

            //if (_viewModels.Count > 0)
            //{
            //    foreach (GraphViewModel _graph in _viewModels)
            //    {
            //        _graph.Clear();
            //    }

            //    switch (SelectedInd)
            //    {
            //        case 0:
            //            for (int i = 0; i < MaxGraphNum; i++)
            //            {
            //                xData[i] = Math.Sin(i * 0.1f);
            //                yData[i] = Math.Cos(i * 0.1f);

            //                xData2[i] = Math.Sin(i * 0.1f) * 2;
            //                yData2[i] = Math.Cos(i * 0.1f) * 2;
            //            }
            //            break;
            //        case 1:
            //            for (int i = 0; i < MaxGraphNum; i++)
            //            {
            //                xData[i] = i;
            //                yData[i] = Math.Cos(i * 0.1f);

            //                xData2[i] = i;
            //                yData2[i] = Math.Cos(i * 0.1f) * 1000;
            //            }
            //            break;
            //        case 2:
            //            return;
            //    }

            //    foreach (GraphViewModel _graph in _viewModels)
            //    {
            //        _graph.AddData(0, xData, yData);
            //        _graph.AddData(1, xData2, yData2);
            //    }
            //}
        }

        private double currentTime = 0.0;
        public double CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));

                // update each graph's vertical lines
                foreach (GraphViewModel gm in _viewModels)
                    gm.SetverticalLineTrackerX(currentTime);
            }
        }
        private double minumumTime = 0.0;
        public double MinumumTime
        {
            get { return minumumTime; }
            set
            {
                minumumTime = value;
                OnPropertyChanged(nameof(MinumumTime));
            }
        }
        private double maximumTime = 5000.0;
        public double MaximumTime
        {
            get { return maximumTime; }
            set
            {
                maximumTime = value;
                OnPropertyChanged(nameof(MaximumTime));
            }
        }
    }
}
