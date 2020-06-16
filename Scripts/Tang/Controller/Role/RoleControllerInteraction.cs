using UnityEngine;
using System;
using DG.Tweening;
using Tang.FrameEvent;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;
using ZS;

namespace Tang
{
    public partial class RoleController
    {
        public virtual void OnInput(string name,InputValue inputValue)
        {
            switch (name)
            {
                case "Move":
                    Vector2 vector2 = inputValue.Get<Vector2>();
                    MoveBy(vector2);
                    break;
                case "WalkCut":
                    if (inputValue.Get<float>() > 0.5)
                    {
                        WalkCutBegin();
                    }
                    else
                    {
                        WalkCutEnd();
                    }
                    break;
                case "Action1":
                    if (inputValue.Get<float>() > 0.5)
                    {
                        Action1Begin();
                    }
                    else
                    {
                        Action1End();
                    }
                    break;
                case "Action2" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        Action2Begin();
                    }
                    else
                    {
                        Action2End();
                    }
                    break;
                case "Jump" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        IntoState1();
                    }
                    break;
                case "Rush" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        IntoRush();
                    }
                    else
                    {
                        ComeOutRush();
                    }
                    break;
                case "Interact":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        Interact();
                    }
                    break;
                case "Roll":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        if (RoleData.FinalVigor > 20)
                        {
                            IntoState3();
                        }
                    }
                    break;
                case "Use":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        Use();
                    }
                    break;
            }
        }

        public void OnXboxInput2(string name, InputValue inputValue)
        {
            switch (name)
            {
                case "XBoxMove":
                    Vector2 vector2 = inputValue.Get<Vector2>();
                    MoveBy(vector2);
                    
                    break;
                case "XBoxAction1":
                    float f = inputValue.Get<float>();
                    if (f > 0.5)
                    {
                        _animator.SetBool("action1", true);
                        _animator.SetInteger("action1_state", 1);
                            
                        _animator.SetTrigger("action1_begin");
                    }
                    else
                    {
                        _animator.SetBool("action1_end", true);
                        _animator.SetInteger("action1_state", 0);
                          
                        _animator.SetTrigger("action1_end");
                    }
                    break;
                case "XBoxAction2" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        _animator.SetInteger("action3_state", 1);

                        _animator.SetTrigger("action3_begin");    
                    }
                    else
                    {
                        _animator.SetInteger("action3_state", 0);
                            
                        _animator.SetTrigger("action3_end");    
                    }
                    break;
                case "XBoxRush" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        isRushing = true;
                    }
                    else
                    {
                        isRushing = false;
                    }
                    break;
                case "XBoxAction3" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        _animator.SetTrigger("action3_2_begin");    
                    }
                    break;
                case "XBoxAction4":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        _animator.SetTrigger("action3_3_begin");    
                    }
                    break;
                case "XBoxAction5":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        _animator.SetTrigger("action2_begin");    
                    }
                    break;
            } 
        }

        public void OnInput2(string name, InputValue inputValue)
        {
            switch (name)
            {
                case "Move":
                    Vector2 vector2 = inputValue.Get<Vector2>();
                    MoveBy(vector2);
                    break;
                case "Action1":
                    float f = inputValue.Get<float>();
                    if (f > 0.5)
                    {
                        Action1Begin();
                    }
                    break;
                case "Action2" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        Action2Begin();
                    }
                    break;
                case "Roll":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        IntoState3();
                    }
                    break;
                case "Rush" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        IntoRush();
                    }
                    else
                    {
                        ComeOutRush();
                    }
                    break;
                case "Jump" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        IntoState2();
                    }
                    break;
                case "Interact":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        
                    }
                    break;
                case "I":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        IntoState1();
                    }
                    break;
                case "O":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        Action4Begin();
                    }
                    break;
                case "P":
                    if (inputValue.Get<float>()>0.5f)
                    {
                       
                    }
                    break;
                case "Y":
                case "M":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        
                    }
                    else
                    {
                        
                    }
                    break;
                case "N":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        
                    }
                    break;
            }
        }
        
        public void ClimbLadder(float jocky)
        {
            TriggerController selfTriggerController = GetComponentInChildren<TriggerController>();
            TriggerController otherTriggerController = selfTriggerController.GetFirstKeepingTriggerController();
            if (selfTriggerController != null && otherTriggerController != null && otherTriggerController.ITriggerDelegate != null)
            {
                GameObject interactObject = otherTriggerController.ITriggerDelegate.GetGameObject();
                LadderController ladderController = interactObject.GetComponent<LadderController>();
                if (ladderController != null)
                {
                    BoxCollider boxCollider = otherTriggerController.gameObject.GetComponent<BoxCollider>();
                    Bounds bou = boxCollider.bounds;
                    if (jocky < 0)
                    {
                        // 上梯子前先转向 add by TangJian 2019/3/23 11:13
                        SetDirectionInt(ladderController.transform.localScale.x > 0 ? -1 : 1);
                    
                        if ((bou.center.y + ((bou.size.y / 2) - 2f)) < transform.position.y)
                        {
                            if (animatorParams.ContainsKey("ladder_type"))
                                _animator.SetInteger("ladder_type", (int)ladderController.laddertype);
                            if (animatorParams.ContainsKey("start_climb_ladder"))
                                _animator.SetBool("start_climb_ladder", true);

                            canClimbLadder = false;
                            //gameObject.layer = LayerMask.NameToLayer("Default");
                            switch (ladderController.laddertype)
                            {
                                case laddertype.Left:
                                    transform.position = new Vector3(interactObject.transform.position.x, transform.position.y - 1.6f, interactObject.transform.position.z);
                                    break;
                                case laddertype.Right:
                                    transform.position = new Vector3(interactObject.transform.position.x, transform.position.y - 1.6f, interactObject.transform.position.z);
                                    break;
                                case laddertype.Center:
                                    transform.position = new Vector3(interactObject.transform.position.x, transform.position.y - 1.6f, interactObject.transform.position.z - 0.1f);
                                    break;
                            }
                            //sdas = true;
                        }
                    }
                    else if (jocky > 0)
                    {
                        // 上梯子前先转向 add by TangJian 2019/3/23 11:13
                        SetDirectionInt(ladderController.transform.localScale.x > 0 ? -1 : 1);

                        if ((bou.center.y + ((bou.size.y / 2) - 2f)) >= transform.position.y)
                        {
                            if (animatorParams.ContainsKey("ladder_type"))
                                _animator.SetInteger("ladder_type", (int)ladderController.laddertype);
                            if (animatorParams.ContainsKey("start_climb_ladder"))
                                _animator.SetBool("start_climb_ladder", true);

                            canClimbLadder = false;

                            //gameObject.layer = LayerMask.NameToLayer("Default");
                            switch (ladderController.laddertype)
                            {
                                case laddertype.Left:
                                    transform.position = new Vector3(interactObject.transform.position.x, transform.position.y, interactObject.transform.position.z);
                                    break;
                                case laddertype.Right:
                                    transform.position = new Vector3(interactObject.transform.position.x, transform.position.y, interactObject.transform.position.z);
                                    break;
                                case laddertype.Center:
                                    transform.position = new Vector3(interactObject.transform.position.x, transform.position.y, interactObject.transform.position.z - 0.1f);
                                    break;
                            }
                            //sdas = true;
                        }
                    }

                    //if (sdas)
                    //{
                    //    if (animatorParams.ContainsKey("ladder_type"))
                    //        _animator.SetInteger("ladder_type", (int)ladderController.laddertype);
                    //    if (animatorParams.ContainsKey("start_climb_ladder"))
                    //        _animator.SetBool("start_climb_ladder", true);

                    //    canClimbLadder = false;
                    //}
                }


            }
        }
        
        public float SelfSuspendTime = 0;

        public float SelfSuspendScale = 0;

        public float OtherSuspendTime = 0;
        public float OtherSuspendScale = 0;

        private Tweener animSpeedDownTween;
        private Tweener animSpeedUpTween;

        private float lastAnimSpeed = 0;
        
        // 击中慢放
        public void AnimSuspend()
        {

            var speed_l = _defaultAnimSpeed * SelfSuspendScale;
            var speed_d = _defaultAnimSpeed;
            var time_d = SelfSuspendTime;
            
            _animator.speed = speed_l;
            DelayFunc("AnimSuspend", () => { _animator.speed = _defaultAnimSpeed; }, time_d);
            
        }
        
        public virtual bool OnHit(DamageData damageData)
        {
            // 事件分发
            if (OnHitEvent != null)
            {
                OnHitEvent(damageData);
            }
            
            // buff刷新 
            BuffController.TriggerBuffEvent(BuffEventType.OnHit, damageData);
            
            // 代理执行击中 add by TangJian 2019/5/10 17:20
            _hitAndHurtDelegate.Hit(damageData);
            
            // 空中击中对象, 自己浮空
            if(isGrounded == false)
            {
                if (damageData.itriggerDelegate is HurtAble && damageData.itriggerDelegate is IObjectController objectController)
                {
                    //击退 add by TangJian 2018/12/8 14:55
                    if (damageData.targetMoveBy.magnitude > 0.01f)
                    {
                        speed.y = (damageData.targetMoveBy *
                                   (1f - (objectController.IsDefending ? objectController.DefenseRepellingResistance : objectController.RepellingResistance))
                            ).MoveByToSpeed().y;
                    }
                }
            }

            // 添加碰撞伤害
//            if (damageData.hitType == (int) HitType.Fly)
//            {
//                this.DelayToDo(0.1f, () =>
//                {
//                    if (damageData.hitGameObject.GetComponent<RoleController>() is IObjectController otherObjController)
//                    {
//                        var damageSize = new Vector3(1, 1, 1);
//                        HitType hittype = HitType.Fly;
//
//                        string damageControllerId = Tools.getOnlyId().ToString();
//                        string damageId = Tools.getOnlyId().ToString();
//
//                        var damageController = FrameEventMethods.RoleAtk((ITriggerDelegate) otherObjController, damageControllerId, damageId,
//                            _roleData.TeamId, FrameEventInfo.ParentType.Transform
//                            , PrimitiveType.Cube
//                            , Vector3.zero
//                            , damageSize
//                            , new Vector3(GetDirectionInt(), 0, 0)
//                            , hittype
//                            , Vector3.zero
//                            , DamageDirectionType.UseSpeed
//                            , DamageEffectType.Strike
//                            , AtkPropertyType.physicalDamage
//                            , 1
//                            , 1);
//
//                        damageController.SetId(damageControllerId);
//                        
//                        // 设计击飞攻击效果为其他
//                        damageController.damageData.HitEffectType = HitEffectType.Other;
//                        
//                        // 绑定到攻击对象的身上
//                        damageController.bindType = FrameEventInfo.RoleAtkFrameEventData.BindType.BindObjectController;
//                        damageController.ObjectController = otherObjController;
//
//                        damageController.NeedRemoveSelf = () => otherObjController.Speed.y <= 0 && otherObjController.IsGrounded();
//                    }
//
//                });    
//            }
            return true;
        }
        public void Test()
        {
            // 添加碰撞伤害
//            this.DelayToDo(0.1f, () =>
//            {
            if (this is IObjectController otherObjController)
            {
                var damageSize = new Vector3(1, 1, 1);
                HitType hittype = HitType.Fly;

                string damageControllerId = Tools.getOnlyId().ToString();
                string damageId = Tools.getOnlyId().ToString();

                var damageController = FrameEventMethods.RoleAtk(this, damageControllerId, damageId,
                    _roleData.TeamId, FrameEventInfo.ParentType.Transform
                    , PrimitiveType.Cube
                    , Vector3.zero
                    , damageSize
                    , new Vector3(GetDirectionInt(), 0, 0)
                    , hittype
                    , Vector3.zero
                    , DamageDirectionType.Directional
                    , DamageEffectType.Strike
                    , AtkPropertyType.physicalDamage
                    , 1
                    , 1);

                damageController.SetId(damageControllerId);

                // 绑定到攻击对象的身上
                damageController.bindType = FrameEventInfo.RoleAtkFrameEventData.BindType.BindObjectController;
                damageController.ObjectController = otherObjController;


//                    damageController.NeedRemoveSelf = () =>
//                    {
//                        return otherObjController.Speed.y <= 0 && otherObjController.IsGrounded();
//                    };
            }

//            });
        }


        public virtual bool OnHurt(DamageData damageData)
        {
            // qte状态下不受伤 add by TangJian 2019/3/21 14:00
            if (IsQte)
                return false;
            
            // 无敌状态 add by TangJian 2018/12/8 15:04
            if (IsInvincible)
            {
                return false;
            }

            // 不攻击自己 add by TangJian 2018/12/8 15:05
            if (damageData.owner == gameObject)
            {
                return false;
            }

            // 判断动画状态, 如果死亡, 则不执行受击 add by TangJian 2018/12/8 15:06
            if (_animator.GetBool("isDead"))
            {
                return false;
            }
            
            HurtPart hurtPart = HurtModeController.GetHurtPart(damageData.hitTarget.name);
            damageData.HurtPart = hurtPart;
            
            return HurtByPart(damageData, hurtPart);
        }
        
        public virtual void OnTriggerIn(TriggerEvent triggerEvent)
        {
            try
            {
                if (triggerEvent.type == TriggerEventType.Default)
                {
                    GameObject gameObject = triggerEvent.otherTriggerController.ITriggerDelegate.GetGameObject();
                    if (gameObject != null)
                    {
                        Properties properties = gameObject.GetComponent<Properties>();
                        if (properties != null)
                        {
                            // Debug.Log("进入交互对象" + gameObject.tag);
                            if (properties.getString("type") == "coin")
                            {
                                var interactive = gameObject.GetComponent<ITriggerDelegate>();
                                if (interactive != null)
                                {
                                    Event evt = new Event();
                                    evt.Type = EventType.ItemPickUp;
                                    evt.Data = GetGameObject();
                                    interactive.OnEvent(evt);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("e = " + e);
                Debug.Log(", triggerEvent.colider.name = " + triggerEvent.colider.name);
                Debug.Log(", triggerEvent.colider.name = " + triggerEvent.otherTriggerController);
            }
        }

        public virtual void OnTriggerOut(TriggerEvent evt) { }

        public virtual void OnTriggerKeep(TriggerEvent evt) { }
    }
}