using System.Collections.Generic;
using UnityEngine;

namespace MainApp
{
    public class Model
    {
        public List<string> AllPicsUrls = new List<string>();
        
        public List<string> OddPicsUrls = new List<string>();
        public List<string> EvenPicsUrls = new List<string>();

        public List<RemoteTextureInfo> RemoteTextureInfos = new List<RemoteTextureInfo>();

        public List<RemoteTextureInfo> RemoteTexturesToDownload = new List<RemoteTextureInfo>();

        private Dictionary<int, Texture2D> downloadedPictures = new Dictionary<int, Texture2D>();

        private List<Texture2D> oddPics = new List<Texture2D>();
        private List<Texture2D> evenPics = new List<Texture2D>();

        public int AllPics => AllPicsUrls.Count;

        public void FillFiltredCollections()
        {
            OddPicsUrls.Clear();
            EvenPicsUrls.Clear();

            foreach (var remTexInfo in RemoteTextureInfos)
            {
                if (remTexInfo.id % 2 == 0)
                {
                    EvenPicsUrls.Add(remTexInfo.url);
                }
                else
                {
                    OddPicsUrls.Add(remTexInfo.url);
                }
                downloadedPictures.Add(remTexInfo.id, null);
            }
        }

        public void AddDownloadedPicture(int id, Texture2D sprite)
        {
            if (!downloadedPictures.ContainsKey(id))
                return;
            
            downloadedPictures[id] = sprite;
            if (id % 2 == 0)
            {
                evenPics.Add(sprite);
            }
            else
            {
                oddPics.Add(sprite);
            }
        }
    }
}
