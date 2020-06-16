using System.Collections.Generic;

namespace Tang
{
    public class BehaviourParentTask : BehaviourTask
    {
        public TaskState executionStatus = TaskState.Inactive;
        public List<BehaviourTask> childList = new List<BehaviourTask>();
        public int childIndex = 0;
        public virtual void AddChild(BehaviourTask task)
        {
            childList.Add(task);
        }
        
        public virtual int CurrentChildIndex()
        {
            return childIndex;
        }

        public virtual bool CanExecute()
        {
            return true;
        }

        public virtual void OnChildExecuted(TaskState childStatus)
        {
            
        }
        
        public BehaviourTask GetChild(int index)
        {
            return childList[index];
        }

        public virtual bool OnReevaluationStarted()
        {
            return true;
        }
        public virtual void OnReevaluationEnded()
        {
            
        }
    }
}