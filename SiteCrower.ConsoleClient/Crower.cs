using SiteCrower.Core;
using System;

namespace SiteCrower.ConsoleClient
{
    class Crower
    {
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Site Root:");
            string root = Console.ReadLine();

            var requestProcessor = new RequestProcessor(root);
            requestProcessor.RequestProceed += RequestProcessor_RequestRequestProceed;
            requestProcessor.Start();
        }

        private static void RequestProcessor_RequestRequestProceed(object sender, ProcessResult e)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(e.Url);

            switch (e.Status)
            {
                case ProcessResultStatus.Ok:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ProcessResultStatus.Fail:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ProcessResultStatus.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(string.Format("...{0}", e.Status));
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
