using System;
using UnityEngine;

namespace Tang
{
    public enum TaskState
    {
        Inactive,
        Failure,
        Success,
        Running
    }

    public interface ITask
    {
        void OnBegin();
        TaskState OnUpdate();
        void OnEnd();

        void OnReset();
    }

    public class BehaviourTask : ITask
    {
        public BehaviourState BehaviourState = BehaviourState.End;
        public GameObject gameObject;
    
        public virtual void OnBegin()
        {
        }
        
        public virtual TaskState OnUpdate()
        {
            return TaskState.Success;
        }

        public virtual void OnEnd()
        {
        }

        public virtual void OnReset()
        {
        }
        
        protected T GetComponent<T>() where T : Component
        {
            return this.gameObject.GetComponent<T>();
        }
    }

    public enum BehaviourState
    {
        Running = 0,
        End = 1
        
    }
}