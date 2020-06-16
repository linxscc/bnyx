using System.Collections.Generic;
using System.Threading.Tasks;
using Spine.Unity;
using UnityEditor;
using UnityEngine;
using Tang.FrameEvent;
using ZS;


namespace Tang.Editor
{
    public class MyGUI
    {
        public static bool FoldoutWithTitle(string title, bool foldoutBool)
        {
            return EditorGUILayout.Foldout(foldoutBool, title);
        }

        public static bool ToggleWithTitle(string title, bool toggleBool)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title + ":");
            toggleBool = EditorGUILayout.Toggle(toggleBool);
            EditorGUILayout.EndHorizontal();
            return toggleBool;
        }

        public static bool Button(string text, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(text, options))
            {
                GUIUtility.keyboardControl = 0;
                return true;
            }
            return false;
        }
        
        public static bool Button(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(text, style, options))
            {
                GUIUtility.keyboardControl = 0;
                return true;
            }
            return false;
        }

        public static string TextFieldWithTitle(string title, string value, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title + ":");
            value = EditorGUILayout.TextField(value, options);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        public static int IntFieldWithTitle(string title, int value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title + ":");
            value = EditorGUILayout.IntField(value);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        public static float FloatFieldWithTitle(string title, float value, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title + ":");
            value = EditorGUILayout.FloatField(value, options);
            EditorGUILayout.EndHorizontal();
            return value;
        }
        public static Vector3 Vector3WithTitle(string title, Vector3 value, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField(title + ":");
            value = EditorGUILayout.Vector3Field(title, value, options);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        public static Vector2 Vector2WithTitle(string title, Vector2 value, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField(title + ":");
            value = EditorGUILayout.Vector2Field(title, value, options);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        public static Vector4 Vector4WithTitle(string title, Vector4 value, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField(title + ":");
            value = EditorGUILayout.Vector4Field(title, value, options);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        public static Vector4 ColorFieldWithTitle(string title, Vector4 value, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField(title + ":");
            value = EditorGUILayout.ColorField(title, value, options);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        public static int PopupWithTitle(string title, int selectedIndex, string[] displayedOptions)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title + ":");
            selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions);
            EditorGUILayout.EndHorizontal();
            return selectedIndex;
        }

        public static System.Enum EnumPopupWithTitle(string title, System.Enum value, params GUILayoutOption[] potions)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title + ":");
            value = EditorGUILayout.EnumPopup(value, potions);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        public static System.Enum EnumPopup(System.Enum value, params GUILayoutOption[] potions)
        {
            value = EditorGUILayout.EnumPopup(value, potions);
            return value;
        }

        public enum AttrDataFiledType
        {
            Default = 0,
            Role = 1,
            Item = 2
        }

        public static AttrData SimplifyAttrDataField(AttrData attrData, AttrDataFiledType attrDataFiledType = AttrDataFiledType.Default)
        {
            attrData.hpMax = MyGUI.FloatFieldWithTitle("hpMax(血量最大值)", attrData.hpMax);
            if (attrDataFiledType == AttrDataFiledType.Role)
                attrData.hp = attrData.hpMax;
            attrData.vigorMax = MyGUI.FloatFieldWithTitle("vigorMax(体力最大值)", attrData.vigorMax);
            if (attrDataFiledType == AttrDataFiledType.Role)
                attrData.vigor = attrData.vigorMax;
            
            attrData.atk = MyGUI.FloatFieldWithTitle("atk(攻击力)", attrData.atk);

            // 伤害范围 怪物用
            attrData.atkMin = MyGUI.FloatFieldWithTitle("atkMin(攻击最小值)", attrData.atkMin);
            attrData.atkMax = MyGUI.FloatFieldWithTitle("atkMax(攻击最大值)", attrData.atkMax);
            
            // 物理减伤百分比
            attrData.physicalDef = MyGUI.FloatFieldWithTitle("physicalDef(物理减伤百分百)", attrData.physicalDef);

            // 魔法减伤百分比
            attrData.magicalDef = MyGUI.FloatFieldWithTitle("magicalDef(魔法减伤百分比)", attrData.magicalDef);
            attrData.def = MyGUI.FloatFieldWithTitle("def(防御力)", attrData.def);
            
            attrData.atkSpeedScale = MyGUI.FloatFieldWithTitle("atkSpeedScale(攻速倍率)", attrData.atkSpeedScale);
            
            attrData.walkSpeed = MyGUI.FloatFieldWithTitle("walkSpeed(走路速度)", attrData.walkSpeed);
            attrData.runSpeed = MyGUI.FloatFieldWithTitle("runSpeed(跑步速度)", attrData.runSpeed);
            attrData.rushSpeed = MyGUI.FloatFieldWithTitle("rushSpeed(冲刺速度)", attrData.rushSpeed);
            attrData.jumpSpeed = MyGUI.FloatFieldWithTitle("jumpSpeed(跳跃速度)", attrData.jumpSpeed);
            //跳跃次数
            attrData.jumpTimes = MyGUI.IntFieldWithTitle("jumpTimes(跳跃次数)", attrData.jumpTimes);
            
            //角色受击抗性
            attrData.RepellingResistance = MyGUI.FloatFieldWithTitle("RepellingResistance(普通抗性)", attrData.RepellingResistance);
            attrData.DefenseRepellingResistance = MyGUI.FloatFieldWithTitle("DefenseRepellingResistance(格挡抗性)", attrData.DefenseRepellingResistance);
            
            attrData.critical = IntFieldWithTitle("criticalRate(暴击率)",attrData.critical);
            attrData.criticalRate = attrData.critical / 100; 
//            attrData.criticalRate = MyGUI.FloatFieldWithTitle("criticalRate(暴击率)", attrData.criticalRate);
            attrData.criticalDamage = MyGUI.FloatFieldWithTitle("criticalDamage(暴击伤害)", attrData.criticalDamage);

            attrData.strength = FloatFieldWithTitle("strength(力量)", attrData.strength);
            attrData.weight = FloatFieldWithTitle("weight(重量)", attrData.weight);
            
            return attrData;
        }
        public static AttrData AttrDataField(AttrData attrData, AttrDataFiledType attrDataFiledType = AttrDataFiledType.Default)
        {
            MyGUIExtend.Instance.lineRetract( (() =>
            {
                MyGUIExtend.Instance.Foldout("AttrData","AttrDataField", () =>
                {
                    attrData.hpMax = MyGUI.FloatFieldWithTitle("hpMax(血量最大值)", attrData.hpMax);
                    if (attrDataFiledType == AttrDataFiledType.Role)
                        attrData.hp = attrData.hpMax;

                    attrData.mpMax = MyGUI.FloatFieldWithTitle("mpMax(魔法最大值)", attrData.mpMax);
                    if (attrDataFiledType == AttrDataFiledType.Role)
                        attrData.mp = attrData.mpMax;

                    attrData.tiliMax = MyGUI.FloatFieldWithTitle("tiliMax(体力最大值)", attrData.tiliMax);
                    if (attrDataFiledType == AttrDataFiledType.Role)
                        attrData.tili = attrData.tiliMax;

                    attrData.atk = MyGUI.FloatFieldWithTitle("atk(攻击力)", attrData.atk);

                    // 伤害范围 怪物用
                    attrData.atkMin = MyGUI.FloatFieldWithTitle("atkMin(攻击最小值)", attrData.atkMin);
                    attrData.atkMax = MyGUI.FloatFieldWithTitle("atkMax(攻击最大值)", attrData.atkMax);

                    // 魔法伤害范围 怪物用
                    attrData.MagicMin = MyGUI.FloatFieldWithTitle("MagicMin(魔法伤害最小值)", attrData.MagicMin);
                    attrData.MagicMax = MyGUI.FloatFieldWithTitle("MagicMax(魔法伤害最大值)", attrData.MagicMax);

                    // 物理减伤百分比
                    attrData.physicalDef = MyGUI.FloatFieldWithTitle("physicalDef(物理减伤百分百)", attrData.physicalDef);

                    // 魔法减伤百分比
                    attrData.magicalDef = MyGUI.FloatFieldWithTitle("magicalDef(魔法减伤百分比)", attrData.magicalDef);
                    attrData.def = MyGUI.FloatFieldWithTitle("def(防御力)", attrData.def);

//            attrData.atkSpeed = MyGUI.FloatFieldWithTitle("atkSpeed(攻速)", attrData.atkSpeed);
//            attrData.moveSpeed = MyGUI.FloatFieldWithTitle("moveSpeed(移速)", attrData.moveSpeed);

                    attrData.criticalRate = MyGUI.FloatFieldWithTitle("criticalRate(暴击率)", attrData.criticalRate);
                    attrData.criticalDamage = MyGUI.FloatFieldWithTitle("criticalDamage(暴击伤害)", attrData.criticalDamage);

                    // 数值倍率 add by TangJian 2018/05/15 12:24:36
                    attrData.atkSpeedScale = MyGUI.FloatFieldWithTitle("atkSpeedScale(攻速倍率)", attrData.atkSpeedScale);
                    attrData.moveSpeedScale = MyGUI.FloatFieldWithTitle("moveSpeedScale(移动速度倍率)", attrData.moveSpeedScale);

                    attrData.walkSpeed = MyGUI.FloatFieldWithTitle("walkSpeed(走路速度)", attrData.walkSpeed);
                    attrData.runSpeed = MyGUI.FloatFieldWithTitle("runSpeed(跑步速度)", attrData.runSpeed);
                    attrData.rushSpeed = MyGUI.FloatFieldWithTitle("rushSpeed(冲刺速度)", attrData.rushSpeed);

                    attrData.jumpSpeed = MyGUI.FloatFieldWithTitle("jumpSpeed(跳跃速度)", attrData.jumpSpeed);

                    //击退抗性与防御击退抗性
                    attrData.RepellingResistance = MyGUI.FloatFieldWithTitle("RepellingResistance(击退抗性)", attrData.RepellingResistance);
                    attrData.DefenseRepellingResistance = MyGUI.FloatFieldWithTitle("DefenseRepellingResistance(防御击退抗性)", attrData.DefenseRepellingResistance);
                    //质量
                    attrData.mass = MyGUI.FloatFieldWithTitle("mass(质量)", attrData.mass);

                    //跳跃次数
                    attrData.jumpTimes = MyGUI.IntFieldWithTitle("jumpTimes(跳跃次数)", attrData.jumpTimes);

                    // 韧性 add by TangJian 2018/12/12 12:13
                    attrData.poiseMax = MyGUI.FloatFieldWithTitle("poiseMax(韧性最大值)", attrData.poiseMax);
                    attrData.poise = MyGUI.FloatFieldWithTitle("poise(当前韧性)", attrData.poise);

                    // 韧性倍率 add by TangJian 2018/12/12 14:46
                    attrData.poiseScale = MyGUI.FloatFieldWithTitle("poiseScale(韧性倍率)", attrData.poiseScale);

                    // 削韧 add by TangJian 2018/12/12 12:14
                    attrData.poiseCut = MyGUI.FloatFieldWithTitle("poiseCut(削韧)", attrData.poiseCut);
            
                    // 霸体数值 add by TangJian 2019/3/23 17:01
                    attrData.superArmor = MyGUI.FloatFieldWithTitle("superArmor(霸体)", attrData.superArmor);
            
                    // 打击效果类型和材质类型
                    attrData.hitEffectType = (HitEffectType)MyGUI.EnumPopupWithTitle("hitEffectType(打击效果类型)", attrData.hitEffectType);
                    attrData.matType = (MatType)MyGUI.EnumPopupWithTitle("matType(材质类型)", attrData.matType);
                });    
            }));
            return attrData;
        }

        public static AttrData AttrDataMinMaxField(AttrData attrData)
        {
            return AttrDataField(attrData, AttrDataFiledType.Item);
            //attrData.atk = MyGUI.FloatFieldWithTitle("atk", attrData.atk);
            //attrData.atkMin = MyGUI.FloatFieldWithTitle("atkMin", attrData.atkMin);
            //attrData.atkMax = MyGUI.FloatFieldWithTitle("atkMax", attrData.atkMax);
            //attrData.MagicMin = MyGUI.FloatFieldWithTitle("MagicMin", attrData.MagicMin);
            //attrData.MagicMax = MyGUI.FloatFieldWithTitle("MagicMax", attrData.MagicMax);
            //attrData.hpMax = MyGUI.FloatFieldWithTitle("hpMax", attrData.hpMax);
            //attrData.mpMax = MyGUI.FloatFieldWithTitle("mpMax", attrData.mpMax);
            //attrData.def = MyGUI.FloatFieldWithTitle("def", attrData.def);
            //attrData.atkSpeed = MyGUI.FloatFieldWithTitle("atkSpeed", attrData.atkSpeed);
            //attrData.moveSpeed = MyGUI.FloatFieldWithTitle("moveSpeed", attrData.moveSpeed);
            //attrData.criticalRate = MyGUI.FloatFieldWithTitle("criticalRate", attrData.criticalRate);
            //attrData.criticalDamage = MyGUI.FloatFieldWithTitle("criticalDamage", attrData.criticalDamage);

            //attrData.tiliMax = MyGUI.FloatFieldWithTitle("tiliMax", attrData.tiliMax);

            //attrData.atkSpeedScale = MyGUI.FloatFieldWithTitle("atkSpeedScale", attrData.atkSpeedScale);
            //attrData.moveSpeedScale = MyGUI.FloatFieldWithTitle("moveSpeedScale", attrData.moveSpeedScale);
            //return attrData;
        }

        private static bool Foldout_RoleData;
        public static CreateRoleData CreateRoleDataField(CreateRoleData createRoleData)
        {
            MyGUIExtend.Instance.Foldout("Role","角色基本信息",(() =>
            {
                createRoleData.Id = MyGUI.TextFieldWithTitle("id", createRoleData.Id);
                createRoleData.roleData.AttrData = SimplifyAttrDataField(createRoleData.roleData.AttrData, AttrDataFiledType.Role);
            }));
            
             Foldout_RoleData = EditorGUILayout.Foldout(Foldout_RoleData, "角色其他信息");
            if (Foldout_RoleData)
            {
                
                // 预制体名 add by TangJian 2018/05/17 15:45:43
                createRoleData.Prefab = MyGUI.TextFieldWithTitle("prefab", createRoleData.Prefab);

                createRoleData.CollideWithRole = MyGUI.ToggleWithTitle("与其他角色碰撞", createRoleData.CollideWithRole);
                createRoleData.NeedTurnBack = MyGUI.ToggleWithTitle("需要受击转面", createRoleData.NeedTurnBack);
                createRoleData.roleControllerType = (RoleControllerType)MyGUI.EnumPopupWithTitle("角色控制器类型", createRoleData.roleControllerType);
                
                // 跌落选项
                createRoleData.FallInOption = (FallInOption)MyGUI.EnumPopupWithTitle("跌落选项", createRoleData.FallInOption);
                createRoleData.FallInHeight = FloatFieldWithTitle("跌落高度", createRoleData.FallInHeight);
                
                //属性 add by TangJian 2018/01/29 12:41:46
//                createRoleData.roleData.AttrData = AttrDataField(createRoleData.roleData.AttrData, AttrDataFiledType.Role);
                
                createRoleData.WithTrunBackAnim = ToggleWithTitle("是否有转身动画", createRoleData.WithTrunBackAnim);
                
                createRoleData.CanRebound = ToggleWithTitle("能否被反弹", createRoleData.CanRebound); 
                createRoleData.Damping = FloatFieldWithTitle("Damping(阻尼)", createRoleData.Damping);
                createRoleData.atkPropertyType = (AtkPropertyType)MyGUI.EnumPopupWithTitle("atkPropertyType(攻击属性类型)", createRoleData.atkPropertyType);
                createRoleData.DefendBoundcenter = MyGUI.Vector3WithTitle("防御包围盒center", createRoleData.DefendBoundcenter);
                createRoleData.DefendBoundsize = MyGUI.Vector3WithTitle("防御包围盒size", createRoleData.DefendBoundsize);

                createRoleData.PauseTime = MyGUI.FloatFieldWithTitle("PauseTime(指当攻击方被角色格挡时发生的攻击停顿时间)", createRoleData.PauseTime);

                createRoleData.damageTargetSwitch =
                    ToggleWithTitle("damageTargetSwitch(受击区域开关)", createRoleData.damageTargetSwitch);
                
                GUILayout.Label("免疫攻击力度类型");
                createRoleData.Atkheavyforcetype = MyGUI.ToggleWithTitle("重", createRoleData.Atkheavyforcetype);
                createRoleData.Atkmoderateforcetype = MyGUI.ToggleWithTitle("中", createRoleData.Atkmoderateforcetype);
                createRoleData.Atklightforcetype = MyGUI.ToggleWithTitle("轻", createRoleData.Atklightforcetype);

                createRoleData.IsMonsterHP = ToggleWithTitle("是否拥有血条", createRoleData.IsMonsterHP);

                // createRoleData.Radius=MyGUI.FloatFieldWithTitle("Radius(圆柱半径)",createRoleData.Radius);
                // createRoleData.Height=MyGUI.FloatFieldWithTitle("height(高度)",createRoleData.Height);
                createRoleData.StepHeight = MyGUI.FloatFieldWithTitle("stepheight(最大垂直距离)", createRoleData.StepHeight);
                EditorGUILayout.LabelField("MaxSlope(可行走的最大傾斜角度)");
                createRoleData.MaxSlope = EditorGUILayout.Slider(createRoleData.MaxSlope, 0f, 60f);

                createRoleData.shadowParameterOnOff = MyGUI.ToggleWithTitle("需要调整影子参数", createRoleData.shadowParameterOnOff);
                if (createRoleData.shadowParameterOnOff)
                {
                    createRoleData.ShadowCutoffDistance = MyGUI.FloatFieldWithTitle("CutoffDistance", createRoleData.ShadowCutoffDistance);
                    createRoleData.ShadowMaxScale = MyGUI.FloatFieldWithTitle("MaxScale", createRoleData.ShadowMaxScale);
                }
                createRoleData.withAI = MyGUI.ToggleWithTitle("withAI", createRoleData.withAI);
                if (createRoleData.withAI)
                {
                    BehaviorDesigner.Runtime.ExternalBehavior externalBehavior = (BehaviorDesigner.Runtime.ExternalBehavior)AssetDatabase.LoadAssetAtPath(createRoleData.AIPath, typeof(BehaviorDesigner.Runtime.ExternalBehavior));
                    externalBehavior = (BehaviorDesigner.Runtime.ExternalBehavior)EditorGUILayout.ObjectField(new GUIContent("AI行为树"), externalBehavior, typeof(BehaviorDesigner.Runtime.ExternalBehavior), true);
                    createRoleData.AIPath = AssetDatabase.GetAssetPath(externalBehavior);
//                createRoleData.AIPath = createRoleData.AIPath.Remove(createRoleData.AIPath.IndexOf(Definition.BehaviorTreePath), Definition.BehaviorTreePath.Length);
                }
                else
                {
                    createRoleData.AIname = MyGUI.TextFieldWithTitle("AI Scrip Name:", createRoleData.AIname);
                }

                Spine.Unity.SkeletonDataAsset SkeletonDataAsset = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(createRoleData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
                UnityEditor.Animations.AnimatorController animatorController = (UnityEditor.Animations.AnimatorController)AssetDatabase.LoadAssetAtPath(createRoleData.AnimControllerPath, typeof(UnityEditor.Animations.AnimatorController));
                animatorController = (UnityEditor.Animations.AnimatorController)EditorGUILayout.ObjectField(new GUIContent("动画控制器"), animatorController, typeof(UnityEditor.Animations.AnimatorController), true);
                SkeletonDataAsset = (Spine.Unity.SkeletonDataAsset)EditorGUILayout.ObjectField(new GUIContent("Skeleton"), SkeletonDataAsset, typeof(Spine.Unity.SkeletonDataAsset), true);
                createRoleData.SkeletonDataAssetPath = AssetDatabase.GetAssetPath(SkeletonDataAsset);
                createRoleData.AnimControllerPath = AssetDatabase.GetAssetPath(animatorController);
//            createRoleData.SkeletonDataAssetPath = createRoleData.SkeletonDataAssetPath.Remove(createRoleData.SkeletonDataAssetPath.IndexOf(Definition.SpineAssetPath), Definition.SpineAssetPath.Length);
//            createRoleData.AnimControllerPath = createRoleData.AnimControllerPath.Remove(createRoleData.AnimControllerPath.IndexOf(Definition.SpineAssetPath), Definition.SpineAssetPath.Length);
                if (SkeletonDataAsset != null)
                {
                    string[] skins = new string[SkeletonDataAsset.GetSkeletonData(false).Skins.Count];
                    int skinIndex = 0;
                    for (int i = 0; i < skins.Length; i++)
                    {
                        string skinNameString = SkeletonDataAsset.GetSkeletonData(false).Skins.Items[i].Name;
                        skins[i] = skinNameString;
                        if (skinNameString == createRoleData.SkinName)
                            skinIndex = i;
                    }
                    skinIndex = EditorGUILayout.Popup("Initial Skin(皮肤)", skinIndex, skins);
                    createRoleData.SkinName = skins[skinIndex];
                }
                createRoleData.size = MyGUI.Vector3WithTitle("size(角色体积大小)", createRoleData.size);
            }
           
            return createRoleData;
        }


        private static Dictionary<int, CSScriptLibrary.AsmHelper> autoSerializedObjectField = new Dictionary<int, CSScriptLibrary.AsmHelper>();
        public static T AutoSerializedObjectField<T>(T current) where T : class
        {
            System.Type type = typeof(T);
            int key = type.FullName.GetHashCode();
            CSScriptLibrary.AsmHelper helper;
            if (autoSerializedObjectField.TryGetValue(key, out helper))
            {

            }
            else
            {
                string tempClassName = "TempClass_" + type.Name;

                string scriptCode = @"
                                using System;
                                using UnityEngine;
                                using UnityEditor;
                                using Tang;
                                using ZS;

                                public class Helper
                                {
                                static " + tempClassName + @" testClass;
                                static SerializedObject currSerializedObject;

                                static public void AutoSerializedObjectField(" + type.Name + @" current)
                               {
                                    if (currSerializedObject == null || testClass == null || (object)currSerializedObject.targetObject != testClass
                                    || testClass.data != current)
                                    {
                                        testClass = ScriptableObject.CreateInstance<" + tempClassName + @">();
                                        testClass.data = current;
                                        //" + "Debug.Log(" + "\"testClass = \" + testClass);" + @"
                                        if (testClass!=null)
                                        {
                                            currSerializedObject = new SerializedObject(testClass);
                                        }
                                    }

                                    if(currSerializedObject != null)
                                    {
                                     " + "Tang.Reflection.Instance.Invoke(\"Editor\", \"DoDrawDefaultInspector\", null, new object[] { currSerializedObject });" + @"
                                        currSerializedObject.ApplyModifiedPropertiesWithoutUndo();
                                    }
                                }
                            " +
                                    "}\n" +

                                    "public class " + tempClassName + " : ScriptableObject" +
                                    @"{
                            public " + type.Name + " data;" +
                                    "}"
                    ;

                //Debug.Log("scriptCode = " + scriptCode);

                helper = new CSScriptLibrary.AsmHelper(CSScriptLibrary.CSScript.LoadCode(scriptCode, null, false));
                autoSerializedObjectField.Add(key, helper);
            }
            helper.Invoke("Helper.AutoSerializedObjectField", current);
            return current;
        }

        public static RoleAIAction RoleAIActionField(RoleAIAction roleAIAction)
        {

            roleAIAction.name = MyGUI.TextFieldWithTitle("Name", roleAIAction.name);
            roleAIAction.actionId = MyGUI.TextFieldWithTitle("actionId", roleAIAction.actionId);
            roleAIAction.interval = MyGUI.FloatFieldWithTitle("间隔时间", roleAIAction.interval);
            roleAIAction.duration = MyGUI.FloatFieldWithTitle("持续时间", roleAIAction.duration);
            // roleAIAction.bounds.center=MyGUI.Vector3WithTitle("center",roleAIAction.bounds.center);
            // roleAIAction.bounds.size=MyGUI.Vector3WithTitle("center",roleAIAction.bounds.size);
            var bounds = new Bounds(MyGUI.Vector3WithTitle("center", roleAIAction.bounds.center), MyGUI.Vector3WithTitle("size(是实际大小的两倍)", roleAIAction.bounds.size));
            roleAIAction.weight = MyGUI.IntFieldWithTitle("随机权重", roleAIAction.weight);
            roleAIAction.bounds = bounds;
            return roleAIAction;
        }
        public static WeaknessData WeaknessDataFielf(WeaknessData weaknessData, CreateRoleData createRoleData, float innerBoxWidth)
        {
            weaknessData.colliderType = (ColliderType)MyGUI.EnumPopupWithTitle("类型:", weaknessData.colliderType);
            switch (weaknessData.colliderType)
            {
                case ColliderType.weakness:
                    weaknessData.WeaknessName = MyGUI.TextFieldWithTitle("弱点名称", weaknessData.WeaknessName);
                    weaknessData.WeaknessHPMax = MyGUI.FloatFieldWithTitle("弱点血量上限", weaknessData.WeaknessHPMax);
                    weaknessData.WeaknessHP = weaknessData.WeaknessHPMax;
                    break;
                case ColliderType.collider:
                {
                    weaknessData.WeaknessName = MyGUI.TextFieldWithTitle("名称", weaknessData.WeaknessName);
                    int Collidertagindex = 0;
                    for (int ign = 0; ign < UnityEditorInternal.InternalEditorUtility.tags.Length; ign++)
                    {
                        if (UnityEditorInternal.InternalEditorUtility.tags[ign] == weaknessData.CollidertagName)
                        {
                            Collidertagindex = ign;
                        }
                    }
                    int Colliderlayerindex = 0;
                    for (int lagn = 0; lagn < UnityEditorInternal.InternalEditorUtility.layers.Length; lagn++)
                    {
                        if (UnityEditorInternal.InternalEditorUtility.layers[lagn] == weaknessData.ColliderlayerName)
                        {
                            Colliderlayerindex = lagn;
                        }
                    }
                    Collidertagindex = EditorGUILayout.Popup("碰撞体Tag", Collidertagindex, UnityEditorInternal.InternalEditorUtility.tags);
                    weaknessData.CollidertagName = UnityEditorInternal.InternalEditorUtility.tags[Collidertagindex];
                    Colliderlayerindex = EditorGUILayout.Popup("碰撞体Layer", Colliderlayerindex, UnityEditorInternal.InternalEditorUtility.layers);
                    weaknessData.ColliderlayerName = UnityEditorInternal.InternalEditorUtility.layers[Colliderlayerindex];
                    if (MyGUI.Button("+"))
                    {
                        weaknessData.ComponentPathList.Add("");
                    }
                    for (int clist = weaknessData.ComponentPathList.Count - 1; clist >= 0; clist--)
                    {
                        EditorGUILayout.BeginHorizontal();
                        MonoScript monoScript = (MonoScript)AssetDatabase.LoadAssetAtPath(weaknessData.ComponentPathList[clist], typeof(MonoScript));
                        monoScript = (UnityEditor.MonoScript)EditorGUILayout.ObjectField(new GUIContent("脚本"), monoScript, typeof(UnityEditor.MonoScript), true);
                        weaknessData.ComponentPathList[clist] = AssetDatabase.GetAssetPath(monoScript);
                        if (MyGUI.Button("-"))
                        {
                            weaknessData.ComponentPathList.RemoveAt(clist);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                    break;
                case ColliderType.Trigger:
                {
                    weaknessData.WeaknessName = MyGUI.TextFieldWithTitle("名称", weaknessData.WeaknessName);
                    int Collidertagindex = 0;
                    for (int ign = 0; ign < UnityEditorInternal.InternalEditorUtility.tags.Length; ign++)
                    {
                        if (UnityEditorInternal.InternalEditorUtility.tags[ign] == weaknessData.CollidertagName)
                        {
                            Collidertagindex = ign;
                        }
                    }
                    int Colliderlayerindex = 0;
                    for (int lagn = 0; lagn < UnityEditorInternal.InternalEditorUtility.layers.Length; lagn++)
                    {
                        if (UnityEditorInternal.InternalEditorUtility.layers[lagn] == weaknessData.ColliderlayerName)
                        {
                            Colliderlayerindex = lagn;
                        }
                    }
                    Collidertagindex = EditorGUILayout.Popup("碰撞体Tag", Collidertagindex, UnityEditorInternal.InternalEditorUtility.tags);
                    weaknessData.CollidertagName = UnityEditorInternal.InternalEditorUtility.tags[Collidertagindex];
                    Colliderlayerindex = EditorGUILayout.Popup("碰撞体Layer", Colliderlayerindex, UnityEditorInternal.InternalEditorUtility.layers);
                    weaknessData.ColliderlayerName = UnityEditorInternal.InternalEditorUtility.layers[Colliderlayerindex];
                    if (MyGUI.Button("+"))
                    {
                        weaknessData.ComponentPathList.Add("");
                    }
                    for (int clist = weaknessData.ComponentPathList.Count - 1; clist >= 0; clist--)
                    {
                        EditorGUILayout.BeginHorizontal();
                        MonoScript monoScript = (MonoScript)AssetDatabase.LoadAssetAtPath(weaknessData.ComponentPathList[clist], typeof(MonoScript));
                        monoScript = (UnityEditor.MonoScript)EditorGUILayout.ObjectField(new GUIContent("脚本"), monoScript, typeof(UnityEditor.MonoScript), true);
                        weaknessData.ComponentPathList[clist] = AssetDatabase.GetAssetPath(monoScript);
                        if (MyGUI.Button("-"))
                        {
                            weaknessData.ComponentPathList.RemoveAt(clist);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    //MonoScript monoScript = (MonoScript)AssetDatabase.LoadAssetAtPath(skillData.componentPath, typeof(MonoScript));
                    //gameobject.AddComponent(monoScript.GetClass());
                }

                    break;
            }
            if (weaknessData.followType != FollowType.slot)
            {
                weaknessData.center = MyGUI.Vector3WithTitle("center", weaknessData.center);
                weaknessData.size = MyGUI.Vector3WithTitle("size", weaknessData.size);
            }
            weaknessData.BoneFollow = MyGUI.ToggleWithTitle("跟随", weaknessData.BoneFollow);
            if (weaknessData.BoneFollow)
            {
                weaknessData.followType = (FollowType)MyGUI.EnumPopupWithTitle("跟随类型", weaknessData.followType);
                Spine.Unity.SkeletonDataAsset SkeletonDataAsset = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(createRoleData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
                if(SkeletonDataAsset != null)
                {
                    Spine.SkeletonData skeletonData = SkeletonDataAsset.GetSkeletonData(false);
                    switch (weaknessData.followType)
                    {
                        case FollowType.bone:
                        {
                            if (SkeletonDataAsset != null)
                            {
                                string[] bones = new string[skeletonData.Bones.Count];
                                int boneindex = 0;
                                for (int i = 0; i < bones.Length; i++)
                                {
                                    string BoneNameString = skeletonData.Bones.Items[i].Name;
                                    bones[i] = BoneNameString;
                                    if (BoneNameString == weaknessData.BoneName)
                                        boneindex = i;
                                }
                                boneindex = EditorGUILayout.Popup("Bone Name(Bone名称)", boneindex, bones);
                                weaknessData.BoneName = bones[boneindex];
                            }
                            //weakn
                        }
                            break;
                        case FollowType.slot:
                        {
                            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth), GUILayout.ExpandHeight(true));
                            if (SkeletonDataAsset != null)
                            {
                                if (MyGUI.Button("+"))
                                {
                                    weaknessData.slotNameList.Add(new WeaknessslotData());
                                }
                                string[] Slots = new string[skeletonData.Slots.Count];
                                for (int i = 0; i < Slots.Length; i++)
                                {
                                    string SlotNameString = skeletonData.Slots.Items[i].Name;
                                    Slots[i] = SlotNameString;
                                }
                                for (int e = weaknessData.slotNameList.Count - 1; e >= 0; e--)
                                {
                                    int index = 0;
                                    for (int i = 0; i < Slots.Length; i++)
                                    {
                                        if (Slots[i] == weaknessData.slotNameList[e].slotname)
                                        {
                                            index = i;
                                        }
                                    }
                                    index = EditorGUILayout.Popup("slot Name(slot 名称)", index, Slots);
                                    weaknessData.slotNameList[e].slotname = Slots[index];
                                    weaknessData.slotNameList[e].z = MyGUI.FloatFieldWithTitle("z", weaknessData.slotNameList[e].z);
                                    weaknessData.slotNameList[e].triggerMode = (TriggerMode)MyGUI.EnumPopupWithTitle("触发模式", weaknessData.slotNameList[e].triggerMode);
                                    weaknessData.slotNameList[e].isTrigger = MyGUI.ToggleWithTitle("isTrigger", weaknessData.slotNameList[e].isTrigger);
                                    if (weaknessData.slotNameList[e].isTrigger)
                                    {
                                        weaknessData.slotNameList[e].TriggerId = MyGUI.TextFieldWithTitle("TriggerId", weaknessData.slotNameList[e].TriggerId);
                                    }
                                    int Collidertagindex = 0;
                                    for (int ign = 0; ign < UnityEditorInternal.InternalEditorUtility.tags.Length; ign++)
                                    {
                                        if (UnityEditorInternal.InternalEditorUtility.tags[ign] == weaknessData.slotNameList[e].tag)
                                        {
                                            Collidertagindex = ign;
                                        }
                                    }
                                    int Colliderlayerindex = 0;
                                    for (int lagn = 0; lagn < UnityEditorInternal.InternalEditorUtility.layers.Length; lagn++)
                                    {
                                        if (UnityEditorInternal.InternalEditorUtility.layers[lagn] == weaknessData.slotNameList[e].layer)
                                        {
                                            Colliderlayerindex = lagn;
                                        }
                                    }
                                    Collidertagindex = EditorGUILayout.Popup("Tag", Collidertagindex, UnityEditorInternal.InternalEditorUtility.tags);
                                    weaknessData.slotNameList[e].tag = UnityEditorInternal.InternalEditorUtility.tags[Collidertagindex];
                                    Colliderlayerindex = EditorGUILayout.Popup("Layer", Colliderlayerindex, UnityEditorInternal.InternalEditorUtility.layers);
                                    weaknessData.slotNameList[e].layer = UnityEditorInternal.InternalEditorUtility.layers[Colliderlayerindex];
                                    if (MyGUI.Button("-"))
                                    {
                                        weaknessData.slotNameList.RemoveAt(e);
                                    }
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                            break;
                    }
                }
                
            }
            return weaknessData;
        }
        
        public static List<string> DamageTargetListField(List<string> list)
        {
            int RemoveIndex = -1;
            List<string> currlist = new List<string>();
            if (list != null)
            {
                currlist = list;
            }
            
            EditorGUILayout.BeginVertical(boxStyle);
            
            if (MyGUI.Button("+"))
            {
                currlist.Add("");
            }
            
            for (int e = 0; e< currlist.Count; e++)
            {
                EditorGUILayout.BeginHorizontal(boxStyle);
                currlist[e] = MyGUI.TextFieldWithTitle(e.ToString(), currlist[e]);
                
                if (MyGUI.Button("-"))
                {
                    RemoveIndex = e;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
            
            if (RemoveIndex>=0)
            {
                currlist.RemoveAt(RemoveIndex);
            }
            
            return currlist;
        }
        
        public static async Task<ItemData> ItemDataField(ItemData itemData, string SkeletonDataAssetPath = null)
        {
            itemData.id = MyGUI.TextFieldWithTitle("id", itemData.id);
            itemData.name = MyGUI.TextFieldWithTitle("name(名称)", itemData.name);
            itemData.desc = MyGUI.TextFieldWithTitle("desc(描述)", itemData.desc);

            itemData.level = MyGUI.IntFieldWithTitle("level(等级)", itemData.level);

            itemData.renderType = (RenderType)MyGUI.EnumPopupWithTitle("RenderType(渲染类型)", itemData.renderType);

            switch (itemData.renderType)
            {
                case RenderType.Image:
                    itemData.icon = MyGUI.TextFieldWithTitle("icon", itemData.icon);
                {
                    Texture2D texture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(Definition.TextureAssetPrefix + "Icon/" + itemData.icon + ".png");
                    EditorGUILayout.ObjectField("贴图:", texture2d, typeof(Texture2D), false);
                }
                    break;
                case RenderType.Anim:
                {
                    itemData.anim = MyGUI.ObjectField<SkeletonDataAsset>("动画:", itemData.anim);
                    itemData.idleAnim = MyGUI.TextFieldWithTitle("待机动画", itemData.idleAnim);
                    itemData.destoryAnim = MyGUI.TextFieldWithTitle("销毁动画", itemData.destoryAnim);
                }
                    break;
                default:
                    break;
            }
            

            itemData.itemType = (ItemType)MyGUI.EnumPopupWithTitle("itemType(物品类型)", itemData.itemType);
            itemData.canStack = MyGUI.ToggleWithTitle("能否叠加", itemData.canStack);
            itemData.pickUpMethod = (PickUpMethod)MyGUI.EnumPopupWithTitle("捡取手段", itemData.pickUpMethod);

            if (itemData is EquipData equipData)
            {
                equipData.groundImgId = MyGUI.TextFieldWithTitle("Imgid", equipData.groundImgId);
                equipData.imgGround = (ImgGround)MyGUI.EnumPopupWithTitle("imgGround", equipData.imgGround);

                switch (equipData.imgGround)
                {
                    case ImgGround.Dedult:
                        break;
                    case ImgGround.ground:
                    {
                        Texture2D texture2d = 
                            await AssetManager.LoadAssetAsync<Texture2D>("Assets/Resources_moved/Textures/Icon/" + equipData.groundImgId+".png");

                        EditorGUILayout.ObjectField("掉落物地面贴图:", texture2d, typeof(Texture2D), false);
                    }
                        break;
                    default:
                        break;
                }
                equipData.equipType = (EquipType)MyGUI.EnumPopupWithTitle("equipType", equipData.equipType);

                equipData.attrData.strength =  MyGUI.FloatFieldWithTitle("力量", equipData.attrData.strength);
                equipData.attrData.weight =  MyGUI.FloatFieldWithTitle("重量", equipData.attrData.weight);
                equipData.attrData.lessenhurt =  MyGUI.FloatFieldWithTitle("减伤百分比（1 = 100%）", equipData.attrData.lessenhurt);
                
                MyGUIExtend.Instance.Foldout("Equip","WeaponData", () =>
                {
                if (equipData is WeaponData weaponData)
                {
                    equipData.attrData = MyGUI.AttrDataMinMaxField(equipData.attrData);
                    weaponData.atkPropertyType = (AtkPropertyType)MyGUI.EnumPopupWithTitle("atkPropertyType(攻击属性类型)", weaponData.atkPropertyType);
                    if (SkeletonDataAssetPath != null && SkeletonDataAssetPath != "")
                    {
                        Spine.Unity.SkeletonDataAsset SkeletonDataAsset = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
                    }
                    else
                    {
                        weaponData.mainHandAttachmentName = MyGUI.TextFieldWithTitle("mainHandAttachmentName(主手握持显示)", weaponData.mainHandAttachmentName);
                        weaponData.offHandAttachmentName = MyGUI.TextFieldWithTitle("offHandAttachmentName(副手握持显示)", weaponData.offHandAttachmentName);
                    }
                    weaponData.damageEffectType = (DamageEffectType)MyGUI.EnumPopupWithTitle("damageEffectType", weaponData.damageEffectType);
                    weaponData.possessType = (PossessType)MyGUI.EnumPopupWithTitle("PossessType(持有类型)", weaponData.possessType);

                }
                else if (equipData is ArmorData)
                {
                    equipData.attrData = MyGUI.AttrDataField(equipData.attrData);
                    ArmorData armorData = equipData as ArmorData;
                    if (SkeletonDataAssetPath != null && SkeletonDataAssetPath != "")
                    {
                    }
                    else
                    {
                        armorData.skinName = MyGUI.TextFieldWithTitle("skinName(皮肤显示)", armorData.skinName);
                    }

                    // armorData.def = MyGUI.FloatFieldWithTitle("def", armorData.def);
                }
                else if (equipData is SoulData)
                {
                    equipData.attrData = MyGUI.AttrDataField(equipData.attrData);
                    SoulData soulData = equipData as SoulData;
                    soulData.skillId = MyGUI.TextFieldWithTitle("skillId", soulData.skillId);
                    soulData.soulCharging.soulType = (SoulType)MyGUI.EnumPopupWithTitle("SoulType(充能类型)", soulData.soulCharging.soulType);
                    switch (soulData.soulCharging.soulType)
                    {
                        case SoulType.time:
                            soulData.soulCharging.cd = MyGUI.FloatFieldWithTitle("cd", soulData.soulCharging.cd);
                            break;
                        case SoulType.NewRoom:
                            soulData.soulCharging.int1 = MyGUI.IntFieldWithTitle("充能次数", soulData.soulCharging.int1);
                            soulData.soulCharging.Chargingcount = MyGUI.IntFieldWithTitle("充能上限", soulData.soulCharging.Chargingcount);
                            break;

                        default:
                            break;
                    }
                }
                else if (equipData is DecorationData)
                {
                    equipData.attrData = MyGUI.AttrDataField(equipData.attrData);
                    DecorationData decorationData = equipData as DecorationData;
                    if (MyGUI.Button("+"))
                    {

                    }
                    if (MyGUI.Button("-"))
                    {

                    }
                    decorationData.buffId = MyGUI.TextFieldWithTitle("buffId", decorationData.buffId);

                }
                
                });
                
            }
            else if (itemData is ConsumableData)
            {
                ConsumableData consumableData = itemData as ConsumableData;
                consumableData.buffId = MyGUI.TextFieldWithTitle("buffId", consumableData.buffId);
                consumableData.consumableUseType = (ConsumableUseType)MyGUI.EnumPopupWithTitle("消耗品类型", consumableData.consumableUseType);
            }

            return itemData;
        }


        public class LayoutOption
        {

        }

        static Dictionary<string, Rect> getIntRectInDyadicIntArrayCache = new Dictionary<string, Rect>();

        private static Rect GetIntRectInDyadicIntArray(int[,] dyadicArray, int tag)
        {
            Rect retRect = new Rect();

            int rows = dyadicArray.GetLength(0);
            int cols = dyadicArray.GetLength(1);

            int beginRow = -1;
            int beginCol = -1;
            int endRow = -1;
            int endCol = -1;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (tag == dyadicArray[r, c])
                    {
                        // 第一次 add by TangJian 2017/12/05 17:03:11
                        if (beginRow == -1)
                        {
                            beginRow = r;
                            beginCol = c;

                            endRow = r;
                            endCol = c;
                        }
                        else
                        {
                            endRow = r;
                            endCol = c;
                        }
                    }
                }
            }

            float itemWidth = 1.0f / (cols);
            float itemHeight = 1.0f / (rows);

            Vector2 size = new Vector2();
            size.x = (float)(endCol - beginCol + 1) / (cols);
            size.y = (float)(endRow - beginRow + 1) / (rows);

            Vector2 leftBottomPos = new Vector2();
            leftBottomPos.x = beginCol * itemWidth;
            leftBottomPos.y = (endRow + 1) * itemHeight;

            retRect.size = size;
            retRect.center = new Vector2(leftBottomPos.x + retRect.size.x / 2, leftBottomPos.y - retRect.size.y / 2);//  - retRect.size / 2;

            return retRect;
        }

        public static void DamageDataField(DamageData damageData)
        {
            damageData.hitType = (int)(HitType)MyGUI.EnumPopupWithTitle("hitType", (HitType)damageData.hitType);
            damageData.damageEffectType = (DamageEffectType)MyGUI.EnumPopupWithTitle("damageEffectType", damageData.damageEffectType);
            damageData.targetMoveBy = MyGUI.Vector3WithTitle("targetMoveBy", damageData.targetMoveBy);
            damageData.SpecialEffectPos = MyGUI.Vector3WithTitle("SpecialEffectPos", damageData.SpecialEffectPos);
            damageData.SpecialEffectRotation = MyGUI.FloatFieldWithTitle("SpecialEffectRotation", damageData.SpecialEffectRotation);
            damageData.SpecialEffectScale = MyGUI.FloatFieldWithTitle("SpecialEffectScale", damageData.SpecialEffectScale);




        }

        public static void VerticalLayout(System.Action action, GUIStyle style)
        {
            // Rect position = EditorGUILayout.GetControlRect();

            // var width = position.size.x - style.border.horizontal;

            EditorGUILayout.BeginVertical(style);
            action();
            EditorGUILayout.EndVertical();
        }

        public static void VerticalLayout(System.Action action)
        {
            // Rect position = EditorGUILayout.GetControlRect();

            // var width = position.size.x - style.border.horizontal;

            EditorGUILayout.BeginVertical();
            action();
            EditorGUILayout.EndVertical();
        }

        static GUIStyle boxStyle = new GUIStyle("box");

        public static Anim.ActionData ActionDataField(Anim.ActionData actionData, AnimEffectData.AnimEffectType animEffectType)
        {
            EditorGUILayout.BeginVertical(boxStyle);

            actionData.actionType = (Anim.ActionType)EnumPopupWithTitle("类型", actionData.actionType);


            actionData.duration = FloatFieldWithTitle("duration", actionData.duration);
            actionData.randomDurationFrom = FloatFieldWithTitle("randomDurationFrom", actionData.randomDurationFrom);
            actionData.randomDurationTo = FloatFieldWithTitle("randomDurationTo", actionData.randomDurationTo);

            switch (actionData.actionType)
            {
                case Anim.ActionType.MoveBy:
                    actionData.pos = Vector3WithTitle("pos", actionData.pos);

                    actionData.isNormalizedrandom = ToggleWithTitle("isNormalizedrandom", actionData.isNormalizedrandom);
                    actionData.randomPosFrom = Vector3WithTitle("randomPosFrom", actionData.randomPosFrom);
                    actionData.randomPosTo = Vector3WithTitle("randomPosTo", actionData.randomPosTo);

                    actionData.ease = (DG.Tweening.Ease)EnumPopupWithTitle("ease", actionData.ease);
                    break;

                case Anim.ActionType.FadeTo:
                    actionData.alpha = FloatFieldWithTitle("alpha", actionData.alpha);

                    actionData.ease = (DG.Tweening.Ease)EnumPopupWithTitle("ease", actionData.ease);

                    break;
                case Anim.ActionType.AddRotation:
                    actionData.RandomType = (Anim.RandomType)EnumPopupWithTitle("固定还是随机", actionData.RandomType);
                    actionData.rotateMode = (DG.Tweening.RotateMode)EnumPopupWithTitle("RotateMode(旋转模式)", actionData.rotateMode);
                    if (actionData.RandomType == Anim.RandomType.Fix)
                    {
                        actionData.rotation = Vector3WithTitle("rotation", actionData.rotation);
                    }
                    else
                    {
                        actionData.randomRotationFrom = Vector3WithTitle("RotationFrom", actionData.randomRotationFrom);
                        actionData.randomRotationTo = Vector3WithTitle("RotationTo", actionData.randomRotationTo);
                    }
                    break;
                case Anim.ActionType.AddForce:
                    if (animEffectType == AnimEffectData.AnimEffectType.Gameobject)
                    {
                        actionData.RandomType = (Anim.RandomType)EnumPopupWithTitle("固定还是随机", actionData.RandomType);
                        if (actionData.RandomType == Anim.RandomType.Fix)
                        {
                            actionData.Force = Vector3WithTitle("力度", actionData.Force);
                        }
                        else
                        {
                            actionData.randomForceMin = Vector3WithTitle("最小力度", actionData.randomForceMin);
                            actionData.randomForceMax = Vector3WithTitle("最大力度", actionData.randomForceMax);
                        }
                        actionData.ForceMode = (ForceMode)EnumPopupWithTitle("力的作用方式(unity)", actionData.ForceMode);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("只支持AnimEffectType类型Gameobject");
                    }
                    break;
                case Anim.ActionType.AddTorque:
                    if (animEffectType == AnimEffectData.AnimEffectType.Gameobject)
                    {
                        actionData.RandomType = (Anim.RandomType)EnumPopupWithTitle("固定还是随机", actionData.RandomType);
                        if (actionData.RandomType == Anim.RandomType.Fix)
                        {
                            actionData.Force = Vector3WithTitle("力度", actionData.Force);
                        }
                        else
                        {
                            actionData.randomForceMin = Vector3WithTitle("最小力度", actionData.randomForceMin);
                            actionData.randomForceMax = Vector3WithTitle("最大力度", actionData.randomForceMax);
                        }
                        actionData.ForceMode = (ForceMode)EnumPopupWithTitle("力的作用方式(unity)", actionData.ForceMode);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("只支持AnimEffectType类型Gameobject");
                    }

                    break;
                case Anim.ActionType.ScaleTo:
                    actionData.scale = Vector3WithTitle("scale", actionData.scale);

                    actionData.isNormalizedrandom = ToggleWithTitle("isNormalizedrandom", actionData.isNormalizedrandom);
                    actionData.randomScaleFrom = Vector3WithTitle("scaleFrom", actionData.randomScaleFrom);
                    actionData.randomScaleTo = Vector3WithTitle("scaleTo", actionData.randomScaleTo);

                    actionData.ease = (DG.Tweening.Ease)EnumPopupWithTitle("ease", actionData.ease);

                    break;

                case Anim.ActionType.MulColorTo:
                    actionData.mulColor = ColorFieldWithTitle("mulColor", actionData.mulColor);



                    actionData.ease = (DG.Tweening.Ease)EnumPopupWithTitle("ease", actionData.ease);

                    break;

                case Anim.ActionType.AddColorTo:
                    actionData.addColor = ColorFieldWithTitle("addColor", actionData.addColor);

                    actionData.ease = (DG.Tweening.Ease)EnumPopupWithTitle("ease", actionData.ease);

                    break;
                case Anim.ActionType.Curve:
                    EditorGUILayout.LabelField("gameobject使用重力时无效");
                    actionData.width = FloatFieldWithTitle("width", actionData.width);
                    actionData.height = FloatFieldWithTitle("height", actionData.height);
                    actionData.t = FloatFieldWithTitle("t", actionData.t);
                    break;
                case Anim.ActionType.Path:
                    EditorGUILayout.LabelField("gameobject使用重力时无效");
                    actionData.pathType = (DG.Tweening.PathType)EnumPopupWithTitle("pathType", actionData.pathType);
                    actionData.pathMode = (DG.Tweening.PathMode)EnumPopupWithTitle("pathMode", actionData.pathMode);
                    EditorGUILayout.LabelField("resolution(路径精细程度)");
                    actionData.resolution = EditorGUILayout.IntSlider(actionData.resolution, 2, 20);
                   
                    //if (MyGUI.Button("parse"))
                    //{

                    //}
                    
                    if (MyGUI.Button("+"))
                    {
                        actionData.vector3List.Add(new Vector3());
                    }
                    
                    EditorGUILayout.BeginVertical();
                    for (int i = actionData.vector3List.Count - 1; i >= 0; i--)
                    {
                        //GUIStyle boxStyle = new GUIStyle("box"+i);
                        EditorGUILayout.BeginHorizontal();
                        actionData.vector3List[i] = Vector3WithTitle(i.ToString(), actionData.vector3List[i]);
                        if (MyGUI.Button("-"))
                        {
                            actionData.vector3List.RemoveAt(i);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    break;
                case Anim.ActionType.Sequence:
                    ActionDataListField(actionData.actionList, animEffectType);
                    break;

                case Anim.ActionType.Parallel:
                    ActionDataListField(actionData.actionList, animEffectType);
                    break;
            }

            EditorGUILayout.EndVertical();

            return actionData;
        }

        public static List<Anim.ActionData> ActionDataListField(List<Anim.ActionData> actionDataList, AnimEffectData.AnimEffectType animEffectType)
        {
            int removeIndex = -1;

            EditorGUILayout.BeginVertical();
            for (int i = 0; i < actionDataList.Count; i++)
            {

                EditorGUILayout.BeginHorizontal();
                ActionDataField(actionDataList[i], animEffectType);

                if (Button("-"))
                {
                    removeIndex = i;
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            if (removeIndex >= 0)
            {
                actionDataList.RemoveAt(removeIndex);
            }

            if (Button("+"))
            {
                actionDataList.Add(new Anim.ActionData());
            }

            return actionDataList;
        }

        public static void TextListField(List<string> stringList)
        {
            int removeIndex = -1;

            for (int i = 0; i < stringList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                stringList[i] = MyGUI.TextFieldWithTitle(i.ToString(), stringList[i]);

                if (Button("-"))
                {
                    removeIndex = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (removeIndex >= 0)
            {
                stringList.RemoveAt(removeIndex);
            }

            if (Button("+"))
            {
                stringList.Add("new");
            }
        }
        
        public static void HurtModeListField(List<HurtMode> hurtModeList)
        {
            int removeIndex = -1;
            
            for (int i = 0; i < hurtModeList.Count; i++)
            {
                using (new GUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        HurtModeField(hurtModeList[i],hurtModeList);
                    }
                    
                    if (GUILayout.Button("-",new GUIStyle("label"),GUILayout.Width(20)))
                    {
                        removeIndex = i;
                    }
                }
            }
            
            if(removeIndex >= 0)
                hurtModeList.RemoveAt(removeIndex);
        }

        public static void HurtModeField(HurtMode hurtMode,List<HurtMode> hurtModeList)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope() )
                {
                    GUILayout.Space(20);
                    hurtMode.Name = TextFieldWithTitle("Name", hurtMode.Name);
                    if (GUILayout.Button("+",new GUIStyle("label"),GUILayout.Width(20)))
                    {
                        hurtMode.HurtPartList.Add(new HurtPart());
                    }    
                }
                HurtPartListField(hurtMode.HurtPartList);
            }
        }

        public static void HurtPartListField(List<HurtPart> hurtPartList)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                int removeIndex = -1;
                
                for (int i = 0; i < hurtPartList.Count; i++)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(40);
                        using (new EditorGUILayout.VerticalScope())
                        {
                            GUILayout.BeginVertical("box",GUILayout.Height(250),GUILayout.Height(150));
                            HurtPartField(hurtPartList[i]);
                            GUILayout.EndVertical();
                        }

                        if (GUILayout.Button("-",new GUIStyle("label"),GUILayout.Width(20)))
                        {
                            removeIndex = i;
                        }
                    }
                }

                if (removeIndex >= 0)
                    hurtPartList.RemoveAt(removeIndex);
            }
        }

        public static void HurtPartField(HurtPart hurtPart)
        {
            hurtPart.Name = TextFieldWithTitle("名字", hurtPart.Name);
            hurtPart.Bounds = EditorGUILayout.BoundsField(hurtPart.Bounds);
            hurtPart.MatType = (MatType) MyGUI.EnumPopupWithTitle("材质", hurtPart.MatType);
            hurtPart.HurtRatio = FloatFieldWithTitle("伤害比率", hurtPart.HurtRatio);
            hurtPart.IsFollowSlot = EditorGUILayout.Toggle("是否跟随骨骼", hurtPart.IsFollowSlot);
            hurtPart.FollowSlotName = TextFieldWithTitle("Bone名字", hurtPart.FollowSlotName);
        }
        
        
        public static void AnimEffectRefListField(List<AnimEffectData.AnimEffectRef> animEffectRefList)
        {
            int removeIndex = -1;

            EditorGUILayout.LabelField("AnimEffectList:");

            for (int i = 0; i < animEffectRefList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(boxStyle);

                EditorGUILayout.BeginVertical();
                animEffectRefList[i].id = MyGUI.TextFieldWithTitle("id", animEffectRefList[i].id);

                animEffectRefList[i].weight = MyGUI.IntFieldWithTitle("weight", animEffectRefList[i].weight);

                animEffectRefList[i].delayTime = MyGUI.FloatFieldWithTitle("delayTime", animEffectRefList[i].delayTime);
                EditorGUILayout.EndVertical();



                if (Button("-"))
                {
                    removeIndex = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (removeIndex >= 0)
            {
                animEffectRefList.RemoveAt(removeIndex);
            }

            if (Button("+"))
            {
                animEffectRefList.Add(new AnimEffectData.AnimEffectRef());
            }
        }

        public static Vector2 AnimEffectDataField(AnimEffectData animEffectData, Vector2 scrollPos)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            VerticalLayout(() =>
            {
                animEffectData.animEffectType = (AnimEffectData.AnimEffectType)EnumPopupWithTitle("类型", animEffectData.animEffectType);


                animEffectData.id = TextFieldWithTitle("id", animEffectData.id);



                switch (animEffectData.animEffectType)
                {
                    case AnimEffectData.AnimEffectType.Image:
                    {
                        Texture2D texture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(animEffectData.imagePath);
                        texture2d = (Texture2D)EditorGUILayout.ObjectField("图片", texture2d, typeof(Texture2D));
                        animEffectData.imagePath = AssetDatabase.GetAssetPath(texture2d);

                        animEffectData.zwriteType = (FrameEventInfo.ZwriteType)EnumPopupWithTitle("Zwrite", animEffectData.zwriteType);
                        animEffectData.renderQueue = (FrameEventInfo.RenderQueue)EnumPopupWithTitle("RenderQueue", animEffectData.renderQueue);

                        animEffectData.anchor = Vector2WithTitle("anchor", animEffectData.anchor);

                        animEffectData.parentType = (FrameEventInfo.ParentType)EnumPopupWithTitle("parent", animEffectData.parentType);

                        animEffectData.renderFlip = ToggleWithTitle("渲染是否翻转", animEffectData.renderFlip);
                        animEffectData.moveFlip = ToggleWithTitle("运动是否翻转", animEffectData.moveFlip);

                        animEffectData.pos = Vector3WithTitle("pos(位置)", animEffectData.pos);
                        animEffectData.rotation = Vector3WithTitle("rotation(旋转)", animEffectData.rotation);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            animEffectData.scale = Vector3WithTitle("scale(缩放)", animEffectData.scale);
                            if (MyGUI.Button("自动大小"))
                            {
                                animEffectData.scale = new Vector3(texture2d.width / 100f, texture2d.height / 100f, animEffectData.scale.z);
                            }
                        }

                        animEffectData.mulColor = ColorFieldWithTitle("mulColor(乘颜色)", animEffectData.mulColor);
                        animEffectData.addColor = ColorFieldWithTitle("addColor(加颜色)", animEffectData.addColor);
                        animEffectData.alpha = FloatFieldWithTitle("alpha(透明度)", animEffectData.alpha);

                        ActionDataField(animEffectData.actionData, AnimEffectData.AnimEffectType.Image);
                    }
                        break;

                    case AnimEffectData.AnimEffectType.Group:

                    {
                        AnimEffectRefListField(animEffectData.animEffectList);
                    }

                        break;

                    case AnimEffectData.AnimEffectType.Random:

                        AnimEffectRefListField(animEffectData.animEffectList);


                        break;
                    case AnimEffectData.AnimEffectType.Gameobject:
                    {
                        animEffectData.gameobjectPathType = (AnimEffectData.GameobjectPathType)EnumPopupWithTitle("GameobjectPathType", animEffectData.gameobjectPathType);
                        if (animEffectData.gameobjectPathType == AnimEffectData.GameobjectPathType.name)
                        {
                            animEffectData.prefabname = TextFieldWithTitle("name", animEffectData.prefabname);
                        }
                        else
                        {
                            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(animEffectData.prefabpath);
                            gameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", gameObject, typeof(GameObject));
                            animEffectData.prefabpath = AssetDatabase.GetAssetPath(gameObject);
                        }
                        animEffectData.anchor = Vector2WithTitle("anchor", animEffectData.anchor);

                        animEffectData.parentType = (FrameEventInfo.ParentType)EnumPopupWithTitle("parent", animEffectData.parentType);

                        animEffectData.renderFlip = ToggleWithTitle("渲染是否翻转", animEffectData.renderFlip);
                        animEffectData.moveFlip = ToggleWithTitle("运动是否翻转", animEffectData.moveFlip);

                        animEffectData.pos = Vector3WithTitle("pos(位置)", animEffectData.pos);
                        animEffectData.rotation = Vector3WithTitle("rotation(旋转)", animEffectData.rotation);
                        animEffectData.scale = Vector3WithTitle("scale(缩放)", animEffectData.scale);
                        animEffectData.mulColor = ColorFieldWithTitle("mulColor(乘颜色)", animEffectData.mulColor);
                        animEffectData.addColor = ColorFieldWithTitle("addColor(加颜色)", animEffectData.addColor);
                        animEffectData.alpha = FloatFieldWithTitle("alpha(透明度)", animEffectData.alpha);

                        ActionDataField(animEffectData.actionData, AnimEffectData.AnimEffectType.Gameobject);
                    }
                        break;
                    case AnimEffectData.AnimEffectType.Anim:
                    {
                        Spine.Unity.SkeletonDataAsset skeletondataasset = AssetDatabase.LoadAssetAtPath<Spine.Unity.SkeletonDataAsset>(animEffectData.animPath);
                        skeletondataasset = (Spine.Unity.SkeletonDataAsset)EditorGUILayout.ObjectField("SkeletonDataAsset", skeletondataasset, typeof(Spine.Unity.SkeletonDataAsset));
                        animEffectData.animPath = AssetDatabase.GetAssetPath(skeletondataasset);
                        if (skeletondataasset != null)
                        {
                            if (skeletondataasset != null && skeletondataasset.GetSkeletonData(false) != null && skeletondataasset.GetSkeletonData(false).Animations != null)
                            {
                                string[] Animations = new string[skeletondataasset.GetSkeletonData(false).Animations.Count];
                                int skinIndex = 0;
                                for (int i = 0; i < Animations.Length; i++)
                                {
                                    string skinNameString = skeletondataasset.GetSkeletonData(false).Animations.Items[i].Name;
                                    Animations[i] = skinNameString;
                                    if (skinNameString == animEffectData.animName)
                                        skinIndex = i;
                                }
                                skinIndex = EditorGUILayout.Popup("Animation(动画片段)", skinIndex, Animations);
                                animEffectData.animName = Animations[skinIndex];
                            }

                        }

                        animEffectData.animSpeed = FloatFieldWithTitle("animSpeed(动画播放速度)", animEffectData.animSpeed);
                        animEffectData.animLoop = ToggleWithTitle("animLoop(动画是否循环播放)", animEffectData.animLoop);

                        animEffectData.anchor = Vector2WithTitle("anchor", animEffectData.anchor);

                        animEffectData.parentType = (FrameEventInfo.ParentType)EnumPopupWithTitle("parent", animEffectData.parentType);
                        animEffectData.moveFlip = ToggleWithTitle("(moveFlip)运动方向是否需要翻转", animEffectData.moveFlip);
                        animEffectData.pos = Vector3WithTitle("pos(位置)", animEffectData.pos);
                        animEffectData.rotation = Vector3WithTitle("rotation(旋转)", animEffectData.rotation);
                        animEffectData.scale = Vector3WithTitle("scale(缩放)", animEffectData.scale);
                        animEffectData.mulColor = ColorFieldWithTitle("mulColor(乘颜色)", animEffectData.mulColor);
                        animEffectData.addColor = ColorFieldWithTitle("addColor(加颜色)", animEffectData.addColor);
                        animEffectData.alpha = FloatFieldWithTitle("alpha(透明度)", animEffectData.alpha);


                        ActionDataField(animEffectData.actionData, AnimEffectData.AnimEffectType.Anim);
                    }
                        break;
                    case AnimEffectData.AnimEffectType.Particle:

                    {
                        animEffectData.prefabname = TextFieldWithTitle("name", animEffectData.prefabname);


                        {
                            animEffectData.anchor = Vector2WithTitle("anchor", animEffectData.anchor);

                            animEffectData.parentType = (FrameEventInfo.ParentType)EnumPopupWithTitle("parent", animEffectData.parentType);

                            animEffectData.renderFlip = ToggleWithTitle("渲染是否翻转", animEffectData.renderFlip);
                            animEffectData.moveFlip = ToggleWithTitle("运动是否翻转", animEffectData.moveFlip);

                            animEffectData.pos = Vector3WithTitle("pos(位置)", animEffectData.pos);
                            animEffectData.rotation = Vector3WithTitle("rotation(旋转)", animEffectData.rotation);
                            animEffectData.scale = Vector3WithTitle("scale(缩放)", animEffectData.scale);
                            animEffectData.mulColor = ColorFieldWithTitle("mulColor(乘颜色)", animEffectData.mulColor);
                            animEffectData.addColor = ColorFieldWithTitle("addColor(加颜色)", animEffectData.addColor);
                            animEffectData.alpha = FloatFieldWithTitle("alpha(透明度)", animEffectData.alpha);

                            ActionDataField(animEffectData.actionData, AnimEffectData.AnimEffectType.Gameobject);
                        }

                    }

                        break;

                    default:
                        EditorGUILayout.LabelField("没有实现的类型");
                        break;
                }


            }, new GUIStyle("box"));

            EditorGUILayout.EndScrollView();

            return scrollPos;
        }

        // public static void AnimEffectSaveDataField(AnimEffectSaveData animEffectSaveData)
        // {
        //     if (animEffectSaveData != null && animEffectSaveData.animEffectDataList != null)
        //     {
        //         for (int i = 0; i < animEffectSaveData.animEffectDataList.Count; i++)
        //         {
        //             AnimEffectDataField(animEffectSaveData.animEffectDataList[i]);
        //         }
        //     }
        // }

        // public static void ShowList(int count, System.Action<int> onDraw, System.Action<int> onAdd System.Action<int> onRemove)
        // {
        //     for (int i = 0; i < count; i++)
        //     {
        //         onDraw(i);
        //     }


        // }

        // public static void 

        public static void  Layout(Rect rect, GUIStyle style, int[,] layout, params System.Action<Rect>[] actions)
        {
            float horizontal = style.border.horizontal / 2;
            float vertical = style.border.vertical / 2;

            for (int i = 0; i < actions.Length; i++)
            {
                Rect itemRect = GetIntRectInDyadicIntArray(layout, i);
                //Debug.Log(i + " = " + itemRect);
                itemRect.size = itemRect.size.Mul(rect.size);
                itemRect.position = itemRect.position.Mul(rect.size);

                // 修改大小 add by TangJian 2017/12/05 20:45:24
                itemRect.position = new Vector2(itemRect.position.x + horizontal / 2, itemRect.position.y + vertical / 2);
                itemRect.size = new Vector2(itemRect.size.x - horizontal, itemRect.size.y - vertical);

                GUILayout.BeginArea(itemRect, style);
                actions[i](itemRect);
                GUILayout.EndArea();
            }
        }

        public static string ObjectField<T>(string title, string path) where T: Object
        {
            T obj = AssetDatabase.LoadAssetAtPath<T>(path);
            obj = EditorGUILayout.ObjectField(title, obj, typeof(T), false) as T;
            return AssetDatabase.GetAssetPath(obj);
        }

        public static Texture2D Objectfield(string title, Texture2D texture)
        {
            // EditorGUILayout.BeginHorizontal();
            texture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent(title), texture, typeof(Texture2D), true);
            // EditorGUILayout.EndHorizontal();
            return texture;
        }
        
        public static void PlayAnimFrameEventDatafield(ref UnityEngine.Object currData, List<string> animIdarrary, FrameEventData eventData, ref int index, params System.Action[] actions)
        {
            FrameEventInfo.PlayAnimFrameEventData data = currData as FrameEventInfo.PlayAnimFrameEventData;

            if (data == null)
            {
                data = Tools.Json2Obj<FrameEventInfo.PlayAnimFrameEventData>(eventData.String ?? "") ?? new FrameEventInfo.PlayAnimFrameEventData();
                currData = data;

            }
            if (data != null)
            {
                index = animIdarrary.IndexOf(data.animId);
                if (index == -1)
                {
                    index = 0;
                }
                index = PopupWithTitle("animId", index, animIdarrary.ToArray());
                data.animId = animIdarrary[index];
                data.parentType = (FrameEventInfo.ParentType)EnumPopupWithTitle("parentType", data.parentType);
                data.sceneDecorationPosition=(SceneDecorationPosition)EnumPopupWithTitle("显示位置", data.sceneDecorationPosition);
                data.animspeedtype = (SpeedType)EnumPopupWithTitle("动画速度类型", data.animspeedtype);
                if (data.animspeedtype == SpeedType.Fixed)
                {
                    data.animspeed = FloatFieldWithTitle("动画速度", data.animspeed);
                }
                data.pos = Vector3WithTitle("pos", data.pos);
                data.scale = Vector3WithTitle("scale", data.scale);
                data.orientation = Vector3WithTitle("orientation", data.orientation);
                if (data.animId != null && data.animId != "")
                {
                    Spine.Unity.SkeletonDataAsset SkeletonDataAsset = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(data.animId, typeof(Spine.Unity.SkeletonDataAsset));
                    if (SkeletonDataAsset != null && SkeletonDataAsset.GetSkeletonData(false) != null && SkeletonDataAsset.GetSkeletonData(false).Animations != null)
                    {
                        string[] Animations = new string[SkeletonDataAsset.GetSkeletonData(false).Animations.Count];
                        int skinIndex = 0;
                        for (int i = 0; i < Animations.Length; i++)
                        {
                            string skinNameString = SkeletonDataAsset.GetSkeletonData(false).Animations.Items[i].Name;
                            Animations[i] = skinNameString;
                            if (skinNameString == data.AnimatedFragment)
                                skinIndex = i;
                        }
                        skinIndex = EditorGUILayout.Popup("Animation(动画片段)", skinIndex, Animations);
                        data.AnimatedFragment = Animations[skinIndex];
                    }
                }
                currData = data;
            }


        }

        public static void PlayAnimList(ref UnityEngine.Object currData, List<string> animIdarrary, FrameEventData eventData, ref Vector2 scrollPos, params System.Action[] actions)
        {
            FrameEventInfo.FrameEventAnimList data = currData as FrameEventInfo.FrameEventAnimList;
            if (data == null)
            {
                data = Tools.Json2Obj<FrameEventInfo.FrameEventAnimList>(eventData.String ?? "") ?? new FrameEventInfo.FrameEventAnimList();
                currData = data;
            }
            if (data != null)
            {

                EditorGUILayout.BeginHorizontal();
                if (Button("+"))
                {
                    data.AnimList.Add(new AnimListFrameEventData());
                }
                if (MyGUI.Button("-"))
                {
                    if (data.AnimList.Count != 0)
                    {
                        data.AnimList.RemoveAt(data.AnimList.Count - 1);
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUIStyle boxStyle = new GUIStyle("box");
                data.pos = Vector3WithTitle("pos", data.pos);
                data.scale = Vector3WithTitle("scale", data.scale);
                EditorGUILayout.BeginVertical(boxStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(400));
                for (int e = data.AnimList.Count - 1; e >= 0; e--)
                {
                    EditorGUILayout.BeginVertical(boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(120));
                    int index = 0;

                    AnimListFrameEventData animListFrameEventData = data.AnimList[e];
                    index = animIdarrary.IndexOf(animListFrameEventData.Animpath);
                    if (index == -1)
                    {
                        index = 0;
                    }
                    index = PopupWithTitle("Animpath", index, animIdarrary.ToArray());
                    animListFrameEventData.Animpath = animIdarrary[index];


                    if (animListFrameEventData.Animpath != null && animListFrameEventData.Animpath != "")
                    {
                        Spine.Unity.SkeletonDataAsset SkeletonDataAsset = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(animListFrameEventData.Animpath, typeof(Spine.Unity.SkeletonDataAsset));
                        if (SkeletonDataAsset != null && SkeletonDataAsset.GetSkeletonData(false) != null && SkeletonDataAsset.GetSkeletonData(false).Animations != null)
                        {
                            string[] Animations = new string[SkeletonDataAsset.GetSkeletonData(false).Animations.Count];
                            int skinIndex = 0;
                            for (int i = 0; i < Animations.Length; i++)
                            {
                                string skinNameString = SkeletonDataAsset.GetSkeletonData(false).Animations.Items[i].Name;
                                Animations[i] = skinNameString;
                                if (skinNameString == animListFrameEventData.AnimatedFragment)
                                    skinIndex = i;
                            }
                            skinIndex = EditorGUILayout.Popup("Animation(动画片段)", skinIndex, Animations);
                            animListFrameEventData.AnimatedFragment = Animations[skinIndex];
                        }
                    }
                    EditorGUILayout.BeginHorizontal();
                    animListFrameEventData.datapos = ToggleWithTitle("使用单独的pos", animListFrameEventData.datapos);
                    animListFrameEventData.datascale = ToggleWithTitle("使用单独的scale", animListFrameEventData.datascale);
                    EditorGUILayout.EndHorizontal();
                    animListFrameEventData.parentType = (FrameEventInfo.ParentType)EnumPopupWithTitle("parentType", animListFrameEventData.parentType);
                    
                    if (animListFrameEventData.datapos)
                    {
                        animListFrameEventData.pos = Vector3WithTitle("pos", animListFrameEventData.pos);
                    }
                    
                    if (animListFrameEventData.datascale)
                    {
                        animListFrameEventData.scale = Vector3WithTitle("scale", animListFrameEventData.scale);
                    }

                    animListFrameEventData.weight = IntFieldWithTitle("权重", animListFrameEventData.weight);
                    animListFrameEventData.type = (AnimListFrameEventType)EnumPopupWithTitle("角度固定或随机", animListFrameEventData.type);
                    switch (animListFrameEventData.type)
                    {
                        case AnimListFrameEventType.Fixed:
                            animListFrameEventData.AnimRotation = FloatFieldWithTitle("角度", animListFrameEventData.AnimRotation);
                            break;
                        case AnimListFrameEventType.Random:
                            animListFrameEventData.AnimRotation = FloatFieldWithTitle("开始角度", animListFrameEventData.AnimRotation);
                            animListFrameEventData.float2 = FloatFieldWithTitle("结束角度", animListFrameEventData.float2);
                            break;
                        default:
                            break;
                    }

                    data.AnimList[e] = animListFrameEventData;
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            currData = data;
        }
        public static void RoleSkillList(ref Object currData, FrameEventData eventData)
        {
            //RoleSkillListFrameEventData data = new RoleSkillListFrameEventData();
            //if (data == null)
            //{
            //    data = Tools.Json2Obj<RoleSkillListFrameEventData>(eventData.String ?? "") ?? new RoleSkillListFrameEventData();
            //    //currData = data;
            //}
            //if (data != null)
            //{
            //    VerticalLayout(() => 
            //    {
            //        data.RoleSkillListType = (RoleSkillListType)MyGUI.EnumPopupWithTitle("类型:", data.RoleSkillListType);
            //        switch (data.RoleSkillListType)
            //        {
            //            case RoleSkillListType.parallel:
            //                SkillListfield(data.roleSkillListFrameEvents);
            //                break;
            //            case RoleSkillListType.random:
            //                SkillListfield(data.roleSkillListFrameEvents);
            //                break;
            //            case RoleSkillListType.sequence:
            //                SkillListfield(data.roleSkillListFrameEvents);
            //                break;
            //            case RoleSkillListType.SkillList:
            //                Skilldatalistfield(data, data.RoleSkillListType);
            //                break;
            //        }
            //    });
            //currData= data;
            //}
            //currData = data;
        }
        public static List<RoleSkillListFrameEventData> SkillListfield(List<RoleSkillListFrameEventData> Datalist)
        {
            int removeIndex = -1;
            //EditorGUILayout.LabelField("AnimEffectList:");
            List<RoleSkillListFrameEventData> currDatalist = Datalist;
            for (int i = 0; i < currDatalist.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(boxStyle);
                EditorGUILayout.BeginVertical(boxStyle);
                currDatalist[i].RoleSkillListType= (RoleSkillListType)MyGUI.EnumPopupWithTitle("类型:", currDatalist[i].RoleSkillListType);
                currDatalist[i] = Skilldatalistfield(currDatalist[i],currDatalist[i].RoleSkillListType);
                EditorGUILayout.EndVertical();
                if (Button("-"))
                {
                    removeIndex = i;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (removeIndex >= 0)
            {
                currDatalist.RemoveAt(removeIndex);
            }
           
            if (Button("+"))
            {
                currDatalist.Add(new RoleSkillListFrameEventData());
            }
            return currDatalist;
        }
        public static RoleSkillListFrameEventData Skilldatalistfield(RoleSkillListFrameEventData data, RoleSkillListType roleSkillListType)
        {
            //VerticalLayout(() => 
            //{
            RoleSkillListFrameEventData currdata = data;
            switch (roleSkillListType)
            {
                case RoleSkillListType.parallel:
                    currdata.timetype = (useskilltype)MyGUI.EnumPopupWithTitle("Time类型:",currdata.timetype);
                    currdata = Timefield(currdata);
                    currdata.postype = (useskilltype)MyGUI.EnumPopupWithTitle("pos类型:", currdata.postype);
                    currdata = Posfield(currdata);
                    currdata.weight = MyGUI.IntFieldWithTitle("权重:", currdata.weight);
                    currdata.roleSkillListFrameEvents = SkillListfield(currdata.roleSkillListFrameEvents);
                    break;
                case RoleSkillListType.random:
                    currdata.timetype = (useskilltype)MyGUI.EnumPopupWithTitle("Time类型:", currdata.timetype);
                    currdata = Timefield(currdata);
                    currdata.postype = (useskilltype)MyGUI.EnumPopupWithTitle("pos类型:", currdata.postype);
                    currdata = Posfield(currdata);
                    currdata.weight = MyGUI.IntFieldWithTitle("权重:", currdata.weight);
                    currdata.roleSkillListFrameEvents = SkillListfield(currdata.roleSkillListFrameEvents);
                    break;
                case RoleSkillListType.sequence:
                    currdata.timetype = (useskilltype)MyGUI.EnumPopupWithTitle("Time类型:", currdata.timetype);
                    currdata = Timefield(currdata);
                    currdata.postype = (useskilltype)MyGUI.EnumPopupWithTitle("pos类型:", currdata.postype);
                    currdata = Posfield(currdata);
                    currdata.weight = MyGUI.IntFieldWithTitle("权重:", currdata.weight);
                    currdata.roleSkillListFrameEvents = SkillListfield(currdata.roleSkillListFrameEvents);
                    break;
                case RoleSkillListType.SkillList:
                    currdata.skillListDatas = SkillListDatalistField(currdata.skillListDatas);
                    break;
            }
            return currdata;
            //});

        }
        public static List<FrameEvent.SkillListData> SkillListDatalistField(List<FrameEvent.SkillListData> skillListDatalist)
        {
            int removeIndex = -1;
            List<FrameEvent.SkillListData> skillListDatas = skillListDatalist;
            //EditorGUILayout.LabelField("AnimEffectList:");
            for (int i = 0; i < skillListDatas.Count; i++)
            {
                EditorGUILayout.BeginVertical(boxStyle);
                skillListDatas[i].id = MyGUI.TextFieldWithTitle("id", skillListDatas[i].id);
                skillListDatas[i].timetype= (useskilltype)MyGUI.EnumPopupWithTitle("Time类型:", skillListDatas[i].timetype);
                skillListDatas[i] = Timefield(skillListDatas[i]);
                skillListDatas[i].postype = (useskilltype)MyGUI.EnumPopupWithTitle("Pos类型:", skillListDatas[i].postype);
                skillListDatas[i] = Posfield(skillListDatas[i]);
                if (Button("-"))
                {
                    removeIndex = i;
                }
                EditorGUILayout.EndVertical();
            }
            if (removeIndex >= 0)
            {
                skillListDatas.RemoveAt(removeIndex);
            }

            if (Button("+"))
            {
                skillListDatas.Add(new FrameEvent.SkillListData());
            }
            return skillListDatas;
        }
        public static RoleSkillListFrameEventData Timefield(RoleSkillListFrameEventData data)
        {
            RoleSkillListFrameEventData currdata = data;
            useskilltype type = currdata.timetype;
            switch (type)
            {
                case useskilltype.fixeda:
                    currdata.time1 = MyGUI.FloatFieldWithTitle("Time:", currdata.time1);
                    break;
                case useskilltype.random:
                    currdata.time1 = MyGUI.FloatFieldWithTitle("MinTime:", currdata.time1);
                    currdata.time2 = MyGUI.FloatFieldWithTitle("MaxTime:", currdata.time2);
                    break;
            }
            return currdata;
        }
        public static FrameEvent.SkillListData Timefield(FrameEvent.SkillListData data)
        {
            FrameEvent.SkillListData currdata = data;
            useskilltype type = currdata.timetype;
            switch (type)
            {
                case useskilltype.fixeda:
                    currdata.time1 = MyGUI.FloatFieldWithTitle("Time:", currdata.time1);
                    break;
                case useskilltype.random:
                    currdata.time1 = MyGUI.FloatFieldWithTitle("MinTime:", currdata.time1);
                    currdata.time2 = MyGUI.FloatFieldWithTitle("MaxTime:", currdata.time2);
                    break;
            }
            return currdata;
        }
        public static RoleSkillListFrameEventData Posfield(RoleSkillListFrameEventData data)
        {
            RoleSkillListFrameEventData currdata = data;
            useskilltype type = data.postype;
            switch (type)
            {
                case useskilltype.fixeda:
                    currdata.pos1 = MyGUI.Vector3WithTitle("Pos:", currdata.pos1);
                    break;
                case useskilltype.random:
                    currdata.pos1 = MyGUI.Vector3WithTitle("MinPos:", currdata.pos1);
                    currdata.pos2 = MyGUI.Vector3WithTitle("MaxPos:", currdata.pos2);
                    break;
            }
            return currdata;
        }
        public static FrameEvent.SkillListData Posfield(FrameEvent.SkillListData data)
        {
            FrameEvent.SkillListData currdata = data;
            useskilltype type = data.postype;
            switch (type)
            {
                case useskilltype.fixeda:
                    currdata.pos1 = MyGUI.Vector3WithTitle("Pos:", currdata.pos1);
                    break;
                case useskilltype.random:
                    currdata.pos1 = MyGUI.Vector3WithTitle("MinPos:", currdata.pos1);
                    currdata.pos2 = MyGUI.Vector3WithTitle("MaxPos:", currdata.pos2);
                    break;
            }
            return currdata;
        }

        // 绘制dictionary add by TangJian 2017/12/07 13:48:30
        // public static void DictionaryField<T>(Dictionary<string, T> dictionary)
        // {
        //     Reflection.Instance.Invoke("Editor", "DoDrawDefaultInspector", null, new object[] { serializedObject });
        // }
    }
}