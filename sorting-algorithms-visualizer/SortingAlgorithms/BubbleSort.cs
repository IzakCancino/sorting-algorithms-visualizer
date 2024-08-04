using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace sorting_algorithms_visualizer.SortingAlgorithms
{
    public class BubbleSort : ISortingAlgorithm
    {
        public string Name { get; }
        public string TimeComplexity { get; }
        public string SpaceComplexity { get; }

        public BubbleSort() {
            Name = "Bubble sort";
            TimeComplexity = "O(n\u00B2)"; // O^2
            SpaceComplexity = "O(1)";
        }

        public async Task Sort(List<RectangleNode> list, MainWindow.SettingsSort settings, CancellationToken cancellationToken)
        {
            RichTextBox log = settings.Log;
            int length = list.Count;

            for (int i = 0; i < length - 1; i++)
            {
                for (int j = 0; j < length - i - 1; j++)
                {
                    // Check if cancellation is requested
                    cancellationToken.ThrowIfCancellationRequested();

                    // Flip values
                    if (list[j].Value > list[j + 1].Value)
                    {
                        (list[j], list[j + 1]) = (list[j + 1], list[j]);
                        list[j].FlipRectangles(list[j + 1]);
                        Log.Print(log, $" - Values {i + 1} and {j + 1} flipped");

                        await Task.Delay(15, cancellationToken);
                    }
                }                

                // Check if cancellation is requested
                cancellationToken.ThrowIfCancellationRequested();

                Log.Print(log, $" + Value {length - i} sorted");
                await list[length - i - 1].BlinkRectangle(Brushes.Red);
            }

            Log.Print(log, " + Value 1 sorted");
            await list[0].BlinkRectangle(Brushes.Red);

            Log.PrintSuccess(log, $"Sorting process finished.");
        }
    }
}
