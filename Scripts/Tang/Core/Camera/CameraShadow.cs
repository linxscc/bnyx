using UnityEngine;





namespace Tang
{
    public class CameraShadow : MonoBehaviour// Colorful.BaseEffect
    {
        static private CameraShadow instance;
        static public CameraShadow Instance
        {
            get
            {
                return instance;
            }
        }

        RenderTexture renderTexture;

        public RenderTexture GetTexture()
        {
            if (renderTexture != null)
                return renderTexture;
            return null;
        }

        private void Awake()
        {
            instance = this;
        }

        // private void OnEnable()
        // {
        //     GetComponent<Camera>().depth = 10;
        // }

        protected void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            renderTexture = source;
        }
    }
}