using GraphCtrlLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
