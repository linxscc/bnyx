using System.Collections;
using UnityEngine;

namespace Tang
{
    public class WaitBehaviourAction : BehaviourAction
    {
        private RoleBehaviorTree roleBehaviorTree;
        private IEnumerator waitIE;
        public override void OnBegin()
        {
            roleBehaviorTree = GetComponent<RoleBehaviorTree>();

            waitIE = roleBehaviorTree.Wait(5);
        }
        public override TaskState OnUpdate()
        {
            Debug.Log("wait...");
            return waitIE.MoveNext() ? TaskState.Running : TaskState.Success;
        }
        
    }
}