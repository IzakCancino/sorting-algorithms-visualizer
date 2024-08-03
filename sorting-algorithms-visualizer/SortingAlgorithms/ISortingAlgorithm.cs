using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace sorting_algorithms_visualizer.SortingAlgorithms
{
    interface ISortingAlgorithm
    {
        public string Name { get; }
        public string TimeComplexity { get; }
        public string SpaceComplexity { get; }

        public Task Sort(List<RectangleNode> list, RichTextBox log, CancellationToken cancellationToken);
    }
}
