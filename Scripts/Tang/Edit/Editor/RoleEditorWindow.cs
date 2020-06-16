using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Spine.Unity;
using Tang.Editor;
using System;
using System.Text.RegularExpressions;
using ZS;
using Random = UnityEngine.Random;

namespace Tang
{
    public enum RoleControllerType
    {
        RoleController = 1,
        HumanController = 2,
        BaronController = 3,
        CaptionController = 4,
        PrisonerController = 5
    }

    public class agentsdata : IEquatable<agentsdata>
    {
        public agentsdata(float r=0 ,float h=0,string ne="")
        {
            radius = r;
            height = h;
            name = ne;
        }
        public string name;
        public List<string> childrenName = new List<string>();
        public float radius;
        public float height;
        public bool Equals(agentsdata other)
        {
            if (radius== other.radius&&height==other.height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    [System.Serializable]
    //------------生成人物预置体所需的数据-------------add by tianjinpeng 2018/06/19 11:17:42
    public class CreateRoleData
    {
        public CreateRoleData(string id = "")
        {
            roleData.Id = id;
        }

        public bool NeedTurnBack { get { return roleData.NeedTurnBack; } set { roleData.NeedTurnBack = value; } }
        public AtkPropertyType atkPropertyType { get { return roleData.atkPropertyType; } set { roleData.atkPropertyType = value; } }
        public string Id { get { return roleData.Id; } set { roleData.Id = value; } }
        public string name;
        public string Prefab { get { return roleData.Prefab; } set { roleData.Prefab = value; } }
        public float Hp { get { return roleData.Hp; } set { roleData.Hp = value; } }
        public float HpMax { get { return roleData.HpMax; } set { roleData.HpMax = value; } }
        public List<string> DamageTargetNameList { get { return roleData.DamageTargetNameList; } set { roleData.DamageTargetNameList = value; } }
        public float Mp { get { return roleData.Mp; } set { roleData.Mp = value; } }
        public float MpMax { get { return roleData.MpMax; } set { roleData.MpMax = value; } }
        public float Tili { get { return roleData.Tili; } set { roleData.Tili = value; } }
        public float TiliMax { get { return roleData.TiliMax; } set { roleData.TiliMax = value; } }
        public float Def { get { return roleData.Def; } set { roleData.Def = value; } }
        public float Atk { get { return roleData.Atk; } set { roleData.Atk = value; } }
        public float AtkMin { get { return roleData.AtkMin; } set { roleData.AtkMin = value; } }
        public float AtkMax { get { return roleData.AtkMax; } set { roleData.AtkMax = value; } }
        public float MagicMin { get { return roleData.MagicMin; } set { roleData.MagicMin = value; } }
        public float MagicMax { get { return roleData.MagicMax; } set { roleData.MagicMax = value; } }
        public float AtkSpeedScale { get { return roleData.AtkSpeedScale; } set { roleData.AtkSpeedScale = value; } }
        public float MoveSpeedScale { get { return roleData.MoveSpeedScale; } set { roleData.MoveSpeedScale = value; } }
        public float RunSpeed { get { return roleData.RunSpeed; } set { roleData.RunSpeed = value; } }
        public float WalkSpeed { get { return roleData.WalkSpeed; } set { roleData.WalkSpeed = value; } }
        public float DefenseRepellingResistance { get { return roleData.DefenseRepellingResistance; } set { roleData.DefenseRepellingResistance = value; } }
        public float RepellingResistance { get { return roleData.RepellingResistance; } set { roleData.RepellingResistance = value; } }
        public float CriticalRate { get { return roleData.CriticalRate; } set { roleData.CriticalRate = value; } }
        public float CriticalDamage { get { return roleData.CriticalDamage; } set { roleData.CriticalDamage = value; } }
        public Vector3 DefendBoundsize { get { return roleData.DefendBoundsize; } set { roleData.DefendBoundsize = value; } }
        public Vector3 DefendBoundcenter { get { return roleData.DefendBoundcenter; } set { roleData.DefendBoundcenter = value; } }
        public float PauseTime { get { return roleData.PauseTime; } set { roleData.PauseTime = value; } }
        public float physicalDef { get { return roleData.PhysicalDef; } set { roleData.PhysicalDef = value; } }
        public float magicalDef { get { return roleData.MagicalDef; } set { roleData.MagicalDef = value; } }
        public float JumpSpeed { get { return roleData.JumpSpeed; } set { roleData.JumpSpeed = value; } }
        public bool Atklightforcetype { get { return roleData.Atklightforcetype; } set { roleData.Atklightforcetype = value; } }
        public bool Atkheavyforcetype { get { return roleData.Atkheavyforcetype; } set { roleData.Atkheavyforcetype = value; } }
        public bool Atkmoderateforcetype { get { return roleData.Atkmoderateforcetype; } set { roleData.Atkmoderateforcetype = value; } }
        public bool CollideWithRole { get { return roleData.CollideWithRole; } set { roleData.CollideWithRole = value; } }
        public List<WeaknessData> WeaknessDataList { get { return roleData.WeaknessDataList; } set { roleData.WeaknessDataList = value; } }
        
        

        public float Damping
        {
            get => roleData.Damping;
            set => roleData.Damping = value;
        }

        // 
        public List<HurtMode> HurtModes = new List<HurtMode>();
        public bool IsFollowBone =false;
        
        //人物角色结构大小
        public Vector3 size = new Vector3(1, 1, 1);
        public bool withAI = false;
        //预制体spine皮肤
        public string SkinName;
        public string AnimControllerPath;
        public string SkeletonDataAssetPath;
        public string AIname = "DefaultAI";
        public string AIPath;

        //影子设置
        public bool shadowParameterOnOff = false;
        public float ShadowCutoffDistance = 10f;
        public float ShadowMaxScale = 0.5f;

        // 寻路数据
        public int agentTypeID;
        public float Radius = 1;
        public float Height = 2;
        public float StepHeight = 0.75f;
        public float MaxSlope = 45f;
        
        // 跌落选项
        public FallInOption FallInOption
        {
            get => roleData.FallInOption;
            set => roleData.FallInOption = value;
        }

        // 跌落高度
        public float FallInHeight
        {
            get => roleData.FallInHeight;
            set => roleData.FallInHeight = value;
        }
        
        // 能否被反弹
        public bool CanRebound = true;
        
        // 是否开启默认碰撞区域 add by TangJian 2018/12/12 13:30
        public bool damageTargetSwitch = true;

        // 脚本类型 add by TangJian 2018/9/20 22:09
        public RoleControllerType roleControllerType = RoleControllerType.RoleController;
        
        // 是否有转身动画
        public bool WithTrunBackAnim = true;
        
        //是否支持血条
        public bool IsMonsterHP = false;
        
        public RoleData roleData = new RoleData();
    }
    //-------------角色AIAction的开关add by tianjinpeng 2018/06/19 11:20:17
    public class roleAIActionOp
    {
        public roleAIActionOp(string das = "", bool dasd = false)
        {
            id = das;
            chushihua = dasd;
        }
        public string id = "";
        public bool chushihua = false;
    }
    public class Weaknessonoff
    {
        public Weaknessonoff(string das = "", bool dasd = false)
        {
            id = das;
            onoff = dasd;
        }
        public string id = "";
        public bool onoff = false;
    }
    public class RoleEditorWindow : EditorWindow
    {
        [MenuItem("Window/角色编辑器")]
        static void Init()
        {
            RoleEditorWindow window = (RoleEditorWindow)EditorWindow.GetWindow(typeof(RoleEditorWindow));
            
            if (window.ANIMSK)
            {
                Selection.activeObject = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(window.currCreateRoleData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
                window.PreviewEditor = UnityEditor.Editor.CreateEditor(window.Target, typeof(Spine.Unity.Editor.SkeletonPreviewEditor)) as Spine.Unity.Editor.SkeletonPreviewEditor;

            }

            window.Show();
        }
        private string path = "Resources_moved/Scripts/Role/";
        private string roleDataFile = "Resources_moved/Scripts/Role/Role.json";
        private string prefabPath = "Assets/Resources_moved/Prefabs/Role/Monster/";
        Dictionary<string, roleAIActionOp> roleAIActionopen = new Dictionary<string, roleAIActionOp>();
        Dictionary<string, Weaknessonoff> WeaknessListOnOffDic = new Dictionary<string, Weaknessonoff>();
        Dictionary<string, List<WeaknessData>> WeaknessDic = new Dictionary<string, List<WeaknessData>>();
        Dictionary<string, List<RoleAIAction>> roleAIActionDic = new Dictionary<string, List<RoleAIAction>>();
        Dictionary<string, RoleData> roleDataDic = new Dictionary<string, RoleData>();
        Dictionary<string, CreateRoleData> createRoleDataDic = new Dictionary<string, CreateRoleData>();

        List<CreateRoleData> createRoleDataList = new List<CreateRoleData>();
        List<RoleData> roleDataList = new List<RoleData>();
        List<roleAIActionOp> oplist = new List<roleAIActionOp>();
        List<Weaknessonoff> WeaknessonoffList = new List<Weaknessonoff>();
        List<RoleAIAction> currRoleAIActionList = new List<RoleAIAction>();
        List<WeaknessData> currWeaknessDataList = new List<WeaknessData>();
        CreateRoleData currCreateRoleData;
        // RoleData currRoleData;
        roleAIActionOp currRoleDataAIop;
        Weaknessonoff currWeaknessonoff;
        Vector2 listScrollViewPos = Vector2.zero;
        Vector2 editScrollViewPos = Vector2.zero;

        //NavMeshAgent 
        SerializedObject m_NavMeshProjectSettingsObject;
        private SerializedProperty m_Agents;
        private SerializedProperty m_SettingNames;

        public float heightInterval;
        public float radiusInterval;

        Rect windowRect = new Rect(100, 100, 500, 500);
        bool ANIMSK;

        bool listbool;
        Vector2 scrollPos;
        Vector2 scrollPos2;

        private bool Foldout_AIAction;
        
        //受击部位s
        private bool Foldout_HurtPart;
        
        private string SearchContext;
        private List<CreateRoleData> SearchRoleDatas = new  List<CreateRoleData>();

        UnityEngine.Object target;
        Vector2 scrollPosition = Vector2.zero;
        Spine.Unity.Editor.SkeletonPreviewEditor previewEditor;//spine动画窗口

        private string excelPath;

        private string ExcelPath
        {
            get { return PlayerPrefs.GetString("RoleEditorWindowExcelPath", excelPath); }
            set
            {
                PlayerPrefs.SetString("RoleEditorWindowExcelPath", value);
            }
        }

        public UnityEngine.Object Target
        {
            get { return target ?? (target = Selection.activeObject); }
            set { target = value; }
        }
        public Spine.Unity.Editor.SkeletonPreviewEditor PreviewEditor
        {
            get
            {
                return previewEditor ?? (Target is SkeletonDataAsset ?
                           (PreviewEditor = UnityEditor.Editor.CreateEditor(Target, typeof(Spine.Unity.Editor.SkeletonPreviewEditor)) as Spine.Unity.Editor.SkeletonPreviewEditor) : null);
            }

            set { previewEditor = value; }
        }

        void OnSelectionChange()
        {
            Target = Selection.activeObject;
            if (PreviewEditor != null)
                Tools.Destroy(PreviewEditor);

            if (Target is SkeletonDataAsset)
            {
                PreviewEditor = UnityEditor.Editor.CreateEditor(Target, typeof(Spine.Unity.Editor.SkeletonPreviewEditor)) as Spine.Unity.Editor.SkeletonPreviewEditor;
                Debug.Log("target=" + Target);
                base.Repaint();
            }

        }
        
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
                    "读取", loadRole
                },
                {
                    "保存", saveRole
                },
                {
                    "清除是不需要的AIActionlist", clearroleAIActionDic
                },
                {
                    "仅生成选中", () =>
                    {
                        saveonlyrole();
                        GameObject roleGameObject;
                        List<RoleAIAction> lisd = new List<RoleAIAction>();
                        List<WeaknessData> weaknessDatas = new List<WeaknessData>();
                        CreateRoleData item = currCreateRoleData;
                        if (WeaknessDic.ContainsKey(item.Id))
                        {
                            weaknessDatas = WeaknessDic[item.Id];
                        }
                        if (roleAIActionDic.ContainsKey(item.Id))
                        {
                            lisd = roleAIActionDic[item.Id];
                            roleGameObject = PrefabCreator.CreateRole(item, prefabPath, lisd, weaknessDatas);
                        }
                        else
                        {
                            roleGameObject = PrefabCreator.CreateRole(item, prefabPath, null, weaknessDatas);
                        }
                        // Tools.UpdatePrefab(roleGameObject, prefabPath+item.Id+".prefab");
                        DestroyImmediate(roleGameObject);                }
                },
            });
            
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));

