using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sorting_algorithms_visualizer
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

        private void GridSplitterPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Move the splitter to its default location
            RootGrid.ColumnDefinitions[0].Width = new GridLength(300);
        }

        private void GridSplitterPanel_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            // Save in a tag the horizontal location of the splitter
            int horizontalLocation = Convert.ToInt32(GridSplitterPanel.Tag.ToString());
            GridSplitterPanel.Tag = horizontalLocation + (int)e.HorizontalChange;
        }
    }
}