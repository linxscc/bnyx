// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using System.Reflection;
// using System.IO;
// using System.Linq;


// namespace Tang.Editor
// {
//     public class DataEditorWindow<T> : EditorWindow
//     {
//         [MenuItem("Window/DataEditorWindow")]
//         static void Init()
//         {
//             DataEditorWindow window = (DataEditorWindow)EditorWindow.GetWindow(typeof(DataEditorWindow));
//             window.Show();
//         }

//         private string dataFilePath = "Resources_moved/Scripts/Soul/Soul.json";
//         private string prefabPath = "Assets/Resources_moved/Prefabs/Item/Equip/Soul";

//         Dictionary<string, T> soulDataDic = new Dictionary<string, T>();
//         List<T> soulDataList = new List<T>();

//         T currSoulData;

//         Vector2 listScrollViewPos = Vector2.zero;
//         Vector2 editScrollViewPos = Vector2.zero;

//         Rect windowRect = new Rect(100, 100, 500, 500);

//         void OnGUI()
//         {
//             GUIStyle boxStyle = new GUIStyle("box");

//             var width = position.size.x - boxStyle.border.horizontal;
//             var height = position.size.y - boxStyle.border.vertical;

//             var innerBoxWidth = width - (boxStyle.padding.horizontal + boxStyle.border.horizontal);
//             var innerBoxHeight = height - (boxStyle.padding.vertical + boxStyle.border.vertical);

//             EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));

//             // 设置路径, 以及存取数据 add by TangJian 2017/11/15 16:22:45
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField(dataFilePath);
//             if (MyGUI.Button("读取"))
//             {
//                 loadSoul();
//             }
//             if (MyGUI.Button("保存"))
//             {
//                 saveSoul();
//             }
//             EditorGUILayout.EndHorizontal();

//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField(prefabPath);
//             if (MyGUI.Button("制作预制体"))
//             {
//                 saveSoul();
//             }
//             EditorGUILayout.EndHorizontal();

//             // 编辑区域 add by TangJian 2017/11/15 16:28:19
//             EditorGUILayout.BeginHorizontal();

//             // // 列表框 add by TangJian 2017/11/15 16:27:46
//             EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
//             listScrollViewPos = EditorGUILayout.BeginScrollView(listScrollViewPos);

//             for (int i = soulDataList.Count - 1; i >= 0; i--)
//             {
//                 var item = soulDataList[i];
//                 EditorGUILayout.BeginHorizontal();

//                 if (MyGUI.Button("删除", GUILayout.Width(50)))
//                 {
//                     soulDataList.RemoveAt(i);
//                 }

//                 if (MyGUI.Button(item.id))
//                 {
//                     currSoulData = item;
//                 }

//                 if (MyGUI.Button("复制", GUILayout.Width(50)))
//                 {
//                     var soulData = Tools.Json2Obj<T>(Tools.Obj2Json(item, true));
//                     soulDataList.Add(soulData);
//                 }

//                 EditorGUILayout.EndHorizontal();
//             }

//             if (MyGUI.Button("+"))
//             {
//                 soulDataList.Add(new T());
//             }

//             EditorGUILayout.EndScrollView();
//             EditorGUILayout.EndVertical();


//             // 编辑框 add by TangJian 2017/11/15 16:28:46
//             EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
//             editScrollViewPos = EditorGUILayout.BeginScrollView(editScrollViewPos);
//             if (currSoulData != null)
//             {
//                 MyGUI.ItemDataField(currSoulData);
//             }
//             EditorGUILayout.EndScrollView();
//             EditorGUILayout.EndVertical();


//             EditorGUILayout.EndHorizontal();

//             EditorGUILayout.EndVertical();
//         }

//         void OnEnable()
//         {
//             title = "魂编辑器";
//             loadSoul();
//         }

//         void loadSoul()
//         {
//             string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + dataFilePath);
//             soulDataDic = Tools.Json2Obj<Dictionary<string, T>>(jsonString);

//             soulDataList = soulDataDic.Values.ToList();
//             currSoulData = soulDataList[0];
//         }

//         void saveSoul()
//         {
//             soulDataDic = soulDataList.ToDictionary(item => item.id, item => item);

//             string jsonString = Tools.Obj2Json(soulDataDic, true);
//             Debug.Log("jsonString = " + jsonString);
//             Tools.WriteStringFromFile(Application.dataPath + "/" + dataFilePath, jsonString);

//             // 生成预制体 add by TangJian 2017/11/16 17:42:05
//             {
//                 foreach (var item in soulDataDic)
//                 {
//                     var itemObject = PrefabCreator.CreateDropItem(item.Value);
//                     Tools.UpdatePrefab(itemObject, prefabPath + "/" + item.Value.id + ".prefab");
//                     DestroyImmediate(itemObject);
//                 }
//             }
//         }

//     }
// }