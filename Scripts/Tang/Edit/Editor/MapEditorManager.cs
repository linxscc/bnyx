using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;


namespace Tang.Editor
{
    [InitializeOnLoad]
    public class MapEditorManager
    {
        public static SceneController CurrSceneController;
        public static int CurrSelectedSceneIndex = -1;
        public static bool IsEditing = false;
        public static bool NeedRepaint = true;

        static MapEditorManager()
        {
            EditorCoroutineParallelRunner.AddCoroutine("WalkSceneController", WalkSceneController());
            EditorCoroutineParallelRunner.AddCoroutine("UpdateInputState", UpdateInputState());

            InitSelectionChange();
        }

        static IEnumerator UpdateInputState()
        {
            while (true)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    Debug.Log(KeyCode.LeftControl.ToString() + "按下");
                }
                yield return null;
            }
        }

        public static List<SceneController> GetCurrSceneControllerList()
        {
            GameObject editObject = GameObject.FindWithTag("Edit");
            if (editObject != null)
            {
                List<SceneController> sceneControllers = editObject.GetComponentList<SceneController>();
                string[] sceneOptions = new string[sceneControllers.Count];
                for (int i = 0; i < sceneControllers.Count; i++)
                {
                    sceneOptions[i] = sceneControllers[i].name;
                }
                return sceneControllers;
            }
            return new List<SceneController>();
        }

        public static void Repaint()
        {
            NeedRepaint = true;
        }

        static void InitSelectionChange()
        {
            Selection.selectionChanged = () =>
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    object obj = Selection.objects[i];
                    GameObject gameObject = obj as GameObject;
                    if (gameObject)
                    {
                        PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
                        switch (prefabType)
                        {
                            case PrefabType.DisconnectedPrefabInstance:
                            case PrefabType.PrefabInstance:
                                // 处理预制体实例 add by TangJian 2018/11/8 17:53
                                {
                                    GameObject prefabParent = AssetDatabase.LoadAssetAtPath<GameObject>(Tools.GetPrefabPath(gameObject));

                                    if (prefabParent.GetComponent<SceneComponent>() != null && gameObject.GetComponent<SceneComponent>() != null)
                                    {
                                        SceneComponent sceneComponent = gameObject.GetComponent<SceneComponent>();

                                        // 自动选择当前编辑的场景 add by TangJian 2018/11/27 21:48
                                        SceneController sceneController = null;
                                        {
                                            sceneComponent.transform.RecursiveParent((Transform parent, int depth) =>
                                            {
                                                if (sceneController == null && parent != null)
                                                {
                                                    sceneController = parent.GetComponent<SceneController>();
                                                }
                                            }, 0, 99);

                                            if (sceneController != null)
                                            {
                                                var currSceneControllerList = GetCurrSceneControllerList();
                                                CurrSelectedSceneIndex = currSceneControllerList.FindIndex((SceneController sc) =>
                                                {
                                                    return sc.name == sceneController.name;
                                                });
                                                MapEditorManager.CurrSceneController = sceneController;
                                            }
                                            Repaint();
                                        }

                                        if (MapEditorManager.IsEditing && CurrSceneController != null)
                                        {
                                            Debug.Log("加入组件 " + sceneComponent.name + " 到场景" + CurrSceneController.name);

                                            Transform parent = CurrSceneController.transform.FindAndAutoAdd(sceneComponent.PathInScene);
                                            sceneComponent.transform.parent = parent;
                                        }

                                        Debug.Log("为场景组件 " + gameObject.name + " 设置预制体路径");
                                        sceneComponent.Editor_UpdatePath();
                                        //sceneComponent.Editor_UpdateRendererSort();
                                        //sceneComponent.Editor_UpdateRendererShader();
                                    }
                                    else if (prefabParent.GetComponent<SceneObjectController>() != null && gameObject.GetComponent<SceneObjectController>())
                                    {
                                        SceneObjectController sceneObjectController = gameObject.GetComponent<SceneObjectController>();

                                        SceneController sceneController = null;
                                        gameObject.transform.RecursiveParent((Transform parent, int depth) =>
                                        {
                                            if (sceneController == null && parent != null)
                                            {
                                                sceneController = parent.GetComponent<SceneController>();
                                            }
                                        }, 0, 99);

                                        if (sceneController != null)
                                        {
                                            sceneObjectController.CurrSceneController = sceneController;
                                        }
                                    }
                                }
                                break;
                            default:
                                Debug.Log("prefabType = " + prefabType);
                                break;
                        }
                    }
                }
            };
        }

        static IEnumerator WalkSceneController()
        {
            while (true)
            {
                if (Application.isPlaying)
                { }
                else
                {
                    var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                    if (prefabStage != null)
                    {
                        SceneController sceneController = prefabStage.prefabContentsRoot.GetComponent<SceneController>();
                        if (sceneController)
                        {
                            List<SceneComponent> sceneComponents = sceneController.gameObject.GetComponentList<SceneComponent>();
                            Tools.ExecuteCoroutine(sceneComponents.Editor_UpdateSortRenderer(false));

                            yield return null;


                            for (int i = 0; i < sceneComponents.Count; i++)
                            {
                                SceneComponent sceneComponent = sceneComponents[i];
                                if (sceneComponent != null)
                                {
                                    // 设置组件的场景 add by TangJian 2019/1/23 16:56
                                    sceneController.gameObject.RecursiveComponent<SceneObjectController>((SceneObjectController soc, int depth) =>
                                    {
                                        soc.CurrSceneController = sceneController;
                                    }, 1, 999);

                                    sceneController.gameObject.RecursiveComponent<SortRenderer>((SortRenderer sr, int depth) =>
                                    {
                                        SortRendererManager.Instance.AddSortRenderer(sr);
                                    }, 1, 999);

                                    yield return null;
                                }
                            }

                            SortRendererManager.Instance.Update();
                            yield return null;
                        }
                    }
                }
                yield return null;
            }
        }
    }
}