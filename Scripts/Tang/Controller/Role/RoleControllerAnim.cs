using System;
using System.Security.Cryptography;
using CSScriptLibrary;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using UnityEngine;
using Tang.Animation;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;

namespace Tang
{
    using  FrameEvent;
    
    public partial class RoleController
    {
        // 判断当前动画的 tag add by TangJian 2018/12/8 13:50
        public bool CurrAnimStateIsTag(string animTag)
        {
            return this.RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag(animTag);
        }

        // 判断当前动画名称 add by TangJian 2018/12/8 13:50
        public bool CurrAnimStateIsname(string animname)
        {
            return this.RoleAnimator.GetCurrentAnimatorStateInfo(0).IsName(animname);
        }

        // 初始化动画状态机 add by TangJian 2018/12/8 1:34
        public virtual void InitAnimator()
        {
            // animator add by TangJian 2017/07/13 23:29:04
            {
                _animator = GetComponentInChildren<Animator>();
                _animator.enabled = false;

                foreach (var parameter in _animator.parameters)
                {
                    animatorParams.Add(parameter.name, true);
                }
            }

            // 动画控制器 add by TangJian 2017/07/24 14:45:15
            if (_roleAnimController == null)
                _roleAnimController = GetComponentInChildren<AnimController>();

            // 骨骼动animator add by TangJian 2017/07/13 23:29:26
            if (skeletonAnimator == null)
                skeletonAnimator = GetComponentInChildren<SkeletonAnimator>();

            if (_animRenderer == null)
            {
                _animRenderer = skeletonAnimator.GetComponent<Renderer>();

                // 设置zorder add by TangJian 2018/06/07 17:47:51
//                _animRenderer.sortingOrder = (int)ZOrder.ObjectMin;


                // animatorStateInfo add by TangJian 2017/07/13 23:29:39
                _currAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);

                // 动画速度设置 add by TangJian 2017/07/13 23:29:56
                _animator.speed = DefaultAnimSpeed;

                // 注册动画重置事件
                skeletonAnimator.ResetAnimDelegates -= RefreshRoleAnim;
                skeletonAnimator.ResetAnimDelegates += RefreshRoleAnim;

                skeletonAnimator.MoveByAnimDelegates -= MoveByAnim;
                skeletonAnimator.MoveByAnimDelegates += MoveByAnim;
            }
            
            // 状态机状态监控 add by TangJian 2019/4/4 11:57
            skeletonAnimator.GetComponent<AnimatorStateTransmit>().OnStateEvents += UpdateMovement;
        }
        
        // 刷新动画状态机 add by TangJian 2018/12/8 1:35
        protected virtual void RefreshAnimator()
        {
        }

        public void SetAnimatorController(string animatorControllerPath)
        {
            RuntimeAnimatorController runtimeAnimatorController =
                AssetManager.LoadAssetAtPath<RuntimeAnimatorController>(animatorControllerPath);
            
            SetAnimatorController(runtimeAnimatorController);   
        }
        
        public void SetAnimatorController(RuntimeAnimatorController runtimeAnimatorController)
        {
            _animator.runtimeAnimatorController = runtimeAnimatorController;

            ReInitAnimator();   
        }
        
        // 重新初始化动画状态机 add by TangJian 2018/12/8 1:34
        public virtual void ReInitAnimator()
        {
            skeletonAnimator.Initialize(true);

            _animator = GetComponentInChildren<Animator>();


            // animator add by TangJian 2017/07/13 23:29:04
            {
                animatorParams.Clear();
                foreach (var parameter in _animator.parameters)
                {
                    animatorParams.Add(parameter.name, true);
                }
            }
        }

        // 刷新角色动画 add by TangJian 2017/07/18 12:02:50
        public virtual void RefreshRoleAnim(SkeletonAnimator skeletonAnimator)
        {
        }

        // 刷新动画状态机状态 add by TangJian 2017/07/13 23:25:55
        public virtual void UpdateAnimatorState()
        {
            //  碰撞体

            #region 更新角色碰撞体

            if (_canCollider)
            {
                switch (collisionState)
                {
                    case RoleCollisionState.Default:
                    {
                        if (IsGrounded())
                        {
                            if (RoleData.CollideWithRole)
                            {
                                gameObject.layer = LayerMask.NameToLayer("CollideWithRole");
                            }
                            else
                            {
                                gameObject.layer = LayerMask.NameToLayer("Role");
                            }
                        }
                        else
                        {
                            gameObject.layer = LayerMask.NameToLayer("RoleCollisionWithoutRole");
                        }
                    }
                        break;
                    case RoleCollisionState.WithoutRole:
                        gameObject.layer = LayerMask.NameToLayer("RoleCollisionWithoutRole");
                        break;
                    case RoleCollisionState.Climb:
                        gameObject.layer = LayerMask.NameToLayer("Default");
                        break;
                }
            }

            
            #endregion


            // 是否在地面 add by TangJian 2017/07/04 21:44:23
            _animator.SetBool("isGrounded", IsGrounded());
            _animator.SetBool("isGround", IsGrounded());
            
            //自身方向
            _animator.SetInteger("self_direction",GetDirectionInt());
            
            
            // 刷新移动状态 add by TangJian 2019/4/16 23:06
            if(WithAI == false)
                if (joystick.magnitude > 0.1)
                {
                    if (isRushing)
                    {
                        MoveState = MoveState.Rush;
                    }
                    else if (IsRunning)
                    {
                        MoveState = MoveState.Run;
                    }
                    else if (IsWalking)
                    {
                        MoveState = MoveState.Walk;
                    }
                    else
                    {
                        MoveState = MoveState.Walk;
                    }
                }
                else
                {
                    MoveState = MoveState.Idle;
                }
            
            // 运动状态设置 add by TangJian 2019/4/17 16:46
            _animator.SetInteger("move_state", (int)MoveState);
            _animator.SetFloat("move_state", (float)MoveState);
            
            // 速度状态 add by TangJian 2017/07/04 21:44:24
            {
                _animator.SetFloat("speed_x", Speed.x);
                _animator.SetFloat("speed_z", Speed.z);
                _animator.SetFloat("speedy", Speed.y);
            }

            {
                if (IsDead)
                {
                    _animator.SetBool("isDead", true);
                }
            }
            
            _animator.SetFloat("MoveSpeed", Speed.magnitude);
        }


