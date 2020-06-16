using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Tang
{
    using Interfaces;

    public static class SceneComponentExtensions
    {
#if UNITY_EDITOR
        public static void GetDataFrom(this SceneComponent target, SceneComponent fromTarget)
        {
            target.isMirror = fromTarget.isMirror;
            target.flipY = fromTarget.flipY;
            target.flipZ = fromTarget.flipZ;

            target.Data = fromTarget.Data;
        }

        public static void GetDataFromTo(SceneSideDoorComponent newTarget, BackDoor target)
        {
            if (target.wallData.wallType == BackWallType.Left)
            {
                newTarget.isMirror = target.isMirror;
                newTarget.flipY = target.flipY;
                newTarget.flipZ = target.flipZ;

                SideWallData sideWallData = newTarget.Data as SideWallData;
                BackWallData backWallData = target.Data as BackWallData;

                sideWallData.widthY = backWallData.widthY;
                sideWallData.widthZ = backWallData.widthZ;

                sideWallData.x = backWallData.x;
                sideWallData.y = backWallData.z;
                sideWallData.z = backWallData.y;
            }
            else
            {
                Debug.LogError("target.wallData.wallType != BackWallType.Left");
            }
        }

        // 更新场景组件到prefab add by TangJian 2018/11/12 20:27
        public static SceneComponent Editor_UpdateToPrefab(this SceneComponent target, GameObject prefab = null)
        {
            if (prefab == null)
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(target.filePath);
                if (prefab == null)
                {
                    prefab = target.prefab;
                }
            }

            if (prefab != null)
            {
                GameObject newTargetGameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

                SceneComponent newTarget = newTargetGameObject.GetComponent<SceneComponent>();

                newTargetGameObject.transform.parent = target.gameObject.transform.parent;
                newTargetGameObject.name = target.gameObject.name;

                if (newTarget as SceneSideDoorComponent && target as BackDoor)
                {
                    GetDataFromTo(newTarget as SceneSideDoorComponent, target as BackDoor);
                }
                else
                {
                    newTarget.GetDataFrom(target);
                }

                // 获取老的场景组件上面的组件数据, 拷贝到新的组件 add by TangJian 2018/12/21 13:58
                {
                    IData[] oldDatas = target.gameObject.GetComponents<IData>();
                    foreach (var oldData in oldDatas)
                    {
                        Type t = oldData.GetType();
                        Debug.Log("t = " + t);
                        IData newData = newTarget.gameObject.GetComponent(t) as IData;
                        if (newData != null && newData.Data != null && oldData != null && oldData.Data != null)
                            newData.Data = oldData.Data;
                    }
                }

                newTarget.Refresh();

                Undo.RegisterCreatedObjectUndo(newTargetGameObject, newTargetGameObject.name);

                Undo.DestroyObjectImmediate(target.gameObject);

                newTarget.RefreshFlip();

                newTarget.Editor_UpdateRendererSort(); // 刷新渲染排序 add by TangJian 2018/11/12 17:26

                return newTarget;
            }
            else
            {
                Debug.LogWarning("UpdateSelf Failed sceneComponentName = " + target.name);
                return target;
            }
        }

        // 更新组件到最新 add by TangJian 2018/11/12 16:49
        public static IEnumerator Editor_UpdateToPrefab(this List<SceneComponent> target)
        {
            List<UnityEngine.Object> selectedObjects = new List<UnityEngine.Object>();
            for (int i = 0; i < target.Count; i++)
            {
                SceneComponent sc = target[i];
                EditorUtility.DisplayProgressBar("更新组件", "更新组件:" + sc.name, (float)i / target.Count);

                yield return null;
                SceneComponent newSceneComponent = sc.Editor_UpdateToPrefab();
                selectedObjects.Add(newSceneComponent.gameObject);
            }
            Selection.objects = selectedObjects.ToArray();
            EditorUtility.ClearProgressBar();
        }

        // 更新渲染排序 add by TangJian 2018/11/12 16:49
        public static IEnumerator Editor_UpdateSortRenderer(this List<SceneComponent> target, bool displayProgress = true)
        {
            List<UnityEngine.Object> selectedObjects = new List<UnityEngine.Object>();
            for (int i = 0; i < target.Count; i++)
            {
                SceneComponent sc = target[i];

                if (displayProgress)
                    EditorUtility.DisplayProgressBar("更新渲染排序", "更新渲染排序:" + sc.name, (float)i / target.Count);

                yield return null;
                sc.Editor_UpdateRendererSort();
            }

            target.Editor_UpdateSideWallRendererSort(); // 更新斜墙 add by TangJian 2019/1/21 19:56

            EditorUtility.ClearProgressBar();
        }

        // 设置场景组件路径 add by TangJian 2018/11/12 20:27
        public static SceneComponent Editor_UpdatePath(this SceneComponent target)
        {
            target.filePath = Tools.GetPrefabPath(target.gameObject);
            target.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(target.filePath);
            return target;
        }

        public static void Editor_UpdateRendererSort(this SceneComponent target)
        {
            if (target.gameObject.GetComponent<Collider>())
            {
                //if (target as SceneSideWallComponent)
                //{
                //    Bounds bounds = target.gameObject.GetComponent<Collider>().bounds;
                //    GameObject rendererGameObject = target.gameObject.GetChild("Renderer", true);

                //    List<Renderer> rendererList = rendererGameObject.GetComponentList<Renderer>();

                //    foreach (var renerer in rendererList)
                //    {
                //        // 添加排序节点 add by TangJian 2018/11/6 21:23
                //        SortRenderer sortRenderer = renerer.gameObject.AddComponentUnique<SortRenderer>(true);
                //        sortRenderer.sortRendererType = SortRendererType.Side;
                //        sortRenderer.rendererBounds = new Bounds(sortRenderer.transform.InverseTransformPoint(bounds.center + new Vector3(0, 0, bounds.size.z / 2f)), new Vector3(bounds.size.x, bounds.size.y, 0));
                //        //sortRenderer.rendererBounds = new Bounds(sortRenderer.transform.InverseTransformPoint(bounds.center), bounds.size);
                //    }
                //}
                //else if (target as SceneCenterWallComponent)
                //{
                //    Bounds bounds = target.gameObject.GetComponent<Collider>().bounds;
                //    GameObject rendererGameObject = target.gameObject.GetChild("Renderer", true);

                //    List<Renderer> rendererList = rendererGameObject.GetComponentList<Renderer>();

                //    foreach (var renerer in rendererList)
                //    {
                //        // 添加排序节点 add by TangJian 2018/11/6 21:23
                //        SortRenderer sortRenderer = renerer.gameObject.AddComponentUnique<SortRenderer>(true);
                //        sortRenderer.rendererBounds = new Bounds(sortRenderer.transform.InverseTransformPoint(bounds.center + new Vector3(0, 0, bounds.size.z / 2f)), new Vector3(bounds.size.x, bounds.size.y, 0));
                //    }
                //}
                //else if (target as BackDoor)
                //{
                //    Bounds bounds = target.gameObject.GetComponent<Collider>().bounds;
                //    GameObject rendererGameObject = target.gameObject.GetChild("Renderer", true);

                //    List<Renderer> rendererList = rendererGameObject.GetComponentList<Renderer>();

                //    foreach (var renerer in rendererList)
                //    {
                //        // 添加排序节点 add by TangJian 2018/11/6 21:23
                //        SortRenderer sortRenderer = renerer.gameObject.AddComponentUnique<SortRenderer>(true);
                //        sortRenderer.rendererBounds = new Bounds(sortRenderer.transform.InverseTransformPoint(bounds.center + new Vector3(0, 0, bounds.size.z / 2f)), new Vector3(bounds.size.x, bounds.size.y, 0));
                //        if ((target as BackDoor).wallData.wallType == BackWallType.Left)
                //        {
                //            sortRenderer.sortRendererType = SortRendererType.Side;
                //        }
                //    }
                //}
                //else if (target as Ground)
                //{
                //    Bounds bounds = target.gameObject.GetComponent<Collider>().bounds;
                //    GameObject rendererGameObject = target.gameObject.GetChild("Renderer", true);

                //    List<Renderer> rendererList = rendererGameObject.GetComponentList<Renderer>();

                //    foreach (var renerer in rendererList)
                //    {
                //        // 添加排序节点 add by TangJian 2018/11/6 21:23
                //        SortRenderer sortRenderer = renerer.gameObject.AddComponentUnique<SortRenderer>(true);
                //        sortRenderer.rendererBounds = new Bounds(sortRenderer.transform.InverseTransformPoint(bounds.center) + new Vector3(0, 0.00001f, 0), bounds.size);
                //    }
                //}
                //else 
                //if (target as ScenePlacementComponent)
                //{
                //    Bounds bounds = target.gameObject.GetComponent<Collider>().bounds;
                //    GameObject rendererGameObject = target.gameObject.GetChild("Renderer", true);

                //    List<Renderer> rendererList = rendererGameObject.GetComponentList<Renderer>();

                //    foreach (var renerer in rendererList)
                //    {
                //        // 添加排序节点 add by TangJian 2018/11/6 21:23
                //        SortRenderer sortRenderer = renerer.gameObject.AddComponentUnique<SortRenderer>(true);
                //        sortRenderer.SetSortRendererPos(new Vector3(0, 0, bounds.max.z));
                //    }
                //}
                //else if (target as SceneCenterWallComponent)
                //{
                //    Bounds bounds = target.gameObject.GetComponent<Collider>().bounds;
                //    GameObject rendererGameObject = target.gameObject.GetChild("Renderer", true);

                //    List<Renderer> rendererList = rendererGameObject.GetComponentList<Renderer>();

                //    foreach (var renerer in rendererList)
                //    {
                //        // 添加排序节点 add by TangJian 2018/11/6 21:23
                //        SortRenderer sortRenderer = renerer.gameObject.AddComponentUnique<SortRenderer>(true);
                //        sortRenderer.rendererBounds = new Bounds(sortRenderer.transform.InverseTransformPoint(bounds.center + new Vector3(0, -bounds.size.y / 2 + 0.00001f, bounds.size.z / 2f + 0.00001f)), new Vector3(bounds.size.x, 0, 0));
                //    }
                //}
                //else

                //if (target as SceneSideWallComponent)
                //{
                //}
                //else if()
                //else
                //{
                //    Bounds bounds = target.gameObject.GetComponent<Collider>().bounds;
                //    GameObject rendererGameObject = target.gameObject.GetChild("Renderer", true);

                //    List<Renderer> rendererList = rendererGameObject.GetComponentList<Renderer>();

                //    foreach (var renerer in rendererList)
                //    {
                //        // 添加排序节点 add by TangJian 2018/11/6 21:23
                //        SortRenderer sortRenderer = renerer.gameObject.AddComponentIfNone<SortRenderer>();
                //        sortRenderer.SetSortRendererPos(new Vector3(bounds.center.x, bounds.center.y, bounds.max.z));
                //    }
                //}
            }
        }

        public static void Editor_UpdateSideWallRendererSort(this List<SceneComponent> target)
        {
            List<ISideWall> leftSideWalls = new List<ISideWall>();
            List<ISideWall> rightSideWalls = new List<ISideWall>();
            for (int i = 0; i < target.Count; i++)
            {
                SceneComponent sceneComponent = target[i];

                ISideWall sideWall = sceneComponent as ISideWall;
                if (sideWall != null)
                {
                    if (sideWall.IsMirror)
                        rightSideWalls.Add(sideWall);
                    else
                        leftSideWalls.Add(sideWall);
                }
            }

            // 左墙 add by TangJian 2019/1/18 16:06
            {
                leftSideWalls.Sort((ISideWall a, ISideWall b) =>
                {
                    if (a.LinkBeginPos.x > b.LinkBeginPos.x)
                    {
                        return 1;
                    }
                    else if (a.LinkBeginPos.x < b.LinkBeginPos.x)
                    {
                        return -1;
                    }
                    else
                    {
                        if (a.LinkBeginPos.y > b.LinkBeginPos.y)
                        {
                            return 1;
                        }
                        else if (a.LinkBeginPos.y < b.LinkBeginPos.y)
                        {
                            return -1;
                        }
                        else
                        {
                            if (a.LinkBeginPos.z > b.LinkBeginPos.z)
                            {
                                return 1;
                            }
                            else if (a.LinkBeginPos.z < b.LinkBeginPos.z)
                            {
                                return -1;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                });

                {
                    List<ISideWall> sideWallGroup = new List<ISideWall>();

                    for (int i = 0; i < leftSideWalls.Count; i++)
                    {
                        ISideWall sideWall = leftSideWalls[i];

                        if (sideWallGroup.Count == 0)
                        {
                            // 开始 add by TangJian 2019/1/18 15:39
                            sideWallGroup.Add(sideWall);
                        }
                        else
                        {
                            if (sideWallGroup[sideWallGroup.Count - 1].LinkEndPos == sideWall.LinkBeginPos)
                            {
                                if (sideWall.IsBreak)
                                {
                                    Vector3 frontSortRendererPos = sideWall.GetFrontSortRendererPos();
                                    foreach (var item in sideWallGroup)
                                    {
                                        item.SetBackSortRendererPos(frontSortRendererPos);
                                    }
                                    sideWallGroup.Clear();
                                }

                                sideWallGroup.Add(sideWall);
                            }
                            else
                            {
                                // 打破连接 add by TangJian 2019/1/18 15:56
                                Vector3 backSortRendererPos = sideWallGroup[sideWallGroup.Count - 1].GetBackSortRendererPos();

                                foreach (var item in sideWallGroup)
                                {
                                    item.SetBackSortRendererPos(backSortRendererPos);
                                }

                                sideWallGroup.Clear();
                                sideWallGroup.Add(sideWall);
                            }
                        }

                        if (i == leftSideWalls.Count - 1)
                        {
                            Vector3 backSortRendererPos = sideWallGroup[sideWallGroup.Count - 1].GetBackSortRendererPos();

                            foreach (var item in sideWallGroup)
                            {
                                item.SetBackSortRendererPos(backSortRendererPos);
                            }
                        }
                    }
                }
            }

            // 右墙 add by TangJian 2019/1/18 16:06
            {
                rightSideWalls.Sort((ISideWall a, ISideWall b) =>
                {
                    if (a.LinkBeginPos.x + a.LinkBeginPos.z > b.LinkBeginPos.x + b.LinkBeginPos.z)
                    {
                        return -1;
                    }
                    else if (a.LinkBeginPos.x + a.LinkBeginPos.z < b.LinkBeginPos.x + b.LinkBeginPos.z)
                    {
                        return 1;
                    }
                    else
                    {
                        if (a.LinkBeginPos.y > b.LinkBeginPos.y)
                        {
                            return 1;
                        }
                        else if (a.LinkBeginPos.y < b.LinkBeginPos.y)
                        {
                            return -1;
                        }
                        else
                        {
                            if (a.LinkBeginPos.z > b.LinkBeginPos.z)
                            {
                                return 1;
                            }
                            else if (a.LinkBeginPos.z < b.LinkBeginPos.z)
                            {
                                return -1;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                });

                //foreach (var item in rightSideWalls)
                //{
                //    Debug.Log(item);
                //}

                {
                    List<ISideWall> sideWallGroup = new List<ISideWall>();

                    for (int i = 0; i < rightSideWalls.Count; i++)
                    {
                        ISideWall sideWall = rightSideWalls[i];

                        if (sideWallGroup.Count == 0)
                        {
                            // 开始 add by TangJian 2019/1/18 15:39
                            sideWallGroup.Add(sideWall);
                        }
                        else
                        {
                            if (sideWallGroup[sideWallGroup.Count - 1].LinkEndPos == sideWall.LinkBeginPos)
                            {
                                if (sideWall.IsBreak)
                                {
                                    Vector3 frontSortRendererPos = sideWall.GetFrontSortRendererPos();
                                    foreach (var item in sideWallGroup)
                                    {
                                        item.SetBackSortRendererPos(frontSortRendererPos);
                                    }
                                    sideWallGroup.Clear();
                                }

                                sideWallGroup.Add(sideWall);
                            }
                            else
                            {
                                // 打破连接 add by TangJian 2019/1/18 15:56
                                Vector3 backSortRendererPos = sideWallGroup[sideWallGroup.Count - 1].GetBackSortRendererPos();

                                foreach (var item in sideWallGroup)
                                {
                                    item.SetBackSortRendererPos(backSortRendererPos);
                                }
                                sideWallGroup.Clear();
                                sideWallGroup.Add(sideWall);
                            }
                        }

                        if (i == rightSideWalls.Count - 1)
                        {
                            Vector3 backSortRendererPos = sideWallGroup[sideWallGroup.Count - 1].GetBackSortRendererPos();

                            foreach (var item in sideWallGroup)
                            {
                                item.SetBackSortRendererPos(backSortRendererPos);
                            }
                        }
                    }
                }
            }


            EditorUtility.ClearProgressBar();
        }

        // 更新组件shader add by TangJian 2018/11/29 23:33
        public static IEnumerator Editor_UpdateRendererShader(this List<SceneComponent> target, bool displayProgress = true)
        {
            for (int i = 0; i < target.Count; i++)
            {
                SceneComponent sc = target[i];

                if (displayProgress)
                    EditorUtility.DisplayProgressBar("更新组件shader", "更新组件shader:" + sc.name, (float)i / target.Count);

                yield return null;
                sc.Editor_UpdateRendererShader();
            }
            EditorUtility.ClearProgressBar();
        }


        public static void Editor_UpdateRendererShader(this SceneComponent target)
        {
            //SceneComponent sceneComponent_ = target;
            //if ((sceneComponent_ as BackDoor) != null)
            //{
            //    sceneComponent_.GetRendererObject().RecursiveComponent<Renderer>((Renderer renderer_, int depth__) =>
            //    {
            //        Renderer renderer = renderer_;
            //        renderer.sharedMaterial.SetFloat("_ZWrite", 0);
            //        renderer.sharedMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            //        renderer.sharedMaterial.renderQueue = 3000;
            //    }, 1, 99);
            //}
            //else if ((sceneComponent_ as Ground) != null)
            //{
            //    sceneComponent_.GetRendererObject().RecursiveComponent<Renderer>((Renderer renderer_, int depth__) =>
            //    {
            //        Renderer renderer = renderer_;
            //        renderer.sharedMaterial.SetFloat("_ZWrite", 1);
            //        renderer.sharedMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            //        renderer.sharedMaterial.SetFloat("_TestFactor", 20);
            //        renderer.sharedMaterial.renderQueue = 2000;
            //    }, 1, 99);
            //}
            //else if ((sceneComponent_ as SceneCenterWallComponent) != null)
            //{
            //    sceneComponent_.GetRendererObject().RecursiveComponent<Renderer>((Renderer renderer_, int depth__) =>
            //    {
            //        renderer_.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Tang/SideSpriteRenderer.mat");
            //    }, 1, 99);
            //}
            //else if ((sceneComponent_ as SceneSideWallComponent) != null)
            //{
            //    sceneComponent_.GetRendererObject().RecursiveComponent<Renderer>((Renderer renderer_, int depth__) =>
            //    {
            //        Renderer renderer = renderer_;
            //        renderer.sharedMaterial.SetFloat("_ZWrite", 0);
            //        renderer.sharedMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            //        renderer.sharedMaterial.SetFloat("_TestFactor", 20);
            //        renderer.sharedMaterial.renderQueue = 3000;
            //    }, 1, 99);
            //}
            //else if ((sceneComponent_ as ScenePlacementComponent) != null)
            //{
            //    sceneComponent_.GetRendererObject().RecursiveComponent<Renderer>((Renderer renderer_, int depth__) =>
            //    {
            //        Renderer renderer = renderer_;
            //        if (renderer.sharedMaterial != null)
            //        {
            //            if (renderer_.GetComponent<Spine.Unity.SkeletonAnimation>() || renderer_.GetComponent<Spine.Unity.SkeletonAnimator>())
            //            {
            //                renderer.sharedMaterial.SetFloat("_ZWrite", 0);
            //                renderer.sharedMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            //                renderer.sharedMaterial.renderQueue = 3000;
            //            }
            //            else
            //            {
            //                renderer.sharedMaterial.SetFloat("_ZWrite", 0);
            //                renderer.sharedMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            //                renderer.sharedMaterial.renderQueue = 3000;
            //            }
            //        }
            //        else
            //        {
            //            Debug.LogWarning("没有ShareMaterial:" + ": " + "name = " + renderer_.name + "parentName = " + renderer_.transform.parent.name);
            //        }
            //    }, 1, 99);
            //}
            //else
            //{
            //    sceneComponent_.GetRendererObject().RecursiveComponent<Renderer>((Renderer renderer_, int depth__) =>
            //    {
            //        Renderer renderer = renderer_;
            //        if (renderer.sharedMaterial != null)
            //        {
            //            renderer.sharedMaterial.SetFloat("_ZWrite", 1);
            //            renderer.sharedMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            //            renderer.sharedMaterial.renderQueue = 2000;
            //        }
            //        else
            //        {
            //            Debug.LogWarning("没有ShareMaterial:" + ": " + "name = " + renderer_.name + "parentName = " + renderer_.transform.parent.name);
            //        }
            //    }, 1, 99);
            //}
        }

        // 构建组件的模型 add by TangJian 2018/11/7 19:26
        public static SceneComponent Editor_Build(this SceneComponent target)
        {
            string goPath = Tools.GetPrefabPath(target.gameObject, true);
            Tools.CreateFolder(goPath + "/Build");
            string path = goPath + "/Build/" + target.gameObject.name;

            target.CreateMesh(path);
            target.Editor_UpdateRendererSort();

            return target;
        }
#endif
    }
}