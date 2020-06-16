using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using ZS;

namespace Tang
{
    public interface IHitAndHurtDelegate
    {
        IObjectController ObjectController { get; set; }

        bool TryGetHitPos(out Vector3 hitPos);
        bool Hit(DamageData damageData);
        bool Hurt(DamageData damageData);
        
        bool Hurt(DamageData damageData, HurtPart hurtPart);
    }

    public class HitAndHurtController : IHitAndHurtDelegate
    {
        public HitAndHurtController(IObjectController objectController)
        {
            this.ObjectController = objectController;
        }

        public IObjectController ObjectController { get; set; }

        public bool TryGetHitPos(out Vector3 hitPos)
        {
            hitPos = Vector3.zero;
            if(ObjectController.MainSkeletonRenderer != null)
                return ObjectController.MainSkeletonRenderer.GetBonePos("hurt_effect_pos", out hitPos);
            return false;
        }

        void AnimSuspend(float suspendTime, float suspendScale)
        {
            ObjectController.MainAnimator.speed = ObjectController.DefaultAnimSpeed;

            var speed_l = ObjectController.DefaultAnimSpeed * suspendScale;
            var speed_d = ObjectController.DefaultAnimSpeed;
            var time_d = suspendTime;
            
            ObjectController.MainAnimator.speed = speed_l;

            if (ObjectController is MyMonoBehaviour myMonoBehaviour)
            {
                myMonoBehaviour.DelayFunc("AnimSuspend", () =>
                {
                    ObjectController.MainAnimator.speed = ObjectController.DefaultAnimSpeed;
                }, time_d);
            }
        }

        Vector3 CalcOffset(IObjectController selfObjectController, IObjectController otherObjectController, Vector3 offset)
        {
            Vector3 forcedOffset = Vector3.zero;

            var otherToSelfOffset = selfObjectController.transform.position - otherObjectController.transform.position;

            if (Mathf.Abs(otherToSelfOffset.x) < Mathf.Abs(offset.x))
            {
                forcedOffset = otherToSelfOffset + new Vector3(offset.x * selfObjectController.GetDirectionInt(), offset.y, offset.z);
            }
                    
            forcedOffset.y = 0;
            forcedOffset.z = 0;
            
            return forcedOffset;
        }

        public bool Hit(DamageData damageData)
        {
            var speed = ObjectController.Speed;
            
            // 强制位移
             Vector3 forcedOffset = Vector3.zero;
            
            // 击退
            var selfObjectController = ObjectController;
            var selfHitAndMat = selfObjectController as HitAble;

            var otherObjectController = damageData.hitGameObject.GetComponent<IObjectController>();
            var otherHurtAble = damageData.hitGameObject.GetComponent<HurtAble>();
            
            
            if (otherObjectController != null && otherObjectController.CanRebound)
            {
                if (otherObjectController.IsDefending)
                {
                    Vector3 moveBySpeed = (damageData.targetMoveBy * (-otherObjectController.DefenseRepellingResistance)).MoveByToSpeed();
                    if (ObjectController.Speed.magnitude < moveBySpeed.magnitude)
                    {
                        // 去除y方向的反弹效果 add by TangJian 2019/1/26 17:19
                        speed.x = moveBySpeed.x;
                        speed.z = moveBySpeed.z;
                    }
                }
                else
                {
                    Vector3 moveBySpeed = (damageData.targetMoveBy * (-otherObjectController.RepellingResistance)).MoveByToSpeed();
                    if (speed.magnitude < moveBySpeed.magnitude)
                    {
                        // 去除y方向的反弹效果 add by TangJian 2019/1/26 17:19
                        speed.x = moveBySpeed.x;
                        speed.z = moveBySpeed.z;
                    }
                }
                
                // 强制位移 add by TangJian 2019/4/30 10:29
                if(damageData.useForcedOffset && otherObjectController is RoleController)
                {
                    forcedOffset = CalcOffset(selfObjectController, otherObjectController, damageData.forcedOffset);
                }
            }

            if (otherObjectController.IsGrounded())
            {
                
            }

            if (otherHurtAble != null && selfHitAndMat != null)
            {
                // 攻击击中时动画暂停
                AnimSuspend(damageData.selfSuspendTime, 1f + (damageData.selfSuspendScale - 1f) * otherHurtAble.Damping);
                
                Vector3 rendererOffset = Vector3.zero;
                if (damageData.effectOrientation.z > 0)
                {
                    rendererOffset.z = otherObjectController.BackZ;
                }
                else
                {
                    rendererOffset.z = otherObjectController.FrontZ;
                }

                Vector3 worldPos;
                if (otherHurtAble.TryGetHitPos(out worldPos))
                {
                    // 打击特效
                    BothAnimManager.Instance.PlayHitEffect(selfHitAndMat.GetHitEffectType(), otherHurtAble.GetMatType(), worldPos + rendererOffset + forcedOffset, damageData.direction.x > 0 ? Direction.Right : Direction.Left, damageData.effectOrientation.GetAngleZ());
                }
                else
                {
                    switch (otherHurtAble.EffectShowMode)
                    {
                        case EffectShowMode.FrontOrBack :
                            BothAnimManager.Instance.PlayHitEffect(selfHitAndMat.GetHitEffectType(), otherHurtAble.GetMatType(), new Vector3(damageData.collidePoint.x, damageData.collidePoint.y, otherObjectController.transform.position.z) + rendererOffset + forcedOffset, damageData.direction.x > 0 ? Direction.Right : Direction.Left, damageData.effectOrientation.GetAngleZ());
                            break;
                        case EffectShowMode.ColliderPoint:
                            BothAnimManager.Instance.PlayHitEffect(selfHitAndMat.GetHitEffectType(), otherHurtAble.GetMatType(), damageData.collidePoint + forcedOffset, damageData.direction.x > 0 ? Direction.Right : Direction.Left, damageData.effectOrientation.GetAngleZ());
                            break;
                        default:
                            BothAnimManager.Instance.PlayHitEffect(selfHitAndMat.GetHitEffectType(), otherHurtAble.GetMatType(), damageData.collidePoint + forcedOffset, damageData.direction.x > 0 ? Direction.Right : Direction.Left, damageData.effectOrientation.GetAngleZ());
                            break;
                    }
                } 
                AudioManager.Instance.PlayEffect(selfHitAndMat.GetHitEffectType(),otherHurtAble.GetMatType(), worldPos + rendererOffset + forcedOffset);
            }

            selfObjectController.Speed = speed;
            return true;
        }

