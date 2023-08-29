using GraphCtrlLib;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphResearch
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
            //if(sender is ListBoxItem listBox && listBox.Content is GraphModel.GraphDataSet graphDataSet)
            ListView item = new ListView();
            item = sender as ListView;

            GraphModel.GraphDataSet graphDataSet = (GraphModel.GraphDataSet)item.SelectedValue;
            {
                DragDrop.DoDragDrop(item, graphDataSet, DragDropEffects.Copy);
            }
        }
    }
}
