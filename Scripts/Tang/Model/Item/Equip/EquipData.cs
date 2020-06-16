namespace Tang
{
    public static class AttrDataExtendMethod
    {
        public static float GetAttr(this AttrData target, AttrType attrType)
        {
            switch (attrType)
            {
                case AttrType.HpMax:
                    return target.hpMax;
                case AttrType.MpMax:
                    return target.mpMax;
                case AttrType.Atk:
                    return target.atk;
                case AttrType.AtkMin:
                    return target.atkMin;
                case AttrType.AtkMax:
                    return target.atkMax;
                case AttrType.MagicalMin:
                    return target.MagicMin;
                case AttrType.MagicalMax:
                    return target.MagicMax;
                case AttrType.Def:
                    return target.def;
//                case AttrType.AtkSpeed:
//                    return target.atkSpeed;
//                case AttrType.MoveSpeed:
//                    return target.moveSpeed;
                case AttrType.CriticalRate:
                    return target.criticalRate;
                case AttrType.CriticalDamage:
                    return target.criticalDamage;
                case AttrType.TiliMax:
                    return target.tiliMax;
                case AttrType.AtkSpeedScale:
                    return target.atkSpeedScale;
                case AttrType.MoveSpeedScale:
                    return target.moveSpeedScale;
                case AttrType.Hp:
                    return target.hp;
                case AttrType.Mp:
                    return target.mp;
                case AttrType.Tili:
                    return target.tili;
                case AttrType.WalkSpeed:
                    return target.walkSpeed;
                case AttrType.RunSpeed:
                    return target.runSpeed;
                case AttrType.Poise:
                    return target.poise;
                case AttrType.PoiseMax:
                    return target.poiseMax;
                case AttrType.PoiseCut:
                    return target.poiseCut;
                case AttrType.PoiseScale:
                    return target.poiseScale;
                case AttrType.Mass:
                    return target.mass;
                case AttrType.SuperArmor:
                    return target.superArmor;
                case AttrType.RushSpeed:
                    return target.rushSpeed;
                
                case AttrType.Vigor:
                    return target.vigor;
                case AttrType.VigorMax:
                    return target.vigorMax;
                case AttrType.Strength:
                    return target.strength;
                case AttrType.Weight:
                    return target.weight;
                case AttrType.LessenHurt: 
                    return target.lessenhurt;
            }
            return 0;
        }

        public static float GetAttr(this EquipData target, AttrType attrType)
        {
            if (target != null && target.attrData != null)
            {
                return target.attrData.GetAttr(attrType);
            }
            return 0;
        }

        public static float GetArmorAttr(this RoleData target, AttrType attrType)
        {
            if (target.EquipData != null && target.EquipData.armorData != null && string.IsNullOrWhiteSpace(target.EquipData.armorData.id) == false)
            {
                return target.EquipData.armorData.GetAttr(attrType);
            }
            return 0;
        }

        public static float GetDecorationAttr(this RoleData target, AttrType attrType)
        {
            if (target.EquipData != null && target.EquipData.DecorationDataList != null)
            {
                float value = 0;
                foreach (var decoration in target.EquipData.DecorationDataList)
                {
                    if(decoration != null && string.IsNullOrWhiteSpace(decoration.id) == false)
                        value += decoration.GetAttr(attrType);
                }
                return value;
            }
            return 0;
        }

        public static float GetMainHandAttr(this RoleData target, AttrType attrType)
        {
            if (target.EquipData != null && target.EquipData.MainHand != null && string.IsNullOrWhiteSpace(target.EquipData.MainHand.id) == false)
            {

                return target.EquipData.MainHand.GetAttr(attrType);

            }
            return 0;
        }
        
        public static float GetMainHandFinalAttr(this RoleData target, AttrType attrType)
        {
            if (target.EquipData != null && target.EquipData.OffHand != null && string.IsNullOrWhiteSpace(target.EquipData.OffHand.id) == false)
            {
                return target.GetFinalAttr(attrType) - target.EquipData.OffHand.GetAttr(attrType);
            }
            return target.GetFinalAttr(attrType);
        }

        public static float GetOffHandAttr(this RoleData target, AttrType attrType)
        {
            if (target.EquipData != null && target.EquipData.OffHand != null && string.IsNullOrWhiteSpace(target.EquipData.OffHand.id) == false)
            {
                return target.EquipData.OffHand.GetAttr(attrType);
            }
            return 0;
        }
        
        public static float GetDefenceAttr(this RoleData target, AttrType attrType)
        {
            if (target.EquipData != null && target.EquipData.OffHand != null && string.IsNullOrWhiteSpace(target.EquipData.OffHand.id) == false)
            {
                return target.GetFinalAttr(attrType) - target.EquipData.OffHand.GetAttr(attrType);
            }
            return target.GetFinalAttr(attrType);
        }
        public static float GetOffHandFinalAttr(this RoleData target, AttrType attrType)
        {
            if (target.EquipData != null && target.EquipData.MainHand != null && string.IsNullOrWhiteSpace(target.EquipData.MainHand.id) == false)
            {

                return target.GetFinalAttr(attrType) - target.EquipData.MainHand.GetAttr(attrType);
            }
            return target.GetFinalAttr(attrType);
        }

        public static float GetAttr(this RoleData target, AttrType attrType)
        {
            return target.AttrData.GetAttr(attrType);
        }
        
        
        public static float GetAttr(this BuffData target, AttrType attrType)
        {
            return target.attrData.GetAttr(attrType);
        }

        public static float GetBuffAttr(this RoleData target, AttrType attrType)
        {
            float value = 0;
            foreach (var item in target.RoleBuffData.buffDataDic)
            {
                value += item.Value.GetAttr(attrType);
            }
            return value;
        }

        public static float GetLevelAwardAttr(this RoleData target, AttrType attrType)
        {
            float value = 0;
            int add = (target.level - 1).Range(0, 9999999);

            switch (attrType)
            {
                case AttrType.HpMax:
                    value = add * 25;
                    break;
                case AttrType.Atk:
                    value = add * 5;
                    break;
                case AttrType.Def:
                    value = add * 3;
                    break;
                default:
                    value = 0;
                    break;
            }
            
            return value;
        }
        
        public static float GetDifficultyLevelAwardMulAttr(this RoleData target, AttrType attrType)
        {
            float value = 0;
            int add = (target.difficultyLevel - 1).Range(0, 9999999);

            switch (attrType)
            {
                case AttrType.HpMax:
                    value = 1 + add * 0.2f;
                    break;
                case AttrType.Atk:
                    value = 1 + add * 0.1f;
                    break;
                case AttrType.Def:
                    value = 1 + add * 0.05f;
                    break;
                default:
                    value = 1;
                    break;
            }
            
            return value;
        } 

        public static float GetFinalAttr(this RoleData target, AttrType attrType)
        {
            float value = 0;

            // 加法 add by TangJian 2018/05/15 12:32:22
            value += target.GetAttr(attrType); // 基础属性 add by TangJian 2018/01/18 15:52:07
            value += target.GetArmorAttr(attrType); // 护甲属性 add by TangJian 2018/01/18 15:52:13
            value += target.GetDecorationAttr(attrType); // 饰品属性 add by TangJian 2018/01/18 15:52:23
            value += target.GetMainHandAttr(attrType); // 主手属性 add by TangJian 2018/01/18 16:24:11
            value += target.GetOffHandAttr(attrType); // 副手属性 add by TangJian 2018/01/18 16:24:19
            value += target.GetBuffAttr(attrType); // 增益属性 add by TangJian 2018/05/15 12:02:07

            value += target.GetLevelAwardAttr(attrType); // 获得等级加成属性 add by TangJian 2019/3/6 22:05

            value *= target.GetDifficultyLevelAwardMulAttr(attrType); // 难度属性加成 add by TangJian 2019/3/13 10:00
            
            return value;
        }

        public static void AddAttr(this RoleData target, AttrType attrType, float value)
        {
            switch (attrType)
            {
                case AttrType.HpMax:
                    target.HpMax += value;
                    break;
                case AttrType.MpMax:
                    target.MpMax += value;
                    break;
                case AttrType.Atk:
                    target.Atk += value;
                    break;
                case AttrType.Hp:
                    target.Hp += value;
                    break;
                case AttrType.Mp:
                    target.Mp += value;
                    break;
                case AttrType.Tili:
                    target.Tili += value;
                    break;
                case AttrType.Def:
                    target.Def += value;
                    break;
                case AttrType.AtkSpeedScale:
                    target.AtkSpeedScale += value;
                    break;
                case AttrType.MoveSpeedScale:
                    target.MoveSpeedScale += value;
                    break;
                case AttrType.CriticalRate:
                    target.CriticalRate += value;
                    break;
                case AttrType.CriticalDamage:
                    target.CriticalDamage += value;
                    break;
                case AttrType.TiliMax:
                    target.TiliMax += value;
                    break;
                default:
                    break;
            }
        }
        
        public static void MulAttr(this RoleData target, AttrType attrType, float value)
        {
            switch (attrType)
            {
                case AttrType.WalkSpeed:
                    target.WalkSpeed *= value;
                    break;
                case AttrType.HpMax:
                    target.HpMax *= value;
                    break;
                case AttrType.MpMax:
                    target.MpMax *= value;
                    break;
                case AttrType.Atk:
                    target.Atk *= value;
                    break;
                case AttrType.Hp:
                    target.Hp *= value;
                    break;
                case AttrType.Mp:
                    target.Mp *= value;
                    break;
                case AttrType.Tili:
                    target.Tili *= value;
                    break;
                case AttrType.Def:
                    target.Def *= value;
                    break;
                case AttrType.AtkSpeedScale:
                    target.AtkSpeedScale *= value;
                    break;
                case AttrType.MoveSpeedScale:
                    target.MoveSpeedScale *= value;
                    break;
                case AttrType.CriticalRate:
                    target.CriticalRate *= value;
                    break;
                case AttrType.CriticalDamage:
                    target.CriticalDamage *= value;
                    break;
                case AttrType.TiliMax:
                    target.TiliMax *= value;
                    break;
                default:
                    break;
            }
        }
    }

    [System.Serializable]
    public class  AttrData
    {
        public float hp = 0;
        public float hpMax = 0;
        public float mp = 0;
        public float mpMax = 0;
        public float tili = 0;
        public float tiliMax = 0;
        public float atk = 0;

        // 伤害范围 怪物用
        public float atkMin = 0;
        public float atkMax = 0;

        // 魔法伤害范围 怪物用
        public float MagicMin = 0;
        public float MagicMax = 0;
        // 物理减伤百分比
        public float physicalDef;
        // 魔法减伤百分比
        public float magicalDef;
        public float def = 0;
        public float atkSpeed = 0;
        public float moveSpeed = 0;

        public int critical;
        public float criticalRate = 0;
        public float criticalDamage = 0;

        // 数值倍率 add by TangJian 2018/05/15 12:24:36
        public float atkSpeedScale = 0;
        public float moveSpeedScale = 0;

        public float walkSpeed = 0;
        public float runSpeed = 0;
        public float rushSpeed = 0;

        public float jumpSpeed = 0;
        
        //击退抗性与防御击退抗性
        public float RepellingResistance = 0;
        public float DefenseRepellingResistance = 0;
        
        //跳跃次数
        public int jumpTimes = 0;

        // 韧性 add by TangJian 2018/12/12 12:13
        public float poiseMax = 0;
        public float poise = 0;

        // 韧性倍率 add by TangJian 2018/12/12 14:46
        public float poiseScale = 0;

        // 削韧 add by TangJian 2018/12/12 12:14
        public float poiseCut = 0;
        //质量参数 
        public float mass = 0;
        
        // 是否霸体 add by TangJian 2019/3/23 15:33
        public float superArmor = 0;
        
        //
        public MatType matType = MatType.Body;
        
        public HitEffectType hitEffectType = HitEffectType.lswd;
        
        // 精力
        public float vigor = 0;
        
        // 精力值上限
        public float vigorMax = 0;
        
        // 力量
        public float strength = 0;
        
        // 重量
        public float weight = 0;
        
        //减伤
        public float lessenhurt;
    }

    [System.Serializable]
    public class EquipData : ItemData
    {
        public string groundImgId;
        public ImgGround imgGround = ImgGround.Dedult;
        public EquipType equipType = EquipType.None;
        public AttrData attrData = new AttrData();
        
    }
    
    //攻击属性类型 
    public enum AtkPropertyType
    {
        // 魔法伤害
        magicalDamage = 1,
        // 物理伤害
        physicalDamage = 2,
        // 混合伤害
        mixDamage = 3

    }
    
    public enum CompareType
    {
        Equals,
        Less,
        LessContain,
        Greate,
        GreateContain
    }
    
    public enum FourFundamentalRulesType
    {
        none,
        Add,
        Minus,
        Multiplied,
        SignOfDivision,
    }
}