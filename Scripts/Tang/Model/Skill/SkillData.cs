using System.Collections.Generic;
using UnityEngine;
using Tang.FrameEvent;
using UnityEngine.Serialization;

namespace Tang
{
    public enum SkillType
    {
        FlySkill=0,
        StaticSkill=1,
        CustomSkill = 2 // 自定义技能类型, 完全由脚本决定 add by TangJian 2019/3/14 20:50
    }
    public enum RendererType
    {
        SkeletonAnimator=1,
        Sprite=2,
        Skeleton = 3,
        Anim = 4
    }

    public enum SkillOrientationMode
    {
        Direction = 0,
        Speed = 1,
        Rotation = 2
    }

    [System.Serializable]
    public class SkillData
    {
        public SkillData()
        {
            
        }
        public SkillData(string id)
        {
            this.id = id;
        }
        public SkillData(SkillData frontSkillData,SkillData centreSkillData,SkillData backSkillData)
        {
            this.FrontSkillData = frontSkillData;
            this.CentreSkillData = centreSkillData;
            this.BackSkillData = backSkillData;
        }
        
        public string id; // 技能编号 add by TangJian 2017/08/09 11:40:42
        public string name; // 技能名称 add by TangJian 2017/08/09 11:40:46
        public SkillUseCondition useCondition; // 技能使用条件 add by TangJian 2017/08/09 11:41:12
        public float cd; // 技能cd时间 add by TangJian 2017/08/09 11:40:54   
        public float cost = 1; // 消耗 add by TangJian 2017/12/20 18:02:28
        public float atk = 0;//攻击力
        public float poiseCut = 1;//削韧
        public bool NoSurvivalTime=false;
        public float SurvivalTime=10f;
        public Vector3 speed = new Vector3();
        public Vector3 rotateSpeed = new Vector3();
        public Vector3 pos = new Vector3();
        //public UnityEditor.MonoScript component;
        public SkillType skillType;
        public RendererType rendererType=RendererType.SkeletonAnimator;
        public string SkeletonDataAssetPath;
        public string AnimControllerPath;
        public string componentPath;
        public string componentTypeName;
        public string SkinName;
        public string AnimationName;
        public string tagName = "Trigger";
        public string layerName= "Interaction";
        public string CollidertagName = "Untagged";
        public string ColliderlayerName = "SmallPlacement";
        public TriggerMode triggerMode = TriggerMode.Default;
        public bool withRigidbody = true;
        public bool useGravity;
        public float gravitationalAcceleration = 20;
        public bool onoffDamage = true;
        public bool onoffCollider = true;
        public bool shadow = false;
        public bool onoffTirgger = false;
        public bool onoffFocus = false;
        public float rigidbodymass = 1;
        public float rigidbodydrag = 0.05f;
        public DamageDirectionType DamageDirectionType= DamageDirectionType.Directional;
        public DamageForceType damageForceType = DamageForceType.light;
        public float Intensity;
        public Vector3 angleIntensity = Vector3.zero;
        public Vector3 shadowScale = new Vector3(3, 3, 3);
        public float CutOffDistance=10f;
        public float MaxScaleMultpler = 0.5f;
        public Vector3 focuspos;
        public Vector3 DamageColliderCenter;
        public Vector3 DamageColliderSize = new Vector3(1, 1, 1);
        public Vector3 colliderCenter;
        public Vector3 colliderSize=new Vector3 (1,1,1);
        public Vector3 TirggercolliderCenter;
        public Vector3 TirggercolliderSize = new Vector3(1, 1, 1);
        
        // 攻击材质类型 add by TangJian 2019/6/4 12:19
        public HitEffectType HitEffectType = HitEffectType.swd;
        
        // add by TangJian 2019/4/20 13:22
        public SkillOrientationMode SkillOrientationMode = SkillOrientationMode.Direction;
        
        // 精灵渲染 add by TangJian 2019/4/20 14:33
        public string SpritePath;
        
        public string AnimEffectName_Role;
        public string AnimEffectName_Scene;
        public bool IsJoinAnim;

        public float DelayTime;
        public bool IsRandomTrans;
        public Vector3 MinRandomVector3 = new Vector3();
        public Vector3 MaxRandomVector3 = new Vector3();

        public FrameEventInfo.ParentType parentType;
        public SkillGroupType type;
        public List<SkillData> skillDatas = new List<SkillData>();

        public bool IsMorePlay;
        public float DurationTime;
        public int PlayCount;


        public bool IsPhasesSkill = false;
        public SkillData FrontSkillData;
        public SkillData CentreSkillData;
        public SkillData BackSkillData;
        
        //支持选取动画片段
        public string SkeletonPath;
        public string SkeletonClipName;
        public string AnimName;
        
        
        public float AnimTime;
        public float BeginAlpha = 1;
        public float AnimSelectAlphaTime;
        public float SelectAlpha;
        
        public float BeginScale = 1;
        public float AnimSelectScaleTime;
        public float SelectScale;
        
    }
    [System.Serializable]
    public class SkillListSaveData
    {
        public string id;
        public RoleSkillListFrameEventData roleSkillListFrameEventData = new RoleSkillListFrameEventData();
    }
    
    public enum SkillGroupType
    {
        None = 0,
        Random = 1,
        Sequence = 2
    }

    public enum SkillPhasesType
    {
        None = -1,
        Front = 0,
        Centre = 1,
        Back = 2
    }
}