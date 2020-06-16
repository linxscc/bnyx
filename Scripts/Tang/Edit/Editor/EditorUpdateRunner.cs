



using System;
using System.Collections.Generic;
using UnityEditor;

namespace Tang.Editor
{
    public static class EditorUpdateRunner
    {

        static Dictionary<string, Action> actionDic = new Dictionary<string, Action>();

        static bool Inited = false;
        static void LazyInit()
        {
            if (Inited == false)
            {
                Inited = true;
                EditorApplication.update += Update;
            }
        }

        public static bool ContainsKey(string id)
        {
            LazyInit();

            return actionDic.ContainsKey(id);
        }

        public static void AddUpdate(string id, Action action)
        {
            LazyInit();

            actionDic.Add(id, action);
        }

        public static void AddUpdateIfNot(string id, Action action)
        {
            LazyInit();

            if (ContainsKey(id) == false)
                AddUpdate(id, action);
        }

        public static void RemoveUpdate(string id)
        {
            LazyInit();

            actionDic.Remove(id);
        }

        private static void Update()
        {
            LazyInit();

            foreach (var item in actionDic)
            {
                item.Value();
            }
        }
    }
}