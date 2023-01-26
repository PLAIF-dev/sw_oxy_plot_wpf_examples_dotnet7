using CommunityToolkit.Mvvm.Input;
using GraphCtrlLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DynamicCreateTest
{
    class MainWindowViewModel : INotifyPropertyChanged
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

        private ObservableCollection<GraphViewModel> _viewModels = new ObservableCollection<GraphViewModel>();

        public ObservableCollection<GraphViewModel> ViewModels
        {
            get { return _viewModels; }
        }

        private object? _contentView;
        public object ContentView
        {
            get { return this._contentView; }
            set
            {
                this._contentView = value;
                NotifyPropertyChanged(nameof(ContentView));
            }
        }

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
        #endregion

        public MainWindowViewModel()
        {
            //this.ContentView = (object)new GraphCtrlLib.GraphViewModel();

            ListBoxChangedCommand = new RelayCommand(ListBoxSeletedChanged);
        }

        private void ListBoxSeletedChanged()
        {
            if (_viewModels.Count > 0)
            {
                foreach (GraphViewModel _graph in _viewModels)
                {
                    _graph.Clear();
                }
            }

            //int i = SelectedInd;
            double[] xData = new double[1000];
            double[] yData = new double[1000];

            double[] xData2 = new double[1000];
            double[] yData2 = new double[1000];

            switch (SelectedInd)
            {
                case 0:

                    break;
                case 1:
 
                    break;

                case 2:
                    _viewModels.Add(new GraphViewModel());

                    if(_viewModels.Count > 0)
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            xData[i] = Math.Sin(i);
                            yData[i] = Math.Cos(i);

                            xData2[i] = Math.Sin(i) * 2;
                            yData2[i] = Math.Cos(i) * 2;
                        }

                        foreach (GraphViewModel _graph in _viewModels)
                        {
                            _graph.AppendSeries(0, xData, yData);
                            //_graph.AppendSeries(1, xData2, yData2);
                        }
                    }

                    break;

                case 3:

                    if (_viewModels.Count > 1)
                    {
                        _viewModels.Clear();
                    }

                    break;
            }
            
        }
    }
}
