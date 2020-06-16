using System;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Tang.FrameEvent;
using UnityEngine.SocialPlatforms.Impl;

namespace Tang
{
    public class DamageController : TriggerController, ITriggerDelegate
    {
        public FrameEventInfo.RoleAtkFrameEventData.BindType bindType = FrameEventInfo.RoleAtkFrameEventData.BindType.Default;
        public SkeletonRenderer bindSkeletonRenderer;
        public string bindSlotName;

        public IObjectController ObjectController;
        
        public DamageData damageData = new DamageData();

        public GameObject GetGameObject() { return gameObject; }

        public Collider mainCollider;

        public Func<bool> NeedRemoveSelf;

        public override void OnEnable()
        {
            base.OnEnable();

            DamageManager.Instance.Add(this);

            mainCollider = GetComponent<Collider>();
            mainCollider.enabled = false;

            //if (Definition.Debug)
            //{
            //    Renderer renderer = GetComponent<Renderer>();
            //    renderer.enabled = true;
            //}
        }

        public override void OnDisable()
        {
            base.OnDisable();

            DamageManager.Instance.Remove(this);
        }

        public void OnTriggerIn(TriggerEvent triggerEvent)
        {
            TryAttack(triggerEvent);
        }

        public void OnTriggerKeep(TriggerEvent triggerEvent)
        {
            TryAttack(triggerEvent);
        }

        public void OnTriggerOut(TriggerEvent evt)
        {
        }

        public bool OnEvent(Event evt)
        {
            return true;
        }

        public void TryAttack(TriggerEvent triggerEvent)
        {
            if (damageData.damageTimes == -1
                || DamageManager.Instance.GetDamageHitObjTimes(damageData.DamageId, triggerEvent.otherTriggerController.id.GetHashCode()) < damageData.damageTimes

                &&
                (Time.time - DamageManager.Instance.GetDamageHitObjTime(damageData.DamageId, triggerEvent.otherTriggerController.id.GetHashCode())) >= damageData.damageInterval
            )
            {
                if (Attack(triggerEvent)) 
                {
                    // 记录对对象
                    DamageManager.Instance.DamageHitObj(damageData.DamageId, triggerEvent.otherTriggerController.id.GetHashCode());
                }
            }
        }

