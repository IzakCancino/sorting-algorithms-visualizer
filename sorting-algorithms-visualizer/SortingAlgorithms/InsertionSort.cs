using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace sorting_algorithms_visualizer.SortingAlgorithms
{
    internal class InsertionSort : ISortingAlgorithm
    {
        public string Name { get; }
        public string TimeComplexity { get; }
        public string AuxiliarySpace { get; }

        public InsertionSort()
        {
            Name = "Insertion sort";
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

            // Insertion sort process
            for (int i = 1; i < length; i++)
            {
                for (int j = i; j > 0; j--)
                {
                    // Check if cancellation is requested
                    cancellationToken.ThrowIfCancellationRequested();

                    await list[j].BlinkRectangle(contrastColor, shortDelay);

                    if (list[j].Value < list[j - 1].Value)
                    {
                        // Flip values
                        (list[j], list[j - 1]) = (list[j - 1], list[j]);
                        list[j].FlipRectangles(list[j - 1]);

                        steps++;
                        continue;
                    }

                    // Check if cancellation is requested
                    cancellationToken.ThrowIfCancellationRequested();

                    // Value completly sorted
                    await list[j - 1].BlinkRectangle(contrastColor, longDelay);
                    Log.Print(log, $" + Value {i} sorted ({steps} steps)");
                    await Task.Delay(shortDelay, cancellationToken);

                    steps = 0;
                    break;
                }
            }
        }
    }
}
