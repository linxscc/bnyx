using System;
using GameCreator.Core;
using Tang;

namespace GameCreator.Scene
{
    using UnityEngine;
    

    [AddComponentMenu("")]
    public class IgniterSceneInterval : Igniter 
    {
#if UNITY_EDITOR
        public new static string NAME = "Scene/间隔触发";
#endif
        public float interval = 1;
        public float intervalFrom = 0;
        public float intervalTo = 0;
        private float tmpInterval = 1;

        private void Start()
        {
            Reset();
        }

        private void OnDestroy()
        {
           
        }

        private void Update() {
            tmpInterval -= Time.deltaTime;
            if(tmpInterval <= 0)
            {
                Reset();
                this.ExecuteTrigger(gameObject);
            }
        }

        private void Reset()
        {
            tmpInterval = interval + Random.Range(intervalFrom, intervalTo);
        }
    }
}