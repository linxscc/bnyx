using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Tang
{
    public class RoleNavMeshAgent : MonoBehaviour
    {
        private NavMeshAgent navMeshAgent;
        private NavMeshPath navMeshPath;
        
        private RoleController selfController;
        private BehaviorDesigner.Runtime.BehaviorTree behaviorTree;
        private void OnEnable()
        {
            Init();
            InitNavMeshAgent();
        }
        
        private void Init()
        {
            selfController = GetComponent<RoleController>();
            behaviorTree = gameObject.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        }
        private void InitNavMeshAgent()
        {
            if (navMeshAgent != null) return;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                navMeshAgent = gameObject.AddComponentUnique<NavMeshAgent>();
                navMeshPath = new NavMeshPath();

                behaviorTree = gameObject.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
                if (behaviorTree != null)
                {
                    behaviorTree.SetValue("Anim", gameObject.transform.Find("Anim").gameObject);
                    behaviorTree.SetValue("target", gameObject);
                    behaviorTree.SetValue("gameObject", gameObject);
                    behaviorTree.enabled = true;
                }
                    
                navMeshAgent.updatePosition = false;
                navMeshAgent.updateRotation = false;
                navMeshAgent.updateUpAxis = false;
                    
                NavMeshAgentEnable();
            }
            else
            {
                behaviorTree = gameObject.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
                if (behaviorTree == null) return;
                behaviorTree.enabled = false;
                behaviorTree = null;
            }
        }

        private void Update()
        {
            InitNavMeshAgent();
        }

        public void NavMeshAgentEnable()
        {

        }
        public void NavMeshAgentDisable()
        {
        }
        
        
        
        // 具有随机时间的巡逻 add by tianjinpeng 2018/08/09 16:47:57
        public Vector3[] JumpPathdian(Vector3 desPos, float distance)
        {
            Vector3[] path = CalculPath(desPos);
            List<Vector3> newPath = new List<Vector3>();
            if (path != null && path.Length != 0)
            {
                newPath = Tools.GetPathList(path, distance);
            }
            return newPath.ToArray();
        }
        
        
        
        public bool TryCalcPath(Vector3 position, out Vector3[] path)
        {
            path = CalculPath(position);
            return path != null && path.Length > 0;
        }
        
         // 计算寻路轨迹 add by TangJian 2017/11/08 12:44:22
        public Vector3[] CalculPath(Vector3 position)
        {
            // 捕获异常, 防止无限计算路径 add by TangJian 2018/11/20 15:57
            try
            {
                navMeshPath.ClearCorners();

                NavMeshAgentEnable();

                if (navMeshAgent.isActiveAndEnabled)
                {
                    if (navMeshAgent.isOnNavMesh)
                    {
                        navMeshAgent.updatePosition = false;
                        navMeshAgent.updateRotation = false;
                        navMeshAgent.updateUpAxis = false;

                        navMeshAgent.Warp(selfController.transform.position);

                        if (navMeshAgent.CalculatePath(position, navMeshPath))
                        {
                            return navMeshPath.corners;
                        }
                    }
                    else
                    {
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(transform.position, out hit, 5, NavMesh.AllAreas))
                        {
                            return new Vector3[] { hit.position };
                        }
                    }
                }

#if UNITY_EDITOR
                // 绘制寻路轨迹 add by TangJian 2017/11/08 12:44:05
                DebugManager.Instance.AddDrawGizmos(gameObject.GetInstanceID() + "CalculPath", () =>
                {
                    for (int i = 0; i < navMeshPath.corners.Length; i++)
                    {
                        if (i > 0)
                        {
                            var from = navMeshPath.corners[i - 1];
                            var to = navMeshPath.corners[i];
                            Gizmos.DrawLine(from, to);
                        }
                    }
                });
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }

            return navMeshPath.corners;
        }
        
        public int TryCalcPathNew(Vector3 position, out Vector3[] path)
        {

            path = null;
            // 捕获异常, 防止无限计算路径 add by TangJian 2018/11/20 15:57
            try
            {

                navMeshPath.ClearCorners();

                NavMeshAgentEnable();

                if (navMeshAgent.isActiveAndEnabled)
                {
                    if (navMeshAgent.isOnNavMesh)
                    {
                        navMeshAgent.updatePosition = false;
                        navMeshAgent.updateRotation = false;
                        navMeshAgent.updateUpAxis = false;

                        navMeshAgent.Warp(selfController.transform.position);

                        if (navMeshAgent.CalculatePath(position, navMeshPath))
                        {
                            path = navMeshPath.corners;
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(transform.position, out hit, 5, NavMesh.AllAreas))
                        {
                            path = new Vector3[] { hit.position };
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }

#if UNITY_EDITOR
                // 绘制寻路轨迹 add by TangJian 2017/11/08 12:44:05
                DebugManager.Instance.AddDrawGizmos(gameObject.GetInstanceID() + "CalculPath", () =>
                {
                    for (int i = 0; i < navMeshPath.corners.Length; i++)
                    {
                        if (i > 0)
                        {
                            var from = navMeshPath.corners[i - 1];
                            var to = navMeshPath.corners[i];
                            Gizmos.DrawLine(from, to);
                        }
                    }
                });
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }

            return 0;
        }
        
        
    }
}