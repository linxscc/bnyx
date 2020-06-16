using UnityEngine;


namespace Tang
{
    public class CameraTest : MonoBehaviour
    {
        public Material Material;

        private Camera mainCamera;

        private Camera _lightCamera;
        private Camera _shadowCamera;

        private Light.ShadowCamera shadowCamera;

        private RenderTexture _lightRendererTexture;
        private RenderTexture _shadowRendererTexture;

        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();


        private void Start()
        {
            InitCamera();
            InitRendererTexture();

            valueMonitorPool.AddMonitor(() => { return Screen.width + Screen.width * 10000; }, (int from, int to) =>
           {
               InitRendererTexture();
           });
        }

        void InitCamera()
        {
            mainCamera = GetComponent<Camera>();

            shadowCamera = gameObject.GetChild("ShadowCamera").GetComponent<Light.ShadowCamera>();

            _lightCamera = gameObject.GetChild("LightCamera").GetComponent<Camera>();
            _shadowCamera = gameObject.GetChild("ShadowCamera").GetComponent<Camera>();

            _lightCamera.orthographicSize = mainCamera.orthographicSize;
            _shadowCamera.orthographicSize = mainCamera.orthographicSize;

            _lightCamera.depth = mainCamera.depth - 1;
            _shadowCamera.depth = mainCamera.depth - 1;
        }

        void InitRendererTexture()
        {
            _lightRendererTexture = new RenderTexture(Screen.width, Screen.height, 32);
            _shadowRendererTexture = new RenderTexture(Screen.width, Screen.height, 32);

            _lightCamera.targetTexture = _lightRendererTexture;
            _shadowCamera.targetTexture = _shadowRendererTexture;
        }

        private void OnEnable()
        {
            mainCamera = GetComponent<Camera>();
        }

        RenderTexture lastRenderTexture;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Material.SetTexture("_LightTex", _lightRendererTexture);
            Material.SetTexture("_ShadowTex", _shadowRendererTexture);
            //Material.SetTexture("_ShadowTex", shadowCamera.MainRenderTexture);
            Graphics.Blit(source, destination, Material, 0);
        }

        //protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
        //{
        //    Material.SetTexture("_LightTex", _lightRendererTexture);
        //    //Material.SetTexture("_ShadowTex", _shadowRendererTexture);
        //    Material.SetTexture("_ShadowTex", shadowCamera.MainRenderTexture);
        //    Graphics.Blit(lastRenderTexture, destination, Material, 0);


        //    lastRenderTexture = source;
        //}

        private void Update()
        {
            valueMonitorPool.Update();
        }

        private void LateUpdate()
        {
            valueMonitorPool.Update();
        }

        //protected override string GetShaderName()
        //{
        //    return "Tang/Camera";
        //}
    }
}