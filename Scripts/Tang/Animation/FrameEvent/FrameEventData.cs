using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using Spine.Unity;
using UnityEngine.Serialization;

namespace Tang.FrameEvent
{

    public class FrameEventInfo
    {
        public enum MovetoTagretType
        {
            Tagret = 0,
            Postion = 1,
            Forward = 2
        }
        public enum MovetoTagretAnimType
        {
            speed = 1,
            time = 2
        }
        public enum AnimspeedType
        {
            setAnimspeed = 0,
            Remove = 1,
        }
        public enum FrameEventType
        {
            Custom = 0,

            RoleAtk = 1,

            RoleSkill = 2,

            PlayAnim = 3,
            VariableSpeed = 4,
            CameraShake = 5,
            PlayAnimList = 6,
            RoleSkillList = 12,

            SetSpeed = 7,
            PlayAnimEffect = 8,
            SetAnimSpeed = 9,
            MoveToTarget = 10,
            RoleAiFlyTo=13,

            SuperArmor = 14,
            
            RoleQTE = 15,
            
            AnimProperty = 16,
            
            PlayAudio = 17,
            
            OnTread = 18
        }
        public enum CameraShakeType
        {
            None = 0,
            LswdCameraShakefierce = 1,
            SwdCameraShakefierce = 2,
            LswdCameraShake = 3,
            SwdCameraShake = 4,
            CameraShake = 5,
            smallshape = 6,
            mediumshape = 7,
            bigshape = 8
        }
        public enum ParentType
        {
            Parent = 0,
            Transform = 1
        }

        public class AnimProperty : Tang.MyObject
        {
            public float speed = 1;
        }

        #region //ZwriteType 2019.3.28

        public enum ZwriteType
        {
            On =0,
            Off =1
        }
         
        public enum RenderQueue
        {
            Geometry = 0,
            AlphaTest = 1,
            Transparent = 2
            
        }

        #endregion
        
        [Serializable]
        public class RoleAtkFrameEventData : Tang.MyObject
        {

            public enum Type
            {
                Default = 0,
                Add = 1,
                Remove = 2
            }

            public enum BindType
            {
                Default = 0,
                BindAnimSlot = 1,
                BindObjectController = 2,
            }

            public Type type = Type.Default;
            public ParentType parentType = ParentType.Transform;
            public PrimitiveType primitiveType = PrimitiveType.Cube;

            public bool useCustomFrameEventBatch = false;

            public string id = "atk";
            public Vector3 pos = new Vector3(0, 0, 0); // 位置 add by TangJian 2017/07/24 15:13:54
            public Vector3 size = new Vector3(1, 1, 1); // 大小 add by TangJian 2017/07/24 15:13:48
            public Vector3 force = new Vector3(1, 0, 0); // 攻击力道 add by TangJian 2017/07/24 15:18:05
            public Tang.DamageDirectionType DamageDirectionType = DamageDirectionType.Directional; // 攻击方向类型 add by TangJian 2018/12/19 16:32
            public Tang.RecordType recordType = Tang.RecordType.NotRecord; //DamageDataRecordType
            public Tang.DamageForceType damageForceType = Tang.DamageForceType.light; //攻击力度类型 
            public bool SpecialEffectOnOff = false; //攻击特效开关开启特效以帧事件为准 add by tianjinpeng 2018/07/16 17:07:16
            public Tang.DamageEffectType damageEffectType = Tang.DamageEffectType.Slash;//攻击特效类型 add by tianjinpeng 2018/07/16 17:11:13
            public float SpecialEffectRotation = 0f; //攻击特效方向 add by tianjinpeng 2018/07/11 16:49:11
            public Vector3 SpecialEffectPos = new Vector3(0, 0, 0);//攻击特效位置 add by tianjinpeng 2018/07/11 16:49:11
            public float SpecialEffectScale = 1f; //攻击特效缩放 add by tianjinpeng 2018/07/11 16:49:11
            public CameraShakeType cameraShakeType = CameraShakeType.None; //camera震动特效 add by tianjinpeng 2018/07/18 13:18:07
            public CameraShakeType cameraShakeTypeCritical = CameraShakeType.None; //camera震动特效(暴击) add by tianjinpeng 2018/07/18 13:18:07
            public Vector3 targetMoveBy = new Vector3(0, 0, 0);
            public Tang.Direction direction = Tang.Direction.Right; // 攻击方向 add by TangJian 2017/07/24 15:29:10
            public Tang.HitType hitType = Tang.HitType.Down; // 攻击类型 add by TangJian 2017/07/24 15:29:17
            public float atk = 1; // 攻击力道 add by TangJian 2017/07/24 15:29:56
            public float tiliCost = 0; // 体力消耗 add by TangJian 2018/01/17 15:09:46
            public float hurtHoldAnimSpeedScale = 1.5f;
            public float poiseCut = 1; // 削韧性 add by TangJian 2018/12/12 12:25
            
