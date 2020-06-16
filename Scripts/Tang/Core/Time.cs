namespace Tang
{
    public class Time
    {
        public static float time => UnityEngine.Time.time;
        public static float deltaTime
        {
            get
            {
//                return 0.01666667F;
                return UnityEngine.Time.deltaTime;
            }
        }

        public static float fixedTime => UnityEngine.Time.fixedTime;
        public static float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;
        public static float timeScale => UnityEngine.Time.timeScale;
        public static float realtimeSinceStartup => UnityEngine.Time.realtimeSinceStartup;
        
        
    }
}