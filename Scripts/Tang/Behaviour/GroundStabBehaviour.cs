using UnityEngine;

namespace Tang
{
    public class GroundStabBehaviour : StateMachineBehaviour
    {
        GroundStabController groundStabController;
        public bool istoground;
        protected void lazyInit(Animator animator)
        {
            if (groundStabController == null)
            {
                groundStabController = animator.GetComponentInParent<GroundStabController>();
            }
            
        }
         public float waittime;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            lazyInit(animator);
            switch (groundStabController.groundStabStateType)
            {
                case GroundStabStateType.loop:
                    if (istoground)
                    {
                        waittime = groundStabController.attackTime;
                    }
                    else
                    {
                        waittime = groundStabController.downTime;
                    }
                    break;
            }
            if (groundStabController.first==false)
            {
                //waittime = groundStabController.firststate;
                //frist = false;
                groundStabController.OnOfftoStab(animator, waittime, istoground == true ? 0 : 1);
            }
            else
            {
                waittime = groundStabController.firststate;
                groundStabController.OnOfftoStab(animator, waittime, istoground == true ? 0 : 1);
            }
            //groundStabController.OnOfftoStab(animator, waittime, istoground == true ? 0 : 1);
        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            lazyInit(animator);
            //switch (groundStabController.groundStabStateType)
            //{
            //    case GroundStabStateType.loop:
            //        waittime -= Time.deltaTime;

            //        if (waittime <= 0)
            //        {
            //            if (istoground)
            //            {
            //                animator.SetInteger("state",0);
            //            }
            //            else
            //            {
            //                animator.SetInteger("state",1);
            //            }
            //        }
            //        break;
            //}
            
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            lazyInit(animator);

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     base.OnStateUpdate(animator, stateInfo, layerIndex);

        //     RoleController.Speed = new Vector3(RoleController.Speed.x, RoleController.jumpSpeed, RoleController.Speed.z);
        // }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     // base.OnStateExit(animator, stateInfo, layerIndex);

        //     // if (roleController == null)
        //     // {
        //     //     roleController = animator.GetComponentInParent<Tang.RoleController>();
        //     // }
        //     // if (roleController != null)
        //     // {
        //     //     roleController.Speed = new Vector3(roleController.Speed.x, roleController.jumpSpeed, roleController.Speed.z);
        //     // }
        // }

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
