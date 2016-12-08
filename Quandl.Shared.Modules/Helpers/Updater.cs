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
                var releases = _client.Repository.Release.GetAll("quandl", "quandl-excel-windows");
                latestRelease = releases.Result[0];

                _updateAvailable = latestRelease.Id > Utilities.GithubReleaseId && !latestRelease.Prerelease && !latestRelease.Draft;
            }
            catch(Exception)
            {
                return;
            }           
        }

    }

    
}
