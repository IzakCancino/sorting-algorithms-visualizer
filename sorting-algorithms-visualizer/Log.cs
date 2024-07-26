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
    public class Log
    {
        public static void Print(RichTextBox log, string message)
        {
            Paragraph p = new Paragraph();
            p.Inlines.Add(message);
            log.Document.Blocks.Add(p);
        }

        public static void PrintSuccess(RichTextBox log, string message)
        {
            Paragraph p = new Paragraph();
            p.Inlines.Add(message);
            p.Foreground = Brushes.LightGreen;
            log.Document.Blocks.Add(p);
        }

        public static void PrintAlert(RichTextBox log, string message)
        {
            Paragraph p = new Paragraph();
            p.Inlines.Add(message);
            p.Foreground = Brushes.Yellow;
            log.Document.Blocks.Add(p);
        }

        public static void PrintError(RichTextBox log, string message)
        {
            Paragraph p = new Paragraph();
            p.Inlines.Add(message);
            p.Foreground = Brushes.Red;
            log.Document.Blocks.Add(p);
        }
    }
}
