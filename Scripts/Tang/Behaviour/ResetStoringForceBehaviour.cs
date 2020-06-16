using UnityEngine;


public enum ResetStoringForceType
{
    ordinary = 0, 
    ClearType=1,
    No=2
}


public class ResetStoringForceBehaviour : StateMachineBehaviour
{
    protected Tang.RoleController roleController;
    private bool isNew = true;
    
    public bool startorexit = true;
    public ResetStoringForceType storingForceType = ResetStoringForceType.ordinary;
    
    public string parametername = "storage_time";

    protected void lazyInit(Animator animator)
    {
        if (roleController == null)
        {
            roleController = animator.GetComponentInParent<Tang.RoleController>();
        }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
//        lazyInit(animator);
//        if(roleController.animatorParams.ContainsKey("storage_time"))
//        animator.SetFloat("storage_time", 0f);
//        
        lazyInit(animator);
        switch (storingForceType)
        {
            case ResetStoringForceType.ordinary:
                if(roleController.animatorParams.ContainsKey(parametername))
                    animator.SetFloat(parametername, 0f);
                break;
            case ResetStoringForceType.ClearType:
                if (startorexit == true)
                {
                    if (roleController.animatorParams.ContainsKey(parametername))
                        animator.SetFloat(parametername, 0f);
                }
                break;
            default:

                break;
        }
        // 变红
        // roleController.SkeletonAnimator.skeleton.DoColorTo(new Color(1, 0.3f, 0.3f), 0.1f).OnComplete(() =>
        // {
        //     roleController.SkeletonAnimator.skeleton.DoColorTo(Color.white, 0.1f);
        // });
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float time = animator.GetFloat(parametername);
        time += Time.deltaTime;
        animator.SetFloat(parametername, time);
        // lazyInit(animator);

        // AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);
        // bool hasNext = nextStateInfo.fullPathHash != 0;

        // if (isNew && hasNext)
        // {
        //     isNew = false;
        //     animator.speed = 0f;
        //     roleController.delayFunc("RoleHurtBehaviour:setAnimSpeed", () =>
        //     {
        //         animator.speed = roleController.DefaultAnimSpeed;
        //     }, 0.5f);
        // }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lazyInit(animator);
        
        
        switch (storingForceType)
        {
            case ResetStoringForceType.ClearType:
                if ( startorexit == false)
                {
                    if (roleController.animatorParams.ContainsKey(parametername))
                        animator.SetFloat(parametername, 0f);
                }
                break;
            default:

                break;
        }
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
