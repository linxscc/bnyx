using System.Collections.Generic;
using UnityEngine;
using ZS;

namespace Tang
{
    public class WoodenBucketController : PlacementController
    {
        public int CurrAtkCount = 0;
        public float CurrHp = 0;
        
        // 爆炸动画add by TangJian 2019/5/10 18:27
        private void Explode(Vector3 force)
        {
            AnimManager.Instance.PlayAnimEffect(Placementdata.deathEffect, gameObject.GetRendererBounds().center, 0, force.x < 0);
        }
        
        // 初始化 add by TangJian 2019/5/10 18:27
        public override void Start()
        {
            base.Start();
            CurrHp = Placementdata.atkhp;
        }
        
        // 受击 add by TangJian 2019/5/10 18:28
        public override bool OnHurt(DamageData damageData)
        {
            switch (Placementdata.hporCount)
            {
                case tian.HporCount.None:
                    Explode(damageData.targetMoveBy);
                    Drop();
                    Destroy(gameObject);
                    break;
                case tian.HporCount.count:
                    if (CurrAtkCount < Placementdata.atkcount - 1)
                    {
                        State = 1;
                        HitAndHurtDelegate.Hurt(damageData);
                        
                        CurrAtkCount++;
                    }
                    else
                    {
                        Explode(damageData.targetMoveBy);
                        Destroy(gameObject);
                    }
                    break;
                case tian.HporCount.Hp:
                    float damage = 0;
                    switch (damageData.atkPropertyType)
                    {
                        case AtkPropertyType.physicalDamage:
                            damage = damageData.atk;
                            break;
                        case AtkPropertyType.magicalDamage:
                            damage = damageData.magical;
                            break;
                        case AtkPropertyType.mixDamage:
                            damage = damageData.atk + damageData.magical;
                            break;
                        default:
                            break;
                    }
                    CurrHp -= damage;
                    if (CurrHp > 0)
                    {
                    }
                    else
                    {
                        Explode(damageData.targetMoveBy);
                        Drop();
                        Destroy(gameObject);
                    }
                    break;
                default:
                    break;
            }

            return true;
        }
    }
}