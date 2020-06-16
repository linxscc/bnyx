using System;

namespace Tang
{
    public class BehaviourSelector :BehaviourParentTask
    {
        public override int CurrentChildIndex()
        {
            return childIndex;
        }

        public override bool CanExecute()
        {
            return childIndex < childList.Count  && executionStatus != TaskState.Success;
        }
        
        public override void OnChildExecuted(TaskState childStatus)
        {
            if (TaskState.Success == childStatus || TaskState.Running == childStatus)
            {
                executionStatus = childStatus;
                return;
            }
            childIndex++;
        }

        public override void OnReset()
        {
            childIndex = 0;
            executionStatus = TaskState.Inactive;
        }
        
        public bool HasLastChild()
        {
            return childIndex >= childList.Count-1;
        }
    }
}