using System;
using UnityEngine;

namespace MainApp.Configs
{
    [Serializable, CreateAssetMenu()]
    public class ServerConfig : ScriptableObject, IConfig
    {
        public string DomenUrl;
        public string FolderUrl;

        public float RequestDelay;
        public bool IsCaching;
        public bool IsClearCachingOnStart;

        public string GetFullFolderUrl => DomenUrl + FolderUrl;
    }
}
