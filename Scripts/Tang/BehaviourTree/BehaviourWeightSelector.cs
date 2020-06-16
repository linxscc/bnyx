using System.Collections.Generic;

namespace Tang
{
    public class BehaviourWeightSelector : BehaviourParentTask
    {
        private List<int> weightList = new List<int>();
        private int currentChildIndex = 0;

        private bool currentExecute = true;
        public override void OnBegin()
        {
            currentChildIndex = Tang.Tools.RandomWithWeight<int>(weightList, (int curr, int index) => curr);
        }
        
        public override int CurrentChildIndex()
        {
            return currentChildIndex;
        }

        public override bool CanExecute()
        {
            return childIndex < childList.Count && currentExecute && executionStatus != TaskState.Failure;
        }
        
        public override void OnChildExecuted(TaskState childStatus)
        {
            currentExecute = false;
            executionStatus = childStatus;
        }
        
        public override void OnReset()
        {
            executionStatus = TaskState.Inactive;
            currentChildIndex = 0;
            currentExecute = true;
        }

        public void AddWeight(int weight)
        {
            weightList.Add(weight);
        }
        
    }
}