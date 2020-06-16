using UnityEngine;





namespace Tang
{
    public class CameraLight : MonoBehaviour// Colorful.BaseEffect
    {
        private Camera mainCamera;
        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();

//        private void Start()
//        {
//            camera = GetComponent<Camera>();
//
//            camera.targetTexture.width = Screen.width;
//            camera.targetTexture.height = Screen.height;
//            
//            valueMonitorPool.AddMonitor(() => { return Screen.width; }, (int from, int to) =>
//                {
//                    camera.targetTexture.width = to;
//                });
//            
//            valueMonitorPool.AddMonitor(() => { return Screen.height; }, (int from, int to) =>
//            {
//                camera.targetTexture.height = to;
//            });
//        }
//
//        private void Update()
//        {
//            valueMonitorPool.Update();
//        }
    }
}