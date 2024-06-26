﻿using System.Collections.ObjectModel;
using GraphCtrlLib;

namespace GraphResearch.ViewModel
{
    public class GraphWindowVM
    {

        private ObservableCollection<GraphViewModel> _viewModels = new ObservableCollection<GraphViewModel>();

        public ObservableCollection<GraphViewModel> ViewModels
        {
            get { return _viewModels; }
        }

        public GraphWindowVM(object obj) 
        {
            if(obj is GraphViewModel)
            {
                GraphViewModel graph = (GraphViewModel)obj;
                GraphViewModel newGraph = new GraphViewModel(graph.ID, graph.Name);

                foreach(var sereies in graph.GetDicLineGraph())
                {
                    newGraph.AddData(sereies.Key, sereies.Value.Points);
                }

                _viewModels.Add(newGraph);
            }
        }
    }
}
