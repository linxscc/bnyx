using System;
using System.Collections.Generic;
using UnityEngine;




namespace Tang
{
    public static class TransformExtend
    {
        public static Transform FindAndAutoAdd(this Transform target, string path)
        {
            path = Tools.GetStandardPath(path);
            Transform currTransform = target;

            List<string> strList = path.SplitRetainSeparator('/');
            string subPath = "";
            for (int i = 0; i < strList.Count; i++)
            {
                if (strList[i] == "/")
                {
                    Transform tf_ = target.Find(subPath);
                    if (tf_ == null)
                    {
                        string name = strList[i + 1];

                        GameObject go = new GameObject();
                        go.transform.parent = currTransform;
                        go.transform.localPosition = Vector3.zero;
                        go.name = name;

                        tf_ = go.transform;
                    }

                    currTransform = tf_;
                    subPath += strList[i];
                }
                else
                {
                    subPath += strList[i];
                }
            }

            Transform tf = target.Find(path);
            if (tf == null)
            {
                GameObject go = new GameObject();
                go.transform.parent = currTransform;
                go.transform.localPosition = Vector3.zero;
                go.name = strList[strList.Count - 1];

                tf = go.transform;
            }
            return tf;
        }
        public static Transform GetChild(this Transform transform, string name)
        {
            Transform ret = null;
            transform.Recursive((Transform tf, int depth) =>
            {
                if (tf.name == name)
                {
                    ret = tf;
                }
            }, 2, 2);
            return ret;
        }

        public static List<Transform> GetChidren(this Transform transform)
        {
            List<Transform> transformList = new List<Transform>();
            transform.Recursive((Transform tf, int depth) =>
            {
                transformList.Add(tf);
            }, 2, 2);
            return transformList;
        }
        public static List<Transform> GetChidrenLayer(this Transform transform, string layername)
        {
            List<Transform> transformList = new List<Transform>();
            transform.Recursive((Transform tf, int depth) =>
            {
                if (tf.gameObject.layer == LayerMask.NameToLayer(layername))
                {
                    transformList.Add(tf);
                }
            }, 2, 2);
            return transformList;
        }

        public static void RecursiveParent(this Transform target, Action<Transform, int> action, int depthMin, int depthMax)
        {
            RecursiveParent(target, action, 0, depthMin, depthMax);
        }

        private static void RecursiveParent(Transform target, Action<Transform, int> action, int depth, int depthMin, int depthMax)
        {
            depth++;

            if (target != null)
            {
                if (depth <= depthMax)
                {
                    RecursiveParent(target.parent, action, depth, depthMin, depthMax);
                }

                if (depth - 1 >= depthMin && depth - 1 <= depthMax)
                    action(target, depth);
            }

            depth--;
        }

        public static void Recursive(this Transform target, Action<Transform, int> action, int depthMin, int depthMax)
        {
            Recursive(target, action, 0, depthMin, depthMax);
        }

        public static void RecursiveComponent<T>(this Transform target, Action<T, int> action, int depthMin, int depthMax) where T : Component
        {
            target.Recursive((Transform tr, int depth) =>
            {
                foreach (var item in tr.GetComponents<T>())
                {
                    action(item, depth);
                }
            }, depthMin, depthMax);
        }

        private static void Recursive(Transform target, Action<Transform, int> action, int depth, int depthMin, int depthMax)
        {
            depth++;

            if (target != null)
            {
                if (depth < depthMax)
                {
                    List<Transform> childList = new List<Transform>();
                    for (int i = 0; i < target.childCount; i++)
                    {
                        childList.Add(target.GetChild(i));
                    }

                    foreach (var child in childList)
                    {
                        Recursive(child, action, depth, depthMin, depthMax);
                    }
                }

                if (depth >= depthMin && depth <= depthMax)
                    action(target, depth);
            }

            depth--;
        }
    }
}