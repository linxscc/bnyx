using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;
using System.Linq;
using ZS;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tang
{
    public class AssetManager : MyMonoBehaviour
    {
        private static Dictionary<string, bool> keyDic;
        
        public static void Initialize()
        {
            keyDic = Addressables.ResourceLocators[0].Keys.ToDictionary(key => key.ToString(), key => true);
            
            BothAnimManager.Init();
            RoleUpgradeDataAsset.GetAsset();
            ItemManager.Init1();

            WalkOnGroundEffectController.LoadRes();
        }

        public static bool ContainsKey(string key)
        {
            return keyDic == null || keyDic.ContainsKey(key);
        }

        public static Task<GameObject> InstantiateRole(string key)
        {
            return Instantiate(Definition.RoleAssetPrefix + key + ".prefab");
        }

        public static Task<GameObject> InstantiateDropItem(string key)
        {
            return Instantiate(Definition.DropItemAssetPrefix + key + ".prefab");
        } 
        
        public static async Task<T> LoadJson<T>(string path)
        {
            return Tools.Json2Obj<T>(await LoadString(path));
        }
        
        public static async Task<string> LoadString(string path)
        {
            var task = LoadAssetAsync<TextAsset>(path);
            var textAsset = (await task);
            return textAsset != null ? textAsset.text : null;
        }
        
        public static async Task<GameObject> SpawnAsync(string path)
        {
            GameObject ret;
            
            if (PoolManager.Instance.ContainKey(path))
            {
                return PoolManager.Instance.Spawn(path);
            }
            var task = LoadAssetAsync<GameObject>(path);
            ret = await task;
            ret.name = path;
            PoolManager.Instance.AddPrefab(ret);
            
            return PoolManager.Instance.Spawn(path);
        }

        public static void DeSpawn(GameObject Obj)
        {
            Tang.PoolManager.Instance.Despawn(Obj);
        }
        
        public static async Task<GameObject> Instantiate(string path)
        {
            var handle = Addressables.InstantiateAsync(path);
            if (handle.IsDone)
            {
                return handle.Result;
            }
            return await handle.Task;
        }
        
        public static async Task<T> LoadAssetAsync<T>(string path) where T : Object
        {
            if (!ContainsKey(path)) return null;
            var handle = Addressables.LoadAssetAsync<T>(path);
            if (handle.IsDone)
            {
                return handle.Result;
            }
            return await handle.Task;
        }
        
        public static T LoadAssetAtPath<T>(string path) where T : Object
        {
#if UNITY_EDITOR 
            Debug.Log("UNITY_EDITOR");
            return AssetDatabase.LoadAssetAtPath<T>(path);
#else
            Debug.Log("NoUNITY_EDITOR");
            return LoadInResources<T>(path);
#endif
        }

        private static T LoadInResources<T>(string path) where T : Object
        {
            string key = "Resources";
            int beginIndex = path.IndexOf(key);
            int endIndex = path.LastIndexOf(".");

            beginIndex = beginIndex < 0 ? 0 : beginIndex + key.Length+1;
            endIndex = endIndex < 0 ? path.Length - 1 : endIndex;
            
            path = path.Substring(beginIndex, endIndex - beginIndex);
            Debug.Log("path:"+path);
            return Resources.Load<T>(path);
        }

        public static string ReadText(string path)
        {
            return File.ReadAllText(path);
        }

        public static void WriteText(string path, string text)
        {
            File.WriteAllText(path, text);
        }
    }
}