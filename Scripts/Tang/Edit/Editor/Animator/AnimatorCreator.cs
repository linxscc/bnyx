
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityCapsuleCollider;
using GameCreator.Variables;
using JetBrains.Annotations;
using UnityEditor.Animations;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;
using Debug = System.Diagnostics.Debug;
using Object = System.Object;

namespace Tang.Editor
{
    public class AnimatorCreator
    {
        private RoleAnimator roleAnimator;
        private AnimatorControllerConfig animatorControllerConfig;

        public RoleAnimator RoleAnimator => roleAnimator;
        
        public AnimatorControllerConfig GetConfig() => animatorControllerConfig;
        
        public AnimatorCreator(RoleAnimator roleAnimator)
        {
            this.roleAnimator = roleAnimator; 
            
            animatorControllerConfig = new AnimatorControllerConfig();
            // 添加参数
            animatorControllerConfig.parameters = new List<AnimatorParameter>()
            {
                new AnimatorParameter()
                {
                    name = "action_roll",
                    type = AnimatorControllerParameterType.Int
                },
                new AnimatorParameter()
                {
                    name = "move_state",
                    type = AnimatorControllerParameterType.Int
                },
                new AnimatorParameter()
                {
                    name = "WalkCut",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "is_turnback",
                    type = AnimatorControllerParameterType.Bool
                },
                new AnimatorParameter()
                {
                    name = "Alt",
                    type = AnimatorControllerParameterType.Bool
                },
                new AnimatorParameter()
                {
                    name = "action1_begin",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "action2_begin",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "action3_begin",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "action4_begin",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "action5_begin",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "action6_begin",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "action7_begin",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "action8_begin",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "action9_begin",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "action10_begin",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "action_jump",
                    type = AnimatorControllerParameterType.Int
                },
                new AnimatorParameter()
                {
                    name = "isGround",
                    type = AnimatorControllerParameterType.Bool
                },
                new AnimatorParameter()
                {
                    name = "hurt",
                    type = AnimatorControllerParameterType.Bool
                },
                new AnimatorParameter()
                {
                    name = "hurt_type",
                    type = AnimatorControllerParameterType.Int
                },
                new AnimatorParameter()
                {
                    name = "hurt_direction",
                    type = AnimatorControllerParameterType.Float
                },
                new AnimatorParameter()
                {
                    name = "hurt_force_value",
                    type = AnimatorControllerParameterType.Float
                },
                new AnimatorParameter()
                {
                    name = "isDead",
                    type = AnimatorControllerParameterType.Bool
                },
                new AnimatorParameter()
                {
                    name = "KeyBoard1",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "KeyBoard2",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "KeyBoard3",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "KeyBoard4",
                    type = AnimatorControllerParameterType.Trigger
                },
                new AnimatorParameter()
                {
                    name = "MoveSpeed",
                    type = AnimatorControllerParameterType.Float
                },
                new AnimatorParameter()
                {
                    name = "curr_jump_time",
                    type = AnimatorControllerParameterType.Int
                },
                new AnimatorParameter()
                {
                    name = "speedy",
                    type = AnimatorControllerParameterType.Float
                },
                new AnimatorParameter()
                {
                    name = "jump",
                    type = AnimatorControllerParameterType.Bool
                }
            };

            // 添加tag列表
            animatorControllerConfig.Tags = new List<AnimatorTag>()
            {
                new AnimatorTag(){tag = "idle"},
                new AnimatorTag(){tag = "walk"},
                new AnimatorTag(){tag = "rewalk"},
                new AnimatorTag(){tag = "run"},
                new AnimatorTag(){tag = "die"},
                new AnimatorTag(){tag = "action"},
                new AnimatorTag(){tag = "hurt"},
                new AnimatorTag(){tag = "landing"},
                new AnimatorTag(){tag = "jumping"},
                new AnimatorTag(){tag = "jumpingAttack"},
                new AnimatorTag(){tag = "roll"},
                new AnimatorTag(){tag = "attack"}
            };
        }

        public void Create()
        {
            StateCreate();
            StateTransition();
            
//            SetDurations(new List<string>() {"idle", "walk", "run", }, 0.2f);
//            SetDurations(new List<string>() {"idle", "roll" }, 0.1f);
//            SetDurations(new List<string>() {"run", "roll" }, 0.1f);
//            SetDurations(new List<string>() {"walk", "roll" }, 0.1f);
        }

        private void StateCreate()
        {
            CreateIdles();
            CreateWalks();
            CreateDeaths();
            CreateActions();
            CreateHurts();
            CreateJumps();
            
        }

        private void StateTransition()
        {
            AllToDying();
            AllToHurt1();
            HurtToHurt();
            HurtToIdle();
            HurtToAction();
            HurtToMove();
            IdleTo();
            WalkTo();
            ActionToIdle();
            IdleToIdle();
//            WalkToWalk();
            JumpTo();
            JumpAttackTo();
        }


        private void SetDurations(List<string> tagList, float duration)
        {
            foreach (var fromTag in tagList)
            {
                foreach (var toTag in tagList)
                {
                    SetDurations(fromTag, toTag, duration);
                }    
            }
        }

        private void SetDurations(string fromTag, string toTag, float duration)
        {
            foreach (var stateConfig in animatorControllerConfig.states)
            {
                if (stateConfig.tag == fromTag)
                {
                    foreach (var transition in stateConfig.transitions)
                    {
                        var toStateConfig = animatorControllerConfig.states.Find(state_ => state_.name == transition.destinationAnimName);

                        if (toStateConfig.tag != null && toStateConfig.tag == toTag)
                        {
                            transition.duration = duration;
                        }
                    }
                }
            }
        }

        private void CreateIdles()
        {
            foreach (var itor in roleAnimator.idle)
            {
                CreateIdle(itor.Value);
            }
        }
        private void CreateIdle(RoleIdleState state)
        {
            AnimatorState state1;
            
            // 动画1
            {
                state1 = new AnimatorState();
                state1.name = state.StateName;
                state1.animName = state.AnimName;
                state1.tag = state.Tag;
                state1.Speed = GetPlaySpeed(state.Start, state.End, state.Speed);
                state1.loop = true;
                
                animatorControllerConfig.states.Add(state1);
            }
        }

        private void CreateWalks()
        {
            foreach (var itor in roleAnimator.walk)
            {
                CreateWalk(itor.Value);
            }
        }
        private void CreateWalk(RoleWalkState state)
        {
            AnimatorState state1;
            
            // 动画1
            {
                state1 = new AnimatorState();
                state1.name = state.StateName;
                state1.animName = state.AnimName;
                state1.tag = state.Tag;
                state1.Speed = GetPlaySpeed(state.Start,state.End,state.Speed);
                state1.MinSpeed = state.MinSpeed;
                if (state1.tag == "roll")
                {
                    state1.loop = false;
                }
                else
                {
                    state1.loop = true;
                }
                animatorControllerConfig.states.Add(state1); 
            }
        }

        private void CreateDeaths()
        {
            foreach (var itor in roleAnimator.death)
            {
                CreateDeath(itor.Value);
            }
        }
        private void CreateDeath(RoleDeathState state)
        {
            AnimatorState state1;
            AnimatorState state2;
            // 动画1
            {
                state1 = new AnimatorState();
                state1.name = state.StateName;
                state1.animName = state.AnimName01;
                state1.tag = "die";
//                state1.speed = state.Speed01;
                state1.Speed = GetPlaySpeed(state.Start01,state.End01,state.Speed01);
                
                animatorControllerConfig.states.Add(state1); 
            }
            
            // 动画2
            {
                state2 = new AnimatorState();
                state2.name = "Death";
                state2.animName = state.AnimName02;
                state2.tag = "die";
//                state1.speed = state.Speed02;
                state2.Speed = GetPlaySpeed(state.Start02,state.End02,state.Speed02);
                
                animatorControllerConfig.states.Add(state2); 
            }
            
            // 链接state1 -> state2
            {
                AnimatorTransition transition = new AnimatorTransition();
                transition.destinationAnimName = state2.name;
                transition.hasExitTime = true;

                state1.transitions.Add(transition);
            }

        }
        