            // 破盾 add by TangJian 2019/5/5 14:51
            public bool breakShield = false;
            
            // 无视盾牌 add by TangJian 2019/5/5 14:52
            public bool ignoreShield = false;
            
            // 攻击属性类型开关
            public bool atkPropertyTypeOnOff = false;
            
            // 攻击属性类型
            public Tang.AtkPropertyType atkPropertyType = Tang.AtkPropertyType.physicalDamage;
            
            // 是否开启强制位移 add by TangJian 2019/5/5 14:56
            public bool useForceOffset = false;
            
            // 强制位移 add by TangJian 2019/5/5 14:54
            public Vector3 targetForcedOffset = Vector3.zero;
            
            // 强制位移时间 add by TangJian 2019/5/5 14:55
            public float targetForcedOffsetDuration = 0; 
            
            // 攻击减速时间 add by TangJian 2019/5/5 14:53
            public float suspendTime = 0;
            
            // 攻击减速得速度 add by TangJian 2019/5/5 14:53
            public float suspendScale = 1;
            
            // 伤害区域绑定 add by TangJian 2019/5/5 14:52
            public BindType bindType = BindType.Default;
            public List<string> slotList = new List<string>();
            
            
            // 特效朝向 add by TangJian 2019/5/9 15:24
            public Vector3 effectOrientation = new Vector3(1, 0, 0);
            
            // 力度
            public float forceValue = 0;
        }
        
        [Serializable]
        public class RoleQTE : Tang.MyObject
        {
            public enum QteType
            {
                Begin = 1,
                Hit = 2,
                End = 3,
                Break = 4,
                LastHit = 5,
            }

            public QteType qteType = QteType.Begin;
            public string id;
            public Vector3 pos = new Vector3(0, 0, 0);
            public Vector3 size = new Vector3(1, 1, 1);
            public float atk = 1;
        }

        public class RoleSkillFrameEventData : Tang.MyObject
        {
            // 技能行为类型 add by TangJian 2018/12/10 16:08
            public enum SkillActionType
            {
                Default = 0,       // 默认, 啥也不处理
                FlyToTarget = 2,   // 飞到目标
                TowardToTarget = 3,   // 朝向目标
            }
            
            public string skillId = "fireball";
            public Vector3 pos = Vector3.zero;
            public Vector3 orientation = Vector3.zero;
            public float atk = 1;
            public float tiliCost = 10;
            public SkillActionType skillActionType = SkillActionType.Default;
            public bool usePointPosition = false;
            public Vector3 angleYMax = Vector3.zero;
            public string teamId = null;
        }
        
        public class OnSkillGroup : Tang.MyObject
        {
            
        }
        public class VariableRoleMoveSpeedData : Tang.MyObject
        {
            public enum Variableeuem
            {
                Add = 0,
                Remove = 1,
                clear = 2
            }
            // public string VariableSpeedID = "";
            public Variableeuem VariableType = Variableeuem.Add;
            public float speed = 1f;
            public float Duration = 0f;

        }


        public class PlayAudio : Tang.MyObject
        {
            public ParentType ParentType = ParentType.Transform;
            public string AudioName;
            public Vector3 Pos = Vector3.zero;
        }

