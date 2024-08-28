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
    public class SelectionSort : ISortingAlgorithm
    {
        public string Name { get; }
        public string TimeComplexity { get; }
        public string AuxiliarySpace { get; }

        public SelectionSort()
        {
            Name = "Selection sort";
            TimeComplexity = "O(n\u00B2)"; // O^2
            AuxiliarySpace = "O(1)";
        }

        public async Task Sort(List<RectangleNode> list, MainWindow.SettingsSort settings, CancellationToken cancellationToken)
        {
            // Settings variables
            RichTextBox log = settings.Log;
            int shortDelay = settings.Delay;
            Brush contrastColor = settings.ContrastColor;

            int longDelay = shortDelay * 2;
            int length = list.Count;

            // Bubble sort process
            for (int i = 0; i < length - 1; i++)
            {
                int min = i;

                for (int j = i + 1; j < length; j++)
                {
                    // Check if cancellation is requested
                    cancellationToken.ThrowIfCancellationRequested();

                    await list[j].BlinkRectangle(contrastColor, shortDelay);

                    if (list[j].Value < list[min].Value)
                    {
                        min = j;
                    }
                }

                // Check if cancellation is requested
                cancellationToken.ThrowIfCancellationRequested();

                // Flip values
                (list[i], list[min]) = (list[min], list[i]);
                list[i].FlipRectangles(list[min]);

                // Value completly sorted
                await list[i].BlinkRectangle(contrastColor, longDelay);
                Log.Print(log, $" + Value {i + 1} sorted");
                await Task.Delay(shortDelay, cancellationToken);
            }

            // Last value completly sorted
            Log.Print(log, $" + Value {length} sorted");
            await list[^1].BlinkRectangle(contrastColor, longDelay);
        }
    }
}
