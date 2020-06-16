using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using System;
using Unity.Collections;


namespace Tang
{
    public partial class RoleController
    {
        static Ray ray = new Ray();

        protected bool isGrounded = false;

        public bool IsGrounded()
        {
            return isGrounded;
        }

        // 移动 add by TangJian 2018/12/8 1:37
        public void Move(Vector3 pos)
        {
            _characterController.Move(pos);
            // Debug.Log("Move"+pos);
        }

        // 移动到 add by TangJian 2018/12/8 1:37
        public void MoveTo(Vector3 pos)
        {
            Move(pos - transform.position);
        }

        public float GetSpeedByMoveState(MoveState moveState)
        {
            float speed = WalkSpeed;
            switch (moveState)
            {
                case MoveState.Idle:
                case MoveState.Walk:
                    speed = WalkSpeed;
                    break;
                case MoveState.Run:
                    speed = RunSpeed;
                    break;
                case MoveState.Rush:
                    speed = RushSpeed;
                    break;
                default:
                    speed = WalkSpeed;
                    break;
            }

            return speed;
        }
        
        // 计算重力 add by TangJian 2019/6/3 16:37
        public Vector3 CalcGravity(Vector3 lSpeed, float deltaTime, bool bounce = false)
        {
            UpdateIsGround();
            
            // 重力 add by TangJian 2017/07/10 21:28:06
            {
                Vector3 gravity = new Vector3(0, GrivityAcceleration, 0);
                if (isGrounded)
                {
                    if (lSpeed.y > 0)
                    {
                    }
                    else if (lSpeed.y < 0)
                    {
                        if (bounce == true)
                        {
                            if (Math.Abs(lSpeed.y) < new Vector3(0, 0.2f, 0).MoveByToSpeed().y)
                            {
                                lSpeed.y = 0;
                            }
                            else
                            {
                                lSpeed.y = -lSpeed.y / 3f;
                                lSpeed.x /= 1.5f;
                                lSpeed.z /= 1.5f;
                                
//                                t = v0 + a * t
//                                v0 * t + a * t * t / 2 = s
                                if (lSpeed.y > 1F)
                                {
                                    lSpeed.y = 1f;
                                }
                                else
                                {
                                    
                                }
                            }
                        }
                        else
                        {
                            lSpeed.y = 0;
                        }
                    }
                    else
                    {
                    }
                }
                else
                {
                    lSpeed = lSpeed - gravity * deltaTime;
                }
            }
            
            // 浮空 add by TangJian 2018/05/31 16:23:36
            if (IsHovering)
            {
                if (lSpeed.y <= 0)
                {
                    lSpeed.y = 0;
                }
            }
            
            return lSpeed;
        }

        // 计算速度 add by TangJian 2017/07/13 23:25:08
        public static Vector3 vector3Zero = new Vector3(0, 0, 0);
        public void CalcSpeed(float deltaTime)
        {
            CalcSpeed(deltaTime, joystick);
        }

        public void CalcSpeed(float deltaTime, Vector2 joystick)
        {
            switch (MoveState)
            {
                case MoveState.Idle:
                case MoveState.Walk:
                    CalcSpeed(deltaTime, joystick, WalkSpeed);
                    break;
                case MoveState.Run:
                    CalcSpeed(deltaTime, joystick, RunSpeed);
                    break;
                case MoveState.Rush:
                    CalcSpeed(deltaTime, joystick, RushSpeed);
                    break;
            }
        }

