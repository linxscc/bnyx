using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEngine.AddressableAssets;

namespace Tang.Editor
{
    public class WeaponEditorWindow : EditorWindow
    {
        [MenuItem("Window/武器编辑器")]
        static void Init()
        {
            WeaponEditorWindow window = (WeaponEditorWindow)EditorWindow.GetWindow(typeof(WeaponEditorWindow));
            window.Show();
        }

        private string weaponDataFile = "Resources_moved/Scripts/Weapon/Weapon.json";
        private string prefabPath = "Assets/Resources_moved/Prefabs/DropItem";
        
        Dictionary<string, WeaponData> weaponDataDic = new Dictionary<string, WeaponData>();
        List<WeaponData> weaponDataList = new List<WeaponData>();

        WeaponData currWeaponData;

        string skeletonDataAssetPath;
        Spine.Unity.SkeletonDataAsset skeletonDataAsset;
        Vector2 listScrollViewPos = Vector2.zero;
        Vector2 editScrollViewPos = Vector2.zero;

        Rect windowRect = new Rect(100, 100, 500, 500);
        string excelPath;

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
                    "读取",loadWeapon
                },
                {
                    "保存",saveWeapon
                },
                {
                    "制作当前预制体",CreatCurrentPrefab
                }
            });
            
            
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));
            
            MyGUIExtend.Instance.Foldout("WeaponEditor","路径信息",(() =>
            {
                EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(weaponDataFile);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            excelPath = MyGUI.TextFieldWithTitle("表格路径", excelPath);
            if (MyGUI.Button("载入表格"))
            {
                EditorTools.AnalyzeExcelToDic(excelPath,dic=> 
                {
                    EquipType equipType = (EquipType)Convert.ToInt32(dic["EquipType"]);
                    bool newWeaponData = true;
                    switch (equipType)
                    {
                        case EquipType.Lswd:
                            break;
                        case EquipType.Shield:
                            break;
                        case EquipType.Swd:
                            break;
                        case EquipType.Sswd:
                            break;
                        case EquipType.Saxe:
                            break;
                        case EquipType.Blunt:
                            break;
                        case EquipType.Katana:
                            break;
                        case EquipType.Bow:
                            break;
                        case EquipType.Spear:
                            break;
                        default:
                            newWeaponData = false;
                            break;
                    }
                    if (newWeaponData)
                    {
                        WeaponData NewWeaponData = new WeaponData();
                        foreach (var item in dic)
                        {
                            switch (item.Key)
                            {
                                case "id":
                                    NewWeaponData.id = Convert.ToString(item.Value);
                                    break;
                                case "IconName":
                                    NewWeaponData.icon = Convert.ToString(item.Value);
                                    break;
                                case "name":
                                    NewWeaponData.name = Convert.ToString(item.Value);
                                    break;
                                case "Lv":
                                    NewWeaponData.level = Convert.ToInt32(item.Value);
                                    break;
                                case "desc":
                                    NewWeaponData.desc = Convert.ToString(item.Value);
                                    break;
                                case "Hp":
                                    NewWeaponData.attrData.hpMax = Convert.ToSingle(item.Value);
                                    break;
                                case "defence":
                                    NewWeaponData.attrData.def = Convert.ToSingle(item.Value);
                                    break;
                                case "ATK":
                                    NewWeaponData.attrData.atk = Convert.ToSingle(item.Value);
                                    break;
                                case "speed":
                                    NewWeaponData.attrData.atkSpeedScale = Convert.ToSingle(item.Value);
                                    break;
                                case "Critical":
                                    NewWeaponData.attrData.criticalRate = Convert.ToSingle(item.Value);
                                    break;
                                case "CriticalDamage":
                                    NewWeaponData.attrData.criticalDamage = Convert.ToSingle(item.Value);
                                    break;
                                case "move":
                                    NewWeaponData.attrData.moveSpeedScale = Convert.ToSingle(item.Value);
                                    break;
                                case "EquipType":
                                    NewWeaponData.equipType = equipType;
                                    break;
                            }
                        }

                        int index = weaponDataList.FindIndex((data) => data.id == NewWeaponData.id);

                        if (index >= 0)
                        {
                            string icon = weaponDataList[index].icon;
                            weaponDataList.RemoveAt(index);
                            NewWeaponData.icon = icon;
                        }

                        weaponDataList.Add(NewWeaponData);
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
            
            
            

            // 编辑区域 add by TangJian 2017/11/15 16:28:19
            EditorGUILayout.BeginHorizontal();
            
            // // 列表框 add by TangJian 2017/11/15 16:27:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            listScrollViewPos = EditorGUILayout.BeginScrollView(listScrollViewPos);

            for (int i = weaponDataList.Count - 1; i >= 0; i--)
            {
                var item = weaponDataList[i];
                EditorGUILayout.BeginHorizontal();

                int Index = MyGUIExtend.Instance.ListSingleButton("WeaponEditor", item.id, i, (() => { currWeaponData = item; }));
                
                MyGUIExtend.Instance.Mouse_RightDrop(new Dictionary<string, Action>
                {
                    {
                        "复制",(() =>
                        {
                            var weaponData = Tools.Json2Obj<WeaponData>(Tools.Obj2Json(weaponDataList[Index], true));
                            weaponDataList.Add(weaponData);
                        })
                    },
                    {
                        "删除",(() => { weaponDataList.RemoveAt(Index); })
                    }
                });
                EditorGUILayout.EndHorizontal();
            }

            if (MyGUI.Button("+"))
            {
                weaponDataList.Add(new WeaponData());
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            // 编辑框 add by TangJian 2017/11/15 16:28:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            editScrollViewPos = EditorGUILayout.BeginScrollView(editScrollViewPos);
            if (currWeaponData != null)
            {
                MyGUI.ItemDataField(currWeaponData,skeletonDataAssetPath);
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void OnEnable()
        {
            title = "武器编辑器";
            loadWeapon();
        }

        void loadWeapon()
        {
            //string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + weaponDataFile);
            //weaponDataDic = Tools.Json2Obj<Dictionary<string, WeaponData>>(jsonString, true);
            weaponDataDic = Tools.LoadOneData<WeaponData>(Application.dataPath + "/" + "Resources_moved/Scripts/Weapon/WeaponList");

            skeletonDataAssetPath = Tools.ReadStringFromFile(Application.dataPath + "/"+"Resources_moved/Scripts/Weapon/WeaponskeletonDataAssetPath.json");

            weaponDataList = weaponDataDic.Values.ToList();
            currWeaponData = weaponDataList[0];
        }

        void saveWeapon()
        {
            weaponDataDic = weaponDataList.ToDictionary(item => item.id, item => item);

            string jsonString = Tools.Obj2Json(weaponDataDic, true);
            Debug.Log("jsonString = " + jsonString);
            Tools.WriteStringFromFile(Application.dataPath + "/" + weaponDataFile, jsonString);
            
            Tools.SaveOneData<WeaponData>(weaponDataDic, Application.dataPath + "/" + "Resources_moved/Scripts/Weapon/WeaponList");
            Tools.WriteStringFromFile(Application.dataPath + "/"+"Resources_moved/Scripts/Weapon/WeaponskeletonDataAssetPath.json",skeletonDataAssetPath);
            UnLoad();
        }

        private void CreatCurrentPrefab()
        {
            saveWeapon();
            var itemObject = PrefabCreator.CreateDropItem(currWeaponData);
            Tools.UpdatePrefab(itemObject, prefabPath + "/" + currWeaponData.id + ".prefab"); 
            DestroyImmediate(itemObject);
        }
        
        public async void UnLoad()
        {
            var obj = await AssetManager.LoadAssetAsync<TextAsset>("WeaponDatas");
            Addressables.Release(obj);   
        }
    }
}