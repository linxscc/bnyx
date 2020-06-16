using System;
using Spine.Unity;
using UnityEngine;

namespace Tang.FrameEvent
{
    public static class FrameEventMethods
    {
        public static void OnSkillAtk(FrameEventInfo.RoleAtkFrameEventData roleAtkFrameEventData
            , string id
            , string damageId
            , string teamId
            , SkeletonRenderer skeletonRenderer
            , ITriggerDelegate triggerDelegate
            , Quaternion rotation

            , DamageEffectType baseDamageEffectType = DamageEffectType.Strike
            , AtkPropertyType baseAtkPropertyType = AtkPropertyType.physicalDamage
            , float baseAtk = 0
            , float baseAtkMin = 0
            , float baseAtkMax = 0
            , float baseMagicAtkMin = 0
            , float baseMagicAtkMax = 0
            , float basePoiseCut = 0
            , float baseMass = 0
            , float criticalRate = 0
            , float criticalAtk = 1
            , Action<DamageController> onAddDamageController = null)
        {
            Vector3 vector = rotation * new Vector3(1, 0, 0);
            
            OnRoleAtk(roleAtkFrameEventData, id, damageId, teamId, skeletonRenderer, triggerDelegate, vector,
                baseDamageEffectType, baseAtkPropertyType, baseAtk, baseAtkMin, baseAtkMax, baseMagicAtkMin,
                baseMagicAtkMax, basePoiseCut, baseMass, criticalRate, criticalAtk, onAddDamageController);
        }

