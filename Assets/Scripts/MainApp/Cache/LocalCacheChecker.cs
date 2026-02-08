using System.Collections; using UnityEngine;  namespace MainApp.Cache
{
    /// <summary>     /// First chain of validation.     /// </summary>     public class LocalCacheChecker : CacheValidationHandler
    {
        public override IEnumerator Handle(RemoteTextureInfo textureInfo, FileCache cache)
        {
            Debug.Log($"[{nameof(LocalCacheChecker)}] Check local cache for {textureInfo.name}");

            if (!cache.HasCachedFile(textureInfo.url))
            {
                Debug.Log($"[{nameof(LocalCacheChecker)}] File {textureInfo.name} not found in cache");
                textureInfo.downloadStatus = RemoteTextureInfo.DownloadStatus.NeedDownload;
                yield break;
            }

            Debug.Log($"[{nameof(LocalCacheChecker)}] File {textureInfo.name} exist in cache, check actuality");
            yield return HandleNext(textureInfo, cache);
        }
    }
}