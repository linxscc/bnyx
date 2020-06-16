using UnityEngine;

namespace Tang
{
    public class UnburiedPickUpBehaviour : StateMachineBehaviour
    {
        public RoleController RoleController { get; set; }
        protected void lazyInit(Animator animator)
        {
            // if (RoleController == null)
            {
                RoleController = animator.GetComponentInParent<RoleController>();
            }
        }
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool("pickup", false);
            lazyInit(animator);
            BehaviorDesigner.Runtime.BehaviorTree behaviorTree= RoleController.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            GameObject wuqi = RoleController.gameObject.GetChild("wuqi");
            wuqi.SetActive(true);
            //float state = animator.GetFloat("State");
            //RoleController.DefendBegin();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            //lazyInit(animator);
            //RoleController.DefendBegin();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            animator.SetBool("pickup", false);
            lazyInit(animator);
            float currstate = animator.GetFloat("State");
            float state = currstate - 1f;
            animator.SetFloat("State", state);
            //RoleController.DefendEnd();
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        // public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     base.OnStateMove(animator, stateInfo, layerIndex);
        // }

        // // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        // public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     base.OnStateIK(animator, stateInfo, layerIndex);
        // }
    }
}