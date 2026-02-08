using System; using System.Collections.Generic; using System.IO; using System.Text; using UnityEngine;  namespace MainApp.Cache
{
    /// <summary>     /// Cache system     /// </summary>     public class FileCache
    {
        private const string CACHE_FOLDER = "DownloadCache";
        private const string META_FILE = "cache_metadata.json";

        [Serializable]
        private class CacheEntry
        {
            public string url;
            public string checksum;
            public DateTime lastChecked;
            public string localPath;
            public long fileSize;
        }

        [Serializable]
        private class CacheDatabase
        {
            public List<CacheEntry> entries = new List<CacheEntry>();
            public DateTime lastUpdated;
        }

        private CacheDatabase database;
        private string cachePath;
        private string metaPath;

        public FileCache()
        {
            cachePath = Path.Combine(Application.persistentDataPath, CACHE_FOLDER);
            metaPath = Path.Combine(cachePath, META_FILE);

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            LoadDatabase();
        }

        public bool HasCachedFile(string url)
        {
            var entry = FindEntry(url);
            if (entry == null) return false;

            return File.Exists(entry.localPath);
        }

        public bool IsFileValid(string url, string currentChecksum)
        {
            var entry = FindEntry(url);
            if (entry == null) return false;

            return entry.checksum == currentChecksum;
        }

        public void UpdateCacheEntry(string url, string checksum)
        {
            var entry = FindEntry(url);

            if (entry == null)
            {
                entry = new CacheEntry
                {
                    url = url,
                    localPath = GenerateCachePath(url)
                };
                database.entries.Add(entry);
            }

            entry.checksum = checksum;
            entry.lastChecked = DateTime.UtcNow;
            SaveDatabase();
        }

        public string GetCachedFilePath(string url)
        {
            var entry = FindEntry(url);
            return entry?.localPath;
        }

        public void SaveFileToCache(string url, byte[] data)
        {
            string filePath = GenerateCachePath(url);
            File.WriteAllBytes(filePath, data);

            UpdateCacheEntry(url, "cached_" + data.Length);
        }

        private CacheEntry FindEntry(string url)
        {
            return database.entries.Find(e => e.url == url);
        }

        private string GenerateCachePath(string url)
        {
            string fileName = Convert.ToBase64String(Encoding.UTF8.GetBytes(url))
                .Replace("/", "_")
                .Replace("+", "-")
                .Replace("=", "");

            return Path.Combine(cachePath, fileName + ".cache");
        }

        private void LoadDatabase()
        {
            if (File.Exists(metaPath))
            {
                try
                {
                    string json = File.ReadAllText(metaPath);
                    database = JsonUtility.FromJson<CacheDatabase>(json);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to load cache database: {e.Message}");
                    database = new CacheDatabase();
                }
            }
            else
            {
                database = new CacheDatabase();
            }
        }

        private void SaveDatabase()
        {
            database.lastUpdated = DateTime.UtcNow;

            try
            {
                string json = JsonUtility.ToJson(database, true);
                File.WriteAllText(metaPath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save cache database: {e.Message}");
            }
        }

        public void ClearCache()
        {
            if (Directory.Exists(cachePath))
            {
                Directory.Delete(cachePath, true);
                Directory.CreateDirectory(cachePath);
            }
            database = new CacheDatabase();
        }

        public long GetCacheSize()
        {
            if (!Directory.Exists(cachePath)) return 0;

            long totalSize = 0;
            var files = Directory.GetFiles(cachePath);
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                totalSize += info.Length;
            }
            return totalSize;
        }
    }
}