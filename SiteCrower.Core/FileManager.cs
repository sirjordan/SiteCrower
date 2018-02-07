using System;
using System.IO;

namespace SiteCrower.Core
{
    public sealed class FileManager
    {
        private string domain;

        public FileManager(string domain)
        {
            this.domain = domain;

            if (Directory.Exists(Settings.DownloadFolder))
            {
                Directory.Delete(Settings.DownloadFolder, true);
            }

            Directory.CreateDirectory(Settings.DownloadFolder);
        }

        public async void Save(byte[] content, string href)
        {
            string fileName = GetFileName(href);

            string filePath = Path.Combine(Settings.DownloadFolder, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var fileStream = File.Create(filePath))
            {
                await fileStream.WriteAsync(content, 0, content.Length);
            }
        }

        private string GetFileName(string href)
        {
            string fileName = href.Replace(domain, string.Empty);

            if (fileName == string.Empty)
            {
                fileName = "index";
            }

            fileName += ".html";
            return fileName.Trim('/');
        }
    }
}