            // 设置路径, 以及存取数据 add by TangJian 2017/11/15 16:22:45
            EditorGUILayout.BeginHorizontal();
            
            MyGUIExtend.Instance.Foldout("RoleEditor","路径信息", () =>
            {
                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField(roleDataFile);
                    EditorGUILayout.LabelField(prefabPath);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        ExcelPath = MyGUI.TextFieldWithTitle("表格位置", ExcelPath);
                        if (MyGUI.Button("载入表格"))
                        {
                            LoadExcel();
                        }
                    }
                }
            });
            
            EditorGUILayout.EndHorizontal();
            if (Application.isPlaying)
            {
                if (MyGUI.Button("生成一个角色"))
                {
                    CreateRole(currCreateRoleData.Id);
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                radiusInterval= MyGUI.FloatFieldWithTitle("radius间隔", radiusInterval);
                heightInterval = MyGUI.FloatFieldWithTitle("height间隔", heightInterval);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField(prefabPath);
            // if (MyGUI.Button("制作预制体"))
            // {
            //     saveRole();
            // }
            EditorGUILayout.EndHorizontal();

            // 编辑区域 add by TangJian 2017/11/15 16:28:19
            EditorGUILayout.BeginHorizontal();

            // // 列表框 add by TangJian 2017/11/15 16:27:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 3), GUILayout.ExpandHeight(true));
            SearchContext = GUILayout.TextField(SearchContext);
            listScrollViewPos = EditorGUILayout.BeginScrollView(listScrollViewPos);
            if (oplist.Count < createRoleDataList.Count)
            {
                for (int qwe = 0; qwe < createRoleDataList.Count - oplist.Count; qwe++)
                {    
                    
                    oplist.Add(new roleAIActionOp());
                }
            }
            if (WeaknessonoffList.Count < createRoleDataList.Count)
            {
                for (int ase = 0; ase < createRoleDataList.Count - WeaknessonoffList.Count; ase++)
                {
                    WeaknessonoffList.Add(new Weaknessonoff(createRoleDataList[ase].Id, false));
                }
            }
            
            
            if (!string.IsNullOrEmpty(SearchContext))
            {
                for (int i = 0; i < createRoleDataList.Count; i++)
                {
                    if (Regex.IsMatch(createRoleDataList[i].Id.ToLower(),SearchContext))
                    {
                        ListID(i);
                    }
                    
                }
            }
            else
            {
                for (int i = createRoleDataList.Count - 1; i >= 0; i--)
                {
                    EditorGUILayout.BeginHorizontal();
                    ListID(i);
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            
            

            if (MyGUI.Button("+"))
            {
                createRoleDataList.Add(new CreateRoleData());
                oplist.Add(new roleAIActionOp());
                WeaknessonoffList.Add(new Weaknessonoff());
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            // 编辑框 add by TangJian 2017/11/15 16:28:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width((innerBoxWidth / 3)*2 ), GUILayout.ExpandHeight(true));
            editScrollViewPos = EditorGUILayout.BeginScrollView(editScrollViewPos);
            if (currCreateRoleData != null)
            {
                MyGUI.CreateRoleDataField(currCreateRoleData);
                // 判断agents是否需要新建
                currRoleDataAIop.id = currCreateRoleData.Id;
               
                currRoleDataAIop.chushihua = MyGUI.ToggleWithTitle("是否需要AIAction", currRoleDataAIop.chushihua);
                if (currRoleDataAIop.chushihua)
                {
                    if (roleAIActionDic.ContainsKey(currCreateRoleData.Id))
                    {
                        currRoleAIActionList = roleAIActionDic[currCreateRoleData.Id];
                    }
                    else
                    {
                        roleAIActionDic.Add(currCreateRoleData.Id, new List<RoleAIAction>());
                        currRoleAIActionList = roleAIActionDic[currCreateRoleData.Id];
                    }

                    using (new  EditorGUILayout.HorizontalScope())
                    {
                        Foldout_AIAction = EditorGUILayout.Foldout(Foldout_AIAction, "AIAction");
                        if (GUILayout.Button("+",new GUIStyle("Boldlabel"),GUILayout.Width(20)))
                        {
                            currRoleAIActionList.Add(new RoleAIAction("", "", 0, 0, new Bounds()));
                        }

                        if (GUILayout.Button("-",new GUIStyle("Boldlabel"),GUILayout.Width(20)))
                        {
                            if (currRoleAIActionList.Count != 0)
                            {
                                currRoleAIActionList.RemoveAt(currRoleAIActionList.Count - 1); 
                            }
                        }
    
                    }
                    
                    if (Foldout_AIAction) 
                    {
                            //-------------------------角色AIActionList加入AIAction---------------------------add by tianjinpeng 2018/06/19 11:32:16
                        EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width((innerBoxWidth / 3) * 2 - 35), GUILayout.ExpandHeight(true));
                        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width((innerBoxWidth / 3) * 2 - 37.5f), GUILayout.Height(500));
               
                        for (int ai = currRoleAIActionList.Count - 1; ai >= 0; ai--)
                        {
                            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width((innerBoxWidth / 3) * 2 - 40), GUILayout.ExpandHeight(true));
                            currRoleAIActionList[ai] = MyGUI.RoleAIActionField(currRoleAIActionList[ai]);
                            EditorGUILayout.EndVertical();
                        }    
                        EditorGUILayout.EndScrollView();
                        EditorGUILayout.EndVertical();
                    }



                    using (new  EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            MyGUIExtend.Instance.Foldout("RoleEditor","受击部位",(() =>
                            {
                                MyGUI.HurtModeListField(currCreateRoleData.HurtModes);
                            }));

                            if (GUILayout.Button("+",new GUIStyle("Boldlabel"),GUILayout.Width(20)))
                            {
                                currCreateRoleData.HurtModes.Add(new HurtMode());
                            }
                        }
                        ErrorInfo();
                    }
                    
                    
                    
                    
                    ANIMSK = MyGUI.ToggleWithTitle("显示动画", ANIMSK);
                    if (ANIMSK)
                    {
                        panduandanqian();

                        foreach (var item in currRoleAIActionList)
                        {
                            //AIAction画线
                            PreviewEditor.AddDrawExtraGuideLineAction(item.name, (Camera camera) =>
                            {
                                if (item.Color == Color.white)
                                    item.Color = new Color(Random.Range((float) 0.5, (float) 1),
                                        Random.Range((float) 0.5, (float) 1), Random.Range((float) 0.5, (float) 1));
                                
                                Handles.color = item.Color;
                                Handles.DrawWireCube(item.bounds.center, item.bounds.size);
                            });
                        }
                        //角色结构大小画线
                        PreviewEditor.AddDrawExtraGuideLineAction(currCreateRoleData.Id, (Camera camera) =>
                        {
                            Handles.color = Color.green;
                            Handles.DrawWireCube(new Vector3(0, currCreateRoleData.size.y / 2, 0), currCreateRoleData.size);
                            Handles.DrawWireCube(currCreateRoleData.DefendBoundcenter, currCreateRoleData.DefendBoundsize);
                        });
                        if (Selection.activeObject != null)
                        {
                            SkeletonAnimOnGUI(boxStyle, innerBoxWidth);
                        }

                    }

                    // }
                    // if (PreviewEditor == null)
                    // {
                    //     return;
                    // }
                    // SkeletonAnimOnGUI(boxStyle,innerBoxWidth);


                }
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            
            
            
            
        }

        private void ListID(int i )
        {
            int index = MyGUIExtend.Instance.ListSingleButton("RoleEditor",createRoleDataList[i].Id,i,action: (() =>
            {
                currCreateRoleData = createRoleDataList[i];
                currRoleDataAIop = oplist[i];
                currWeaknessonoff = WeaknessonoffList[i];
                if (ANIMSK)
                {
                    Selection.activeObject = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(currCreateRoleData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
                }
            }));
                
            MyGUIExtend.Instance.Mouse_RightDrop(new Dictionary<string, Action>
            {
                {
                    "删除", () =>
                    {
                        createRoleDataList.RemoveAt(index);
                        oplist.RemoveAt(index);
                    }
                },
                {
                    "复制", () =>
                    {
                        var createRoleData = Tools.Json2Obj<CreateRoleData>(Tools.Obj2Json(createRoleDataList[index], true));
                        createRoleDataList.Add(createRoleData);
                        var roleai = Tools.Json2Obj<roleAIActionOp>(Tools.Obj2Json(oplist[index], true));
                        oplist.Add(oplist[index]);
                    }
                }
            });
        }

        private void ErrorInfo()
        {
            for (int i = 0; i < currCreateRoleData.HurtModes.Count; i++)
            {
                var item = currCreateRoleData.HurtModes[i];
                for (int k = 0; k < item.HurtPartList.Count; k++)
                {
                    for (int L = 0; L < item.HurtPartList.Count; L++)
                    {
                        if (k != L && item.HurtPartList[k].Name == item.HurtPartList[L].Name)
                        {
                            EditorGUILayout.HelpBox(item.Name +"中"+k+"  "+L + "  Name相等", MessageType.Error);
                            goto LoopEnd;
                        }   
                    }    
                }
                            
                for (int j = 0; j < currCreateRoleData.HurtModes.Count; j++)
                {
                    if (i != j && currCreateRoleData.HurtModes[i].Name == currCreateRoleData.HurtModes[j].Name)
                    {
                        EditorGUILayout.HelpBox("hurtMode_" + i + " 与 hurtMode_" + j + "  Name相等", MessageType.Error);
                        goto LoopEnd;
                    }
                }
            }
            EditorGUILayout.HelpBox("", MessageType.None);
            LoopEnd:
            var a = 1;
        }
        
        void OnEnable()
        {
            title = "角色编辑器";

            loadRole();
        }

        void LoadExcel()
        {
            EditorTools.AnalyzeExcelToDic(ExcelPath, objects =>
            {
                string id = Convert.ToString(objects["id"]);

                var createRoleData = createRoleDataList.Find(item =>
                {
                    return item.Id == id;
                });
                
                if (createRoleData == null)
                {
                    Debug.LogWarning("找不到角色: " + id);
                    return;
                }
                
                foreach (var pair in objects)
                {
                    switch (pair.Key)
                    {
                        case "id":
                            break;
                        case "name":
                            createRoleData.name = Convert.ToString(pair.Value);
                            break;
                        case "atkPropertyType":
                            createRoleData.atkPropertyType = (AtkPropertyType)Convert.ToInt32(pair.Value);
                            break;
                        case "hpMax":
                            createRoleData.HpMax = Convert.ToSingle(pair.Value);
                            createRoleData.Hp = createRoleData.HpMax;
                            break;
                        case "mpMax":
                            createRoleData.MpMax = Convert.ToSingle(pair.Value);
                            createRoleData.Mp = createRoleData.MpMax;
                            break;
                        case "atk":
                            createRoleData.Atk = Convert.ToSingle(pair.Value);
                            break;
                        case "def":
                            createRoleData.Def = Convert.ToSingle(pair.Value);
                            break;
                        case "atkSpeed":
                            createRoleData.AtkSpeedScale = Convert.ToSingle(pair.Value);
                            break;
                        case "criticalRate":
                            createRoleData.CriticalRate = Convert.ToSingle(pair.Value);
                            break;
                        case "criticalDamage":
                            createRoleData.CriticalDamage = Convert.ToSingle(pair.Value);
                            break;
                        case "walkSpeed":
                            createRoleData.WalkSpeed = Convert.ToSingle(pair.Value);
                            break;
                        case "runSpeed":
                            createRoleData.RunSpeed = Convert.ToSingle(pair.Value);
                            break;
                        case "jumpSpeed":
                            createRoleData.JumpSpeed = Convert.ToSingle(pair.Value);
                            break;
                        case "atkSpeedScale":
                            createRoleData.AtkSpeedScale = Convert.ToSingle(pair.Value);
                            break;
                        case "moveSpeedScale":
//                            createRoleData. = Convert.ToSingle(pair.Value);
                            break;
                        case "RepellingResistance":
                            createRoleData.RepellingResistance = Convert.ToSingle(pair.Value);
                            break;
                        case "mass":
                            break;
                        case "jumpTimes":
                            break;
                        default:
                            Debug.LogWarning("未实现参数:" + pair.Key);
                            break;
                    }
                }
            });
        }

        void loadRole()
        {
            //-------------- 读取预制体Data -----------------add by tianjinpeng 2018/06/19 11:27:58
            string createRoledataString = Tools.ReadStringFromFile(Application.dataPath + "/" + path + "CurrRoleData.json");
            createRoleDataDic = Tools.Json2Obj<Dictionary<string, CreateRoleData>>(createRoledataString);
            createRoleDataList = createRoleDataDic.Values.ToList();
            currCreateRoleData = createRoleDataList[0];

            //-------------- 读取RoleData -------------------add by tianjinpeng 2018/06/19 11:28:35
            string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + roleDataFile);
            roleDataDic = Tools.Json2Obj<Dictionary<string, RoleData>>(jsonString);

            createRoleDataList = createRoleDataDic.Values.ToList();
            currCreateRoleData = createRoleDataList[0];

            //--------------- 读取角色是否需要AIAction -------------------add by tianjinpeng 2018/06/19 11:29:37
            string roleAIActionopenstring = Tools.ReadStringFromFile(Application.dataPath + "/" + path + "roleAIActionopen.json");
            roleAIActionopen = Tools.Json2Obj<Dictionary<string, roleAIActionOp>>(roleAIActionopenstring);

            oplist = roleAIActionopen.Values.ToList();
            currRoleDataAIop = oplist[0];
            //-----------------读取角色是否需要弱点--------------------------add by tianjinpeng 2018/11/14 09:32:38s
            string WeaknessListOnOffDicstring = Tools.ReadStringFromFile(Application.dataPath + "/" + path + "WeaknessListOnOffDic.json");
            WeaknessListOnOffDic = Tools.Json2Obj<Dictionary<string, Weaknessonoff>>(WeaknessListOnOffDicstring);
            WeaknessonoffList = WeaknessListOnOffDic.Values.ToList();
            currWeaknessonoff = WeaknessonoffList[0];
            //-------------------读取角色弱点列表-------------------add by tianjinpeng 2018/11/14 11:31:31
            string WeaknessDicstring = Tools.ReadStringFromFile(Application.dataPath + "/" + path + "WeaknessDic.json");
            WeaknessDic = Tools.Json2Obj<Dictionary<string, List<WeaknessData>>>(WeaknessDicstring);
            //-------------- 读取角色roleAIActionList ------------------------add by tianjinpeng 2018/06/19 11:37:40
            string roleAIActionstring = Tools.ReadStringFromFile(Application.dataPath + "/" + path + "roleAIAction.json");
            roleAIActionDic = Tools.Json2Obj<Dictionary<string, List<RoleAIAction>>>(roleAIActionstring);
        }
        void saveonlyrole()
        {
            //-------------- 保存预制体Data -----------------add by tianjinpeng 2018/06/19 11:27:58
            createRoleDataDic = createRoleDataList.ToDictionary(item => item.Id, item => item);
            string createRoledataString = Tools.Obj2Json(createRoleDataDic, true);
            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "CurrRoleData.json", createRoledataString);

            //-------------- 保存RoleData -------------------add by tianjinpeng 2018/06/19 11:28:35
            ForDicCreateRoleData();
            roleDataDic = roleDataList.ToDictionary(item => item.Id, item => item);
            string jsonString = Tools.Obj2Json(roleDataDic, true);
            Debug.Log("jsonString = " + jsonString);
            Tools.WriteStringFromFile(Application.dataPath + "/" + roleDataFile, jsonString);

            //--------------- 保存角色是否需要AIAction -------------------add by tianjinpeng 2018/06/19 11:29:37
            roleAIActionopen = oplist.ToDictionary(item => item.id);
            string roleAIActionopenstring = Tools.Obj2Json(roleAIActionopen, true);
            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "roleAIActionopen.json", roleAIActionopenstring);
            //----------------保存角色是否需要弱点--------------------add by tianjinpeng 2018/11/13 18:25:17