        public class PlayAnimFrameEventData : Tang.MyObject
        {
            public string animId = "";
            public ParentType parentType = ParentType.Transform;
            public SceneDecorationPosition sceneDecorationPosition = SceneDecorationPosition.Positive;
            public Vector3 pos = Vector3.zero;
            public Vector3 orientation = Vector3.zero;
            public string AnimatedFragment = "";
            public Vector3 scale = new Vector3(1, 1, 1);
            public Tang.SpeedType animspeedtype = Tang.SpeedType.Fixedanimatorspeed;
            public float animspeed = 1f;
        }

        public class PlayAnimEffectFrameEventData : Tang.MyObject
        {
            public string id;
            public ParentType parentType = ParentType.Transform;
            public Vector3 pos = Vector3.zero;
            public Vector3 orientation = Vector3.zero;
        }

        public class CustomFrameEventData : Tang.MyObject
        {
            public enum EventType
            {
                HoveringBegin = 1,
                HoveringEnd = 2,
                CameraShake = 3,

                DefendBegin = 4,
                DefendEnd = 5,

                NoHurtBegin = 6,
                NoHurtEnd = 7,
                MoveToTarget = 8,

                OrientateToTarget = 9,

                AddCustomFrameEventBatchId = 10,
                SetJoystickDirection=11,
                RoleAiFlyTo=12,
                
                DestroySelf = 13
            }

            public EventType eventType;
            public string id;
        }
        public class FrameEventAnimList : Tang.MyObject
        {
            public List<AnimListFrameEventData> AnimList = new List<AnimListFrameEventData>();
            public Vector3 pos = Vector3.zero;
            public Vector3 scale = new Vector3(1, 1, 1);
        }

        public class SetSpeedFrameEvent : Tang.MyObject
        {
            public Vector3 speed = Vector3.zero;
            public bool x = true;
            public bool y = true;
            public bool z = true;
        }
        public class SetAnimSpeedFrameEvent : Tang.MyObject
        {
            public float speed = 1;
            public AnimspeedType animspeedType = AnimspeedType.setAnimspeed;
        }
        public class MoveToTargetFrameEvent : Tang.MyObject
        {
            public MovetoTagretType movetoTagretType = MovetoTagretType.Tagret;

            public float distance = 1;
            public float time;
            
            public MovetoTagretAnimType movetoTagretAnimType = MovetoTagretAnimType.time;
            public DG.Tweening.Ease ease;
        }
        public class RoleAiFlyToFrameEventData : Tang.MyObject
        {
            public Vector3 RangeMax=new Vector3();
            public float xyangle=45f;
            public float xzangle = 45f;
            public Vector3 targetDeviation = new Vector3();
            public float GrivityAcceleration = 30f;
        }
        public class CameraShakeFrameEvent : Tang.MyObject
        {
            public CameraShakeType cameraShakeType;
            public float time;
        }

        // 霸体帧事件 add by TangJian 2018/12/12 13:35
        public class SuperArmorFrameEvent : Tang.MyObject
        {
            public enum OperationType
            {
                Add = 0,
                Remove = 1
            }

            public OperationType operationType = OperationType.Add;
            public string id;
            public float poiseScale = 2;
        }
        public class RoleSkillListFrameEvent : Tang.MyObject
        {
            public string id = "";
        }
        
        public class OnTread : Tang.MyObject//踩地
        {
            public string animName;
            public WalkType walkType;
            public Vector3 animVector3;
        };
    }

    public enum AnimListFrameEventType
    {
        Random = 1,

