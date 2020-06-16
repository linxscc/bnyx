using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;


namespace Tang.Editor
{
    public class AnimManagerEditorWindow : MyEditorWindow
    {
        [MenuItem("Window/动画编辑器")]
        static void Init()
        {
            AnimManagerEditorWindow window = (AnimManagerEditorWindow)EditorWindow.GetWindow(typeof(AnimManagerEditorWindow));
            window.Show();
        }

//        private string animManagerObjectPath = "Prefabs/Manager/ManagerObject";
        private GameObject animManagerObject;
        AnimManagerEditor animManagerEditor;
        private string animEffectSaveDataPath = "Assets/Resources_moved/Manager/AnimEffectSaveDataAsset.asset";
        string animEffectSaveDataFileName
        {
            get
            {
                return Application.dataPath + "/Resources_moved/Configs/AnimEffectSaveData";
                // return Tools.ResourcesPathToAbsolutePath("Configs/AnimEffectSaveData.json");
            }
        }

        AnimEffectSaveData animEffectSaveData;
        AnimEffectSaveDataAsset ada;
        void OnEnable()
        {
            title = "动画编辑器";
            
            Load();
            
        }

        GUIStyle boxStyle
        {
            get
            {
                return new GUIStyle("box");
            }
        }

        int[,] layout;

        int[,] Layout
        {
            get
            {
                if (layout == null)
                {
                    layout = new int[,]
                    {
                        { 0, 0, 3, 3},
                        { 1, 2, 3, 3},
                        { 1, 2, 3, 3},
                        { 1, 2, 3, 3},
                        { 1, 2, 3, 3},
                        { 1, 2, 3, 3},
                        { 1, 2, 3, 3},
                    };
                }
                return layout;
            }
        }
        
        public int focusIndex = 0;
        public int subFocusIndex = -1;


        Vector2 rootScrollViewPos = Vector2.zero;

        Vector2 animEffectDataFieldScrollPos;

        int testTimes = 1;
        float testInterval = 1;
        // Direction testDirection = Direction.Right;
        bool testFlip = false;
        Vector3 moveOrientation = Vector3.right;
        Vector3 testPosition = Vector3.zero;
        AnimEffectData testAnimEffectData;

