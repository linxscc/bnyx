using UnityEngine;
using UnityEngine.AI;

namespace Tang
{
    public class FenceDoorController : PlacementController, IInteractable
    {
        GameObject closedColliderObject;
        BoxCollider boxCollider;
        MeshRenderer meshRenderer;
        NavMeshObstacle navMeshObstacle;
        Mesh mesh;
        //GameObject openedColliderObject;
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
                if (MainAnimator == null)
                    return true;

                return MainAnimator.IsCurrName("Opened");
            }
        }
        public override void Start()
        {
            base.Start();
            InitValueMonitor();
            initclosedColliderObject();
        }
        void InitValueMonitor()
        {
            valueMonitorPool.AddMonitor(() => { return MainAnimator == null ? 0 : MainAnimator.GetCurrAnimNameHash(); }, (int from, int to) =>
            {
                if (MainAnimator.IsCurrName("Opened"))
                {
                    SetColliderState(1);
                }
                else if (MainAnimator.IsCurrName("Closed"))
                {
                    SetColliderState(0);
                }
            });
        }
        void initclosedColliderObject()
        {
            closedColliderObject = gameObject.GetChild("Collider").GetChild("boxc");
            boxCollider = closedColliderObject.GetComponent<BoxCollider>();
            meshRenderer = MainAnimator.GetComponent<MeshRenderer>();
            mesh = MainAnimator.GetComponent<MeshFilter>().sharedMesh;
            navMeshObstacle = closedColliderObject.GetComponent<NavMeshObstacle>();
        }
        public override int State
        {
            set
            {
                if (MainAnimator != null)
                {
                    Debug.Log("设置 State = " + value);
                    MainAnimator.SetInteger("State", value);
                }

            }
            get
            {
                return MainAnimator.GetInteger("State");
            }
        }
        void SetColliderState(int state)
        {
            if (closedColliderObject == null)
                return;

            if (state == 0)
            {
                closedColliderObject.gameObject.SetActive(true);
                //openedColliderObject.gameObject.SetActive(false);
            }
            else if (state == 1)
            {
                //openedColliderObject.gameObject.SetActive(true);
                closedColliderObject.gameObject.SetActive(true);
            }
        }

        private void LateUpdate()
        {
            //boxCollider.gameObject.transform.position = meshRenderer.bounds.center;
            //boxCollider.center = Vector3.zero;
            //if (boxCollider.gameObject.transform.localRotation.y != 0)
            //{
            //    boxCollider.gameObject.transform.position = meshRenderer.bounds.center;
            //    boxCollider.center = Vector3.zero;
            //    float afa = 1/Mathf.Cos((boxCollider.gameObject.transform.localRotation.y > 0 ? 1 : -1) * Mathf.Atan(2));
            //    Vector3 size = new Vector3(meshRenderer.bounds.size.x * afa, meshRenderer.bounds.size.y, meshRenderer.bounds.size.z);
            //    boxCollider.size = size + new Vector3(0, 0, 1);

            //}
            //else
            {
                closedColliderObject.transform.position = meshRenderer.bounds.center;
                if (navMeshObstacle != null)
                {
                    navMeshObstacle.center = Vector3.zero;
                }
                boxCollider.center = Vector3.zero;
                boxCollider.size = meshRenderer.bounds.size + new Vector3(0, 0, 0.1f);
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

