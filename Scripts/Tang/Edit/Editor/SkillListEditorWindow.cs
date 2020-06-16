using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Tang.FrameEvent;


namespace Tang.Editor
{
    public class SkillListEditorWindow : MyEditorWindow
    {
//        [MenuItem("Window/Skill列表编辑")]
        static void Init()
        {
            SkillListEditorWindow window = (SkillListEditorWindow)EditorWindow.GetWindow(typeof(SkillListEditorWindow));
            window.Show();
        }
        private string skillDataFile = "Resources_moved/Scripts/Skill/SkillList.json";
        private string skillDataAssetFile = "Assets/Resources_moved/Manager/SkillListManagerDataAsset.asset";

        List<SkillListSaveData> skillListDatas = new List<SkillListSaveData>();
        Dictionary<string, SkillListSaveData> skillListDic = new Dictionary<string, SkillListSaveData>();

        AnimEffectSaveData animEffectSaveData;
        Vector2 Scrollpos = new Vector2();
        void OnEnable()
        {
            title = "Skill列表编辑";


            Load();


            // //
            // valueMonitorPool.Clear();
            // valueMonitorPool.AddMonitor<GameObject>(() => { return animManagerObject; }, (GameObject from, GameObject to) =>
            // {
            //     if (animManagerObject != null)
            //     {
            //         animManagerEditor = Editor.CreateEditor(animManagerObject.GetComponent<AnimManager>(), typeof(AnimManagerEditor)) as AnimManagerEditor;
            //     }
            // });

            // animManagerObject = Resources.Load<GameObject>(animManagerObjectPath);
        }

        GUIStyle boxStyle
        {
            get
            {
                return new GUIStyle("box");
            }
        }

