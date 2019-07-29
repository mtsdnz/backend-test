using System;

namespace Application.Helpers
{
    public static class ConsoleHelper
    {
        public static void WriteHeader(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"-------------------------{title}-------------------------");
            Console.WriteLine();
        }


        /*
            Os métodos: PrintLine, PrintRow & AlignCentre  foram obtidos em:
            https://stackoverflow.com/a/856918/4036646
         */

        static int tableWidth = 100;

        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        public static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
    }
}