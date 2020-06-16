using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using Tang;
   
    public class AttackAngle : Action
    {
        
        private RoleController selfController;
        private RoleBehaviorTree roleBehaviorTree;

        public float originDuration = 1f;

        private float duration;
        private float elapsed = 0;
        
        public override void OnStart()
        {
            selfController = gameObject.GetComponent<RoleController>();
            roleBehaviorTree = selfController._roleBehaviorTree;

            duration = originDuration;
            elapsed = 0;
        }

        public override TaskStatus OnUpdate()
        {
            elapsed += Time.deltaTime;
            
            CalculateAngle();
            selfController.SetWorldPointPosition(roleBehaviorTree.TargetController.transform.position);

            if (elapsed < duration)
            {
                return TaskStatus.Running;
            }
            else
            {
                return TaskStatus.Success;
            }
        }

        

        private void CalculateAngle()
        {
            var quaternion = Quaternion.FromToRotation(roleBehaviorTree.TargetController.transform.position - transform.position,
                transform.position + Vector3.right);
            float angle;
           
            if (quaternion.eulerAngles.y>15&& quaternion.eulerAngles.y<180)
            {
                angle = 1;
            }else if (quaternion.eulerAngles.y>180&& quaternion.eulerAngles.y<345)
            {
                angle = 0;
            }else if (quaternion.eulerAngles.y<=15)
            {
                angle = ((float)(0.5 / 15)) * quaternion.eulerAngles.y+0.5f;
            }
            else
            {
                angle = 0.5f- ((float) (0.5 / 15)) * (360- quaternion.eulerAngles.y);
            }
            
           selfController.RoleAnimator.SetFloat("attack_state",angle);
        }

               
    }
    
    
   
}

