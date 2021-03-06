﻿using UnityEngine;

namespace Tang
{
    public class RoleClimbCenterOverBehaviour : RoleBaseStateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            //RoleController.IsHovering = true;
            RoleController.CollisionState = RoleCollisionState.Climb;
            RoleController.gameObject.layer = LayerMask.NameToLayer("Default");
            RoleController.canClimbLadder = false;
        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            RoleController.Closejock();
            //RoleController.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            RoleController.transform.position = new Vector3(RoleController.transform.position.x, RoleController.FirstClimbPos.y, RoleController.FirstClimbPos.z);
            RoleController.Closejock();
            //RoleController.IsHovering = false;
            RoleController.CollisionState = RoleCollisionState.Default;
            RoleController.gameObject.layer = LayerMask.NameToLayer("Role");
            RoleController.canClimbLadder = true;
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
