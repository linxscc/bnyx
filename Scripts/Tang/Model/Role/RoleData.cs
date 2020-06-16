using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Newtonsoft.Json;


namespace Tang
{
    [JsonObject(MemberSerialization.Fields)]
    [System.Serializable]
    public class RoleData : GameObjectData
    {
        public RoleData()
        {
            attrData.hpMax = 100; // 总血量 add by TangJian 2017/10/24 16:05:42
            attrData.hp = 100; // 当前血量 add by TangJian 2017/10/24 16:05:30

            attrData.mpMax = 100;
            attrData.mp = 100;

            attrData.tiliMax = 100;
            attrData.tili = 100;

            attrData.atk = 5; // 攻击力 add by TangJian 2017/10/24 16:05:20
            attrData.def = 1; // 防御力 add by TangJian 2017/10/24 16:05:15

//            attrData.atkSpeed = 1; // 攻击速度 add by TangJian 2017/10/24 16:05:11
//            attrData.moveSpeed = 1; // 移动速度 add by TangJian 2017/10/24 16:05:08

            attrData.criticalRate = 0.1f; // 暴击率 add by TangJian 2017/12/20 21:51:30
            attrData.criticalDamage = 1.5f; // 暴击率 add by TangJian 2017/12/20 21:51:30

            attrData.atkSpeedScale = 1;
            attrData.moveSpeedScale = 1;

            attrData.walkSpeed = 5f;
            attrData.runSpeed = 10f;

            attrData.jumpSpeed = 10f;

            // 跳跃次数 add by TangJian 2018/12/12 12:27
            attrData.jumpTimes = 1;

            // 韧性 add by TangJian 2018/12/12 12:28
            attrData.poise = 100;
            attrData.poiseMax = 100;
            attrData.poiseScale = 1;

            attrData.poiseCut = 50;

            attrData.vigorMax = 100;
            attrData.vigor = attrData.vigorMax;

            attrData.strength = 0;
            attrData.weight = 0;
            attrData.lessenhurt = 0;
        }

        [SerializeField] private RoleType roleType = RoleType.None; // 角色类型 add by TangJian 2017/10/24 16:05:55
        [SerializeField] private string id; // 编号 add by TangJian 2017/10/24 16:05:50

        [SerializeField] private string prefab; // 预制体名 add by TangJian 2018/05/17 15:41:01
        public string Prefab { get { return prefab; } set { prefab = value; } }

        [SerializeField] private AttrData attrData = new AttrData();
        public AttrData AttrData { get { return attrData; } set { attrData = value; } }

        [SerializeField] private float damping = 1;
        public float Damping
        {
            get => damping;
            set => damping = value;
        }

        [SerializeField] private Vector3 defendBoundsize = new Vector3();
        [SerializeField] private Vector3 defendBoundcenter = new Vector3();

        public Vector3 DefendBoundsize { get { return defendBoundsize; } set { defendBoundsize = value; } }
        public Vector3 DefendBoundcenter { get { return defendBoundcenter; } set { defendBoundcenter = value; } }
        [SerializeField] private RoleEquipData equipData = new RoleEquipData(); // 角色装备数据 add by TangJian 2017/08/28 21:04:57
        [SerializeField] private RoleSkillData skillData = new RoleSkillData(); // 角色技能数据 add by TangJian 2017/08/28 21:05:18
        [SerializeField] private RoleItemData itemData = new RoleItemData(); // 角色物品数据 add by TangJian 2017/10/24 15:25:25
        [SerializeField] private RoleBuffData roleBuffData = new RoleBuffData(); // 角色增益 add by TangJian 2017/11/20 20:53:4
        [SerializeField] private RoleLooksData looksData = new RoleLooksData(); // 角色技能数据 add by wwl
        [SerializeField] private string teamId = "1"; // 队伍id add by TangJian 2017/10/24 16:04:58

        [SerializeField] private int currConsumableItemIndex;
        public int CurrConsumableItemIndex { get { return currConsumableItemIndex; } set { currConsumableItemIndex = value; } }

        [SerializeField] private List<ConsumableData> consumableItemList = new List<ConsumableData>();
        public List<ConsumableData> ConsumableItemList { get { return consumableItemList; } }

        [SerializeField] private List<ItemData> otherItemList = new List<ItemData>();
        public List<ItemData> OtherItemList { get { return otherItemList; } set { otherItemList = value; } }

        [SerializeField] private List<DropItem> dropItemList;
        public List<DropItem> DropItemList { get { return dropItemList; } set { dropItemList = value; } }

