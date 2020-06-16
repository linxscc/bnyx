using UnityEngine;

namespace Tang
{
    public class HumanMainHandAttackBehaviour : RoleBaseStateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.speed = RoleController.AtkSpeed;

            RoleController.RoleData.FinalPoise = Mathf.Max(0.8f * RoleController.RoleData.FinalPoiseMax, RoleController.RoleData.FinalPoise);

            RoleController.currRoleAttackType = RoleAttackType.MainHand;
        }

        // // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        // public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     base.OnStateUpdate(animator, stateInfo, layerIndex);
        // }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            animator.speed = RoleController.DefaultAnimSpeed;
        }

        // // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        // public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     base.OnStateMove(animator, stateInfo, layerIndex);
        // }

        // // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        // public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     base.OnStateIK(animator, stateInfo, layerIndex);
        // }
    }
}