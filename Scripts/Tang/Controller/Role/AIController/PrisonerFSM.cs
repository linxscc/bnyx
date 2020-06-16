using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class PrisonerFSM :MonoBehaviour
    {
        private RoleController targetRoleController;
        private RoleController selfRoleController;
        private RoleBehaviorTree roleBehaviorTree;
        private RoleNavMeshAgent roleNavMeshAgent;
        private IRoleAction roleAction;
        
        private FSM fsm = new FSM();
        private List<State> states;
        private ValueMonitorPool valueMonitorPool = new ValueMonitorPool();

        private class State
        {
            public string name;
            public int weight = 0;
        }


         private void Start()
         {
             Init();
             InitFsm();
             InitTrigger();
         }
         private void Update()
         {
             valueMonitorPool.Update();
             fsm.Update();
         }
        
         private void Init()
         {
             selfRoleController = GetComponent<RoleController>();
             roleBehaviorTree = GetComponent<RoleBehaviorTree>();
             roleNavMeshAgent = GetComponent<RoleNavMeshAgent>();
         }
         public void InitTrigger()
        {
            states = new List<State>();
            states.AddRange(new List<State>()
            {
                new State(){ name = "Idle", weight = 0 },
                new State(){ name = "MoveToTargetAttack", weight = 0 },
            });
            
            valueAdd();
        }
         
         private void valueAdd()
         {
             valueMonitorPool.AddMonitor(
                 () => targetRoleController ? ((targetRoleController.transform.position - selfRoleController.transform.position).magnitude < 10f) : false,
                 (b, b1) =>
                 {
                     if(b1)
                         states.Find((state => state.name == "MoveToTargetAttack")).weight += 1;
                 });
         }

        public void InitFsm()
        {
            fsm.SetCurrStateName("Idle");
            
            fsm.AddState("Idle", 
                () => { },
                () =>
            {
                if (FindTargetController() is RoleController roleController)
                {
                    targetRoleController = roleController;
                    var state = SelectState("Idle");
                    if (state!=null)
                    {
                        fsm.SendEvent("To" + state);
                    }
                }
            }, 
                () =>
            {
                valueMonitorPool.Reset();
            });

            var moveto = MoveTo();
            fsm.AddState("MoveToTargetAttack",
                () =>
            {                
                moveto = MoveTo();
            },  
                () =>
            {
                if (!moveto.MoveNext())
                {
                    fsm.SendEvent("ToIdle");
                }
            }, 
                Action1);
            
        }

        private string SelectState(string currStateName)
        {
            int weightMax = 0;
            string retName = null;
            foreach (var state in states)
            {
                if (state.weight > weightMax)
                {
                    weightMax = state.weight;
                    retName = state.name;
                }
            }
            return retName;
        }


        
        private RoleController FindTargetController()
        {
            return roleBehaviorTree.enemys.Count <= 0 ? null : roleBehaviorTree.enemys[0].GetComponent<RoleController>();
        }
        private IEnumerator MoveTo()
        {
            while (!selfRoleController.IsGrounded())
            {
                yield return null;
            }
            
            var selfPos = selfRoleController.transform.position;
            var targetPos = targetRoleController.transform.position;

            var path = roleNavMeshAgent.CalculPath(targetPos);
            if (path.Length <= 0)
            {
                Debug.LogError("path为空！");
            }
            
            for (int currIndex = 1; currIndex < path.Length; currIndex++)
            {
                var position = path[currIndex];
                IEnumerator moveTo = roleBehaviorTree.MoveTo(position, selfRoleController.GetSpeedByMoveState(MoveState.Walk));

                while (true)
                {
                    if (moveTo.MoveNext())
                    {
                        yield return CoroutineStatus.Running;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        private void Action1()
        {
            selfRoleController.Action1Begin();
            valueAdd();
        }

    }
}