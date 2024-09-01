using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace sorting_algorithms_visualizer.SortingAlgorithms
{
    internal class MergeSort : ISortingAlgorithm
    {
        public string Name { get; }
        public string TimeComplexity { get; }
        public string AuxiliarySpace { get; }

        public MergeSort()
        {
            Name = "Merge sort";
            TimeComplexity = "O(n log(n))";
            AuxiliarySpace = "O(n)";
        }

        // Merges a list based in two ranges sorting them
        static async Task<List<RectangleNode>> Merge(List<RectangleNode> list, int indexLeftBorder, int indexMiddle, int indexRightBorder, MainWindow.SettingsSort settings, CancellationToken cancellationToken)
        {
            // Settings variables
            RichTextBox log = settings.Log;
            int shortDelay = settings.Delay;
            Brush contrastColor = settings.ContrastColor;

            // Find sizes of two sublists to be merged
            int lengthLeft = indexMiddle - indexLeftBorder + 1;
            int lengthRight = indexRightBorder - indexMiddle;

            // Create temp lists
            List<RectangleNode> listLeft = new List<RectangleNode>();
            List<RectangleNode> listRight = new List<RectangleNode>();

            int countLeft;
            int countRight;

            // Copy data to temp lists
            for (countLeft = 0; countLeft < lengthLeft; ++countLeft)
            {
                listLeft.Add(list[indexLeftBorder + countLeft]);
                await list[indexLeftBorder + countLeft].BlinkRectangle(contrastColor, shortDelay);

                // Check if cancellation is requested
                cancellationToken.ThrowIfCancellationRequested();
            }
            for (countRight = 0; countRight < lengthRight; ++countRight)
            {
                listRight.Add(list[indexMiddle + 1 + countRight]);
                await list[indexMiddle + 1 + countRight].BlinkRectangle(contrastColor, shortDelay);

                // Check if cancellation is requested
                cancellationToken.ThrowIfCancellationRequested();
            }

            //
            // Merge the temp lists
            //

            // Initial index of sublists
            countLeft = 0;
            countRight = 0;

            // Initial index of merged list
            int indexActual = indexLeftBorder;

            while (countLeft < lengthLeft && countRight < lengthRight)
            {
                if (listLeft[countLeft].Value <= listRight[countRight].Value)
                {
                    list[indexActual] = listLeft[countLeft];
                    countLeft++;
                }
                else
                {
                    list[indexActual] = listRight[countRight];
                    countRight++;
                }
                indexActual++;

                // Check if cancellation is requested
                cancellationToken.ThrowIfCancellationRequested();
            }

            // Copy remaining elements of L[] if any
            while (countLeft < lengthLeft)
            {
                list[indexActual] = listLeft[countLeft];

                countLeft++;
                indexActual++;

                // Check if cancellation is requested
                cancellationToken.ThrowIfCancellationRequested();
            }

            // Copy remaining elements of R[] if any
            while (countRight < lengthRight)
            {
                list[indexActual] = listRight[countRight];

                countRight++;
                indexActual++;

                // Check if cancellation is requested
                cancellationToken.ThrowIfCancellationRequested();
            }

            return list;
        }

        // General function that sorts the list splitting them in two by using the Merge() function
        public async Task<List<RectangleNode>> General(List<RectangleNode> list, int indexLeftBorder, int indexRightBorder, MainWindow.SettingsSort settings, CancellationToken cancellationToken)
        {
            if (indexLeftBorder < indexRightBorder)
            {
                // Check if cancellation is requested
                cancellationToken.ThrowIfCancellationRequested();

                // Find the middle point
                int indexMiddle = indexLeftBorder + (indexRightBorder - indexLeftBorder) / 2;

                // Sort first and second halves
                list = await General(list, indexLeftBorder, indexMiddle, settings, cancellationToken);
                list = await General(list, indexMiddle + 1, indexRightBorder, settings, cancellationToken);

                // Check if cancellation is requested
                cancellationToken.ThrowIfCancellationRequested();

                // Merge the sorted halves
                list = await Merge(list, indexLeftBorder, indexMiddle, indexRightBorder, settings, cancellationToken);

                // Reorder rectangles in canvas
                for (int i = indexLeftBorder; i <= indexRightBorder; i++)
                {
                    Canvas.SetLeft(list[i].Rectangle, settings.YFirstBar + (settings.YDifferenceBar * i));

                    if (indexLeftBorder == 0 && indexRightBorder == list.Count - 1)
                    {
                        // Value completly sorted
                        await list[i].BlinkRectangle(settings.ContrastColor, settings.Delay * 2);
                        Log.Print(settings.Log, $" + Value {i + 1} sorted");
                        await Task.Delay(settings.Delay, cancellationToken);
                    }
                    else
                    {
                        // Value in sublist sorted
                        await list[i].BlinkRectangle(settings.ContrastColor, settings.Delay);
                    }
                    
                    // Check if cancellation is requested
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }

            return list;
        }

        public async Task Sort(List<RectangleNode> list, MainWindow.SettingsSort settings, CancellationToken cancellationToken)
        {
            // Merge sort process
            await General(list, 0, list.Count - 1, settings, cancellationToken);
        }
    }
}
