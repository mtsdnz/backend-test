using System;

namespace Application.Services
{
    public sealed class LoggerService : ILoggerService
    {
        public void Write(string msg)
        {
            WriteLine(msg);
        }

        public void WriteError(string error)
        {
            WriteLine(error, ConsoleColor.Red);
        }

        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            if (string.IsNullOrEmpty(message))
                return;

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }
    }
}