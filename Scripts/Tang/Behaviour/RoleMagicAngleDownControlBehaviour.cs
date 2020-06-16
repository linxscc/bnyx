using UnityEngine;
using System.ComponentModel;
using System.Linq;

namespace Tang
{
    public class RoleMagicAngleDownControlBehaviour : RoleBaseStateMachineBehaviour
    {
        public float animspeed = 1f;
        public float Angle = 45f;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            lazyInit(animator);

            animator.speed = 0;
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            float currAngle = animator.GetFloat("MagicAngle");
            float control = animator.GetFloat("relative_speed_y");
            float angleMax = 45;
            
//            currAngle += (control > 0 ? 1 : -1) * (Time.deltaTime * angleMax * 0.6f).Range(-20, 20);
            if (control > 0)
            {
                currAngle += (Time.deltaTime * angleMax / 0.3f).Range(-45, 45);
            }
            else if (control < 0)
            {
                currAngle -= (Time.deltaTime * angleMax / 0.3f).Range(-45, 45);
            }
            else
            {
            }
            
            
            animator.SetFloat("MagicAngle", currAngle.Range(-45,45));

            if (currAngle <= 0)
            {
                var currNormalizeTime = (-currAngle / angleMax).Range(0, 1);
                animator.Play("Action-A-Down", 0, currNormalizeTime);
            }
            else
            {
            }
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            animator.speed = 1f;
        }
    }
}
