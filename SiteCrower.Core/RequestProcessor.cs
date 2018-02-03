using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SiteCrower.Core
{
    // TODO: 
    // 1. Make it asyncronous
    // 2. use HttpClient insead of WebClient
    // 3. VisitedUrls should be use instead of LinksProcessed
    public class RequestProcessor
    {
        private readonly string pattern = "href=\"";
        private readonly string[] escapeLinks = new string[] { "#", "/", "//" };

        private Queue<string> links;
        private WebClient webClient;
        private LinkDispatcher linkDispatcher;
        private readonly string root;
        private FileManager fileManager;
        
        private DateTime startTime;
        private int totalKBytesPerSecondReceived;
        private int totalDownloadSpeedsTimes;

        public event EventHandler<ProcessResult> RequestProceed;
        public event EventHandler<TimeSpan> Finished;
        
        public ICollection<string> VisitedUrls { get; private set; }

        public ICollection<string> ErrorUrls { get; private set; }

        public ICollection<string> FailedUrls { get; private set; }
        
        public TimeSpan AvgResponseTime { get { return TimeSpan.FromMilliseconds(this.totalDownloadSpeedsTimes / this.LinksProcessed); } }

        /// <summary>
        /// KB / s
        /// </summary>
        public int AvgDownloadSpeed { get { return this.totalKBytesPerSecondReceived / this.LinksProcessed; } }

        public int LinksProcessed { get; private set; }

        public RequestProcessor(string siteRoot)
        {
            this.webClient = new WebClient();
            this.root = this.EnsureProtocol(siteRoot);
            this.links = new Queue<string>(new string[] { root });
            this.VisitedUrls = new HashSet<string>();
            this.linkDispatcher = new LinkDispatcher(root);
            this.ErrorUrls = new HashSet<string>();
            this.FailedUrls = new HashSet<string>();
            this.fileManager = new FileManager(root);
        }

        public void Start()
        {
            startTime = DateTime.Now;

            string linkToVisit;
            string content;
            string childLink;
            int openIndex;
            int closeIndex;
            DateTime requestStart;

            while (this.links.Count > 0)
            {
                linkToVisit = this.links.Dequeue();
                requestStart = DateTime.Now;
                ProcessResult result = new ProcessResult() { Url = linkToVisit };

                try
                {
                    this.LinksProcessed++;

                    linkToVisit = this.linkDispatcher.DecorateUrl(linkToVisit);
                    content = this.Get(linkToVisit);

                    // Statistics
                    result.Finished = DateTime.Now - requestStart;
                    int kBytesPerSecond = (int)(((content.Length * sizeof(char)) / 1024) / result.Finished.TotalSeconds);
                    this.totalKBytesPerSecondReceived += kBytesPerSecond;
                    this.totalDownloadSpeedsTimes += (int)result.Finished.TotalMilliseconds;

                    openIndex = content.IndexOf(pattern) + pattern.Length;
                    while (openIndex != -1)
                    {
                        closeIndex = content.IndexOf("\"", openIndex + pattern.Length);
                        childLink = content.Substring(openIndex + pattern.Length, closeIndex - openIndex - pattern.Length);
                        if (ShouldVisit(childLink))
                        {
                            this.links.Enqueue(childLink);
                            this.VisitedUrls.Add(childLink);
                        }

                        openIndex++;
                        openIndex = content.IndexOf(pattern, openIndex);
                    }

                    result.Status = ProcessResultStatus.Ok;

                    // TODO: Test only
                    if (Settings.SaveContent)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(content);
                        this.fileManager.Save(bytes, linkToVisit);
                    }
                }
                catch (WebException)
                {
                    result.Status = ProcessResultStatus.Fail;
                    this.FailedUrls.Add(linkToVisit);
                }
                catch (Exception ex)
                {
                    result.Status = ProcessResultStatus.Error;
                    this.ErrorUrls.Add(linkToVisit);
                }

                this.RequestProceed?.Invoke(this, result);
            }

            this.Finished?.Invoke(this, DateTime.Now - startTime);
        }

        /// <summary>
        /// Visit only links in the current domain
        /// </summary>
        private bool ShouldVisit(string link)
        {
            if (!string.IsNullOrEmpty(link) && !escapeLinks.Contains(link) && !this.VisitedUrls.Contains(link))
            {
                if (this.linkDispatcher.GetDomain(link) == this.linkDispatcher.GetDomain(root))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Defines if the root is https, http or invalid
        /// </summary>
        private string EnsureProtocol(string siteRoot)
        {
            if (siteRoot.Contains(Constants.Http))
                return siteRoot;

            try
            {
                try
                {
                    string https = $"{Constants.Https}{siteRoot}";
                    this.webClient.DownloadString(https);

                    return https;
                }
                catch (WebException)
                {
                    string http = $"{Constants.Http}{siteRoot}";
                    this.webClient.DownloadString(http);

                    return http;
                }

            }
            catch (WebException webEx)
            {
                throw new ApplicationException("Invalid root address", webEx);
            }
        }

        private string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
