// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using WpfExamples;

namespace RealtimeDemo
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example("Plotting a curve that updates automatically.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.DataContext = new MainViewModel();
        }
    }
}