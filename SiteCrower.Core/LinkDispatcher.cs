using System;
using System.Text.RegularExpressions;

namespace SiteCrower.Core
{
    public class LinkDispatcher
    {
        private static readonly string validUrlPattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";

        private string root;

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
            Regex reg = new Regex(validUrlPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(url);
        }
        
        public string GetDomain(string rawUrl)
        {
            if (rawUrl.StartsWith("/") || (!rawUrl.ToLower().StartsWith(Constants.Http) && !rawUrl.ToLower().StartsWith(Constants.Https))) 
            {
                return GetDomain(root);
            }

            Uri uri = new Uri(rawUrl);
            string url = uri.Host.ToString();

            return url;
        }

        public string DecorateUrl(string url)
        {
            if (url.StartsWith("/") || (!url.ToLower().StartsWith(Constants.Http) && !url.ToLower().StartsWith(Constants.Https)))
            {
                string separator = url.StartsWith("/") ? "" : "/";
                url = string.Format("{0}{1}{2}", GetDomain(root), separator, url);
            }

            if (!url.ToLower().StartsWith(Constants.Http))
            {
                string prefix = Constants.Http;
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