        //弱点列表
        [SerializeField] private List<WeaknessData> weaknessdataList;
        //受击区域名字列表
        [SerializeField] private List<string> damageTargetNameList =new List<string>();
        public List<string> DamageTargetNameList { get { return damageTargetNameList; } set { damageTargetNameList = value; } }
        public List<WeaknessData> WeaknessDataList { get { return weaknessdataList; } set { weaknessdataList = value; } }

        // 难度等级 add by TangJian 2019/3/7 14:33
        public int difficultyLevel = 1;
        
        // 角色经验值 add by TangJian 2019/3/6 21:59
        [NonSerialized] public int exp = 0;
        
        // 角色等级 add by TangJian 2019/3/6 22:00
        [NonSerialized] public int level = 1;
        
        // 钱 add by TangJian 2018/01/22 15:34:55        
        [SerializeField] private int money = 0;
        public int Money { get { return money; } set { money = value; } }
        
        // 攻击效果类型 
        public HitEffectType HitEffectType {
            get
            {
                if (Id == "Human")
                {
                    return (HitEffectType)equipData.getMainHandWeaponType();        
                }
                Debug.Log(AttrData.hitEffectType);
                return AttrData.hitEffectType;
            }
        }

        public MatType MatType => attrData.matType;

        // 魂 add by TangJian 2018/01/22 15:34:46
        [SerializeField] private int soul = 0;
        public int Soul { get { return soul; } set { soul = value; } }

        public string Id { get { return id; } set { id = value; } }
        public float HpMax { get { return attrData.hpMax; } set { attrData.hpMax = value; } }

        public float Hp
        {
            get
            {
                return attrData.hp;
            }
            set
            {
                attrData.hp = value.Range(0, FinalHpMax);
            }
        }

        public float HpPercent { get { return (attrData.hp / FinalHpMax) * 100; } }

        public float MpMax { get { return attrData.mpMax; } set { attrData.mpMax = value; } }
        public float Mp { get { return attrData.mp; } set { attrData.mp = value; } }
        public float MpPercent { get { return (attrData.mp / attrData.mpMax) * 100; } }

        public float TiliMax { get { return attrData.tiliMax; } set { attrData.tiliMax = value; } }
        public float Tili { get { return attrData.tili; } set { attrData.tili = value; } }
        public float TiliPercent { get { return (attrData.tili / FinalTiliMax) * 100; } }

        public float PoisePercent { get { return (FinalPoise / FinalPoiseMax) * 100; } }

        public float Atk { get { return attrData.atk; } set { attrData.atk = value; } }
        public float AtkMin { get { return attrData.atkMin; } set { attrData.atkMin = value; } }
        public float AtkMax { get { return attrData.atkMax; } set { attrData.atkMax = value; } }
        public float MagicMin { get { return attrData.MagicMin; } set { attrData.MagicMin = value; } }
        public float MagicMax { get { return attrData.MagicMax; } set { attrData.MagicMax = value; } }
        public float MagicalDef { get { return attrData.magicalDef; } set { attrData.magicalDef = value; } }
        public float PhysicalDef { get { return attrData.physicalDef; } set { attrData.physicalDef = value; } }
        public float Def { get { return attrData.def; } set { attrData.def = value; } }

        public float AtkSpeedScale { get { return attrData.atkSpeedScale; } set { attrData.atkSpeedScale = value; } }
        public float MoveSpeedScale { get { return attrData.moveSpeedScale; } set { attrData.moveSpeedScale = value; } }

        public float RunSpeed { get { return attrData.runSpeed; } set { attrData.runSpeed = value; } }
        public float RushSpeed {
            get { return attrData.rushSpeed; }
        }
        public float WalkSpeed { get { return attrData.walkSpeed; } set { attrData.walkSpeed = value; } }
        public float JumpSpeed { get => attrData.jumpSpeed * WeightAndStrengthSpeedEffectFactor;
            set => attrData.jumpSpeed = value;
        }

        public float RepellingResistance { get { return attrData.RepellingResistance; } set { attrData.RepellingResistance = value; } }
        public float DefenseRepellingResistance { get { return attrData.DefenseRepellingResistance; } set { attrData.DefenseRepellingResistance = value; } }
        public float RoleMass { get { return attrData.mass; } set { attrData.mass = value; } }

        public string TeamId { get { return teamId; } set { teamId = value; } }

