using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


namespace Tang.Editor
{
    public class ConsumableDataEditorWindow : EditorWindow
    {
        [MenuItem("Window/消耗品编辑器")]
        static void Init()
        {
            ConsumableDataEditorWindow window = (ConsumableDataEditorWindow)EditorWindow.GetWindow(typeof(ConsumableDataEditorWindow));
            window.Show();
        }

        // 消耗品数据文件
        private string consumableDataFile = "Resources_moved/Scripts/ConsumableData.json";
        private string prefabPath = "Assets/Resources_moved/Prefabs/DropItem";

        Dictionary<string, ConsumableData> consumableDataDic = new Dictionary<string, ConsumableData>();
        List<ConsumableData> consumableDataList = new List<ConsumableData>();

        ConsumableData currConsumableData;

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
            EditorGUILayout.LabelField(consumableDataFile);
            if (MyGUI.Button("读取"))
            {
                loadConsumableData();
            }
            if (MyGUI.Button("保存"))
            {
                saveConsumableData();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(prefabPath);
            if (MyGUI.Button("制作预制体"))
            {
                saveConsumableData();
            }
            EditorGUILayout.EndHorizontal();

            // 编辑区域 add by TangJian 2017/11/15 16:28:19
            EditorGUILayout.BeginHorizontal();

            // // 列表框 add by TangJian 2017/11/15 16:27:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            listScrollViewPos = EditorGUILayout.BeginScrollView(listScrollViewPos);

            for (int i = consumableDataList.Count - 1; i >= 0; i--)
            {
                var item = consumableDataList[i];
                EditorGUILayout.BeginHorizontal();

                if (MyGUI.Button("删除", GUILayout.Width(50)))
                {
                    consumableDataList.RemoveAt(i);
                }

                if (MyGUI.Button(item.id))
                {
                    currConsumableData = item;
                }

                if (MyGUI.Button("复制", GUILayout.Width(50)))
                {
                    var consumableData = Tools.Json2Obj<ConsumableData>(Tools.Obj2Json(item, true));
                    consumableDataList.Add(consumableData);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (MyGUI.Button("+"))
            {
                consumableDataList.Add(new ConsumableData());
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            // 编辑框 add by TangJian 2017/11/15 16:28:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            editScrollViewPos = EditorGUILayout.BeginScrollView(editScrollViewPos);
            if (currConsumableData != null)
            {
                MyGUI.ItemDataField(currConsumableData);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        // void showWindows()
        // {
        //     // Begin Window
        //     BeginWindows();

        //     // All GUI.Window or GUILayout.Window must come inside here
        //     //所有GUI.Window 或 GUILayout.Window 必须在这里面                            
        //     windowRect = GUILayout.Window(1, windowRect, (int id) =>
        //     {
        //         MyGUI.ItemDataField(ref currConsumableData);

        //         currConsumableData.id = MyGUI.TextFieldWithTitle("id", currConsumableData.id);
        //         currConsumableData.name = MyGUI.TextFieldWithTitle("name", currConsumableData.name);
        //         currConsumableData.icon = MyGUI.TextFieldWithTitle("icon", currConsumableData.icon);
        //         currConsumableData.itemType = (ItemType)MyGUI.IntFieldWithTitle("itemType", (int)currConsumableData.itemType);
        //         currConsumableData.equipType = (EquipType)MyGUI.IntFieldWithTitle("equipType", (int)currConsumableData.equipType);
        //         currConsumableData.skillId = MyGUI.TextFieldWithTitle("skillId", currConsumableData.skillId);

        //         MyGUI.Button("Hi");
        //         GUI.DragWindow();
        //     }, "Hi There");

        //     // Collect all the windows between the two.
        //     //在这两者之间搜集所有窗口
        //     EndWindows();
        // }

        void OnEnable()
        {
            title = "消耗品编辑器";
            loadConsumableData();
        }

        void loadConsumableData()
        {
            //string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + consumableDataFile);
            consumableDataDic = Tools.LoadOneData<ConsumableData>(Application.dataPath + "/" + "Resources_moved/Scripts/Consumable/ConsumableList");

            consumableDataList = consumableDataDic.Values.ToList();
            currConsumableData = consumableDataList[0];
        }

        void saveConsumableData()
        {
            consumableDataDic = consumableDataList.ToDictionary(item => item.id, item => item);

            //string jsonString = Tools.Obj2Json(consumableDataDic, true);
            //Debug.Log("jsonString = " + jsonString);
            //Tools.WriteStringFromFile(Application.dataPath + "/" + consumableDataFile, jsonString);
            Tools.SaveOneData<ConsumableData>(consumableDataDic, Application.dataPath + "/" + "Resources_moved/Scripts/Consumable/ConsumableList");

            // 生成预制体 add by TangJian 2017/11/16 17:42:05
            {
                foreach (var item in consumableDataDic)
                {
                    var itemObject = PrefabCreator.CreateDropItem(item.Value);
                    Tools.UpdatePrefab(itemObject, prefabPath + "/" + item.Value.id + ".prefab");
                    DestroyImmediate(itemObject);
                }
            }
        }

    }
}