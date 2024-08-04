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

        /// <summary>
        /// Sort a specific list of <c>RectangleNode</c> type.
        /// </summary>
        /// <param name="list">List of nodes to sort.</param>
        /// <param name="log">Log text box used to print information about the process.</param>
        /// <param name="cancellationToken">Token used in case of the process is canceled by another one.</param>
        public Task Sort(List<RectangleNode> list, RichTextBox log, CancellationToken cancellationToken);
    }
}