        public float CriticalRate { get { return attrData.criticalRate; } set { attrData.criticalRate = value; } }
        public float CriticalDamage { get { return attrData.criticalDamage; } set { attrData.criticalDamage = value; } }
        //跳跃次数
        public int jumpTimes { get { return attrData.jumpTimes; } set { attrData.jumpTimes = value; } }
        // 攻击属性类型
        [SerializeField] private AtkPropertyType atkpropertytype = AtkPropertyType.physicalDamage;
        public AtkPropertyType atkPropertyType { get { return atkpropertytype; } set { atkpropertytype = value; } }
        [SerializeField] private bool collideWithRole = false;
        public bool CollideWithRole { get { return collideWithRole; } set { collideWithRole = value; } }
        [SerializeField] private bool atklightforcetype = false;
        [SerializeField] private bool atkheavyforcetype = false;
        [SerializeField] private bool atkmoderateforcetype = false;
        public bool Atklightforcetype { get { return atklightforcetype; } set { atklightforcetype = value; } }
        public bool Atkheavyforcetype { get { return atkheavyforcetype; } set { atkheavyforcetype = value; } }
        public bool Atkmoderateforcetype { get { return atkmoderateforcetype; } set { atkmoderateforcetype = value; } }
        // 击退距离
        [SerializeField] private bool needTurnBack = true;
        public bool NeedTurnBack { get { return needTurnBack; } set { needTurnBack = value; } }
        //顿帧停顿时间
        [SerializeField] private float Pausetime = 0f;
        public float PauseTime { get { return Pausetime; } set { Pausetime = value; } }
        // 是否僵直 add by TangJian 2018/05/14 20:15:34
        public bool IsSuperArmor
        {
            get
            {
                return this.GetFinalAttr(AttrType.SuperArmor) >= 1.0f;
            }
        }
        
        // 最终属性 add by TangJian 2018/01/18 16:00:26
        public float FinalHpMax { get { return this.GetFinalAttr(AttrType.HpMax); } }
        public float FinalHp { 
            get => Hp;
            set => Hp = value;
        }
        public float FinalMpMax { get { return this.GetFinalAttr(AttrType.MpMax); } }
        public float FinalMp { get { return Mp; } }
        public float FinalTiliMax { get { return this.GetFinalAttr(AttrType.TiliMax); } }
        public float FinalTili { get { return Tili; } }
        
        public float FinalStrength => this.GetFinalAttr(AttrType.Strength);
        public float FinalWeight => this.GetFinalAttr(AttrType.Weight);

        public float FinalLessenHurt => this.GetFinalAttr(AttrType.LessenHurt);
         
        public float FinalVigor
        {
            get => attrData.vigor;
            set => attrData.vigor = value;
        }
        public float FinalVigorMax => this.GetFinalAttr(AttrType.VigorMax);
        
        public float FinalAtk => this.GetFinalAttr(AttrType.Atk) + FinalStrength / 2f;
        public float FinalDef => this.GetFinalAttr(AttrType.Def);
        public float FinalCriticalRate => this.GetFinalAttr(AttrType.CriticalRate);
        public float FinalCriticalDamage => this.GetFinalAttr(AttrType.CriticalDamage);
        public float FinalMass => this.GetFinalAttr(AttrType.Mass);
        public float FinalAtkSpeed => this.GetFinalAttr(AttrType.AtkSpeedScale).Range(0.1f, 2f);
        
        // 重量和力量对速度额的影响系数
        public float WeightAndStrengthSpeedEffectFactor =>
            FinalStrength >= FinalWeight ? 1 : (1f - (FinalWeight - FinalStrength) * 0.001f);
        
        public float FinalMoveSpeedScale => this.GetFinalAttr(AttrType.MoveSpeedScale) * WeightAndStrengthSpeedEffectFactor;

        public float FinalWalkSpeed => this.GetFinalAttr(AttrType.WalkSpeed) * FinalMoveSpeedScale;

        public float FinalRunSpeed => this.GetFinalAttr(AttrType.RunSpeed) * FinalMoveSpeedScale;

        public float FinalRushSpeed => this.GetFinalAttr(AttrType.RushSpeed) * FinalMoveSpeedScale;

        public float FinalPoise
        {
            get => attrData.poise * this.GetFinalAttr(AttrType.PoiseScale);

            set => attrData.poise = value / this.GetFinalAttr(AttrType.PoiseScale);
        }
        public float FinalPoiseMax { get { return this.GetFinalAttr(AttrType.PoiseMax) * this.GetFinalAttr(AttrType.PoiseScale); } }

        public float FinalPoiseScale { get { return this.GetFinalAttr(AttrType.PoiseScale); } }


