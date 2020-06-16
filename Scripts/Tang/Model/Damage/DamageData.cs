using System;
using System.Collections.Generic;
using UnityEngine;
using ZS;

namespace Tang
{
    using FrameEvent;

    [System.Serializable]
    public partial class DamageData : System.ICloneable, HitAble
    {
        public RecordType recordType = RecordType.NotRecord;
        public DamageDirectionType DamageDirectionType = DamageDirectionType.Directional;
        public DamageForceType forceType = DamageForceType.light;
        [NonSerialized] public GameObject owner;
        [NonSerialized] public GameObject hitGameObject;
        [NonSerialized] public string WeaknessName;
        public string DamageId;
        public string teamId = "2";
        [NonSerialized] public ITriggerDelegate itriggerDelegate;

        public float poiseCut = 0; // 削弱韧性 add by TangJian 2018/12/12 12:22
        public float DamageMass = 0;

        public float atk = 0;
        
        public float magical;
        public bool isCritical = false;
        public Vector3 direction; // 伤害方向
        public Vector3 force; // 伤害力度
        public FrameEventInfo.CameraShakeType weaponEquipType = FrameEventInfo.CameraShakeType.None;//震动类型 add by tianjinpeng 2018/07/18 11:56:29
        public float SpecialEffectRotation = 0f; //攻击特效方向 add by tianjinpeng 2018/07/11 16:49:11
        public Vector3 SpecialEffectPos = new Vector3(0, 0, 0);//攻击特效位置偏移 add by tianjinpeng 2018/07/11 16:49:11
        public float SpecialEffectScale = 1f;//攻击特效缩放 add by tianjinpeng 2018/07/11 16:49:11
        public Vector3 targetMoveBy; // 对象位移 add by TangJian 2017/08/29 22:10:08
        public Vector3 targetMoveTo; // 对象位移 add by TangJian 2017/08/29 22:10:08
        public AtkPropertyType atkPropertyType = AtkPropertyType.physicalDamage;//攻击属性类型
        public int hitType;
        public WeaponType WeapondeadType = 0;//对应武器死亡动画
        public DamageEffectType damageEffectType = DamageEffectType.Strike;
        [NonSerialized] public GameObject hitTarget = null;
        [NonSerialized] public Bounds collideBounds;
        [NonSerialized] public Vector3 collidePoint;
        public float hurtHoldAnimSpeedScale = 1;
        public int damageTimes = 1;
        public float damageInterval = 0;

        public bool useForcedOffset = false; // 是否使用强制位移 add by TangJian 2019/5/5 15:36
        public Vector3 forcedOffset = Vector3.zero; // 强制位移 add by TangJian 2019/4/30 10:26
        public float forcedOffsetDuration = 0;
        
        // 特效方向 add by TangJian 2019/5/9 16:37
        public Vector3 effectOrientation = new Vector3(1, 0, 0);
        
        // 破盾 add by TangJian 2019/4/3 16:57
        public bool breakShield = false;
        public bool ignoreShield = false;
        
        public delegate void OnHitDelegate(DamageData damageData);
        public event OnHitDelegate OnDamageComplete;
        
        // 顿帧时间 add by TangJian 2019/5/10 17:31
        public float selfSuspendTime = 0;
        public float selfSuspendScale = 1;

        // 力度值
        public float forceValue = 0;
        
        // 队友击飞
        public bool DiaupTeammate = true;

        public List<int> ignoreObjectList = new List<int>();
        
        public DamageData()
        {
            DamageDirectionType = DamageDirectionType.Directional;
            atk = 1;
            direction = new Vector3(1, 0, 0);
            force = new Vector3(10, 0, 0);
            hitType = 2;
        }

        public object Clone()
        {
            var p = Tools.DepthClone(this);
            p.itriggerDelegate = this.itriggerDelegate;
            p.owner = this.owner;
            p.hitTarget = hitTarget;
            p.collideBounds = this.collideBounds;
            return p;
        }
    }

    public partial class DamageData
    {
        public HurtPart HurtPart;
        
        public HitEffectType HitEffectType = HitEffectType.Other;
        
        public HitEffectType GetHitEffectType()
        {
            return HitEffectType;
        }

        public bool OnHit(DamageData damageData)
        {
            return true;
        }
    }
}