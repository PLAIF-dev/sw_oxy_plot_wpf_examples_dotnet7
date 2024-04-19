using GraphResearch.View;
using GraphResearch.ViewModel;

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
