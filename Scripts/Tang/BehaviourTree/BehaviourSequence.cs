using System;
using System.Collections.Generic;

namespace Tang
{
    public class BehaviourSequence : BehaviourParentTask
    {
        public override int CurrentChildIndex()
        {
            return childIndex;
        }

        public override bool CanExecute()
        {
            return childIndex < childList.Count && executionStatus != TaskState.Failure;
        }

        public override void OnChildExecuted(TaskState childStatus)
        {
            if (childStatus == TaskState.Failure) executionStatus = TaskState.Failure;
            childIndex++;
        }

        public override void OnReset()
        {
            childIndex = 0;
            executionStatus = TaskState.Inactive;
        }
    }
}