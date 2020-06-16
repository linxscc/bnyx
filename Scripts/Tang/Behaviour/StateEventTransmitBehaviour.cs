using UnityEngine;
using Tang;

public class StateEventTransmitBehaviour : StateMachineBehaviour
{
    public string StateNawme;
    private IAnimatorStateDelegate _animatorStateDelegate;

    private float beginTime;

    void LazyInit(Animator animator)
    {
        if (_animatorStateDelegate == null)
        {
            _animatorStateDelegate = animator.GetComponent<IAnimatorStateDelegate>();
        }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        LazyInit(animator);
        beginTime = Tang.Time.time;
        _animatorStateDelegate?.OnStateEvent(StateNawme, AnimatorStateEventType.OnStateEnter, animator, stateInfo, layerIndex, Tang.Time.time - beginTime);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        LazyInit(animator);
        

        _animatorStateDelegate?.OnStateEvent(StateNawme, AnimatorStateEventType.OnStateUpdate, animator, stateInfo, layerIndex, Tang.Time.time - beginTime);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        LazyInit(animator);
        _animatorStateDelegate?.OnStateEvent(StateNawme, AnimatorStateEventType.OnStateExit, animator, stateInfo, layerIndex, Tang.Time.time - beginTime);
    }
 
    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        LazyInit(animator);
        _animatorStateDelegate?.OnStateEvent(StateNawme, AnimatorStateEventType.OnStateMove, animator, stateInfo, layerIndex, Tang.Time.time - beginTime);
    }

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateIK(animator, stateInfo, layerIndex);
        LazyInit(animator);
        _animatorStateDelegate?.OnStateEvent(StateNawme, AnimatorStateEventType.OnStateIK, animator, stateInfo, layerIndex, Tang.Time.time - beginTime);
    }
}