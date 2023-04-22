using Swift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEditor.FilePathAttribute;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using System.Collections;

namespace TowerDefance.Res
{
    public class ResManager
    {
        public static string ResRootPath = "Assets/AddressableResources";


        public enum AniResType
        {
            Tower,
            Enemy,
            Effect,
            Avatar
        }

        public void LoadAni(AniResType aniType, string name, Action<Sprite[]> onLoaded)
        {
            var resPath = ResRootPath + "/" + aniType.ToString() + "/" + name;
            Addressables.LoadResourceLocationsAsync(resPath).Completed += (handles) =>
            {
                var sprites = new List<Sprite>();
                if (handles.Status == AsyncOperationStatus.Succeeded)
                {
                    IList<IResourceLocation> locations = handles.Result;

                    foreach (IResourceLocation location in locations)
                    {

                        Addressables.LoadAssetAsync<Sprite>(location).Completed += (assetHandle) =>
                        {
                            if (assetHandle.Status == AsyncOperationStatus.Succeeded)
                                sprites.Add(assetHandle.Result);
                            else
                                Debug.LogError("Failed to load asset at location: " + location);

                            Addressables.Release(assetHandle);
                        };
                    }

                    // onLoaded?.Invoke(sprites.ToArray());
                }

                Addressables.Release(handles);
            };
        }
    }
}
