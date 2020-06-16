using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class RoleManager : MonoBehaviour
    {
        private static RoleManager instance;
        public static RoleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<RoleManager>();
                }
                return instance;
            }
        }
        
        private Dictionary<string, RoleData> roleDataDict = new Dictionary<string, RoleData>();
        private Dictionary<string,List<RoleAIAction>> roleAIActionDic=new Dictionary<string, List<RoleAIAction>>();


        private  Dictionary<string, RoleController> RoleControllers = new Dictionary<string, RoleController>();

        public void AddRoleController(RoleController roleController)
        {
            RoleControllers.Add(roleController.GetHashCode().ToString(), roleController);
        }

        public void RemoveRoleController(RoleController roleController)
        {
            RoleControllers.Remove(roleController.GetHashCode().ToString());
        }

        void Awake()
        {
            loadRoleData();
        }

        public async void loadRoleData()
        {
          
        }

        public RoleData getRoleDataById(string id)
        {
            if (roleDataDict.ContainsKey(id))
            {
                var data = roleDataDict[id];
                return Tools.DepthClone(data);
            }
            return null;
        }
    }
}