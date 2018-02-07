using System;
using System.IO;

namespace SiteCrower.Core
{
    internal static class Settings
    {
        public static bool SaveContent
        {
            get { return true; }
        }

        public static string DownloadFolder
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SiteCrower", "Download");
            }
        }
    }
}