        public static void OnRoleAtk(FrameEventInfo.RoleAtkFrameEventData roleAtkFrameEventData
            , string id
            , string damageId
            , string teamId
            , SkeletonRenderer skeletonRenderer
            , ITriggerDelegate triggerDelegate
            , Vector3 rotation

            , DamageEffectType baseDamageEffectType = DamageEffectType.Strike
            , AtkPropertyType baseAtkPropertyType = AtkPropertyType.physicalDamage
            , float baseAtk = 0
            , float baseAtkMin = 0
            , float baseAtkMax = 0
            , float baseMagicAtkMin = 0
            , float baseMagicAtkMax = 0
            , float basePoiseCut = 0
            , float baseMass = 0
            , float criticalRate = 0
            , float criticalAtk = 1
            , Action<DamageController> onAddDamageController = null)
        {
            if (roleAtkFrameEventData == null)
            {            
                Debug.LogWarning(roleAtkFrameEventData == null);
                return;
            }

            if (roleAtkFrameEventData.type == FrameEventInfo.RoleAtkFrameEventData.Type.Default
                || roleAtkFrameEventData.type == FrameEventInfo.RoleAtkFrameEventData.Type.Add)
            {
                if (roleAtkFrameEventData.bindType == FrameEventInfo.RoleAtkFrameEventData.BindType.Default)
                {
                    DamageController damageController = RoleAtk(triggerDelegate, id, damageId, teamId,
                        roleAtkFrameEventData.parentType, roleAtkFrameEventData.primitiveType, roleAtkFrameEventData.pos, roleAtkFrameEventData.size,
                        rotation, roleAtkFrameEventData.hitType, roleAtkFrameEventData.targetMoveBy,
                        roleAtkFrameEventData.DamageDirectionType,
                        roleAtkFrameEventData.SpecialEffectOnOff
                            ? roleAtkFrameEventData.damageEffectType
                            : baseDamageEffectType
                        , roleAtkFrameEventData.atkPropertyTypeOnOff
                            ? roleAtkFrameEventData.atkPropertyType
                            : baseAtkPropertyType
                        , roleAtkFrameEventData.atk
                        , roleAtkFrameEventData.poiseCut
                        , baseAtk
                        , baseAtkMin
                        , baseAtkMax
                        , baseMagicAtkMin
                        , baseMagicAtkMax
                        , basePoiseCut
                        , baseMass
                        , criticalRate
                        , criticalAtk
                    );
                    
                    if (roleAtkFrameEventData.type == FrameEventInfo.RoleAtkFrameEventData.Type.Add)
                    {
                        damageController.SetId(id);
                    }
                    else
                    {
                        damageController.SetId(id);
                        damageController.SetDestroyTime(0.1f);
                    }
                    
                    // 破盾 add by TangJian 2019/4/3 17:00
                    damageController.SetBreakShiled(roleAtkFrameEventData.breakShield);
                    damageController.SetIgnoreShiled(roleAtkFrameEventData.ignoreShield);
                    
                    if (onAddDamageController != null)
                        onAddDamageController(damageController);
                }
                else if (roleAtkFrameEventData.bindType == FrameEventInfo.RoleAtkFrameEventData.BindType.BindAnimSlot)
                {
                    foreach (var slotName in roleAtkFrameEventData.slotList)
                    {
                        DamageController damageController = RoleAtk(triggerDelegate, id, damageId, teamId,
                            roleAtkFrameEventData.parentType, roleAtkFrameEventData.primitiveType, roleAtkFrameEventData.pos, roleAtkFrameEventData.size,
                            rotation, roleAtkFrameEventData.hitType, roleAtkFrameEventData.targetMoveBy,
                            roleAtkFrameEventData.DamageDirectionType,
                            roleAtkFrameEventData.SpecialEffectOnOff
                                ? roleAtkFrameEventData.damageEffectType
                                : baseDamageEffectType
                            , roleAtkFrameEventData.atkPropertyTypeOnOff
                                ? roleAtkFrameEventData.atkPropertyType
                                : baseAtkPropertyType
                            , roleAtkFrameEventData.atk
                            , roleAtkFrameEventData.poiseCut
                            , baseAtk
                            , baseAtkMin
                            , baseAtkMax
                            , baseMagicAtkMin
                            , baseMagicAtkMax
                            , basePoiseCut
                            , baseMass
                        );

                        // 伤害绑定slot add by TangJian 2018/12/5 17:14
                        damageController.bindType = FrameEventInfo.RoleAtkFrameEventData.BindType.BindAnimSlot;
                        damageController.bindSkeletonRenderer = skeletonRenderer;
                        damageController.bindSlotName = slotName;

                        if (roleAtkFrameEventData.type == FrameEventInfo.RoleAtkFrameEventData.Type.Add)
                        {
                            damageController.SetId(id);
                        }
                        else
                        {
                            damageController.SetId(id);
                            damageController.SetDestroyTime(0.1f);
                        }
                        
                        // 破盾 add by TangJian 2019/4/3 17:00
                        damageController.SetBreakShiled(roleAtkFrameEventData.breakShield);
                        damageController.SetIgnoreShiled(roleAtkFrameEventData.ignoreShield);
                        
                        if (onAddDamageController != null)
                            onAddDamageController(damageController);
                    }
                }
            }
            else if (roleAtkFrameEventData.type == FrameEventInfo.RoleAtkFrameEventData.Type.Remove)
            {
                DamageManager.Instance.Remove(id);
            }
        }

        public static DamageController RoleAtk(ITriggerDelegate triggerDelegate,
            string id
            , string damageId
            , string teamId
            , FrameEventInfo.ParentType parentType
            , PrimitiveType primitiveType
            , Vector3 pos
            , Vector3 size
            , Vector3 orientation
            , HitType hitType
            , Vector3 hitMoveBy
            
            , DamageDirectionType damageDirectionType
            , DamageEffectType damageEffectType
            , AtkPropertyType atkPropertyType
            , float atkScale
            , float poiseCutScale
            
            
            // 基础值 add by TangJian 2018/12/18 21:03
//            , DamageEffectType baseDamageEffectType = DamageEffectType.Strike
//            , AtkPropertyType baseAtkPropertyType = AtkPropertyType.physicalDamage
            , float baseAtk = 0
            , float baseAtkMin = 0
            , float baseAtkMax = 0
            , float baseMagicAtkMin = 0
            , float baseMagicAtkMax = 0
            , float basePoiseCut = 0
            ,float baseMass=0
            , float criticalRate = 0
            , float criticalAtk = 1)
        {
            
            
            // 根据公式计算攻击力 add by TangJian 2018/12/18 21:27
            float atk = 0;
            switch (atkPropertyType)
            {
                case AtkPropertyType.physicalDamage:
                {
                    atk = (baseAtk + UnityEngine.Random.Range(baseAtkMin, baseAtkMax)) * atkScale;
                }
                    break;
                case AtkPropertyType.magicalDamage:
                {
                    atk = (baseAtk + UnityEngine.Random.Range(baseAtkMin, baseAtkMax)) * atkScale;
                }
                    break;
                case AtkPropertyType.mixDamage:
                {
                    float atk_ = UnityEngine.Random.Range(baseAtkMin, baseAtkMax);
                    float magical = UnityEngine.Random.Range(baseMagicAtkMin, baseMagicAtkMax);
        
                    atk_ = (baseAtk + atk) * atk_;
                    magical = (baseAtk + magical) * atk_;

                    atk = magical;
                }
                    break;
                default:
                {
                    atk = (baseAtk + UnityEngine.Random.Range(baseAtkMin, baseAtkMax)) * atkScale;
                }
                    break;
            }

            return Atk(triggerDelegate, id, damageId, teamId, parentType, primitiveType, pos, size, orientation, hitType, hitMoveBy, damageDirectionType, damageEffectType, atkPropertyType, atk, basePoiseCut * poiseCutScale, baseMass, criticalRate, criticalAtk);
        }
        
