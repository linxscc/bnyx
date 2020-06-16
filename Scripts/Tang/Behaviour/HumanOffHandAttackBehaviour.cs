using UnityEngine;

namespace Tang
{
    public class HumanOffHandAttackBehaviour : StateMachineBehaviour
    {
        protected RoleController roleController;
        protected void lazyInit(Animator animator)
        {
            if (roleController == null)
            {
                roleController = animator.GetComponentInParent<RoleController>();
            }
        }
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            lazyInit(animator);
            roleController.currRoleAttackType = RoleAttackType.OffHand;
        }
    }
}