using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tang;

public class GoodsAnimReset : StateMachineBehaviour
{
    public int state = 0;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("state",state);
    }
}
