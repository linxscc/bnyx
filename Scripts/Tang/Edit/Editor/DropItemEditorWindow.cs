using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Tang.Editor
{
    public class DropItemEditorWindow : EditorWindow
    {
        [MenuItem("Assets/创建ScriptableObject/DropItemsDataAsset")]
        public static void CreateScriptableObject()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            Debug.Log(path);
            if (System.IO.Path.HasExtension(path))
            {
                Debug.LogError("请点击文件夹空白区域创建");
            }
            else
            {
                string fileName = path + "/" + "DropItemsDataAsset.asset";
                DropItemsDataAsset dropItemsDataAsset = ScriptableObject.CreateInstance<DropItemsDataAsset>();
                AssetDatabase.CreateAsset(dropItemsDataAsset, fileName);
            }
        }

        [MenuItem("Window/掉落物品配置")]
        static void Init()
        {
            DropItemEditorWindow window = (DropItemEditorWindow)EditorWindow.GetWindow(typeof(DropItemEditorWindow));
            window.titleContent = new GUIContent("掉落物品配置");
            window.Show();
        }

        public DropItemsDataAsset dropItemsDataAsset;

        private string excelPath
        {
            get { return PlayerPrefs.GetString("DropItemEditorWindow/ExcelPath"); }
            set
            {
                PlayerPrefs.SetString("DropItemEditorWindow/ExcelPath", value);
            }
        }

        public string text = "123";
        public Vector2 textScrollViewPos;
        
        private void OnEnable()
        {
            dropItemsDataAsset =
                AssetDatabase.LoadAssetAtPath<DropItemsDataAsset>("Assets/Resources_moved/Manager/DropItemsDataAsset.asset");
        }

        private void OnGUI()
        {
            dropItemsDataAsset = (DropItemsDataAsset) EditorGUILayout.ObjectField("dropItemsDataAsset",
                dropItemsDataAsset, typeof(DropItemsDataAsset));
            
            excelPath = MyGUI.TextFieldWithTitle("表格路径", excelPath);
            
//            text = GUILayout.TextArea(text, new GUIStyle(){});

            if (MyGUI.Button("载入表格"))
            {
                dropItemsDataAsset.DropItemList.Clear();

                EditorTools.AnalyzeExcel(excelPath, (List<KeyValuePair<string, object>> pairs) =>
                {
                    DropItemsDataAsset.DropItem dropItem = new DropItemsDataAsset.DropItem();
                    foreach (var pair in pairs)
                    {
                        switch (pair.Key)
                        {
                            case "id":
                                dropItem.Id = Convert.ToString(pair.Value);
                                break;
                            case "MonsterID":
                                dropItem.RoleId = Convert.ToString(pair.Value);
                                break;
                            case "Name":
                                dropItem.RoleName = Convert.ToString(pair.Value);
                                break;
                            case "Lv":
                                string[] lvs = Convert.ToString(pair.Value).Split(';');
                                dropItem.LvFrom = Convert.ToInt32(lvs[0]);
                                dropItem.LvTo = Convert.ToInt32(lvs[1]);
                                break;
                            case "DropOne":
                                dropItem.DropOne = Convert.ToInt32(pair.Value);
                                break;
                            case "DropTwo":
                                dropItem.DropTwo = Convert.ToInt32(pair.Value);
                                break;
                            case "DropThree":
                                dropItem.DropThree = Convert.ToInt32(pair.Value);
                                break;
                            case "DropSoul":
                                dropItem.DropSoul = Convert.ToInt32(pair.Value);
                                break;
                        }    
                    }
                    dropItemsDataAsset.DropItemList.Add(dropItem);
                });
                
                EditorUtility.SetDirty(dropItemsDataAsset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (MyGUI.Button("测试"))
            {
                text = "";

                int instanceLevelMin = 1;
                int instanceLevelMax = 99;
                
                var roleIds = DropItemsDataAsset.Instance.GetAllRoleId();
                foreach (var roleId in roleIds)
                {
                    for (int instanceLevel = instanceLevelMin; instanceLevel <= instanceLevelMax; instanceLevel++)
                    {
                        List<string> dropItemIdList = DropItemsDataAsset.Instance.GetDropItemIdList(instanceLevel, roleId);
                        int soulCount = DropItemsDataAsset.Instance.GetDropSoulCount(instanceLevel, roleId);

                        text += "id: " + roleId + ", " + "instanceLevel: " + instanceLevel + ", dropItems: ";
                        EditorUtility.DisplayProgressBar("测试", "id: " + roleId + ", " + "instanceLevel: " + instanceLevel + ", dropItems: ", (float)instanceLevel / instanceLevelMax);

                        if (dropItemIdList != null)
                        {
                            for (int i = 0; i < dropItemIdList.Count; i++)
                            {
                                var dropItemId = dropItemIdList[i];
                                if (i == dropItemIdList.Count - 1)
                                {
                                    text += dropItemId;
                                    text += "\n";
                                }
                                else
                                {
                                    text += dropItemId;
                                    text += ", ";
                                }
                            }
                        }
                        
                        // 掉落魂数目 add by TangJian 2019/3/13 12:55
                        if (soulCount > 0)
                        {
                            text += ", Soulx" + soulCount ;
                            
                        }
                        
                        text += "\n";
                    }        
                }

                EditorUtility.ClearProgressBar();
            }

            if (MyGUI.Button("测试1"))
            {
//                List<string> itemList = ItemManager.Instance.GetItemIdListDescendingOrder(0, 1, 0);
//                foreach (var itemId in itemList)
//                {
//                    text += itemId;
//                    text += "\n";
//                }

                List<string> itemList = DropItemsDataAsset.Instance.GetDropItemIdList(9, "Human");
                foreach (var itemId in itemList)
                {
                    text += itemId;
                    text += "\n";
                }
            }
            
            textScrollViewPos = GUILayout.BeginScrollView(textScrollViewPos);
            text = EditorGUILayout.TextArea(text);
            GUILayout.EndScrollView();
        }
    }
}