        public float FinalPoiseCut { get { return this.GetFinalAttr(AttrType.PoiseCut); } }


        public float FinalMainHandAtk
        {
            get
            {
                float value = 0;
                value += FinalAtk;


                // if (equipData.mainHand != null)
                // {
                //     value += equipData.mainHand.GetAttr(AttrType.Atk);
                // }
                return value;
            }
        }

        public float FinalOffHandAtk
        {
            get
            {
                float value = 0;
                value += FinalAtk;


                // if (equipData.offHand != null)
                // {
                //     value += equipData.offHand.GetAttr(AttrType.Atk);
                // }
                return value;
            }
        }

        public RoleEquipData EquipData { get { return equipData; } set { equipData = value; } }
        public RoleSkillData SkillData { get { return skillData; } set { skillData = value; } }
        public RoleItemData ItemData { get { return itemData; } set { itemData = value; } }

        public RoleBuffData RoleBuffData { get { return roleBuffData; } set { roleBuffData = value; } }
        
        
        // 跌落选项
        public FallInOption FallInOption = FallInOption.Dying;

        // 跌落高度
        public float FallInHeight = -10;
    }


    public static class RoleDataConsumableItemExtendMethod
    {
        public static bool ContainsConsumableItem(this RoleData target, string id)
        {
            return target.ConsumableItemList.Exists((ConsumableData consumableData) =>
            {
                if (consumableData.id == id)
                    return true;
                return false;
            });
        }

        public static int GetConsumableItemIndex(this RoleData target, string id)
        {
            return target.ConsumableItemList.FindIndex((ConsumableData consumableData) =>
            {
                if (consumableData.id == id)
                    return true;
                return false;
            });
        }

        public static bool TryGetConsumableItem(this RoleData target, string id, out ConsumableData consumableDataOut)
        {

            consumableDataOut = target.ConsumableItemList.Find((ConsumableData consumableData) =>
            {
                if (consumableData.id == id)
                    return true;
                return false;
            });

            if (consumableDataOut == null)
                return false;
            return true;
        }

        public static void AddConsumableItem(this RoleData target, ConsumableData consumableData)
        {
            Debug.Assert(consumableData != null, "AddConsumableItem consumableData == null");
            target.AddConsumableItem(consumableData.id, consumableData.count);
        }

        public static void AddConsumableItem(this RoleData target, string id, int count = 1)
        {
            ConsumableData addConsumableItem;
            if (target.TryGetConsumableItem(id, out addConsumableItem))
            {
                addConsumableItem.count += count;
            }
            else
            {
                ConsumableData consumableData = ItemManager.Instance.getItemDataById<ConsumableData>(id);
                consumableData.count = count;
                target.ConsumableItemList.Add(consumableData);
            }
        }

        public static void SubConsumableItem(this RoleData target, string id, int count = 1)
        {
            ConsumableData addConsumableItem;
            if (target.TryGetConsumableItem(id, out addConsumableItem))
            {
                if (addConsumableItem.count > 1)
                {
                    addConsumableItem.count -= count;
                }
                else
                {
                    target.RemoveConsumableItem(id);
                }

                // if (addConsumableItem.count <= 1)
                // {

                // }
            }
            else
            {
                Debug.LogError("SubConsumableItem 异常");
            }
        }

        public static void RemoveConsumableItem(this RoleData target, string id)
        {
            int index = target.GetConsumableItemIndex(id);
            if (index >= 0)
            {
                target.ConsumableItemList.RemoveAt(index);
            }
            else
            {
                Debug.Log("ConsumableItemList 不存在 id = " + id);
            }
        }
        public static WeaknessData GetWeakness(this RoleData target, int index)
        {
            WeaknessData weaknessData;
            if (target.WeaknessDataList.TryGet(index, out weaknessData))
            {
                return weaknessData;
            }
            return null;
        }
        public static ConsumableData GetConsumableItem(this RoleData target, int index)
        {
            ConsumableData retConsumableData;
            if (target.ConsumableItemList.TryGet(index, out retConsumableData))
            {
                return retConsumableData;
            }
            return null;
        }

        public static ConsumableData GetCurrConsumableItem(this RoleData target)
        {
            return target.GetConsumableItem(target.CurrConsumableItemIndex);
        }

        public static void RemoveOtherItem(this RoleData target, string id, int count = 1)
        {
            target.OtherItemList.RemoveItemWithId(id, count);
        }

        public static void RemoveOtherItemWithIndex(this RoleData target, int index, int count = 1)
        {
            target.OtherItemList.RemoveItemWithIndex(index, count);
        }
    }
}