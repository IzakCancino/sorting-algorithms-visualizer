using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace sorting_algorithms_visualizer
{
    /// <summary>
    /// Node that storage different information about a specific rectangle in the canvas
    /// </summary>
    public class RectangleNode
    {
        /// <summary>
        /// A integer number that represents the weight of the rectangle. It is used to sort them.
        /// </summary>
        public int Value { get; set; }
        public System.Windows.Shapes.Rectangle Rectangle { get; set; }

        public RectangleNode(System.Windows.Shapes.Rectangle rectangle, int value)
        {
            Rectangle = rectangle;
            Value = value;
        }

        /// <summary>
        /// Changes the position in canvas between two rectangles: the actual and the <c>otherNode</c>.
        /// </summary>
        /// <param name="otherNode">The rectangle with to change position.</param>
        public void FlipRectangles(RectangleNode otherNode)
        {
            double otherLeft = Canvas.GetLeft(otherNode.Rectangle);
            Canvas.SetLeft(otherNode.Rectangle, Canvas.GetLeft(this.Rectangle));
            Canvas.SetLeft(this.Rectangle, otherLeft);
        }

        /// <summary>
        /// Changes the color of the rectangle in the node for a specific amount of time.
        /// </summary>
        /// <param name="brush">The color which use for the blink.</param>
        public async Task BlinkRectangle(Brush brush) 
        {
            Brush tempBrush = Rectangle.Fill;
            Rectangle.Fill = brush;

            await Task.Delay(30);
            Rectangle.Fill = tempBrush;
        }
    }
}
