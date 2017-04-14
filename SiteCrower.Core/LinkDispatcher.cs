using System;
using System.Text.RegularExpressions;

namespace SiteCrower.Core
{
    public class LinkDispatcher
    {
        private readonly string root;

        public LinkDispatcher(string root)
        {
            if (!IsUrlValid(root))
            {
                throw new ArgumentException("Uri not well formatted!");
            }
            
            this.root = root;
        }

        public static bool IsUrlValid(string url)
        {
            string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(url);
        }
        
        public string GetDomain(string rawUrl)
        {
            if (rawUrl.StartsWith("/") || (!rawUrl.ToLower().StartsWith("http://") && !rawUrl.ToLower().StartsWith("https://"))) 
            {
                return GetDomain(root);
            }

            Uri uri = new Uri(rawUrl);
            string url = uri.Host.ToString();
            return url;
        }

        public string DecorateUrl(string url)
        {
            if (url.StartsWith("/") || (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://")))
            {
                string separator = url.StartsWith("/") ? "" : "/";
                url = string.Format("{0}{1}{2}", GetDomain(root), separator, url);
            }

            if (!url.ToLower().StartsWith("http://"))
            {
                string prefix = "http://";
                if (url.StartsWith("/"))
                {
                    prefix = "http:/";
                }

                url = prefix + url;
            }

            return url;
        }
    }
}
