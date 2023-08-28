using System.Windows;
using System.Windows.Input;

namespace OxyPlotTest
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

        private void officialBtn_Click(object sender, RoutedEventArgs e)
        {
            WpfExamples.MainWindow window = new();
            window.Show();
        }

        private void dynamicBtn_Click(object sender, RoutedEventArgs e)
        {
            DynamicCreateTest.MainWindow window = new();
            window.Show();
        }

        private void graphreseachBtn_Click(object sender, RoutedEventArgs e)
        {
            GraphResearch.MainWindow window = new();
            window.Show();
        }

        private void rostopicBtn_Click(object sender, RoutedEventArgs e)
        {
            RostopicTest.MainWindow window = new();
            window.Show();
        }
    }
}
