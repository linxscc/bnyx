using System;
using UnityEngine;

namespace Tang.Animation
{
    public class AnimatorStateTransmit : MonoBehaviour, IAnimatorStateDelegate
    {
        public event Action<string, AnimatorStateEventType, Animator, AnimatorStateInfo,
            int, float> OnStateEvents;

        public void OnStateEvent(string stateName, AnimatorStateEventType eventType, Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex, float time)
        {
            if (OnStateEvents != null)
            {
                OnStateEvents(stateName, eventType, animator, stateInfo, layerIndex, time);
            }
        }
    }
}