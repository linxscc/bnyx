using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Tang.Editor
{
    public class AnimatorInspectorCache
    {
        public string AnimatorName;
        public string JsonPath;
        public string ExcelPath;

        public AnimatorInspectorCache(string animatorName, string jsonPath, string excelPath)
        {
            AnimatorName = animatorName;
            JsonPath = jsonPath;
            ExcelPath = excelPath;
        }
    }
    
    public class AnimatorInspectorCacheManager
    {
        private static AnimatorInspectorCacheManager instance;

        public static AnimatorInspectorCacheManager Instance => instance ?? (instance = new AnimatorInspectorCacheManager());

        private Dictionary<string, AnimatorInspectorCache> CacheDic;
        private JObject _jObject = new JObject();
        private string JsonPath = "Assets/Scripts/Tang/Edit/Editor/Animator/AnimatorInspectorCache.json";
        public void LoadJson()
        {
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>(JsonPath); 
            CacheDic = Tools.Json2Obj<Dictionary<string, AnimatorInspectorCache>>(json.text);
        }

        public void SaveJson(AnimatorInspectorCache animatorInspectorCache)
        {
            //if it Contains ,Replace.  Otherwise,Add
            if (_jObject.ContainsKey(animatorInspectorCache.AnimatorName))
            {
                _jObject[animatorInspectorCache.AnimatorName].Replace(JObject.FromObject(animatorInspectorCache));
            }
            else
            {
                _jObject.Add(animatorInspectorCache.AnimatorName,JObject.FromObject(animatorInspectorCache));
            }
            
            Tools.Save(_jObject,JsonPath);
            AssetDatabase.Refresh();
        }

        public AnimatorInspectorCache GetCache(string Name)
        {
            LoadJson();
            return CacheDic.ContainsKey(Name) ? CacheDic[Name] : null;
        }
    }
}