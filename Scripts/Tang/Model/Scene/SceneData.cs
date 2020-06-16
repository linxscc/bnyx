using System.Collections.Generic;

namespace Tang
{
    [System.Serializable]
    public class SceneData
    {
        public string rawSceneId; // 原始Id add by TangJian 2017/08/02 19:16:37
        public string sceneId; // 场景Id add by TangJian 2017/08/02 19:16:38                                
        public List<PortalData> portalDatas = new List<PortalData>(); // 传送门数据 add by TangJian 2017/08/02 20:29:19
        public List<RoleData> roleDatas = new List<RoleData>(); // 角色数据 add by TangJian 2017/08/02 19:16:39
        public List<ItemData> itemDatas = new List<ItemData>(); // 物品数据 add by TangJian 2017/08/21 17:08:36
        public List<TreasureBoxData> treasureBoxDatas = new List<TreasureBoxData>(); // 宝箱数据 add by TangJian 2018/01/23 15:47:43
        public List<tian.PlacementData> PlacementDatas = new List<tian.PlacementData>(); 
        public bool inited = false;
    }
}