using System.Collections.Generic;


namespace Tang
{
    public static class RoleItemDataExtend
    {
        // 获得当前聚焦的物品 add by TangJian 2017/10/24 16:36:46
        public static ItemData GetFocusItem(this RoleItemData roleItemData)
        {
            return roleItemData.ItemList[roleItemData.FocusIndex];
        }

        public static ItemData GetPreviousItem(this RoleItemData roleItemData)
        {
            return roleItemData.ItemList[(roleItemData.FocusIndex + 1) % roleItemData.ItemList.Count];
        }

        public static ItemData GetNextItem(this RoleItemData roleItemData)
        {
            return roleItemData.ItemList[(roleItemData.FocusIndex - 1) % roleItemData.ItemList.Count];
        }

        public static void RemoveItemWithIndex(this List<ItemData> target, int index, int count = 1)
        {
            ItemData itemData = target[index];
            if (itemData != null)
            {
                itemData.count -= count;
                if (itemData.count <= 0)
                {
                    target.RemoveAt(index);
                }
            }
        }

        public static void RemoveItemWithId(this List<ItemData> target, string id, int count = 1)
        {
            int index = target.FindIndex((ItemData item) =>
            {
                return item.id == id;
            });

            target.RemoveItemWithIndex(index);
        }
    }
}