using System;
using System.Collections.Generic;
using UnityEngine;




namespace Tang
{
    public static class GameObjectExtend
    {
        public static Bounds GetRendererBounds(this GameObject target, int depthMin = 1, int depthMax = 99)
        {
            bool isInited = false;
            Bounds retBounds = new Bounds();// = new Bounds(target.transform.position, Vector3.zero);
            target.RecursiveComponent((Renderer renderer, int depth) =>
            {
                if (isInited == false)
                {
                    retBounds = renderer.bounds;
                    isInited = true;
                }
                else
                {
                    retBounds.Encapsulate(renderer.bounds);
                }
            }, depthMin, depthMax);

            return retBounds;
        }

        // 得到游戏对象碰撞区域大小 add by TangJian 2017/11/07 17:58:47
        public static Bounds GetColliderBounds(this GameObject target, List<string> ignoreList = null, int depthMin = 1, int depthMax = 99)
        {

            bool isInited = false;
            Bounds retBounds = new Bounds();
            target.RecursiveComponent((Collider collider, int depth) =>
            {
                if (ignoreList != null && ignoreList.Contains(collider.gameObject.name))
                {
                }
                else
                {
                    if (collider is MeshCollider)
                    {
                        MeshRenderer meshRenderer = collider.gameObject.GetComponent<MeshRenderer>();
                        // meshRenderer.enabled = false;
                        if (Application.isPlaying)
                        {
                            if (isInited == false)
                            {
                                retBounds = meshRenderer.bounds;
                                isInited = true;
                            }
                            else
                            {
                                retBounds.Encapsulate(meshRenderer.bounds);
                            }
                        }
                        else
                        {
                            if (isInited == false)
                            {
                                retBounds = collider.bounds;
                                isInited = true;
                            }
                            else
                            {
                                retBounds.Encapsulate(collider.bounds);
                            }
                        }
                    }
                    else
                    {
                        if (isInited == false)
                        {
                            retBounds = collider.bounds;
                            isInited = true;
                        }
                        else
                        {
                            retBounds.Encapsulate(collider.bounds);
                        }
                    }
                }
            }, depthMin, depthMax);
            return retBounds;
        }

        public static T AddComponentUnique<T>(this GameObject target, bool immediately = true) where T : Component
        {
            target.RecursiveComponent<T>((T needRemoveComponent, int depth) =>
            {
                if (immediately)
                {
                    UnityEngine.Object.DestroyImmediate(needRemoveComponent, true);
                }
                else
                {
                    Tools.Destroy(needRemoveComponent);
                }
            }, 1, 1);
            return target.AddComponent<T>();
        }

        public static T AddComponentIfNone<T>(this GameObject target) where T : Component
        {
            T t = target.GetComponent<T>();
            if (t == null)
            {
                t = target.AddComponent<T>();
            }
            return t;
        }

        public static GameObject GetChild(this GameObject target, string name, bool autoAdd = false)
        {
            GameObject ret = null;
            target.Recursive((GameObject go, int depth) =>
            {
                if (ret == null)
                {
                    if (go.name == name)
                    {
                        ret = go;
                    }
                }
            }, 2, 2);

            if (ret == null && autoAdd)
            {
                ret = new GameObject(name);
                ret.transform.parent = target.transform;
                ret.transform.localPosition = Vector3.zero;
            }

            return ret;
        }

        public static void DestoryChildren(this GameObject target)
        {
            target.CallChildren((GameObject go) =>
            {
                Tools.Destroy(go);
            });
        }

        public static void CallChildren(this GameObject target, Action<GameObject> action)
        {
            target.Recursive((GameObject go, int depth) =>
            {
                if (depth == 2)
                {
                    action(go);
                }
            }, 2, 2);
        }

        public static void Recursive(this GameObject target, Action<GameObject, int> action, int depthMin, int depthMax)
        {
            target.transform.Recursive((Transform transform, int depth) =>
           {
               action(transform.gameObject, depth);
           }, depthMin, depthMax);
        }

        public static void RecursiveComponent<T>(this GameObject target, Action<T, int> action, int depthMin, int depthMax) where T : Component
        {
            target.transform.RecursiveComponent<T>(action, depthMin, depthMax);
        }

        public static List<GameObject> GetChildren(this GameObject target)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            target.Recursive((GameObject go, int depth) =>
            {
                gameObjectList.Add(go);
            }, 2, 2);
            return gameObjectList;
        }

        public static List<T> GetComponentList<T>(this GameObject target, int depthMin = 0, int depthMax = 99) where T : Component
        {
            List<T> componentList = new List<T>();
            target.RecursiveComponent<T>((T go, int depth) =>
            {
                componentList.Add(go);
            }, depthMin, depthMax);
            return componentList;
        }
    }
}