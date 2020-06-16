using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;

namespace Tang.Editor
{
    [CustomEditor(typeof(GameObjectManager))]
    public class GameObjectManagerEditor : UnityEditor.Editor
    {
        GameObjectManager gameObjectManager;

        public string rolePrefabPath = "Assets/Resources_moved/Prefabs/Role";
        public string itemPrefabPath = "Assets/Resources_moved/Prefabs/Item";
        public string skillPrefabPath = "Assets/Resources_moved/Prefabs/Skill";
        public string effectPrefabPath = "Assets/Resources_moved/Prefabs/Effect";
        public string placementPrefabPath = "Assets/Resources_moved/Prefabs/Placement";
        public string trapPrefabPath = "Assets/Resources_moved/Prefabs/Trap";
        public string otherPrefabPath = "Assets/Resources_moved/Prefabs/Other";
        public string terrainPrefabPath = "Assets/Resources_moved/Prefabs/Terrains";
        public string particlePrefabPath = "Assets/Particles";


        void OnEnable()
        {
            gameObjectManager = target as GameObjectManager;
        }

        void Start()
        {
        }

        //参数1 为要查找的总路径， 参数2 保存路径  
        private static void GetDirs(string dirPath, ref List<string> dirs)
        {
            foreach (string path in Directory.GetFiles(dirPath))
            {
                //获取所有文件夹中包含后缀为 .prefab 的路径  
                if (System.IO.Path.GetExtension(path) == ".prefab")
                {
                    dirs.Add(path.Substring(path.IndexOf("Assets")));
                    Debug.Log(path.Substring(path.IndexOf("Assets")));
                }
            }

            if (Directory.GetDirectories(dirPath).Length > 0)  //遍历所有文件夹  
            {
                foreach (string path in Directory.GetDirectories(dirPath))
                {
                    GetDirs(path, ref dirs);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            otherPrefabPath = MyGUI.TextFieldWithTitle("otherPrefabPath", otherPrefabPath);

            if (GUILayout.Button("添加预制体"))
            {
                try
                {
                    gameObjectManager.PrefabPathList.Clear();
                    
                    // 添加角色预制体 add by TangJian 2017/11/17 17:10:46
                    List<string> prefabs = Tools.GetFileListInFolder(Tools.AssetPathToSysPath(rolePrefabPath), ".prefab");
                    for (int i = 0; i < prefabs.Count; i++)
                    {
                        var prefabPath = Tools.SysPathToAssetPath(prefabs[i]);
                        var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                        
                        EditorUtility.DisplayProgressBar("添加角色预制体", i + "/" + prefabs.Count, (float)i / prefabs.Count);
                        gameObjectManager.PrefabPathList.Add(new GameObjectManager.PrefabPath(prefabName, prefabPath));
                    }

                    // 添加物品预制体 add by TangJian 2017/11/17 17:10:53
                    prefabs = Tools.GetFileListInFolder(Tools.AssetPathToSysPath(itemPrefabPath), ".prefab");
                    for (int i = 0; i < prefabs.Count; i++)
                    {
                        var prefabPath = Tools.SysPathToAssetPath(prefabs[i]);
                        var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                        
                        EditorUtility.DisplayProgressBar("添加物品预制体", i + "/" + prefabs.Count, (float)i / prefabs.Count);
                        gameObjectManager.PrefabPathList.Add(new GameObjectManager.PrefabPath(prefabName, prefabPath));
                    }

                    // 添加技能预制体 add by TangJian 2017/11/17 17:10:59
                    prefabs = Tools.GetFileListInFolder(Tools.AssetPathToSysPath(skillPrefabPath), ".prefab");
                    for (int i = 0; i < prefabs.Count; i++)
                    {
                        var prefabPath = Tools.SysPathToAssetPath(prefabs[i]);
                        var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                        
                        EditorUtility.DisplayProgressBar("添加技能预制体", i + "/" + prefabs.Count, (float)i / prefabs.Count);
                        gameObjectManager.PrefabPathList.Add(new GameObjectManager.PrefabPath(prefabName, prefabPath));
                    }

                    // 添加特效预制体 add by TangJian 2017/11/17 17:11:09
                    prefabs = Tools.GetFileListInFolder(Tools.AssetPathToSysPath(effectPrefabPath), ".prefab");
                    for (int i = 0; i < prefabs.Count; i++)
                    {
                        var prefabPath = Tools.SysPathToAssetPath(prefabs[i]);
                        var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                        
                        EditorUtility.DisplayProgressBar("添加特效预制体", i + "/" + prefabs.Count, (float)i / prefabs.Count);
                        gameObjectManager.PrefabPathList.Add(new GameObjectManager.PrefabPath(prefabName, prefabPath));
                    }

                    // 场景组件预制体 add by TangJian 2017/11/17 17:18:10
                    prefabs = Tools.GetFileListInFolder(Tools.AssetPathToSysPath(placementPrefabPath), ".prefab");
                    for (int i = 0; i < prefabs.Count; i++)
                    {
                        var prefabPath = Tools.SysPathToAssetPath(prefabs[i]);
                        var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                        
                        EditorUtility.DisplayProgressBar("其他预制体", i + "/" + prefabs.Count, (float)i / prefabs.Count);
                        gameObjectManager.PrefabPathList.Add(new GameObjectManager.PrefabPath(prefabName, prefabPath));
                    }

                    // 添加地形预制体 add by TangJian 2019/3/25 21:21
                    prefabs = Tools.GetFileListInFolder(Tools.AssetPathToSysPath(terrainPrefabPath), ".prefab");
                    for (int i = 0; i < prefabs.Count; i++)
                    {
                        var prefabPath = Tools.SysPathToAssetPath(prefabs[i]);
                        var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                        
                        EditorUtility.DisplayProgressBar("加其他预制体", i + "/" + prefabs.Count, (float)i / prefabs.Count);
                        gameObjectManager.PrefabPathList.Add(new GameObjectManager.PrefabPath(prefabName, prefabPath));
                    }

                    // 其他预制体 add by TangJian 2017/11/17 17:18:10
                    prefabs = Tools.GetFileListInFolder(Tools.AssetPathToSysPath(otherPrefabPath), ".prefab");
                    for (int i = 0; i < prefabs.Count; i++)
                    {
                        var prefabPath = Tools.SysPathToAssetPath(prefabs[i]);
                        var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                        
                        EditorUtility.DisplayProgressBar("其他预制体", i + "/" + prefabs.Count, (float)i / prefabs.Count);
                        gameObjectManager.PrefabPathList.Add(new GameObjectManager.PrefabPath(prefabName, prefabPath));
                    }

                    // 粒子 add by TangJian 2017/11/17 17:18:10
                    prefabs = Tools.GetFileListInFolder(Tools.AssetPathToSysPath(particlePrefabPath), ".prefab");
                    for (int i = 0; i < prefabs.Count; i++)
                    {
                        var prefabPath = Tools.SysPathToAssetPath(prefabs[i]);
                        var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                        
                        EditorUtility.DisplayProgressBar("粒子", i + "/" + prefabs.Count, (float)i / prefabs.Count);
                        gameObjectManager.PrefabPathList.Add(new GameObjectManager.PrefabPath(prefabName, prefabPath));
                    }

                    // 遍历所有预制体, 保存引用 
                    {
                        List<string> fileNames = Tools.GetFilesInFolder(Application.dataPath, ".prefab");
                        for (int i = 0; i < fileNames.Count; i++)
                        {
                            string fileName = fileNames[i];
                            EditorUtility.DisplayProgressBar("遍历所有预制体, 保存引用", fileName, (float)i / fileNames.Count);
                            
                            var prefabPath = Tools.SysPathToAssetPath(fileNames[i]);
                            var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                            
                            gameObjectManager.PrefabPathList.Add(new GameObjectManager.PrefabPath(prefabPath, prefabPath));
                        }
                    }

                    foreach (var prefab in gameObjectManager.PrefabPathList)
                    {
                        if (gameObjectManager.prefabPathDic.ContainsKey(prefab.key))
                        {
                            Debug.Log("存在同名预制体: " + prefab.key);
                        }
                        else
                        {
                            gameObjectManager.prefabPathDic.Add(prefab.key, prefab.path);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }
            base.DrawDefaultInspector();
        }
    }
}