using System;
using UnityEngine;

namespace MainApp
{
    [Serializable]
    public class RemoteTextureInfo
    {
        public string name;
        public string url;
        public string extension;
        public string checksum; // using for cache
        public int id;
        public DownloadStatus downloadStatus;
        public long fileSize;
        public DateTime lastModified;
        public bool isPremium;

        public enum DownloadStatus
        {
            NotChecked = 0,
            CachedValid = 1,
            NeedDownload = 2,
            DownloadError = 3,
        }

        public void InitID()
        {
            if (int.TryParse(name, out id))
            {
                isPremium = id % 4 == 0;
            }
            else
            {
                id = -1;
                downloadStatus = DownloadStatus.NeedDownload;
                Debug.LogWarning($"File \"{name}\" has incorrect name. Loading priority changed to LOW");
            }
        }
    }
}