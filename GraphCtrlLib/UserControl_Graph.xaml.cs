using System.Windows;
using System.Windows.Controls;

namespace GraphCtrlLib
{
    /// <summary>
    /// UserControl_Graph.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserControl_Graph : UserControl
    {
        public UserControl_Graph()
        {
            InitializeComponent();
        }

        private void PlotView_DragOver(object sender, DragEventArgs e)
        {
            if(e.Data.GetData(typeof(GraphModel.GraphDataSet)) is GraphModel.GraphDataSet)
            {
                e.Effects= DragDropEffects.Copy;
            }
        }
    }
}
