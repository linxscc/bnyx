using UnityEngine;

namespace Tang
{
    public enum PickUpMethod
    {
        Interact = 1, // 交互 add by TangJian 2018/01/24 00:05:39
        FlyIn = 2, // 飞到玩家身上 add by TangJian 2018/01/24 00:06:08
    }

    public enum RenderType
    {
        None = 0,
        Image = 1,
        Anim = 2
    }
    
    [System.Serializable]
    public class ItemData
    {
        public string id = ""; // 编号 add by TangJian 2018/01/24 00:04:18
        public string name = ""; // 名称 add by TangJian 2018/01/24 00:04:13
        public string desc = ""; // 描述 add by TangJian 2018/01/24 00:04:07

        public RenderType renderType = RenderType.Image; // 渲染模式 add by TangJian 2019/3/7 11:40
        public string icon = ""; // 图标 add by TangJian 2018/01/24 00:04:05
        
        public string anim = ""; //动画 add by TangJian 2019/3/7 11:39
        public string idleAnim = null; // 待机动画 add by TangJian 2019/3/7 12:13
        public string destoryAnim = null; // 销毁动画 add by TangJian 2019/3/7 12:13

        public int level = 1; // 等级 add by TangJian 2019/3/8 15:26
        
        public Vector3 position = new Vector3(); // 位置 add by TangJian 2018/01/24 00:04:02
        public ItemType itemType = ItemType.Consumable; // 物品类型 add by TangJian 2018/01/24 00:03:59
        public bool canStack = false; // 能否堆叠 add by TangJian 2018/01/24 00:03:50
        public int count = 1; // 数目 add by TangJian 2018/01/24 00:03:54
        public PickUpMethod pickUpMethod = PickUpMethod.Interact; // 拾取方式 add by TangJian 2018/01/24 00:06:44
    }
}