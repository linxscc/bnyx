using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tang
{
    public class WaitForSeconds
    {
        public WaitForSeconds(float seconds)
        {
            this.seconds = seconds;
        }
        private float seconds = 0;
        public float Seconds { get { return seconds; } set { seconds = value; } }
    }

    public class Coroutine
    {
        public Coroutine(IEnumerator ienumerator)
        {
            this.ienumerator = ienumerator;
        }
        private IEnumerator ienumerator;
        private float waitTime = 0;
        private bool canMoveNext = true;

        public bool Finished { get { return canMoveNext == false; } }

        public void Update()
        {
            waitTime -= Time.deltaTime;
            if (canMoveNext && waitTime <= 0)
            {
                canMoveNext = ienumerator.MoveNext();
                WaitForSeconds waitForSeconds = ienumerator.Current as WaitForSeconds;
                if (waitForSeconds != null)
                {
                    waitTime = waitForSeconds.Seconds;
                }
            }
        }
    }

    public class CoroutineController
    {
        private Dictionary<string, Coroutine> coroutineDic = new Dictionary<string, Coroutine>();
        private List<string> needRemoveList = new List<string>();
    
        public void AddCoroutine(string id, Coroutine coroutine)
        {
            RemoveCoroutine(id);
            coroutineDic.Add(id, coroutine);
        }

        public bool RemoveCoroutine(string id)
        {
            if (coroutineDic.ContainsKey(id))
                return coroutineDic.Remove(id);
            return false;
        }

        public bool HasCoroutine(string id)
        {
            return coroutineDic.ContainsKey(id);
        }

        public void Update()
        {
            needRemoveList.Clear();

            List<Coroutine> coroutineList = new List<Coroutine>();
            foreach (var coroutine in coroutineDic)
            {
                coroutineList.Add(coroutine.Value);
                if (coroutine.Value.Finished)
                    needRemoveList.Add(coroutine.Key);
            }

            foreach (var coroutine in coroutineList)
            {
                coroutine.Update();
            }

            foreach (var item in needRemoveList)
            {
                RemoveCoroutine(item);
            }
        }
    }

    public static class MyMonoBehaviourFunctions
    {
        public static IEnumerator DelayFunc(System.Action action, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            action();
        }
    }

    public class MyMonoBehaviour : MonoBehaviour
    {
        private Transform _transform;

        public Transform MTransform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = transform;
                }

                return _transform;
            }
        }

        [SerializeField] private bool pause = false;
        public bool Pause { get { return pause; } set { pause = value; } }
        //public string DamageOnlyID;
        CoroutineController coroutineController = new CoroutineController();
        CoroutineController CoroutineController
        {
            get
            {
                if (coroutineController == null)
                {
                    coroutineController = new CoroutineController();
                }
                return coroutineController;
            }
        }

        public void DelayFunc(string id, Action action, float delaySeconds)
        {
            StartCoroutine(id, MyMonoBehaviourFunctions.DelayFunc(action, delaySeconds));
        }

        public void DelayFunc(Action action, float delaySeconds)
        {
            if (delaySeconds <= 0)
            {
                action();
                return;
            }
            StartCoroutine(Tools.getOnlyId().ToString(), MyMonoBehaviourFunctions.DelayFunc(action, delaySeconds));
        }

        public void StartCoroutine(string id, IEnumerator routine)
        {
            coroutineController.AddCoroutine(id, new Coroutine(routine));
        }

        public bool HasCoroutine(string id)
        {
            return coroutineController.HasCoroutine(id);
        }

        public void RemoveCoroutine(string id)
        {
            coroutineController.RemoveCoroutine(id);
        }

        public virtual void Update()
        {
            if (Pause)
            {

            }
            else
            {
                CoroutineController.Update();
                OnUpdate();
            }
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void FixedUpdate()
        {
            if (Pause)
            {

            }
            else
            {
                OnFixedUpdate();
            }
        }

        public virtual void OnFixedUpdate()
        {
        }
    }
}