




using System.Collections;
using UnityEditor;

namespace Tang.Editor
{
    public static class EditorCoroutineParallelRunner
    {

        public static CoroutineParallel coroutineParallel;

        static bool Inited = false;
        static void LazyInit()
        {
            if (Inited == false)
            {
                Inited = true;

                coroutineParallel = new CoroutineParallel();

                EditorApplication.update += Update;
            }
        }

        public static int FindCoroutine(string id)
        {
            LazyInit();

            return coroutineParallel.FindCoroutine(id);
        }

        public static void AddCoroutine(string id, IEnumerator enumerator)
        {
            LazyInit();

            coroutineParallel.AddCoroutine(id, enumerator);
        }

        public static void AddCoroutineIfNot(string id, IEnumerator enumerator)
        {
            LazyInit();

            if (coroutineParallel.FindCoroutine(id) < 0)
                coroutineParallel.AddCoroutine(id, enumerator);
        }

        public static void RemoveCoroutine(string id)
        {
            LazyInit();

            coroutineParallel.RemoveCoroutine(id);
        }

        private static void Update()
        {
            LazyInit();

            coroutineParallel.CallStep();
        }
    }
}