using System.Collections.Generic;
using Tang.FrameEvent;
using UnityEngine;
using UnityEngine.AI;

namespace Tang
{
    public class FenceCylinderDoorController : PlacementController, IInteractable
    {
        GameObject closedColliderObject;
        
        List<Animator> animatorList = new List<Animator>();

        public void Open()
        {
            State = 1;
        }

        public void Close()
        {
            State = 0;
        }
        public void OpenPortal()
        {
            if (IsOpen) // 门已经开了, 就不处理 add by TangJian 2018/10/17 20:05
            {
                Close();
            }
            else // 不需要钥匙, 直接开门 add by TangJian 2018/10/17 20:08
            {
                Open();
            }
        }
        public bool IsOpen
        {
            get
            {
                if (animatorList.Count==0)
                    return true;

                return animatorList[0].IsCurrName("Opened");
            }
        }
        public override void InitAnimator()
        {
            animatorList.Clear();
            Animator[] animators=gameObject.GetChild("Renderer").GetComponentsInChildren<Animator>();
            animatorList.AddArray(animators);
        }
        public override void Start()
        {
            base.Start();
            InitValueMonitor();
            initclosedColliderObject();
            State=1;
        }
        void InitValueMonitor()
        {
            valueMonitorPool.AddMonitor(() => animatorList[0] == null ? 0 : animatorList[0].GetCurrAnimNameHash(), (int from, int to) =>
            {
                if (animatorList[0].IsCurrName("Opened"))
                {
                    SetColliderState(1);
                }
                else if (animatorList[0].IsCurrName("Closed"))
                {
                    SetColliderState(0);
                }
            });
        }
        void initclosedColliderObject()
        {
            closedColliderObject = gameObject.GetChild("Collider").GetChild("boxc");
        }
        public override int State
        {
            set
            {
                if (animatorList.Count != 0)
                {
                    Debug.Log("设置 State = " + value);
                    foreach(var item in animatorList)
                    {
                        item.SetInteger("State", value);
                    }
                }

            }
            get
            {
                return animatorList[0].GetInteger("State");
            }
        }
        void SetColliderState(int state)
        {
            if (closedColliderObject == null)
                return;

            if (state == 0)
            {
                closedColliderObject.gameObject.SetActive(false);
                //openedColliderObject.gameObject.SetActive(false);
            }
            else if (state == 1)
            {
                //openedColliderObject.gameObject.SetActive(true);
                closedColliderObject.gameObject.SetActive(true);
            }
        }

       

        public bool CanInteract()
        {
            return true;
        }
        
        public void Interact()
        {
            State = (++State) % 2;
        }
    }
}

