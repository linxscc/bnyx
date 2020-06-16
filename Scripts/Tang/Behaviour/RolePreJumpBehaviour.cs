using UnityEngine;

namespace Tang
{
    public class RolePreJumpBehaviour : RoleBaseStateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            RoleController.Speed = new Vector3(RoleController.Speed.x, RoleController.JumpSpeed, RoleController.Speed.z);
            if (RoleController.IsGrounded())
            {
                RoleController.currjumpTimes = 0;
                if (RoleController.animatorParams.ContainsKey("no_jump"))
                {
                    animator.SetBool("no_jump", false);
                }
            }
            //
            RoleController.currjumpTimes++;
            if(RoleController.currjumpTimes>= RoleController.jumpTimes)
            {
                if (RoleController.animatorParams.ContainsKey("no_jump"))
                {
                    animator.SetBool("no_jump", true);
                }
            }

            #region //二段跳 2019.3.29

            animator.SetInteger("curr_jump_times", RoleController.currjumpTimes);
            #endregion
           
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            //RoleController.Speed = new Vector3(RoleController.Speed.x, RoleController.JumpSpeed, RoleController.Speed.z);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     base.OnStateUpdate(animator, stateInfo, layerIndex);

        //     RoleController.Speed = new Vector3(RoleController.Speed.x, RoleController.jumpSpeed, RoleController.Speed.z);
        // }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     // base.OnStateExit(animator, stateInfo, layerIndex);

        //     // if (roleController == null)
        //     // {
        //     //     roleController = animator.GetComponentInParent<Tang.RoleController>();
        //     // }
        //     // if (roleController != null)
        //     // {
        //     //     roleController.Speed = new Vector3(roleController.Speed.x, roleController.jumpSpeed, roleController.Speed.z);
        //     // }
        // }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}