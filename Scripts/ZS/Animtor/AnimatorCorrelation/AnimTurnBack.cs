using Tang;
using UnityEngine;

public class AnimTurnBack : RoleBaseStateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        RoleController.SetDirectionInt(-RoleController.GetDirectionInt());
        animator.SetBool("is_turnback",false);    
        animator.SetBool("Alt",false);
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateExit(animator, animatorStateInfo, layerIndex);
//        animator.SetBool("is_turnback",false);
//        RoleController.SetDirectionInt(-RoleController.GetDirectionInt());
    }
}
