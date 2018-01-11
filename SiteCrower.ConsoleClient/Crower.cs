using SiteCrower.Core;
using System;
using System.Text;

namespace SiteCrower.ConsoleClient
{
    class Crower
    {
        static string siteRoot;
        static int urlProcessed = 0;
        static DateTime start;

        static void Main()
        {
            start = DateTime.Now;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Site Root:");

            siteRoot = Console.ReadLine();

            var requestProcessor = new RequestProcessor(siteRoot);
            requestProcessor.RequestProceed += RequestProcessor_RequestRequestProceed;
            requestProcessor.Start();
        }

        private static void RequestProcessor_RequestRequestProceed(object sender, ProcessResult e)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();

            Console.WriteLine(siteRoot);
            Console.WriteLine();

            Console.WriteLine($"Url processed: {++urlProcessed} @ {string.Format("{0:0.0}", (DateTime.Now - start).TotalSeconds)} seconds");
            Console.WriteLine();

            Console.Write($"Processing: {e.Url}");

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
