using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphCtrlLib;

namespace DynamicCreateTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListView) 
            {
                ListView item = (ListView)sender;
                GraphModel.GraphDataSet graphDataSet = (GraphModel.GraphDataSet)item.SelectedValue;
                {
                    DragDrop.DoDragDrop(item, graphDataSet, DragDropEffects.Copy);
                }
            }
        }
    }
}
