using UnityEngine;

namespace Tang
{
    public class RoleClimbLadderBehaviour : RoleBaseStateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        Vector3 speed = new Vector3(0, 2f, 0);
        Vector3 speede = new Vector3(0, -2f, 0);
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            lazyInit(animator);
            RoleController.CollisionState = RoleCollisionState.Climb;
            RoleController.gameObject.layer= LayerMask.NameToLayer("Default");
            animator.speed = 0f;
            animator.StartRecording(0);
        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            RoleController.gameObject.layer = LayerMask.NameToLayer("Default");
            if (RoleController.Joystick.y == 0)
            {
                animator.speed = 0f;
                animator.StopRecording();
            }
            else if(RoleController.Joystick.y > 0)
            {
                RoleController.SkeletonAnimator.SetBackwards(false);
                animator.speed = 1f;
                //stateInfo.speed = 1f;
                
                RoleController.Move(speed * Time.deltaTime);
            }
            else
            {
                RoleController.SkeletonAnimator.SetBackwards(true);
                animator.speed = 1f;
                
                RoleController.Move(speede * Time.deltaTime);
            }
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            RoleController.SkeletonAnimator.SetBackwards(false);
            lazyInit(animator);
            RoleController.gameObject.layer = LayerMask.NameToLayer("Role");
            RoleController.CollisionState = RoleCollisionState.Default;
            animator.speed = 1f;
            animator.StopRecording();
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
