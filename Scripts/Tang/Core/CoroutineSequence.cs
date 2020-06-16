

using UnityEngine;
using System.Collections.Generic;
using System.Collections;



namespace Tang
{
    public class CoroutineSequence
    {
        public class Coroutine
        {
            public string id;
            public IEnumerator enumerator;
            int stepTimes = 1;

            public Coroutine(string id, IEnumerator enumerator, int stepTimes = 1)
            {
                this.id = id;
                this.enumerator = enumerator;
                this.stepTimes = stepTimes > 0 ? stepTimes : 1;

                Debug.Assert(this.id != null);
                Debug.Assert(this.enumerator != null);
            }

            public int StepTimes { get { return stepTimes; } }

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

        public void AddCoroutine(string id, IEnumerator enumerator, int stepTimes = 1)
        {
            coroutineList.Add(new Coroutine(id, enumerator, stepTimes));
        }

        public void RemoveCoroutine(string id)
        {
            coroutineList.RemoveAll((Coroutine c) =>
            {
                return c.id == id;
            });
        }

        public void CallStep()
        {
            if (coroutineList.Count > 0)
            {
                Coroutine coroutine = coroutineList[0];
                for (int i = 0; i < coroutine.StepTimes; i++)
                {
                    if (coroutine.MoveNext())
                    {
                    }
                    else
                    {
                        coroutineList.RemoveAt(0);
                        break;
                    }
                }
            }
        }
    }
}