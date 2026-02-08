using DI;
using MainApp.Cache;
using MainApp.Configs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace MainApp
{
    public class ServerService : MonoBehaviour, 
        IService, 
        IConfigurable, 
        IInitializable
    {
        private FileCache fileCache;
        private CacheValidationHandler validationChain;

        private Model _model;
        private ServerConfig _serverConfig;
        private WaitForSecondsRealtime _delay;

        public void SetModel(Model model)
        {
            _model = model;
        }

        public void SetConfig(IConfig config)
        {
            if (config is ServerConfig serverConfig)
            {
                _serverConfig = serverConfig;
                _delay = new WaitForSecondsRealtime(_serverConfig.RequestDelay);
            }
        }

        public void Initialize()
        {
            InitializeCache();
            InitializeValidationChain();
            StartCoroutine(ProcessServerFiles());
        }

        private void InitializeCache()
        {
            fileCache = new FileCache();

            if (_serverConfig.IsClearCachingOnStart)
            {
                fileCache.ClearCache();
                Debug.Log("Cache cleared");
            }

            long cacheSize = fileCache.GetCacheSize();
            Debug.Log($"Cache size: {FormatBytes(cacheSize)}");
        }

        private void InitializeValidationChain()
        {
            var localChecker = new LocalCacheChecker();
            var etagChecker = new ETagValidator();
            var lmclChecker = new LMCLValidator();

            localChecker.SetNext(etagChecker).SetNext(lmclChecker);
            validationChain = localChecker;
        }

        IEnumerator ProcessServerFiles()
        {
            Debug.Log("Start server files process...");

            yield return StartCoroutine(GetFileListFromServer(_model.AllPicsUrls));

            Debug.Log($"Content files found: {_model.AllPicsUrls.Count}");

            foreach (string fileUrl in _model.AllPicsUrls)
            {
                RemoteTextureInfo textureInfo = CreateTextureInfo(fileUrl);
                _model.RemoteTextureInfos.Add(textureInfo);

                yield return StartCoroutine(validationChain.Handle(textureInfo, fileCache));

                switch (textureInfo.downloadStatus)
                {
                    case RemoteTextureInfo.DownloadStatus.CachedValid:
                        Debug.Log($"{textureInfo.name} - already contains in cache, using local");
                        break;

                    case RemoteTextureInfo.DownloadStatus.NeedDownload:
                        Debug.Log($"{textureInfo.name} - need download");
                        _model.RemoteTexturesToDownload.Add(textureInfo);
                        //yield return StartCoroutine(DownloadFile(textureInfo));
                        break;

                    case RemoteTextureInfo.DownloadStatus.DownloadError:
                        Debug.LogWarning($"{textureInfo.name} - validation error");
                        break;
                }

                yield return _delay;
            }

            _model.FillFiltredCollections();

            PrintResults();
        }

        IEnumerator GetFileListFromServer(List<string> fileUrls)
        {
            string fullUrl = _serverConfig.GetFullFolderUrl;

            using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string html = request.downloadHandler.text;
                    var matches = System.Text.RegularExpressions.Regex.Matches(
                        html,
                        @"<a href=""([^""]+\.(png|jpg|svg))""",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );

                    foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                        if (match.Groups.Count >= 2)
                        {
                            string relativeUrl = match.Groups[1].Value;
                            string absoluteUrl = _serverConfig.GetFullFolderUrl + relativeUrl;
                            fileUrls.Add(absoluteUrl);
                        }
                    }
                }
            }

            yield break;
        }

        private RemoteTextureInfo CreateTextureInfo(string url)
        {
            var result = new RemoteTextureInfo
            {
                name = Path.GetFileNameWithoutExtension(url),
                url = url,
                extension = Path.GetExtension(url).ToLower(),
                downloadStatus = RemoteTextureInfo.DownloadStatus.NotChecked
            };
            result.InitID();

            return result;
        }

        public IEnumerator DownloadFile(RemoteTextureInfo textureInfo)
        {
            Debug.Log($"Downloading {textureInfo.name}...");

            using (UnityWebRequest request = UnityWebRequest.Get(textureInfo.url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    byte[] data = request.downloadHandler.data;

                    if (_serverConfig.IsCaching)
                    {
                        fileCache.SaveFileToCache(textureInfo.url, data);
                        Debug.Log($"File {textureInfo.name} saved to cache ({FormatBytes(data.Length)})");
                    }

                    Texture2D texture = new Texture2D(920, 920);
                    texture.LoadImage(data);
                    _model.AddDownloadedPicture(textureInfo.id, texture);
                }
                else
                {
                    Debug.LogError($"Downloading error {textureInfo.name}: {request.error}");
                }
            }
        }

        private void PrintResults()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== PROCESSING RESULT ===");

            int cached = 0;
            int toDownload = 0;
            int errors = 0;

            foreach (var info in _model.RemoteTextureInfos)
            {
                switch (info.downloadStatus)
                {
                    case RemoteTextureInfo.DownloadStatus.CachedValid: cached++; break;
                    case RemoteTextureInfo.DownloadStatus.NeedDownload: toDownload++; break;
                    case RemoteTextureInfo.DownloadStatus.DownloadError: errors++; break;
                }
            }

            sb.AppendLine($"Files count: {_model.RemoteTextureInfos.Count}");
            sb.AppendLine($"Cached (current accorded): {cached}");
            sb.AppendLine($"Need download: {toDownload}");
            sb.AppendLine($"Errors: {errors}");

            long cacheSize = fileCache.GetCacheSize();
            sb.AppendLine($"Summary cache size: {FormatBytes(cacheSize)}");
            Debug.Log(sb);
        }

        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }

        public void ClearCacheButton()
        {
            fileCache.ClearCache();
            Debug.Log("Cache cleaned manualy");
        }

        public void RecheckFilesButton()
        {
            StopAllCoroutines();
            _model.RemoteTextureInfos.Clear();
            StartCoroutine(ProcessServerFiles());
        }
    }
}