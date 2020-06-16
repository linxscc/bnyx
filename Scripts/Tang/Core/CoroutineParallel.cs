

using UnityEngine;
using System.Collections.Generic;
using System.Collections;



namespace Tang
{
    public class CoroutineParallel
    {
        public class Coroutine
        {
            public string id;
            public IEnumerator enumerator;

            public Coroutine(string id, IEnumerator enumerator)
            {
                this.id = id;
                this.enumerator = enumerator;

                Debug.Assert(this.id != null);
                Debug.Assert(this.enumerator != null);
            }

            public object Current { get { return enumerator.Current; } }

            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                enumerator.Reset();
            }
        }

        public List<Coroutine> coroutineList = new List<Coroutine>();

        public int FindCoroutine(string id)
        {
            return coroutineList.FindIndex((Coroutine c) =>
            {
                return c.id == id;
            });
        }

        public void AddCoroutine(string id, IEnumerator enumerator)
        {
            coroutineList.Add(new Coroutine(id, enumerator));
        }

        public void RemoveCoroutine(string id)
        {
            coroutineList.RemoveAll((Coroutine c) =>
            {
                return c.id == id;
            });
        }

        List<int> removeList = new List<int>();

        public void CallStep()
        {
            if (coroutineList.Count > 0)
            {
                for (int i = 0; i < coroutineList.Count; i++)
                {
                    Coroutine coroutine = coroutineList[i];
                    if (coroutine.MoveNext())
                    {
                    }
                    else
                    {
                        removeList.Add(i);
                    }
                }

                if (removeList.Count > 0)
                {
                    for (int i = removeList.Count - 1; i >= 0; i--)
                    {
                        coroutineList.RemoveAt(i);
                    }
                    removeList.Clear();
                }
            }
        }
    }
}