




using System.Collections;
using UnityEditor;

namespace Tang.Editor
{
    public static class EditorCoroutineSequenceRunner
    {

        public static CoroutineSequence coroutineQueue;

        static bool Inited = false;
        static void LazyInit()
        {
            if (Inited == false)
            {
                Inited = true;

                coroutineQueue = new CoroutineSequence();

                EditorApplication.update += Update;
            }
        }

        public static int FindCoroutine(string id)
        {
            LazyInit();

            return coroutineQueue.FindCoroutine(id);
        }

        public static void AddCoroutine(string id, IEnumerator enumerator, int stepTimes = 1)
        {
            LazyInit();

            coroutineQueue.AddCoroutine(id, enumerator, stepTimes);
        }

        public static void AddCoroutine(IEnumerator enumerator, int stepTimes = 1)
        {
            LazyInit();

            string id = Tools.getOnlyId().ToString();
            coroutineQueue.AddCoroutine(id, enumerator, stepTimes);
        }

        public static void AddCoroutineIfNot(string id, IEnumerator enumerator, int stepTimes = 1)
        {
            LazyInit();

            if (coroutineQueue.FindCoroutine(id) < 0)
                coroutineQueue.AddCoroutine(id, enumerator, stepTimes);
        }

        public static void RemoveCoroutine(string id)
        {
            LazyInit();

            coroutineQueue.RemoveCoroutine(id);
        }

        private static void Update()
        {
            LazyInit();

            coroutineQueue.CallStep();
        }
    }
}