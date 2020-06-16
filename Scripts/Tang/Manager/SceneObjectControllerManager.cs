using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Tang
{
    public class SceneObjectControllerManager : MonoBehaviour
    {
        private static SceneObjectControllerManager instance;
        public static SceneObjectControllerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<SceneObjectControllerManager>();
                }
                return instance;
            }
        }

#if UNITY_EDITOR
        [SerializeField]List<SceneObjectController> SceneObjectControllers = new List<SceneObjectController>();
#endif
        
        Dictionary<string, List<SceneObjectController>> SceneObjectControllerDic = new Dictionary<string, List<SceneObjectController>>();
        
        public void Add(string id, SceneObjectController SceneObjectController)
        {
            AddSceneObjectController(id, SceneObjectController);
        }

        public void Remove(string id, SceneObjectController SceneObjectController)
        {
            RemoveSceneObjectController(id, SceneObjectController);
        }
        
        public List<SceneObjectController> Get(string id)
        {
            List<SceneObjectController> SceneObjectControllers_;
            if (SceneObjectControllerDic.TryGetValue(id, out SceneObjectControllers_))
            {
                return SceneObjectControllers_;
            }

            return null;
        }

        public SceneObjectController GetFirst(string id)
        {
            List<SceneObjectController> sceneObjectControllers = Get(id);
            if (sceneObjectControllers !=null && sceneObjectControllers.Count > 0)
            {
                return sceneObjectControllers[0];
            }

            return null;
        }

        public void AddSceneObjectController(string id, SceneObjectController SceneObjectController)
        {
            List<SceneObjectController> SceneObjectControllers_;
            if (SceneObjectControllerDic.TryGetValue(id, out SceneObjectControllers_))
            {
                SceneObjectControllers_.Add(SceneObjectController);
            }
            else
            {
                SceneObjectControllerDic.Add(id, new List<SceneObjectController>(){SceneObjectController});
            }

#if UNITY_EDITOR
            SceneObjectControllers.Add(SceneObjectController);
#endif
        }

        public void RemoveSceneObjectController(string id, SceneObjectController SceneObjectController)
        {
            List<SceneObjectController> SceneObjectControllers_;
            if (SceneObjectControllerDic.TryGetValue(id, out SceneObjectControllers_))
            {
                SceneObjectControllers_.Remove(SceneObjectController);
                if (SceneObjectControllers_.Count <= 0)
                {
                    SceneObjectControllerDic.Remove(id);
                }
            }
            
#if UNITY_EDITOR
            SceneObjectControllers.Remove(SceneObjectController);
#endif
        }
    }
}