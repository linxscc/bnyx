using UnityEngine;

namespace Tang
{
    public class MainManager : MonoBehaviour
    {
        static MainManager s_mainManager;
        public static MainManager GetInstance()
        {
            if (s_mainManager == null)
            {
                GameObject mainManagerObject = GameObject.FindGameObjectWithTag("ManagerObject");
                if (mainManagerObject != null)
                {
                    mainManagerObject.tag = "ManagerObject";
                    s_mainManager = mainManagerObject.GetComponent<MainManager>();
                }
                else
                {
                    Debug.LogWarning("mainManagerObject对象已经被销毁");
                }
            }
            return s_mainManager;
        }

        public T GetManager<T>()
        {
            return s_mainManager.gameObject.GetComponent<T>();
        }
    }
}

