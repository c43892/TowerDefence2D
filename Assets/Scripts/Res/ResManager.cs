using Newtonsoft.Json;
using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using static UnityEditor.FilePathAttribute;

namespace GalPanic.Res
{
    public class ResManager
    {
        public static IEnumerator LoadConfig<T>(string label, Action<T> onLoaded)
        {
            yield return Addressables.InitializeAsync();

            var assetsOp = Addressables.LoadAssetAsync<TextAsset>(label);

            // Wait for the loading to complete
            yield return assetsOp;

            if (assetsOp.Status == AsyncOperationStatus.Succeeded)
            {
                var jsonString = assetsOp.Result.text;
                onLoaded?.Invoke(JsonConvert.DeserializeObject<T>(jsonString));
            }

            // Release the loaded assets when no longer needed
            Addressables.Release(assetsOp);
        }

        public static IEnumerator LoadSpritesFromGroup(string label, Action<Sprite[]> onLoaded)
        {
            yield return Addressables.InitializeAsync();

            var assetsOp = Addressables.LoadAssetsAsync<Sprite>(label, null);

            // Wait for the loading to complete
            yield return assetsOp;

            if (assetsOp.Status == AsyncOperationStatus.Succeeded)
            {
                var spriteArr = assetsOp.Result.ToArray();
                Array.Sort(spriteArr, (s1, s2) =>
                {
                    Match m1 = Regex.Match(s1.name, @"\d+");
                    Match m2 = Regex.Match(s2.name, @"\d+");

                    var n1 = m1.Success ? int.Parse(m1.Value) : 0;
                    var n2 = m2.Success ? int.Parse(m2.Value) : 0;
                    return n1.CompareTo(n2);
                });

                onLoaded?.Invoke(spriteArr);
            }

            // Release the loaded assets when no longer needed
            Addressables.Release(assetsOp);
        }
    }
}
