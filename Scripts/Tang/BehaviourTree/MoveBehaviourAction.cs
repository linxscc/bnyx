using System.Collections;
using UnityEngine;

namespace Tang
{
    public class MoveBehaviourAction : BehaviourAction
    {
        private RoleBehaviorTree roleBehaviorTree;
        private IEnumerator ieEnumerator;
        
        public override void OnBegin()
        {
            roleBehaviorTree = GetComponent<RoleBehaviorTree>();
            if (roleBehaviorTree.enemys.Count <= 0) return;
            roleBehaviorTree.TargetController = roleBehaviorTree.enemys[0].GetComponent<RoleController>();
            
            ieEnumerator = roleBehaviorTree.MovePath(roleBehaviorTree.TargetController.transform.position,
                roleBehaviorTree.TargetController.GetSpeedByMoveState(MoveState.Walk));
        }

        public override TaskState OnUpdate()
        {
            if (ieEnumerator.MoveNext())
            {
                Debug.Log("Moving...");
                return (TaskState)ieEnumerator.Current;
            }
            else
            {
                return TaskState.Success;
            }
        }
        
    }
}