//            WeaknessListOnOffDic = WeaknessonoffList.ToDictionary(item => item.id, item => item);
//            string WeaknessListOnOffDicstring = Tools.Obj2Json(WeaknessListOnOffDic, true);
//            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "WeaknessListOnOffDic.json", WeaknessListOnOffDicstring);
//            //----------------保存角色弱点列表----------------add by tianjinpeng 2018/11/14 11:20:15
//            clearweaknessDic();
//            string WeaknessDicstring = Tools.Obj2Json(WeaknessDic, true);
//            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "WeaknessDic.json", WeaknessDicstring);
            //-------------- 保存角色roleAIActionList ------------------------add by tianjinpeng 2018/06/19 11:37:40
            clearroleAIActionDic();
            string roleAIActionstring = Tools.Obj2Json(roleAIActionDic, true);
            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "roleAIAction.json", roleAIActionstring);

        }
        void saveRole()
        {
            // InitProjectSettings();
            // InitAgents();
            wendensslist(createRoleDataList);
            // // -------------保存NavAgent--------------------
            //NewAgents(createRoleDataList);
            if (heightInterval != 0 && radiusInterval != 0)
            {
                initAgentList(createRoleDataList);
            }
            

            //-------------- 保存预制体Data -----------------add by tianjinpeng 2018/06/19 11:27:58
            createRoleDataDic = createRoleDataList.ToDictionary(item => item.Id, item => item);
            string createRoledataString = Tools.Obj2Json(createRoleDataDic, true);
            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "CurrRoleData.json", createRoledataString);

            //-------------- 保存RoleData -------------------add by tianjinpeng 2018/06/19 11:28:35
            ForDicCreateRoleData();
            roleDataDic = roleDataList.ToDictionary(item => item.Id, item => item);
            string jsonString = Tools.Obj2Json(roleDataDic, true);
            Debug.Log("jsonString = " + jsonString);
            Tools.WriteStringFromFile(Application.dataPath + "/" + roleDataFile, jsonString);

            //--------------- 保存角色是否需要AIAction -------------------add by tianjinpeng 2018/06/19 11:29:37
            roleAIActionopen = oplist.ToDictionary(item => item.id);
            string roleAIActionopenstring = Tools.Obj2Json(roleAIActionopen, true);
            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "roleAIActionopen.json", roleAIActionopenstring);
            //----------------保存角色是否需要弱点--------------------add by tianjinpeng 2018/11/13 18:25:17
