using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Tang
{
    public partial class SceneController : MonoBehaviour
    {
        // 角色列表 add by TangJian 2019/4/2 14:52
        public List<RoleController> RoleControllerList = new List<RoleController>();

        // 活着的角色个数 add by TangJian 2019/4/2 15:34
        public int RoleAliveCount = 0;
        
        // 活着的怪物个数 add by TangJian 2019/4/2 15:34
        public int MonsterAliveCount = 0;

        public event Action<RoleController> OnSceneRoleDying;
        
        public void AddRoleController(RoleController roleController)
        {
            RoleControllerList.Add(roleController);
            
            // 注册角色死亡事件 add by TangJian 2019/4/2 15:13
            roleController.OnDying += OnRoleDying;
            
            // 判断新加入的角色是否死亡 角色没死, 活着的角色数目 +1 add by TangJian 2019/4/2 15:14
            if (roleController.IsDead == false)
            {
                RoleAliveCount++;
                
                // 怪物数目, 去除玩家 add by TangJian 2019/4/16 14:41
                if (roleController.CompareTag("Role") && !roleController.CompareTag("Player"))
                {
                    MonsterAliveCount++;
                }
            }
        }

        public void RemoveRoleController(RoleController roleController)
        {
            RoleControllerList.Remove(roleController);
            
            roleController.OnDying -= OnRoleDying;
            
            // 角色离开, 如果角色没死, 活着的角色 -1 add by TangJian 2019/4/2 15:15
            if (roleController.IsDead == false)
            {
                RoleAliveCount--;
                
                // 怪物数目, 去除玩家 add by TangJian 2019/4/16 14:41
                if (roleController.CompareTag("Role") && !roleController.CompareTag("Player"))
                {
                    MonsterAliveCount--;
                }
            }
        }

        public void EnterRoleController(RoleController roleController)
        {
            // 从上一个场景中移除RoleController add by TangJian 2019/4/2 14:56
            if(roleController.CurrSceneController != null)
                roleController.CurrSceneController.RemoveRoleController(roleController);
            
            // 加入到新的场景 add by TangJian 2019/4/2 14:57
            AddRoleController(roleController);
        }


        void OnRoleDying(RoleController roleController)
        {
            // 角色死亡, 活着的角色 -1 add by TangJian 2019/4/2 15:16
            RoleAliveCount--;
            
            // 怪物数目, 去除玩家 add by TangJian 2019/4/16 14:41
            if (roleController.CompareTag("Role") && !roleController.CompareTag("Player"))
            {
                MonsterAliveCount--;
            }
            
            OnSceneRoleDying?.Invoke(roleController);
        }

        public RoleController GetLeastDistanceRole(Vector3 selfPos,string teamId)
        {
            if (RoleControllerList.Count<= 0) return null;
            
            List<RoleController> roleControllers;
            roleControllers = RoleControllerList.Where(controller => controller.TeamId == teamId).ToList().
                OrderBy(n => (n.transform.position - selfPos).magnitude).ToList();
            return roleControllers[0];
        }
    }
}