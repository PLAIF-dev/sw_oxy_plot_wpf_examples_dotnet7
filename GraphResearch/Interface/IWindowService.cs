using GraphCtrlLib;
using GraphResearch.View;
using GraphResearch.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphResearch.Interface
{
    interface IWindowService
    {
        void ShowWindow() { }
        void Close() { }
    }

    class GraphWindowService : IWindowService
    {
        public void ShowWindow(object obj) 
        {
            var view = new GraphWindow()
            {
                DataContext = new GraphWindowVM(obj)  
            };
            view.Show();
        }
    }
}
