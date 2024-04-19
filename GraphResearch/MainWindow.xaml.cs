using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using GraphCtrlLib;
using GraphCtrlLib.Message;
using GraphResearch.Model;
using GraphResearch.Utility;

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

        private object? lastClickedItem = null;

        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startpoint = e.GetPosition(null);

            if(sender is ListView listView)
            {
                temporarilySelectedItems.Clear();

                if(listView.SelectedItems.Count > 0)
                {
                    foreach (var item in listView.SelectedItems)
                    {
                        temporarilySelectedItems.Add(item);
                    }
                }

                if(listView.SelectedItems.Count > 1)
                {
                    var clickedItem = listView.InputHitTest(e.GetPosition(listView)) as DependencyObject;
                    while (clickedItem != null && !(clickedItem is ListViewItem))
                    {
                        clickedItem = VisualTreeHelper.GetParent(clickedItem);
                    }
                    lastClickedItem = (clickedItem as ListViewItem)?.DataContext;

                    // 이벤트 처리를 중단하여 기본 선택 동작을 막는다.
                    e.Handled = true;
                }
            }
        }
        private void ListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;

            if (sender is ListView listView)
            {
                if (lastClickedItem != null && listView.SelectedItems.Count > 1)
                {
                    //모든 선택을 취소하고 마지막에 클릭된 항목만 선택된다.
                    // 모든 선택을 취소하고 마지막에 클릭된 항목만 선택한다.
                    listView.SelectedItems.Clear();
                    listView.SelectedItems.Add(lastClickedItem);

                    // 마지막에 클릭된 항목을 초기화한다.
                    lastClickedItem = null;

                    // 이벤트 처리를 중단한다.
                    e.Handled = true;
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
                    if (sender is ListView listview)
                    {
                        if(listview.SelectedItems.Count == 1 && temporarilySelectedItems.Count == 1) 
                        {
                            //listview의 SelectedItems를 그대로 쓰도록 한다.
                        }
                        else
                        {
                            //listview의 SelectedItems을 초기화하고 temporarilySelectedItems값으로 다시 채워 놓는다.
                            listview.SelectedItems.Clear();

                            foreach (var item in temporarilySelectedItems)
                            {
                                listview.SelectedItems.Add(item);
                            }
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

        private void MultiSelectTreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startpoint = e.GetPosition(null);

            if (sender is MultiSelectTreeView treeView)
            {
                temporarilySelectedItems.Clear();

                if (treeView.SelectedItems.Count > 0)
                {
                    foreach (var item in treeView.SelectedItems)
                    {
                        temporarilySelectedItems.Add(item);
                    }
                }

                if (treeView.SelectedItems.Count > 1)
                {
                    var clickedItem = treeView.InputHitTest(e.GetPosition(treeView)) as DependencyObject;
                    while (clickedItem != null && !(clickedItem is MultiSelectTreeViewItem))
                    {
                        clickedItem = VisualTreeHelper.GetParent(clickedItem);
                    }
                    lastClickedItem = (clickedItem as MultiSelectTreeViewItem)?.DataContext;

                    // 이벤트 처리를 중단하여 기본 선택 동작을 막는다.
                    e.Handled = true;
                }
            }
        }

        private void MultiSelectTreeView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;

            if (sender is MultiSelectTreeView treeView)
            {
                if (lastClickedItem != null && treeView.SelectedItems.Count > 1)
                {
                    //모든 선택을 취소하고 마지막에 클릭된 항목만 선택된다.
                    // 모든 선택을 취소하고 마지막에 클릭된 항목만 선택한다.
                    treeView.SelectedItems.Clear();
                    treeView.SelectedItems.Add(lastClickedItem);

                    // 마지막에 클릭된 항목을 초기화한다.
                    lastClickedItem = null;

                    // 이벤트 처리를 중단한다.
                    e.Handled = true;
                }
            }
        }

        private void MultiSelectTreeView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Point mousePos = e.GetPosition(null);
                Vector diff = startpoint - mousePos;

                if (e.LeftButton == MouseButtonState.Pressed &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumHorizontalDragDistance))
                {
                    if (sender is MultiSelectTreeView treeview)
                    {
                        if (treeview.SelectedItems.Count == 1 && temporarilySelectedItems.Count == 1)
                        {
                            //treeview의 SelectedItems를 그대로 쓰도록 한다.
                        }
                        else
                        {
                            //treeview의 SelectedItems을 초기화하고 temporarilySelectedItems값으로 다시 채워 놓는다.
                            treeview.SelectedItems.Clear();

                            foreach (var item in temporarilySelectedItems)
                            {
                                treeview.SelectedItems.Add(item);
                            }
                        }

                        var selectedItems = treeview?.SelectedItems;
                        if (selectedItems != null && selectedItems.Count > 0)
                        {
                            List<PositionNode> tempPosionNodes = new();
                            List<object> graphDataSets = new();

                            foreach (var item in selectedItems)
                            {
                                List<PositionNode> templist = TreeNodeUtility.TravelNode((TreeNode)item);

                                foreach (var item2 in templist)
                                {
                                    if (!tempPosionNodes.Exists(x => x.Name == item2.Name))
                                    {
                                        tempPosionNodes.Add(item2);
                                    }
                                }
                            }

                            foreach (var item in tempPosionNodes) 
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

                            DragDrop.DoDragDrop(treeview, graphDataSets, DragDropEffects.Copy);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MultiSelectTreeView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is MultiSelectTreeView treeview)
            {
                var selectedItem = treeview?.LastSelectedItem;
                if (selectedItem != null)
                {
                    List<PositionNode> tempPosionNodes = new();
                    List<object> graphDataSets = new();

                    List<PositionNode> templist = TreeNodeUtility.TravelNode((TreeNode)selectedItem);

                    foreach (var item in templist)
                    {
                        if (!tempPosionNodes.Exists(x => x.Name == item.Name))
                        {
                            tempPosionNodes.Add(item);
                        }
                    }
                    
                    foreach (var item in tempPosionNodes)
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

                    WeakReferenceMessenger.Default.Send(new SharedAddLineMessage
                    {
                        GraphDataSets = graphDataSets,
                    });
                }
            }
        }
    }
}
