using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{

    [CustomEditor(typeof(SceneComponentGroup))]
    public class SceneComponentGroupEditor : UnityEditor.Editor
    {
        SceneComponentGroup sceneComponentGroup;

        private void OnEnable()
        {
            sceneComponentGroup = target as SceneComponentGroup;
        }

        public override void OnInspectorGUI()
        {
            if (sceneComponentGroup == null)
                return;

            if (MyGUI.Button("更新场景组件到最新的预制体"))
            {
                List<SceneComponent> sceneComponentList = sceneComponentGroup.gameObject.GetComponentList<SceneComponent>();
                EditorCoroutineSequenceRunner.AddCoroutine(sceneComponentList.Editor_UpdateToPrefab(), 9999);
            }

            if (MyGUI.Button("更新组件排序"))
            {
                List<SceneComponent> sceneComponentList = sceneComponentGroup.gameObject.GetComponentList<SceneComponent>();
                EditorCoroutineSequenceRunner.AddCoroutine(sceneComponentList.Editor_UpdateRendererShader(), 9999);
                EditorCoroutineSequenceRunner.AddCoroutine(sceneComponentList.Editor_UpdateSortRenderer(), 9999);
            }

            if (GUILayout.Button("给所有渲染节点添加阴影"))
            {
                List<Renderer> renderers = sceneComponentGroup.gameObject.GetComponentList<Renderer>();
                foreach (var renderer in renderers)
                {
                    Light.ShadowRenderer shadowRenderer = renderer.gameObject.AddComponentUnique<Light.ShadowRenderer>();
                    //if (renderer.sharedMaterial.mainTexture != null)
                    //{
                    //    string texturePath = AssetDatabase.GetAssetPath(renderer.sharedMaterial.mainTexture);
                    //    string normalPath = texturePath.Insert(texturePath.LastIndexOf('.'), "_normal");
                    //    Debug.Log("texturePath =" + texturePath);
                    //    Debug.Log("normalPath =" + normalPath);
                    //    Texture2D normalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);
                    //    if (normalTexture != null)
                    //    {
                    //        shadowRenderer.ShadowTexture = normalTexture;
                    //    }
                    //}
                }
            }

            if (GUILayout.Button("保存所有场景预制体"))
            {
                List<SceneController> sceneControllers = sceneComponentGroup.gameObject.GetComponentList<SceneController>();
                EditorCoroutineSequenceRunner.AddCoroutine(sceneControllers.Editor_ApplySceneController(), 9999);
            }

            if (GUILayout.Button("更新组件到最新->保存场景"))
            {
                List<SceneController> sceneControllers = sceneComponentGroup.gameObject.GetComponentList<SceneController>();
                foreach (var sceneController in sceneControllers)
                {
                    List<SceneComponent> sceneComponentList = sceneController.gameObject.GetComponentList<SceneComponent>();
                    EditorCoroutineSequenceRunner.AddCoroutine(sceneComponentList.Editor_UpdateToPrefab(), 9999);
                }

                EditorCoroutineSequenceRunner.AddCoroutine(sceneControllers.Editor_InitSceneObjectControllers(), 9999);
                EditorCoroutineSequenceRunner.AddCoroutine(sceneControllers.Editor_ApplySceneController(), 9999);
            }

            if (MyGUI.Button("设置场景地形组件的SortRenderer"))
            {
                List<SceneComponent> sceneComponentList = sceneComponentGroup.gameObject.GetComponentList<SceneComponent>();

                sceneComponentList.Editor_UpdateSideWallRendererSort();
                //Editor_UpdatePlacementsRendererSort
            }
        }
    }
}