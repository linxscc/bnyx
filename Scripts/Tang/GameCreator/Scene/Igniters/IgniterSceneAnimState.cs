using System;
using GameCreator.Core;
using Tang;
using UnityEngine;

namespace GameCreator.Scene
{
    public class IgniterSceneAnimState : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Scene/AnimState";
#endif
    
        
        private ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
        private void Start()
        {
            var Joy = transform.parent.parent;
            if (!Joy) return;

            var Anim = Joy.GetComponentInChildren<Animator>();
            if (!Anim) return;
            
            valueMonitorPool.Clear();
            valueMonitorPool.AddMonitor(() => Anim.GetCurrAnimNameHash(), (from, to) =>
            {
                this.ExecuteTrigger(Joy.gameObject);
            });

        }
        
        private void Update()
        {
            valueMonitorPool.Update();
        }
    }
}