//            WeaknessListOnOffDic = WeaknessonoffList.ToDictionary(item => item.id, item => item);
//            string WeaknessListOnOffDicstring = Tools.Obj2Json(WeaknessListOnOffDic, true);
//            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "WeaknessListOnOffDic.json", WeaknessListOnOffDicstring);
            //----------------保存角色弱点列表----------------add by tianjinpeng 2018/11/14 11:20:15
//            clearweaknessDic();
//            string WeaknessDicstring = Tools.Obj2Json(WeaknessDic, true);
//            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "WeaknessDic.json", WeaknessDicstring);
            //-------------- 保存角色roleAIActionList ------------------------add by tianjinpeng 2018/06/19 11:37:40
            clearroleAIActionDic();
            string roleAIActionstring = Tools.Obj2Json(roleAIActionDic, true);
            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "roleAIAction.json", roleAIActionstring);
            //-----------------生成预制体----------------------add by tianjinpeng 2018/06/19 11:23:39
            foreach (var item in createRoleDataList)
            {
                GameObject roleGameObject;
                List<RoleAIAction> lisd = new List<RoleAIAction>();
                List<WeaknessData> weaknessDatas = new List<WeaknessData>();
                if (WeaknessDic.ContainsKey(item.Id))
                {
                    weaknessDatas = WeaknessDic[item.Id];
                }
                if (roleAIActionDic.ContainsKey(item.Id))
                {
                    lisd = roleAIActionDic[item.Id];
                    roleGameObject = PrefabCreator.CreateRole(item, prefabPath, lisd, weaknessDatas);
                }
                else
                {
                    roleGameObject = PrefabCreator.CreateRole(item, prefabPath, null, weaknessDatas);
                }
                // Tools.UpdatePrefab(roleGameObject, prefabPath+item.Id+".prefab");
                DestroyImmediate(roleGameObject);
            }

        }
        //-----------清除不需要的AIActionList------------------add by tianjinpeng 2018/06/19 11:24:42
        void clearroleAIActionDic()
        {
            if (roleAIActionDic != null && roleAIActionDic.Count != 0)
            {
                foreach (var item in roleAIActionopen)
                {
                    if (item.Value.chushihua) { }
                    else
                    {
                        roleAIActionDic.Remove(item.Key);
                    }
                }
            }
        }
        //-------------清除不需要的弱点列表------------------------add by tianjinpeng 2018/11/14 11:21:36
        void clearweaknessDic()
        {
            if (WeaknessDic != null && WeaknessDic.Count != 0)
            {
                foreach (var item in WeaknessListOnOffDic)
                {
                    if (item.Value.onoff) { }
                    else
                    {
                        WeaknessDic.Remove(item.Key);
                    }
                }
            }
        }
        //清理roleDataList并导入createRoleDataList中的roleData  add by tianjinpeng 2018/06/19 11:26:13
        void ForDicCreateRoleData()
        {
            roleDataList.Clear();
            foreach (var item in createRoleDataList)
            {
                roleDataList.Add(item.roleData);
            }
        }

        //spine动画窗口
        void SkeletonAnimOnGUI(GUIStyle boxStyle, System.Single innerBoxWidth)
        {
            EditorGUILayout.BeginHorizontal();
            if (Target is SkeletonDataAsset && Target == Selection.activeObject)
            {
                EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(((innerBoxWidth / 3) * 2 - 32) * 0.8f), GUILayout.Height(300));

                if (PreviewEditor.HasPreviewGUI())
                {
                    Rect r = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    // previewEditor.OnPreviewGUI(r, GUIStyle.none);                    
                    // previewEditor.OnInteractivePreviewGUI (EditorGUILayout.GetControlRect (), GUIStyle.none);
                    // GUI.backgroundColor = m_originColor;
                    GUIStyle style = new GUIStyle("PreBackground");
                    PreviewEditor.OnInteractivePreviewGUI(r, style);



                    // Color oldColor = Handles.color;
                    // Handles.color = Color.red;
                    // Handles.DrawWireCube(new Vector3(0,0,0), new Vector3(1,1,1));
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(((innerBoxWidth / 3) * 2 - 32) * 0.2f), GUILayout.ExpandHeight(true));

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                // previewEditor.OnInspectorGUI ();
                EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
                PreviewEditor.DrawAnimationList();
                EditorGUILayout.Space();
                PreviewEditor.DrawSlotList();
                EditorGUILayout.Space();
                PreviewEditor.DrawUnityTools();
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();


            }
            EditorGUILayout.EndHorizontal();
        }
        //判断选中的资源是否是当前角色的spine动画资源
        void panduandanqian()
        {
            if (Selection.activeObject == (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(currCreateRoleData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset)))
            {

            }
            else
            {
                Selection.activeObject = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(currCreateRoleData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
            }

        }
        private void InitProjectSettings()
        {
            UnityEngine.Object obj = Unsupported.GetSerializedAssetInterfaceSingleton("NavMeshProjectSettings");
            m_NavMeshProjectSettingsObject = new SerializedObject(obj);
        }
        private void InitAgents()
        {
            m_Agents = m_NavMeshProjectSettingsObject.FindProperty("m_Settings");
            m_SettingNames = m_NavMeshProjectSettingsObject.FindProperty("m_SettingNames");
        }

        public void newAgent(CreateRoleData createRoleData)
        {
            // InitProjectSettings();
            // InitAgents();
            bool onoff = true;
            int ind = -1;

            for (int i = 0; i < UnityEngine.AI.NavMesh.GetSettingsCount(); i++)
            {
                SerializedProperty nameProp = m_SettingNames.GetArrayElementAtIndex(i);
                if (nameProp != null)
                {
                    if (nameProp.stringValue == createRoleData.Id)
                    {
                        onoff = false;
                        ind = i;
                        break;
                    }
                }

            }


            if (onoff)
            {
                UnityEngine.AI.NavMeshBuildSettings navMeshBuildSettings = UnityEngine.AI.NavMesh.CreateSettings();
                // navMeshBuildSettings.agentHeight=createRoleData.Height;
                // navMeshBuildSettings.agentRadius=createRoleData.Radius;
                // navMeshBuildSettings.agentClimb=createRoleData.StepHeight;
                // navMeshBuildSettings.agentSlope=createRoleData.MaxSlope;

                InitProjectSettings();
                InitAgents();

                SerializedProperty nameProp = m_SettingNames.GetArrayElementAtIndex(UnityEngine.AI.NavMesh.GetSettingsCount() - 1);
                nameProp.stringValue = createRoleData.Id;
                SerializedProperty selectedAgent = m_Agents.GetArrayElementAtIndex(UnityEngine.AI.NavMesh.GetSettingsCount() - 1);
                SerializedProperty radiusProp = selectedAgent.FindPropertyRelative("agentRadius");
                SerializedProperty heightProp = selectedAgent.FindPropertyRelative("agentHeight");
                SerializedProperty stepHeightProp = selectedAgent.FindPropertyRelative("agentClimb");
                SerializedProperty maxSlopeProp = selectedAgent.FindPropertyRelative("agentSlope");
                radiusProp.floatValue = createRoleData.Radius;
                heightProp.floatValue = createRoleData.Height;
                stepHeightProp.floatValue = createRoleData.StepHeight;
                maxSlopeProp.floatValue = createRoleData.MaxSlope;

                m_NavMeshProjectSettingsObject.ApplyModifiedProperties();
                navMeshBuildSettings = UnityEngine.AI.NavMesh.GetSettingsByIndex(UnityEngine.AI.NavMesh.GetSettingsCount() - 1);
                createRoleData.agentTypeID = navMeshBuildSettings.agentTypeID;
            }
            else
            {
                SerializedProperty selectedAgent = m_Agents.GetArrayElementAtIndex(ind);
                SerializedProperty radiusProp = selectedAgent.FindPropertyRelative("agentRadius");
                SerializedProperty heightProp = selectedAgent.FindPropertyRelative("agentHeight");
                SerializedProperty stepHeightProp = selectedAgent.FindPropertyRelative("agentClimb");
                SerializedProperty maxSlopeProp = selectedAgent.FindPropertyRelative("agentSlope");

                if (heightProp.floatValue == createRoleData.Height &&
                    radiusProp.floatValue == createRoleData.Radius &&
                    stepHeightProp.floatValue == createRoleData.StepHeight &&
                    maxSlopeProp.floatValue == createRoleData.MaxSlope)
                {

                }
                else
                {
                    radiusProp.floatValue = createRoleData.Radius;
                    heightProp.floatValue = createRoleData.Height;
                    stepHeightProp.floatValue = createRoleData.StepHeight;
                    maxSlopeProp.floatValue = createRoleData.MaxSlope;
                    m_NavMeshProjectSettingsObject.ApplyModifiedProperties();
                }
            }

        }
        public void wendensslist(List<CreateRoleData> createRoleDataList)
        {
            clearweaknessDic();
            foreach (var item in createRoleDataList)
            {
                if (WeaknessDic.ContainsKey(item.Id))
                {
                    item.WeaknessDataList = WeaknessDic[item.Id];
                }
            }
        }
        public void NewAgents(List<CreateRoleData> createRoleDataList)
        {
            UnityEngine.Object obj = Unsupported.GetSerializedAssetInterfaceSingleton("NavMeshProjectSettings");
            m_NavMeshProjectSettingsObject = new SerializedObject(obj);

            m_Agents = m_NavMeshProjectSettingsObject.FindProperty("m_Settings");
            m_SettingNames = m_NavMeshProjectSettingsObject.FindProperty("m_SettingNames");

            while (m_Agents.arraySize > 0)
            {
                m_Agents.DeleteArrayElementAtIndex(0);
            }

            while (m_SettingNames.arraySize > 1)
            {
                m_SettingNames.DeleteArrayElementAtIndex(m_SettingNames.arraySize - 1);
            }

            m_NavMeshProjectSettingsObject.ApplyModifiedProperties();


            //
            for (int i = 0; i < createRoleDataList.Count; i++)
            {
                UnityEngine.AI.NavMeshBuildSettings navMeshBuildSettings = UnityEngine.AI.NavMesh.CreateSettings();
            }


            m_NavMeshProjectSettingsObject.Update();




            for (int i = 0; i < createRoleDataList.Count; i++)
            {
                var createRoleData = createRoleDataList[i];

                SerializedProperty nameProp = m_SettingNames.GetArrayElementAtIndex(i + 1);
                nameProp.stringValue = createRoleData.Id;
                SerializedProperty selectedAgent = m_Agents.GetArrayElementAtIndex(i + 1);
                SerializedProperty radiusProp = selectedAgent.FindPropertyRelative("agentRadius");
                SerializedProperty heightProp = selectedAgent.FindPropertyRelative("agentHeight");
                SerializedProperty stepHeightProp = selectedAgent.FindPropertyRelative("agentClimb");
                SerializedProperty maxSlopeProp = selectedAgent.FindPropertyRelative("agentSlope");
                radiusProp.floatValue = createRoleData.size.x / 2f;
                heightProp.floatValue = createRoleData.size.y;
                stepHeightProp.floatValue = createRoleData.StepHeight;
                maxSlopeProp.floatValue = createRoleData.MaxSlope;

                m_NavMeshProjectSettingsObject.ApplyModifiedProperties();
                var navMeshBuildSettings = UnityEngine.AI.NavMesh.GetSettingsByIndex(i + 1);
                createRoleData.agentTypeID = navMeshBuildSettings.agentTypeID;
            }

            m_NavMeshProjectSettingsObject.ApplyModifiedProperties();



            // //

            // m_NavMeshProjectSettingsObject.ApplyModifiedProperties();
        }
        List<agentsdata> agentsdatas = new List<agentsdata>();
        List<float> radiuslist = new List<float>();
        List<float> heightlist = new List<float>();

        void initAgentList(List<CreateRoleData> createRoleDataList)
        {
            float heightMax=0;
            float radiusMax = 0;
            List<agentsdata> agentsdatasdix = new List<agentsdata>();
            radiuslist.Clear();
            heightlist.Clear();
            agentsdatas.Clear();
            foreach(var item in createRoleDataList)
            {
                if((item.size.x / 2f) > radiusMax)
                {
                    radiusMax = item.size.x / 2f;
                }
                if(item.size.y> heightMax)
                {
                    heightMax = item.size.y;
                }
                agentsdatasdix.Add(new agentsdata(item.size.x / 2f, item.size.y,item.Id));
            }
            if (heightInterval != 0&& radiusInterval!=0)
            {
                int heightcount = Mathf.CeilToInt(heightMax / heightInterval);
                int radiuscount = Mathf.CeilToInt(radiusMax / radiusInterval);
                for (int i=0;i< heightcount; i++)
                {
                    heightlist.Add(heightInterval * (i + 1));
                }
                for(int e=0;e< radiuscount; e++)
                {
                    radiuslist.Add(radiusInterval * (e + 1));
                }
                NewAgentList(agentsdatasdix);
                createAgents();
            }
            
        }
        void NewAgentList(List<agentsdata> agentsdatasdix)
        {
            foreach (var item in agentsdatasdix)
            {
                float height=0;
                float radius=0;
                for(int h = 0; h < heightlist.Count; h++)
                {
                    if(item.height <= heightlist[h])
                    {
                        height = heightlist[h];
                        break;
                    }
                }
                for(int r=0;r< radiuslist.Count; r++)
                {
                    if(item.radius<= radiuslist[r])
                    {
                        radius = radiuslist[r];
                        break;
                    }
                }
                string NewName = "height-" + height.ToString() + "radius-" + radius.ToString();
                agentsdata sa = new agentsdata(radius, height, NewName);
                if (agentsdatas.Contains(sa))
                {
                    agentsdata  sdaa= agentsdatas.Find((agentsdata oth) => 
                    {
                        if (oth.name == NewName)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                    if (sdaa != null)
                    {
                        agentsdatas[agentsdatas.IndexOf(sdaa)].childrenName.Add(item.name);
                    }
                }
                else
                {
                    sa.childrenName.Add(item.name);
                    agentsdatas.Add(sa);
                }
                //agentsdatas.
            }
        }
        void createAgents()
        {
            InitProjectSettings();
            InitAgents();
            foreach (var item in agentsdatas)
            {
                bool onoff = true;
                int ind = -1;
                UnityEngine.AI.NavMeshBuildSettings navMeshBuildSettings;
                for (int i = 0; i < UnityEngine.AI.NavMesh.GetSettingsCount(); i++)
                {
                    SerializedProperty nameProp = m_SettingNames.GetArrayElementAtIndex(i);
                    if (nameProp != null)
                    {
                        if (nameProp.stringValue == item.name)
                        {
                            onoff = false;
                            ind = i;
                            break;
                        }
                    }
                }
                if (onoff)
                {

                    navMeshBuildSettings = UnityEngine.AI.NavMesh.CreateSettings();

                    InitProjectSettings();
                    InitAgents();

                    SerializedProperty nameProp = m_SettingNames.GetArrayElementAtIndex(UnityEngine.AI.NavMesh.GetSettingsCount() - 1);
                    nameProp.stringValue =item.name;
                    SerializedProperty selectedAgent = m_Agents.GetArrayElementAtIndex(UnityEngine.AI.NavMesh.GetSettingsCount() - 1);
                    SerializedProperty radiusProp = selectedAgent.FindPropertyRelative("agentRadius");
                    SerializedProperty heightProp = selectedAgent.FindPropertyRelative("agentHeight");
                    SerializedProperty stepHeightProp = selectedAgent.FindPropertyRelative("agentClimb");
                    SerializedProperty maxSlopeProp = selectedAgent.FindPropertyRelative("agentSlope");
                    radiusProp.floatValue = item.radius;
                    heightProp.floatValue = item.height;
                    stepHeightProp.floatValue = 1.1f;
                    maxSlopeProp.floatValue = 45f;

                    m_NavMeshProjectSettingsObject.ApplyModifiedProperties();

                    navMeshBuildSettings = UnityEngine.AI.NavMesh.GetSettingsByIndex(UnityEngine.AI.NavMesh.GetSettingsCount() - 1);

                }
                else
                {
                    navMeshBuildSettings = UnityEngine.AI.NavMesh.GetSettingsByIndex(ind);
                }
                for(int i = 0; i < item.childrenName.Count; i++)
                {
                    CreateRoleData x= createRoleDataList.Find((CreateRoleData other) =>
                    {
                        if (item.childrenName[i] == other.name)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                    if (x != null)
                    {
                        createRoleDataList[createRoleDataList.IndexOf(x)].agentTypeID = navMeshBuildSettings.agentTypeID;
                    }
                }
            }
        }
        
        // 在玩家边上刷怪 add by TangJian 2019/3/22 20:37
        async void CreateRole(string roleId, bool withAI = false, string teamId = "2")
        {
            var player = GameManager.Instance.Player1;
            Debug.Assert(player != null);

            var target = await AssetManager.InstantiateRole(roleId);
            Debug.Assert(target != null);

            RoleController roleController = target.GetComponent<RoleController>();
            Debug.Assert(roleController != null);

            if (withAI)
            {
                Tools.AddComponent<RoleBehaviorTree>(target);
                Tools.AddComponent<RoleNavMeshAgent>(target);
            }

            // 设置角色队伍 add by TangJian 2017/12/20 22:02:00
            roleController.RoleData.TeamId = teamId;

            SceneManager.Instance.ObjectEnterSceneWithWorldPosition(roleController, player.SceneId, player.transform.position + new Vector3(0, 3, 0));
        }
        
        void Update()
        {
            if (mouseOverWindow) // 如果鼠标在窗口上, 则重绘界面 add by TangJian 2017/09/28 21:57:44
            {
                base.Repaint();
            }
        }

    }
}