        void OnDisable()
        {
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
                        { 0, 0, 0, 0},
                        { 1, 2, 2, 2},
                        { 1, 2, 2, 2},
                        { 1, 2, 2, 2},
                        { 1, 2, 2, 2},
                        { 1, 2, 2, 2},
                        { 1, 3, 3, 3},
                    };
                }
                return layout;
            }
        }

        public int focusIndex = -1;
        public int subFocusIndex = -1;
        public SkillListSaveData currskillListData;

        Vector2 rootScrollViewPos = Vector2.zero;

        Vector2 animEffectDataFieldScrollPos;

        int testTimes = 1;
        float testInterval = 1;
        // Direction testDirection = Direction.Right;
        bool testFlip = false;
        Vector3 moveOrientation = Vector3.right;
        Vector3 testPosition = Vector3.zero;
        AnimEffectData testAnimEffectData;

        //SkillListManagerDataAsset skillManagerDataAsset;

        void OnGUI()
        {
            MyGUI.Layout(
                position,
                boxStyle,
                Layout,
                (Rect position) =>
                {
                    EditorGUILayout.LabelField(skillDataFile);
                    if (MyGUI.Button("读取"))
                    {
                        Load();
                    }

                    if (MyGUI.Button("保存"))
                    {
                        Save();
                    }


                },
                (Rect position) =>
                {
                    int removeIndex = -1;
                    int copyIndex = -1;
                    if (skillListDatas.Count > 0)
                    {
                        for(int i = 0; i < skillListDatas.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            if (MyGUI.Button("复制", GUILayout.Width(20)))
                            {
                                copyIndex = i;
                            }
                            if (MyGUI.Button(skillListDatas[i].id))
                            {
                                currskillListData = skillListDatas[i];
                                focusIndex = i;
                            }
                            if (MyGUI.Button("-", GUILayout.Width(20)))
                            {
                                removeIndex = i;
                            }
                            GUILayout.EndHorizontal();
                        }
                    }

                    if (removeIndex >= 0)
                    {
                        skillListDatas.RemoveAt(removeIndex);
                    }

                    if (copyIndex >= 0)
                    {
                        //SkillListData skill = skillListDatas[copyIndex];
                        skillListDatas.Add(Tools.DepthClone(skillListDatas[copyIndex]));
                    }

                    if (MyGUI.Button("+"))
                    {
                        skillListDatas.Add(new SkillListSaveData());
                    }

                },
                (Rect position) =>
                {
                    if (currskillListData != null)
                    {
                        Scrollpos = EditorGUILayout.BeginScrollView(Scrollpos);
                        currskillListData.id = MyGUI.TextFieldWithTitle("id:", currskillListData.id);
                        currskillListData.roleSkillListFrameEventData.RoleSkillListType = (RoleSkillListType)MyGUI.EnumPopupWithTitle("类型:", currskillListData.roleSkillListFrameEventData.RoleSkillListType);
                        currskillListData.roleSkillListFrameEventData = MyGUI.Skilldatalistfield(currskillListData.roleSkillListFrameEventData, currskillListData.roleSkillListFrameEventData.RoleSkillListType);
                        EditorGUILayout.EndScrollView();
                    }
                },
                (Rect position) =>
                {
                    if (Application.isPlaying)
                    {
                        testTimes = MyGUI.IntFieldWithTitle("测试次数", testTimes);
                        testInterval = MyGUI.FloatFieldWithTitle("间隔时间", testInterval);
                        //moveOrientation = MyGUI.Vector3WithTitle("移动方向", moveOrientation);
                        //testPosition = MyGUI.Vector3WithTitle("位置", testPosition);
                        testFlip = MyGUI.ToggleWithTitle("翻转", testFlip);


                        if (MyGUI.Button("测试特效"))
                        {
                            EditorCoroutineSequenceRunner.RemoveCoroutine("SkillList");
                            EditorCoroutineSequenceRunner.AddCoroutineIfNot("SkillList", TestAnimEffect(currskillListData, testTimes, testInterval));
                        }
                    }
                });
        }

        public System.Collections.IEnumerator TestAnimEffect(SkillListSaveData skillListData, int times, float interval)
        {
            var playerObject = GameObject.Find("Player1");

            Direction direction = testFlip==true ? Direction.Left : Direction.Right;

            RoleController roleController = playerObject.GetComponent<RoleController>();

            for (int i = 0; i < times; i++)
            {
                SkillManager.Instance.useSkillList(skillListData.roleSkillListFrameEventData, direction, playerObject.transform, roleController.RoleData.TeamId);
                //AnimManager.Instance.PlayAnimEffect(animEffectData, playerObject.transform.position + testPosition, testFlip, moveOrientation, playerObject.transform);
                float time = 0;
                while (time < interval)
                {
                    time += Time.deltaTime;
                    yield return 0;
                }
            }

            yield return 0;
        }
        public override void Update()
        {
            base.Update();
            if (mouseOverWindow) // 如果鼠标在窗口上, 则重绘界面 add by TangJian 2017/09/28 21:57:44
            {
                base.Repaint();
            }
        }
        void Load()
        {
            string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + skillDataFile);
            skillListDic = Tools.Json2Obj<Dictionary<string, SkillListSaveData>>(jsonString);

            skillListDatas = skillListDic.Values.ToList();
            currskillListData = skillListDatas[0];



            //skillManagerDataAsset = AssetDatabase.LoadAssetAtPath<SkillListManagerDataAsset>(skillDataAssetFile);
            //if (skillManagerDataAsset == null)
            //{
            //    skillManagerDataAsset = ScriptableObject.CreateInstance<SkillListManagerDataAsset>();
            //    AssetDatabase.CreateAsset(skillManagerDataAsset, skillDataAssetFile);
            //}
        }

        void Save()
        {
            skillListDic = skillListDatas.ToDictionary(item => item.id, item => item);

            string jsonString = Tools.Obj2Json(skillListDic, true);
            //AssetDatabase.CreateAsset(jsonString, Application.dataPath + "/" + "Resources_moved/Scripts/Skill/SkillList");
            Debug.Log("jsonString = " + jsonString);
            Tools.WriteStringFromFile(Application.dataPath + "/" + skillDataFile, jsonString);

            //skillManagerDataAsset.skillListSaveDatas = skillListDatas;
            ////skillManagerDataAsset.skillListSaveDatasString = jsonString;

            //EditorUtility.SetDirty(skillManagerDataAsset);
            //AssetDatabase.SaveAssets();
        }
    }
}
