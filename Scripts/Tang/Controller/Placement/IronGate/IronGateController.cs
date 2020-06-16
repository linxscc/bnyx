using UnityEngine;
using Spine;
using Spine.Unity;

namespace Tang
{
    public class IronGateController : MyMonoBehaviour, ITriggerDelegate, I2dSort
    {
        enum IronGateType
        {
            Left,
            Right,
            Center
        }
        [SerializeField] IronGateType ironGateType = IronGateType.Center;
        ValueMonitor<IronGateType> ironGateTypeMonitor;
        [SerializeField] SkeletonAnimation skeletonAnimation;
        Collider body;
        Collider standGround;

        SimpleFSM fsm;
        string state = "opened";

        string openAnimName;
        string closeAnimName;


        void Start()
        {
            init();
        }

        void init()
        {

            // 初始化Collider add by TangJian 2017/10/31 15:20:16
            gameObject.GetChild("Collider").Recursive((GameObject go, int depth) =>
                                  {
                                      if (go.name == "Body")
                                      {
                                          body = go.GetComponent<Collider>();
                                      }
                                      else if (go.name == "StandGround")
                                      {
                                          standGround = go.GetComponent<Collider>();
                                      }
                                  }, 2, 4);

            // 初始化动画 add by TangJian 2017/10/31 15:20:25
            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

            switch (ironGateType)
            {
                case IronGateType.Center:
                    openAnimName = "attack";
                    closeAnimName = "attack2";
                    break;
                case IronGateType.Left:
                case IronGateType.Right:
                    openAnimName = "attack-";
                    closeAnimName = "attack-2";
                    break;
                default:
                    Debug.LogError("没有门类型" + ironGateType);
                    break;
            }

            skeletonAnimation.state.Complete += (TrackEntry trackEntry) =>
            {
                if (trackEntry.Animation.Name == openAnimName)
                {
                    body.enabled = true;
                }
                else if (trackEntry.Animation.Name == closeAnimName)
                {
                    body.enabled = false;
                }
            };

            skeletonAnimation.state.SetAnimation(0, openAnimName, false);

            // 初始化状态机 add by TangJian 2017/10/31 15:20:33
            fsm = new SimpleFSM();

            state = "open";

            fsm.SetState("opened");
            fsm.AddEvent("opened", "closed", () =>
            {
                return state == "close";
            }, () =>
            {
                skeletonAnimation.state.SetAnimation(0, closeAnimName, false);
            });

            fsm.AddEvent("closed", "opened", () =>
            {
                return state == "open";
            }, () =>
            {
                skeletonAnimation.state.SetAnimation(0, openAnimName, false);
            });
            fsm.Begin();



            ironGateTypeMonitor = new ValueMonitor<IronGateType>(() => { return ironGateType; }, (IronGateType from, IronGateType to) =>
            {
                reInit();
            });
        }

        void reInit()
        {
            switch (ironGateType)
            {
                case IronGateType.Center:
                    openAnimName = "attack";
                    closeAnimName = "attack2";
                    break;
                case IronGateType.Left:
                case IronGateType.Right:
                    openAnimName = "attack-";
                    closeAnimName = "attack-2";
                    break;
                default:
                    Debug.LogError("没有门类型" + ironGateType);
                    break;
            }

            skeletonAnimation.state.SetAnimation(0, openAnimName, false);

            // 初始化状态机 add by TangJian 2017/10/31 15:20:33
            fsm = new SimpleFSM();

            state = "open";

            fsm.SetState("opened");
            fsm.AddEvent("opened", "closed", () =>
            {
                return state == "close";
            }, () =>
            {
                skeletonAnimation.state.SetAnimation(0, closeAnimName, false);
            });

            fsm.AddEvent("closed", "opened", () =>
            {
                return state == "open";
            }, () =>
            {
                skeletonAnimation.state.SetAnimation(0, openAnimName, false);
            });
            fsm.Begin();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            fsm.Update();
            ironGateTypeMonitor.Update();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool OnEvent(Event evt)
        {
            if (evt.Type == EventType.Interact)
            {
                if (state == "open")
                {
                    state = "close";
                }
                else if (state == "close")
                {
                    state = "open";
                }
            }
            return true;
        }

        public void OnTriggerIn(TriggerEvent evt)
        { }
        public void OnTriggerOut(TriggerEvent evt)
        { }
        public void OnTriggerKeep(TriggerEvent evt)
        { }

        public int GetDirection()
        {
            return 1;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void SetOrder(int order)
        {
            skeletonAnimation.gameObject.GetComponent<Renderer>().sortingOrder = order;
        }
    }
}