        public void CalcSpeed(float deltaTime, Vector2 joystick, float controlSpeed)
        {
            Vector3 speedxz = new Vector3(Speed.x, 0, Speed.z);
            
            
            Vector3 controlForce = new Vector3(joystick.x, 0, joystick.y);
            
            
            // 操控力 add by TangJian 2018/9/3 19:53
            if (controlForce.magnitude >= 0.1f)
            {
                speedxz = controlForce.normalized * Mathf.Lerp(speedxz.magnitude, controlSpeed, deltaTime / 0.17f);
                Debug.Log("移动速度controlSpeed：" + controlSpeed);
                
//                if (speedxz.magnitude < controlSpeed)
//                {
////                    speedxz += controlForce.normalized * controlSpeed * deltaTime / 0.2f;
////
////                    speedxz = speedxz.magnitude > controlSpeed ? speedxz.normalized * controlSpeed : speedxz;
//                    
//                    speedxz = speedxz.normalized * Mathf.Lerp(speedxz.magnitude, controlSpeed, deltaTime);
//                }
//                else
//                {
////                    speedxz = controlForce.normalized * speedxz.magnitude; 
//                    speedxz = speedxz.normalized * Mathf.Lerp(speedxz.magnitude, controlSpeed, deltaTime);
//                }
            }
            else
            {
                // 摩擦力 add by TangJian 2018/9/3 19:53
                Vector3 frictionAcceleration = IsGrounded() ? -speedxz.normalized * GroundFrictionAcceleration * deltaTime : Vector3.zero;

                // 摩擦力影响速度 add by TangJian 2018/10/16 19:33
                speedxz = frictionAcceleration.magnitude > speedxz.magnitude ? Vector3.zero : speedxz + frictionAcceleration;
                //Debug.LogError("speedxz" + speedxz);

            }

            Speed = new Vector3(speedxz.x, Speed.y, speedxz.z);
            //Debug.LogError("计算速度" + Speed);
            // 沿着地面移动 add by TangJian 2018/10/16 19:30
            if (IsGrounded())
            {
                if (Speed.y <= 0 &&
                    (Mathf.Abs(Speed.x) > 0 || Mathf.Abs(Speed.z) > 0))
                {
                    float gap = 0.05f;
                    ray.origin = transform.position + new Vector3(Speed.normalized.x * gap, 1, Speed.normalized.z * gap);
                    ray.direction = new Vector3(0, -1, 0);

                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, 2, Tools.GetLayerMask(new List<string>() { "Ground" })))
                    {
                        Vector3 moveToPos = transform.InverseTransformPoint(hitInfo.point);
                        //Debug.LogError("计算速度22" + Speed + "moveToPos" + moveToPos + "SpeedXZmagnitude" + new Vector2(Speed.x, Speed.z).magnitude + "moveToPos.normalized" + moveToPos.normalized+"分子" + new Vector2(Speed.x, Speed.z).magnitude * moveToPos.normalized);
                        if((new Vector2(Speed.x, Speed.z).magnitude * moveToPos.normalized).Equals(Vector3.zero)|| new Vector2(moveToPos.normalized.x, moveToPos.normalized.z).magnitude == 0)
                        {
                            Speed = Vector3.zero;
                        }
                        else
                        {
                            if (controlForce.magnitude > 0.1f)
                                Speed = new Vector2(Speed.x, Speed.z).magnitude * moveToPos.normalized / new Vector2(moveToPos.normalized.x, moveToPos.normalized.z).magnitude;
                            else
                                Speed = new Vector2(Speed.x, Speed.z).magnitude * moveToPos.normalized / new Vector2(moveToPos.normalized.x, moveToPos.normalized.z).magnitude;
                        }
                        //Debug.LogError("ce"+ new Vector3(1f, 1f, 1f) / 0f +"计算速度33" + Speed+ "分母"+ new Vector2(moveToPos.normalized.x, moveToPos.normalized.z).magnitude);
                    }
                }
            }
            else
            {
                ClimbWallEdge();
            }
            //Debug.LogError("计算最终速度" + Speed);
            //SlopeRaycast();
        }
        
        // 角色转脸 add by TangJian 2018/12/8 1:37
        public virtual void TurnBack()
        {
            if (joystick.x < 0)
            {
                SetDirection(Direction.Left);
            }
            else if (joystick.x > 0)
            {
                SetDirection(Direction.Right);
            }
        }
        
        public virtual void TurnBackWithAnim()
        {
            // 如果按住alt， 则不能转身
            if (IsAltPressed)
            {
                isRushing = false;
                return;
            }

            if (WithTrunBackAnim)
            {
                if((joystick.x * GetDirectionInt()) < 0)
                {
                    _animator.SetBool("is_turnback", true);
                }
                else
                {
                    _animator.SetBool("is_turnback", false);
                }
            }
            else
            {
                TurnBack();
            }
        }
        
        protected void UpdateIsGround()
        {
            #region 是否着地处理
            
            Vector3 oldPos = _characterController.transform.position;
            
            if (isGrounded && speed.y <= 0)
            {
                _characterController.Move(new Vector3(0, -1.0f, 0));
            }
            isGrounded = _characterController.isGrounded;
            _characterController.transform.position = oldPos;

            #endregion
        }

        protected float lastUpdateMovementTime = -1;

        private string currStateName;
        
        public virtual void UpdateMovement(string stateName, AnimatorStateEventType eventType, Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex, float time)
        {
            currStateName = stateName;
            
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
//                var deltaTime = Time.deltaTime / 2f;

//                deltaTime *= _animator.speed * currStateInfo.speed;
                
                // 通过移动动画额定速度， 调整动画播放速度
//                float moveAnimSpeedScale = skeletonAnimator.GetMoveAnimSpeedScale(stateName, speed.magnitude);
//                _animator.speed = moveAnimSpeedScale;
                
                
                if (
                    currStateInfo.IsTag("hurt")
                    || currStateInfo.IsTag("hurtHold"))
                {
                    joystick = Vector2.zero;

                    CalcSpeed(deltaTime);
                    speed = CalcGravity(speed, deltaTime, true);
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
                    
                    CalcSpeed(deltaTime);
                    Move(Speed * deltaTime);
                }
                else if (    
                    currStateInfo.IsTag("move")
//                    || currStateInfo.IsTag("walk")
//                    || currStateInfo.IsTag("rush")
//                    || currStateInfo.IsTag("run")
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
                    currStateInfo.IsTag("run")
                    || currStateInfo.IsTag("walk")
                    || currStateInfo.IsTag("move"))
                {
                    TurnBack();
                    
                    speed = CalcGravity(speed, deltaTime);
                    CalcSpeed(deltaTime);
//                    Move(Speed * deltaTime);
                }
                else if (
                    currStateInfo.IsTag("rush_end")
                )
                {
                    speed = CalcGravity(speed, deltaTime);

                    
                    joystick = Vector2.zero;
                    //GroundFrictionAcceleration = 30f;
                    CalcSpeed(deltaTime);
                    //GroundFrictionAcceleration = 60f;
                    //Speed = new Vector3(Speed.x, Speed.y, Speed.z / 2f);
                    Move(Speed * deltaTime);
                }
                else if (
                    currStateInfo.IsTag("roll")
                    || currStateInfo.IsTag("dodge"))
                {
                    TurnBack();
                    
                    speed = CalcGravity(speed, deltaTime);
                    CalcSpeed(deltaTime, Vector2.zero);
                    Move(Speed * deltaTime);
                }
                else if (currStateInfo.IsTag("attack"))
                {
                    speed = CalcGravity(speed, deltaTime);

                    joystick = Vector2.zero;

                    CalcSpeed(deltaTime);
                    Move(Speed * deltaTime);
                }
                else if (currStateInfo.IsTag("jumping"))
                {
                    speed = CalcGravity(speed, deltaTime);

                    TurnBack();

                    CalcSpeed(deltaTime);


                    if (joystick.magnitude <= 0.1f)
                    {
                        speed.x = 0;
                        speed.z = 0;
                    }

                    Move(Speed * deltaTime);
                }
                else if (currStateInfo.IsTag("jumpingAttack"))
                {
                    speed = CalcGravity(speed, deltaTime);

                    joystick = Vector2.zero;

                    CalcSpeed(deltaTime);
                    Vector3 finalSpeed = Vector3.zero;
                    finalSpeed.x = Speed.x;
                    finalSpeed.y = Speed.y * currVariableSpeed;
                    finalSpeed.z = Speed.z;
                    Move(new Vector3(finalSpeed.x / 5f, finalSpeed.y, finalSpeed.z / 5f) * deltaTime);
                }
                else if (currStateInfo.IsTag("landing"))
                {
                    speed = CalcGravity(speed, deltaTime);

                    joystick = Vector2.zero;

                    CalcSpeed(deltaTime);
                    Move(Speed * deltaTime);
                }
                else if (currStateInfo.IsTag("nomove"))
                {
                    speed = CalcGravity(speed, deltaTime);

                    joystick = Vector2.zero;
                    Speed = Vector3.zero;
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

                    joystick = Vector2.zero;

                    CalcSpeed(deltaTime);
                    Move(Speed * deltaTime);
                }
                else if (currStateInfo.IsTag("climb"))
                {
                    speed = CalcGravity(speed, deltaTime);

                    //joystick = Vector2.zero;
                    speed.y = 0;
                    //calcSpeed(deltaTime);
                    //Move(Speed * deltaTime);
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
                else
                {
                    speed = CalcGravity(speed, deltaTime);
                    CalcSpeed(deltaTime, Vector2.zero);
                    Move(Speed * deltaTime);
                }
//            
//                if (currStateInfo.IsTag("walk") == false
//                    && currStateInfo.IsTag("run") == false)
//                {
//                    SetBackwards(Vector3.zero);
//                }
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

//                if (stateInfo.IsTag("roll")
//                    || stateInfo.IsTag("dodge"))
//                {
//                    speed.x = 0;
//                    speed.z = 0;
//                }
            }
        }

        // 根据动画root移动角色 add by TangJian 2018/12/8 1:37
        public virtual void MoveByAnim(SkeletonAnimator skeletonAnimator)
        {
            AnimatorStateInfo nextStateInfo = _animator.GetNextAnimatorStateInfo(0);
            bool hasNext = nextStateInfo.fullPathHash != 0;

            // 锁定位置 add by TangJian 2018/8/2 23:00
            Vector3 moveByPos = skeletonAnimator.RootBonePos;
            RoleAnimController.transform.localPosition = new Vector3(GetDirectionInt() > 0 ? moveByPos.x : -moveByPos.x, -moveByPos.y, RoleAnimController.transform.localPosition.z);

            var movePosition = moveByPos - skeletonAnimator.RootBoneLastPos;
            skeletonAnimator.RootBoneLastPos = moveByPos;
            
            // 使用方向向量影响动画移动
            var moveVector = new Vector3(joystick.x, 0, joystick.y);
            moveVector = movePosition.magnitude * moveVector.normalized;
            movePosition = moveVector;
            
            Vector3 moveXZ = new Vector3(movePosition.x * AnimMoveSpeedScale, -0.001f, movePosition.z * AnimMoveSpeedScale);
            Vector3 moveY = new Vector3(0, movePosition.y, 0);

            if (moveXZ.magnitude > 0.01)
            {
                Move(moveXZ);
                Debug.Log("moveXZ" + moveXZ);
            }
            if (moveY.magnitude > 0.01)
            {
                Move(moveY);
                Debug.Log("moveY" + moveY);
            }
        }
        
        public bool canClimbLadder = false;
        Dictionary<string, LadderController> ladderDic = new Dictionary<string, LadderController>();
        public void AddClimbLadder(string id, LadderController ladderController)
        {
            if (ladderDic.Count == 0)
            {
                canClimbLadder = true;
            }
            if (ladderDic.ContainsKey(id) == false)
            {
                ladderDic.Add(id, ladderController);
            }
        }
        public void RemoveClimbLadder(string id)
        {
            if (ladderDic.ContainsKey(id))
            {
                ladderDic.Remove(id);
            }
            if (ladderDic.Count == 0)
            {
                //canClimbLadder = false;
                if (animatorParams.ContainsKey("climb_ladder_over_type"))
                    _animator.SetInteger("climb_ladder_over_type", 1);

                if (animatorParams.ContainsKey("climb_ladder_over"))
                    _animator.SetBool("climb_ladder_over", true);
            }
        }
        public void OverClimbLadder()
        {
            int ladderType = _animator.GetInteger("ladder_type");
            laddertype LadderType = (laddertype)ladderType;
            Vector3 origin;
            Vector3 direction;
            switch (LadderType)
            {
                case laddertype.Left:
                {
                    if (joystick.y > 0)
                    {
                        origin = transform.position + new Vector3(-1f, 2f, 0f);
                    }
                    else
                    {
                        origin = transform.position + new Vector3(0f, 0.7f, 0f);
                    }
                    direction = new Vector3(0, -1, 0);
                    OverLadderRaycast(origin, direction, 1f, Tools.GetLayerMask(new List<string> { "Ground" }), (RaycastHit raycastHit) =>
                    {
                        var ground = raycastHit.collider.gameObject.GetComponentInParent<Ground>();
                        if (ground != null)
                        {
                            FirstClimbPos = raycastHit.point;
                            if (animatorParams.ContainsKey("climb_ladder_over_type"))
                                _animator.SetInteger("climb_ladder_over_type", 0);
                            if (animatorParams.ContainsKey("climb_ladder_over"))
                                _animator.SetBool("climb_ladder_over", true);
                            canClimbLadder = true;
                        }

                    });
                }
                    break;
                case laddertype.Right:
                {
                    if (joystick.y > 0)
                    {
                        origin = transform.position + new Vector3(1f, 2f, 0f);
                    }
                    else
                    {
                        origin = transform.position + new Vector3(0f, 0.7f, 0f);
                    }
                    direction = new Vector3(0, -1, 0);
                    OverLadderRaycast(origin, direction, 1f, Tools.GetLayerMask(new List<string> { "Ground" }), (RaycastHit raycastHit) =>
                    {
                        var ground = raycastHit.collider.gameObject.GetComponentInParent<Ground>();
                        if (ground != null)
                        {
                            FirstClimbPos = raycastHit.point;
                            if (animatorParams.ContainsKey("climb_ladder_over_type"))
                                _animator.SetInteger("climb_ladder_over_type", 0);
                            if (animatorParams.ContainsKey("climb_ladder_over"))
                                _animator.SetBool("climb_ladder_over", true);
                            canClimbLadder = true;
                        }

                    });
                }
                    break;
                case laddertype.Center:
                {
                    if (joystick.y > 0)
                    {
                        origin = transform.position + new Vector3(0f, 2f, 1f);
                    }
                    else
                    {
                        origin = transform.position + new Vector3(0f, 0.7f, 0f);
                    }
                    DebugManager.Instance.AddDrawGizmos(gameObject.GetInstanceID() + "overds", () =>
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(origin, 0.5f);
                        //UnityEditor.Handles.SphereCap(0, hitInfo.point + new Vector3(5, 0, 0), new Quaternion(), 0.5f);
                    });
                    direction = new Vector3(0, -1, 0);
                    OverLadderRaycast(origin, direction, 1f, Tools.GetLayerMask(new List<string> { "Ground" }), (RaycastHit raycastHit) =>
                    {
                        var ground = raycastHit.collider.gameObject.GetComponentInParent<Ground>();
                        if (ground != null)
                        {
                            FirstClimbPos = raycastHit.point;
                            if (animatorParams.ContainsKey("climb_ladder_over_type"))
                                _animator.SetInteger("climb_ladder_over_type", 0);
                            if (animatorParams.ContainsKey("climb_ladder_over"))
                                _animator.SetBool("climb_ladder_over", true);
                            canClimbLadder = true;
                        }

                    });
                }
                    break;
            }
        }

        public void OverLadderRaycast(Vector3 origin, Vector3 direction, float dis, int layer, Action<RaycastHit> action)
        {
            Ray piont = new Ray();
            piont.origin = origin;
            piont.direction = direction;
            RaycastHit raycastHit;
            if (Physics.Raycast(piont, out raycastHit, dis, layer))
            {
                action(raycastHit);
            }
        }

        public void ClimbWallEdge()
        {
            if (IsGrounded() == false)
            {
                Vector3 controlForce = new Vector3(joystick.x, 0, joystick.y);
                if (controlForce.magnitude >= 0.1f)
                {
                    float gap = 1f;
                    Vector3 orign = transform.position + new Vector3(controlForce.normalized.x * gap, 2.8f, controlForce.normalized.z * gap);
                    Vector3 direction = new Vector3(0, -1, 0);
                    //DebugManager.Instance.AddDrawGizmos(gameObject.GetInstanceID() + "push", () =>
                    //{
                    //    Gizmos.color = Color.red;
                    //    Gizmos.DrawRay(piont);
                    //});
                    OverLadderRaycast(orign, direction, 1f, Tools.GetLayerMask(new List<string>() { "Ground" }), (RaycastHit hitInfo) =>
                    {
                        //DebugManager.Instance.AddDrawGizmos(gameObject.GetInstanceID() + "push1", () =>
                        //{
                        //    Gizmos.color = Color.red;
                        //    Gizmos.DrawSphere(hitInfo.point, 0.5f);
                        //});
                        Vector3 dir = new Vector3(controlForce.normalized.x, 0, controlForce.normalized.z);
                        OverLadderRaycast(transform.position, dir, 2f, Tools.GetLayerMask(new List<string>() { "SceneComponent" }), (RaycastHit wallhitInfo) =>
                        {
                            var center = wallhitInfo.collider.gameObject.GetComponent<SceneCenterWallComponent>();
                            var side = wallhitInfo.collider.gameObject.GetComponent<SceneSideWallComponent>();
                            float height = 1.8f;
                            if (center != null)
                            {
                                ClimbWallEdgePlayAnim(2, 5f, height, hitInfo);
                            }
                            else if (side != null)
                            {
                                ClimbWallEdgePlayAnim(1, 5f, height, hitInfo);
                            }
                            else
                            { }
                        });
                    });
                }
            }

        }
        public void SetBackwards(Vector3 offest)
        {
            if (gameObject.GetComponent<RoleBehaviorTree>() != null)
            {
                if (_currAnimatorStateInfo.IsTag("idle")
                    || _currAnimatorStateInfo.IsTag("walk")
                    || _currAnimatorStateInfo.IsTag("run"))
                {
                    if (direction == Direction.Left)
                    {
                        if (offest.x > 0)
                        {
                            SkeletonAnimator.SetBackwards(true);
                        }
                        else
                        {
                            SkeletonAnimator.SetBackwards(false);
                        }
                    }
                    else
                    {
                        if (offest.x < 0)
                        {
                            SkeletonAnimator.SetBackwards(true);
                        }
                        else
                        {
                            SkeletonAnimator.SetBackwards(false);
                        }
                    }
                }
                else
                {
                    SkeletonAnimator.SetBackwards(false);
                }
            }
        }

        private void ClimbWallEdgePlayAnim(int ClimbType, float waittime, float height, RaycastHit hitInfo)
        {
            if (animatorParams.ContainsKey("climb_begin") || animatorParams.ContainsKey("clim_type"))
            {

                if (hitInfo.point.y - transform.position.y > height - 0.5f
                    && hitInfo.point.y - transform.position.y < height + 0.7f)
                {
                    float posy = hitInfo.point.y - height;
                    FirstClimbPos = hitInfo.point;
                    _animator.SetInteger("clim_type", ClimbType);
                    _animator.SetBool("climb_begin", true);
                    DelayFunc("climb_drop", () =>
                    {
                        if (animatorParams.ContainsKey("climb_drop"))
                        {
                            _animator.SetBool("climb_drop", true);
                        }
                    }, waittime);
                }
            }
        }

        private void SlopeRaycast()
        {
            Vector3 controlForce = new Vector3(joystick.x, 0, joystick.y);
            Vector3 origin = transform.position + new Vector3(controlForce.normalized.x * 0.5f, 2f, controlForce.normalized.z * 0.5f);
            Vector3 direction = new Vector3(0, -1, 0);
            OverLadderRaycast(origin, direction, 2.5f, Tools.GetLayerMask(new List<string>() { "SceneComponent" }), (RaycastHit raycastHit) =>
            {
                DebugManager.Instance.AddDrawGizmos(gameObject.GetInstanceID() + "push22", () =>
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(raycastHit.point, 0.5f);
                });
                var sceneSlopeComponent = raycastHit.collider.gameObject.GetComponent<SceneSlopeComponent>();
                if (sceneSlopeComponent != null)
                {
                    _characterController.slopeLimit = 60f;
                    _characterController.stepOffset = 0.5f;
                }
                else
                {
                    _characterController.slopeLimit = 0f;
                    _characterController.stepOffset = 0f;
                }
            });
        }

    }
}