        private void CreateActions()
        {
            //添加action
            foreach (var itor in roleAnimator.action)
            {
                CreateAction(itor.Value);
            }
        }
        private void CreateAction(RoleActionState state)
        {
            AnimatorState state1;
            AnimatorState state2;
            AnimatorState state3;
            AnimatorState state4;

            // 动画1
            {
                state1 = new AnimatorState();
                state1.totalFrame = state.AnimName01_totalFrame;
                state1.name = state.StateName + "_1";
                state1.animName = state.AnimName01;
                state1.tag = state.Tag;
//                state1.speed = state.AnimName01_Speed;
                state1.Speed = GetPlaySpeed(state.AnimName01_Start,state.AnimName01_End,state.AnimName01_Speed);

                state1.MoveVector = state.MoveVector;

                state1.beginTime = FrameStartToTime(state.AnimName01_Start);
                state1.endTime = FrameEndToTime(state.AnimName01_End);

                animatorControllerConfig.states.Add(state1); 
            }
            
            //动画2
            {
                state2 = new AnimatorState();
                state2.totalFrame = state.AnimName02_totalFrame;
                state2.name = state.StateName + "_2";
                state2.animName = string.IsNullOrEmpty(state.AnimName02) ? state.AnimName01 : state.AnimName02;
                state2.tag = state.Tag;
//                state1.speed = state.AnimName02_Speed;
                state2.Speed = GetPlaySpeed(state.AnimName02_Start,state.AnimName02_End,state.AnimName02_Speed);
                
                state2.MoveVector = state.MoveVector;
                state2.beginTime = FrameStartToTime(state.AnimName02_Start);
                state2.endTime = FrameEndToTime(state.AnimName02_End);
                
                animatorControllerConfig.states.Add(state2); 
            }
            
            //动画3
            {
                state3 = new AnimatorState();
                state3.totalFrame = state.AnimName03_totalFrame;
                state3.name = state.StateName + "_3";
                state3.animName = string.IsNullOrEmpty(state.AnimName03) ? state.AnimName01 : state.AnimName03;
                state3.tag = state.Tag;
//                state1.speed = state.AnimName03_Speed;
                state3.Speed = GetPlaySpeed(state.AnimName03_Start,state.AnimName03_End,state.AnimName03_Speed);
                
                state3.MoveVector = state.MoveVector;
                
                state3.beginTime = FrameStartToTime(state.AnimName03_Start);
                state3.endTime = FrameEndToTime(state.AnimName03_End);
                
                animatorControllerConfig.states.Add(state3); 
            }
            
            //动画4
            {
                
                state4 = new AnimatorState();
                state4.totalFrame = state.AnimName04_totalFrame;
                state4.name = state.StateName + "_4";
                state4.animName = string.IsNullOrEmpty(state.AnimName04) ? state.AnimName01 : state.AnimName04;
                state4.tag = state.Tag;
//                state1.speed = state.AnimName03_Speed;
                state4.Speed = GetPlaySpeed(state.AnimName04_Start,state.AnimName04_End,state.AnimName04_Speed);
                
                state4.MoveVector = state.MoveVector;
                
                state4.beginTime = FrameStartToTime(state.AnimName04_Start);
                state4.endTime = FrameEndToTime(state.AnimName04_End);
                
                animatorControllerConfig.states.Add(state4);
            }
            
            
        }

        
        private void CreateHurts()
        {
            foreach (var itor in roleAnimator.hurt)
            {
                CreateHurt(itor.Value);
            }
        }
        private void CreateHurt(RoleHurtState state)
        {
            CreateHurtState(state.StateName + "_1", state.AnimName01, state.AnimName01_totalFrame,
                "hurt", state.AnimName01_Start, state.AnimName01_End, state.AnimName01_Speed);
            
            CreateHurtState(state.StateName + "_2", state.AnimName02, state.AnimName02_totalFrame,
                "hurt", state.AnimName02_Start, state.AnimName02_End, state.AnimName02_Speed);
            
            CreateHurtState(state.StateName + "_3", state.AnimName03, state.AnimName03_totalFrame,
                "hurt", state.AnimName03_Start, state.AnimName03_End, state.AnimName03_Speed);
        }

        private void CreateHurtState(string stateName, string animName, int totalFrame, string tag, int frameFrom, int frameTo, float duration)
        {
            var state = new AnimatorState();
            state.name = stateName;
            state.animName = animName;
            state.totalFrame = totalFrame;
            state.tag = tag;
            state.Speed = GetPlaySpeed(frameFrom, frameTo, duration);
            animatorControllerConfig.states.Add(state);
        }

        private void CreateJumps()
        {
            //添加jump
            foreach (var itor in roleAnimator.jump)
            {
                CreateJump(itor.Value);
            }
        }

        private void CreateJump(RoleJumpState state)
        {
            CreateJumpState(state.StateName + "_1", state.AnimName01, state.AnimName01_Tag,
                state.AnimName01_Start, state.AnimName01_End, state.AnimName01_Speed);
            CreateJumpState(state.StateName + "_2", state.AnimName02, state.AnimName02_Tag,
                state.AnimName02_Start, state.AnimName02_End, state.AnimName02_Speed);
            CreateJumpState(state.StateName + "_3", state.AnimName03, state.AnimName03_Tag,
                state.AnimName03_Start, state.AnimName03_End, state.AnimName03_Speed);
            CreateJumpState(state.StateName + "_4", state.AnimName04, state.AnimName04_Tag,
                state.AnimName04_Start, state.AnimName04_End, state.AnimName04_Speed);
        }

        private void CreateJumpState(string stateName, string animName, string tag, int frameFrom, int frameTo, float duration)
        {
            var state = new AnimatorState();
            state.name = stateName;
            state.animName = animName;
            state.tag = tag;
            state.Speed = GetPlaySpeed(frameFrom, frameTo, duration);
            animatorControllerConfig.states.Add(state);
        }
        


        private void HurtToIdle()
        {
            foreach (var iHurtState in roleAnimator.hurt)
            {
                var state1 =
                    animatorControllerConfig.states.Find((_state => _state.name == iHurtState.Value.StateName + "_1"));
                var state2 = 
                    animatorControllerConfig.states.Find((_state => _state.name == iHurtState.Value.StateName+"_2"));
                var state3 = 
                    animatorControllerConfig.states.Find((_state => _state.name == iHurtState.Value.StateName+"_3"));

                // 链接state1 -> state2
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = state2.name;
                    transition.hasExitTime = true;
                    transition.offset = GetOffset(iHurtState.Value.AnimName02_Start,state2.totalFrame);
                    transition.exitTime = GetExitTime(iHurtState.Value.AnimName01_End,state1.totalFrame);
                
                    state1.transitions.Add(transition);
                }

                // 链接state2 -> state3
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = state3.name;
                    transition.hasExitTime = true;
                    transition.offset = GetOffset(iHurtState.Value.AnimName03_Start,state3.totalFrame);
                    transition.exitTime = GetExitTime(iHurtState.Value.AnimName02_End,state2.totalFrame);
                 
                    state2.transitions.Add(transition);
                }