        // 设置动画enable add by TangJian 2018/12/8 13:54
        public void SetAnimVisble(bool b)
        {
            _animRenderer.enabled = b;
        }

        // 自定义帧事件 add by TangJian 2018/12/8 13:55
        public void OnCustom(FrameEventInfo.CustomFrameEventData customFrameEventData)
        {
            switch (customFrameEventData.eventType)
            {
                case FrameEventInfo.CustomFrameEventData.EventType.HoveringBegin:
                    Debug.Log("进入浮空状态");
                    IsHovering = true;
                    break;
                case FrameEventInfo.CustomFrameEventData.EventType.HoveringEnd:
                    Debug.Log("离开浮空状态");
                    IsHovering = false;
                    break;
                case FrameEventInfo.CustomFrameEventData.EventType.CameraShake:
                    AnimManager.Instance.Shake(0.5f, 0.75f, 80, 60);
                    break;
                case FrameEventInfo.CustomFrameEventData.EventType.NoHurtBegin:
                    Debug.Log("进入无敌状态");
                    IsInvincible = true;
                    break;
                case FrameEventInfo.CustomFrameEventData.EventType.NoHurtEnd:
                    Debug.Log("离开无敌状态");
                    IsInvincible = false;
                    break;
                case FrameEventInfo.CustomFrameEventData.EventType.MoveToTarget:
                {
                    RoleBehaviorTree roleBehaviorTree = GetComponent<RoleBehaviorTree>();
                    if (roleBehaviorTree != null)
                    {
                        if (roleBehaviorTree.TargetController != null)
                        {
                            Vector3 targetpos = roleBehaviorTree.TargetController.transform.position;
                            Vector3 selfpos = transform.position;
                            float posx;
                            float posy;
                            if (Mathf.Abs(targetpos.x - selfpos.x) > 8 && Mathf.Abs(targetpos.x - selfpos.x) < 10)
                            {
                                float fl = (targetpos.x - selfpos.x) > 0 ? -8 : 8;
                                posx = targetpos.x - selfpos.x + fl;
                            }
                            else if (Mathf.Abs(targetpos.x - selfpos.x) >= 10)
                            {
                                float fl = (targetpos.x - selfpos.x) > 0 ? 1 : -1;
                                posx = fl * 2f;
                            }
                            else
                            {
                                posx = 0f;
                            }
                            if (Mathf.Abs(targetpos.z - selfpos.z) > 2 && Mathf.Abs(targetpos.z - selfpos.z) < 4)
                            {
                                float fl = (targetpos.z - selfpos.z) > 0 ? -2 : 2;
                                posy = targetpos.z - selfpos.z + fl;
                            }
                            else if (Mathf.Abs(targetpos.z - selfpos.z) >= 4)
                            {
                                float fl = (targetpos.z - selfpos.z) > 0 ? 1 : -1;
                                posy = fl * 2f;
                            }
                            else
                            {
                                posy = 0f;
                            }
                            Vector3 movepos = new Vector3(posx, 0, posy);
                            Debug.Log("MoveToTarget:" + movepos);
                            //Speed = new Vector3(
                            //((targetpos.z - selfpos.z) > 0 ? 1 : -1) * Mathf.Sqrt(Mathf.Abs(2 * GroundFrictionAcceleration * movepos.x)),
                            //Mathf.Sqrt(2 * GrivityAcceleration * movepos.y),
                            //((targetpos.z - selfpos.z) > 0 ? 1 : -1) * Mathf.Sqrt(Mathf.Abs(2 * GroundFrictionAcceleration * movepos.z)));
                            this.DoMoveBy(transform.position + movepos, 0.07f);
                            //Move(movepos);
                        }
                    }
                }
                    break;
                case FrameEventInfo.CustomFrameEventData.EventType.OrientateToTarget:
                {
                    RoleBehaviorTree roleBehaviorTree = GetComponent<RoleBehaviorTree>();
                    if (roleBehaviorTree != null && roleBehaviorTree.TargetController != null)
                    {
                        float div = roleBehaviorTree.TargetController.transform.position.x - transform.position.x;
                        if (div > 0)
                        {
                            SetDirectionInt(1);
                        }
                        else if (div < 0)
                        {
                            SetDirectionInt(-1);
                        }
                        else
                        {

                        }
                    }
                }
                    break;
                case FrameEventInfo.CustomFrameEventData.EventType.AddCustomFrameEventBatchId :
                    skeletonAnimator.CustomFrameEventBatchId++;
                    break;
                case FrameEventInfo.CustomFrameEventData.EventType.SetJoystickDirection:
                    //if((joystick.x>0&&GetDirectionInt()<0)|| (joystick.x < 0 && GetDirectionInt() > 0))
                    //{
                    //    SetDirectionInt(joystick.x>0?1:-1);
                    //}
                    if (_animator.GetFloat("relative_speed_x") < 0)
                    {
                        SetDirectionInt(GetDirectionInt() > 0 ? -1 : 1);
                    }
                    break;
                case FrameEventInfo.CustomFrameEventData.EventType.RoleAiFlyTo:
                    if (GetComponent<RoleBehaviorTree>()  != null)
                    {
                        if (GetComponent<RoleBehaviorTree>().TargetController != null)
                        {
                            Speed = Tools.GetFlyToPosSpeed(new Vector2(1, 1), GetComponent<RoleBehaviorTree>().TargetController.transform.position - transform.position + new Vector3(0, 1f, 0), -60f);
                        }
                    }
                    break;
                default:
                    break;
            }
        }