        Fixed = 2
    }
    [Serializable]
    public class RoleSkillListFrameEventData
    {
        public RoleSkillListType RoleSkillListType = RoleSkillListType.sequence;
        public List<RoleSkillListFrameEventData> roleSkillListFrameEvents = new List<RoleSkillListFrameEventData>();
        public List<SkillListData> skillListDatas = new List<SkillListData>();
        public useskilltype timetype = useskilltype.fixeda;
        public float time1 = 0f;
        public float time2 = 0f;
        public useskilltype postype = useskilltype.fixeda;
        public Vector3 pos1 = new Vector3();
        public Vector3 pos2 = new Vector3();
        public int weight;
    }
    [Serializable]
    public class AnimListFrameEventData
    {
        public string Animpath = "";
        public string AnimatedFragment = "";
        public int weight = 1;
        public bool datapos = false;
        public bool datascale = false;
        public Vector3 pos = Vector3.zero;
        public Vector3 scale = new Vector3(1, 1, 1);
        public float AnimRotation = 0f;
        public AnimListFrameEventType type = AnimListFrameEventType.Fixed;
        public float float2 = 0f;
        public FrameEventInfo.ParentType parentType = FrameEventInfo.ParentType.Transform;

    }
    public enum useskilltype
    {
        random = 1,
        fixeda = 2,
    }
    public enum RoleSkillListType
    {
        sequence = 1,
        parallel = 2,
        random = 3,
        SkillList = 4,
    }

    [Serializable]
    public class SkillListData
    {
        public string id;
        public useskilltype timetype = useskilltype.fixeda;
        public float time1 = 0f;
        public float time2 = 0f;
        public useskilltype postype = useskilltype.fixeda;
        public Vector3 pos1 = new Vector3();
        public Vector3 pos2 = new Vector3();
    }

    [Serializable]
    public class FrameEventData
    {
        public FrameEventInfo.FrameEventType frameEventType = FrameEventInfo.FrameEventType.Custom;

        public string id; // 帧事件唯一标识 add by TangJian 2018/04/13 11:37:30

        public String name;
        public float time;
        public String type;
        public float Float;
        public int Int;
        public String String;
        public JObject EventData;

        public bool forceExecute = false;

        public FrameEventData Copy()
        {
            return Tools.Obj2JObj(this, true).ToObject<FrameEventData>();
//            return Tang.Tools.DepthClone<FrameEventData>(this); //万能深拷贝 add by TangJian 2018/03/26 15:44:15s
        }

        public T GetData<T>() where T : class
        {
            T ret = EventData?.ToObject<T>();
            return ret == null ? Tang.Tools.Json2Obj<T>(String) : ret;
        }
    }

    public enum DataType
    {
        Int = 0,
        Float,
        String
    }

    public class JsonFileTools
    {
        static object mutexReadAndWrite = new object();

        public JsonFileTools() { }

        public static Dictionary<String, List<FrameEventData>> ReadEventJsonData(String path)
        {
            lock (mutexReadAndWrite)
            {
                try
                {
                    if (File.Exists(path)) //如果有这个文件 
                    {
                        string jsonString = Tang.Tools.ReadStringFromFile(path);
                        Dictionary<String, List<FrameEventData>> root = Tang.Tools.Json2Obj<Dictionary<String, List<FrameEventData>>>(jsonString);
                        return root;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            return new Dictionary<String, List<FrameEventData>>();
        }

        public static string ReadEventJsonString(String path)
        {
            lock (mutexReadAndWrite)
            {
                try
                {
                    if (File.Exists(path)) //如果有这个文件 
                    {
                        string jsonString = Tang.Tools.ReadStringFromFile(path);
                        return jsonString;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            return null;
        }


        public static Dictionary<String, List<FrameEventData>> ReadFrameEventDataFromString(string str)
        {
            try
            {
                string jsonString = str;
                Dictionary<String, List<FrameEventData>> root = Tang.Tools.Json2Obj<Dictionary<String, List<FrameEventData>>>(jsonString);
                return root;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        public static string GetAniEventPath(SkeletonDataAsset skeletonDataAsset)
        {
#if UNITY_EDITOR
            string filepath = AssetDatabase.GetAssetPath(skeletonDataAsset).Replace("\\", "/").Replace("//", "/");
            return filepath.Replace("_SkeletonData", "_EventData").Replace(".asset", ".json");
#else
        return null;
#endif
        }

        public static string saveRoot2JsonFile(Dictionary<String, List<FrameEventData>> root, String aniEventPath)
        {
            lock (mutexReadAndWrite)
            {
                string s = Tang.Tools.Obj2Json<Dictionary<String, List<FrameEventData>>>(root);
                Tang.Tools.WriteStringFromFile(aniEventPath, s);
                return s;
            }
        }
    }
}