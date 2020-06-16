




using System.Collections;
using UnityEditor;

namespace Tang.Editor
{
    public static class EditorCoroutineRunner
    {

        public static CoroutinePool coroutinePool;

        static bool Inited = false;
        static void LazyInit()
        {
            if (Inited == false)
            {
                Inited = true;

                coroutinePool = new CoroutinePool();

                EditorApplication.update += Update;
            }
        }

        public static int FindCoroutine(string id)
        {
            LazyInit();

            return coroutinePool.FindCoroutine(id);
        }

        public static void AddCoroutine(string id, IEnumerator enumerator)
        {
            LazyInit();

            coroutinePool.AddCoroutine(id, enumerator);
        }

        public static void AddCoroutineIfNot(string id, IEnumerator enumerator)
        {
            LazyInit();

            if (coroutinePool.FindCoroutine(id) < 0)
                coroutinePool.AddCoroutine(id, enumerator);
        }

        public static void RemoveCoroutine(string id)
        {
            LazyInit();

            coroutinePool.RemoveCoroutine(id);
        }

        private static void Update()
        {
            LazyInit();

            coroutinePool.CallStep();
        }
    }
}