using Spine.Unity;
using Tang.Animation;
using UnityEngine;


namespace Tang
{
    public class FragileFloorController : PlacementController
    {
        public float breakingDelay = 1f;
        public float recoverDelay = 1f;
        
        private GameObject colliderParent;
        private GameObject triggerParent;

        private Renderer mainAnimRenderer;
        private Material mainAnimMaterial;

        private FSM fsm;

        private AnimatorStateTransmit _animatorStateTransmit;
        
        public override object Data {
            get
            {
                return this;
            }
            set
            {
                var newThis = value as FragileFloorController;
                this.breakingDelay = newThis.breakingDelay;
                this.recoverDelay = newThis.recoverDelay;
            }
            
        }

        public override void Start()
        {
            base.Start();

            colliderParent = gameObject.GetChild("Collider", true);
            triggerParent = gameObject.GetChild("Trigger", true);

            mainAnimMaterial = MainAnimator.GetComponent<Renderer>().material;
            mainAnimRenderer = MainAnimator.GetComponent<Renderer>();

            _animatorStateTransmit = gameObject.GetComponentInChildren<AnimatorStateTransmit>();
            _animatorStateTransmit.OnStateEvents += OnStateEvent;
            
            InitFSM();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            _animatorStateTransmit.OnStateEvents -= OnStateEvent;
        }

        public void InitFSM()
        {
//            fsm = new FSM();
//
//            fsm.SetCurrStateName("0");
//            
//            float elapseTime = 0;
//            
//            fsm.AddState("0", () =>
//            {
//                State = 0;
//                colliderParent.SetActive(true);
//                triggerParent.SetActive(true);
//
//                MainSkeletonAnimator.SetFillVertexType(FillVertexType.Ground);
//
////                mainAnimRenderer.material.renderQueue = 2000;
////                mainAnimRenderer.material.SetFloat("_ZWrite", 1);l
//
////                MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
////                mainAnimRenderer.GetPropertyBlock(materialPropertyBlock);
////                materialPropertyBlock.SetFloat("_ZWrite", 1);
////                mainAnimRenderer.SetPropertyBlock(materialPropertyBlock);
//                
//                
////                elapseTime = 0;
//            }, () =>
//            {
////                elapseTime += Time.deltaTime;
////                if (elapseTime >= recoverDelay)
////                {
////                    fsm.SendEvent("AllTo1");
////                }
//            });
//            
//            fsm.AddState("1",
//                () =>
//                {
//                    elapseTime = 0;
//                    State = 1;
//                },
//                () =>
//                {
//                    elapseTime += Time.deltaTime;
//                    if (elapseTime >= breakingDelay)
//                    {
//                        fsm.SendEvent("AllTo2");
//                    }
//                }, 
//                () =>
//                {
//                
//                });
//            
//            fsm.AddState("2", () =>
//                {
//                    elapseTime = 0;
//                    State = 2;
//                    colliderParent.SetActive(false);
//                    triggerParent.SetActive(false);
//                    
//                    MainSkeletonAnimator.SetFillVertexType(FillVertexType.Center);
//                    
////                    mainAnimRenderer.material.renderQueue = 3000;
//////                    mainAnimRenderer.material.SetFloat("_ZWrite", 0);
////                    
////                    MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
////                    mainAnimRenderer.GetPropertyBlock(materialPropertyBlock);
////                    materialPropertyBlock.SetFloat("_ZWrite", 0);
////                    mainAnimRenderer.SetPropertyBlock(materialPropertyBlock);
//                },
//                () =>
//                {
//                    elapseTime += Time.deltaTime;
//                    if (elapseTime >= recoverDelay)
//                    {
//                        fsm.SendEvent("AllTo0");
//                    }
//                });
//            
//            fsm.AddState("3",
//                () =>
//                {
//                    elapseTime = 0;
//                    State = 3;
//                },
//                () =>
//                {
//                    elapseTime += Time.deltaTime;
//                    if (elapseTime >= breakingDelay)
//                    {
//                        fsm.SendEvent("AllTo2");
//                    }
//                }, 
//                () =>
//                {
//                
//                });
//
//            fsm.AddEvent("1To1-0", "1", "1-0");
        }

        public override void OnTriggerIn(TriggerEvent evt)
        {
            TriggerController triggerController = evt.selfTriggerController.GetFirstKeepingTriggerController();
            if (triggerController.ITriggerDelegate as RoleController && triggerController.ITriggerDelegate.gameObject .name.IndexOf("Player") >= 0)
            {
                State = 1;

//                fsm.SendEvent("1To1-0");
//                fsm.SendEvent("AllTo1");

//                if (HasCoroutine("FragileFloorController-breaking") == false)
//                {
//                    DelayFunc("FragileFloorController-breaking", () =>
//                    {
//                        State = 0; 
//                        colliderParent.SetActive(false);
//                    }, breakingDelay);
//                }
            }
        }

        public override void OnTriggerOut(TriggerEvent evt)
        {
//            TriggerController triggerController = evt.selfTriggerController.GetFirstKeepingTriggerController();
//            if (triggerController != null && triggerController.ITriggerDelegate as RoleController)
//            {
//            }
//            else
//            {
//                if (HasCoroutine("FragileFloorController-recover") == false)
//                {
//                    DelayFunc("FragileFloorController-recover", () =>
//                    {
//                        State = 1;
//                        colliderParent.SetActive(true);
//                    }, recoverDelay);
//                }
//            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if(fsm!=null)
                fsm.Update();
            
//            valueMonitorPool.Update();
        }

        public void OnStateEvent(string stateName, AnimatorStateEventType eventType, Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex, float time)
        {
            
            Debug.Log("StateName = " + stateName + ", eventType = " + eventType);
            if (stateName == "Normal")
            {
                if (eventType == AnimatorStateEventType.OnStateEnter)
                {
                    colliderParent.SetActive(true);
                    triggerParent.SetActive(true);
                    
                    MainSkeletonAnimator.SetFillVertexType(FillVertexType.Ground);
                }
            }
            else if (stateName == "BreakingHold" && time >= breakingDelay)
            {
                switch (eventType)
                {
                    case AnimatorStateEventType.OnStateUpdate:
                        if (stateInfo.normalizedTime >= 1)
                        {
                            State = 2;
                        }
                        break;
                }
            }
            else if (stateName == "Breaking")
            {
                if (eventType == AnimatorStateEventType.OnStateEnter)
                {
                    colliderParent.SetActive(false);
                    triggerParent.SetActive(false);
                    
                    MainSkeletonAnimator.SetFillVertexType(FillVertexType.Center);
                }
                else if (eventType == AnimatorStateEventType.OnStateUpdate)
                {
                    if (stateInfo.normalizedTime >= 1)
                    {
                        State = 3;
                    }
                }
            }
            else if(stateName == "Breaked")
            {
                if (eventType == AnimatorStateEventType.OnStateUpdate)
                {
                    if (stateInfo.normalizedTime >= 1 && time >= recoverDelay)
                    {
                        State = 4;
                    }
                }
            }
            else if (stateName == "Recovering")
            {
                if (eventType == AnimatorStateEventType.OnStateUpdate)
                {
                    if (stateInfo.normalizedTime >= 1)
                    {
                        State = 0;
                    }
                }
            }
        }
    }
}