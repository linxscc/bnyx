
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using MeshMakerNamespace;
using UnityEditor.AI;

namespace Tang.Editor
{
    [CustomEditor(typeof(SceneController))]
    [CanEditMultipleObjects]
    public class SceneControllerEditor : UnityEditor.Editor
    {
        static List<SceneController> manageredSceneControllerList = new List<SceneController>();
        int count = 0;
        int currcount = 0;
        //void AddSceneController(SceneController sceneController)
        //{
        //    RemoveSceneController(sceneController);
        //    manageredSceneControllerList.Add(sceneController);
        //}

        //void RemoveSceneController(SceneController sceneController)
        //{
        //    manageredSceneControllerList.Remove(sceneController);
        //}

        //private void OnEnable()
        //{
        //    AddSceneController(target as SceneController);

        //    AddAllSortRendererToSortRendererManager();
        //}

        //private void OnDisable()
        //{

        //}

        //static IEnumerator ienumerator;

        //public void AddAllSortRendererToSortRendererManager()
        //{
        //    EditorUpdateRunner.AddUpdateIfNot("AddAllSortRendererToSortRendererManager", () =>
        //    {
        //        if (ienumerator == null)
        //        {
        //            ienumerator = SortSortRenderer();
        //        }
        //        if (ienumerator.MoveNext() == false)
        //        {
        //            ienumerator = null;
        //        }
        //    });
        //    // EditorCoroutineRunner.AddCoroutineIfNot("AddAllSortRendererToSortRendererManager", SortSortRenderer());
        //}

        //public IEnumerator SortSortRenderer()
        //{
        //    while (true)
        //    {
        //        List<SceneController> tmpList = new List<SceneController>(manageredSceneControllerList);
        //        foreach (SceneController sceneController in tmpList)
        //        {
        //            try
        //            {
        //                sceneController.gameObject.RecursiveComponent<SortRenderer>((SortRenderer sr, int depth) =>
        //                {
        //                    SortRendererManager.Instance.AddSortRenderer(sr);
        //                }, 2, 99);
        //            }
        //            catch
        //            {
        //                RemoveSceneController(sceneController);
        //            }

        //            yield return 0;
        //        }

        //        SortRendererManager.Instance.Update();
        //        yield return 0;
        //    }
        //}

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("保存场景预制体", "MiniButtonRight", new GUILayoutOption[0]))
            {
                string bartitile;
                string textbar;
                float secs = 0f;
                float progress = 0f;
                bartitile = "保存场景预制体";
                secs = targets.Length;
                foreach (SceneController sceneController in targets)
                {
                    progress++;
                    textbar = "保存" + sceneController.name;
                    EditorUtility.DisplayProgressBar(bartitile, textbar, progress / secs);
                    Tools.ModifyPrefab(sceneController.gameObject);
                }
                EditorUtility.ClearProgressBar();
            }

            if (MyGUI.Button("更新场景组件"))
            {
                EditorCoroutineSequenceRunner.AddCoroutineIfNot("更新场景组件", UpdateSceneComponents());
                EditorCoroutineSequenceRunner.AddCoroutineIfNot("整理排场景组件", ArrangeSceneComponents());
                EditorCoroutineSequenceRunner.AddCoroutineIfNot("更新渲染", UpdateRenderer());
                EditorCoroutineSequenceRunner.AddCoroutineIfNot("更新显示区域", UpdateViewArea());
                EditorCoroutineSequenceRunner.AddCoroutineIfNot("重新挖洞", restartDig());

            }

            // if (MyGUI.Button("刷新场景组件"))
            // {
            //     EditorCoroutineRunner.AddCoroutineIfNot("刷新场景组件", RefreshSceneComponents());
            // }

            // if (MyGUI.Button("整理排场景组件"))
            // {
            //     EditorCoroutineRunner.AddCoroutineIfNot("整理排场景组件", ArrangeSceneComponents());
            // }

            // if (MyGUI.Button("更新显示区域"))
            // {
            //     EditorCoroutineRunner.AddCoroutineIfNot("更新显示区域", UpdateViewArea());
            // }