        public bool Attack(TriggerEvent triggerEvent)
        {
            if (damageData == null)
            {
                Debug.Log("damageData == null");
                return false;
            }
            
//            damageData = damageData.Clone() as DamageData;

            switch (damageData.DamageDirectionType)
            {
                case DamageDirectionType.Directional:
                    break;
                case DamageDirectionType.DirectionalWithoutY:
                    damageData.targetMoveBy.y = 0;
                    break;
                case DamageDirectionType.Radial:
                {
                    var damagePos = transform.position;
                    var targetPos = triggerEvent.colider.gameObject.transform.position;
                    var damageToTargetVec3 = targetPos - damagePos;

                    // 计算力的方向 add by TangJian 2017/07/31 16:29:04
//                        var forceMagnitude = damageData.force.magnitude;
//                        damageData.force = damageToTargetVec3.normalized * forceMagnitude;

                    damageData.targetMoveBy = damageToTargetVec3.normalized * damageData.targetMoveBy.magnitude;
                        
                    // 计算方向 add by TangJian 2017/07/31 16:33:54
                    if (damageToTargetVec3.x > 0)
                    {
                        damageData.direction = new Vector3(1, 0, 0);
                    }
                    else if (damageToTargetVec3.x < 0)
                    {
                        damageData.direction = new Vector3(-1, 0, 0);
                    }
                }
                    break;
                case DamageDirectionType.RadialWithoutY:
                {
                    var damagePos = transform.position;
                    var targetPos = triggerEvent.colider.gameObject.transform.position;
                    var damageToTargetVec3 = targetPos - damagePos;

                    // 计算力的方向 add by TangJian 2017/07/31 16:29:04
//                        var forceMagnitude = damageData.force.magnitude;
//                        damageData.force = damageToTargetVec3.normalized * forceMagnitude;

                    damageData.targetMoveBy = damageToTargetVec3.normalized * damageData.targetMoveBy.magnitude;
                    damageData.targetMoveBy.y = 0;
                    
                    // 计算方向 add by TangJian 2017/07/31 16:33:54
                    if (damageToTargetVec3.x > 0)
                    {
                        damageData.direction = new Vector3(1, 0, 0);
                    }
                    else if (damageToTargetVec3.x < 0)
                    {
                        damageData.direction = new Vector3(-1, 0, 0);
                    }
                }
                    break;
                case DamageDirectionType.UseSpeed:
                    Vector3 VecSpeed = GetSpeed();
                    if (VecSpeed!=Vector3.zero)
                    {
                        damageData.targetMoveBy = VecSpeed;
                        damageData.targetMoveBy.y = 0;
                        if (VecSpeed.x>0)
                        {
                            damageData.direction = new Vector3(1, 0, 0);
                        }
                        else
                        {
                            damageData.direction = new Vector3(-1, 0, 0);
                        }    
                    }
                    break;
            }

            damageData.hitTarget = triggerEvent.colider.gameObject;
            damageData.WeaknessName = triggerEvent.colider.name;
            damageData.collideBounds = triggerEvent.colider.bounds;
            damageData.collidePoint = triggerEvent.colidePoint;

            if (damageData.itriggerDelegate is IHitAndMat selfHitAndHurt 
                && triggerEvent.otherTriggerController.ITriggerDelegate is IHitAndMat otherHitAndHurt)
            {
                damageData.hitGameObject = otherHitAndHurt.gameObject;
//                damageData.HitEffectType = selfHitAndHurt.GetHitEffectType();

                if (damageData.ignoreObjectList != null && damageData.ignoreObjectList.FindIndex(id => id == otherHitAndHurt.gameObject.GetHashCode()) >= 0)
                {
                    return false;
                }

                if (damageData.teamId == otherHitAndHurt.TeamId) // 队友不伤害 add by TangJian 2018/12/1 17:42
                {
                    if (damageData.DiaupTeammate)
                    {
                        damageData.atk = 0;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (otherHitAndHurt.OnHurt(damageData))
                {
                    selfHitAndHurt?.OnHit(damageData);
                }
            }
            return true;
        }

        //public override void OnUpdate()
        //{
        //    base.OnUpdate();

        //    if (bindType == FrameEventInfo.RoleAtkFrameEventData.BindType.BindAnimSlot)
        //    {
        //        if (bindSkeletonAnimator != null && bindSlotName != null)
        //        {
        //            Vector3 worldPostion, worldScale;
        //            Quaternion worldRotation;
        //            if (bindSkeletonAnimator.TryGetSlotAttachmentCube(bindSlotName, out worldPostion, out worldScale, out worldRotation))
        //            {
        //                transform.position = worldPostion;
        //                transform.localScale = worldScale;
        //                transform.localRotation = worldRotation;

        //                collider.enabled = true;
        //            }
        //        }
        //        else
        //        {
        //            collider.enabled = false;
        //        }
        //    }
        //}

        private Vector3 OldPos=Vector3.zero;
        private Vector3 BeforePos = Vector3.zero;
//        private float TimeRecord;
 
        public Vector3 GetSpeed()
        {
            if (OldPos!=Vector3.zero&&BeforePos!=Vector3.zero)
            {
                
                return Vector3Extensions.MoveByToSpeed(target: OldPos - BeforePos)/5f;
            }
            else
            {
                return  Vector3.zero;
            }
        }
        
        private void LateUpdate()
        {
            if (bindType == FrameEventInfo.RoleAtkFrameEventData.BindType.BindAnimSlot)
            {
                if (bindSkeletonRenderer != null && bindSlotName != null)
                {
                    Vector3 worldPostion, worldScale;
                    Quaternion worldRotation;
                    if (bindSkeletonRenderer.TryGetSlotAttachmentCube(bindSlotName, out worldPostion, out worldScale, out worldRotation))
                    {
                        transform.position = worldPostion;
                        transform.localScale = new Vector3(worldScale.x, worldScale.y, transform.localScale.z); // z保持不变 add by TangJian 2018/12/5 17:43
                        transform.localRotation = worldRotation;

                        mainCollider.enabled = true;


                        if (Definition.Debug)
                        {
                            DebugManager.Instance.AddDrawGizmos("DamageController" + GetInstanceID(), () =>
                            {
                                bindSkeletonRenderer.DrawGizmosSlotAttachmentBox(bindSlotName);
                            });
                        }
                    }
                }
                else
                {
                    mainCollider.enabled = false;
                }
            }
            else if (bindType == FrameEventInfo.RoleAtkFrameEventData.BindType.BindObjectController)
            {
                if (ObjectController != null)
                {
                    transform.position = ObjectController.transform.position;
                    mainCollider.enabled = true;
                }
            }
            else
            {
                mainCollider.enabled = true;
            }


            // 移除自己            
            if (NeedRemoveSelf != null && NeedRemoveSelf())
            {
                NeedRemoveSelf = null;
                
                DelayFunc(() =>
                {
                    DamageManager.Instance.Remove(this);
                }, 0.01f);
            }

            if (OldPos==Vector3.zero&& BeforePos==Vector3.zero)
            {
                OldPos = transform.position;
            }
            else
            {
//                if (Time.time-TimeRecord>0.1f)
//                {
                    BeforePos = new Vector3(OldPos.x,OldPos.y,OldPos.z);
                    OldPos = transform.position;
//                    TimeRecord = Time.time; 
//                }
            }
        }
    }
}