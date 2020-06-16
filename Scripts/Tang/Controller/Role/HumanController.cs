using System;
using FairyGUI;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tang
{
    public class HumanController : RoleController
    {
        public override void Start()
        {
            base.Start();
            InitWeapon();
            InitUIMessage();
        }

        private void InitWeapon()
        {
            if (RoleData.EquipData.MainHand != null && !string.IsNullOrEmpty(RoleData.EquipData.MainHand.id)) return;
            RoleData.EquipData.MainHand = ItemManager.Instance.getItemDataById<WeaponData>("");
            RoleData.EquipData.OffHand = ItemManager.Instance.getItemDataById<WeaponData>("");
        }

        private void InitUIMessage()
        {
            // 精力
            _valueMonitorPool.AddMonitor(() => Vigor, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_VIGOR, new object []{f1, VigorMax});
            }, true);
            
            _valueMonitorPool.AddMonitor(() => Vigor, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_VIGOR, new object []{f1, VigorMax});
            }, true);
            
            _valueMonitorPool.AddMonitor(() => FinalHp, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_FinalHp, new object []{FinalHp, FinalHpmax});
            }, true);
            _valueMonitorPool.AddMonitor(() => Exp, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_Exp, new object []{Exp});
            }, true);
            _valueMonitorPool.AddMonitor(() => Level, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_Level, new object []{Level});
            }, true);
            _valueMonitorPool.AddMonitor(() => FinalAtk, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_Damage, new object []{FinalAtk,FinalDef});
            }, true);
            _valueMonitorPool.AddMonitor(() => FinalMovingSpeed, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_MovingSpeed, new object []{FinalMovingSpeed});
            }, true);
            _valueMonitorPool.AddMonitor(() => FinalCritical, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_FinalCritical, new object []{FinalCritical,FinalCriticalDamage});
            }, true);
            
            // 血量
            _valueMonitorPool.AddMonitor(() => Hp, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_HP, new object []{Hp, HpMax});
            }, true);
            
            _valueMonitorPool.AddMonitor(() => HpMax, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_HP, new object []{Hp, HpMax});
            }, true);
            
            // 魔力
            _valueMonitorPool.AddMonitor(() => Mp, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_MP, new object []{Mp, MpMax});
            }, true);
            
            _valueMonitorPool.AddMonitor(() => MpMax, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_MP, new object []{Mp, MpMax});
            }, true);
            
            
            _valueMonitorPool.AddMonitor(() => Money, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_Money, new object []{Money});
            }, true);
            _valueMonitorPool.AddMonitor(() => Soul, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_Soul, new object []{Soul});
            }, true);
            _valueMonitorPool.AddMonitor(() => SoulIcon, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_SoulIcon, new object []{SoulIcon});
            }, true);
            _valueMonitorPool.AddMonitor(() => ConsumableItemList, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_ConsumableItemList, new object []{ConsumableItemList});
            }, true);
            _valueMonitorPool.AddMonitor(() => CurrConsumableItemIndex, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_CurrConsumableItemIndex, new object []{CurrConsumableItemIndex});
            }, true);
            _valueMonitorPool.AddMonitor(() => ComBo, (f, f1) =>
            {
                MessageManager.Instance.Dispatch(MessageName.SET_PLAYER1_Combo, new object []{f1});
            }, true);
        }

        public override void RefreshRoleAnim(SkeletonAnimator skeletonAnimator)
        {
            Debug.Log("RefreshRoleAnim: " + name);

            if (RoleData.EquipData.HasMainHand())
            {
                SkeletonAnimator.AddSkin("MainHand", RoleData.EquipData.MainHand.mainHandAttachmentName);

                //RefreshSkin(PartSkinType.MainHand, RoleData.EquipData.MainHand.mainHandAttachmentName);
            }
            else
            {
                SkeletonAnimator.AddSkin("MainHand", null);
                //RefreshSkin(PartSkinType.MainHand, "NONE");
            }

            //设置副手武器皮肤 
            if (RoleData.EquipData.HasOffHand())
            {
                SkeletonAnimator.AddSkin("OffHand", RoleData.EquipData.OffHand.offHandAttachmentName);
            }
            else
            {
                SkeletonAnimator.AddSkin("OffHand", null);
            }

            //设置护甲皮肤
            if (RoleData.EquipData.armorData != null && RoleData.EquipData.armorData.skinName != null && RoleData.EquipData.HasArmorData())
            {
                SkeletonAnimator.AddSkin("Armor", RoleData.EquipData.armorData.skinName);
            }
            else
            {
                SkeletonAnimator.AddSkin("Armor", null);
            }
        }

        public override void SkillBegin(SkillData skillData)
        {
            if (skillData != null && skillData.id != null)
            {
                currSkillData = skillData;
                switch (currSkillData.id.ToLower())
                {
                    case "fireball":
                        // animator.SetInteger("castType", 1);
                        OnEvtSkill();
                        break;
                    case "arrow":
                        // animator.SetInteger("castType", 1);
                        break;
                    default:
                        Debug.LogError("没有技能:" + currSkillData.id);
                        break;
                }
            }
        }

        public override void SkillEnd(SkillData skillData)
        {
            currSkillData = skillData;
        }
     
        public override void OnEvtSkill()
        {
            if (RoleData.EquipData.SoulData.soulCharging.CanUse())
            {
                Cast(currSkillData);
            }
        }

        public override void Cast(SkillData skillData)
        {
            if (skillData != null && RoleData.Mp >= currSkillData.cost)
            {
                RoleData.EquipData.SoulData.soulCharging.Reset();
                // roleData.Mp -= currSkillData.cost;
                switch (skillData.id.ToLower())
                {
                    case "fireball":
                    {
                        // roleData.Mp -= currSkillData.cost;

                        // 发射火球术
                        var fireBall = GameObjectManager.Instance.Spawn("FireBall");

                        fireBall.transform.parent = transform.parent;
                        fireBall.transform.localPosition = transform.localPosition + new Vector3(0, 1, 0);

                        FlySkillController flySkillController = fireBall.GetComponent<FlySkillController>();

                        flySkillController.UseGravity = false;
                        flySkillController.Speed = new Vector3(GetDirectionInt() * 10, 0, 0);

                        flySkillController.MainDamageData.owner = gameObject;
                        flySkillController.MainDamageData.teamId = RoleData.TeamId;

                        flySkillController.MainDamageData.atk = RoleData.Atk * 2;
                        flySkillController.MainDamageData.force = new Vector3(1, 1, 1) * 10;
                        flySkillController.MainDamageData.DamageDirectionType = DamageDirectionType.Radial;
                    }
                        break;
                    case "arrow":
                    {
                        RoleData.Mp -= currSkillData.cost;

                        var arrow = GameObjectManager.Instance.Spawn("Arrows");

                        arrow.transform.parent = transform.parent;
                        arrow.transform.localPosition = transform.localPosition + new Vector3(GetDirectionInt() * 0.2f, 1.4f, 0);

                        ArrowsController arrowController = arrow.GetComponent<ArrowsController>();
                        arrowController.speed = new Vector3(GetDirectionInt() * 20, 2, 0);

                        arrowController.DamageController.damageData.owner = gameObject;
                        arrowController.DamageController.damageData.teamId = RoleData.TeamId;
                        arrowController.DamageController.damageData.itriggerDelegate = this;

                        arrowController.DamageController.damageData.atk = RoleData.Atk * 1.5f;
                    }
                        break;
                    default:
                        Debug.LogError("没有技能:" + currSkillData.id);
                        break;
                }
            }
        }

        public override void Interact()
        {
            TriggerController selfTriggerController = gameObject.GetChild("InteractiveCube").GetComponent<TriggerController>();
            TriggerController otherTriggerController = selfTriggerController.GetFirstKeepingTriggerController();
            if (selfTriggerController != null && otherTriggerController != null && otherTriggerController.ITriggerDelegate != null)
            {
                GameObject interactObject = otherTriggerController.ITriggerDelegate.GetGameObject();

                if (interactObject.tag == "NPC")
                {
                    RolePromptController rolePromptController = GetComponentInChildren<RolePromptController>();
                    rolePromptController.killtalk();
                    OnInteractAction(interactObject);
                    AnimManager.Instance.PrintBubble("[color=#FFFFFF,#000000]fgtdfg[/color]", interactObject);
                }
                else if (interactObject.tag == "Ladder")
                {
                    LadderController ladderController = interactObject.GetComponent<LadderController>();
                    BoxCollider boxCollider = otherTriggerController.gameObject.GetComponent<BoxCollider>();
                    Bounds bou = boxCollider.bounds;
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    if ((interactObject.transform.position.y + bou.center.y + ((bou.size.y / 2) - 1.6f)) < transform.position.y)
                    {
                        switch (ladderController.laddertype)
                        {
                            case laddertype.Left:
                                transform.position = new Vector3(interactObject.transform.position.x + CharacterController.radius, transform.position.y - 1.6f, interactObject.transform.position.z);
                                break;
                            case laddertype.Right:
                                transform.position = new Vector3(interactObject.transform.position.x - CharacterController.radius, transform.position.y - 1.6f, interactObject.transform.position.z);
                                break;
                            case laddertype.Center:
                                transform.position = new Vector3(interactObject.transform.position.x, transform.position.y - 1.6f, interactObject.transform.position.z - 0.1f);
                                break;
                        }
                    }
                    else
                    {
                        switch (ladderController.laddertype)
                        {
                            case laddertype.Left:
                                transform.position = new Vector3(interactObject.transform.position.x + CharacterController.radius, transform.position.y, interactObject.transform.position.z);
                                break;
                            case laddertype.Right:
                                transform.position = new Vector3(interactObject.transform.position.x - CharacterController.radius, transform.position.y, interactObject.transform.position.z);
                                break;
                            case laddertype.Center:
                                transform.position = new Vector3(interactObject.transform.position.x, transform.position.y, interactObject.transform.position.z - 0.1f);
                                break;
                        }
                    }
                    if (animatorParams.ContainsKey("ladder_type"))
                        _animator.SetInteger("ladder_type", (int)ladderController.laddertype);
                    if (animatorParams.ContainsKey("start_climb_ladder"))
                        _animator.SetBool("start_climb_ladder", true);
                }
                else if (interactObject.GetComponent<IInteractable>() != null)
                {
                    IInteractable interactable = interactObject.GetComponent<IInteractable>();
                    if (interactable.CanInteract())
                    {
                        interactable.Interact();
                    }
                }
                else if (interactObject.GetComponent<PortalController>())
                {
                    PortalController portalController = interactObject.GetComponent<PortalController>();
                    if (portalController)
                        this.OpenPortal(portalController);
                }
                else if (interactObject.GetComponent<FenceDoorController>())
                {
                    FenceDoorController fenceDoorController = interactObject.GetComponent<FenceDoorController>();
                    if (fenceDoorController)
                        fenceDoorController.OpenPortal();
                }
                else
                {
                    if (interactObject != null && interactObject.Equals(null) == false)
                    {
                        DropItemController dropItemController = interactObject.GetComponent<DropItemController>();
                        if (dropItemController != null)
                        {
                            var itemData = ItemManager.Instance.getItemDataById<ItemData>(dropItemController.ItemId);
                            GameManager.Instance.PickupTips("Textures/Icon/" + itemData.icon, itemData.name, itemData.count.ToString());
                            switch (itemData.itemType)
                            {
                                case ItemType.Consumable:
                                    Debug.Log("捡到消耗品 id = " + itemData.id);
                                    
                                    Destroy(interactObject);
                                    ConsumableData consumableData = ItemManager.Instance.getItemDataById<ConsumableData>(itemData.id);
                                    if (consumableData != null)
                                    {
                                        switch (consumableData.consumableUseType)
                                        {
                                            case ConsumableUseType.deposit:
                                                RoleData.AddConsumableItem(itemData.id, itemData.count);
                                                PickupConsumable();
                                                break;
                                            case ConsumableUseType.ImmediateUse:
                                                BuffController.AddBuff(consumableData.buffId);
                                                break;
                                        }
                                    }
                                    break;
                                case ItemType.Other:
                                    RoleData.OtherItemList.Add(itemData);
                                    dropItemController.RemoveSelf();
                                    break;
                                case ItemType.Equip:
                                    var weaponData = ItemManager.Instance.getItemDataById<WeaponData>(dropItemController.ItemId);
                                    if (weaponData != null)
                                    {
                                        this.JuageEquipPossessType(weaponData);
                                    }
                                    else
                                    {
                                        var equipData = ItemManager.Instance.getItemDataById<EquipData>(itemData.id);
                                        RoleData.EquipData.UseEquip(equipData, interactObject.transform.localPosition + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1.0f, 1.5f), Random.Range(-0.5f, 0.5f)));
                                    }
                                    Destroy(interactObject);
                                    break;
                            }
                        }
                        else
                        {
                            var interactive = interactObject.GetComponent<ITriggerDelegate>();
                            if (interactive != null)
                            {
                                Event evt = new Event();
                                evt.Type = EventType.Interact;
                                evt.Data = GetGameObject();
                                interactive.OnEvent(evt);
                            }
                        }
                    }
                }
            }
        }
        
        public override bool OnHurt(DamageData damageData)
        {
            bool sda = base.OnHurt(damageData);
            if (sda)
            {
                ComBo = 0;
                comboCameraScale();
            }

            return sda;
        }
        
        public override bool OnHit(DamageData damageData)
        {
            base.OnHit(damageData);
            if (damageData.hitGameObject.tag == "Role" || damageData.hitGameObject.tag == "Player")
            {
                

                ComBo++;
                comboCameraScale();
                DelayFunc("ComboClear", () =>
                {
                    ComBo = 0;
                    comboCameraScale();
                }, 2f);
            }
            return true;
        }

        private Transition transition;
        int showState = 0;
        private GComponent Gaming;
        private GComponent ui;
        private UIPanel uiPanel;
        
        public override void Use()
        {
            ConsumableData consumableData = RoleData.GetCurrConsumableItem();
            if (consumableData != null)
            {
                RoleData.SubConsumableItem(consumableData.id);
                BuffController.AddBuff(consumableData.buffId);
            }
            else
            {
                Debug.Log("身上没有消耗品");
            }
        }
  
        protected override void RefreshAnimator()
        {
            bool isSuccess = false;

            var mainHandWeapon = RoleData.EquipData.getMainHand<WeaponData>();
            var offHandWeapon = RoleData.EquipData.getOffHand<WeaponData>();
            EquipType mainHandWeaponType = EquipType.None;
            EquipType offHandWeaponType = EquipType.None;
            if (mainHandWeapon != null)
            {
                mainHandWeaponType = mainHandWeapon.equipType;
            }
            if (offHandWeapon != null)
            {
                offHandWeaponType = offHandWeapon.equipType;
            }

            foreach (var weaponMixData in AnimManager.Instance.WeaponMixDataList)
            {
                if (mainHandWeaponType == weaponMixData.mainHand && offHandWeaponType == weaponMixData.offHand)
                {
                    Debug.Log("动画状态机切换为: " + weaponMixData.runtimeAnimatorController.name);
                    _animator.runtimeAnimatorController = weaponMixData.runtimeAnimatorController;

                    ReInitAnimator();

                    isSuccess = true;
                    break;
                }
            }

            if (isSuccess)
            {
                Debug.Log("没有找到与" + mainHandWeaponType.ToString() + ", " + offHandWeaponType.ToString() + "对应的动画状态机");
            }
        }
        
        public override void UpdateMovement(string stateName, AnimatorStateEventType eventType, Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex, float time)
        {
            AnimatorStateInfo currStateInfo = stateInfo;
            
            AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(layerIndex);
            bool hasNext = nextStateInfo.fullPathHash != 0;
            
            if (hasNext)
            {
                currStateInfo = nextStateInfo;
            }
            else if (skeletonAnimator.IsInterruptionActive(layerIndex))
            {
                currStateInfo = skeletonAnimator.GetInterruptingStateInfo(layerIndex);
            }

            if (eventType == AnimatorStateEventType.OnStateUpdate)
            {
                if (lastUpdateMovementTime < 0)
                    lastUpdateMovementTime = Time.time;
                
                var deltaTime = Time.time - lastUpdateMovementTime;
                lastUpdateMovementTime = Time.time;
            
                if (
                    currStateInfo.IsTag("hurt")
                    || currStateInfo.IsTag("hurtHold"))
                {
                    CalcSpeed(deltaTime, Vector2.zero);
                    speed = CalcGravity(speed, deltaTime);
                    Move(Speed * deltaTime);
                }
                else if (currStateInfo.IsTag("idle"))
                {
                    speed = CalcGravity(speed, deltaTime);
                    
                    TurnBackWithAnim();

                    if (joystick.magnitude < 0.5f)
                    {
                        speed.x = 0;
                        speed.z = 0;    
                    }
                    
                    Move(Speed * deltaTime);
                }
                else if (
                    currStateInfo.IsTag("move")
                    || currStateInfo.IsTag("run")
                    || currStateInfo.IsTag("walk")
                    // || currAnimatorStateInfo.IsTag("accelerateRoll")
                )
                {
                    // 倒播移动
                    if (IsAltPressed && joystick.x * GetDirectionInt() < 0)
                    {
                        SkeletonAnimator.SetBackwards(true);
                    }
                    else
                    {
                        SkeletonAnimator.SetBackwards(false);
                    }
                    
                    speed = CalcGravity(speed, deltaTime);
                    
                    TurnBackWithAnim();
                
                    CalcSpeed(deltaTime);
                    Move(Speed * deltaTime);
                }
                else if (
                    currStateInfo.IsTag("rush")
                )
                {
                    speed = CalcGravity(speed, deltaTime);
                    
                    TurnBackWithAnim();
                    
                    CalcSpeed(deltaTime, new Vector2(joystick.x, joystick.y / 2), RushSpeed);
                    Move(Speed * deltaTime);
                }
                else if (
                    currStateInfo.IsTag("rush_end")
                )
                {
                    speed = CalcGravity(speed, deltaTime);
                    
                    TurnBackWithAnim();
                
                    CalcSpeed(deltaTime, Vector2.zero);
                    Move(Speed * deltaTime);
                }
                else if (currStateInfo.IsTag("attack"))
                {
                    speed = CalcGravity(speed, deltaTime);
                    
                    CalcSpeed(deltaTime, Vector2.zero);
                    Move(Speed * deltaTime);
                }
                else if (currStateInfo.IsTag("jumping"))
                {
                    speed = CalcGravity(speed, deltaTime);
                    
//                    TurnBack();

                    CalcSpeed(deltaTime);


                    if (joystick.magnitude <= 0.1f)
                    {
                        speed.x = 0;
                        speed.z = 0;
                    }

                    if (speed.x * GetDirectionInt() < 0)
                    {
                        Move(new Vector3(Speed.x / 2f, Speed.y, Speed.z) * deltaTime);
                    }
                    else
                    {
                        Move(Speed * deltaTime);
                    }
                }
                else if (currStateInfo.IsTag("jumpingAttack"))
                {
                    speed = CalcGravity(speed, deltaTime);
                    
                    CalcSpeed(deltaTime, Vector2.zero);
                    Vector3 finalSpeed = Vector3.zero;
                    finalSpeed.x = Speed.x;
                    finalSpeed.y = Speed.y * currVariableSpeed;
                    finalSpeed.z = Speed.z;
                    Move(new Vector3(finalSpeed.x / 5f, finalSpeed.y, finalSpeed.z / 5f) * deltaTime);
                }
                else if (currStateInfo.IsTag("landing"))
                {
                    speed = CalcGravity(speed, deltaTime);
                    
                    CalcSpeed(deltaTime, Vector2.zero);
                    Move(Speed * deltaTime);
                }
                else if (currStateInfo.IsTag("nomove"))
                {
                    speed = CalcGravity(speed, deltaTime);
                }
                else if (currStateInfo.IsTag("dun-idle") || currStateInfo.IsTag("dun-move"))
                {
                    speed = CalcGravity(speed, deltaTime);
                    
                    CalcSpeed(deltaTime);
                    Move(Speed / 3f * deltaTime);
                }
                else if (currStateInfo.IsTag("nocontrol"))
                {
                    speed = CalcGravity(speed, deltaTime);
                    
                    CalcSpeed(deltaTime, Vector2.zero);
                    Move(Speed * deltaTime);
                }
                else if (currStateInfo.IsTag("climb"))
                {
                    speed = CalcGravity(speed, deltaTime);
                    
                    speed.y = 0;
                }
                else if (currStateInfo.IsTag("ClimbLadder"))
                {
                    speed = CalcGravity(speed, deltaTime);
                    
                    speed.y = 0;
                    if (joystick.y != 0f)
                    {
                        OverClimbLadder();
                    }
                }
                else if (currStateInfo.IsTag("jumpattack"))
                {
                    speed = CalcGravity(speed, deltaTime);
                    CalcSpeed(deltaTime, Vector2.zero);
                    Move(new Vector3(Speed.x / 10f, Speed.y, Speed.x / 10f) * deltaTime);
                }
                else
                {
                    speed = CalcGravity(speed, deltaTime);
                    CalcSpeed(deltaTime, Vector2.zero);
                    Move(Speed * deltaTime);
                }
            }
            else if (eventType == AnimatorStateEventType.OnStateExit)
            {
                if (
                    currStateInfo.IsTag("walk")
                    || currStateInfo.IsTag("run")
                    || currStateInfo.IsTag("move")
                    // || currAnimatorStateInfo.IsTag("accelerateRoll")
                )
                {
                    skeletonAnimator.SetBackwards(false);
                }
            }
        }

        public override bool IntoState2()
        {
            Interact();
            return true;
        }

        public override bool IntoState4()
        {
            Use();
            return true;
        }
    }
    
    
}