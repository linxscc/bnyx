using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class NetworkObjectManager : MonoBehaviour
    {
        private static NetworkObjectManager instance;

        public static NetworkObjectManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<NetworkObjectManager>();
                }

                return instance;
            }
        }

        public Dictionary<string, object> networkObjects = new Dictionary<string, object>();
        
//        public void Add(object)
    }
}