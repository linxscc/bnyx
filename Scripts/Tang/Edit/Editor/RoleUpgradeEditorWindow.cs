using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Tang.Editor
{
    public class RoleUpgradeEditorWindow : EditorWindow
    {
        [MenuItem("Assets/创建ScriptableObject/RoleUpgradeDataAsset")]
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
                string fileName = path + "/" + "RoleUpgradeDataAsset.asset";
                RoleUpgradeDataAsset scriptableObject = ScriptableObject.CreateInstance<RoleUpgradeDataAsset>();
                AssetDatabase.CreateAsset(scriptableObject, fileName);
            }
        }

        [MenuItem("Window/角色升级配置")]
        static void Init()
        {
            RoleUpgradeEditorWindow window = (RoleUpgradeEditorWindow)EditorWindow.GetWindow(typeof(RoleUpgradeEditorWindow));
            window.titleContent = new GUIContent("角色升级配置");
            window.Show();
        }

        public RoleUpgradeDataAsset roleUpgradeDataAsset;
        
        private string excelPath
        {
            get { return PlayerPrefs.GetString("RoleUpgradeEditorWindow/ExcelPath"); }
            set
            {
                PlayerPrefs.SetString("RoleUpgradeEditorWindow/ExcelPath", value);
            }
        }

        private Vector2 outputTextScrollViewPos;
        private string outputText;
        
        private void OnEnable()
        {
            roleUpgradeDataAsset =
                AssetDatabase.LoadAssetAtPath<RoleUpgradeDataAsset>("Assets/Resources_moved/Manager/RoleUpgradeDataAsset.asset");
        }

        private void OnGUI()
        {
            roleUpgradeDataAsset = (RoleUpgradeDataAsset) EditorGUILayout.ObjectField("roleUpgradeDataAsset",
                roleUpgradeDataAsset, typeof(DropItemsDataAsset));
            
            excelPath = MyGUI.TextFieldWithTitle("表格路径", excelPath);
            
//            text = GUILayout.TextArea(text, new GUIStyle(){});

            if (MyGUI.Button("载入表格"))
            {
                roleUpgradeDataAsset.Clear();

                EditorTools.AnalyzeExcel(excelPath, (List<KeyValuePair<string, object>> pairs) =>
                {
                    DropItemsDataAsset.DropItem dropItem = new DropItemsDataAsset.DropItem();
                    
                    int lv = -1;
                    int exp = -1;
                    
                    foreach (var pair in pairs)
                    {
                        
                        switch (pair.Key)
                        {
                            case "lv":
                                lv = Convert.ToInt32(pair.Value);
                                break;
                            case "exp":
                                exp = Convert.ToInt32(pair.Value);
                                break;
                        }    
                    }
                    
                    Debug.Assert(lv>=1 && exp >=0);
                    roleUpgradeDataAsset.AddItem(lv, exp);
                });
                
                EditorUtility.SetDirty(roleUpgradeDataAsset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            if (MyGUI.Button("测试"))
            {
                outputText = "";

                outputText += "通过经验获取等级:\n";
                for (int exp = 0; exp <= RoleUpgradeDataAsset.Instance.GetLevelMaxExp() * 2; exp++)
                {
                    int level = RoleUpgradeDataAsset.Instance.GetLevel(exp);

                    outputText += "exp: " + exp + " -> level: " + level + "\n";
                }

                for (int i = 0; i < 10; i++)
                {
                    outputText += "\n";
                }
                
                outputText += "通过等级获取经验:\n";
                for (int level = 1; level <= RoleUpgradeDataAsset.Instance.GetLevelMax(); level++)
                {
                    int exp = RoleUpgradeDataAsset.Instance.GetExp(level);

                    outputText += "level: " + level + " -> exp: " + exp + "\n";
                } 
            }

            outputTextScrollViewPos = EditorGUILayout.BeginScrollView(outputTextScrollViewPos, GUILayout.ExpandWidth(true));
            
            outputText = EditorGUILayout.TextArea(outputText);
            
            EditorGUILayout.EndScrollView();
        }
    }
}