                // 链接state3 -> Idle
                {
                    foreach (var itemidle in roleAnimator.idle)
                    {
                        addToWalkConditionByTag(state3, itemidle.Value, itemidle.Value.Tag);
                    }
                }
            }
        }

        private void ActionToIdle()
        {
            foreach (var actionAnimName in roleAnimator.action)
            {
                int n = 1;
                
                var state1 = animatorControllerConfig.states.Find(
                    (_state => _state.name == actionAnimName.Value.StateName + "_1"));
                var state2 = animatorControllerConfig.states.Find(
                    (_state => _state.name == actionAnimName.Value.StateName + "_2"));
                var state3 = animatorControllerConfig.states.Find(
                    (_state => _state.name == actionAnimName.Value.StateName + "_3"));
                var state4 = animatorControllerConfig.states.Find(
                    (_state => _state.name == actionAnimName.Value.StateName + "_4"));
//                var staten = animatorControllerConfig.states.Find(
//                    (_state => _state.name == actionAnimName.Value.StateName + "_" + n));


                // 链接state1 -> state2
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = state2.name;
                    transition.hasExitTime = true;
                    transition.offset = GetOffset(actionAnimName.Value.AnimName02_Start,actionAnimName.Value.AnimName02_totalFrame);
                    transition.exitTime = GetExitTime(actionAnimName.Value.AnimName01_End,actionAnimName.Value.AnimName01_totalFrame);
                
                    state1.transitions.Add(transition);
                }
            
                // 链接state2 -> state3
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = state3.name;
                    transition.hasExitTime = true;
                    transition.offset = GetOffset(actionAnimName.Value.AnimName03_Start,actionAnimName.Value.AnimName03_totalFrame);
                    transition.exitTime = GetExitTime(actionAnimName.Value.AnimName02_End,actionAnimName.Value.AnimName02_totalFrame);
                
                    state2.transitions.Add(transition);
                }
                
                // 链接state3 -> state4
                {
//                    state4 = RoleWaitI
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = state4.name;
                    transition.hasExitTime = true;
                    transition.offset = GetOffset(actionAnimName.Value.AnimName04_Start,actionAnimName.Value.AnimName04_totalFrame);
                    transition.exitTime = GetExitTime(actionAnimName.Value.AnimName03_End,actionAnimName.Value.AnimName03_totalFrame);
                
                    state3.transitions.Add(transition);
                }
                // 链接state4 -> 移动walk,run
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    foreach (var iRun in roleAnimator.walk)
                    {
                        var run = animatorControllerConfig.states.Find(
                            (_state => _state.name == iRun.Value.StateName));
                        TryAddTransitionStateToState(state4, run, actionAnimName.Value, iRun.Value);
                    }
                }
                // 链接state4 -> 攻击action
                {
                    foreach (var iAction in roleAnimator.action)
                    {
                        var toAnimatorAction = animatorControllerConfig.states.Find(
                            (_state => _state.name == iAction.Value.StateName + "_1"));
                        TryAddTransitionStateToState(state4, toAnimatorAction, actionAnimName.Value, iAction.Value);
                    }
                }
                
                
                // 链接state4 -> Idle[0]
                {
                    foreach (var itemidle in roleAnimator.idle)
                    {
                        var toAnimatorState = animatorControllerConfig.states.Find(
                            (_state => _state.name == itemidle.Value.StateName));
                        TryAddTransitionStateToState(state4, toAnimatorState, actionAnimName.Value, itemidle.Value);
                    }
                }
            }
        }

        private void HurtToHurt()
        {
            foreach (var fromHurt in roleAnimator.hurt)
            {
                var fromState1 = animatorControllerConfig.states.Find((_state =>_state.name == fromHurt.Value.StateName + "_1"));
                var fromState2 = animatorControllerConfig.states.Find((_state => _state.name == fromHurt.Value.StateName + "_2"));
                var fromState3 = animatorControllerConfig.states.Find((_state => _state.name == fromHurt.Value.StateName + "_3"));

                foreach (var toHurt in roleAnimator.hurt)
                {
                    var toState1 = animatorControllerConfig.states.Find((_state =>_state.name == toHurt.Value.StateName + "_1"));

                    {
                        AnimatorTransition transition = new AnimatorTransition();
                        transition.destinationAnimName = toState1.name;
                        transition.hasExitTime = true;
                        transition.offset = GetOffset(toHurt.Value.AnimName01_Start, toState1.totalFrame);
                        transition.exitTime = GetExitTime(fromHurt.Value.AnimName01_End, fromState1.totalFrame);
                        transition.conditions = new List<AnimatorTransitionCondition>()
                        {
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.Greater,
                                parameter = "hurt_direction",
                                threshold = 0
                            },
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.If,
                                parameter = "hurt",
                                threshold = 0.1f
                            },
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.Greater,
                                parameter = "hurt_force_value",
                                threshold = toHurt.Value.ForceValue
                            }
                        };

                        fromState1.transitions.Add(transition);
                    }
                    
                    {
                        AnimatorTransition transition = new AnimatorTransition();
                        transition.destinationAnimName = toState1.name;
                        transition.hasExitTime = true;
                        transition.offset = GetOffset(toHurt.Value.AnimName01_Start, toState1.totalFrame);
                        transition.exitTime = GetExitTime(fromHurt.Value.AnimName02_End, fromState2.totalFrame);
                        transition.conditions = new List<AnimatorTransitionCondition>()
                        {
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.Greater,
                                parameter = "hurt_direction",
                                threshold = 0
                            },
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.If,
                                parameter = "hurt",
                                threshold = 0.1f
                            },
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.Greater,
                                parameter = "hurt_force_value",
                                threshold = toHurt.Value.ForceValue
                            }
                        };

                        fromState2.transitions.Add(transition);  
                    }
                    
                    {
                        AnimatorTransition transition = new AnimatorTransition();
                        transition.destinationAnimName = toState1.name;
                        transition.hasExitTime = true;
                        transition.offset = GetOffset(toHurt.Value.AnimName01_Start, toState1.totalFrame);
                        transition.exitTime = GetExitTime(fromHurt.Value.AnimName03_End, fromState3.totalFrame);
                        transition.conditions = new List<AnimatorTransitionCondition>()
                        {
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.Greater,
                                parameter = "hurt_direction",
                                threshold = 0
                            },
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.If,
                                parameter = "hurt",
                                threshold = 0.1f
                            },
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.Greater,
                                parameter = "hurt_force_value",
                                threshold = toHurt.Value.ForceValue
                            }
                        };

                        fromState3.transitions.Add(transition);  
                    }
                }
                
