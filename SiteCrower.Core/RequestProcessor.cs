using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SiteCrower.Core
{
    // TODO: Make it asyncronous
    public class RequestProcessor
    {
        private readonly string pattern = "href=\"";
        private readonly string[] escapeLinks = new string[] { "#", "/", "//" };

        private Queue<string> links;
        private WebClient webClient;
        private IList<string> visited;
        private LinkDispatcher linkDispatcher;
        private readonly string root;

        public event EventHandler<ProcessResult> RequestProceed;

        public RequestProcessor(string siteRoot)
        {
            this.links = new Queue<string>(new string[] { siteRoot });
            this.webClient =  new WebClient();
            this.visited = new List<string>();
            this.root = siteRoot;
            this.linkDispatcher = new LinkDispatcher(siteRoot);
        }

        public void Start()
        {
            string linkToVisit;
            string content;
            string childLink;
            int openIndex;
            int closeIndex;

            while (links.Count > 0)
            {
                linkToVisit = links.Dequeue();
                ProcessResult result = new ProcessResult() { Url = linkToVisit };

                try
                {
                    linkToVisit = linkDispatcher.DecorateUrl(linkToVisit);                   
                    content = webClient.DownloadString(linkToVisit);

                    openIndex = content.IndexOf(pattern) + pattern.Length;
                    while (openIndex != -1)
                    {
                        closeIndex = content.IndexOf("\"", openIndex + pattern.Length);
                        childLink = content.Substring(openIndex + pattern.Length, closeIndex - openIndex - pattern.Length);
                        if (ShouldVisit(childLink))
                        {
                            links.Enqueue(childLink);
                            visited.Add(childLink);
                        }

                        openIndex++;
                        openIndex = content.IndexOf(pattern, openIndex);
                    }

                    result.Status = ProcessResultStatus.Ok;
                }
                catch (WebException)
                {
                    result.Status = ProcessResultStatus.Fail;
                }
                catch (Exception)
                {
                    result.Status = ProcessResultStatus.Error;
                }

                this.RequestProceed(this, result);
            }
        }

        private bool ShouldVisit(string link)
        {
            if (!string.IsNullOrEmpty(link) && !escapeLinks.Contains(link) && !visited.Contains(link))
            {
                if (this.linkDispatcher.GetDomain(link) == this.linkDispatcher.GetDomain(root))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
