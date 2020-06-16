using UnityEngine;

namespace Tang
{
    public class ShadowController : MyMonoBehaviour
    {
        public float shadowSize = 1;
        protected float nearClipPlane = -0.5f;
        protected float farClipPlane = 6;
        protected Projector projector;
        protected Ray ray;
        protected int layerMask;

        [SerializeField] GameObject shadowGameObject;
        GameObject ShadowGameObject
        {
            get
            {
                if (shadowGameObject == null)
                    shadowGameObject = gameObject.GetChild("Shadow");
                return shadowGameObject;
            }
        }

        void Start()
        {
            // projector = GetComponent<Projector>();
            // projector.nearClipPlane = nearClipPlane;
            // projector.farClipPlane = farClipPlane;
            // projector.aspectRatio = 1;
            // projector.orthographic = true;
            // projector.enabled = false;
            // projector.ignoreLayers = ~LayerMask.GetMask(new string[] { "Ground" });

            // ~(1 << 10) 打开除了第10之外的层。
            // ~(1 << 0) 打开所有的层。
            // (1 << 10) | (1 << 8) 打开第10和第8的层。
            // layerMask = 1 << (LayerMask.NameToLayer("cube"));
            // layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Terrain"));
            // layerMask = LayerMask.GetMask(new string[] { "Ground", "Terrain" });

            layerMask = LayerMask.GetMask(new string[] { "SceneComponent", "Ground" });

            ray = new Ray();
            ray.direction = Vector3.down;

            // transform.localPosition = new Vector3(transform.localPosition.x, 1.0f, transform.localPosition.z);
        }

        public float ShadowSize
        {
            set
            {
                if (ShadowGameObject != null)
                    ShadowGameObject.transform.localScale = new Vector3(1, 1, 1) * value;
            }

            get
            {
                if (ShadowGameObject != null)
                    return ShadowGameObject.transform.localScale.x;
                return 1;
            }
        }

        public override void Update()
        {
            base.Update();
            
            ray.origin = transform.position;

            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, farClipPlane, layerMask))
            {
                if (raycastHit.distance >= nearClipPlane && raycastHit.distance <= farClipPlane)
                {
                    raycastHit.distance -= 1.0f;
                    raycastHit.distance = Mathf.Max(raycastHit.distance, 0.0f);

                    // projector.enabled = true;
                    if (raycastHit.distance >= 0)
                    {
                        ShadowSize = Mathf.Max(shadowSize * ((farClipPlane - raycastHit.distance) / farClipPlane), 0.6f);
                        // projector.orthographicSize = Mathf.Max(shadowSize * ((farClipPlane - raycastHit.distance) / farClipPlane), 0.6f);
                    }
                }
                else
                {
                    // projector.enabled = false;
                }
            }
        }
    }
}