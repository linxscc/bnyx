using UnityEditor;
using UnityEngine;

namespace Tang
{
    public class ConfigManager
    {
        private static ConfigManager s_instance;

        public static ConfigManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new ConfigManager();
                }
                return s_instance;
            }
        }

        Cache _cache = new Cache();
        
        public T GetConfigFromJson<T>(string path)
        {
            int key = path.GetHashCode();
            object obj = null;
            if (_cache.TryGet(key, out obj))
            {
            }
            else
            {
                obj = Tools.Json2Obj<T>(Tools.ReadStringFromFile(path));
                _cache.Set(key, obj);
            }
            return (T)obj;
        }
        
        public T GetConfigFromJson<T>(TextAsset textAsset)
        {
            int key = textAsset.GetHashCode();
            object obj = null;
            if (_cache.TryGet(key, out obj))
            {
            }
            else
            {
                obj = Tools.Json2Obj<T>(textAsset.text);
                _cache.Set(key, obj);
            }
            return (T)obj;
        }

        public void Refresh()
        {
            _cache.Clear();
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}