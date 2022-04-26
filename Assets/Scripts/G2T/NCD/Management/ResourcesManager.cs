using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace G2T.NCD.Management {
    public class ResourcesManager : SingletonBehaviour<ResourcesManager> {
        public async Task<T> LoadAsync<T>(string path, Transform parent = null) where T : Object {
            if(!path.StartsWith("Assets/Resources/")) {
                throw new System.Exception(string.Format("Invalid path: {0}", path));
            }

            path = path.Replace("Assets/Resources/", "").Replace(Path.GetExtension(path), "");

            var request = Resources.LoadAsync<T>(path);

            await request;

            if(request.asset == null) {
                throw new System.Exception(string.Format("There is no asset, path: {0}", path));
            }

            return request.asset as T;
        }
    }
}