        public static DamageController Atk(ITriggerDelegate triggerDelegate,
            string id
            , string damageId
            , string teamId
            , FrameEventInfo.ParentType parentType
            , PrimitiveType primitiveType
            , Vector3 pos
            , Vector3 size
            , Vector3 orientation
            , HitType hitType
            , Vector3 hitMoveBy
            
            , DamageDirectionType damageDirectionType
            , DamageEffectType damageEffectType
            , AtkPropertyType atkPropertyType
            , float atk
            , float poiseCut
            , float Mass
            , float criticalRate = 0
            , float criticalAtk = 1)
        {
            DamageController damageController = DamageManager.Instance.CreateDamage(

                    pos.RotateFrontTo(orientation)

                    , triggerDelegate.transform, parentType
                    , size
                    , primitiveType)

                // 设置DamageControllerId
                .SetId(id)
                //设置damageid
                .SetDamageId(damageId)
                // 设置队伍 add by TangJian 2018/10/9 16:17
                .SetTeamId(teamId)

                // 设置拥有者 add by TangJian 2018/10/9 16:43
                .SetOwner(triggerDelegate.gameObject)

                // 设置触发器代理 add by TangJian 2018/10/9 16:55
                .SetTriggerDelegate(triggerDelegate)

                // 设置攻击类型 add by TangJian 2018/10/9 17:29
                .SetHitType(hitType)

                // 设置攻击朝向 add by TangJian 2018/10/9 17:30
                .SetDirection(orientation)

                // 设置击退距离 add by TangJian 2018/10/9 17:30
                .SetMoveBy(hitMoveBy.RotateFrontTo(orientation));

            damageController.setDamageDirectionType(damageDirectionType);

            // 伤害效果类型 add by TangJian 2017/08/16 15:33:01
            damageController.SetDamageEffectType(damageEffectType);
            
            // 攻击属性类型
            damageController.SetAtkPropertyType(atkPropertyType);
            
            // 削韧 add by TangJian 2018/12/12 12:32
            damageController.SetPoiseCut(poiseCut);
            //攻击质量
            damageController.SetDamageMass(Mass);
            
            // 暴击 add by TangJian 2019/3/25 15:20
            if (Tools.RandomWithWeight(criticalRate.Range(0, 1), 1 - criticalRate.Range(0, 1)) == 0)
            {
                damageController.SetCritical(true);
            }
            
            // 设置攻击力 add by TangJian 2018/12/18 20:41
            switch (atkPropertyType)
            {
                case AtkPropertyType.physicalDamage:
                {
                    if (damageController.IsCritical())
                    {
                        damageController.SetAtk(atk * criticalAtk);
                    }
                    else
                    {
                        damageController.SetAtk(atk);

                    }
                }
                    break;
                case AtkPropertyType.magicalDamage:
                {
                    damageController.SetMagical(atk);
                }
                    break;
                case AtkPropertyType.mixDamage:
                {
                    damageController.SetMagical(atk);
                }
                    break;
                default:
                {
                    damageController.SetAtk(atk);
                }
                    break;
            }

            return damageController;
        }
    }
}