using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;


namespace Tang
{
    public class ActionDataExecutor
    {
        public static async void ExecuteRoleAction(RoleController roleController, ActionData actionData)
        {
            if (actionData.chance>=Random.Range(0,101))
            {
                switch (actionData.type)
                {
                    case ActionType.AddAttr:
                        {
                            roleController.AddAttr((AttrType)actionData.int1, actionData.float1);
                            // roleController.AddAttr(actionData.string1, actionData.float1);
                        }
                        break;
                    case ActionType.AddAnim:
                        {
                            var animGameObject = AnimManager.Instance.PlayAnim(actionData.type + actionData.string1, actionData.string1, actionData.bool1, roleController.CharacterController.bounds.center + new Vector3(0, 0, -0.5f), 1, 1, actionData.float1, roleController.transform);
                        }
                        break;
                    case ActionType.RemoveAnim:
                        {
                            AnimManager.Instance.removeAnim(actionData.type + actionData.string1);
                        }
                        break;
                    case ActionType.SetColor:
                        // 变红
                        roleController.SkeletonAnimator.skeleton.DoColorTo(new Color(actionData.float1, actionData.float2, actionData.float3), 0.2f).OnComplete(() =>
                        {
                            roleController.SkeletonAnimator.skeleton.DoColorTo(Color.white, 0.2f);
                        });
                        break;
                    case ActionType.CreateDamage:
                        var triggerObject = GameObject.Instantiate(await AssetManager.LoadAssetAsync<GameObject>("Assets/Resources_moved/Prefabs/Trigger/TriggerCube.prefab"));
                        triggerObject.tag = "Damage";
                        triggerObject.layer = LayerMask.NameToLayer("Interaction");

                        float destroyDelayTime = 0.1f;

                        Tools.RemoveComponent<TriggerController>(triggerObject);
                        var damageController = triggerObject.AddComponent<DamageController>();

                        // 不渲染 add by TangJian 2017/07/07 22:40:39
                        {
                            var meshRenderer = triggerObject.GetComponent<MeshRenderer>();
                            if (DebugManager.Instance.debugData.debug)
                            {
                                meshRenderer.enabled = true;
                                destroyDelayTime = 1;
                            }
                            else
                            {
                                meshRenderer.enabled = false;
                            }
                        }

                        // 设置大小 add by TangJian 2017/07/07 22:40:40
                        {
                            var boxCollider = triggerObject.GetComponent<BoxCollider>();
                            triggerObject.transform.localScale = new Vector3(actionData.vector3_2.x, actionData.vector3_2.y, actionData.vector3_2.z);
                        }

                        // 设置位置 add by TangJian 2017/07/07 22:34:48
                        {
                            triggerObject.transform.parent = roleController.gameObject.transform.parent;

                            // var a = direction;

                            var x = roleController.gameObject.transform.localPosition.x + actionData.vector3_1.x * roleController.GetDirectionInt();
                            var y = roleController.gameObject.transform.localPosition.y + actionData.vector3_1.y;
                            var z = roleController.gameObject.transform.localPosition.z + actionData.vector3_1.z;

                            triggerObject.transform.localPosition = new Vector3(x, y, z);
                        }

                        var damageData = new DamageData();
                        damageController.damageData = damageData;

                        // 设置持有者 add by TangJian 2017/07/27 20:26:44
                        damageData.teamId = roleController.RoleData.TeamId;
                        damageData.owner = roleController.gameObject;
                        damageData.itriggerDelegate = roleController;

                        // 力度 add by TangJian 2017/07/24 15:16:23
                        {
                            damageData.force = new Vector3(roleController.GetDirectionInt() * actionData.vector3_3.x, actionData.vector3_3.y, actionData.vector3_3.z);
                        }

                        // 攻击方向 add by TangJian 2017/07/24 15:17:37
                        {
                            damageData.direction = new Vector3(roleController.GetDirectionInt() * (int)actionData.int2, 0, 0);
                        }

                        // 攻击类别 add by TangJian 2017/07/24 15:17:35
                        {
                            damageData.hitType = (int)actionData.int1;
                        }

                        // 攻击击退 add by TangJian 2017/08/29 22:18:55
                        // {
                        //     damageData.targetMoveBy = new Vector3(roleController.getDirectionInt() * roleAtkFrameEventData.targetMoveBy.x, roleAtkFrameEventData.targetMoveBy.y, roleAtkFrameEventData.targetMoveBy.z);
                        // }

                        // 伤害效果类型 add by TangJian 2017/08/16 15:33:01
                        // {
                        //     var mainHandWeapon = RoleData.EquipData.getMainHand<WeaponData>();
                        //     if (mainHandWeapon != null)
                        //     {
                        //         damageData.damageEffectType = mainHandWeapon.damageEffectType;
                        //     }
                        // }

                        // 攻击力 add by TangJian 2017/07/24 15:17:22
                        // damageData.atk = roleData.Atk * roleAtkFrameEventData.atk;

                        // 暴击 add by TangJian 2017/12/20 21:50:30
                        // if (roleData.FinalCriticalRate >= UnityEngine.Random.Range(0.0f, 1.0f))
                        // {
                        //     //damageData.isCritical = true;
                        //     //damageData.atk *= roleData.CriticalDamage;
                        // }

                        triggerObject.DestoryChildren();
                        Tools.Destroy(triggerObject, destroyDelayTime);
                        break;
                }
            }
            
        }

        public static void ExecuteRoleActions(RoleController roleController, List<ActionData> actionDatas)
        {
            foreach (var actionData in actionDatas)
            {
                ExecuteRoleAction(roleController, actionData);
            }
        }
    }
}