using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            Log.PrintAlert(TextLog, "Starting population process...");

            // Restart the canvas
            CanvasGraph.Children.Clear();

            // Variables used
            int amountOfValues = Convert.ToInt32(InputNums.Text);
            System.Windows.Shapes.Rectangle[] rectangles = {};

            // Absolute dimensions of the canva
            double totalWidth = CanvasGraph.ActualWidth;
            double totalHeight = CanvasGraph.ActualHeight;
            double widthMargin = totalWidth * 0.05;
            double heightMargin = totalHeight * 0.05;
            double widthOfEachRectangle = (totalWidth - (widthMargin * 4)) / ((amountOfValues * 2) - 1);
            double heightOfEachRectangle = (totalHeight - heightMargin * 4) / amountOfValues;

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

            // Color bars variables
            int barSections = (int)Math.Ceiling(amountOfValues / 51d);
            int countBarsInActualSection = 0;
            int countOfBarSections = 1;
            int scaleColor = (int)Math.Floor(255 / Math.Ceiling((double) amountOfValues / barSections));

            // Populate the canva with rectangles
            for (int i = 0; i < amountOfValues; i++)
            { 
                // Colorizing rectangles
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();

                countBarsInActualSection++;
                if (countBarsInActualSection > barSections)
                {
                    // A bar section completed, so the color scale is updated
                    countOfBarSections++;
                    countBarsInActualSection = 1;
                }

                // Green shades color based in scale
                mySolidColorBrush.Color = System.Windows.Media.Color.FromRgb(0, (byte)(scaleColor * countOfBarSections), (byte) countOfBarSections);

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

            Log.PrintSuccess(TextLog, $"Population process finished. {amountOfValues} values added.");
        }

        private void InputNums_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Validation of "values to sort" input. It must be an integer greater or equal than 2
            var value = InputNums.Text;

            if (value != null && int.TryParse(value, out int num) && num >= 2)
            {
                BtnPlay.IsEnabled = true;
                return;
            }

            BtnPlay.IsEnabled = false;
            InputNums.Text = "10";
            InputNums.Focus();
        }
    }
}