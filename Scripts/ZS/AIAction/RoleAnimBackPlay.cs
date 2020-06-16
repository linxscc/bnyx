using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using  Spine;
using Spine.Unity;

namespace ZS
{
    using  Tang;
    public class RoleAnimBackPlay : Action
    {
        private RoleController roleController;
        private RoleBehaviorTree roleBehaviorTree;
        private Vector3 lastPos;

        public override void OnStart()
        {
            roleController = gameObject.GetComponent<RoleController>();
            roleBehaviorTree = gameObject.GetComponent<RoleBehaviorTree>();
            lastPos = roleController.transform.position;
        }

        public override TaskStatus OnUpdate()
        {
            RoleController targetController = roleBehaviorTree.TargetController;
            
            if ((roleController.transform.position.x - lastPos.x) * (targetController.transform.position.x - roleController.transform.position.x) <= 0)
            {
                roleController.SkeletonAnimator.SetBackwards(true);   

                return TaskStatus.Running;
            }
            else
            {
                roleController.SkeletonAnimator.SetBackwards(false);   

                return TaskStatus.Running;
            }
           
        }

        public override void OnLateUpdate()
        {
            lastPos = roleController.transform.position;
        }

        public override void OnEnd()
        {
            roleController.SkeletonAnimator.SetBackwards(false);
        }
    }


}
