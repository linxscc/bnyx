using System;
using Tang.Animation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Tang
{
    public class DogHumanQTE : MonoBehaviour, IQte
    {
        public event Action OnStart;
        public event Action OnUpdate;
        public event Action OnBreak;
        public event Action OnEnd;

        public RoleController selfController;
        public RoleController targetController;

        private QteState _qteState = QteState.QteBegin;
        
        private int StruggleTimes = 0;
        
        FunctionPool _functionPool = new FunctionPool();

        private bool isLockTarget = false;
        
        private void OnEnable()
        {
            _functionPool.AddFunc(() =>
            {
                if (selfController != null)
                {
                    AnimatorStateTransmit selfAnimatorStateTransmit =
                        selfController.GetComponentInChildren<AnimatorStateTransmit>();
                    selfAnimatorStateTransmit.OnStateEvents += OnSelfStateEvent;
                    return true;
                }
                return false;
            });
            
            _functionPool.AddFunc(() =>
            {
                if (targetController != null)
                {
                    AnimatorStateTransmit targetAnimatorStateTransmit =
                        targetController.GetComponentInChildren<AnimatorStateTransmit>();
                    targetAnimatorStateTransmit.OnStateEvents += OnTargetStateEvent;
                    return true;
                }

                return false;
            });
            
            _functionPool.CallFuncs();
        }

        private void OnDisable()
        {
            _functionPool.AddFunc(() =>
            {
                if (selfController != null)
                {
                    AnimatorStateTransmit selfAnimatorStateTransmit =
                        selfController.GetComponentInChildren<AnimatorStateTransmit>();
                    selfAnimatorStateTransmit.OnStateEvents -= OnSelfStateEvent;
                    return true;
                }
                return false;
            });
            
            _functionPool.AddFunc(() =>
            {
                if (targetController != null)
                {
                    AnimatorStateTransmit targetAnimatorStateTransmit =
                        targetController.GetComponentInChildren<AnimatorStateTransmit>();
                    targetAnimatorStateTransmit.OnStateEvents -= OnTargetStateEvent;
                    return true;
                }

                return false;
            });
            
            _functionPool.CallFuncs();
        }

        public void OnSelfStateEvent(string stateName, AnimatorStateEventType eventType, Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex, float time)
        {
//            if (_qteState == QteState.QteRunning)
//            {
//                if (stateName == "Qte_ToHuman_Success")
//                {
//                    if (eventType == AnimatorStateEventType.OnStateExit)
//                    {
//                        Success();
//                    }   
//                }
//            }
        }

        public void OnTargetStateEvent(string stateName, AnimatorStateEventType eventType, Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex, float time)
        {
            if (_qteState == QteState.QteRunning)
            {
                if (eventType == AnimatorStateEventType.OnStateEnter)
                {
                    if (stateName == "Qte_DogToHuman_Struggle")
                    {
                        StruggleTimes++;
                        if (StruggleTimes >= 1)
                        {
                            _qteState = QteState.QteFailure;
                            targetController.RoleAnimator.SetTrigger("qte_struggleSuccess");
                        }
                    }
                } 
            }
        }

        public void Init(RoleController a, RoleController b)
        {
            selfController = a;
            targetController = b;
            
            Debug.Assert(selfController != null && targetController != null);
            Debug.Log(selfController.name + " qte " + targetController.name);
        }

        public void Begin()
        {
            selfController.RoleAnimator.SetInteger("qteState", 1);
            targetController.RoleAnimator.SetTrigger("qte_dog_to_human");

            selfController.IsQte = true;
            targetController.IsQte = true;

            // 给qte目标设置朝向 add by TangJian 2019/3/20 21:47
            targetController.SetDirectionInt(-selfController.GetDirectionInt());
            
            LockTarget();

            _qteState = QteState.QteRunning;
        }

        public void Hit()
        {
            targetController.RoleAnimator.SetBool("hurt", true);

            float atk = selfController.RoleData.Atk;
            targetController.QteHurt(new Vector3(selfController.GetDirectionInt(), 0, 0), new Vector3(-1, 0, 0), atk);
        }

        public void LastHit()
        {
            targetController.RoleAnimator.SetTrigger("qte_struggleFailure");
            
            float atk = selfController.RoleData.Atk;
            targetController.QteHurt(new Vector3(selfController.GetDirectionInt(), 0, 0), new Vector3(-1, 0, 0), atk);
        }

        public void Success()
        {
            _qteState = QteState.QteSuccess;
            Tools.Destroy(this);
        }

        public void Failure()
        {
            selfController.RoleAnimator.SetInteger("qteState", -1);
            Tools.Destroy(this);
        }

        public void LockTarget()
        {
            Vector3 worldPostion;
            if (selfController.SkeletonAnimator.TryGetBonePos("qte", out worldPostion))
            {
                worldPostion.z += 0.1f;
                targetController.transform.position = worldPostion;
            }
        }
        
        private void Update()
        {
            _functionPool.CallFuncs();

            if (_qteState == QteState.QteRunning)
            {
                LockTarget();
            }
        }

        private void OnDestroy()
        {
            selfController.IsQte = false;
            targetController.IsQte = false;
            
            targetController.RoleAnimator.ResetTrigger("qte_struggleSuccess");
            targetController.RoleAnimator.ResetTrigger("qte_struggleFailure");
            
            selfController.RoleAnimator.SetInteger("qteState", 0);
        }
    }
}