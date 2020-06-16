using System;
using System.Collections.Generic;
using UnityEngine;


namespace Tang
{
    public class BehaviourTree : MonoBehaviour
    {
        public ITask TaskRoot;

        public void AddRoot(ITask task)
        {
            TaskRoot = task;
            GiveValue(TaskRoot);
        }

        private void GiveValue(ITask taskRoot)
        {
            switch (taskRoot)
            {
                case BehaviourParentTask parentTask:
                    foreach (var task in parentTask.childList)
                    {
                        GiveValue(task);    
                    }
                    break;
                case BehaviourTask behaviourTask:
                    behaviourTask.gameObject = gameObject;
                    break;
                default:
                    throw new Exception(taskRoot  + " is Null");
            }
        }
        
        private void Update()
        {
            RunTask(TaskRoot);
        }

        private TaskState RunTask(ITask task)
        {
            switch (task)
            {
                case null:
                    return TaskState.Failure;
                case BehaviourParentTask parentTask:
                    return RunParentTask(parentTask);
                case BehaviourTask behaviourTask:
                    return RunResultState(behaviourTask);
                default:
                    throw new Exception("UnKnow: "+task);
            }
        }
        private TaskState RunParentTask(BehaviourParentTask parentTask)
        {
            switch (parentTask)
            {
                case BehaviourWeightSelector weightSelector:
                    return RunWeightSelector(weightSelector);
                case BehaviourSelectorEvaluator selectorEvaluator:
                    return RunSelectorEvaluator(selectorEvaluator);
                case BehaviourSelector selector:
                    return RunSelector(selector);
                case BehaviourSequence sequence:
                    return RunSequence(sequence);
                default:
                    throw new Exception("Unknown "+ parentTask +" Precess Mode");
            } 
        } 
        
        
        private TaskState RunResultState(BehaviourTask behaviourTask)
        {
            if (behaviourTask.BehaviourState == BehaviourState.End)
            {
                behaviourTask.OnBegin();
                behaviourTask.BehaviourState = BehaviourState.Running;
            }
            var result = behaviourTask.OnUpdate();
                
            if (result == TaskState.Success || result == TaskState.Failure)
            {
                behaviourTask.BehaviourState = BehaviourState.End;
                behaviourTask.OnEnd();
            }

            return result;
        }

        private TaskState RunWeightSelector(BehaviourWeightSelector parentTask)
        {
            if (parentTask.executionStatus == TaskState.Inactive)
            {
                parentTask.OnBegin();
                parentTask.executionStatus = TaskState.Running;    
            }
            if (parentTask.CanExecute())
            {
                TaskState state = RunTask(parentTask.GetChild(parentTask.CurrentChildIndex()));
                if (state == TaskState.Success ||state == TaskState.Failure)
                {
                    parentTask.OnChildExecuted(state);
                }
                return state;
            }
            parentTask.OnReset();
            return TaskState.Failure;
        }

        private TaskState RunSelectorEvaluator(BehaviourSelectorEvaluator parentTask)
        {
            if (parentTask.CanExecute())
            {
                TaskState state = RunTask(parentTask.GetChild(parentTask.CurrentChildIndex()));
                if (parentTask.HasLastChild()) state = TaskState.Running;
                if (state == TaskState.Failure)
                {
                    parentTask.OnChildExecuted(state);
                    state = RunParentTask(parentTask);
                }
                parentTask.OnReset();
                return state;    
            }
            parentTask.OnReset();
            return TaskState.Failure;
        }

        private TaskState RunSelector(BehaviourSelector parentTask)
        {
            if (parentTask.CanExecute())
            {
                TaskState state = RunTask(parentTask.GetChild(parentTask.CurrentChildIndex()));
                if (parentTask.HasLastChild())
                {
                    if (state != TaskState.Running)
                    {
                        state = TaskState.Success;    
                    }
                }
                parentTask.OnChildExecuted(state);
                
                if (state == TaskState.Failure)
                {
                    state = RunParentTask(parentTask);
                }
                return state;
            }

            parentTask.OnReset();
            return TaskState.Failure;
        }
        
        private TaskState RunSequence(BehaviourParentTask parentTask)
        {
            if (parentTask.CanExecute())
            {
                TaskState state = RunTask(parentTask.GetChild(parentTask.CurrentChildIndex()));
                if (state == TaskState.Success ||state == TaskState.Failure)
                {
                    parentTask.OnChildExecuted(state);
                }
                return state;
            }
            parentTask.OnReset();
            return TaskState.Failure;
        }
    }
    
    
    
}