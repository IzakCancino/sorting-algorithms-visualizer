using sorting_algorithms_visualizer.SortingAlgorithms;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
        public List<RectangleNode> Rectangles;

        private CancellationTokenSource? _cancellationTokenSource;
        private bool _showLogAlert;

        public Dictionary<string, ColorPalette> ColorPalettes;
        public ColorPalette ActualColorPalette;

        /// <summary>
        /// Structure that storages all the settings, objects and values necessary to execute a sort algorithm.
        /// </summary>
        public struct SettingsSort
        {
            public RichTextBox Log { get; set; }
            public int Delay { get; set; }
            public Brush ContrastColor { get; set; }
        }

        /// <summary>
        /// Structure with the mask (to create the color) or contrast color used in the rectangles 
        /// </summary>
        public struct ColorPalette
        {
            public int[] MainMask { get; set; }
            public Brush ContrastColor { get; set; }
        }


        /// <summary>
        /// Suffles the rectangle nodes list
        /// </summary>
        /// <param name="list">A list of rectangle nodes in any order</param>
        /// <returns>A list of rectangle nodes in a completly random order</returns>
        public static List<RectangleNode> ShuffleRectangleNodes(List<RectangleNode> list)
        { 
            Random r = new Random();
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = r.Next(0, i + 1);
                (list[j], list[i]) = (list[i], list[j]);
                list[i].FlipRectangles(list[j]);
            }

            return list;
        }

        public MainWindow()
        {
            Rectangles = new List<RectangleNode>();

            // Filling the dictionary of color palettes with all the options of colors to use in the graph
            ColorPalettes = new Dictionary<string, ColorPalette>
            {
                {
                    "Green",
                    new ColorPalette()
                    {
                        MainMask = [0, 1, 0],
                        ContrastColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0))
                    }
                },
                {
                    "Yellow",
                    new ColorPalette()
                    {
                        MainMask = [1, 1, 0],
                        ContrastColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 230))
                    }
                },
                {
                    "Blue",
                    new ColorPalette()
                    {
                        MainMask = [0, 1, 1],
                        ContrastColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 85, 0))
                    }
                },
                {
                    "B&W",
                    new ColorPalette()
                    {
                        MainMask = [1, 1, 1],
                        ContrastColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0))
                    }
                }
            };

            InitializeComponent();
        }

        /// <summary>
        /// Starts the scramble process, by generating the determined number of rectagles in a random order
        /// </summary>
        private void BtnScramble_Click(object sender, RoutedEventArgs e)
        {
            // Validation of "values to sort" input. It must be an integer greater or equal than 2
            var value = InputNums.Text;
            if (value == null || !int.TryParse(value, out int num) || num < 2)
            {
                InputNums.Text = "10";
                InputNums.Focus();
                Log.PrintError(TextLog, "*** Error: Numbers to sort must be an integer greater or equal than 2 ***");
                return;
            }

            // Disable and enable specific elements
            InputNums.IsEnabled = true;
            SelectorSortingAlgorithm.IsEnabled = true;
            SliderSpeed.IsEnabled = true;
            BtnScramble.IsEnabled = true;
            BtnPlay.IsEnabled = true;
            BtnCancel.IsEnabled = false;

            // Starting process
            Log.Clear(TextLog);
            Log.PrintAlert(TextLog, "Starting population process...");

            // Restart the canvas
            CanvasGraph.Children.Clear();

            // Variables used
            int amountOfValues = Convert.ToInt32(InputNums.Text);
            Rectangles = new List<RectangleNode>();

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
            int scaleColor = (int)Math.Floor(255 / Math.Ceiling((double)amountOfValues / barSections));
            
            ActualColorPalette = ColorPalettes[
                ((ComboBoxItem)ColorSelector.SelectedItem)
                .Content.ToString() 
                ?? "Green"
            ];

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

                // Color shades based in scale
                int amountOfColor = scaleColor * countOfBarSections;

                mySolidColorBrush.Color = System.Windows.Media.Color.FromRgb(
                    (byte)(amountOfColor * ActualColorPalette.MainMask[0]),    
                    (byte)(amountOfColor * ActualColorPalette.MainMask[1]),    
                    (byte)(amountOfColor * ActualColorPalette.MainMask[2])
                );

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

                Rectangles.Add(new RectangleNode(rectangle, i));
            }

            Log.PrintSuccess(TextLog, $"Population process finished. {amountOfValues} values added.");

            // Shuffle list of nodes
            Rectangles = ShuffleRectangleNodes(Rectangles);
        }

        /// <summary>
        /// Start ordering the list <c>Rectangles</c> by using the selected sorting algorithm. This operation is asynchronous, and it can be cancelled.
        /// </summary>
        private async void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            // Disable and enable specific elements
            InputNums.IsEnabled = false;
            SelectorSortingAlgorithm.IsEnabled = false;
            SliderSpeed.IsEnabled = false;
            BtnScramble.IsEnabled = false;
            BtnPlay.IsEnabled = false;
            BtnCancel.IsEnabled = true;

            // Create a new CancellationTokenSource for the new sort operation
            _cancellationTokenSource = new CancellationTokenSource();

            // Sorting algorithm selected
            int selection = SelectorSortingAlgorithm.SelectedIndex;
            ISortingAlgorithm sortingAlgorithm;

            switch (selection)
            {
                case 0:
                    sortingAlgorithm = new BubbleSort();
                    break;
                default:
                    SelectorSortingAlgorithm.Focus();
                    Log.PrintError(TextLog, "*** Error: Invalid sorting algorithm selected ***");
                    return;
            }

            Log.PrintAlert(TextLog, $"Starting sorting process...");
            Log.Print(TextLog, $"Algorithm: {sortingAlgorithm.Name}");

            // Calculations of delate based in the speed input
            double speed = SliderSpeed.Value;
            int delay;
            if (speed == 5)
            {
                // Fastest execution. No delay
                delay = 0;
            }
            else if (speed < 1)
            {
                // More delay between steps
                delay = (int)Math.Ceiling(15 / speed);
            }
            else
            {
                // Less delay between steps
                delay = (int)Math.Ceiling((0.625 * Math.Pow(speed, 2)) - (6.25 * speed) + 15.725);
            }

            // Settings structure used in the sorting
            SettingsSort settings = new SettingsSort()
            {
                Log = TextLog,
                Delay = delay,
                ContrastColor = ActualColorPalette.ContrastColor
            };

            // Sort execution
            try
            {
                await sortingAlgorithm.Sort(Rectangles, settings, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Log.PrintError(TextLog, "Sorting execution stopped.");
            }
            catch (Exception ex)
            {
                Log.PrintError(TextLog, $"Error: {ex.Message}.");
            }
            finally
            {
                BtnCancel_Click(sender, e);
            }
        }

        /// <summary>
        /// Stops the asynchronous execution of the function <c>BtnPlay_Click()</c>
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Cancel the previous sort operation if it is still running
            _cancellationTokenSource?.Cancel();

            // Disable and enable specific elements
            InputNums.IsEnabled = true;
            SelectorSortingAlgorithm.IsEnabled = true;
            SliderSpeed.IsEnabled = true;
            BtnScramble.IsEnabled = true;
            BtnPlay.IsEnabled = false;
            BtnCancel.IsEnabled = false;
        }

        /// <summary>
        /// Changes the information about the selected algorithm
        /// </summary>
        private void SelectorSortingAlgorithm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selection = SelectorSortingAlgorithm.SelectedIndex;
            ISortingAlgorithm sortingAlgorithm;

            switch (selection)
            {
                case 0:
                    sortingAlgorithm = new BubbleSort();
                    break;
                default:
                    return;
            }

            TextTimeComplexity.Content = sortingAlgorithm.TimeComplexity;
            TextSpaceComplexity.Content = sortingAlgorithm.SpaceComplexity;
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!_showLogAlert)
            {
                var result = MessageBox.Show("Having the log tab opened while the sorting is being executed, could cause lag in the program.\nContinue showing this alert?", "Log Alert", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        _showLogAlert = true;
                        break;
                    case MessageBoxResult.Yes:
                        _showLogAlert = false;
                        break;
                    case MessageBoxResult.No:
                        _showLogAlert = true;
                        break;
                }
            }
            
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