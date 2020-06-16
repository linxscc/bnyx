namespace Tang
{
    public class BehaviourSelectorEvaluator :BehaviourParentTask
    {
        private int stored = -1;
        public override int CurrentChildIndex()
        {
            return childIndex;
        }

        public override bool CanExecute()
        {
            return childIndex < childList.Count;
        }

        public override void OnChildExecuted(TaskState childStatus)
        {
            childIndex++;
        }

        public bool HasLastChild()
        {
            return childIndex >= childList.Count-1;
        }

        public override void OnReset()
        {
            childIndex = 0;
            executionStatus = TaskState.Inactive;
        }
    }
}