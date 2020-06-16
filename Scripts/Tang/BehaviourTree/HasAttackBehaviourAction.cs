using UnityEngine;

namespace Tang
{
    public class HasAttackBehaviourAction : BehaviourAction
    {
        private RoleBehaviorTree roleBehaviorTree;

        private float Dis = 5;
        public override void OnBegin()
        {
            roleBehaviorTree = GetComponent<RoleBehaviorTree>();
            roleBehaviorTree.TargetController = roleBehaviorTree.enemys[0].GetComponent<RoleController>();
        }

        public override TaskState OnUpdate()
        {
            var dis = Vector3.Distance(roleBehaviorTree.SelfController.transform.position,roleBehaviorTree.TargetController.transform.position);

            if (dis < Dis)
            {
                Debug.Log("Attack");
                roleBehaviorTree.SelfController.Action1Begin();
            }
            return dis < Dis ? TaskState.Success : TaskState.Failure;
        }
    }
}