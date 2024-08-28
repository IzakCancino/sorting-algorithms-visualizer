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
        public string AuxiliarySpace { get; }

        public BubbleSort() {
            Name = "Bubble sort";
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
            int steps = 0;

            // Bubble sort process
            for (int i = 0; i < length - 1; i++)
            {
                for (int j = 0; j < length - i - 1; j++)
                {
                    // Check if cancellation is requested
                    cancellationToken.ThrowIfCancellationRequested();

                    await list[j].BlinkRectangle(contrastColor, shortDelay);

                    // Flip values
                    if (list[j].Value > list[j + 1].Value)
                    {
                        (list[j], list[j + 1]) = (list[j + 1], list[j]);
                        list[j].FlipRectangles(list[j + 1]);
                        steps++;
                    }
                }

                // Check if cancellation is requested
                cancellationToken.ThrowIfCancellationRequested();

                // Value completly sorted
                await list[length - i - 1].BlinkRectangle(contrastColor, longDelay);
                Log.Print(log, $" + Value {length - i} sorted ({steps} steps)");
                steps = 0;
                await Task.Delay(shortDelay, cancellationToken);
            }

            // Last value completly sorted
            Log.Print(log, $" + Value 1 sorted ({steps} steps)");
            await list[0].BlinkRectangle(contrastColor, longDelay);
        }
    }
}
