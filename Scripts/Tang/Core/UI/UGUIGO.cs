using UnityEngine;
using UnityEngine.UI;





namespace Tang
{
    public class UGUIGO : MyMonoBehaviour
    {
        [SerializeField] private GameObject showObject;
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private GameObject cameraObject;
        [SerializeField] private GameObject rawImageObject;

        [SerializeField] private string cameraObjectName = "MyCamera";
        [SerializeField] private string rawImageObjectName = "MyRawImage";
        [SerializeField] private Vector2 offset = Vector2.zero;
        [SerializeField] private Vector2 scale = new Vector2(1, 1);
        public Vector2 Offset { get { return offset; } set { offset = value; } }
        public Vector2 Scale { get { return scale; } set { scale = value; } }

        public void SetShowObject(GameObject showObject)
        {
            if (this.showObject != null)
                Tools.Destroy(this.showObject);
            this.showObject = showObject;
        }

        void clear()
        {
            if (renderTexture != null)
            {
                Tools.Destroy(renderTexture);
            }
            if (cameraObject != null)
            {
                Tools.Destroy(cameraObject);
            }
            if (rawImageObject != null)
            {
                Tools.Destroy(rawImageObject);
            }
        }

        public void Show()
        {
            clear();

            // 渲染纹理 add by TangJian 2017/10/24 21:33:22
            renderTexture = new RenderTexture(256, 256, 32);

            // 摄像机对象 add by TangJian 2017/10/24 21:33:29
            GameObject cameraObject = new GameObject();
            cameraObject.name = cameraObjectName;

            showObject.transform.parent = transform;
            cameraObject.transform.parent = transform;

            // 移除显示对象下面的所有刚体 add by TangJian 2017/10/24 21:31:37
            showObject.RecursiveComponent((Rigidbody rb, int depth) =>
            {
                Tools.Destroy(rb);
            }, 1, 99);

            CameraTest cameraAutoFit = cameraObject.AddComponent<CameraTest>();

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 1;
            camera.targetTexture = renderTexture;

            GameObject rawImageObject = new GameObject();
            rawImageObject.name = rawImageObjectName;
            rawImageObject.transform.parent = transform;

            RawImage rawImage = rawImageObject.AddComponent<RawImage>();
            rawImage.texture = renderTexture;

            rawImageObject.transform.localPosition = Vector3.zero;

            var farawayPosition = Tools.getOnlyId() * new Vector3(999, 999, 999);
            cameraObject.transform.localPosition = Vector3.zero + farawayPosition;
            showObject.transform.localPosition = new Vector3(Offset.x * scale.x, Offset.y * scale.y, 1) + farawayPosition;
            showObject.transform.localScale = scale;

            // delayFunc(() =>
            // {
            //     Debug.Log("delayFunc test");
            //     camera.targetTexture = null;
            //     Tools.Destroy(cameraObject);
            //     Tools.Destroy(showObject);
            // }
            // , 0);
        }

        void Start()
        {

        }


    }
}