using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tang
{
    public class DropItemsDataAsset : ScriptableObject
    {
        [System.Serializable]
        public class DropItem
        {
            public string Id;
            public string RoleId;
            public string RoleName;
            
            public int LvFrom;
            public int LvTo;
            public int DropOne;
            public int DropTwo;
            public int DropThree;
            public int DropSoul;
            
            public void SetData()
            {
                
            }
        }
        
        public static DropItemsDataAsset s_DropItemsDataAsset;

        public static DropItemsDataAsset Instance
        {
            get
            {
                if (s_DropItemsDataAsset == null)
                {
                    GetAsset();
                }

                return s_DropItemsDataAsset;
            }
        }

        private static async void GetAsset()
        {
            s_DropItemsDataAsset =
                await AssetManager.LoadAssetAsync<DropItemsDataAsset>("Manager/DropItemsDataAsset.asset");
        }
        public List<DropItem> DropItemList = new List<DropItem>();
        
        // 获取所有怪物的Id add by TangJian 2019/3/8 16:47
        public SortedSet<string> GetAllRoleId()
        {
            SortedSet<string> roleIdSet = new SortedSet<string>();
            
            foreach (var item in DropItemList)
            {
                roleIdSet.Add(item.RoleId);
            }

            return roleIdSet;
        }

        public List<string> GetDropItemIdList(int instanceLevel, string roleId)
        {
            int count = GetDropItemCount(instanceLevel, roleId);
            List<string> retItemIdList = new List<string>();
            if (count > 0)
            {
                List<string> itemIdList = ItemManager.Instance.GetDropItemByLevel(instanceLevel);
            
                for (int i = 0; i < count; i++)
                {
                    string dropItem = itemIdList[Random.Range(0, itemIdList.Count - 1)];
                    retItemIdList.Add(dropItem);   
                }
                return retItemIdList;
            }

            return retItemIdList;
        }
        
        public int GetDropItemCount(int instanceLevel, string roleId)
        {
            DropItem dropItem = GetDropItem(instanceLevel, roleId);
            if (dropItem != null)
            {
                return Tang.Tools.RandomWithWeight(100 - dropItem.DropOne - dropItem.DropTwo - dropItem.DropThree, dropItem.DropOne, dropItem.DropTwo, dropItem.DropThree);
            }

            return 0;
        }

        public DropItem GetDropItem(int instanceLevel, string roleId)
        {
            DropItem dropItem = DropItemList.Find(item =>
            {
                if (roleId == item.RoleId && instanceLevel >= item.LvFrom && instanceLevel <= item.LvTo)
                {
                    return true;
                }
                return false;
            });
            return dropItem;
        }

        public int GetDropSoulCount(int instanceLevel, string roleId)
        {
            var itemList = DropItemList.AsParallel().Where(item =>
            {
                return item.RoleId == roleId && (instanceLevel >= item.LvFrom && instanceLevel <= item.LvTo);
            });

            if (itemList.Count() > 0)
            {
                DropItem dropItem = itemList.First();
                return dropItem.DropSoul;
            }

            return 0;
        }
    }
}