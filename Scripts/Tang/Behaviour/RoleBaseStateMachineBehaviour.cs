using UnityEngine;

namespace Tang
{
    public class RoleBaseStateMachineBehaviour : StateMachineBehaviour
    {
        public RoleController RoleController { get; set; }
        protected void lazyInit(Animator animator)
        {
            // if (RoleController == null)
            {
                RoleController = animator.GetComponentInParent<RoleController>();
            }
        }

        public virtual void Log(string msg)
        {
            // if (RoleController != null)
            // {
            //     Debug.Log(RoleController.gameObject.name + ":" + msg);
            // }
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            lazyInit(animator);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            lazyInit(animator);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            lazyInit(animator);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateMove(animator, stateInfo, layerIndex);
            lazyInit(animator);
        }

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateIK(animator, stateInfo, layerIndex);
            lazyInit(animator);
        }
    }

}