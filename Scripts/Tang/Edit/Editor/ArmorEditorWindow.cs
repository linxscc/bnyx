using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace Tang.Editor
{
    public class ArmorEditorWindow : EditorWindow
    {
        [MenuItem("Window/护甲编辑器")]
        static void Init()
        {
            ArmorEditorWindow window = (ArmorEditorWindow)EditorWindow.GetWindow(typeof(ArmorEditorWindow));
            window.Show();
        }

        private string armorDataFile = "Resources_moved/Scripts/Armor/Armor.json";
        private string prefabPath = "Assets/Resources_moved/Prefabs/DropItem";

        Dictionary<string, ArmorData> armorDataDic = new Dictionary<string, ArmorData>();
        List<ArmorData> armorDataList = new List<ArmorData>();

        ArmorData currArmorData;

        string skeletonDataAssetPath;
        Spine.Unity.SkeletonDataAsset skeletonDataAsset;

        Vector2 listScrollViewPos = Vector2.zero;
        Vector2 editScrollViewPos = Vector2.zero;
        string excelPath;
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
                    "读取",(loadArmor)
                },
                {
                    "保存",saveArmor
                },
                {
                    "制作预制体",saveArmor
                }
            });
            
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));

            MyGUIExtend.Instance.Foldout("ArmorEditor","路径及数据信息",(() =>
            {
                 EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(armorDataFile);
            
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            excelPath = MyGUI.TextFieldWithTitle("表格路径", excelPath);
            if (MyGUI.Button("载入表格"))
            {
                EditorTools.AnalyzeExcelToDic(excelPath,dic=> 
                {
                    EquipType equipType = (EquipType)Convert.ToInt32(dic["EquipType"]);
                    switch (equipType)
                    {
                        case EquipType.Armor:
                            ArmorData NewArmorData = new ArmorData();
                            foreach (var item in dic)
                            {
                                switch (item.Key)
                                {
                                    case "id":
                                        NewArmorData.id = Convert.ToString(item.Value);
                                        break;
                                    case "IconName":
                                        NewArmorData.icon = Convert.ToString(item.Value);
                                        break;
                                    case "name":
                                        NewArmorData.name = Convert.ToString(item.Value);
                                        break;
                                    case "Lv":
                                        NewArmorData.level = Convert.ToInt32(item.Value);
                                        break;
                                    case "desc":
                                        NewArmorData.desc = Convert.ToString(item.Value);
                                        break;
                                    case "Hp":
                                        NewArmorData.attrData.hpMax = Convert.ToSingle(item.Value);
                                        break;
                                    case "defence":
                                        NewArmorData.attrData.def = Convert.ToSingle(item.Value);
                                        break;
                                    case "ATK":
                                        NewArmorData.attrData.atk = Convert.ToSingle(item.Value);
                                        break;
                                    case "speed":
                                        NewArmorData.attrData.atkSpeedScale = Convert.ToSingle(item.Value);
                                        break;
                                    case "Critical":
                                        NewArmorData.attrData.criticalRate = Convert.ToSingle(item.Value);
                                        break;
                                    case "CriticalDamage":
                                        NewArmorData.attrData.criticalDamage = Convert.ToSingle(item.Value);
                                        break;
                                    case "move":
                                        NewArmorData.attrData.moveSpeedScale = Convert.ToSingle(item.Value);
                                        break;
                                    case "EquipType":
                                        NewArmorData.equipType = equipType;
                                        break;
                                }
                            }

                            int index = armorDataList.FindIndex((data) =>
                            {
                                return data.id == NewArmorData.id;
                            });

                            if (index >= 0)
                            {
                                string icon = armorDataList[index].icon;
                                armorDataList.RemoveAt(index);
                                NewArmorData.icon = icon;
                            }

                            armorDataList.Add(NewArmorData);
                            break;
                        default:
                            break;
                    }
                    
                });
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(prefabPath);
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            skeletonDataAsset = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(skeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
            skeletonDataAsset = (Spine.Unity.SkeletonDataAsset)EditorGUILayout.ObjectField(new GUIContent("SkeletonDataAsstet"), skeletonDataAsset, typeof(Spine.Unity.SkeletonDataAsset), true);
            skeletonDataAssetPath = AssetDatabase.GetAssetPath(skeletonDataAsset);

            EditorGUILayout.EndHorizontal();
            }));
           
            EditorGUILayout.BeginHorizontal();

            // // 列表框 add by TangJian 2017/11/15 16:27:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            listScrollViewPos = EditorGUILayout.BeginScrollView(listScrollViewPos);

            for (int i = armorDataList.Count - 1; i >= 0; i--)
            {
                var item = armorDataList[i];
                EditorGUILayout.BeginHorizontal();
                
                int Index = MyGUIExtend.Instance.ListSingleButton("ArmorEditor",item.id,i,(() => { currArmorData = item; }) );
                
                MyGUIExtend.Instance.Mouse_RightDrop(new Dictionary<string, Action>
                {
                    {
                        "删除",(() => { armorDataList.RemoveAt(Index); })
                    },
                    {
                        "复制",(() => 
                        { 
                            var armorData = Tools.Json2Obj<ArmorData>(Tools.Obj2Json(armorDataList[Index], true));
                            armorDataList.Add(armorData);
                        })
                    }
                });
                
                EditorGUILayout.EndHorizontal();
            }

            if (MyGUI.Button("+"))
            {
                armorDataList.Add(new ArmorData());
            }
            
            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            // 编辑框 add by TangJian 2017/11/15 16:28:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            editScrollViewPos = EditorGUILayout.BeginScrollView(editScrollViewPos);
            if (currArmorData != null)
            {
                MyGUI.ItemDataField(currArmorData,skeletonDataAssetPath);
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void OnEnable()
        {
            title = "护甲编辑器";
            loadArmor();
        }

        void loadArmor()
        {
            //string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + armorDataFile);
            armorDataDic = Tools.LoadOneData<ArmorData>(Application.dataPath + "/" + "Resources_moved/Scripts/Armor/ArmorList");

            skeletonDataAssetPath=Tools.ReadStringFromFile(Application.dataPath + "/" +"Resources_moved/Scripts/Armor/ArmorskeletonDataAssetPath.json");

            armorDataList = armorDataDic.Values.ToList();
            currArmorData = armorDataList[0];
        }

        void saveArmor()
        {
            armorDataDic = armorDataList.ToDictionary(item => item.id, item => item);

            //string jsonString = Tools.Obj2Json(armorDataDic, true);

            //Debug.Log("jsonString = " + jsonString);
            //Tools.WriteStringFromFile(Application.dataPath + "/" + armorDataFile, jsonString);
            Tools.SaveOneData<ArmorData>(armorDataDic, Application.dataPath + "/" + "Resources_moved/Scripts/Armor/ArmorList");

            Tools.WriteStringFromFile(Application.dataPath + "/" +"Resources_moved/Scripts/Armor/ArmorskeletonDataAssetPath.json",skeletonDataAssetPath);

            // 生成预制体 add by TangJian 2017/11/16 17:42:05
            {
                foreach (var item in armorDataDic)
                {
                    var itemObject = PrefabCreator.CreateDropItem(item.Value);
                    Tools.UpdatePrefab(itemObject, prefabPath + "/" + item.Value.id + ".prefab");
                    DestroyImmediate(itemObject);
                }
            }
        }

    }
}