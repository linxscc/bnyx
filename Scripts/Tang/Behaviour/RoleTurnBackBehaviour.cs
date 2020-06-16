using UnityEngine;

public class RoleTurnBackBehaviour : StateMachineBehaviour
{
    Tang.RoleController roleController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (roleController == null)
        {
            roleController = animator.GetComponentInParent<Tang.RoleController>();
        }

        if (roleController != null)
        {
            if (roleController.GetDirectionInt() > 0)
            {
                roleController.SetDirection(Tang.Direction.Left);
            }
            else
            {
                roleController.SetDirection(Tang.Direction.Right);
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    // }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     base.OnStateExit(animator, stateInfo, layerIndex);

    //     if (roleController == null)
    //     {
    //         roleController = animator.GetComponentInParent<Tang.RoleController>();
    //     }

    //     if (roleController != null)
    //     {
    //         roleController.delayFunc("delayTurn", () =>
    //         {
    //             if (roleController.getDirectionInt() > 0)
    //             {
    //                 roleController.setDirection(Tang.Direction.Left);
    //             }
    //             else
    //             {
    //                 roleController.setDirection(Tang.Direction.Right);
    //             }
    //         }, 0.0f);
    //     }
    // }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    // override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    // }

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    // override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {        
    // }
}
