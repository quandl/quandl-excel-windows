using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Quandl.Shared.Helpers
{
    public class Updater
    {
        public Release latestRelease;
        private bool _updateAvailable;
        private GitHubClient _client;

        public Updater()
        {
            _client = GetClient;
            CheckForUpdate();
        }

        private GitHubClient GetClient {
            get
            {
                if (_client == null)
                {
                    _client = new GitHubClient(new ProductHeaderValue("Quandl-Excel-Addin"));
                }
                return _client;
            }
        }

        public bool UpdateAvailable { get { return _updateAvailable; } }

        public void GetLastestUpdate()
        {
            string requestPath = latestRelease.Assets[0].BrowserDownloadUrl;

            string fullFile = Syroot.Windows.IO.KnownFolders.Downloads.Path + "\\" + GetFileNameFrom(new Uri(requestPath));

            DownloadSync(requestPath, fullFile);

            // NOTE This opens explorer with the file you just downloaded selected
            Process.Start("explorer.exe", $"/select,  {fullFile}");
        }

        private void DownloadSync(string requestPath, string fileName)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile(new Uri(requestPath), fileName);
        }

        private string GetFileNameFrom(Uri uri)
        {
            return Path.GetFileName(uri.LocalPath);
        }

        private void CheckForUpdate()
        {
            try
            {
                var task = _client.Repository.Release.GetAll("quandl", "quandl-excel-windows");
                var releases = task.Result;

                // Figure out what the current release is by finding its equivalent in github.
                Release currentRelease = null;
                foreach (var release in releases)
                {
                    if (release.Name == Utilities.ReleaseVersion) {
                        currentRelease = release;
                        break;
                    }
                }

                // Figure out what the latest published release is. The latest release cannot be a draft or Pre-release.
                foreach (var release in releases)
                {
                    if (!release.Prerelease && !release.Draft)
                    {
                        latestRelease = release;
                        break;
                    }
                }

                // Only update if there is a latest release and its greater than the currentRelease or we could not identify the current release.
                _updateAvailable = latestRelease != null && (currentRelease == null || latestRelease.PublishedAt > currentRelease.PublishedAt);
            }
            catch (Exception e)
            {
                Logger.log(e);
            }
        }
    }    
}