//                foreach (var iHurtStateStart in roleAnimator.hurt)
//                {
//                    //state1 -> state1
//                    {
//                        switch (iHurtState.Value.StateName)
//                        {
//                            case "fly": //重击
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state2 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_1"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName01_End, state1.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state2.transitions.Add(transition);
//                                break;
//                            case "backhurt": //背身受击
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state2 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_1"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName01_End, state1.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_direction",
//                                        threshold = 0
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state2.transitions.Add(transition);
//                                break;
//                            case "fallhurt": //击倒
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state2 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_1"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName01_End, state1.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state2.transitions.Add(transition);
//                                break;
//                            case "softhurt": //轻击
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state2 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_1"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName01_End, state1.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state2.transitions.Add(transition);
//                                break;
//                        }
//                    }
//
//                }
//
//                foreach (var iHurtStateStart in roleAnimator.hurt)
//                {
//                    //state2 -> state1
//                    {
//                        switch (iHurtState.Value.StateName)
//                        {
//                            case "fly": //重击
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state2 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_2"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName02_End, state2.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state2.transitions.Add(transition);
//                                break;
//                            case "backhurt": //背身受击
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state2 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_2"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName02_End, state2.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_direction",
//                                        threshold = 0
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state2.transitions.Add(transition);
//                                break;
//                            case "fall": //击倒
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state2 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_2"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName02_End, state2.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state2.transitions.Add(transition);
//                                break;
//                            case "softhurt": //轻击
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state2 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_2"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName02_End, state2.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state2.transitions.Add(transition);
//                                break;
//                        }
//                    }
//
//                }
//                
//                foreach (var iHurtStateStart in roleAnimator.hurt)
//                {
//                    //state3 -> state1
//                    {
//                        switch (iHurtState.Value.StateName)
//                        {
//                            case "fly": //重击
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state3 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_3"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName03_End, state3.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state3.transitions.Add(transition);
//                                break;
//                            case "backhurt": //背身受击
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state3 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_3"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName03_End, state3.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_direction",
//                                        threshold = 0
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state3.transitions.Add(transition);
//                                break;
//                            case "fall": //击倒
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state3 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_3"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName03_End, state3.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state3.transitions.Add(transition);
//                                break;
//                            case "softhurt": //轻击
//                                state1 = animatorControllerConfig.states.Find((_state =>
//                                    _state.name == iHurtState.Value.StateName + "_1"));
//                                state3 =animatorControllerConfig.states.Find((_state => 
//                                    _state.name == iHurtStateStart.Value.StateName + "_3"));
//                                transition.destinationAnimName = state1.name;
//                                transition.hasExitTime = true;
//                                transition.offset = GetOffset(iHurtState.Value.AnimName01_Start, state1.totalFrame);
//                                transition.exitTime = GetExitTime(iHurtStateStart.Value.AnimName03_End, state3.totalFrame);
//                                transition.conditions = new List<AnimatorTransitionCondition>()
//                                {
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.If,
//                                        parameter = "hurt",
//                                        threshold = 0.1f
//                                    },
//                                    new AnimatorTransitionCondition()
//                                    {
//                                        animatorConditionMode = AnimatorConditionMode.Greater,
//                                        parameter = "hurt_force_value",
//                                        threshold = iHurtStateStart.Value.ForceValue
//                                    }
//                                };
//
//                                state3.transitions.Add(transition);
//                                break;
//                        }
//                    }
//
//                }
            }
        }

        private void HurtToAction()
        {
            foreach (var iHurtState in roleAnimator.hurt)
            {
                var state3 = animatorControllerConfig.states.Find
                    ((_state => _state.name == iHurtState.Value.StateName+"_3"));
                
                foreach (var iAction in roleAnimator.action)
                {
                    var toAnimatorAction = animatorControllerConfig.states.Find
                        ((_state => _state.name == iAction.Value.StateName+"_1"));
                    TryAddTransitionStateToState(state3, toAnimatorAction, iHurtState.Value, iAction.Value);
                }
            }
        }

        private void HurtToMove()
        {
            //walk
            foreach (var iHurtState in roleAnimator.hurt)
            {
                var state3 = animatorControllerConfig.states.Find
                    ((_state => _state.name == iHurtState.Value.StateName+"_3"));

                foreach (var iWalkState in roleAnimator.walk)
                {
                    var toAnimatorState = animatorControllerConfig.states.Find
                        ((_state => _state.name == iWalkState.Value.StateName));
                    TryAddTransitionStateToState(state3, toAnimatorState, iHurtState.Value, iWalkState.Value);
                }
            }
        }

        private void IdleTo()
        {
            // 连接idle到action
            foreach (var idleItor in roleAnimator.idle)
            {
                var fromAnimatorState = animatorControllerConfig.states.Find(
                    (_state => _state.name == idleItor.Value.StateName));

                foreach (var actionItor in roleAnimator.action)
                {
                    var toAnimatorState =
                        animatorControllerConfig.states.Find((_state => _state.name == actionItor.Value.StateName + "_1"));
                    TryAddTransitionStateToState(fromAnimatorState, toAnimatorState,
                            idleItor.Value, actionItor.Value);
                }
            }
            
            // 连接idle => walk,dodge,run
            foreach (var itor in roleAnimator.idle)
            {
                var fromState = animatorControllerConfig.states.Find((_state => _state.name == itor.Value.StateName));
                
                foreach (var item in roleAnimator.walk)
                {
                    var toState = animatorControllerConfig.states.Find((_state => _state.name == item.Value.StateName));
                    TryAddTransitionStateToState(fromState, toState, fromState.animState, toState.animState);
                }
            }

            // 连接idle => jump
            foreach (var idleToJump in roleAnimator.idle)
            {
                var state = animatorControllerConfig.states.Find((_state => _state.name == idleToJump.Value.StateName));
                
                foreach (var item in roleAnimator.jump)
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = item.Value.StateName + "_1";
                    transition.hasExitTime = false;
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Equals,
                            parameter = "action_jump",
                            threshold = 1
                        }
                    };
                    state.transitions.Add(transition);    
                }
            }
        }

        private void IdleToIdle()
        {
            foreach (var itor in roleAnimator.idle)
            {
                var fromState = animatorControllerConfig.states.Find((_state => _state.name == itor.Value.StateName));

                for (int i = 1; i <= 9; i++)
                {
                    RoleIdleState toIdle;
                    if (roleAnimator.idle.TryGetValue("idle" + i, out toIdle))
                    {
                        AnimatorTransition transition = new AnimatorTransition();
                        transition.destinationAnimName = toIdle.StateName;
                        transition.hasExitTime = false;
                        transition.conditions = new List<AnimatorTransitionCondition>()
                        {
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.If,
                                parameter = "KeyBoard" + i,
                                threshold = 0.1f
                            }
                        };
                        fromState.transitions.Add(transition);   
                    }
                }
            }
        }

        private void WalkTo()
        {
            // 连接walk,run,dodge到action
            foreach (var idleItor in roleAnimator.walk)
            {
                var fromAnimatorState = animatorControllerConfig.states.Find((_state => _state.name == idleItor.Value.StateName));

                foreach (var actionItor in roleAnimator.action)
                {
                    var toAnimatorState = animatorControllerConfig.states.Find((_state => _state.name == actionItor.Value.StateName + "_1"));
                    TryAddTransitionStateToState(fromAnimatorState, toAnimatorState, idleItor.Value, actionItor.Value);
                }
            }

            // 连接walk,run,dodge到jump
            foreach (var walkToJump in roleAnimator.walk)
            {
                var state = animatorControllerConfig.states.Find((_state => _state.name == walkToJump.Value.StateName));

                {
                    // => jumpPre
                    foreach (var item in roleAnimator.jump)
                    {
                        AnimatorTransition transition = new AnimatorTransition();
                        transition.destinationAnimName = item.Value.StateName + "_1";
                        transition.hasExitTime = false;
                        transition.conditions = new List<AnimatorTransitionCondition>()
                        {
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.Equals,
                                parameter = "action_jump",
                                threshold = 1
                            }
                        };
                        state.transitions.Add(transition);    
                    }
                    
                    // => jump3
                    foreach (var item in roleAnimator.jump)
                    {
                        AnimatorTransition transition = new AnimatorTransition();
                        transition.destinationAnimName = item.Value.StateName + "_3";
                        transition.hasExitTime = false;
                        transition.conditions = new List<AnimatorTransitionCondition>()
                        {
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.Less,
                                parameter = "speedy",
                                threshold = 0
                            },
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.IfNot,
                                parameter = "isGround",
                                threshold = 0.1f
                            }
                        };
                        state.transitions.Add(transition);    
                    }
                    
                }
            }
            
            // 连接walk,run,dodge到idle,run,dodge
            foreach (var itor in roleAnimator.walk)
            {
                var fromAnimatorState = animatorControllerConfig.states.Find((_state => _state.name == itor.Value.StateName));
                // => idle
                foreach (var itemidle in roleAnimator.idle)
                {
                    var toAnimatorState = animatorControllerConfig.states.Find((_state => _state.name == itemidle.Value.StateName));
                    TryAddTransitionStateToState(fromAnimatorState, toAnimatorState, itor.Value, itemidle.Value);
                }
                
                //=> run,dodge
                foreach (var itemrun in roleAnimator.walk)
                {
                    var toAnimatorState = animatorControllerConfig.states.Find((_state => _state.name == itemrun.Value.StateName));
                    TryAddTransitionStateToState(fromAnimatorState, toAnimatorState, itor.Value, itemrun.Value);
                }
            }
        }

        private void WalkToWalk()
        {
            List<RoleWalkState> walkStates = new List<RoleWalkState>()
            {
                
            };

            List<RoleWalkState> roleWalkStateList = roleAnimator.walk.Values.ToList();
//            roleWalkStateList.RemoveAll(roleWalkStateList.FindAll() != "walk");
            
            for (int i = 0; i < roleWalkStateList.Count; i++)
            {
                RoleWalkState fromRoleWalkState = roleWalkStateList[i % roleWalkStateList.Count];
                RoleWalkState toRoleWalkState = roleWalkStateList[(i + 1) % roleWalkStateList.Count];
                
                var fromState = animatorControllerConfig.states.Find((_state => _state.name == fromRoleWalkState.StateName));
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = toRoleWalkState.StateName;
                    transition.hasExitTime = false;
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = "WalkCut",
                            threshold = 0.1f
                        }
                    };

                    fromState.transitions.Add(transition);
                }
            }
        }

        private void JumpTo()
        {
            List<RoleJumpState> roleJumpStateList = roleAnimator.jump.Values.ToList();
            
            // jump内单线连接
            foreach (var jumpAnimator in roleAnimator.jump)
            {
                int lastFrame = jumpAnimator.Value.AnimName04_End;
                
                var state1 = animatorControllerConfig.states.Find(
                    (_state => _state.name == jumpAnimator.Value.StateName + "_1"));
                var state2 = animatorControllerConfig.states.Find(
                    (_state => _state.name == jumpAnimator.Value.StateName + "_2"));
                var state3 = animatorControllerConfig.states.Find(
                    (_state => _state.name == jumpAnimator.Value.StateName + "_3"));
                var state4 = animatorControllerConfig.states.Find(
                    (_state => _state.name == jumpAnimator.Value.StateName + "_4"));
//                var staten = animatorControllerConfig.states.Find(
//                    (_state => _state.name == actionAnimName.Value.StateName + "_" + n));

                // 链接state1 -> state2
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = state2.name;
                    transition.hasExitTime = true;
                    transition.offset = GetOffset(jumpAnimator.Value.AnimName02_Start,jumpAnimator.Value.AnimName01_End);
                    transition.exitTime = GetExitTime(jumpAnimator.Value.AnimName01_End,jumpAnimator.Value.AnimName01_End);

                    state1.transitions.Add(transition);
                }
            
                // 链接state2 -> state3
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = state3.name;
                    transition.hasExitTime = false;
                    transition.offset = GetOffset(jumpAnimator.Value.AnimName03_Start,jumpAnimator.Value.AnimName02_End);
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Less,
                            parameter = "speedy",
                            threshold = 0
                        }
                    };
                
                    state2.transitions.Add(transition);
                }
                
                // 链接state2 -> state4
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = state4.name;
                    transition.hasExitTime = true;
                    transition.offset = GetOffset(jumpAnimator.Value.AnimName04_Start,jumpAnimator.Value.AnimName02_End);
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = "isGround",
                            threshold = 0.1f
                        }
                    };
                
                    state2.transitions.Add(transition);
                }
                
                // 链接state3 -> state4
                {
//                    state4 = RoleWaitI
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = state4.name;
                    transition.hasExitTime = false;
                    transition.offset = GetOffset(jumpAnimator.Value.AnimName04_Start,jumpAnimator.Value.AnimName03_End);
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = "isGround",
                            threshold = 0.1f
                        }
                    };
                
                    state3.transitions.Add(transition);
                }
                
                // 链接state4 -> 受击hurt
                {
                    foreach (var itemhurt in roleAnimator.hurt) 
                    {
                        addHurtConditions(itemhurt, state4);
                    }
                }
                // 链接state4 -> 攻击action
                {
                    foreach (var iAction in roleAnimator.action)
                    {
                        AnimatorTransition transition = new AnimatorTransition();
                        lastFrame = state4.totalFrame;
                        transition.destinationAnimName = iAction.Value.StateName + "_1"; 
                        transition.hasExitTime = false;
                        transition.exitTime = GetExitTime(jumpAnimator.Value.AnimName04_End,lastFrame);
                        transition.conditions = new List<AnimatorTransitionCondition>()
                        {
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.If,
                                parameter = iAction.Value.StateName + "_begin",
                                threshold = 1
                            }
                        }; 
                        
                        state4.transitions.Add(transition);
                    }
                }
                
            }
            
            // jump4连接idle,dodge
            foreach (var itor in roleAnimator.jump)
            {
                var state = animatorControllerConfig.states.Find((_state => _state.name == itor.Value.StateName + "_4"));
                // => idle
                foreach (var itemidle in roleAnimator.idle)
                {
                    addToWalkConditionByTag(state, itemidle.Value, itemidle.Value.Tag);
                }
                
                // => dodge,walk,run
                foreach (var move in roleAnimator.walk)
                {
                    addToWalkConditionByTag(state, move.Value, move.Value.Tag);
                }
            }
            
            // jump连接攻击
            foreach (var jumpState in roleAnimator.jump)
            {
                AnimatorTransition transition;
                var state2 = animatorControllerConfig.states.Find(
                    (_state => _state.name == jumpState.Value.StateName + "_2"));
                var state3 = animatorControllerConfig.states.Find(
                    (_state => _state.name == jumpState.Value.StateName + "_3"));
                
                // 2,3 => 攻击
                foreach (var toJumpAttack in roleAnimator.action)
                {
                    if (toJumpAttack.Value.Tag == "jumpingAttack")
                    {
                        addToAction1ConditionByTag(state2, toJumpAttack.Value, toJumpAttack.Value.Tag);
                        addToAction1ConditionByTag(state3, toJumpAttack.Value, toJumpAttack.Value.Tag);
                    }
                }

//                List<RoleSoftJumpActionState> RoleSoftJumpActionState = roleAnimator.jumpSoftAttack.Values.ToList();
//                RoleSoftJumpActionState toRoleJumpState = RoleSoftJumpActionState[0];
//                var softJumpActionState = animatorControllerConfig.states.Find(
//                    (_state => _state.name == toRoleJumpState.StateName + "_1"));
//                
//                // 2 => 轻击
//                transition = new AnimatorTransition();
//                transition.destinationAnimName = softJumpActionState.name;
//                transition.hasExitTime = false;
//                transition.offset = GetOffset(toRoleJumpState.AnimName01_Start,toRoleJumpState.AnimName01_totalFrame);
//                transition.conditions = new List<AnimatorTransitionCondition>()
//                {
//                    new AnimatorTransitionCondition()
//                    {
//                        animatorConditionMode = AnimatorConditionMode.If,
//                        parameter = "action1_begin",
//                        threshold = 0.1f
//                    }
//                };
//                state2.transitions.Add(transition);
//                
//                // 3 => 轻击
//                transition = new AnimatorTransition();
//                transition.destinationAnimName = softJumpActionState.name;
//                transition.hasExitTime = false;
//                transition.offset = GetOffset(toRoleJumpState.AnimName01_Start,toRoleJumpState.AnimName01_totalFrame);
//                transition.conditions = new List<AnimatorTransitionCondition>()
//                {
//                    new AnimatorTransitionCondition()
//                    {
//                        animatorConditionMode = AnimatorConditionMode.If,
//                        parameter = "action1_begin",
//                        threshold = 0.1f
//                    }
//                };
//                state3.transitions.Add(transition);
            }
            
        }
        private void AllToHurt1()
        {
            // 所有到受击
            foreach (var fromState in animatorControllerConfig.states)
            {
                if (fromState.tag == "die") continue;

                foreach (var itemhurt in roleAnimator.hurt)
                {
                    addHurtConditions(itemhurt,fromState);
                }
            }
        }
        private void AllToDying()
        {
            //All -> Dying
            foreach (var fromState in animatorControllerConfig.states)
            {
                foreach (var itemdeath in roleAnimator.death)
                {
                    AnimatorTransition transition = new AnimatorTransition
                    {
                        destinationAnimName = itemdeath.Value.StateName,
                        ignoreSourceAnimTags = new List<string> {"die"},
                        hasExitTime = false,
                        conditions = new List<AnimatorTransitionCondition>()
                        {
                            new AnimatorTransitionCondition()
                            {
                                animatorConditionMode = AnimatorConditionMode.If,
                                parameter = "isDead",
                                threshold = 0.1f
                            }
                        }
                    };
                    fromState.transitions.Add(transition);
                }
            }
        }

        private void JumpAttackTo()
        {
            foreach (var jumpActionState in roleAnimator.action)
            {
                if (jumpActionState.Value.Tag == "jumpingAttack")
                {
                    var state4 =animatorControllerConfig.states.Find(
                        (_state => _state.name == jumpActionState.Value.StateName + "_4"));
                    addJumpAction4To(state4);
                }
            }
        }


