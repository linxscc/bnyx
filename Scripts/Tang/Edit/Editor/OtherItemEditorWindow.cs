using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


namespace Tang.Editor
{
    public class OtherItemEditorWindow : EditorWindow
    {
        [MenuItem("Window/其他物品编辑器")]
        static void Init()
        {
            OtherItemEditorWindow window = (OtherItemEditorWindow)EditorWindow.GetWindow(typeof(OtherItemEditorWindow));
            window.Show();
        }

        private string otherItemDataFile = "Resources_moved/Scripts/OtherItem/OtherItem.json";
        private string prefabPath = "Assets/Resources_moved/Prefabs/Item/OtherItem";

        Dictionary<string, ItemData> otherItemDataDic = new Dictionary<string, ItemData>();
        List<ItemData> otherItemDataList = new List<ItemData>();

        ItemData currItemData;

        Vector2 listScrollViewPos = Vector2.zero;
        Vector2 editScrollViewPos = Vector2.zero;

        Rect windowRect = new Rect(100, 100, 500, 500);

        void OnGUI()
        {
            GUIStyle boxStyle = new GUIStyle("box");

            var width = position.size.x - boxStyle.border.horizontal;
            var height = position.size.y - boxStyle.border.vertical;

            var innerBoxWidth = width - (boxStyle.padding.horizontal + boxStyle.border.horizontal);
            var innerBoxHeight = height - (boxStyle.padding.vertical + boxStyle.border.vertical);

            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));

            // 设置路径, 以及存取数据 add by TangJian 2017/11/15 16:22:45
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(otherItemDataFile);
            if (MyGUI.Button("读取"))
            {
                loadOtherItem();
            }
            if (MyGUI.Button("保存"))
            {
                saveOtherItem();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(prefabPath);
            if (MyGUI.Button("制作预制体"))
            {
                saveOtherItem();
            }
            EditorGUILayout.EndHorizontal();

            // 编辑区域 add by TangJian 2017/11/15 16:28:19
            EditorGUILayout.BeginHorizontal();

            // // 列表框 add by TangJian 2017/11/15 16:27:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            listScrollViewPos = EditorGUILayout.BeginScrollView(listScrollViewPos);

            for (int i = otherItemDataList.Count - 1; i >= 0; i--)
            {
                var item = otherItemDataList[i];
                EditorGUILayout.BeginHorizontal();

                if (MyGUI.Button("删除", GUILayout.Width(50)))
                {
                    otherItemDataList.RemoveAt(i);
                }

                if (MyGUI.Button(item.id))
                {
                    currItemData = item;
                }

                if (MyGUI.Button("复制", GUILayout.Width(50)))
                {
                    var otherItemData = Tools.Json2Obj<ItemData>(Tools.Obj2Json(item, true));
                    otherItemDataList.Add(otherItemData);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (MyGUI.Button("+"))
            {
                otherItemDataList.Add(new ItemData());
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            // 编辑框 add by TangJian 2017/11/15 16:28:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            editScrollViewPos = EditorGUILayout.BeginScrollView(editScrollViewPos);
            if (currItemData != null)
            {
                MyGUI.ItemDataField(currItemData);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void OnEnable()
        {
            title = "其他物品编辑器";
            loadOtherItem();
        }

        void loadOtherItem()
        {
            //string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + otherItemDataFile);
            //otherItemDataDic = Tools.Json2Obj<Dictionary<string, ItemData>>(jsonString);
            otherItemDataDic = Tools.LoadOneData<ItemData>(Application.dataPath + "/" + "Resources_moved/Scripts/OtherItem/OtherItemList");

            otherItemDataList = otherItemDataDic.Values.ToList();
            currItemData = otherItemDataList[0];
        }

        void saveOtherItem()
        {
            otherItemDataDic = otherItemDataList.ToDictionary(item => item.id, item => item);

            //string jsonString = Tools.Obj2Json(otherItemDataDic, true);
            //Debug.Log("jsonString = " + jsonString);
            //Tools.WriteStringFromFile(Application.dataPath + "/" + otherItemDataFile, jsonString);
            Tools.SaveOneData<ItemData>(otherItemDataDic, Application.dataPath + "/" + "Resources_moved/Scripts/OtherItem/OtherItemList");

            // 生成预制体 add by TangJian 2017/11/16 17:42:05
            {
                foreach (var item in otherItemDataDic)
                {
                    var itemObject = PrefabCreator.CreateDropItem(item.Value);
                    Tools.UpdatePrefab(itemObject, prefabPath + "/" + item.Value.id + ".prefab");
                    DestroyImmediate(itemObject);
                }
            }
        }
    }
}