        public bool Hurt(DamageData damageData)
        {
            IObjectController ownerObjectController = damageData.owner.GetComponent<IObjectController>();

            // 强制位移 add by TangJian 2019/4/30 10:29
            Vector3 forcedOffset = Vector3.zero;
            if(damageData.useForcedOffset && ownerObjectController != null && ObjectController is RoleController)
            {
                forcedOffset = CalcOffset(ownerObjectController, ObjectController, damageData.forcedOffset);

                // 如果是角色则执行移动 add by TangJian 2019/5/10 17:54
                (ObjectController as RoleController)?.DoMoveBy(forcedOffset, damageData.forcedOffsetDuration).SetEase(Ease.OutSine);
            }
            
            // 播放特效
            if (damageData is HitAble selfHitAble && ObjectController is HurtAble hurtAble)
            {
                Vector3 rendererOffset = Vector3.zero;
                if (damageData.effectOrientation.z > 0)
                {
                    rendererOffset.z += 0.1f;
                }
                else
                {
                    rendererOffset.z -= 0.1f;
                }
                     
                Vector3 worldPos;
                if (TryGetHitPos(out worldPos))
                {
                    // 打击特效
                    BothAnimManager.Instance.PlayHurtEffect(selfHitAble.GetHitEffectType(), hurtAble.GetMatType(), worldPos + rendererOffset + forcedOffset, damageData.direction.x > 0 ? Direction.Right : Direction.Left, 0, damageData.targetMoveBy);   
                }
                else
                {
                    // 打击特效
                    BothAnimManager.Instance.PlayHurtEffect(selfHitAble.GetHitEffectType(), hurtAble.GetMatType(), damageData.collidePoint + rendererOffset + forcedOffset, damageData.direction.x > 0 ? Direction.Right : Direction.Left, 0, damageData.targetMoveBy);
                }
                AudioManager.Instance.PlayEffect(selfHitAble.GetHitEffectType(),hurtAble.GetMatType(), worldPos + rendererOffset + forcedOffset);
            }

            return true;
        }

        public bool Hurt(DamageData damageData, HurtPart hurtPart)
        {
            IObjectController ownerObjectController = damageData.owner.GetComponent<IObjectController>();
            
            // 强制位移 add by TangJian 2019/4/30 10:29
            Vector3 forcedOffset = Vector3.zero;
            if(damageData.useForcedOffset && ownerObjectController != null && ObjectController is RoleController)
            {
                forcedOffset = CalcOffset(ownerObjectController, ObjectController, damageData.forcedOffset);

                // 如果是角色则执行移动 add by TangJian 2019/5/10 17:54
                (ObjectController as RoleController)?.DoMoveBy(forcedOffset, damageData.forcedOffsetDuration).SetEase(Ease.OutSine);
            }
            
            // 播放特效
            if (damageData is HitAble selfHitAble && ObjectController is HurtAble hurtAble)
            {
                Vector3 rendererOffset = Vector3.zero;
                if (damageData.effectOrientation.z > 0)
                {
                    rendererOffset.z += 0.1f;
                }
                else
                {
                    rendererOffset.z -= 0.1f;
                }
                     
                Vector3 worldPos;
                if (TryGetHitPos(out worldPos))
                {
                    // 打击特效
                    BothAnimManager.Instance.PlayHurtEffect(selfHitAble.GetHitEffectType(), hurtPart.MatType, worldPos + rendererOffset + forcedOffset, damageData.direction.x > 0 ? Direction.Right : Direction.Left, 0, damageData.targetMoveBy);   
                }
                else
                {
                    // 打击特效
                    BothAnimManager.Instance.PlayHurtEffect(selfHitAble.GetHitEffectType(), hurtPart.MatType, damageData.collidePoint + rendererOffset + forcedOffset, damageData.direction.x > 0 ? Direction.Right : Direction.Left, 0, damageData.targetMoveBy);
                }
                AudioManager.Instance.PlayEffect(selfHitAble.GetHitEffectType(), hurtPart.MatType, worldPos + rendererOffset + forcedOffset);
            }

            return true;
        }
    }
}