//        private void createAnimController(RoleAnimator)
//        {
//            AnimatorState state1;
//            state1 = new AnimatorState();
//            state1.name = state.StateName;
//            state1.animName = state.AnimName1;
//            state1.tag = state.Tag;
//            state1.Speed = GetPlaySpeed(state.Start, state.End, state.Speed);
//            state1.loop = true;
//                
//            animatorControllerConfig.states.Add(state1);
//        }




        void addConditionByTag(string excelTag, AnimatorState ActionState, RoleAnimState toState, List<AnimatorTag> animatorTag)
        {
            foreach (var tag in animatorTag)
            {
                if (tag.ToString() == excelTag)
                {
                    
                }
            }
        }


        void TryAddTransitionStateToState(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleAnimState toAnimState)
        {
            if (fromAnimatorState.tag == "idle")
            {
                if (toAnimatorState.tag == "walk" || toAnimatorState.tag == "run")
                {
                    AddTransitionAnyToWalk(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState);
                }
                else if (toAnimatorState.tag == "roll" || toAnimatorState.tag == "dodge")
                {
                    AddTransitionAnyToDodge(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState);
                }
                else if (toAnimatorState.tag == "action")
                {
                    AddTransitionAnyToAction(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleActionState);
                }
                else if (toAnimatorState.tag == "hurt" || toAnimatorState.tag == "die")
                {
                    AddTransitionAnyStateToHurtState(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleHurtState);
                }
            }
            else if (fromAnimatorState.tag == "walk")
            {
                if (toAnimatorState.tag == "idle")
                {
                    AddTransitionAnyMoveToIdle(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState);
                }
                else if (toAnimatorState.tag == "run")
                {
                    AddTransitionAnyToWalk(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState);
                }
                else if (toAnimatorState.tag == "roll" || toAnimatorState.tag == "dodge")
                {
                    AddTransitionAnyToDodge(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState);
                }
                else if (toAnimatorState.tag == "action")
                {
                    AddTransitionAnyToAction(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleActionState);
                }
                else if (toAnimatorState.tag == "hurt" || toAnimatorState.tag == "die")
                {
                    AddTransitionAnyStateToHurtState(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleHurtState);
                }
            }
            else if (fromAnimatorState.tag == "run")
            {
                if (toAnimatorState.tag == "idle")
                {
                    AddTransitionAnyMoveToIdle(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState);
                }
                else if (toAnimatorState.tag == "walk")
                {
                    AddTransitionRunToWalk(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState);
                }
                else if (toAnimatorState.tag == "roll")
                {
                    AddTransitionAnyToDodge(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState);
                }
                else if (toAnimatorState.tag == "attack")
                {
                    AddTransitionAnyToAction(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleActionState);
                }
                else if (toAnimatorState.tag == "hurt" || toAnimatorState.tag == "die")
                {
                    AddTransitionAnyStateToHurtState(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleHurtState);
                }
                
            }
            else if (fromAnimatorState.tag == "roll")
            {
                if (toAnimatorState.tag == "run" || toAnimatorState.tag == "walk")
                {
                    AddTransitionDodgeToWalk(fromAnimatorState, toAnimatorState, fromAnimState ,toAnimState);
                }
                else if (toAnimatorState.tag == "idle")
                {
                    AddTransitionDodgeToIdle(fromAnimatorState, toAnimatorState, fromAnimState ,toAnimState);
                }
                else if (toAnimatorState.tag == "hurt" || toAnimatorState.tag == "die")
                {
                    AddTransitionAnyStateToHurtState(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleHurtState);
                }
            }
            else if (fromAnimatorState.tag == "action")
            {
                if (toAnimatorState.tag == "action")
                {
                    AddTransitionAnyToAction(fromAnimatorState, toAnimatorState, fromAnimState as RoleActionState, toAnimState as RoleActionState);
                }
                else if (toAnimatorState.tag == "dodge")
                {
                    AddTransitionAnyToDodge(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleActionState);
                }
                else if (toAnimatorState.tag == "run" || toAnimatorState.tag == "walk")
                {
                    AddTransitionAnyActionToWalk(fromAnimatorState, toAnimatorState, fromAnimState as RoleActionState, toAnimState as RoleActionState);
                }
                else if (toAnimatorState.tag == "idle")
                {
                    AddTransitionActionToIdle(fromAnimatorState, toAnimatorState, fromAnimState as RoleActionState,toAnimState as RoleActionState);
                }
                else if (toAnimatorState.tag == "hurt" || toAnimatorState.tag == "die")
                {
                    AddTransitionAnyStateToHurtState(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleHurtState);
                }
            }
            else if (fromAnimatorState.tag == "attack")
            {
                if (toAnimatorState.tag == "action")
                {
                    AddTransitionAnyToAction(fromAnimatorState, toAnimatorState, fromAnimState as RoleActionState, toAnimState as RoleActionState);
                }
                else if(toAnimatorState.tag == "run" || toAnimatorState.tag == "walk")
                {
                    AddTransitionAnyActionToWalk(fromAnimatorState, toAnimatorState, fromAnimState as RoleActionState, toAnimState);
                }
                else if(toAnimatorState.tag == "dodge")
                {
                    AddTransitionAnyToDodge(fromAnimatorState, toAnimatorState, fromAnimState as RoleActionState, toAnimState);
                }
                else if(toAnimatorState.tag == "idle")
                {
                    AddTransitionActionToIdle(fromAnimatorState, toAnimatorState, fromAnimState as RoleActionState, toAnimState);
                }
                else if (toAnimatorState.tag == "hurt" || toAnimatorState.tag == "die")
                {
                    AddTransitionAnyStateToHurtState(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleHurtState);
                }
                else { }
            }
            else if (fromAnimatorState.tag == "hurt")
            {
                if (toAnimatorState.tag == "action")
                {
                    AddTransitionHurtToAction(fromAnimatorState, toAnimatorState, fromAnimState as RoleHurtState, toAnimState as RoleActionState);
                }
                else if (toAnimatorState.tag == "walk" || toAnimatorState.tag == "run")
                {
                    AddTransitionDodgeToWalk(fromAnimatorState, toAnimatorState, fromAnimState as RoleHurtState, toAnimState);
                }
                else if (toAnimatorState.tag == "dodge")
                {
                    AddTransitionDodgeToDodge(fromAnimatorState, toAnimatorState, fromAnimState as RoleHurtState, toAnimState);
                }
                else if (toAnimatorState.tag == "idle")
                {
                    AddTransitionDodgeToWalk(fromAnimatorState, toAnimatorState, fromAnimState as RoleHurtState, toAnimState);
                }
                else if (toAnimatorState.tag == "hurt" || toAnimatorState.tag == "die")
                {
                    AddTransitionAnyStateToHurtState(fromAnimatorState, toAnimatorState, fromAnimState, toAnimState as RoleHurtState);
                }
            }
            else if (fromAnimatorState.tag == "jumpingAttack")
            {
                if (toAnimatorState.tag == "jumping")
                {
                    
                }
                else if(toAnimatorState.tag == "landing")
                {
                    
                }
                else { }
            }
            else if (fromAnimatorState.tag == "jump")
            {
                
            }
            else if (fromAnimatorState.tag == "die") { }
            else
            {
                Debug.Print(fromAnimatorState.tag + "is nonentity!");
            }
        }

        void AddTransitionAnyStateToHurtState(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleHurtState toAnimState)
        {
            if (toAnimatorState.tag == "die")
            {
                AnimatorTransition transition = new AnimatorTransition
                {
                    destinationAnimName = toAnimatorState.name,
                    ignoreSourceAnimTags = new List<string> {"die"},
                    hasExitTime = false,
                    conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = "isDead",
                            threshold = 0.1f
                        }
                    }
                };
                fromAnimatorState.transitions.Add(transition);
            }
            else if (toAnimatorState.tag == "hurt")
            {
                AnimatorTransition transition = new AnimatorTransition();;
                transition.destinationAnimName = toAnimatorState.name + "_1";
                transition.ignoreSourceAnimTags = new List<string> {"die", "hurt"};
                transition.hasExitTime = false;
            
                transition.conditions = new List<AnimatorTransitionCondition>()
                {
                    new AnimatorTransitionCondition()
                    {
                        animatorConditionMode = toAnimState.IsFront ? AnimatorConditionMode.Greater : AnimatorConditionMode.Less,
                        parameter = "hurt_direction",
                        threshold = 0
                    },
                    new AnimatorTransitionCondition()
                    {
                        animatorConditionMode = AnimatorConditionMode.If,
                        parameter = "hurt",
                        threshold = 0.1f
                    },
                    new AnimatorTransitionCondition()
                    {
                        animatorConditionMode = AnimatorConditionMode.Greater,
                        parameter = "hurt_force_value",
                        threshold = toAnimState.ForceValue
                    }
                };
                fromAnimatorState.transitions.Add(transition);
            }
            else { }
        }

        void AddTransitionDodgeToIdle(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleAnimState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = true;
            transition.exitTime = 1;
            fromAnimatorState.transitions.Add(transition);
        }
        
        void AddTransitionAnyMoveToIdle(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleAnimState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = false;
            transition.exitTime = 1;
            transition.conditions = new List<AnimatorTransitionCondition>()
            {
                        
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.Less,
                    parameter = "MoveSpeed",
                    threshold = 0.1f
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = "isGround",
                    threshold = 0.1f
                }
            };
            fromAnimatorState.transitions.Add(transition);
        }
        
        void AddTransitionRunToWalk(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleAnimState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = false;
            transition.exitTime = 1;
            transition.conditions = new List<AnimatorTransitionCondition>()
            {
                        
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.Less,
                    parameter = "MoveSpeed",
                    threshold = fromAnimatorState.MinSpeed
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = "isGround",
                    threshold = 0.1f
                }
            };
            fromAnimatorState.transitions.Add(transition);
        }
        
        void AddTransitionActionToIdle(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleActionState fromAnimState, RoleAnimState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = true;
            transition.exitTime = GetExitTime(fromAnimState.AnimName04_End, fromAnimState.AnimName04_totalFrame);
            fromAnimatorState.transitions.Add(transition);
        }

        // => walk,run
        void AddTransitionAnyToWalk(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleAnimState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = false;
            transition.exitTime = 1;
            transition.conditions = new List<AnimatorTransitionCondition>()
            {
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.Greater,
                    parameter = "MoveSpeed",
                    threshold = toAnimatorState.MinSpeed
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = "isGround",
                    threshold = 0.1f
                }
            };
            fromAnimatorState.transitions.Add(transition);
        }
        
        void AddTransitionAnyActionToWalk(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleActionState fromAnimState, RoleAnimState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = false;
            transition.exitTime = 1;
            transition.conditions = new List<AnimatorTransitionCondition>()
            {
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.Greater,
                    parameter = "move_state",
                    threshold = 0
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = "isGround",
                    threshold = 0.1f
                }
            };
            fromAnimatorState.transitions.Add(transition);
        }
        
        void AddTransitionDodgeToWalk(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleAnimState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = true;
            transition.exitTime = 1;
            transition.conditions = new List<AnimatorTransitionCondition>()
            {
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.Greater,
                    parameter = "MoveSpeed",
                    threshold = toAnimatorState.MinSpeed
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = "isGround",
                    threshold = 0.1f
                }
            };
            fromAnimatorState.transitions.Add(transition);
        }
        
