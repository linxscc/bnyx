using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tang
{
    [System.Serializable]
    public class RoleEquipDataBase
    {
        [SerializeField] private WeaponData mainHand;
        [SerializeField] private WeaponData offHand;

        public WeaponData MainHand
        {
            get
            {
                if (mainHand == null)
                    return new WeaponData();
                return mainHand;
            }

            set
            {
                mainHand = value;
            }
        }

        public WeaponData OffHand
        {
            get
            {
                if (offHand == null)
                    return new WeaponData();
                return offHand;
            }

            set
            {
                offHand = value;
            }
        }

        [SerializeField] private SoulData soulData;
        public SoulData SoulData
        {
            get
            {
                if (soulData == null)
                    return new SoulData();
                return soulData;
            }

            set
            {
                soulData = value;
            }
        }

        public bool HasMainHand()
        {
            if (mainHand != null && mainHand.id != "")
            {
                return true;
            }
            return false;
        }

        public bool HasOffHand()
        {
            if (offHand != null && offHand.id != "")
            {
                return true;
            }
            return false;
        }
        public bool HassoulData()
        {
            if (soulData != null && soulData.id != "")
            {
                return true;
            }
            return false;
        }
        public bool IsOneHanded(EquipData equipData)
        {
            return equipData.equipType == EquipType.Swd || equipData.equipType == EquipType.Sswd;
        }
    }

    public enum EquipPlaceType
    {
        Helmet = 1,
        Necklace = 2,
        Armor = 3,
        Glove = 4,
        Trousers = 5,
        Shoe = 6,
        Ring1 = 7,
        Ring2 = 8,
        Weapon_Main = 9,
        Weapon_Secondry = 10
    }

    [System.Serializable]
    public class RoleEquipData : RoleEquipDataBase
    {
        public ArmorData armorData;

        public EquipData HelmetData;
        public EquipData necklaceData;
        public EquipData gloveData;
        public EquipData TrousersData;
        public EquipData shoeData;
        public EquipData ring1Data;
        public EquipData ring2Data;
        
        [SerializeField] private List<DecorationData> decorationDataList = new List<DecorationData>();

        public List<DecorationData> DecorationDataList { get { return decorationDataList; } set { decorationDataList = value; } }

        public ArmorData GetArmorData()
        {
            if (armorData!=null)
            {
                return armorData;
            }
            else
            {
                return new ArmorData();
            }
            
        }

        public EquipType getMainHandWeaponType()
        {
            EquipData equipData = getMainHand<EquipData>();
            if (equipData != null)
            {
                return equipData.equipType;
            }
            return EquipType.None;
        }

        public EquipType getOffHandWeaponType()
        {
            EquipData equipData = getOffHand<EquipData>();
            if (equipData != null)
            {
                return equipData.equipType;
            }
            return EquipType.None;
        }

       
        public bool HasArmorData()
        {
            if (armorData != null && armorData.id != "")
            {
                return true;
            }
            return false;
        }
        public bool HasDecoration(int index)
        {
            DecorationData decorationData;
            if (decorationDataList.TryGet(index,out decorationData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public T getMainHand<T>() where T : EquipData
        {
            if (MainHand != null && MainHand.id != "")
            {
                return MainHand as T;
            }
            return null;
        }
        public T getOffHand<T>() where T : EquipData
        {
            if (OffHand != null && OffHand.id != "")
            {
                return OffHand as T;
            }
            return null;
        }
        public bool HasEquipRing(int index)
        {
            switch (index)
            {
                case 0:
                    if (ring1Data != null && ring1Data.id != "")
                    {
                        return true;
                    }
                    break;
                case 1:
                    if (ring2Data != null && ring2Data.id != "")
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
        public bool HasEquip(EquipType equipType)
        {
            switch (equipType)
            {
                case EquipType.Necklace:
                    if (necklaceData != null && necklaceData.id != "")
                    {
                        return true;
                    }
                    break;
                case EquipType.Glove:
                    if (gloveData != null && gloveData.id != "")
                    {
                        return true;
                    }
                    break;
                case EquipType.Trousers:
                    if (TrousersData != null && TrousersData.id != "")
                    {
                        return true;
                    }
                    break;
                case EquipType.Shoe:
                    if (shoeData != null && shoeData.id != "")
                    {
                        return true;
                    }
                    break;
                case EquipType.Helmet:
                    if (HelmetData != null && HelmetData.id != "")
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
        
        public EquipData GetEquip(EquipType equipType)
        {
            switch (equipType)
            {
                case EquipType.Necklace:
                    if (necklaceData != null && necklaceData.id != "")
                    {
                        return necklaceData;
                    }
                    break;
                case EquipType.Glove:
                    if(gloveData!=null&& gloveData.id != "")
                    {
                        return gloveData;
                    }
                    break;
                case EquipType.Trousers:
                    if(TrousersData!=null && TrousersData.id != "")
                    {
                        return TrousersData;
                    }
                    break;
                case EquipType.Shoe:
                    if (shoeData != null && shoeData.id != "")
                    {
                        return shoeData;
                    }
                    break;
                case EquipType.Helmet:
                    if(HelmetData!=null && HelmetData.id != "")
                    {
                        return HelmetData;
                    }
                    break;
            }
            return null;
        }

        public EquipData GetEquipByPlace(EquipPlaceType equipPlaceType)
        {
            switch (equipPlaceType)
            {
                case EquipPlaceType.Helmet:
                    return HelmetData;
                case EquipPlaceType.Necklace:
                    return necklaceData;
                case EquipPlaceType.Glove:
                    return gloveData;
                case EquipPlaceType.Trousers:
                    return TrousersData;
                case EquipPlaceType.Shoe:
                    return shoeData;
                case EquipPlaceType.Ring1:
                    return ring1Data;
                case EquipPlaceType.Ring2:
                    return ring2Data;
                case EquipPlaceType.Weapon_Main:
                    return MainHand;
                case EquipPlaceType.Weapon_Secondry:
                    return OffHand;
            }

            return null;
        }

        public void SetEquipByPlace(EquipPlaceType equipPlaceType, EquipData equipData)
        {
            switch (equipPlaceType)
            {
                case EquipPlaceType.Helmet:
                    HelmetData = equipData;
                    break;
                case EquipPlaceType.Necklace:
                    necklaceData = equipData;
                    break;
                case EquipPlaceType.Glove:
                    gloveData = equipData;
                    break;
                case EquipPlaceType.Trousers:
                    TrousersData = equipData;
                    break;
                case EquipPlaceType.Shoe:
                    shoeData = equipData;
                    break;
                case EquipPlaceType.Ring1:
                    ring1Data = equipData;
                    break;
                case EquipPlaceType.Ring2:
                    ring2Data = equipData;
                    break;
                case EquipPlaceType.Weapon_Main:
                    MainHand = equipData as WeaponData;
                    break;
                case EquipPlaceType.Weapon_Secondry:
                    OffHand = equipData as WeaponData;
                    break;
                
            }
        }
        
        public EquipData GetEquipRing(int index)
        {
            switch (index)
            {
                case 0:
                    if (ring1Data != null && ring1Data.id != "")
                    {
                        return ring1Data;
                    }
                    break;
                case 1:
                    if (ring2Data != null && ring2Data.id != "")
                    {
                        return ring2Data;
                    }
                    break;
            }
            return null;
        }
        public void UseEquip(EquipData equipData,Vector3 lPosition)
        {
            Debug.Log("装备为 id = " + equipData.id);
            switch (equipData.equipType)
            {
                case EquipType.Armor:
                    DropEquip(HasArmorData(), GetArmorData().id, lPosition);
                    AddEquip(equipData);
                    break;
                case EquipType.Ring:
                    if (ring1Data == null)
                    {
                        AddEquip<EquipData>(equipData);
                    }
                    else
                    {
                        if(ring2Data != null)
                        {
                            DropEquip(ring2Data != null, ring2Data.id, lPosition);
                        }
                        AddEquip<EquipData>(equipData);
                    }
                    break;
                case EquipType.Necklace:
                    if (necklaceData != null)
                    {
                        DropEquip(necklaceData != null, necklaceData.id, lPosition);
                    }
                    AddEquip<EquipData>(equipData);
                    break;
                case EquipType.Glove:
                    if(gloveData != null)
                    {
                        DropEquip(gloveData != null, gloveData.id, lPosition);
                    }
                    AddEquip<EquipData>(equipData);
                    break;
                case EquipType.Trousers:
                    if (TrousersData != null)
                    {
                        DropEquip(TrousersData != null, TrousersData.id, lPosition);
                    }
                    AddEquip<EquipData>(equipData);
                    break;
                case EquipType.Shoe:
                    if (shoeData != null)
                    {
                        DropEquip(shoeData != null, shoeData.id, lPosition);
                    }
                    AddEquip<EquipData>(equipData);
                    break;
//                case EquipType.Shield:
//                    if (IsOneHanded(MainHand))
//                    {
//                        DropEquip(HasOffHand(), OffHand.id, lPosition);
//                        AddEquip<EquipData>(equipData);
//                    }
                    //break;
                case EquipType.Helmet:
                    if (HelmetData != null)
                    {
                        DropEquip(HelmetData != null, HelmetData.id, lPosition);
                    }
                    AddEquip<EquipData>(equipData);
                    break;
                case EquipType.None:
                    break;
                default:
                    break;
            }
        }
        public void DropEquip(bool onoff,string roleEquipID, Vector3 lPosition)
        {
            if (onoff)
            {
                if (GameObjectManager.Instance.GetPrefab(roleEquipID) != null)
                {
                    // 丢下身上的装备 add by TangJian 2017/09/01 21:41:20
                    GameObject giveUpItem = GameObjectManager.Instance.Spawn(roleEquipID);
                    SceneManager.Instance.CurrScene.DropItemEnterWithLocalPosition(giveUpItem, lPosition + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1.0f, 1.5f), Random.Range(-0.5f, 0.5f)));
                }
            }
        }

        public void DropEquipByPlace(EquipPlaceType equipPlaceType, string sceneId, Vector3 position)
        {
            EquipData equipData = GetEquipByPlace(equipPlaceType);
            if (equipData != null && !string.IsNullOrWhiteSpace(equipData.id))
            {
                string equipId = equipData.id;
                if (GameObjectManager.Instance.GetPrefab(equipId) != null)
                {
                    // 丢下身上的装备 add by TangJian 2017/09/01 21:41:20
                    GameObject giveUpItem = GameObjectManager.Instance.Spawn(equipId);
                    DropItemController dropItemController = giveUpItem.GetComponent<DropItemController>();
                    SceneManager.Instance.DropItemEnterSceneWithWorldPosition(dropItemController, sceneId,
                        position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1.0f, 1.5f),
                            Random.Range(-0.5f, 0.5f)));
                }
            }
        }

        public void RemoveEquipByPlace(EquipPlaceType type)
        {
            EquipData equipData = GetEquipByPlace(type);
            SetEquipByPlace(type, equipData);
        }

        public void AddEquip<T>(T Data)where T : EquipData
        {
            switch (Data.equipType)
            {
                case EquipType.Helmet:
                    HelmetData = Data;
                    break;
                case EquipType.Armor:
                    armorData = Data as ArmorData;
                    break;
                case EquipType.Ring:
                    if (ring1Data != null)
                    {
                        ring2Data = Data;
                    }
                    else
                    {
                        ring1Data = Data;
                    }
                    break;
                case EquipType.Necklace:
                    necklaceData = Data;
                    break;
                case EquipType.Glove:
                    gloveData = Data;
                    break;
                case EquipType.Trousers:
                    TrousersData = Data;
                    break;
                case EquipType.Shoe:
                    shoeData = Data;
                    break;
                case EquipType.Shield:
                    if (IsOneHanded(MainHand))
                    {
                        OffHand = Data as WeaponData;
                    }
                    break;
                case EquipType.None:
                    break;
                default:
                    if (IsOneHanded(Data))
                    {
                        if (HasMainHand())
                        {
                            if (IsOneHanded(MainHand))
                            {
                                OffHand = Data as WeaponData;
                            }
                            else
                            {
                                MainHand = Data as WeaponData;
                            }
                        }
                        else
                        {
                            MainHand=Data as WeaponData;
                        }
                    }
                    else
                    {
                        MainHand = Data as WeaponData;
                        OffHand = null;
                    }
                    break;
            }
        }
        
    }
}