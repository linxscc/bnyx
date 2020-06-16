using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    [CustomEditor(typeof(SceneComponent), true)]
    [CanEditMultipleObjects]
    public class SceneComponentEditor : UnityEditor.Editor
    {
        SceneComponent terrainComponent;

        void OnEnable()
        {
            foreach (SceneComponent terrainComponent in targets)
            {
                terrainComponent.color = new Color(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f));

                //// 设置资源路径 add by TangJian 2018/03/13 15:37:42
                //{
                //    if (PrefabUtility.GetPrefabType(terrainComponent.gameObject) == PrefabType.Prefab)
                //    {
                //        if (terrainComponent.filePath == null || terrainComponent.filePath == "")
                //            terrainComponent.filePath = Tools.GetPrefabPath(terrainComponent.gameObject) ?? terrainComponent.filePath;
                //    }

                //    terrainComponent.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(terrainComponent.filePath);
                //}

                if (terrainComponent.GetComponent<Cask>())
                {
                    Cask cask = terrainComponent.GetComponent<Cask>();
                    Bounds bounds = cask.gameObject.GetRendererBounds(2, 99);
                    cask.data.width = bounds.size.x;
                    cask.data.height = bounds.size.y;
                }
            }
            AssetDatabase.SaveAssets();
        }

        public override void OnInspectorGUI()
        {
            foreach (SceneComponent _sceneComponent in targets)
            {
                if (PrefabUtility.GetPrefabType(_sceneComponent.gameObject) == PrefabType.PrefabInstance)
                {
                    _sceneComponent.MoveByGridPos(Vector3.zero);
                }
            }

            if (MyGUI.Button("更新组件"))
            {
                EditorCoroutineSequenceRunner.AddCoroutineIfNot("更新组件", targets.MyToList<SceneComponent>().Editor_UpdateToPrefab());
            }

            if (MyGUI.Button("生成2d图片"))
            {
                foreach (SceneComponent item in targets)
                {
                    if (item is SceneSlopeComponent)
                        (item as SceneSlopeComponent).CreateTexture2D();

                    if (item is BackWall)
                        (item as BackWall).CreateTexture2D();

                    if (item is SceneSideWallComponent)
                        (item as SceneSideWallComponent).CreateTexture2D();

                    if (item is SceneCenterWallComponent)
                        (item as SceneCenterWallComponent).CreateTexture2D();

                    if (item is SceneSlopeWallComponent)
                        (item as SceneSlopeWallComponent).CreateTexture2D();

                    if (item is SceneTriangleWallComponent)
                        (item as SceneTriangleWallComponent).CreateTexture2D();

                    if (item is Ground)
                        (item as Ground).CreateTexture2D();
                }
            }

            SceneComponent sceneComponent = (target as SceneComponent);
            GUILayout.Label("ComponentType: " + sceneComponent.ComponentType);

            if (PrefabUtility.GetPrefabType(sceneComponent.gameObject) == PrefabType.Prefab)
            {
                if (MyGUI.Button("生成纹理"))
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        SceneComponent _sceneComponent = targets[i] as SceneComponent;
                        if (_sceneComponent != null)
                        {
                            EditorUtility.DisplayProgressBar("生成纹理", "生成纹理: " + _sceneComponent.name, (float)i / targets.Length);

                            Tools.ModifyPrefab(_sceneComponent.gameObject, (GameObject go) =>
                            {
                                go.GetComponent<SceneComponent>().Editor_Build();
                            });
                        }
                    }

                    EditorUtility.ClearProgressBar();
                }
            }

            DrawDefaultInspector();

            // 刷新组件的翻转 add by TangJian 2018/10/31 14:25
            foreach (SceneComponent _sceneComponent in targets)
            {
                _sceneComponent.RefreshFlip();
            }
        }
    }
}