        public void OnSetSpeedFrameEvent(FrameEventInfo.SetSpeedFrameEvent setSpeedFrameEvent)
        {
            if (setSpeedFrameEvent.x)
            {
                speed.x = GetDirectionInt() * setSpeedFrameEvent.speed.x;
            }

            if (setSpeedFrameEvent.y)
            {
                speed.y = setSpeedFrameEvent.speed.y;
            }

            if (setSpeedFrameEvent.z)
            {
                speed.z = setSpeedFrameEvent.speed.z;
            }
        }

        public void OnPlayAnimEffect(FrameEventInfo.PlayAnimEffectFrameEventData playAnimFrameEventData)
        {
            Vector3 pos = new Vector3(playAnimFrameEventData.pos.x * GetDirectionInt(), playAnimFrameEventData.pos.y, playAnimFrameEventData.pos.z);
            AnimManager.Instance.PlayAnimEffect(playAnimFrameEventData.id, transform.position + pos, 0, GetDirection() == Direction.Left, Vector3.right, transform);
        }
        
        public void OnRoleAiFlyTo(FrameEventInfo.RoleAiFlyToFrameEventData roleAiFlyToFrameEventData)
        {
            var roleBehaviourTree = GetComponent<RoleBehaviorTree>();
            if (roleBehaviourTree != null)
            {
                if (roleBehaviourTree.TargetController != null)
                {
                    float ang = roleAiFlyToFrameEventData.xzangle;
                    float yyy = roleBehaviourTree.TargetController.transform.position.y - transform.position.y > roleAiFlyToFrameEventData.RangeMax.y ? roleAiFlyToFrameEventData.RangeMax.y : roleBehaviourTree.TargetController.transform.position.y;
                    Vector3 pos = new Vector3(roleBehaviourTree.TargetController.transform.position.x,yyy,roleBehaviourTree.TargetController.transform.position.z);
                    Vector3 newSpeed = Tools.GetFlyToPosSpeed(new Vector2(1, Mathf.Tan(roleAiFlyToFrameEventData.xyangle*Mathf.Deg2Rad)), pos - transform.position + new Vector3(0, 1f, 0), -roleAiFlyToFrameEventData.GrivityAcceleration);
                    //Speed = Tools.GetFlyToPosSpeed(new Vector2(1, 1), pos - transform.position + new Vector3(0, 0.5f, 0), -GrivityAcceleration);
                    float aas = Mathf.Atan2(newSpeed.z, newSpeed.x) * Mathf.Rad2Deg;
                    Vector3 force = new Vector3();
                    switch (GetDirection())
                    {
                        case Direction.Left:
                            if (aas < 180f - ang && aas > 0)
                            {
                                force = new Vector3(Mathf.Cos((180f - ang) * Mathf.Deg2Rad), 0, Mathf.Sin((180f - ang) * Mathf.Deg2Rad));
                            }
                            else if (aas > ang - 180f && aas < 0)
                            {
                                force = new Vector3(Mathf.Cos((180f + ang) * Mathf.Deg2Rad), 0, Mathf.Sin((180f + ang) * Mathf.Deg2Rad));
                            }
                            else
                            {

                            }
                            break;
                        case Direction.Right:
                            if (aas < -ang)
                            {
                                force = new Vector3(Mathf.Cos((-ang) * Mathf.Deg2Rad), 0, Mathf.Sin((-ang) * Mathf.Deg2Rad));
                            }
                            else if (aas > ang)
                            {
                                force = new Vector3(Mathf.Cos((ang) * Mathf.Deg2Rad), 0, Mathf.Sin((ang) * Mathf.Deg2Rad));
                            }
                            else
                            {

                            }
                            break;
                    }
                    if (force.x == 0 && force.z == 0)
                    {
                        Vector3 linshi = new Vector3(newSpeed.x, 0, newSpeed.z);
                        if ((pos - transform.position).magnitude > roleAiFlyToFrameEventData.RangeMax.x)
                        {
                            Speed = (linshi.normalized * roleAiFlyToFrameEventData.RangeMax.x) + new Vector3(0, newSpeed.y, 0);
                        }
                        else
                        {
                            speed = newSpeed;
                        }
                    }
                    else
                    {
                        Speed = (force.normalized * roleAiFlyToFrameEventData.RangeMax.x) + new Vector3(0, roleAiFlyToFrameEventData.RangeMax.y, 0);
                    }
                }
            }
            
            
        }
        
        float oldspeed = 0;
        bool fristold = true;
        public void OnSetAnimSpeedFrameEvent(FrameEventInfo.SetAnimSpeedFrameEvent setAnimSpeedFrameEvent)
        {
            if (setAnimSpeedFrameEvent.animspeedType == FrameEventInfo.AnimspeedType.setAnimspeed)
            {
                if (fristold)
                {
                    oldspeed = RoleAnimator.speed;
                    fristold = false;
                }
                RoleAnimator.speed = setAnimSpeedFrameEvent.speed;
            }
            else
            {
                fristold = true;
                if (oldspeed != 0f)
                {
                    RoleAnimator.speed = oldspeed;
                }

            }
        }
        
        public void OnCameraShakeFrameEvent(FrameEventInfo.CameraShakeFrameEvent cameraShakeFrameEvent)
        {
            AnimManager.Instance.WeaponCremaShake(cameraShakeFrameEvent.cameraShakeType);
        }

