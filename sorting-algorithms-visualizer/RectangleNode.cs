using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace sorting_algorithms_visualizer
{
    public class RectangleNode
    {
        // Nodes used to storage the rectangles information
        public int Value { get; set; }
        public System.Windows.Shapes.Rectangle Rectangle { get; set; }

        public RectangleNode(System.Windows.Shapes.Rectangle rectangle, int value)
        {
            Rectangle = rectangle;
            Value = value;
        }

        public void FlipRectangles(RectangleNode otherNode)
        {
            // Changes the position in canvas between two rectangles
            double otherLeft = Canvas.GetLeft(otherNode.Rectangle);
            Canvas.SetLeft(otherNode.Rectangle, Canvas.GetLeft(this.Rectangle));
            Canvas.SetLeft(this.Rectangle, otherLeft);
        }

        public async Task BlinkRectangle(Brush brush) 
        {
            Brush tempBrush = Rectangle.Fill;
            Rectangle.Fill = brush;

            await Task.Delay(30);
            Rectangle.Fill = tempBrush;
        }
    }
}
