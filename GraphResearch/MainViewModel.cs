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
using System.Windows.Threading;
using System.Windows.Input;
using GraphResearch.Interface;
using GraphResearch.Model;
using System.Windows.Controls;
using GraphResearch.Utility;
using System.Drawing;
using System.Numerics;
using System.Windows;

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
        public ICommand BtnNewClick { get; set; }
        public ICommand BtnDeleteClick { get; set; }
        public ICommand BtnStartClick { get; set; }
        public ICommand BtnStopClick { get; set; }
        public ICommand BtnClearClick { get; set; }
        public ICommand TreeViewPreviewMouseLeftDown { get; set; }
        public ICommand TreeViewPreviewMouseLeftUp { get; set; }
        public ICommand TreeViewPreviewMouseMove { get; set; }

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

        public List<TreeNode> Roots { get; set; }
        
        private List<PositionNode> temporailySelectedPostions = new();

        private System.Windows.Point startpoint = new();

        public ObservableCollection<GraphModel.GraphDataSet> GraphDataSets { get; set; }

        private DispatcherTimer timer;
        #endregion

        public const double MaxGraphNum = 50;

        GraphWindowService _GraphWindowService = new GraphWindowService();

        public MainViewModel()
        {
            BtnNewClick = new RelayCommand(BtnNewCommand);
            BtnDeleteClick = new RelayCommand(BtnDeleteCommand);
            BtnStartClick = new RelayCommand(BtnStartCommand);
            BtnStopClick = new RelayCommand(BtnStopCommand);
            BtnClearClick = new RelayCommand(BtnClearCommand);
            TreeViewPreviewMouseLeftDown = new RelayCommand<object>(TreeViewPreviewMouseLeftDownCommand);
            TreeViewPreviewMouseLeftUp = new RelayCommand<object>(TreeViewPreviewMouseLeftUpCommand);
            TreeViewPreviewMouseMove = new RelayCommand<object>(TreeViewPreviewMouseMoveCommand);

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
                ID = 0,
                LineName = "lineA",
                DataX = xData,
                DataY = yData
            });
            GraphDataSets.Add(new GraphModel.GraphDataSet()
            {
                ID = 1,
                LineName = "lineB",
                DataX = xData2,
                DataY = yData2
            });
            GraphDataSets.Add(new GraphModel.GraphDataSet()
            {
                ID = 2,
                LineName = "lineC",
                DataX = xData2,
                DataY = yData2
            });

            //Timer 초기화
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5);
            timer.Tick += CallBackTimer;

            #region TreeNode
            Roots = new();

            TreeNode Root = new()
            {
                Name = "Joint",
            };

            Root.Children.Add( new PositionNode()
            {
                Name = "joint1",
                PositionsX = xData,
                PositionsY = yData
            });
            Root.Children.Add(new PositionNode()
            {
                Name = "joint2",
                PositionsX = xData2,
                PositionsY = yData2
            });
            Root.Children.Add(new PositionNode()
            {
                Name = "joint3",
                PositionsX = xData,
                PositionsY = yData
            });
            Root.Children.Add(new PositionNode()
            {
                Name = "joint4",
                PositionsX = xData2,
                PositionsY = yData2
            });
            Root.Children.Add(new PositionNode()
            {
                Name = "joint5",
                PositionsX = xData,
                PositionsY = yData
            });
            Root.Children.Add(new PositionNode()
            {
                Name = "joint6",
                PositionsX = xData2,
                PositionsY = yData2
            });

            Roots.Add(Root);

            #endregion

            #region Messenger

            var Messeenger = WeakReferenceMessenger.Default;
            Messeenger.Register<GraphCtrlLib.Message.SharedMessge>(this, OnMessageReceived);
            Messeenger.Register<GraphCtrlLib.Message.SharedSplitMessage>(this, OnSplitMessageReceived);
            Messeenger.Register<GraphCtrlLib.Message.SharedDeleteMessage>(this, OnDeleteMessageReceived);
            Messeenger.Register<GraphCtrlLib.Message.SharedNewWindowMessage>(this, OnNewWindowMessageReceived);
            #endregion

        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param> 객체 전송자
        /// <param name="message"></param> 메세지
        private void OnMessageReceived(object obj, GraphCtrlLib.Message.SharedMessge message)
        {
            double dataX = message.DataX;
            double dataY = message.DataY;

            double sDataX = message.sDataX;
            double sDataY = message.sDataY;

            var e = message.e;
            if (e != null)
            {
                int index = message.DataIndex;
                foreach (GraphViewModel _graph in _viewModels)
                {
                    _graph.SyncTracker(dataX, dataY, sDataX, sDataY, e, index);
                }
            }
        }

        private void OnSplitMessageReceived(object ojb, GraphCtrlLib.Message.SharedSplitMessage message)
        {
            int graphID = message.GraphID;
            List<string> linenamelist = message.LineName;
            int lineCount = linenamelist.Count;

            if (lineCount > 1)
            {
                //1. 현재 Graph 객체를 삭제한다.
                Delete_Graph(graphID);

                //2. line 수 만큼 Graph 객체를 생성한 후 line을 추가해준다.
                foreach (var linename in linenamelist)
                {
                    var result = GraphDataSets.First(x => x.LineName.Equals(linename));
                    Add_Graph();
                    GetLastGraph().AddData(linename, result.DataX, result.DataY);                  
                }
            }
        }

        private void OnDeleteMessageReceived(object obj, GraphCtrlLib.Message.SharedDeleteMessage message)
        {
            int graphID = message.GraphID;
            string graphName = message.GraphName;

            Delete_Graph(graphID);
        }

        private void OnNewWindowMessageReceived(object obj, GraphCtrlLib.Message.SharedNewWindowMessage message)
        {
            int graphID = message.GraphID;
            string graphName = message.GraphName;

            object? graph = GetGraph(graphID);

            if (graph != null)
            {
                _GraphWindowService.ShowWindow(graph);
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

        private void TreeViewPreviewMouseLeftDownCommand(object? parameter)
        {    
            try
            {
                if (parameter is MouseButtonEventArgs args)
                {
                    startpoint = args.GetPosition(null);

                    //if (args.Source is TreeView treeView)
                    //{
                    //    temporailySelectedPostions.Clear();
                    //    foreach (var item in treeView.ItemsSource)
                    //    {
                    //        if (item is TreeNode treeNode)
                    //        {
                    //            temporailySelectedPostions = TreeNodeUtility.TravelNode(treeNode);
                    //        }
                    //    }
                    //}
                }
            }
            catch
            {

            }
        }

        private void TreeViewPreviewMouseLeftUpCommand(object? parameter)
        {

        }

        private void TreeViewPreviewMouseMoveCommand(object? parameter)
        {
            try
            {
                if (parameter is MouseEventArgs args)
                {
                    System.Windows.Point mousePos = args.GetPosition(null);
                    System.Windows.Vector diff = startpoint - mousePos;

                    if (args.LeftButton == MouseButtonState.Pressed &&
                        (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(diff.Y) > SystemParameters.MinimumHorizontalDragDistance))
                    {
                        if (args.Source is TreeView treeView)
                        {
                            temporailySelectedPostions.Clear();

                            if(treeView.SelectedItem is TreeNode treeNode)
                            {
                                temporailySelectedPostions = TreeNodeUtility.TravelNode(treeNode);
                            }

                            if(temporailySelectedPostions.Count > 0)
                            {
                                List<object> graphDataSets = new();

                                foreach(var item in temporailySelectedPostions)
                                {
                                    GraphModel.GraphDataSet graphDataSet = new()
                                    {
                                        ID = 0,
                                        LineName = item.Name,
                                        DataX = item.PositionsX,
                                        DataY = item.PositionsY,
                                    };
                                    graphDataSets.Add(graphDataSet);
                                }
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
                                {
                                    DragDrop.DoDragDrop(treeView, graphDataSets, DragDropEffects.Copy);
                                }));
                            }
                        }
                    }
                }
            }
            catch
            {
                
            }
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

        private void BtnNewCommand()
        {
            Add_Graph();
        }

        private void BtnDeleteCommand()
        {
            Delete_Graph();
        }

        #region Graph 관리

        private GraphViewModel GetLastGraph()
        {
            return _viewModels.Last();    
        }

        private GraphViewModel? GetGraph(int _id)
        {
            return _viewModels.FirstOrDefault(x => x.ID == _id);
        }
        private void Add_Graph()
        {
            if (_viewModels != null)
            {
                if(_viewModels.Count <= 0)
                {
                    _viewModels.Add(new GraphViewModel(1)); //ID : 1
                }
                else
                {
                    if (_viewModels.Count < 8)
                    {
                        _viewModels.Add(new GraphViewModel(_viewModels.Last().ID + 1));
                    }
                }
            }
        }

        private void Delete_Graph()
        {
            _viewModels.Clear();
        }

        private void Delete_Graph(int _id)
        {
            foreach (GraphViewModel _graph in _viewModels)
            {
                if (_graph.ID == _id)
                {
                    _viewModels.Remove(_graph);
                    break;
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
        #endregion

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
