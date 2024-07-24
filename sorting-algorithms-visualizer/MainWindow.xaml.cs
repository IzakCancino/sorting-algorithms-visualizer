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

        private void GridSplitterConsole_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            int verticalLocation = Convert.ToInt32(GridSplitterConsole.Tag.ToString());
            int newVerticalLocation = verticalLocation + (int)e.VerticalChange;

            if (newVerticalLocation > 0)
            {
                // Console tab too closed
                RootGrid.RowDefinitions[1].Height = new GridLength(28);
                GridSplitterConsole.Tag = 0;
                return;
            }

            GridSplitterConsole.Tag = newVerticalLocation;
        }

        private void TabItemConsole_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int verticalLocation = Convert.ToInt32(GridSplitterConsole.Tag.ToString());

            if (verticalLocation >= 0)
            {
                // Show the Console tab
                RootGrid.RowDefinitions[1].Height = new GridLength(120);
                GridSplitterConsole.Tag = -92;
            }
            else if (verticalLocation < 0)
            {
                // Hide the Console tab
                RootGrid.RowDefinitions[1].Height = new GridLength(28);
                GridSplitterConsole.Tag = 0;
            }
        }
    }
}