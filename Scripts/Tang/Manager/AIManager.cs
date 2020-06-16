using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Tang
{
    public class AIManager : MyMonoBehaviour
    {
        private static AIManager instance;
        public static AIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<AIManager>();
                }
                return instance;
            }
        }

        public List<NavMeshAgent> navMeshAgentList;
        public NavMeshPath navMeshPath;
        public NavMeshAgent navMeshAgent;

        void Start()
        {
            navMeshAgent = new NavMeshAgent();
            navMeshPath = new NavMeshPath();

            gameObject.RecursiveComponent((NavMeshAgent navMeshAgent, int depth) =>
            {
                Tools.Destroy(navMeshAgent);
            }, 1, 1);

            // human
            {
                navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
                navMeshAgentList.Add(navMeshAgent);

                navMeshAgent.updatePosition = false;
                navMeshAgent.updateRotation = false;
                navMeshAgent.updateUpAxis = false;
            }
        }

        public Vector3[] CalcPath(Vector3 from, Vector3 to)
        {
            navMeshAgent.nextPosition = from;
            if (navMeshAgent.CalculatePath(to, navMeshPath))
            {
                return navMeshPath.corners;
            }
            return null;
        }
    }
}