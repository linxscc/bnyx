using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    using FrameEvent;

    public class DamageManager : MyMonoBehaviour
    {
        private static DamageManager instance;

        public static DamageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<DamageManager>();
                }
                return instance;
            }
        }

        IEnumerator waitRemove(float time, System.Action action)
        {
            for (float timer = time; timer >= 0; timer -= Time.deltaTime)
            {
                yield return 0;
            }
            action();
        }

        ThreadWork threadWork = new ThreadWork();

        public List<DamageController> damageControllers = new List<DamageController>();
        public Dictionary<string, DamageAndTargetData> damgeHitObjTime = new Dictionary<string, DamageAndTargetData>();

        public void Add(DamageController damageController)
        {
            damageControllers.Add(damageController);
        }

        public void Remove(DamageController damageController)
        {
            if (damageController != null)
            {
                damageControllers.Remove(damageController);
                Tools.Destroy(damageController.gameObject);
            }

        }

        public void Remove(string id)
        {
            //List<DamageController> damageControllers = this.damageControllers.FindAll((DamageController dc) =>
            //{
            //    return id == dc.id;
            //});

            //foreach (var damageController in damageControllers)
            //{
            //    Remove(damageController);
            //}

            threadWork.AddSubThreadAction(() =>
            {
                List<DamageController> damageControllers = this.damageControllers.FindAll((DamageController dc) =>
                {
                    return id == dc.id;
                });

                threadWork.AddMainThreadAction(() =>
                {
                    foreach (var damageController in damageControllers)
                    {
                        Remove(damageController);
                    }
                });
            });
        }

        public void DamageHitObj(string damageId, int objId)
        {
            string key = damageId + objId;
            if (damgeHitObjTime.ContainsKey(key))
            {
                damgeHitObjTime[key].times += 1;
                damgeHitObjTime[key].time = Time.time;
            }
            else
            {
                damgeHitObjTime.Add(key, new DamageAndTargetData(Time.time, 1));
            }
        }

        private DamageAndTargetData GetDamageAndTargetData(string damageId, int objId)
        {
            DamageAndTargetData damageAndTargetData;
            if (damgeHitObjTime.TryGetValue(damageId + objId, out damageAndTargetData))
            {
                return damageAndTargetData;
            }
            return null;
        }

        public int GetDamageHitObjTimes(string damageId, int objId)
        {
            DamageAndTargetData damageAndTargetData = GetDamageAndTargetData(damageId, objId);
            return damageAndTargetData == null ? 0 : damageAndTargetData.times;
        }

        public float GetDamageHitObjTime(string damageId, int objId)
        {
            DamageAndTargetData damageAndTargetData = GetDamageAndTargetData(damageId, objId);
            return damageAndTargetData == null ? 0 : damageAndTargetData.time;
        }

        public DamageController CreateDamage(Vector3 position
            , Transform parentTransform
            , FrameEventInfo.ParentType parentType
            , Vector3 size
            , PrimitiveType primitiveType
        )
        {
            var triggerObject = GameObject.CreatePrimitive(primitiveType);
            Collider collider = triggerObject.GetComponent<Collider>();
            collider.isTrigger = true;

            Rigidbody rigidbody = triggerObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            triggerObject.tag = "Damage";
            triggerObject.layer = LayerMask.NameToLayer("Interaction");

            // 隐藏渲染 add by TangJian 2018/10/9 17:20
            triggerObject.GetComponent<Renderer>().enabled = false;

            // 绑定DamgeData add by TangJian 2018/10/9 15:54
            var damageController = triggerObject.AddComponent<DamageController>();
//            DamageData damageData = new DamageData();
//            damageController.damageData = damageData;

            // 设置位置 add by TangJian 2018/10/9 16:08
            {
                if (parentType == FrameEventInfo.ParentType.Transform)
                {
                    triggerObject.transform.parent = parentTransform;
                    triggerObject.transform.localPosition = position;
                }
                else
                {
                    triggerObject.transform.parent = parentTransform.parent;
                    triggerObject.transform.position = parentTransform.TransformPoint(position);
                }
            }

            // 设置大小 add by TangJian 2018/10/9 16:14
            {
                if (primitiveType == PrimitiveType.Cylinder)
                {
                    triggerObject.transform.localScale = new Vector3(size.x, size.y * 0.5f, size.z);
                }
                else
                {
                    triggerObject.transform.localScale = size;
                }
            }


            return damageController;
        }

        public override void Update()
        {
            base.Update();

            threadWork.Update();
        }


        public class DamageAndTargetData
        {
            public float time;
            public int times;

            //public DamageAndTargetData()
            //{
            //    times = 0;
            //    time = Time.time;
            //}

            public DamageAndTargetData(float time, int times)
            {
                this.time = time;
                this.times = times;
            }
        }
    }


    public static class DamageControllerSettingExtensions
    {
        public static DamageController SetDestroyTime(this DamageController target, float time)
        {
            GameObject.Destroy(target.gameObject, time);
            return target;
        }

        public static DamageController SetTeamId(this DamageController target, string teamId)
        {
            target.damageData.teamId = teamId;
            return target;
        }
        public static DamageController SetDamageId(this DamageController target, string teamId)
        {
            target.damageData.DamageId = teamId;
            return target;
        }
        public static DamageController SetRecordType(this DamageController target, RecordType recordType)
        {
            target.damageData.recordType = recordType;
            return target;
        }
        public static DamageController SetDamageForceType(this DamageController target, DamageForceType damageForceType)
        {
            target.damageData.forceType = damageForceType;
            return target;
        }
        public static DamageController SetOwner(this DamageController target, GameObject owner)
        {
            target.damageData.owner = owner;
            return target;
        }

        public static DamageController SetTriggerDelegate(this DamageController target, ITriggerDelegate itriggerDelegate)
        {
            target.damageData.itriggerDelegate = itriggerDelegate;
            return target;
        }

        public static DamageController SetId(this DamageController target, string id)
        {
            target.id = id;
            return target;
        }

        public static DamageController SetHitType(this DamageController target, HitType hitType)
        {
            target.damageData.hitType = (int)hitType;
            return target;
        }

        public static DamageController SetDirection(this DamageController target, Vector3 direction)
        {
            target.damageData.direction = direction;
            return target;
        }

        public static DamageController SetMoveBy(this DamageController target, Vector3 moveBy)
        {
            target.damageData.targetMoveBy = moveBy;
            return target;
        }

        public static DamageController setDamageDirectionType(this DamageController target, DamageDirectionType damageDirectionType)
        {
            target.damageData.DamageDirectionType = damageDirectionType;
            return target;
        }

        public static DamageController SetDamageEffectType(this DamageController target, DamageEffectType damageEffectType)
        {
            target.damageData.damageEffectType = damageEffectType;
            return target;
        }

        public static DamageController SetAtkPropertyType(this DamageController target, AtkPropertyType atkPropertyType)
        {
            target.damageData.atkPropertyType = atkPropertyType;
            return target;
        }

        public static DamageController SetAtk(this DamageController target, float atk)
        {
            target.damageData.atk = atk;
            return target;
        }

        public static DamageController SetMagical(this DamageController target, float magical)
        {
            target.damageData.magical = magical;
            return target;
        }

        public static DamageController SetPoiseCut(this DamageController target, float value)
        {
            target.damageData.poiseCut = value;
            return target;
        }
        public static DamageController SetDamageMass(this DamageController target, float value)
        {
            target.damageData.DamageMass = value;
            return target;
        }

        public static DamageController SetCritical(this DamageController target, bool isCritical)
        {
            target.damageData.isCritical = isCritical;
            return target;
        }

        public static bool IsCritical(this DamageController target)
        {
            return target.damageData.isCritical;
        }

        public static DamageController SetBreakShiled(this DamageController target, bool breakShield)
        {
            target.damageData.breakShield = breakShield;
            return target;
        }
        
        public static DamageController SetIgnoreShiled(this DamageController target, bool ignoreShield)
        {
            target.damageData.ignoreShield = ignoreShield;
            return target;
        }
        
        public static DamageController SetUseForcedOffset(this DamageController target, bool value)
        {
            target.damageData.useForcedOffset = value;
            return target;
        }
        
        public static DamageController SetForcedOffset(this DamageController target, Vector3 value)
        {
            target.damageData.forcedOffset = value;
            return target;
        }
        
        public static DamageController SetForcedOffsetDuration(this DamageController target, float value)
        {
            target.damageData.forcedOffsetDuration = value;
            return target;
        }
        
        public static DamageController SetEffectOrientation(this DamageController target, Vector3 value)
        {
            target.damageData.effectOrientation = value;
            return target;
        }
        
        // 设置顿帧
        public static DamageController SetSelfSuspend(this DamageController target, float selfSuspendTime, float selfSuspendScale)
        {
            target.damageData.selfSuspendTime = selfSuspendTime;
            target.damageData.selfSuspendScale = selfSuspendScale;
            return target;
        }
        
        // 设置力度值
        public static DamageController SetForceValue(this DamageController target, float value)
        {
            target.damageData.forceValue = value;
            return target;
        } 
    }
}