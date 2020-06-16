using UnityEngine;

namespace Tang
{
    public class RoleMoveByAnimStateMachineBehaviour : StateMachineBehaviour
    {
        protected RoleController roleController;
        protected Vector3 lastRootPosition;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (roleController == null)
            {
                roleController = animator.GetComponentInParent<RoleController>();
            }

            // 初始化
            lastRootPosition = Vector3.zero;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Debug.Log("HumanMoveByAnimBehaviour OnStateUpdate");
            AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);
            bool hasNext = nextStateInfo.fullPathHash != 0;

            var skeletonAnimator = roleController.SkeletonAnimator;
            var roleAnimController = roleController.RoleAnimController;
            foreach (var bone in skeletonAnimator.skeleton.Bones)
            {
                if (bone.Data.name == "root")
                {
                    Vector3 moveByPos = new Vector3(-bone.X, 0, 0);
                    roleAnimController.transform.localPosition = roleController.GetDirectionInt() > 0 ? moveByPos : -moveByPos;
                    if (hasNext)
                    {
                    }
                    else
                    {
                        var movePosition = new Vector3();
                        if (lastRootPosition == null)
                        {
                            lastRootPosition = moveByPos;
                        }
                        else
                        {
                            movePosition = moveByPos - lastRootPosition;
                            lastRootPosition = moveByPos;

                            movePosition.x *= -roleController.GetDirectionInt();
                        }

                        Debug.Log("movePosition = " + movePosition);

                        if (movePosition.magnitude > 0.001)
                        {
                            roleController.CharacterController.Move(movePosition);
                        }
                    }
                    break;
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

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

