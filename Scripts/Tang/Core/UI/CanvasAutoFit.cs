using UnityEngine;
using UnityEngine.UI;





namespace Tang
{
    public class CanvasAutoFit : MyMonoBehaviour
    {
        Canvas canvas;
        CanvasScaler canvasScaler;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            canvas = GetComponent<Canvas>();
            canvasScaler = GetComponent<CanvasScaler>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            if (canvas != null && canvasScaler != null)
            {
                // canvasScaler.referenceResolution = new Vector2(1920, 1080);
                // canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                // canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

                // // var x = canvasScaler.referenceResolution - 

                // canvasScaler.matchWidthOrHeight = Screen.height / (Screen.height + Screen.width);

                float a = (float)Screen.height / Screen.width;

                // canvasScaler.matchWidthOrHeight = 1.0f / (a + 1.0f);

                if (canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y >= Screen.width / Screen.height)
                {

                }

                // Debug.Log("canvasScaler.uiScaleMode = " + (int)canvasScaler.uiScaleMode);
                // Debug.Log("canvasScaler.screenMatchMode = " + (int)canvasScaler.screenMatchMode);
                // Debug.Log("canvasScaler.referenceResolution = " + canvasScaler.referenceResolution);
            }
        }
    }
}