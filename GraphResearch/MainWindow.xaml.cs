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

        // Drag 판단 여부를 위한 마우스 포인트
        Point startpoint= new Point();

        // 임시로 선택된 항목을 저장할 리스트
        private List<object> temporarilySelectedItems = new List<object>();

        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startpoint = e.GetPosition(null);

            if(sender != null)
            {
                ListView? listView = sender as ListView;
                temporarilySelectedItems.Clear();

                if(listView != null && listView.SelectedItems.Count > 0)
                {
                    foreach (var item in listView.SelectedItems)
                    {
                        temporarilySelectedItems.Add(item);
                    }
                }
            }
        }

        private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Point mousePos = e.GetPosition(null);
                Vector diff = startpoint - mousePos;

                if (e.LeftButton == MouseButtonState.Pressed &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumHorizontalDragDistance))
                {
                    ListView? listview = null;

                    if (sender != null)
                    {
                        listview = sender as ListView;
                        listview.SelectedItems.Clear();

                        foreach(var item in temporarilySelectedItems)
                        {
                            listview.SelectedItems.Add(item);
                        }

                        var selectedItems = listview?.SelectedItems;
                        if (selectedItems != null && selectedItems.Count > 0)
                        {
                            List<object> graphDataSets = new List<object>();

                            foreach (var item in selectedItems)
                            {
                                graphDataSets.Add(item);
                            }
                            DragDrop.DoDragDrop(listview, graphDataSets, DragDropEffects.Copy);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
