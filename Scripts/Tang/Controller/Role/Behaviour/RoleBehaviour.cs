using UnityEngine;
using Tang;

public class RoleBehaviour : StateMachineBehaviour
{
    public float StateDuration = -1;
    protected Tang.RoleController roleController;

    private void lazyInit(Animator animator)
    {
        roleController = animator.GetComponentInParent<Tang.RoleController>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        lazyInit(animator);
        
        // 设置动画状态机的结束时间 add by TangJian 2019/5/13 18:36
        if (StateDuration >= 0)
        {
            roleController.SkeletonAnimator.SetStateTime(animatorStateInfo.fullPathHash, StateDuration);
        }
        
        if (roleController.animatorParams.ContainsKey("state_running_percent"))
            animator.SetFloat("state_running_percent", animatorStateInfo.normalizedTime * 100); 

        animator.SetBool("Alt", false);

        animator.SetBool("action1", false);

        animator.SetBool("action2", false);

        animator.SetBool("action3", false);

        animator.SetBool("action4", false);
        
        if (roleController.animatorParams.ContainsKey("KeyBoard1"))
            animator.SetBool("KeyBoard1", false);
        if (roleController.animatorParams.ContainsKey("KeyBoard2"))
            animator.SetBool("KeyBoard2", false);
        if (roleController.animatorParams.ContainsKey("KeyBoard3"))
            animator.SetBool("KeyBoard3", false);


        if (roleController.animatorParams.ContainsKey("action5"))
            animator.SetBool("action5", false);
        if (roleController.animatorParams.ContainsKey("action6"))
            animator.SetBool("action6", false);
        if (roleController.animatorParams.ContainsKey("action7"))
            animator.SetBool("action7", false);
        if (roleController.animatorParams.ContainsKey("action-skill"))
            animator.SetBool("action-skill", false);

        if (roleController.animatorParams.ContainsKey("climb_begin"))
            animator.SetBool("climb_begin", false);
        
        if (roleController.animatorParams.ContainsKey("climb_drop"))
        {
            animator.SetBool("climb_drop", false);
        }

        if (roleController.animatorParams.ContainsKey("pickup"))
            animator.SetBool("pickup", false);

        if (roleController.animatorParams.ContainsKey("broken"))
            animator.SetBool("broken", false);

        if (roleController.animatorParams.ContainsKey("renew"))
            animator.SetBool("renew", false);

        if (roleController.animatorParams.ContainsKey("defence_back_hurt"))
            animator.SetBool("defence_back_hurt", false);

        if (roleController.animatorParams.ContainsKey("start_climb_ladder"))
            animator.SetBool("start_climb_ladder", false);

        if (roleController.animatorParams.ContainsKey("climb_ladder_over"))
            animator.SetBool("climb_ladder_over", false);
        if (roleController.animatorParams.ContainsKey("action2_end"))
            animator.SetBool("action2_end", false);

        animator.ResetTrigger("action1_begin");
        animator.ResetTrigger("action2_begin");
        
        if (roleController.animatorParams.ContainsKey("action1_end"))
            animator.SetBool("action1_end", false);

        if (roleController.animatorParams.ContainsKey("action-skill_end"))
            animator.SetBool("action-skill_end", false);
        
        if (roleController.animatorParams.ContainsKey("action_jump"))
            animator.SetInteger("action_jump", 0);

        // if (roleController.animatorParams.ContainsKey("action_roll"))
        animator.SetInteger("action_roll", 0);

        if (roleController.animatorParams.ContainsKey("action3over"))
            animator.SetBool("action3over", false);
        
        if(roleController.animatorParams.ContainsKey("dodge"))
            animator.SetBool("dodge", false);
        
        if (roleController.animatorParams.ContainsKey("hurt"))
            animator.SetBool("hurt", false);

        if (animatorStateInfo.IsTag("idle"))
        {
            if (roleController && roleController.BeHaviorTree)
                roleController.BeHaviorTree.SendEvent("idleon");
        }
        roleController.AnimMoveSpeedScale = 1f;
        
        // idle状态跳跃重置
        if (roleController.IsGrounded() || 
            roleController.GetCurrAnimTagHash() == Animator.StringToHash("idle") || 
            roleController.GetCurrAnimTagHash() == Animator.StringToHash("run"))
        {
            roleController.currjumpTimes = 0;
            if (roleController.animatorParams.ContainsKey("no_jump"))
            {
                animator.SetBool("no_jump", false);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {//重置状态
        lazyInit(animator);

        roleController.ClearCurrAnimDamageGameObject();

        animator.SetBool("action1", false);

        animator.SetBool("action2", false);

        animator.SetBool("action3", false);

        animator.SetBool("action4", false);


        if (roleController.animatorParams.ContainsKey("action5"))
            animator.SetBool("action5", false);
        if (roleController.animatorParams.ContainsKey("action6"))
            animator.SetBool("action6", false);
        if (roleController.animatorParams.ContainsKey("action7"))
            animator.SetBool("action7", false);
        if (roleController.animatorParams.ContainsKey("action-skill"))
            animator.SetBool("action-skill", false);

        if (roleController.animatorParams.ContainsKey("pickup"))
            animator.SetBool("pickup", false);

        if (roleController.animatorParams.ContainsKey("broken"))
            animator.SetBool("broken", false);

        if (roleController.animatorParams.ContainsKey("renew"))
            animator.SetBool("renew", false);

        if (roleController.animatorParams.ContainsKey("defence_back_hurt"))
            animator.SetBool("defence_back_hurt", false);

        if (roleController.animatorParams.ContainsKey("climb_begin"))
            animator.SetBool("climb_begin", false);
        if (roleController.animatorParams.ContainsKey("climb_drop"))
        {
            animator.SetBool("climb_drop", false);
        }
        if (roleController.animatorParams.ContainsKey("start_climb_ladder"))
            animator.SetBool("start_climb_ladder", false);
        if (roleController.animatorParams.ContainsKey("climb_ladder_over"))
            animator.SetBool("climb_ladder_over", false);
        if (roleController.animatorParams.ContainsKey("action2_end"))
            animator.SetBool("action2_end", false);
        
        animator.ResetTrigger("action1_begin");
        animator.ResetTrigger("action2_begin");
        
        if (roleController.animatorParams.ContainsKey("action1_end"))
            animator.SetBool("action1_end", false);

        if (roleController.animatorParams.ContainsKey("action-skill_end"))
            animator.SetBool("action-skill_end", false);

        if (roleController.animatorParams.ContainsKey("action_jump"))
            animator.SetInteger("action_jump", 0);

        if (roleController.animatorParams.ContainsKey("action_roll"))
            animator.SetInteger("action_roll", 0);

        if (roleController.animatorParams.ContainsKey("hurt"))
            animator.SetBool("hurt", false);

        if (roleController.animatorParams.ContainsKey("action3over"))
            animator.SetBool("action3over", false);
        
        if (roleController.animatorParams.ContainsKey("time"))
            animator.SetFloat("time", 0);
        
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        lazyInit(animator);

        if(roleController)
            if (roleController.animatorParams.ContainsKey("state_running_percent"))
                animator.SetFloat("state_running_percent", animatorStateInfo.normalizedTime * 100);

        animator.SetFloat("time", animatorStateInfo.normalizedTime);
    }
}