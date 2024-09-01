using sorting_algorithms_visualizer.SortingAlgorithms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Timers;
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
        private List<RectangleNode> _rectangles = [];
        private ColorPalette _actualColorPalette;

        private ISortingAlgorithm _selectedSortingAlgorithm = new BubbleSort();
        private CancellationTokenSource? _cancellationTokenSource;



        /// <summary>
        /// Structure that storages all the settings, objects and values necessary to execute a sort algorithm.
        /// </summary>
        public struct SettingsSort
        {
            public RichTextBox Log { get; }
            public int Delay { get; }
            public Brush ContrastColor { get; }
            public double YFirstBar { get; }
            public double YDifferenceBar { get; }

            public SettingsSort(RichTextBox log, int delay, Brush contrastColor, double yFirstBar, double yDifferenceBar)
            {
                Log = log;
                Delay = delay;
                ContrastColor = contrastColor;
                YFirstBar = yFirstBar;
                YDifferenceBar = yDifferenceBar;
            }
        }

        /// <summary>
        /// Structure with the mask (to create the color) or contrast color used in the rectangles.
        /// </summary>
        public struct ColorPalette
        {
            public int[] MainMask { get; }
            public Brush ContrastColor { get; }

            public ColorPalette(int[] mainMask, Brush contrastColor)
            {
                MainMask = mainMask;
                ContrastColor = contrastColor;
            }
        }



        /// <summary>
        /// Suffles the rectangle nodes list.
        /// </summary>
        /// <param name="list">A list of rectangle nodes in any order.</param>
        /// <returns>A list of rectangle nodes in a completly random order.</returns>
        private static List<RectangleNode> ShuffleRectangleNodes(List<RectangleNode> list)
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

        /// <summary>
        /// It gets the color palette selected by the user to use in the graph.
        /// </summary>
        /// <param name="selection">The index of the user's selection.</param>
        /// <returns>The color palette selected.</returns>
        private static ColorPalette GetSelectedColorPalette(int selection)
        {
            return selection switch
            {
                // Green
                0 => new ColorPalette(
                        mainMask: [0, 1, 0],
                        contrastColor: new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0))
                    ),
                // Yellow
                1 => new ColorPalette(
                        mainMask: [1, 1, 0],
                        contrastColor: new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 230))
                    ),
                // Blue
                2 => new ColorPalette(
                        mainMask: [0, 1, 1],
                        contrastColor: new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 85, 0))
                    ),
                // B&W
                3 => new ColorPalette(
                        mainMask: [1, 1, 1],
                        contrastColor: new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0))
                    ),
                // Default: Green
                _ => new ColorPalette(
                        mainMask: [0, 1, 0],
                        contrastColor: new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0))
                    ),
            };
        }

        /// <summary>
        /// It gets the sorting algorithm selected by the user to execute in the graph.
        /// </summary>
        /// <param name="selection">The index of the user's selection.</param>
        /// <returns>A new object of the class based in the algorithm selected.</returns>
        private static ISortingAlgorithm GetSelectedSortingAlgorithm(int selection)
        {
            return selection switch
            {
                0 => new BubbleSort(),
                1 => new SelectionSort(),
                2 => new InsertionSort(),
                3 => new MergeSort(),
                _ => new BubbleSort(),
            };
        }



        /// <summary>
        /// Window initiatilization.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Starts the scramble process, by generating the determined number of rectagles in a random order.
        /// </summary>
        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
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
            ColorSelector.IsEnabled = true;
            BtnGenerate.IsEnabled = true;
            BtnPlay.IsEnabled = true;
            BtnCancel.IsEnabled = false;
            LabelSortingTime.Content = string.Empty;
            LabelSortingTime.Visibility = Visibility.Visible;

            // Starting process
            Log.Clear(TextLog);
            Log.PrintAlert(TextLog, "Starting population process...");

            // Restart the canvas
            CanvasGraph.Children.Clear();

            // Variables used
            int amountOfValues = Convert.ToInt32(InputNums.Text);
            _rectangles = new List<RectangleNode>();

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
            
            _actualColorPalette = GetSelectedColorPalette(ColorSelector.SelectedIndex);

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
                    (byte)(amountOfColor * _actualColorPalette.MainMask[0]),    
                    (byte)(amountOfColor * _actualColorPalette.MainMask[1]),    
                    (byte)(amountOfColor * _actualColorPalette.MainMask[2])
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

                _rectangles.Add(new RectangleNode(rectangle, i));
            }

            Log.PrintSuccess(TextLog, $"Population process finished. {amountOfValues} values added.");

            // Shuffle list of nodes
            _rectangles = ShuffleRectangleNodes(_rectangles);
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
            ColorSelector.IsEnabled = false;
            BtnGenerate.IsEnabled = false;
            BtnPlay.IsEnabled = false;
            BtnCancel.IsEnabled = true;

            // Create a new CancellationTokenSource for the new sort operation
            _cancellationTokenSource = new CancellationTokenSource();

            Log.PrintAlert(TextLog, $"Starting sorting process...");
            Log.Print(TextLog, $"Algorithm: {_selectedSortingAlgorithm.Name}");

            // Calculations of delay based in the speed input
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
                delay = (int)Math.Ceiling(30 / speed);
            }
            else
            {
                // Less delay between steps
                delay = (int)Math.Ceiling((-9.58 * speed) + 39.4);
            }

            // Settings structure used in the sorting
            SettingsSort settings = new SettingsSort(
                log: TextLog,
                delay: delay,
                contrastColor: _actualColorPalette.ContrastColor,
                yFirstBar: Canvas.GetLeft(_rectangles[0].Rectangle),
                yDifferenceBar: Canvas.GetLeft(_rectangles[1].Rectangle) - Canvas.GetLeft(_rectangles[0].Rectangle)
            );

            // Sort execution
            var timer = new Stopwatch();
            timer.Start();
            try
            {
                await _selectedSortingAlgorithm.Sort(_rectangles, settings, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Log.PrintError(TextLog, "Sorting execution stopped.");
                LabelSortingTime.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Log.PrintError(TextLog, $"Error: {ex.Message}.");
                LabelSortingTime.Visibility = Visibility.Hidden;
            }
            finally
            {
                Log.PrintSuccess(TextLog, $"Sorting process finished.");

                // Sorting timer stopped
                timer.Stop();
                TimeSpan timeTaken = timer.Elapsed;

                if (timeTaken.Minutes != 0)
                {
                    LabelSortingTime.Content =
                        "Sorting time: "
                        + timeTaken.Minutes + "m "
                        + timeTaken.Seconds + "s "
                        + timeTaken.Milliseconds + "ms ";
                }
                else if (timeTaken.Seconds != 0)
                {
                    LabelSortingTime.Content =
                        "Sorting time: "
                        + timeTaken.Seconds + "s "
                        + timeTaken.Milliseconds + "ms ";
                }
                else
                {
                    LabelSortingTime.Content =
                        "Sorting time: "
                        + timeTaken.Milliseconds + "ms "
                        + timeTaken.Microseconds + "us "
                        + timeTaken.Nanoseconds + "ns";
                }

                BtnCancel_Click(sender, e);
            }
        }

        /// <summary>
        /// Stops the asynchronous execution of the function <c>BtnPlay_Click()</c>.
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Cancel the previous sort operation if it is still running
            _cancellationTokenSource?.Cancel();

            // Disable and enable specific elements
            InputNums.IsEnabled = true;
            SelectorSortingAlgorithm.IsEnabled = true;
            SliderSpeed.IsEnabled = true;
            ColorSelector.IsEnabled = true;
            BtnGenerate.IsEnabled = true;
            BtnPlay.IsEnabled = false;
            BtnCancel.IsEnabled = false;
        }

        /// <summary>
        /// Changes the information about the selected algorithm.
        /// </summary>
        private void SelectorSortingAlgorithm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSortingAlgorithm = GetSelectedSortingAlgorithm(SelectorSortingAlgorithm.SelectedIndex);

            TextTimeComplexity.Content = _selectedSortingAlgorithm.TimeComplexity;
            TextAuxiliarySpace.Content = _selectedSortingAlgorithm.AuxiliarySpace;
        }

        /// <summary>
        /// Used when attempted to enable or disable the log.
        /// </summary>
        private void CheckBoxLog_Click(object sender, RoutedEventArgs e)
        {
            // Enabling the log
            if (CheckBoxLog.IsChecked ?? false)
            {
                var result = MessageBox.Show("Having the log execution enabled could slow down the program, and it could add extra time to the sorting timer.\nDo you want to continue?", "Log Alert", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Log.IsEnabled = true;
                    TextLog.IsEnabled = true;
                    Log.Clear(TextLog);
                    return;
                }

                CheckBoxLog.IsChecked = false;
            }

            TextLog.Document.Blocks.Clear();
            Paragraph p = new Paragraph();
            p.Inlines.Add("Log disabled");
            TextLog.Document.Blocks.Add(p);

            TextLog.IsEnabled = false;
            Log.IsEnabled = false;
        }

        /// <summary>
        /// Resets the grid width size.
        /// </summary>
        private void GridSplitterPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Move the splitter to its default location
            RootGrid.ColumnDefinitions[0].Width = new GridLength(300);
        }

        /// <summary>
        /// Updates the tag about the location of the grid splitter.
        /// </summary>
        private void GridSplitterPanel_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            // Save in a tag the horizontal location of the splitter
            int horizontalLocation = Convert.ToInt32(GridSplitterPanel.Tag.ToString());
            GridSplitterPanel.Tag = horizontalLocation + (int)e.HorizontalChange;
        }
    }
}