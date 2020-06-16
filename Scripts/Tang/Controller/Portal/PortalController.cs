using UnityEngine;

namespace Tang
{
    public class PortalController : SceneObjectController, ITriggerDelegate, IInteractable
    {
        public PortalData PortalData;
        public GameObject EntryRoot;
        public GameObject ExitRoot;

        Transform closedColliderObject;
        Transform openedColliderObject;

        private FSM fsm;

        private Animator _animator;

        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();

        public bool forceClose = false;
        
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        int state = 0;
        public int State
        {
            set
            {
                state = value;
            }

            get
            {
                return state;
            }
        }

        public bool CanInteract()
        {
            return true;
        }

        public void Interact()
        {
            State = ++State % 2;
        }

        public void Open()
        {
            State = 1;
        }

        public void Close()
        {
            State = 0;
        }

        public bool IsOpen
        {
            get
            {
                if (_animator == null)
                    return true;

                return _animator.IsCurrName("Opened");
            }
        }

        public string OpenKey
        {
            get
            {
                return PortalData.door != null ? PortalData.door.key : null;
            }
        }

        public override void Awake()
        {
            base.Awake();

            InitCollider();
            InitAnimator();
            InitTrigger();
            //InitAnimator();
            //InitValueMonitor();
        }

        public override void Start()
        {
            base.Start();

//            InitValueMonitor();
        }

        void InitTrigger()
        {
            // 创建碰撞区域 add by TangJian 2017/09/26 16:58:04
            gameObject.Recursive((GameObject go, int depth) =>
            {
                if (go.tag == "PortalEntry")
                {
                    EntryRoot = go;
                }
                else if (go.tag == "PortalExit")
                {
                    ExitRoot = go;
                }
            }, 1, 99);

            Debug.Assert(EntryRoot != null, name + ":" + "entryRoot == null");
            Debug.Assert(ExitRoot != null, name + ":" + "exitRoot == null");

            // 设置exitRoot层
            {
                MeshCollider meshCollider = EntryRoot.AddComponentUnique<MeshCollider>();
                meshCollider.convex = true;
                meshCollider.isTrigger = true;

                EntryRoot.layer = LayerMask.NameToLayer("All");
                EntryRoot.tag = "Portal";

                TriggerController triggerController = EntryRoot.AddComponentUnique<TriggerController>();
            }
        }

        void InitCollider()
        {
            openedColliderObject = transform.Find("Collider/Opened");
            closedColliderObject = transform.Find("Collider/Closed");
        }

        void InitAnimator()
        {
            _animator = GetComponentInChildren<Animator>();

        }

        void InitValueMonitor()
        {
            valueMonitorPool.AddMonitor(() => { return _animator == null ? 0 : _animator.GetCurrAnimNameHash(); }, (int from, int to) =>
            {
                if (_animator.IsCurrName("Opened"))
                {
                    SetColliderState(1);
                    SceneEventManager.Instance.ObjStateChange(name, 1);
                }
                else if (_animator.IsCurrName("Closed"))
                {
                    SetColliderState(0);
                    SceneEventManager.Instance.ObjStateChange(name, 0);
                }
            }, true);
        }



        public bool GetCurrentAnim()
        {
            return _animator.IsCurrName("Opened");
        }
        
        void UpdateAnimator()
        {
            if (_animator != null)
                _animator.SetInteger("State", forceClose ? 0 : state);
        }

        public void SetColliderState(int state)
        {
            if (closedColliderObject == null || openedColliderObject == null)
                return;

            if (state == 0)
            {
                closedColliderObject.gameObject.SetActive(true);
                openedColliderObject.gameObject.SetActive(false);
            }
            else if (state == 1)
            {
                openedColliderObject.gameObject.SetActive(true);
                closedColliderObject.gameObject.SetActive(false);
            }
        }

        public void OnTriggerIn(TriggerEvent evt)
        {
            if (evt.selfTriggerController.name == "Entry")
            {
                if (IsOpen)
                {
                    DelayFunc("Portal", () =>
                    {
                        var toSceneId = PortalData.toSceneId;
                        var toScenePortalId = PortalData.toPortalId;

                        if (toSceneId == null || toScenePortalId == null || toSceneId == "" || toScenePortalId == "")
                        {
                            return;
                        }

                        var gameObject = evt.otherTriggerController.ITriggerDelegate.GetGameObject();
                        if (gameObject)
                        {
                            var roleController = gameObject.GetComponent<RoleController>();
                            if (roleController)
                            {
                                if (gameObject.name == "Player1" || gameObject.name == "Player2")// 只有玩家能进入传送门 add by TangJian 2019/1/9 15:13
                                {
                                    if (SceneManager.Instance.RoleEnterSceneByPortal(roleController,
                                        PortalData.sceneId, PortalData.id, toSceneId, toScenePortalId))
                                    {
                                        if (gameObject.name == "Player1")
                                        {
                                            SceneManager.Instance.SwitchToScene(toSceneId);
                                        }
                                    }
                                }
                            }
                        }
                    }, 0);
                }
            }
        }

        public void OnTriggerOut(TriggerEvent evt)
        {

        }

        public void OnTriggerKeep(TriggerEvent evt)
        {

        }

        public bool OnEvent(Event evt)
        {
            Debug.Log("与门交互!!!");

            return true;
        }

        public override void Update()
        {
            base.Update();

            UpdateAnimator();

            if (_animator != null)
                valueMonitorPool.Update();
        }
    }
}