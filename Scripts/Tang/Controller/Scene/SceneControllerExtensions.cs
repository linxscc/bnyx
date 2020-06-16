using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tang
{
    public static class SceneControllerExtensions 
    {
#if UNITY_EDITOR
        
        // 保存场景 add by TangJian 2019/1/8 12:42
        public static IEnumerator Editor_ApplySceneController(this List<SceneController> target)
        {
            List<SceneController> sceneControllers = target;
            for (int i = 0; i < sceneControllers.Count; i++)
            {
                SceneController sceneController = sceneControllers[i];
                EditorUtility.DisplayProgressBar("保存场景预制体", sceneController.name, (float)(i + 1) / sceneControllers.Count);
                Tools.ModifyPrefab(sceneController.gameObject);
                yield return null;
            }
            EditorUtility.ClearProgressBar();
        }
        
        // 为场景中的每一个SceneObjectController设置当前场景引用 add by TangJian 2019/1/8 12:42
        public static IEnumerator Editor_InitSceneObjectControllers(this List<SceneController> target)
        {
            List<SceneController> sceneControllers = target;
            for (int i = 0; i < sceneControllers.Count; i++)
            {
                SceneController sceneController = sceneControllers[i];
                EditorUtility.DisplayProgressBar("保存场景预制体", sceneController.name, (float)(i + 1) / sceneControllers.Count);

                List<SceneObjectController> sceneObjectControllers = sceneController.gameObject.GetComponentList<SceneObjectController>(1, 9999);

                foreach (var sceneObjectController in sceneObjectControllers)
                {
                    sceneObjectController.CurrSceneController = sceneController;
                }
                
                yield return null;
            }
            EditorUtility.ClearProgressBar();
        }
        
        // 更新每个场景预制体到最新 add by TangJian 2019/1/8 17:44
//        public static IEnumerator Editor_UpdateSceneControllers(this List<SceneController> target)
//        {
//            
//        }
#endif
    }
}