        public void OnMoveToTargetFrameEvent(FrameEventInfo.MoveToTargetFrameEvent moveToTargetFrameEvent)
        {
            
            Vector3 selfpos = transform.position;
            Vector3 targetpos = Vector3.zero;
            GetTargetPos(selfpos,ref targetpos);
            
            switch (moveToTargetFrameEvent.movetoTagretType)
            {
                case FrameEventInfo.MovetoTagretType.Tagret:
                {
                    TurnBackByMoveTo(selfpos,targetpos);
                    Bounds bounds = new Bounds(selfpos, new Vector3(500, 10, 500));

                    Vector3 movePos = Vector3.zero;
                    movePos = bounds.ClosestPoint(targetpos);
                    movePos = movePos - selfpos;

                    if (movePos.magnitude > 0.01f)
                    {
                        Debug.Log("MoveToTarget:" + movePos);
                        movePos.y = 0; // 去除y方向移动 add by TangJian 2019/3/23 11:50
                        switch (moveToTargetFrameEvent.movetoTagretAnimType)
                        {
                            case FrameEventInfo.MovetoTagretAnimType.time:
                                StopMoveToTarget();
                                tweenerDoMoveBy = this.DoMoveBy(movePos, moveToTargetFrameEvent.time, moveToTargetFrameEvent.ease);
                                break;
                            case FrameEventInfo.MovetoTagretAnimType.speed:
                                float time = movePos.magnitude / moveToTargetFrameEvent.time;
                                StopMoveToTarget();
                                tweenerDoMoveBy = this.DoMoveBy(movePos, time, moveToTargetFrameEvent.ease);
                                break;
                        }
                    }
                    
                }
                    break;
                case FrameEventInfo.MovetoTagretType.Postion:
//                    Vector3 vector3pos = new Vector3(selfpos.x+ moveToTargetFrameEvent.distance, 0, targetpos.z);
//                    vector3pos.y = 0; // 去除y方向移动 add by TangJian 2019/3/23 11:50
//                    StopMoveToTarget();
//                    tweenerDoMoveBy = this.DoMoveBy(vector3pos, moveToTargetFrameEvent.time);
                    break;
                case FrameEventInfo.MovetoTagretType.Forward:
                {
                    TurnBackByMoveTo(selfpos,targetpos);
                    Vector3 forward = (targetpos - selfpos).normalized;
                    Vector3 movePos = forward * moveToTargetFrameEvent.distance;

                    if (movePos.magnitude > 0.01f)
                    {
                        Debug.Log("MoveToTarget:" + movePos);
                        movePos.y = 0; // 去除y方向移动 add by TangJian 2019/3/23 11:50
                        switch (moveToTargetFrameEvent.movetoTagretAnimType)
                        {
                            case FrameEventInfo.MovetoTagretAnimType.time:
                                StopMoveToTarget();
                                tweenerDoMoveBy = this.DoMoveBy(movePos, moveToTargetFrameEvent.time, moveToTargetFrameEvent.ease);
                                break;
                            case FrameEventInfo.MovetoTagretAnimType.speed:
                                float time = movePos.magnitude / moveToTargetFrameEvent.time;
                                StopMoveToTarget();
                                tweenerDoMoveBy = this.DoMoveBy(movePos, time, moveToTargetFrameEvent.ease);
                                break;
                        }
            
                    }
                }
                    break;
                default:
                    throw new Exception("unknown FrameEventInfo.MovetoTagretType");
            }
        }

        private void GetTargetPos(Vector3 selfpos,ref Vector3 targetpos)
        {
                RoleBehaviorTree roleAIController = GetComponent<RoleBehaviorTree>();
            if (roleAIController == null || roleAIController.TargetController == null)
            {
                var target = SceneManager.Instance.CurrScene.GetLeastDistanceRole(selfpos,"1");
                if (!target)
                {
                    throw new  Exception("Don't Find Target");
                }
                
                targetpos = target.transform.position;
            }
            else
            {
                targetpos = roleAIController.TargetController.transform.position;    
            }
        }
        
        private void TurnBackByMoveTo(Vector3 selfPos,Vector3 targetPos)
        {
            
            if (selfPos.x > targetPos.x && GetDirectionInt() > 0)
            {
                if (WithTrunBackAnim)
                {
                    _animator.SetBool("is_turnback", true);
                }
                else
                {
                    SetDirection(Direction.Left);    
                }
            }

            if (selfPos.x < targetPos.x && GetDirectionInt() < 0)
            {
                if (WithTrunBackAnim)
                {
                    _animator.SetBool("is_turnback", true);
                }
                else
                {
                    SetDirection(Direction.Right);    
                }
            }
        }
        public void StopMoveToTarget()
        {
            tweenerDoMoveBy?.Kill();
            tweenerDoMoveBy = null;
        }

        float calcdir(float targetpos, float selfpos, float Postion, float postion2)
        {
            float pos;
            if (Mathf.Abs(targetpos - selfpos) > Postion && Mathf.Abs(targetpos - selfpos) < postion2)
            {
                float absx = Mathf.Abs(Postion);
                float fl = (targetpos - selfpos) > 0 ? -absx : absx;
                pos = targetpos - selfpos + fl;
            }
            else if (Mathf.Abs(targetpos - selfpos) >= postion2)
            {
                float fl = (targetpos - selfpos) > 0 ? 1 : -1;
                pos = fl * Mathf.Abs(Postion - postion2);
            }
            else
            {
                pos = 0f;
            }
            return pos;
        }

