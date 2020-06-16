using UnityEngine;





namespace Tang
{
    public class CameraAutoFit : MyMonoBehaviour
    {
        private float scale = 1;
        private Camera mainCamera;

        public float Scale { get { return scale; } set { scale = value; } }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            mainCamera = gameObject.GetComponent<Camera>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        public override void Update()
        {
            // int ManualWidth = 1920;
            // int ManualHeight = 1080;
            // int manualHeight;
            // if (System.Convert.ToSingle(Screen.height) / Screen.width > System.Convert.ToSingle(ManualHeight) / ManualWidth)
            //     manualHeight = Mathf.RoundToInt(System.Convert.ToSingle(ManualWidth) / Screen.width * Screen.height);
            // else
            //     manualHeight = ManualHeight;
            // Camera camera = GetComponent<Camera>();
            // float scale = System.Convert.ToSingle(manualHeight / 1080f);
            // camera.fieldOfView *= scale;

            if (mainCamera != null)
            {
                mainCamera.orthographicSize = (Screen.height / 2) / 100.0f;
            }
        }
    }
}