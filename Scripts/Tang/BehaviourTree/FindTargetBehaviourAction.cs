using UnityEngine;

namespace Tang
{
    public class FindTargetBehaviourAction : BehaviourAction
    {
        private RoleBehaviorTree roleBehaviorTree;
        public float distance = 8;
        public override void OnBegin()
        {
            roleBehaviorTree = GetComponent<RoleBehaviorTree>();
        }

        public override TaskState OnUpdate()
        {
            if (roleBehaviorTree.enemys.Count >0)
            {
                roleBehaviorTree.TargetController = roleBehaviorTree.enemys[0].GetComponent<RoleController>();

                float currentDis = Vector3.Distance(roleBehaviorTree.TargetController.transform.position,
                    roleBehaviorTree.SelfController.transform.position);

                if (currentDis > distance)
                {
                    Debug.Log("有敌人，但距离大于"+distance);
                    return TaskState.Failure;
                }
                else
                {
                    return TaskState.Success;
                }
            }
            else
            {
                Debug.Log("无敌人...");
                return TaskState.Failure;
            }
        }

        
    }
}