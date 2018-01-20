using SiteCrower.Core;
using System;
using System.Text;

namespace SiteCrower.ConsoleClient
{
    class Crower
    {
        static string siteRoot;
        static RequestProcessor requestProcessor;

        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Site Root:");

            siteRoot = Console.ReadLine();

            try
            {
                requestProcessor = new RequestProcessor(siteRoot);
                requestProcessor.RequestProceed += RequestProcessor_RequestRequestProceed;
                requestProcessor.Finished += RequestProcessor_Finished;          
                requestProcessor.Start();
            }
            catch (ApplicationException appEx)
            {
                Console.WriteLine(appEx.Message);
            }

            Console.WriteLine();
        }

        private static void RequestProcessor_Finished(object sender, TimeSpan e)
        {
            WriteSummary();
            Console.WriteLine($"Finished in { string.Format("{0:0.0}", e.TotalSeconds)} seconds");
        }

        private static void RequestProcessor_RequestRequestProceed(object sender, ProcessResult e)
        {
            WriteSummary();

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

            Console.Write($"...{e.Status}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($" ({e.Finished.Milliseconds} ms)");
        }

        private static void WriteSummary()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();

            Console.WriteLine(siteRoot);
            Console.WriteLine();

            Console.WriteLine($"Url Processed: {requestProcessor.LinksProcessed} | Avg Responce: {requestProcessor.AvgResponseTime.TotalMilliseconds} ms | Avg Speed: {requestProcessor.AvgDownloadSpeed} KB/s");
            Console.WriteLine();
        }
    }
}
