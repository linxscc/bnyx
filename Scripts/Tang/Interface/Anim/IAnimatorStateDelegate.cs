using UnityEngine;

namespace Tang
{
    public enum AnimatorStateEventType
    {
        OnStateEnter,
        OnStateUpdate,
        OnStateExit,
        OnStateMove,
        OnStateIK
    }
    
    public interface IAnimatorStateDelegate
    {
        void OnStateEvent(string stateName, AnimatorStateEventType eventType, Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex, float time);
    }
}