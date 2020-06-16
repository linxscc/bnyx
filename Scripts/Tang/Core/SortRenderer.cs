using UnityEngine;



namespace Tang
{
    public class SortRenderer : MonoBehaviour
    {
        public static string SortRendererPosName = "SortRendererPos";

        Renderer mainRenderer;
        Renderer MainRenderer
        {
            get
            {
                if (mainRenderer == null)
                    mainRenderer = GetComponent<Renderer>();
                return mainRenderer;
            }

            set
            {
                mainRenderer = value;
            }
        }

        public Vector3 Pos = new Vector3();
        public Vector3 Offset = new Vector3();

        Transform mainTransform;
        public Transform MainTransform
        {
            get
            {
                if (mainTransform == null)
                {
                    mainTransform = transform.Find(SortRendererPosName);
                    if (mainTransform == null)
                        mainTransform = transform.Find("SortRenderPos");

                    if (mainTransform == null)
                    {
                        mainTransform = transform;
                    }
                }
                return mainTransform;
            }

            set
            {
                mainTransform = value;
            }
        }

        public void OnEnable()
        {
            Init();

            SortRendererManager.Instance.AddSortRenderer(this);
        }

        public void OnDisable()
        {
            SortRendererManager.Instance.RemoveSortRenderer(this);
        }

        public void Update()
        {
            UpdateState();
        }

        // 初始化 add by TangJian 2019/1/17 15:23
        public void Init()
        {
            MainRenderer = GetComponent<Renderer>();

            MainTransform = transform.Find(SortRendererPosName);
            if (MainTransform == null)
            {
                MainTransform = transform;
            }
        }

        // 绘制辅助线 add by TangJian 2019/1/17 15:23
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(MainTransform.position, 0.1f);
        }

        // 更新状态 add by TangJian 2019/1/17 15:23
        public void UpdateState()
        {
            Pos.z = MainTransform.position.z + Offset.z;
        }

        // 获取Z add by TangJian 2019/1/17 15:23
        public float PosZ
        {
            get
            {
                return Pos.z;
            }
        }

        // 设置渲染顺序 add by TangJian 2019/1/17 15:23
        public void SetZorder(int zorder)
        {
//            if (mainRenderer != null)
//                mainRenderer.sortingOrder = zorder;
        }


        // 设置SortRendererPos add by TangJian 2019/1/17 15:26
        public void SetSortRendererPos(Vector3 pos)
        {
            var go = gameObject.GetChild(SortRendererPosName, true);
            go.transform.position = pos;

            MainTransform = go.transform;
        }
    }
}