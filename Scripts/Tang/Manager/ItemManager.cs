using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Parallel = System.Threading.Tasks.Parallel;

namespace Tang
{
    public class ItemManager
    {
//        [RuntimeInitializeOnLoadMethod]
        public static void Init1()
        {
            instance = new ItemManager();
            instance.Init();
        }

        private static ItemManager instance;
        public static ItemManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ItemManager();
                    instance.Init();
                }
                return instance;
            }
        }
        public Dictionary<string, ItemData> itemDataDict = new Dictionary<string, ItemData>();
        public Dictionary<int, List<string>> itemLevelDic = new Dictionary<int, List<string>>();
        public List<string> itemLevelList = new List<string>();
        
        public void Init()
        {
            
            LoadWeaponMap();
            LoadArmorMap();
            LoadConsumableMap();
            LoadSoulMap();
            LoadDecorationMap();
            LoadOtherItemMap();
            Debug.Log("itemDataDict = " + Tools.Obj2Json<Dictionary<string, ItemData>>(itemDataDict));
            
            GenerateItemLevelList();
            UnLoad();
        }

        private async void UnLoad()
        {
            var obj = await AssetManager.LoadAssetAsync<TextAsset>("WeaponDatas");
            Addressables.Release(obj);   
        }
        public async void LoadWeaponMap()
        {
            var weaponDataDict = await AssetManager.LoadJson<Dictionary<string, WeaponData>>("WeaponDatas");
            foreach (KeyValuePair<string, WeaponData> kvp in weaponDataDict)
            {
                itemDataDict.Add(kvp.Key, kvp.Value);
            }
        }
        public async void LoadArmorMap()
        {
            var armorDataMap = await AssetManager.LoadJson<Dictionary<string, ArmorData>>("ArmorDatas");
            foreach (KeyValuePair<string, ArmorData> kvp in armorDataMap)
            {
                itemDataDict.Add(kvp.Key, kvp.Value);
            }
        }
        public async void LoadConsumableMap()
        {
            var otherItemMap = await AssetManager.LoadJson<Dictionary<string, ConsumableData>>("ConsumableDatas");
            foreach (KeyValuePair<string, ConsumableData> kvp in otherItemMap)
            {
                itemDataDict.Add(kvp.Key, kvp.Value);
            }
        }
        public async void LoadSoulMap()
        {
            var consumableDataMap = await AssetManager.LoadJson<Dictionary<string, SoulData>>("SoulDatas");
            foreach (KeyValuePair<string, SoulData> kvp in consumableDataMap)
            {
                itemDataDict.Add(kvp.Key, kvp.Value);
            }
        }
        public async void LoadDecorationMap()
        {
            var armorDataMap = await AssetManager.LoadJson<Dictionary<string, DecorationData>>("DecorationDatas");
            foreach (KeyValuePair<string, DecorationData> kvp in armorDataMap)
            {
                itemDataDict.Add(kvp.Key, kvp.Value);
            }
        }

        public async void LoadOtherItemMap()
        {
            var otherItemMap = await AssetManager.LoadJson<Dictionary<string, ItemData>>("OtherItemDatas");
            foreach (KeyValuePair<string, ItemData> kvp in otherItemMap)
            {
                itemDataDict.Add(kvp.Key, kvp.Value);
            }
        }

        public void loadTreasureBoxMap()
        {
            // var otherItemMap = Tools.getObjectFromResource<Dictionary<string, ItemData>>("Scripts/OtherItem/OtherItem");
            // foreach (KeyValuePair<string, ItemData> kvp in otherItemMap)
            // {
            //     itemDataDict.Add(kvp.Key, kvp.Value);
            // }
        }

        public void GenerateItemLevelList()
        {
            foreach (var itemPair in itemDataDict)
            {
                List<string> itemIdList;
                if (itemLevelDic.TryGetValue(itemPair.Value.level, out itemIdList))
                {
                    itemIdList.Add(itemPair.Value.id);
                }
                else
                {
                    itemLevelDic.Add(itemPair.Value.level, new List<string>() { itemPair.Value.id });
                }
            }
        }

        public T getItemDataById<T>(string id) where T : ItemData
        {
            if (itemDataDict.ContainsKey(id))
            {
                var itemData = itemDataDict[id];
                return itemData as T;
            }
            return null;
        }

        public void addWeaponDataToItemDict(string id, string name, EquipType equipType, string weaponAttachmentName)
        {
            itemDataDict.Add(id, newWeaponData(id, name, equipType, weaponAttachmentName));
        }

        public WeaponData newWeaponData(string id, string name, EquipType equipType, string weaponAttachmentName)
        {
            WeaponData weaponData = new WeaponData();
            weaponData.id = id;
            weaponData.name = name;
            weaponData.equipType = equipType;
            weaponData.weaponAttachmentName = weaponAttachmentName;
            return weaponData;
        }

        private object lo = new object();

        public List<ItemData> GetItemDataListWithLevel(int fromLevel, int toLevel)
        {
            List<ItemData> retItemIdList = new List<ItemData>();

            var itemList = itemLevelDic.AsParallel().Where(pair => { return pair.Key >= fromLevel && pair.Key <= toLevel; });

            foreach (var itemsListPair in itemList)
            {
                foreach (var itemId in itemsListPair.Value)
                {
                    retItemIdList.Add(getItemDataById<ItemData>(itemId));
                }
            }

            return retItemIdList;
        }

        public List<string> GetItemIdListDescendingOrder(int fromLevel, int toLevel, int count)
        {
            return GetItemDataListWithLevel(fromLevel, toLevel).Select(i => i.id).ToList();
        }

        public List<string> GetDropItemByLevel(int level)
        {
            var itemLists = itemLevelDic.AsParallel().Where(pair => { return pair.Key >= 0 && pair.Key <= level; }).OrderBy(pair => pair.Key);

            Parallel.ForEach(itemDataDict, pair =>
            {

            });

            int needItemCout = 3;
            List<string> retItemList = new List<string>();

            foreach (var itemList in itemLists)
            {
                retItemList.AddRange(itemList.Value);
                if (retItemList.Count >= needItemCout)
                {
                    break;
                }
            }

            return retItemList;
        }
    }
}