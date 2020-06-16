using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Tang
{
    public partial class SceneController : MonoBehaviour
    {
        // 场景门的列表 add by TangJian 2019/4/2 15:36
        public List<PortalController> PortalControllerList = new List<PortalController>();
        
        // 初始化传送门 add by TangJian 2019/4/2 15:37
        public void InitPortal()
        {
            PortalControllerList = this.GetComponentsInChildren<PortalController>().ToList();

            _valueMonitorPool.AddMonitor<int>(() => MonsterAliveCount, (from, to) =>
            {
                if (to > 0)
                {
                    CloseAllPortal();
                }
                else if(to < 0)
                {
                    OpenAllPortal();
                }
                else
                {
                    OpenAllPortal();
                }
            }, true);
        }

        // 打开所有传送门 add by TangJian 2019/4/2 15:38
        public void OpenAllPortal()
        {
            foreach (var portalController in PortalControllerList)
            {
                portalController.forceClose = false;
            }
        }

        // 关闭所有传送门 add by TangJian 2019/4/2 15:38
        public void CloseAllPortal()
        {
            foreach (var portalController in PortalControllerList)
            {
                portalController.forceClose = true;
            }
        }
    }
}