//      void AddTransition
        void AddTransitionAnyToAction(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleActionState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = false;
            transition.offset = GetOffset(toAnimState.AnimName01_Start,toAnimState.AnimName01_totalFrame);
            transition.conditions = new List<AnimatorTransitionCondition>()
            {
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = "isGround",
                    threshold = 0.1f
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = toAnimState.StateName + "_begin",
                    threshold = 0.1f
                }
            };
            fromAnimatorState.transitions.Add(transition);
        }
        
        void AddTransitionHurtToAction(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleActionState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = true;
            transition.exitTime = 1;
            transition.offset = GetOffset(toAnimState.AnimName01_Start,toAnimState.AnimName01_totalFrame);
            transition.conditions = new List<AnimatorTransitionCondition>()
            {
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = "isGround",
                    threshold = 0.1f
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = toAnimState.StateName + "_begin",
                    threshold = 0.1f
                }
            };
            fromAnimatorState.transitions.Add(transition);
        }

        void AddTransitionAnyToDodge(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleAnimState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = false;
            transition.exitTime = 1;
            transition.conditions = new List<AnimatorTransitionCondition>()
            {
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.Equals,
                    parameter = "action_roll",
                    threshold = 1
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = "isGround",
                    threshold = 0.1f
                }
            };
            fromAnimatorState.transitions.Add(transition);
        }
        
        void AddTransitionDodgeToDodge(AnimatorState fromAnimatorState, AnimatorState toAnimatorState, RoleAnimState fromAnimState, RoleAnimState toAnimState)
        {
            AnimatorTransition transition = new AnimatorTransition();
            transition.destinationAnimName = toAnimatorState.name;
            transition.hasExitTime = true;
            transition.exitTime = 1;
            transition.conditions = new List<AnimatorTransitionCondition>()
            {
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.Equals,
                    parameter = "action_roll",
                    threshold = 1
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = "isGround",
                    threshold = 0.1f
                }
            };
            fromAnimatorState.transitions.Add(transition);
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        void addToWalkConditionByTag(AnimatorState fromState, RoleAnimState toState, string toTag)
        {
            AnimatorTransition transition = new AnimatorTransition();
            switch (toTag)
            {
                case "idle":
                    transition.destinationAnimName = toState.StateName;
                    transition.hasExitTime = true;
                    transition.exitTime = 1;
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Less,
                            parameter = "MoveSpeed",
                            threshold = 0.1f
                        },
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = "isGround",
                            threshold = 0.1f
                        }
                    };
                    fromState.transitions.Add(transition);
                    break;
                case "roll":
                    transition.destinationAnimName = toState.StateName;
                    transition.hasExitTime = false;
                    transition.offset = GetOffset(toState.Start, toState.End);
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = "isGround",
                            threshold = 0.1f
                        },
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Equals,
                            parameter = "action_roll",
                            threshold = 1
                        }
                    };
                    fromState.transitions.Add(transition);
                    break;
                case "walk":
                    transition.destinationAnimName = toState.StateName;
                    transition.hasExitTime = false;
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = "isGround",
                            threshold = 0.1f
                        },
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Greater,
                            parameter = "MoveSpeed",
                            threshold = 0.1f
                        },
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Less,
                            parameter = "MoveSpeed",
                            threshold = fromState.MinSpeed
                        }
                    };
                    fromState.transitions.Add(transition);  
                    break;
                case "run":
                    transition.destinationAnimName = toState.StateName;
                    transition.hasExitTime = false;
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = "isGround",
                            threshold = 0.1f
                        },
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Greater,
                            parameter = "MoveSpeed",
                            threshold = fromState.MinSpeed
                        }
                    };
                    fromState.transitions.Add(transition);  
                    break;
                default:
                    Debug.Print("\"" + toTag + "\" is nonexistent!");
                    break;
            }
        }

        void addToAction1ConditionByTag(AnimatorState fromState, RoleActionState toState, string toTag)
        {
            AnimatorTransition transition = new AnimatorTransition();
            switch (toTag)
            {
                case "attack":
                case "action":
                    transition.destinationAnimName = toState.StateName+"_1";
                    transition.hasExitTime = false;
                    transition.offset = GetOffset(toState.AnimName01_Start,toState.AnimName01_totalFrame);
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = "isGround",
                            threshold = 0.1f
                        },
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = toState.StateName + "_begin",
                            threshold = 0.1f
                        }
                    };
                    fromState.transitions.Add(transition);
                    break;
                case "jumpingAttack":
                    transition.destinationAnimName = toState.StateName+"_1";
                    transition.hasExitTime = false;
                    transition.offset = GetOffset(toState.AnimName01_Start,toState.AnimName01_totalFrame);
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.IfNot,
                            parameter = "isGround",
                            threshold = 0.1f
                        },
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = toState.StateName + "_begin",
                            threshold = 0.1f
                        }
                    };
                    fromState.transitions.Add(transition);
                    break;
                default:
                    Debug.Print("\"" + toTag + "\" is nonexistent!");
                    break;
            }
        }

        void addToJumpConditionByTag(AnimatorState fromState, RoleActionState toState, string toTag)
        {
            AnimatorTransition transition = new AnimatorTransition();
            switch (toTag)
            {
                case "landing":
                    transition.destinationAnimName = toState.StateName + "_1";
                    transition.hasExitTime = false;
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Equals,
                            parameter = "action_jump",
                            threshold = 1
                        }
                    };
                    fromState.transitions.Add(transition);  
                    break;
                default:
                    Debug.Print("\"" + toTag + "\" is nonexistent!");
                    break;
            }
        }

        void addJumpAction4To(AnimatorState state)
        {
            // N => jumpUp
                foreach (var jumpUpState in roleAnimator.jump)
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = jumpUpState.Value.StateName + "_3";
                    transition.hasExitTime = false;
                    transition.offset = GetOffset(jumpUpState.Value.AnimName01_Start, jumpUpState.Value.AnimName01_End);
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Greater,
                            parameter = "speedy",
                            threshold = 0
                        },
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.If,
                            parameter = "isGround",
                            threshold = 0.1f
                        },
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Equals,
                            parameter = "action_jump",
                            threshold = 1
                        }
                    };

                    state.transitions.Add(transition);
                }
                
            // N => walk,run,dodge
                foreach (var walkState in roleAnimator.walk)
                {
                    addToWalkConditionByTag(state, walkState.Value, walkState.Value.Tag);
                }

            // N => idle
                foreach (var idleState in roleAnimator.idle)
                {
                    addToWalkConditionByTag(state, idleState.Value, idleState.Value.Tag);
                }
                
            // N => jumpDown
                foreach (var jumpDownState in roleAnimator.jump)
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = jumpDownState.Value.StateName + "_3";
                    transition.hasExitTime = false;
                    transition.offset = GetOffset(jumpDownState.Value.AnimName03_Start, jumpDownState.Value.AnimName03_End);
                    transition.conditions = new List<AnimatorTransitionCondition>()
                    {
                        new AnimatorTransitionCondition()
                        {
                            animatorConditionMode = AnimatorConditionMode.Less,
                            parameter = "speedy",
                            threshold = 0
                        }
                    };

                    state.transitions.Add(transition);
                }
                
            // N => jumpDownLanding
                foreach (var jumpDownLandingState in roleAnimator.jump)
                {
                    AnimatorTransition transition = new AnimatorTransition();
                    transition.destinationAnimName = jumpDownLandingState.Value.StateName + "_4";
                    transition.hasExitTime = false;
                    transition.offset = GetOffset(jumpDownLandingState.Value.AnimName04_Start, jumpDownLandingState.Value.AnimName04_End);

                    state.transitions.Add(transition);
                }
        }

        
        
        void addHurtConditions(KeyValuePair<string,RoleHurtState> itemHurt, AnimatorState startAnimName)
        {
            AnimatorTransition transition = new AnimatorTransition();;
            transition.destinationAnimName = itemHurt.Value.StateName + "_1";
            transition.ignoreSourceAnimTags = new List<string> {"die", "hurt"};
            transition.hasExitTime = false;
            
            transition.conditions = new List<AnimatorTransitionCondition>()
            {
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = itemHurt.Value.IsFront ? AnimatorConditionMode.Greater : AnimatorConditionMode.Less,
                    parameter = "hurt_direction",
                    threshold = 0
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.If,
                    parameter = "hurt",
                    threshold = 0.1f
                },
                new AnimatorTransitionCondition()
                {
                    animatorConditionMode = AnimatorConditionMode.Greater,
                    parameter = "hurt_force_value",
                    threshold = itemHurt.Value.ForceValue
                }
            };
            startAnimName.transitions.Add(transition);
        }

        
        private float GetPlaySpeed(int startFrame,int endFrame,float playetime)
        {
            float frameTime = ((float)endFrame - startFrame)/30;
            float speed = frameTime / playetime;
            return speed <= 0.01f ? 0.02f: speed;
        }
        
        private float GetOffset(int nextStartFrame,int nextTotalFrame) //下个动画从何时播放
        {
            float offset = (float)nextStartFrame / nextTotalFrame;
            return  offset>= 0 ? offset : 0;
        }
        private float GetExitTime(int endFrame,int lastFrame)//当前动画退出时间
        {
            float time = (float)endFrame / lastFrame;
            return time<= 0.01 ? 0.02f : time ;
        }
        
        private float FrameStartToTime(int startFrame)
        {
            float time = (float)startFrame / 30f;
            return time<= 0.01 ? 0.02f : time ;
        }

        private float FrameEndToTime(int endFrame)
        {
            float time = (float)endFrame / 30f;
            return time<= 0.01 ? 0.02f : time ;
        }
    }
}