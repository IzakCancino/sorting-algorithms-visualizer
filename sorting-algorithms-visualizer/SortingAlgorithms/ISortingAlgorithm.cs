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
        public string AuxiliarySpace { get; }

        /// <summary>
        /// Sort a specific list of <c>RectangleNode</c> type.
        /// </summary>
        /// <param name="list">List of nodes to sort.</param>
        /// <param name="settings">Structure with different setting to be used in the execution.</param>
        /// <param name="cancellationToken">Token used in case of the process is canceled by another one.</param>
        public Task Sort(List<RectangleNode> list, MainWindow.SettingsSort settings, CancellationToken cancellationToken);
    }
}
