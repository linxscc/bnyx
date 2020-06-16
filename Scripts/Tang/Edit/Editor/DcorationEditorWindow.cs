using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


namespace Tang.Editor
{
    public class DecorationEditorWindow : EditorWindow
    {
        [MenuItem("Window/饰品编辑器")]
        static void Init()
        {
            DecorationEditorWindow window = (DecorationEditorWindow)EditorWindow.GetWindow(typeof(DecorationEditorWindow));
            window.Show();
        }

        private string decorationDataFile = "Resources_moved/Scripts/Decoration/Decoration.json";
        private string prefabPath = "Assets/Resources_moved/Prefabs/DropItem";

        public string excelPath
        {
            get { return PlayerPrefs.GetString("DecorationEditorWindow excelPath"); }
            set
            {
                PlayerPrefs.SetString("DecorationEditorWindow excelPath", value);
            }
        }

        Dictionary<string, DecorationData> decorationDataDic = new Dictionary<string, DecorationData>();
        List<DecorationData> decorationDataList = new List<DecorationData>();

        DecorationData currDecorationData;

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
            
            MyGUIExtend.Instance.ToolbarButton(new Dictionary<string, Action>
            {
                {
                    "读取",LoadData
                },
                {
                    "保存",SaveData
                },
                {
                    "制作预制体",CreatePrefabs
                }
            });
            
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));

            
            MyGUIExtend.Instance.Foldout("DcorationEditor","路径信息",(() =>
            { 
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(decorationDataFile);
                excelPath = MyGUI.TextFieldWithTitle("表格路径", excelPath);
                if (MyGUI.Button("载入Excel"))
                {
                    EditorTools.AnalyzeExcelToDic(excelPath, dic =>
                    {
                        EquipType equipType = (EquipType) Convert.ToInt32(dic["EquipType"]);
                        switch (equipType)
                        {
                            case EquipType.Decoration:
                            case EquipType.Glove:
                            case EquipType.Shoe:
                            case EquipType.Ring:
                                DecorationData newDecorationData = new DecorationData();
                                
                                foreach (var item in dic)
                                {
                                    switch (item.Key)
                                    {
                                        case "id":
                                            newDecorationData.id = Convert.ToString(item.Value);
                                            break;
                                        case "IconName":
                                            newDecorationData.icon = Convert.ToString(item.Value);
                                            break;
                                        case "name":
                                            newDecorationData.name = Convert.ToString(item.Value);
                                            break;
                                        case "Lv":
                                            newDecorationData.level = Convert.ToInt32(item.Value);
                                            break;
                                        case "desc":
                                            newDecorationData.desc = Convert.ToString(item.Value);
                                            break;
                                        case "Hp":
                                            newDecorationData.attrData.hpMax = Convert.ToSingle(item.Value);
                                            break;
                                        case "defence":
                                            newDecorationData.attrData.def = Convert.ToSingle(item.Value);
                                            break;
                                        case "ATK":
                                            newDecorationData.attrData.atk = Convert.ToSingle(item.Value);
                                            break;
                                        case "speed":
                                            newDecorationData.attrData.atkSpeedScale = Convert.ToSingle(item.Value);
                                            break;
                                        case "Critical":
                                            newDecorationData.attrData.criticalRate = Convert.ToSingle(item.Value);
                                            break;
                                        case "CriticalDamage":
                                            newDecorationData.attrData.criticalDamage = Convert.ToSingle(item.Value);
                                            break;
                                        case "move":
                                            newDecorationData.attrData.moveSpeedScale = Convert.ToSingle(item.Value);
                                            break;
                                        case "EquipType":
                                            newDecorationData.equipType = equipType;
                                            break;
                                    }
                                }

                                int index = decorationDataList.FindIndex((data) =>
                                {
                                    return data.id == newDecorationData.id;
                                });

                                if (index >= 0)
                                {
                                    decorationDataList.RemoveAt(index);
                                }

                                decorationDataList.Add(newDecorationData);

                                break;
                        }
                    });
                }
                EditorGUILayout.EndHorizontal();
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(prefabPath);
                EditorGUILayout.EndHorizontal();
            }));
            
           

            

            // 编辑区域 add by TangJian 2017/11/15 16:28:19
            EditorGUILayout.BeginHorizontal();

            // // 列表框 add by TangJian 2017/11/15 16:27:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            listScrollViewPos = EditorGUILayout.BeginScrollView(listScrollViewPos);

            for (int i = decorationDataList.Count - 1; i >= 0; i--)
            {
                var item = decorationDataList[i];

                EditorGUILayout.BeginHorizontal();
                int Index = MyGUIExtend.Instance.ListSingleButton("DocrationEditor", item.id, i, (() => { currDecorationData = item; }));
                
                MyGUIExtend.Instance.Mouse_RightDrop(new Dictionary<string, Action>
                {
                    {
                        "删除",(() => { decorationDataList.RemoveAt(Index); })
                    },
                    {
                        "复制", () =>
                        {
                            var decorationData = Tools.Json2Obj<DecorationData>(Tools.Obj2Json(decorationDataList[Index], true));
                            decorationDataList.Add(decorationData);
                        }
                    }
                });
                EditorGUILayout.EndHorizontal();
            }

            if (MyGUI.Button("+"))
            {
                decorationDataList.Add(new DecorationData());
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            // 编辑框 add by TangJian 2017/11/15 16:28:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            editScrollViewPos = EditorGUILayout.BeginScrollView(editScrollViewPos);
            if (currDecorationData != null)
            {
                MyGUI.ItemDataField(currDecorationData);
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void OnEnable()
        {
            title = "饰品编辑器";
            LoadData();
        }

        void LoadExcel()
        {
            
        }

        void LoadData()
        {
            //string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + decorationDataFile);
            decorationDataDic = Tools.LoadOneData<DecorationData>(Application.dataPath + "/" + "Resources_moved/Scripts/Decoration/DecorationList");

            decorationDataList = decorationDataDic.Values.ToList();
            currDecorationData = decorationDataList[0];
        }

        void SaveData()
        {
            decorationDataDic = decorationDataList.ToDictionary(item => item.id, item => item);

            string jsonString = Tools.Obj2Json(decorationDataDic, true);
            Debug.Log("jsonString = " + jsonString);
            Tools.WriteStringFromFile(Application.dataPath + "/" + decorationDataFile, jsonString);
            
            Tools.SaveOneData<DecorationData>(decorationDataDic, Application.dataPath + "/" + "Resources_moved/Scripts/Decoration/DecorationList");
        }

        void CreatePrefabs()
        {
            // 生成预制体 add by TangJian 2017/11/16 17:42:05
            {
                foreach (var item in decorationDataDic)
                {
                    var itemObject = PrefabCreator.CreateDropItem(item.Value);
                    Tools.UpdatePrefab(itemObject, prefabPath + "/" + item.Value.id + ".prefab");
                    DestroyImmediate(itemObject);
                }
            }
        }

    }
}