        Vector2 verVector2 = new Vector2(60,0);
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
                    "读取",Load
                },
                {
                    "保存",Save
                }
            });
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 3), GUILayout.ExpandHeight(true));
            int removeIndex = -1;
            int copyIndex = -1;
            SearchContext = GUILayout.TextField(SearchContext);
            verVector2 = GUILayout.BeginScrollView(verVector2);
            
            
            if (!string.IsNullOrEmpty(SearchContext))
            {
                for (int i = 0; i < animEffectSaveData.animEffectDataList.Count; i++)
                {
                    if (Regex.IsMatch(animEffectSaveData.animEffectDataList[i].id.ToLower(),SearchContext))
                    {
                        ListID(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < animEffectSaveData.animEffectDataList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    ListID(i);
                    GUILayout.EndHorizontal();
                }
            }
            
            
            GUILayout.EndScrollView();
                
            if (MyGUI.Button("+"))
            {
                animEffectSaveData.animEffectDataList.Add(new AnimEffectData());
            }

            EditorGUILayout.EndVertical();
            
            
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 6), GUILayout.ExpandHeight(true));
             //  二级
                AnimEffectData animEffectData;

                if (animEffectSaveData.animEffectDataList.TryGet(focusIndex, out animEffectData))
                {
//                    int removeIndex = -1;
//                    int copyIndex = -1;

                    for (int i = 0; i < animEffectData.animEffectDataList.Count; i++)
                    {
                        GUILayout.BeginHorizontal();

                        int Index = MyGUIExtend.Instance.ListSingleButton("AnimEditor_1", i + ":" + animEffectData.animEffectDataList[i].id,
                            i, (
                                () =>
                                {
                                    subFocusIndex = i;
                                }));
                        
                        MyGUIExtend.Instance.Mouse_RightDrop(new Dictionary<string, Action>
                        {
                            {
                                "复制",(() =>
                                    {
                                        animEffectData.animEffectDataList.Add(
                                            Tools.DepthClone(animEffectData.animEffectDataList[Index]));
                                    })
                            },
                            {
                                "删除",(() => { animEffectData.animEffectDataList.RemoveAt(Index); })
                            }
                        });
                        

                        GUILayout.EndHorizontal();

                    }
                    
                    if (animEffectData.animEffectType == AnimEffectData.AnimEffectType.Group || animEffectData.animEffectType == AnimEffectData.AnimEffectType.Random)
                    {
                        if (MyGUI.Button("+"))
                        {
                            animEffectData.animEffectDataList.Add(new AnimEffectData());
                        }
                    }

                }
            EditorGUILayout.EndVertical();
            
            
            
            
            
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            AnimEffectData animeffectData;

            if (animEffectSaveData.animEffectDataList.TryGet(focusIndex, out animeffectData))
            {
                if (subFocusIndex >= 0)
                {
                    if (animeffectData.animEffectDataList.TryGet(subFocusIndex, out animeffectData))
                    {
                    }
                    else
                    {
                        // 找不到subFocusIndex位置的AnimEffectData
                        return;
                    }
                }

                animEffectDataFieldScrollPos = MyGUI.AnimEffectDataField(animeffectData, animEffectDataFieldScrollPos);


                if (Application.isPlaying)
                {
                    testTimes = MyGUI.IntFieldWithTitle("测试次数", testTimes);
                    testInterval = MyGUI.FloatFieldWithTitle("间隔时间", testInterval);
                    moveOrientation = MyGUI.Vector3WithTitle("移动方向", moveOrientation);
                    testPosition = MyGUI.Vector3WithTitle("位置", testPosition);
                    testFlip = MyGUI.ToggleWithTitle("翻转", testFlip);


                    if (MyGUI.Button("测试特效"))
                    {
                        EditorCoroutineSequenceRunner.RemoveCoroutine("AnimEffectTest");
                        EditorCoroutineSequenceRunner.AddCoroutineIfNot("AnimEffectTest", TestAnimEffect(animeffectData, testTimes, testInterval));
                    }
                }
            }
            EditorGUILayout.EndVertical();
            
            
            EditorGUILayout.EndHorizontal();
            
        }

        private string SearchContext;

        private void ListID(int i)
        {
            int Index = MyGUIExtend.Instance.ListSingleButton("AnimEditor", i + ":" + animEffectSaveData.animEffectDataList[i].id,
                i, (
                    () =>
                    {
                        focusIndex = i;
                        subFocusIndex = -1;
                    }));
            MyGUIExtend.Instance.Mouse_RightDrop(new Dictionary<string, Action>
            {
                {
                    "复制",(() =>
                    {
                        animEffectSaveData.animEffectDataList.Add(Tools.DepthClone(animEffectSaveData.animEffectDataList[Index]));
                    })
                },
                {
                    "删除",(() =>
                    {
                        animEffectSaveData.animEffectDataList.RemoveAt(Index);
                    })
                }
            });
        }

        public System.Collections.IEnumerator TestAnimEffect(AnimEffectData animEffectData, int times, float interval)
        {
            var playerObject = GameObject.Find("Player1");

            RoleController roleController = playerObject.GetComponent<RoleController>();

            for (int i = 0; i < times; i++)
            {
                AnimManager.Instance.PlayAnimEffect(animEffectData, playerObject.transform.position + testPosition, 0, testFlip, moveOrientation, playerObject.transform);

                float time = 0;
                while (time < interval)
                {
                    time += Time.deltaTime;
                    yield return 0;
                }
            }

            yield return 0;
        }

        void Load()
        {
            
            animEffectSaveData = Tools.Load<AnimEffectSaveData>(animEffectSaveDataFileName);
            if (animEffectSaveData == null)
            {
                animEffectSaveData = new AnimEffectSaveData();
            }
            ada = AssetDatabase.LoadAssetAtPath<AnimEffectSaveDataAsset>(animEffectSaveDataPath);
            if (ada == null)
            {
                ada = ScriptableObject.CreateInstance<AnimEffectSaveDataAsset>();
                AssetDatabase.CreateAsset(ada, animEffectSaveDataPath);
            }
            //animEffectSaveData = Resources.Load<AnimEffectSaveData>("Manager/AnimEffectSaveData");
        }

        void Save()
        {
            Tools.Save<AnimEffectSaveData>(animEffectSaveData, animEffectSaveDataFileName);
            //AnimManager.Instance.InitAnimEffect();

            //AssetDatabase.CreateAsset(animEffectSaveData, animEffectSaveDataPath);
            //ada = ScriptableObject.CreateInstance<AnimEffectSaveData>();
            ada.animEffectDataList = animEffectSaveData.animEffectDataList;
            EditorUtility.SetDirty(ada);
            AssetDatabase.SaveAssets();
            AnimManager.Instance.InitAnimEffect();
            //AssetDatabase.CreateAsset(ada, animEffectSaveDataPath);

        }
    }
}