            if (MyGUI.Button("保存场景下所有未保存的网格"))
            {

                foreach (var item in targets)
                {
                    SceneController sceneController = item as SceneController;

                    sceneController.gameObject.GetChild("Terrains").RecursiveComponent<MeshFilter>((MeshFilter mf, int depth) =>
                    {
                        Debug.Log("mf.sharedMesh.name = " + mf.sharedMesh.name);
                        Debug.Log("AssetDatabase.GetAssetPath(mf.sharedMesh) = " + AssetDatabase.GetAssetPath(mf.sharedMesh));

                        if (mf.sharedMesh != null && AssetDatabase.GetAssetPath(mf.sharedMesh) == "")
                        {
                            string meshesPath = Tools.GetPrefabPath(sceneController.gameObject, true) + "/" + Tools.GetPrefabRoot(sceneController.gameObject).name + "/Meshes";
                            Debug.Log("materialPath = " + meshesPath);

                            Tools.CreateFolder(meshesPath);

                            string meshSavePath = meshesPath + "/" + mf.gameObject.transform.parent.parent.name + ".asset";

                            AssetDatabase.CreateAsset(mf.sharedMesh, meshSavePath);
                            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshSavePath);
                        }
                    }, 1, 999);
                }
            }

            if (MyGUI.Button("新建默认NavMeshSurface"))
            {
                string bartitile = "新建默认NavMeshSurface";
                float secs = targets.Length;
                float progress=0f;
                string textbar;
                NavMeshAssetManager.instance.overUpdate = () => 
                {
                    EditorUtility.ClearProgressBar();
                };
                foreach (SceneController sceneController in targets)
                {
                    LayerMask layerMask = new LayerMask();
                    layerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Terrain");
                    GameObject gameObject = sceneController.gameObject;
                    UnityEngine.AI.NavMeshBuildSettings navMeshBuildSettings = UnityEngine.AI.NavMesh.GetSettingsByIndex(0);
                    UnityEngine.AI.NavMeshSurface[] navMeshSurfaces = gameObject.GetComponents<UnityEngine.AI.NavMeshSurface>();
                    textbar = "场景：" + sceneController.name + "    Agent:" + UnityEngine.AI.NavMesh.GetSettingsNameFromID(navMeshBuildSettings.agentTypeID);
                    //EditorUtility.DisplayProgressBar(bartitile, textbar, secs);
                    progress += 1f / secs;
                    EditorUtility.DisplayProgressBar(bartitile, textbar, progress);
                    if (navMeshSurfaces.Length == 0)
                    {
                        UnityEngine.AI.NavMeshSurface navMeshSurface = gameObject.AddComponent<UnityEngine.AI.NavMeshSurface>();
                        navMeshSurface.agentTypeID = navMeshBuildSettings.agentTypeID;
                        navMeshSurface.collectObjects = UnityEngine.AI.CollectObjects.Children;
                        navMeshSurface.layerMask = layerMask;
                        navMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;

                        NavMeshAssetManager.instance.StartBakingSurfaces(gameObject.GetComponents<UnityEngine.AI.NavMeshSurface>());
                    }
                    else
                    {
                        bool firs = true;
                        foreach(UnityEngine.AI.NavMeshSurface navMeshSurface in navMeshSurfaces)
                        {
                            if(navMeshSurface.agentTypeID== navMeshBuildSettings.agentTypeID)
                            {
                                navMeshSurface.collectObjects = UnityEngine.AI.CollectObjects.Children;
                                navMeshSurface.layerMask = layerMask;
                                navMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
                                firs = false;
                                NavMeshAssetManager.instance.StartBakingSurfaces(gameObject.GetComponents<UnityEngine.AI.NavMeshSurface>());
                            }
                        }
                        if (firs)
                        {
                            UnityEngine.AI.NavMeshSurface navMeshSurface = gameObject.AddComponent<UnityEngine.AI.NavMeshSurface>();
                            navMeshSurface.agentTypeID = navMeshBuildSettings.agentTypeID;
                            navMeshSurface.collectObjects = UnityEngine.AI.CollectObjects.Children;
                            navMeshSurface.layerMask = layerMask;
                            navMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;

                            NavMeshAssetManager.instance.StartBakingSurfaces(gameObject.GetComponents<UnityEngine.AI.NavMeshSurface>());
                        }
                    }
                }
                //string bartitile;
                //string textbar;
                //float secs = 0f;
                //bartitile = "新建默认NavMeshSurface";
                //secs = targets.Length;
                //bool immediately = true;
                //foreach (SceneController sceneController in targets)
                //{
                //    LayerMask layerMask = new LayerMask();
                //    layerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Terrain");
                //    GameObject gameObject = sceneController.gameObject;

                //    //NavMeshAssetManager.instance.ClearSurfaces(gameObject.GetComponents<UnityEngine.AI.NavMeshSurface>());

                //    //foreach (UnityEngine.AI.NavMeshSurface nav in gameObject.GetComponents<UnityEngine.AI.NavMeshSurface>())
                //    UnityEngine.AI.NavMeshSurface[] navs = gameObject.GetComponents<UnityEngine.AI.NavMeshSurface>();
                //    int navsLength = navs.Length;
                //    for (int i = 0; i < navsLength; i++)
                //    {
                //        var nav = navs[i];
                //        DestroyImmediate(nav);
                //        PrefabUtility.ApplyRemovedComponent(gameObject, nav, InteractionMode.AutomatedAction);
                //    }


                //    //Reflection.Instance.Invoke("PrefabUtility", "RemoveRemovedComponentOverride", null, removedComponents.ToArray());

                //    //gameObject.RecursiveComponent<UnityEngine.AI.NavMeshSurface>((UnityEngine.AI.NavMeshSurface needRemoveComponent, int depth) =>
                //    //{
                //    //    if (immediately)
                //    //    {
                //    //        UnityEngine.Object.DestroyImmediate(needRemoveComponent);
                //    //    }
                //    //    else
                //    //    {
                //    //        Tools.Destroy(needRemoveComponent);
                //    //    }
                //    //}, 1, 1);

                //    //UnityEngine.AI.NavMeshBuildSettings navMeshBuildSettings = UnityEngine.AI.NavMesh.GetSettingsByIndex(0);
                //    //textbar = "场景：" + sceneController.name + "    Agent:" + UnityEngine.AI.NavMesh.GetSettingsNameFromID(navMeshBuildSettings.agentTypeID);
                //    //EditorUtility.DisplayProgressBar(bartitile, textbar, secs);
                //    //UnityEngine.AI.NavMeshSurface navMeshSurface = gameObject.AddComponent<UnityEngine.AI.NavMeshSurface>();

                //    //navMeshSurface.agentTypeID = navMeshBuildSettings.agentTypeID;
                //    //navMeshSurface.collectObjects = UnityEngine.AI.CollectObjects.Children;
                //    //navMeshSurface.layerMask = layerMask;
                //    //navMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;

                //    //NavMeshAssetManager.instance.StartBakingSurfaces(gameObject.GetComponents<UnityEngine.AI.NavMeshSurface>());

                //    {
                //        //NavMeshAssetManager.instance.StartBakingSurfaces(navMeshSurface);
                //        //int sad = UnityEngine.AI.NavMesh.GetSettingsCount();
                //        //for (int i = 0; i < UnityEngine.AI.NavMesh.GetSettingsCount(); i++)
                //        //{

                //        //    UnityEngine.AI.NavMeshBuildSettings navMeshBuildSettings = UnityEngine.AI.NavMesh.GetSettingsByIndex(i);

                //        //    progress += 1f / (float)sad;
                //        //    textbar = "场景：" + sceneController.name + "    Agent:" + UnityEngine.AI.NavMesh.GetSettingsNameFromID(navMeshBuildSettings.agentTypeID);
                //        //    EditorUtility.DisplayProgressBar(bartitile, textbar, progress / secs);

                //        //    UnityEngine.AI.NavMeshSurface navMeshSurface = gameObject.AddComponent<UnityEngine.AI.NavMeshSurface>();
                //        //    navMeshSurface.agentTypeID = navMeshBuildSettings.agentTypeID;
                //        //    navMeshSurface.collectObjects = UnityEngine.AI.CollectObjects.Children;
                //        //    navMeshSurface.layerMask = layerMask;

                //        //    var delete = GetNavMeshAssetToDelete(navMeshSurface);
                //        //    if (delete != null)
                //        //    {
                //        //        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(delete));
                //        //    }
                //        //    navMeshSurface.RemoveData();


                //        //    UnityEngine.AI.NavMeshData navMeshData = InitializeBakeData(navMeshSurface);
                //        //    navMeshSurface.navMeshData = navMeshData;
                //        //    navMeshSurface.AddData();


                //        //    navMeshSurface.UpdateNavMesh(navMeshData);
                //        //    EditorUtility.SetDirty(navMeshData);
                //        //    CreateNavMeshAsset(navMeshSurface, UnityEngine.AI.NavMesh.GetSettingsNameFromID(navMeshBuildSettings.agentTypeID));
                //        //    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(navMeshSurface.gameObject.scene);
                //        //}
                //    }

                //    //var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                //    //if (prefabStage == null)
                //    //{
                //    //    //prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
                //    //    string path = AssetDatabase.GetAssetPath(target);
                //    //    PrefabUtility.SaveAsPrefabAsset(gameObject, path);
                //    //}
                //    //else
                //    //{
                //    //    string path = prefabStage.prefabAssetPath;
                //    //    PrefabUtility.SaveAsPrefabAsset(gameObject, path);
                //    //}

                //}

                ////NavMeshAssetManager.instance.StartBakingSurfaces(objects.ToArray());
                //EditorUtility.ClearProgressBar();
            }
            if (MyGUI.Button("新建所有Agents的NavMeshSurface"))
            {
                NavMeshAssetManager.instance.overUpdate = () =>
                {
                    EditorUtility.ClearProgressBar();
                };
                string bartitile;
                bartitile = "新建所有Agents的NavMeshSurface";
                List<UnityEngine.AI.NavMeshSurface> navMeshSurfaceList = new List<UnityEngine.AI.NavMeshSurface>();
                List<UnityEngine.AI.NavMeshSurface> RemoveNavMeshSurfaceList = new List<UnityEngine.AI.NavMeshSurface>();
                foreach (SceneController sceneController in targets)
                {
                    //progress += 1f / secs;
                    //textbar = "场景：" + sceneController.name;
                    //EditorUtility.DisplayProgressBar(bartitile, textbar, progress);
                    LayerMask layerMask = new LayerMask();
                    layerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Terrain");
                    GameObject gameObject = sceneController.gameObject;
                    Dictionary<int, bool> navMeshBuildSettingsDic = new Dictionary<int, bool>();
                    

                    for (int i=0;i< UnityEngine.AI.NavMesh.GetSettingsCount(); i++)
                    {
                        UnityEngine.AI.NavMeshBuildSettings navMeshBuildSettings = UnityEngine.AI.NavMesh.GetSettingsByIndex(i);
                        navMeshBuildSettingsDic.Add(navMeshBuildSettings.agentTypeID, false);
                    }

                    foreach (UnityEngine.AI.NavMeshSurface navMeshSurface in gameObject.GetComponents<UnityEngine.AI.NavMeshSurface>())
                    {
                        if (navMeshBuildSettingsDic.ContainsKey(navMeshSurface.agentTypeID))
                        {
                            navMeshSurface.collectObjects = UnityEngine.AI.CollectObjects.Children;
                            navMeshSurface.layerMask = layerMask;
                            navMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
                            navMeshSurfaceList.Add(navMeshSurface);

                            navMeshBuildSettingsDic[navMeshSurface.agentTypeID] = true;
                        }
                        else
                        {
                            RemoveNavMeshSurfaceList.Add(navMeshSurface);
                        }
                    }

                    foreach(var keyValue in navMeshBuildSettingsDic)
                    {
                        if (keyValue.Value == false)
                        {
                            UnityEngine.AI.NavMeshSurface navMeshSurface = gameObject.AddComponent<UnityEngine.AI.NavMeshSurface>();
                            navMeshSurface.agentTypeID = keyValue.Key;
                            navMeshSurface.collectObjects = UnityEngine.AI.CollectObjects.Children;
                            navMeshSurface.layerMask = layerMask;
                            navMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
                            navMeshSurfaceList.Add(navMeshSurface);
                        }
                    }
                }
                count = navMeshSurfaceList.Count;
                currcount = 0;
                NavMeshAssetManager.instance.RecordNumber = (int Removecount,string agentname) => 
                {
                    currcount += Removecount;
                    float sds = ((float)currcount / (float)count);
                    EditorUtility.DisplayProgressBar(bartitile, agentname, sds);
                };
                NavMeshAssetManager.instance.StartBakingSurfaces(navMeshSurfaceList.ToArray());
                // string bartitile;
                // string textbar;
                // float secs = 0f;
                // float progress = 0f;
                // bartitile = "NavMeshSurface复用修改";
                // secs = targets.Length;
                // List<GameObject> gameObjects = new List<GameObject>();
                // foreach (SceneController sceneController in targets)
                // {
                //     LayerMask layerMask = new LayerMask();
                //     layerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Terrain");
                //     PrefabType prefabType = PrefabUtility.GetPrefabType(sceneController.gameObject);
                //     GameObject gameObject;
                //     UnityEngine.Object parent = null;
                //     if (prefabType == PrefabType.Prefab)
                //     {
                //         parent = sceneController.gameObject;
                //         gameObject = PrefabUtility.InstantiatePrefab(parent) as GameObject;
                //         gameObjects.Add(gameObject);
                //     }
                //     else if (prefabType != PrefabType.None)
                //     {
                //         gameObject = sceneController.gameObject;
                //         parent = PrefabUtility.GetPrefabParent(sceneController.gameObject);
                //     }
                //     else
                //     {
                //         //parent = null;
                //         gameObject = null;
                //     }

                //     //List<string> navdatapath = new List<string>();
                //     //Dictionary<string, string> navmeshdatadic = new Dictionary<string, string>();
                //     if (gameObject != null)
                //     {
                //         gameObject.RecursiveComponent<UnityEngine.AI.NavMeshSurface>((UnityEngine.AI.NavMeshSurface needRemoveComponent, int depth) =>
                //         {
                //             UnityEngine.Object.DestroyImmediate(needRemoveComponent, true);
                //         }, 1, 1);
                //         int sad = UnityEngine.AI.NavMesh.GetSettingsCount();
                //         for (int i = 0; i < UnityEngine.AI.NavMesh.GetSettingsCount(); i++)
                //         {
                //             progress += 1f / (float)sad;
                //             int fori = UnityEngine.AI.NavMesh.GetSettingsCount();
                //             UnityEngine.AI.NavMeshBuildSettings navMeshBuildSettings = UnityEngine.AI.NavMesh.GetSettingsByIndex(i);
                //             string SettingsName = UnityEngine.AI.NavMesh.GetSettingsNameFromID(navMeshBuildSettings.agentTypeID);
                //             string npath = Path.GetDirectoryName(Tools.GetPrefabPath(gameObject)) + "/" + "NavMeshs" + "/" + gameObject.name + "/" + "NavMesh-" + gameObject.name + "~" + SettingsName + ".asset"; ;

                //             UnityEngine.AI.NavMeshSurface navMeshSurface = gameObject.AddComponent<UnityEngine.AI.NavMeshSurface>();
                //             navMeshSurface.agentTypeID = navMeshBuildSettings.agentTypeID;
                //             navMeshSurface.collectObjects = UnityEngine.AI.CollectObjects.Children;
                //             navMeshSurface.layerMask = layerMask;

                //             var delete = GetNavMeshAssetToDelete(navMeshSurface);
                //             if (delete != null)
                //             {
                //                 AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(delete));
                //             }
                //             navMeshSurface.RemoveData();

                //             UnityEngine.AI.NavMeshData navMeshDataload = (UnityEngine.AI.NavMeshData)AssetDatabase.LoadAssetAtPath(npath, typeof(UnityEngine.AI.NavMeshData));

                //             if (navMeshDataload != null)
                //             {
                //                 textbar = "场景：" + sceneController.name + "    复用NavMeshSurface:" + UnityEngine.AI.NavMesh.GetSettingsNameFromID(navMeshBuildSettings.agentTypeID);
                //                 EditorUtility.DisplayProgressBar(bartitile, textbar, progress / secs);
                //                 navMeshSurface.navMeshData = navMeshDataload;
                //                 navMeshSurface.navMeshDataPath = npath;
                //                 navMeshSurface.AddData();
                //                 navMeshSurface.UpdateNavMesh(navMeshDataload);
                //                 EditorUtility.SetDirty(navMeshDataload);
                //                 UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(navMeshSurface.gameObject.scene);
                //                 //navmeshdatadic.Remove(gameObject.name + "~" + SettingsName);
                //             }
                //             else
                //             {
                //                 textbar = "场景：" + sceneController.name + "    新建NavMeshSurface:" + UnityEngine.AI.NavMesh.GetSettingsNameFromID(navMeshBuildSettings.agentTypeID);
                //                 EditorUtility.DisplayProgressBar(bartitile, textbar, progress / secs);
                //                 UnityEngine.AI.NavMeshData navMeshData = InitializeBakeData(navMeshSurface);
                //                 navMeshSurface.navMeshData = navMeshData;
                //                 navMeshSurface.AddData();

                //                 navMeshSurface.UpdateNavMesh(navMeshData);
                //                 //EditorUtility.SetDirty(navMeshData);
                //                 CreateNavMeshAsset(navMeshSurface, UnityEngine.AI.NavMesh.GetSettingsNameFromID(navMeshBuildSettings.agentTypeID));
                //                 UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(navMeshSurface.gameObject.scene);
                //             }

                //         }
                //         if (prefabType == PrefabType.Prefab)
                //         {
                //             if (parent != null)
                //                 PrefabUtility.ReplacePrefab(gameObject, parent, ReplacePrefabOptions.ConnectToPrefab);
                //         }
                //         else if (prefabType != PrefabType.None)
                //         {
                //             if (parent != null)
                //                 PrefabUtility.ReplacePrefab(gameObject, parent, ReplacePrefabOptions.ConnectToPrefab);
                //         }

                //     }


                // }
                // EditorUtility.ClearProgressBar();
                // AssetDatabase.SaveAssets();
                // for (int qwe = gameObjects.Count - 1; qwe >= 0; qwe--)
                // {
                //     Tools.Destroy(gameObjects[qwe]);
                // }
            }

            if (MyGUI.Button("重新挖洞"))
            {
                string bartitile;
                string textbar;
                float secs = 0f;
                float progress = 0f;
                bartitile = "重新挖洞";
                secs = targets.Length;
                foreach (SceneController sceneController in targets)
                {
                    GameObject gameObject = sceneController.gameObject;
                    GameObject Terrains = gameObject.GetChild("Terrains", false);
                    GameObject grounds = Terrains.GetChild("Grounds", false);
                    GameObject cubeGround = gameObject.GetChild("DigObjects", false);
                    if (cubeGround != null)
                    {
                        CSG.EPSILON = 1;
                        CSG csg = new CSG();

                        int countss = cubeGround.GetChildren().Count;
                        foreach (var item in cubeGround.GetChildren())
                        {
                            textbar = "场景" + sceneController.name + "第" + cubeGround.GetChildren().IndexOf(item) + "个";
                            progress += 1f / (float)countss;
                            EditorUtility.DisplayProgressBar(bartitile, textbar, progress / secs);
                            csg.Brush = item;
                            grounds.RecursiveComponent<Ground>((Ground g, int depth) =>
                            {
                                var cubeBounds = item.GetRendererBounds();
                                var groundBounds = g.gameObject.GetRendererBounds();

                                if (cubeBounds.Intersects(groundBounds))
                                {
                                    g.gameObject.GetChild("Renderer").RecursiveComponent<MeshFilter>((MeshFilter mf, int depth_1) =>
                                    {
                                        if (mf.gameObject.activeSelf)
                                        {
                                            Undo.RecordObject(mf.gameObject, mf.gameObject.name);

                                            csg.Target = mf.gameObject;
                                            csg.OperationType = CSG.Operation.Subtract;
                                            // csg.customMaterial = new Material(Shader.Find("Standard"));
                                            csg.useCustomMaterial = false;
                                            csg.hideGameObjects = true;
                                            csg.keepSubmeshes = true;
                                            GameObject newObject = csg.PerformCSG();
                                            Debug.Log(newObject.name);

                                            newObject.transform.parent = mf.gameObject.transform.parent;

                                            // 给新创建地面添加网格碰撞 add by TangJian 2018/03/22 15:34:30
                                            newObject.layer = LayerMask.NameToLayer("Ground");
                                            newObject.AddComponentUnique<MeshCollider>();

                                            Undo.RegisterCreatedObjectUndo(newObject, "PerformCSG");
                                            Undo.DestroyObjectImmediate(mf.gameObject);

                                            if (newObject.GetComponent<MeshFilter>().sharedMesh != null && AssetDatabase.GetAssetPath(newObject.GetComponent<MeshFilter>().sharedMesh) == "")
                                            {
                                                string meshesPath = Tools.GetPrefabPath(sceneController.gameObject, true) + "/" + Tools.GetPrefabRoot(sceneController.gameObject).name + "/Meshes";

                                                Tools.CreateFolder(meshesPath);

                                                string meshSavePath = meshesPath + "/" + g.gameObject.name + ".asset";

                                                AssetDatabase.CreateAsset(newObject.GetComponent<MeshFilter>().sharedMesh, meshSavePath);
                                                newObject.GetComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshSavePath);
                                            }
                                        }
                                    }, 1, 999);
                                }
                            }, 1, 999);

                        }
                    }
                }
                EditorUtility.ClearProgressBar();
            }

            if (MyGUI.Button("删除挖洞数据"))
            {

                EditorCoroutineSequenceRunner.AddCoroutineIfNot("更新场景组件", UpdateSceneComponents());
                EditorCoroutineSequenceRunner.AddCoroutineIfNot("整理排场景组件", ArrangeSceneComponents());
                EditorCoroutineSequenceRunner.AddCoroutineIfNot("更新渲染", UpdateRenderer());
                EditorCoroutineSequenceRunner.AddCoroutineIfNot("更新显示区域", UpdateViewArea());

                foreach (SceneController sceneController in targets)
                {
                    GameObject gameObject = sceneController.gameObject;
                    GameObject cubeGround = gameObject.GetChild("DigObjects", false);
                    if (cubeGround != null)
                    {
                        Tools.Destroy(cubeGround);
                    }
                    //    string meshesPath = Tools.GetPrefabPath(sceneController.gameObject, true) + "/" + Tools.GetPrefabRoot(sceneController.gameObject).name + "/Meshes";
                    //    Directory.Delete(meshesPath, true);
                }

            }

            //if (MyGUI.Button("创建排序节点"))
            //{

            //    foreach (SceneComponent item in targets)
            //    {
            //        item.Editor_UpdateRendererSort();
            //    }
            //}

            // if (MyGUI.Button("生成网格碰撞"))
            // {
            //     foreach (SceneController sceneController in targets)
            //     {

            //         string materialPath = Tools.GetPrefabPath(sceneController.gameObject, true) + "/" + Tools.GetPrefabRoot(sceneController.gameObject).name;
            //         Debug.Log("materialPath = " + materialPath);

            //         Tools.CreateFolder(materialPath);

            //         sceneController.gameObject.Recursive((GameObject go, int depth) =>
            //         {
            //             if (go.tag == "Ground" || go.tag == "BackWall" || go.tag == "FrontWall")
            //             {
            //                 var ground = go.GetComponent<TerrainComponent>();
            //                 ground.CreateMesh(materialPath + "/" + Tools.getOnlyId());
            //             }
            //         }, 1, 999);
            //     }
            // }

            // if (MyGUI.Button("刷新渲染"))
            // {
            //     foreach (SceneController sceneController in targets)
            //     {
            //         sceneController.gameObject.Recursive((GameObject go, int depth) =>
            //         {
            //             if (go.tag == "Ground" || go.tag == "BackWall" || go.tag == "FrontWall")
            //             {
            //                 var ground = go.GetComponent<TerrainComponent>();
            //                 ground.UpdateRenderer();
            //             }
            //         }, 1, 999);
            //     }
            // }

            DrawDefaultInspector();


            // 刷新场景位置
            foreach (var item in targets)
            {
                SceneController sceneController = item as SceneController;
                sceneController.transform.position = new Vector3(sceneController.transform.position.x, 0, sceneController.transform.position.z);
            }
        }
        static UnityEngine.AI.NavMeshData GetNavMeshAssetToDelete(UnityEngine.AI.NavMeshSurface navSurface)
        {
            var prefabType = PrefabUtility.GetPrefabType(navSurface);
            if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
            {
                // Don't allow deleting the asset belonging to the prefab parent
                var parentSurface = PrefabUtility.GetCorrespondingObjectFromSource(navSurface) as UnityEngine.AI.NavMeshSurface;
                if (parentSurface && navSurface.navMeshData == parentSurface.navMeshData)
                    return null;
            }
            return navSurface.navMeshData;
        }
        static string GetAndEnsureTargetPath(UnityEngine.AI.NavMeshSurface surface, string activeScenePath = null)
        {
            // Create directory for the asset if it does not exist yet.
            if (activeScenePath != null)
            {

            }
            else
            {
                activeScenePath = surface.gameObject.scene.path;
            }
            //var activeScenePath = surface.gameObject.scene.path;

            var targetPath = "Assets";
            if (!string.IsNullOrEmpty(activeScenePath))
                targetPath = Path.Combine(Path.GetDirectoryName(activeScenePath), Path.GetFileNameWithoutExtension(activeScenePath));
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
            return targetPath;
        }
        static void CreateNavMeshAsset(UnityEngine.AI.NavMeshSurface surface, string AgentTypeName, string activeScenePath = null, string prefabPath = null)
        {
            var targetPath = GetAndEnsureTargetPath(surface, activeScenePath);

            //var combinedAssetPath = Path.Combine(targetPath, "NavMesh-" + surface.name + ".asset");
            if (prefabPath == null)
            {
                prefabPath = Path.GetDirectoryName(Tools.GetPrefabPath(surface.gameObject)) + "/" + "NavMeshs" + "/" + surface.gameObject.name + "/" + "NavMesh-" + surface.name + "~" + AgentTypeName + ".asset";
            }

            Tools.CreateAssetFolder(Path.GetDirectoryName(prefabPath));
            AssetDatabase.CreateAsset(surface.navMeshData, prefabPath);
            //surface.navMeshDataPath = prefabPath;
        }
        static UnityEngine.AI.NavMeshData InitializeBakeData(UnityEngine.AI.NavMeshSurface surface)
        {
            var emptySources = new List<UnityEngine.AI.NavMeshBuildSource>();
            var emptyBounds = new Bounds();
            return UnityEngine.AI.NavMeshBuilder.BuildNavMeshData(surface.GetBuildSettings(), emptySources, emptyBounds
                , surface.transform.position, surface.transform.rotation);
        }
        public IEnumerator UpdateSceneComponents()
        {
            Debug.Log("开始更新组件");
            {
                string bartitile;
                string textbar;
                float secs = 0f;
                float progress = 0f;
                bartitile = "更新场景组件预制体";
                secs = targets.Length;
                for (int i = 0; i < targets.Length; i++)
                {

                    SceneController sceneController = targets[i] as SceneController;

                    List<SceneComponent> sceneComponentList = new List<SceneComponent>();

                    // 获得租金啊最小X
                    float minX = 0;
                    sceneController.gameObject.RecursiveComponent<SceneComponent>((SceneComponent tc, int dp) =>
                    {
                        minX = tc.GridPos.x < minX ? tc.GridPos.x : minX;
                        sceneComponentList.Add(tc);
                    }, 1, 99);

                    sceneComponentList.Sort((SceneComponent a, SceneComponent b) =>
                    {
                        if (a.GridPos.x > b.GridPos.x)
                        {
                            return 1;
                        }
                        else if (a.GridPos.x < b.GridPos.x)
                        {
                            return -1;
                        }
                        return 0;
                    });
                    float count = sceneComponentList.Count;
                    for (int j = 0; j < sceneComponentList.Count; j++)
                    {
                        progress = (j / count) + i;
                        textbar = "场景：" + sceneController.name + "更新组件:" + sceneComponentList[j].name;
                        EditorUtility.DisplayProgressBar(bartitile, textbar, progress / secs);

                        SceneComponent sceneComponent = sceneComponentList[j];

                        Debug.Log("刷新组件: " + sceneComponent.name);

                        sceneComponent.GridPos = new Vector3(sceneComponent.GridPos.x + Mathf.Abs(minX), sceneComponent.GridPos.y, sceneComponent.GridPos.z);
                        //GameObject gameObject;
                        //if (sceneComponent.gameObject.tag == "FrontWall" || sceneComponent.gameObject.tag == "BackWall")
                        //{
                        //    gameObject = GameObjectManager.Instance.terrains.Find((GameObject gameobj) => { return sceneComponent.gameObject.name.Contains(gameobj.name); });
                        //}
                        //else
                        //{
                        //    gameObject = GameObjectManager.Instance.placement.Find((GameObject gameobj) => { return sceneComponent.gameObject.name.Contains(gameobj.name); });
                        //}
                        //string filepath = sceneComponent.FilePath;
                        //char[] charlist = { '/', '.' };
                        //string[] stringlist = filepath.Split(charlist);
                        //string namepath = null;
                        //if (stringlist.Length > 2)
                        //{
                        //    namepath = stringlist[stringlist.Length - 2];
                        //}
                        //if (namepath != null)
                        //{
                        //    if (sceneComponent.gameObject.name.Contains(namepath))
                        //    {

                        //    }
                        //    else
                        //    {
                        //        sceneComponent.FilePath = AssetDatabase.GetAssetPath(gameObject);
                        //    }
                        //}
                        //else
                        //{
                        //    sceneComponent.FilePath = AssetDatabase.GetAssetPath(gameObject);
                        //}
                        try
                        {
                            sceneComponent.Editor_UpdateToPrefab();
                        }
                        catch
                        {
                            Debug.LogWarning("更新组件失败, 移除异常GameObject:" + sceneController.name + "-" + sceneComponent.name);
                            GameObject.DestroyImmediate(sceneComponent.gameObject);
                        }
                        //yield return 0;
                    }
                    //yield return 0;
                }
                EditorUtility.ClearProgressBar();
            }
            yield return 0;
        }
        //重新挖洞
        public IEnumerator restartDig()
        {
            string bartitile;
            string textbar;
            float secs = 0f;
            float progress = 0f;
            bartitile = "重新挖洞";
            secs = targets.Length;
            foreach (SceneController sceneController in targets)
            {
                GameObject gameObject = sceneController.gameObject;
                GameObject Terrains = gameObject.GetChild("Terrains", false);
                GameObject grounds = Terrains.GetChild("Grounds", false);
                GameObject cubeGround = gameObject.GetChild("DigObjects", false);
                if (cubeGround != null)
                {
                    CSG.EPSILON = 1;
                    CSG csg = new CSG();

                    foreach (var item in cubeGround.GetChildren())
                    {
                        cubeGround.GetChildren().IndexOf(item);
                        csg.Brush = item;
                        grounds.RecursiveComponent<Ground>((Ground g, int depth) =>
                        {
                            var cubeBounds = item.GetRendererBounds();
                            var groundBounds = g.gameObject.GetRendererBounds();

                            if (cubeBounds.Intersects(groundBounds))
                            {
                                g.gameObject.GetChild("Renderer").RecursiveComponent<MeshFilter>((MeshFilter mf, int depth_1) =>
                                {
                                    if (mf.gameObject.activeSelf)
                                    {
                                        Undo.RecordObject(mf.gameObject, mf.gameObject.name);

                                        csg.Target = mf.gameObject;
                                        csg.OperationType = CSG.Operation.Subtract;
                                        // csg.customMaterial = new Material(Shader.Find("Standard"));
                                        csg.useCustomMaterial = false;
                                        csg.hideGameObjects = true;
                                        csg.keepSubmeshes = true;
                                        GameObject newObject = csg.PerformCSG();
                                        Debug.Log(newObject.name);

                                        newObject.transform.parent = mf.gameObject.transform.parent;

                                        // 给新创建地面添加网格碰撞 add by TangJian 2018/03/22 15:34:30
                                        newObject.layer = LayerMask.NameToLayer("Ground");
                                        newObject.AddComponentUnique<MeshCollider>();

                                        Undo.RegisterCreatedObjectUndo(newObject, "PerformCSG");
                                        Undo.DestroyObjectImmediate(mf.gameObject);

                                        if (newObject.GetComponent<MeshFilter>().sharedMesh != null && AssetDatabase.GetAssetPath(newObject.GetComponent<MeshFilter>().sharedMesh) == "")
                                        {
                                            string meshesPath = Tools.GetPrefabPath(sceneController.gameObject, true) + "/" + Tools.GetPrefabRoot(sceneController.gameObject).name + "/Meshes";

                                            Tools.CreateFolder(meshesPath);

                                            string meshSavePath = meshesPath + "/" + g.gameObject.name + ".asset";

                                            AssetDatabase.CreateAsset(newObject.GetComponent<MeshFilter>().sharedMesh, meshSavePath);
                                            newObject.GetComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshSavePath);
                                        }
                                    }
                                }, 1, 999);
                            }
                        }, 1, 999);
                        textbar = "场景" + sceneController.name + "第" + cubeGround.GetChildren().IndexOf(item) + "个";
                        progress += 1 / cubeGround.GetChildren().Count;
                        EditorUtility.DisplayProgressBar(bartitile, textbar, progress / secs);


                    }
                }
            }
            EditorUtility.ClearProgressBar();
            yield return 0;
        }
        public IEnumerator RefreshSceneComponents()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                SceneController sceneController = targets[i] as SceneController;
                List<SceneComponent> sceneComponentList = new List<SceneComponent>();

                sceneController.gameObject.RecursiveComponent<SceneComponent>((SceneComponent tc, int dp) =>
                {
                    sceneComponentList.Add(tc);
                }, 1, 99);

                foreach (var sceneComponent in sceneComponentList)
                {
                    // sceneComponent.Refresh();
                }
            }
            yield return 0;
        }

        public IEnumerator ArrangeSceneComponents()
        {
            // 刷新组件关系 add by TangJian 2018/04/26 15:15:23
            {
                string bartitile;
                string textbar;
                float secs = 0f;
                float progress = 0f;
                bartitile = "整理场景组件";
                secs = targets.Length;
                for (int i = 0; i < targets.Length; i++)
                {

                    SceneController sceneController = targets[i] as SceneController;
                    List<SceneComponent> sceneComponentList = new List<SceneComponent>();
                    textbar = "场景：" + sceneController.name + "整理中";
                    // 获得租金啊最小X
                    float minPosX = float.MaxValue;
                    float minPosZ = float.MaxValue;
                    sceneController.gameObject.RecursiveComponent<SceneComponent>((SceneComponent tc, int dp) =>
                    {
                        minPosX = Mathf.Min(minPosX, tc.transform.localPosition.x);
                        minPosZ = Mathf.Min(minPosZ, tc.transform.localPosition.z);
                        sceneComponentList.Add(tc);
                    }, 1, 99);

                    // 移动场景中得地形区域
                    GameObject terrains = sceneController.gameObject.GetChild("Terrains");
                    terrains.transform.localPosition = Vector3.zero;
                    terrains.Recursive((GameObject terrain, int depth) =>
                    {
                        terrain.transform.localPosition = Vector3.zero;
                    }, 2, 2);
                    progress += 0.2f;
                    EditorUtility.DisplayProgressBar(bartitile, textbar, progress / secs);
                    // Placements区域
                    GameObject placements = sceneController.gameObject.GetChild("Placements");
                    placements.transform.localPosition = Vector3.zero;

                    // Decorations区域
                    GameObject decorations = sceneController.gameObject.GetChild("Decorations");
                    decorations.transform.localPosition = Vector3.zero;

                    // Items区域
                    GameObject items = sceneController.gameObject.GetChild("Items");
                    items.transform.localPosition = Vector3.zero;

                    // 移动场景位置到父节点中心位置
                    // foreach (var item in sceneComponentList)
                    // {
                    //     if (item is ScenePlacementComponent || item is SceneDecorationComponent)
                    //     {

                    //     }
                    //     else
                    //     {
                    //         item.MoveByLocalPos(-new Vector3(minPosX, 0, minPosZ));
                    //     }
                    // }

                    // 移动场景中得刷怪区域
                    GameObject areas = sceneController.gameObject.GetChild("Areas");
                    areas.transform.localPosition = Vector3.zero;

                    // areas.Recursive((GameObject areaGO, int depth) =>
                    // {
                    //     areaGO.transform.localPosition = Vector3.zero;

                    //     areaGO.Recursive((GameObject cubeGO, int depth1) =>
                    //     {
                    //         cubeGO.transform.localPosition = new Vector3(cubeGO.transform.localPosition.x - minPosX
                    //         , cubeGO.transform.localPosition.y
                    //         , cubeGO.transform.localPosition.z - minPosZ);
                    //         Debug.Log("cubeGO.name = " + cubeGO.name);
                    //     }, 2, 2);
                    // }, 2, 2);



                    List<Ground> groundList = new List<Ground>();

                    // 设置地面高度 防止重叠 add by TangJian 2018/04/25 19:28:22
                    foreach (var sceneComponent in sceneComponentList)
                    {
                        if (sceneComponent is Ground)
                            groundList.Add(sceneComponent as Ground);
                    }
                    progress += 0.2f;
                    EditorUtility.DisplayProgressBar(bartitile, textbar, progress / secs);
                    float floorGap = 0.01f / 100f;

                    // 找到中间地面 add by TangJian 2018/04/25 19:30:19                    
                    for (int j = 0; j < groundList.Count; j++)
                    {
                        Ground ground = groundList[j];

                        var rendererObject = ground.gameObject.GetChild("Renderer");

                        if (ground.groundData.groundType == GroundType.Center)
                        {
                            rendererObject.transform.localPosition = new Vector3(rendererObject.transform.localPosition.x, -floorGap * 2, rendererObject.transform.localPosition.z);
                            rendererObject.RecursiveComponent((MeshRenderer mr, int depth) =>
                            {
                                mr.sortingOrder = -1;
                            }, 1, 99);
                        }
                        else
                        {
                            if (j % 2 == 0)
                            {
                                rendererObject.transform.localPosition = new Vector3(rendererObject.transform.localPosition.x, 0f, rendererObject.transform.localPosition.z);
                                rendererObject.RecursiveComponent((MeshRenderer mr, int depth) =>
                                {
                                    mr.sortingOrder = 0;
                                }, 1, 99);
                            }
                            else
                            {
                                rendererObject.transform.localPosition = new Vector3(rendererObject.transform.localPosition.x, floorGap * j, rendererObject.transform.localPosition.z);
                                rendererObject.RecursiveComponent((MeshRenderer mr, int depth) =>
                                {
                                    mr.sortingOrder = 1;
                                }, 1, 99);
                            }
                        }
                        progress += 0.6f * (1 / groundList.Count);
                        EditorUtility.DisplayProgressBar(bartitile, textbar, progress / secs);
                        Debug.Log("排列组件: " + ground.name);
                        //yield return 0;
                    }
                    //yield return 0;
                }
                EditorUtility.ClearProgressBar();
            }
            yield return 0;
        }

        public IEnumerator UpdateRenderer()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                SceneController sceneController = targets[i] as SceneController;
                List<SortRenderer> rortRendererList = new List<SortRenderer>();

                sceneController.gameObject.RecursiveComponent<SortRenderer>((SortRenderer sr, int depth) =>
                {
                    rortRendererList.Add(sr);
                }, 1, 999);

                rortRendererList.Sort((SortRenderer a, SortRenderer b) =>
                {
                    if (a.PosZ > b.PosZ)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                });

                for (int j = 0; j < rortRendererList.Count; j++)
                {
                    SortRenderer sortRenderer = rortRendererList[j];
                    int currZOrder = (int)ZOrder.ObjectMin + j;
                    sortRenderer.SetZorder(currZOrder);
                }
                //yield return 0;
            }

            yield return 0;
        }

        // 更新可视区域 add by TangJian 2018/04/27 17:02:41
        public IEnumerator UpdateViewArea()
        {
            Debug.Log("开始 UpdateViewArea");
            for (int i = 0; i < targets.Length; i++)
            {
                SceneController sceneController = targets[i] as SceneController;

                GameObject sideWalls = sceneController.gameObject.GetChild("Terrains").GetChild("SideWalls");
                Debug.Assert(sideWalls != null, sceneController.name + ":" + "必须要有SideWalls");

                Bounds viewBounds = sideWalls.GetColliderBounds(null, 2, 2);

                GameObject viewAreas = sceneController.gameObject.GetChild("ViewAreas", true);
                viewAreas.transform.localPosition = Vector3.zero;

                GameObject vb1 = viewAreas.GetChild("vb1", true);

                BoxCollider boxCollider = vb1.AddComponentUnique<BoxCollider>();

                boxCollider.center = viewAreas.transform.InverseTransformPoint(viewBounds.center);
                boxCollider.size = viewBounds.size;

                //yield return 0;
            }
            Debug.Log("完成 UpdateViewArea");
            yield return 0;
        }
    }
}