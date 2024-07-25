using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
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

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            // Restart the canvas
            CanvasGraph.Children.Clear();

            // Variables used
            Random rand = new Random();
            int NUMS_USED = rand.Next(2, 100);
            System.Windows.Shapes.Rectangle[] rectangles = {};

            // Absolute dimensions of the canva
            double totalWidth = CanvasGraph.ActualWidth;
            double totalHeight = CanvasGraph.ActualHeight;
            double widthMargin = totalWidth * 0.05;
            double heightMargin = totalHeight * 0.05;
            double widthOfEachRectangle = (totalWidth - (widthMargin * 4)) / ((NUMS_USED * 2) - 1);
            double heightOfEachRectangle = (totalHeight - heightMargin * 4) / NUMS_USED;

            // Creation of the main background
            System.Windows.Shapes.Rectangle mainArea = new System.Windows.Shapes.Rectangle()
            {
                Width = totalWidth - (widthMargin * 2),
                Height = totalHeight - (heightMargin * 2),
                Fill = Brushes.White
            };

            CanvasGraph.Children.Add(mainArea);
            Canvas.SetLeft(mainArea, widthMargin);
            Canvas.SetBottom(mainArea, heightMargin);

            // Populate the canva with rectangles
            for (int i = 0; i < NUMS_USED; i++)
            { 
                // Random colors
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = System.Windows.Media.Color.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));

                // Rectangle creation and settings
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle()
                {
                    Width = widthOfEachRectangle,
                    Height = heightOfEachRectangle * (i + 1),
                    Fill = mySolidColorBrush                    
                };

                CanvasGraph.Children.Add(rectangle);

                Canvas.SetBottom(rectangle, heightMargin * 2);
                Canvas.SetLeft(rectangle, (widthMargin * 2) + (widthOfEachRectangle * i * 2));

                rectangles.Append(rectangle);
            }
        }
    }
}