        public virtual void OnAtk(FrameEventInfo.RoleAtkFrameEventData roleAtkFrameEventData)
        {
            bool useForceOffset = roleAtkFrameEventData.useForceOffset;
            Vector3 targetForcedOffset = roleAtkFrameEventData.targetForcedOffset;
            float targetForcedOffsetDuration = roleAtkFrameEventData.targetForcedOffsetDuration;
                
            // 设置暂停时间 add by TangJian 2019/4/28 14:31
            SelfSuspendScale = roleAtkFrameEventData.suspendScale;
            SelfSuspendTime = roleAtkFrameEventData.suspendTime / SelfSuspendScale;
            
            string damageControllerId = GetInstanceID() + ":" + roleAtkFrameEventData.id;
            string damageId = roleAtkFrameEventData.useCustomFrameEventBatch
                ? damageControllerId + skeletonAnimator.CustomFrameEventBatchId
                : damageControllerId + skeletonAnimator.FrameEventBatchId;

            float finalAtk = 0;
            DamageEffectType baseDamageEffectType = DamageEffectType.Strike;
            
            switch (this.currRoleAttackType)
            {
                case RoleAttackType.MainHand:
                {
                    var mainHandWeapon = RoleData.EquipData.getMainHand<WeaponData>();
                    baseDamageEffectType = mainHandWeapon != null
                        ? mainHandWeapon.damageEffectType
                        : roleAtkFrameEventData.damageEffectType;

                    finalAtk = RoleData.GetMainHandFinalAttr(AttrType.Atk);
                }
                    break;
                case RoleAttackType.OffHand:
                {
                    var offHandWeapon = RoleData.EquipData.getOffHand<WeaponData>();
                    baseDamageEffectType = offHandWeapon != null
                        ? offHandWeapon.damageEffectType
                        : roleAtkFrameEventData.damageEffectType;
                    
                    finalAtk = RoleData.GetOffHandFinalAttr(AttrType.Atk);
                }
                    break;
                default:
                    finalAtk = _roleData.FinalAtk;
                    break;
            }
            
            AtkPropertyType baseAtkPropertyType = roleAtkFrameEventData.atkPropertyTypeOnOff ? roleAtkFrameEventData.atkPropertyType : RoleData.atkPropertyType;
            
            FrameEventMethods.OnRoleAtk(roleAtkFrameEventData, damageControllerId, damageId, _roleData.TeamId, skeletonAnimator, this
                , new Vector3(GetDirectionInt(), 0, 0)
                , baseDamageEffectType
                , _roleData.atkPropertyType
                , finalAtk
                , _roleData.AtkMin
                , _roleData.AtkMax
                , _roleData.MagicMin
                , _roleData.MagicMax
                , _roleData.FinalPoiseCut
                , _roleData.RoleMass
                , _roleData.CriticalRate
                , _roleData.CriticalDamage
                , (DamageController damageController) =>
                {
                    #region //确定武器，更改死亡动画 2019.3.27

                    {
                        try
                        {
                            switch (RoleData.EquipData.getMainHand<WeaponData>().equipType)
                            {
                                case EquipType.Lswd:
                                    damageController.damageData.WeapondeadType = WeaponType.Lswd;
                                    break;
                                case EquipType.Swd:
                                    damageController.damageData.WeapondeadType = WeaponType.Swd;
                                    break;
                                default:
                                    damageController.damageData.WeapondeadType = WeaponType.None;
                                    break;
                                    ;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        
                    }                        
                    #endregion

                    damageController.SetUseForcedOffset(useForceOffset);
                    damageController.SetForcedOffset(targetForcedOffset);
                    damageController.SetForcedOffsetDuration(targetForcedOffsetDuration);

                    damageController.SetEffectOrientation(roleAtkFrameEventData.effectOrientation);

                    damageController.SetSelfSuspend(SelfSuspendTime, SelfSuspendScale);

                    damageController.damageData.HitEffectType = GetHitEffectType();
                    
                    switch (roleAtkFrameEventData.type)
                    {
                        case FrameEventInfo.RoleAtkFrameEventData.Type.Default:
                            damageController.SetId(damageControllerId);
                            damageController.SetDestroyTime(0.1f);

                            damageController.SetForceValue(roleAtkFrameEventData.forceValue);

                            break;
                        case FrameEventInfo.RoleAtkFrameEventData.Type.Add:
                            AddCurrAnimDamageGameObject(damageController);
                            damageController.SetId(damageControllerId);
                            
                            damageController.SetForceValue(roleAtkFrameEventData.forceValue);

                            break;
                    }
                    
                    
                });
            
            //体力扣减
//            RoleData.Tili-=roleAtkFrameEventData.tiliCost;

            
        }
        
        public virtual void OnSkill(FrameEventInfo.RoleSkillFrameEventData roleSkillFrameEventData)
        {
            string teamId = string.IsNullOrEmpty(roleSkillFrameEventData.teamId) ? RoleData.TeamId : roleSkillFrameEventData.teamId;
            ISkillController skillController = SkillManager.Instance.UseSkill(roleSkillFrameEventData.skillId, gameObject.transform, teamId, GetDirection(), roleSkillFrameEventData.pos);
            
//            if (skillController !=null )
            {
                Vector3 position = transform.position;
                var roleBehaviourTree = GetComponent<RoleBehaviorTree>();
                
                if (roleBehaviourTree != null && roleBehaviourTree.TargetController != null)
                {
                    position = roleBehaviourTree.TargetController.transform.position;
                }

                if (roleSkillFrameEventData.usePointPosition)
                {
                    position = PointPosition;
                }

                var localPosition = transform.InverseTransformPoint(position);

                localPosition.x *= GetDirectionInt();
                
                // 限制角度 add by TangJian 2019/4/22 13:56
                Vector3 angle = localPosition.GetAngle();
                if (angle.y > roleSkillFrameEventData.angleYMax.y)
                {
                    localPosition = localPosition.AngleYLimit(roleSkillFrameEventData.angleYMax.y);
                }
                else if (angle.y < -roleSkillFrameEventData.angleYMax.y)
                {
                    localPosition = localPosition.AngleYLimit(-roleSkillFrameEventData.angleYMax.y);
                }

                angle = localPosition.GetAngle();

                localPosition.x *= GetDirectionInt();
                
                position = transform.TransformPoint(localPosition);

                switch (roleSkillFrameEventData.skillActionType)
                {
                    case FrameEventInfo.RoleSkillFrameEventData.SkillActionType.FlyToTarget:
                        skillController.FlyTo(position);
                        break;
                    case FrameEventInfo.RoleSkillFrameEventData.SkillActionType.TowardToTarget:
                        skillController.TowardTo(position);
                        break;
                    default:
                        break;
                }
            } 
        }

        public async virtual void OnPlayAnim(FrameEventInfo.PlayAnimFrameEventData PlayAnimFrameEventData)
        {
            if (!string.IsNullOrEmpty(PlayAnimFrameEventData.animId))
            {
                // Instantiate(SpecialEffect)
                GameObject InstantiateSpecialEffect = new GameObject();
                InstantiateSpecialEffect.SetActive(false);

                if (PlayAnimFrameEventData.parentType == FrameEventInfo.ParentType.Parent)
                {
                    InstantiateSpecialEffect.transform.parent = gameObject.transform;
                }
                else
                {
                    InstantiateSpecialEffect.transform.parent = gameObject.transform.parent;
                }
                InstantiateSpecialEffect.transform.position = transform.position + new Vector3(PlayAnimFrameEventData.pos.x * GetDirectionInt(), PlayAnimFrameEventData.pos.y, PlayAnimFrameEventData.pos.z);
                InstantiateSpecialEffect.AddComponentUnique<SortRenderer>();
                SkeletonAnimation skeletonAnimation = InstantiateSpecialEffect.AddComponentUnique<SkeletonAnimation>();
                
                var SkeletonDataAssetpath = PlayAnimFrameEventData.animId.Replace('\\', '/');
                skeletonAnimation.skeletonDataAsset =
                    await AssetManager.LoadAssetAsync<SkeletonDataAsset>(SkeletonDataAssetpath);
                
                //InstantiateSpecialEffect.transform.eulerAngles = new Vector3(0, GetDirectionInt() > 0 ? 0 : 180, MathUtils.SpeedToDirection(PlayAnimFrameEventData.orientation));
                //InstantiateSpecialEffect.transform.localScale = PlayAnimFrameEventData.scale;
                InstantiateSpecialEffect.SetActive(true);
                Spine.TrackEntry TrackEntry = skeletonAnimation.state.SetAnimation(0, PlayAnimFrameEventData.AnimatedFragment, false);
                var skeletonAnimatorRenderer = skeletonAnimation.GetComponent<Renderer>();
                FrameEventPlayAnimSpeedController frameEventPlayAnimSpeedController = InstantiateSpecialEffect.AddComponentUnique<FrameEventPlayAnimSpeedController>();
                frameEventPlayAnimSpeedController.animator = RoleAnimator;
                frameEventPlayAnimSpeedController.speedType = PlayAnimFrameEventData.animspeedtype;
                if (PlayAnimFrameEventData.animspeedtype == SpeedType.Fixed)
                {
                    frameEventPlayAnimSpeedController.speed = PlayAnimFrameEventData.animspeed;
                }
                skeletonAnimatorRenderer.enabled = false;
                DelayFunc(() =>
                {
                    skeletonAnimatorRenderer.enabled = true;
                }, 0.00001f);
                //skeletonAnimation.state.End += (entry) =>
                //{
                //    //skeletonAnimation.timeScale = 0f;
                //    //TrackEntry.TimeScale = 0f;
                //    Tools.Destroy(InstantiateSpecialEffect);
                //};
                
                // 动画缩放
                if (PlayAnimFrameEventData.sceneDecorationPosition == SceneDecorationPosition.ground)
                {
                    skeletonAnimation.transform.eulerAngles = new Vector3(90f, GetDirectionInt() > 0 ? 0 : 180, MathUtils.SpeedToDirection(PlayAnimFrameEventData.orientation));
                    skeletonAnimation.LocalScale = new Vector3(PlayAnimFrameEventData.scale.x, PlayAnimFrameEventData.scale.y*(1f/Mathf.Tan(30f*Mathf.Deg2Rad)), PlayAnimFrameEventData.scale.z);
                }
                else
                {
                    skeletonAnimation.transform.eulerAngles = new Vector3(0, GetDirectionInt() > 0 ? 0 : 180, MathUtils.SpeedToDirection(PlayAnimFrameEventData.orientation));
                    skeletonAnimation.LocalScale = PlayAnimFrameEventData.scale;
                }
            }}

        public virtual void OnVariableRoleMoveSpeed(FrameEventInfo.VariableRoleMoveSpeedData variableRoleMoveSpeedData)
        {
            if (variableRoleMoveSpeedData.VariableType == FrameEventInfo.VariableRoleMoveSpeedData.Variableeuem.Add)
            {
                AddCurrVariableRoleMoveSpeed(variableRoleMoveSpeedData.speed, variableRoleMoveSpeedData.Duration);
            }
            else if (variableRoleMoveSpeedData.VariableType == FrameEventInfo.VariableRoleMoveSpeedData.Variableeuem.Remove)
            {
                RemoveCurrVariableRoleMoveSpeed(variableRoleMoveSpeedData.Duration);
            }
            else if (variableRoleMoveSpeedData.VariableType == FrameEventInfo.VariableRoleMoveSpeedData.Variableeuem.clear)
            {
                ClearCurrVariableRoleMoveSpeed();
            }
            else
            {

            }

        }

        public virtual async void OnPlayAnimList(FrameEventInfo.FrameEventAnimList frameEventAnimList)
        {
            if (frameEventAnimList != null && frameEventAnimList.AnimList != null && frameEventAnimList.AnimList.Count != 0)
            {
                int ro = Tools.RandomWithWeight(frameEventAnimList.AnimList, (AnimListFrameEventData curr, int index) =>
                {
                    int weight = curr.weight;
                    return weight;
                });

                AnimListFrameEventData animListFrameEventData = frameEventAnimList.AnimList[ro];
                if (animListFrameEventData.Animpath != null && animListFrameEventData.Animpath != "")
                {
                    // GameObject SpecialEffect = new GameObject();
                    GameObject InstantiateSpecialEffect = new GameObject();
                    InstantiateSpecialEffect.SetActive(false);
                    InstantiateSpecialEffect.AddComponentUnique<SortRenderer>();
                    SkeletonAnimation skeletonAnimation = InstantiateSpecialEffect.AddComponentUnique<SkeletonAnimation>();
                    var SkeletonDataAssetpath = animListFrameEventData.Animpath.Replace('\\', '/');
                    skeletonAnimation.skeletonDataAsset =
                        await AssetManager.LoadAssetAsync<SkeletonDataAsset>(SkeletonDataAssetpath);

                    if (animListFrameEventData.parentType == FrameEventInfo.ParentType.Parent)
                    {
                        InstantiateSpecialEffect.transform.parent = gameObject.transform;
                    }
                    else
                    {
                        InstantiateSpecialEffect.transform.parent = gameObject.transform.parent;
                    }
                    if (animListFrameEventData.datapos)
                    {
                        InstantiateSpecialEffect.transform.position = transform.position + new Vector3(animListFrameEventData.pos.x * GetDirectionInt(), animListFrameEventData.pos.y, animListFrameEventData.pos.z);
                    }
                    else
                    {
                        InstantiateSpecialEffect.transform.position = transform.position + new Vector3(frameEventAnimList.pos.x * GetDirectionInt(), frameEventAnimList.pos.y, frameEventAnimList.pos.z);
                    }
                    if (animListFrameEventData.datascale)
                    {
                        InstantiateSpecialEffect.transform.localScale = animListFrameEventData.scale;
                    }
                    else
                    {
                        InstantiateSpecialEffect.transform.localScale = frameEventAnimList.scale;
                    }
                    if (animListFrameEventData.type == AnimListFrameEventType.Random)
                    {
                        float rotation = UnityEngine.Random.Range(animListFrameEventData.AnimRotation, animListFrameEventData.float2);
                        InstantiateSpecialEffect.transform.eulerAngles = new Vector3(0, GetDirectionInt() > 0 ? 0 : 180, AngleCorrection(GetDirectionInt(), rotation));
                    }
                    else
                    {
                        InstantiateSpecialEffect.transform.eulerAngles = new Vector3(0, GetDirectionInt() > 0 ? 0 : 180, AngleCorrection(GetDirectionInt(), animListFrameEventData.AnimRotation));
                    }

                    InstantiateSpecialEffect.SetActive(true);
                    Spine.TrackEntry TrackEntry = skeletonAnimation.state.SetAnimation(0, animListFrameEventData.AnimatedFragment, false);
                    skeletonAnimation.state.End += (entry) =>
                    {
                        Tools.Destroy(InstantiateSpecialEffect);
                        // Tools.Destroy(SpecialEffect);
                    };
                }
            }
        }

        public void OnSuperArmor(FrameEventInfo.SuperArmorFrameEvent superArmorFrameEvent)
        {
            string key = "SuperArmor";
            switch (superArmorFrameEvent.operationType)
            {
                case FrameEventInfo.SuperArmorFrameEvent.OperationType.Add:
                    BuffData buffData = new BuffData();

                    
                    // 设置角色霸体 add by TangJian 2019/3/23 15:48
                    buffData.attrData.superArmor = 1;

                    // 去除韧性增加 add by TangJian 2019/3/23 15:37
//                    buffData.attrData.poiseScale = superArmorFrameEvent.poiseScale;

                    buffData.id = key;
                    BuffController.AddPermanentBuff(key, buffData);
                    break;
                case FrameEventInfo.SuperArmorFrameEvent.OperationType.Remove:
                    BuffController.RemoveBuff(key);
                    break;
            }
        }
        public void OnSkillList(FrameEventInfo.RoleSkillListFrameEvent roleSkillListFrameEvent)
        {
            Direction direction=Direction.Right;
            
            RoleBehaviorTree roleBehaviourTree = GetComponent<RoleBehaviorTree>();
            if (roleBehaviourTree != null)
            {
                SkillManager.Instance.useSkillList(roleSkillListFrameEvent.id, direction,roleBehaviourTree.TargetController.transform,RoleData.TeamId);
            }
            else
            {
                SkillManager.Instance.useSkillList(roleSkillListFrameEvent.id, direction, transform, RoleData.TeamId);
            }

        }

        public void OnRoleQte(FrameEventInfo.RoleQTE roleQte)
        {
            switch (roleQte.qteType)
            {
                case FrameEventInfo.RoleQTE.QteType.Begin:
                    RoleQteBegin(roleQte);
                    break;
                case FrameEventInfo.RoleQTE.QteType.Hit:
                    if(MyQte!=null)
                        MyQte.Hit();
                    break;
                case FrameEventInfo.RoleQTE.QteType.LastHit:
                    if(MyQte!=null)
                        MyQte.LastHit();
                    break;
                case FrameEventInfo.RoleQTE.QteType.End:
                    if(MyQte!=null)
                        MyQte.Success();
                    break;
                case FrameEventInfo.RoleQTE.QteType.Break:
                    if (HisQte != null)
                    {
                        HisQte.Failure();
                    }
                    break;
            }
        }

        void RoleQteBegin(FrameEventInfo.RoleQTE roleQte)
        {
            GameObject triggerGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            triggerGameObject.transform.position = transform.position + roleQte.pos * GetDirectionInt();
            triggerGameObject.transform.localScale = roleQte.size;

            triggerGameObject.GetComponent<Renderer>().enabled = false;
            
            triggerGameObject.layer = LayerMask.NameToLayer("Interaction");
            triggerGameObject.tag = "Interaction";
            
            TriggerController triggerController = triggerGameObject.AddComponent<TriggerController>();

            triggerController.OnTriggerInEvent += (TriggerEvent triggerEvent) =>
            {
                Debug.Log("OnQteTriggerIn:" + triggerEvent.selfTriggerController.name + " - " + triggerEvent.otherTriggerController.name);
            
                RoleController otherRoleController = triggerEvent.otherTriggerController.ITriggerDelegate as RoleController;

                if (otherRoleController != null && otherRoleController != this && otherRoleController.RoleData.TeamId != RoleData.TeamId)
                {
                    MyQte = RoleQTEDataAsset.Instance.Qte(this, otherRoleController, roleQte.id);
                    otherRoleController.HisQte = MyQte;
                    
                    MyQte.Begin();
                }
            };
            
            Tools.Destroy(triggerGameObject, 0.1f);
        }

        void RoleQteHit(FrameEventInfo.RoleQTE roleQte)
        {
            
        }

        public void OnPlayAudio(FrameEventInfo.PlayAudio playAudio)
        {
            AudioManager.Instance.PlayEffect(playAudio.AudioName, transform.position + playAudio.Pos.Flip(GetDirectionInt() < 0));   
        }

        public void OnJObject(JObject jObject)
        {
            Debug.Log((jObject.ToString()));
            string eventType = jObject["name"].ToObject<string>();
            if (eventType == "attack")
            {
                string attackId = jObject["attackId"].ToString();
                bool useCustomFrameEventBatch = false;
                
                Vector3 damagePos = jObject["pos"].ToObject<Vector3>();
                Vector3 damageSize = jObject["size"].ToObject<Vector3>();
                HitType hitType = jObject["hitType"].ToObject<HitType>();
                
                Vector3 targetForceOffset = jObject["targetForceOffset"].ToObject<Vector3>();
                float targetForceOffsetDuration = jObject["targetForceOffsetDuration"].ToObject<float>();
                Vector3 targetMoveBy = jObject["targetMoveBy"].ToObject<Vector3>();
                
                // 强制位移 add by TangJian 2019/4/30 10:26
                
                
                // 设置暂停时间 add by TangJian 2019/4/28 14:31
                SelfSuspendScale = jObject["selfSuspendScale"].ToObject<float>();
                SelfSuspendTime = jObject["selfSuspendTime"].ToObject<float>() / SelfSuspendScale;
                
                OtherSuspendScale = jObject["otherSuspendScale"].ToObject<float>();
                OtherSuspendTime = jObject["otherSuspendTime"].ToObject<float>() / OtherSuspendScale;
                
                string damageControllerId = GetInstanceID() + ":" + attackId;
                string damageId = useCustomFrameEventBatch
                    ? damageControllerId + skeletonAnimator.CustomFrameEventBatchId
                    : damageControllerId + skeletonAnimator.FrameEventBatchId;

                float finalAtk = 0;
                DamageEffectType baseDamageEffectType = DamageEffectType.Strike;
            
                switch (this.currRoleAttackType)
                {
                    case RoleAttackType.MainHand:
                    {
                        var mainHandWeapon = RoleData.EquipData.getMainHand<WeaponData>();
                        baseDamageEffectType = mainHandWeapon != null
                            ? mainHandWeapon.damageEffectType
                            : DamageEffectType.Slash;

                        finalAtk = RoleData.GetMainHandFinalAttr(AttrType.Atk);
                    }
                        break;
                    case RoleAttackType.OffHand:
                    {
                        var offHandWeapon = RoleData.EquipData.getOffHand<WeaponData>();
                        baseDamageEffectType = offHandWeapon != null
                            ? offHandWeapon.damageEffectType
                            : DamageEffectType.Slash;
                    
                        finalAtk = RoleData.GetOffHandFinalAttr(AttrType.Atk);
                    }
                        break;
                    default:
                        finalAtk = _roleData.FinalAtk;
                        break;
                }
            
                AtkPropertyType baseAtkPropertyType = RoleData.atkPropertyType;
                    
                var damageController =FrameEventMethods.RoleAtk(this, damageControllerId, damageId, _roleData.TeamId, FrameEventInfo.ParentType.Transform
                    , PrimitiveType.Cube
                    , damagePos
                    , damageSize
                    , new Vector3(GetDirectionInt(), 0, 0)
                    , hitType
                    , targetMoveBy
                    , DamageDirectionType.Directional
                    , baseDamageEffectType
                    , baseAtkPropertyType
                    , 1
                    , 1);
                
                damageController.SetId(damageControllerId);
                damageController.SetDestroyTime(0.1f);
                
                // 设置强制位移 add by TangJian 2019/4/30 10:28
                damageController.SetForcedOffset(targetForceOffset);
                damageController.SetForcedOffsetDuration(targetForceOffsetDuration);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("other.gameObject.tag = " + other.gameObject.tag);
            Debug.Log("LayerMask.LayerToName(other.gameObject.layer) = " 
            + LayerMask.LayerToName(other.gameObject.layer));
        }

        private void OnCollisionExit(Collision other)
        {
            
        }
        
        

        public void OnTread(FrameEventInfo.OnTread onTread)
        {
            MatType matType = MatType.iron_ground;
//            string animName = onTread.animName;
            // 判断在地面上
            if (IsGrounded())
            {
                TriggerController selfTriggerController = gameObject.GetChild("InteractiveCube").GetComponent<TriggerController>();
                HurtAble hurtAble = selfTriggerController.GetFirstComponent<HurtAble>();
                
                if (hurtAble != null)
                {
                    _walkOnGroundEffectDelegate.PlayEffect(onTread.walkType, hurtAble.GetMatType(), gameObject.transform, 
                        transform.position ,onTread.animVector3, GetDirectionInt() < 0);
                }
                else
                {
                    Debug.Log("找不到地面");
                }
            }
        }

        public void OnAnimProperty(FrameEventInfo.AnimProperty animProperty)
        {
            
        }
    }
}