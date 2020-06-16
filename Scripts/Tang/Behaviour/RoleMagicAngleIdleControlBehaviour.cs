using UnityEngine;

namespace Tang
{
    public class RoleMagicAngleIdleControlBehaviour : RoleBaseStateMachineBehaviour
    {//重置MagicAngle数值
        public float animspeed = 1f;
        public float Angle = 45f;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            lazyInit(animator);
            
            animator.SetFloat("MagicAngle", 0);
//            animator.SetFloat("storage_time", 0);
        }
//       
//        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//        {
//            base.OnStateUpdate(animator, stateInfo, layerIndex);
//        }
//        
//        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//        {
//            base.OnStateExit(animator, stateInfo, layerIndex);
//        }
    }
}
