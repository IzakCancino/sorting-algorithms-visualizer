using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace sorting_algorithms_visualizer
{
    /// <summary>
    /// Static class with different methods to print information in the <c>RichTextBox</c> used as the log.
    /// </summary>
    public static class Log
    {
        public static void Print(RichTextBox log, string message)
        {            
            Paragraph p = new Paragraph();
            p.Inlines.Add(message);
            log.Document.Blocks.Add(p);
            log.ScrollToEnd();
        }

        public static void PrintSuccess(RichTextBox log, string message)
        {
            Paragraph p = new Paragraph();
            p.Inlines.Add(message);
            p.Foreground = Brushes.LightGreen;
            log.Document.Blocks.Add(p);
            log.ScrollToEnd();
        }

        public static void PrintAlert(RichTextBox log, string message)
        {
            Paragraph p = new Paragraph();
            p.Inlines.Add(message);
            p.Foreground = Brushes.Yellow;
            log.Document.Blocks.Add(p);
            log.ScrollToEnd();
        }

        public static void PrintError(RichTextBox log, string message)
        {
            Paragraph p = new Paragraph();
            p.Inlines.Add(message);
            p.Foreground = Brushes.Red;
            log.Document.Blocks.Add(p);
            log.ScrollToEnd();
        }

        public static void Clear(RichTextBox log)
        {
            log.Document.Blocks.Clear();
            Print(log, " = Sorting Algorithms Visualizer (Log) =");
            log.ScrollToEnd();
        }
    }
}
