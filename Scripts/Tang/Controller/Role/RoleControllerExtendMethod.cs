using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Tang
{
    public static class RoleControllerExtendMethod
    {
        // 角色扣血 
        public static float DoLoseHp(this RoleController target, float atk,float HurtRatio =1, bool reduce = true)
        {
            // 新的伤害计算公式 add by TangJian 2019/3/23 16:19
            float avoidInjuryRate = Mathf.Pow(target.RoleData.FinalDef, 0.2f) / 10f;
            float damage = atk * (1.0f - avoidInjuryRate) * HurtRatio ;
            
            // 扣血 add by TangJian 2017/07/14 18:02:17
            if (reduce)
            {
                target.RoleData.Hp -= damage;
            }
            
            return damage;
        }

        public static void PlayLoseHpAnim(this RoleController target, float damage, bool isCritical)
        {
            #region //飘字动画 2019.3.25

            if (isCritical)
            {
                UIHurtAni.Instance.PlayUIHurt(target.gameObject,HurtAniEnum.Cricital,((int)damage).ToString());
            }
            else
            {
                UIHurtAni.Instance.PlayUIHurt(target.gameObject,HurtAniEnum.Ordinary,((int)damage).ToString());
            }                
            #endregion
        }

        // 掉落物品列表 add by TangJian 2018/01/04 17:24:47
        public static void DropItemList(this RoleController target)
        {
            if (target.RoleData.DropItemList != null && target.RoleData.DropItemList.Count > 0)
            {
                foreach (var item in target.RoleData.DropItemList)
                {
                    if (item.chance >= Random.Range(1, 100))
                    {
                        target.DropItem(item.itemId);
                    }
                }
            }
        }

        // 掉落物品 add by TangJian 2018/01/04 17:25:00
        public static async Task<GameObject> DropItem(this RoleController target, string itemId)
        {
            GameObject go = await AssetManager.InstantiateDropItem(itemId);
            DropItemController dropItemController = go.GetComponent<DropItemController>();
            Debug.Assert(dropItemController!=null);
            
            SceneManager.Instance.DropItemEnterSceneWithLocalPosition(dropItemController, target.SceneId, target.transform.localPosition + new Vector3(0, 1, 0));
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            
            if (rigidbody != null)
            {
                Vector3 targetPos = new Vector3(Random.Range(-2f, 2f), -1f, Random.Range(-2f, 2f));
                NavMeshHit hit;
                if (NavMesh.SamplePosition(go.transform.TransformPoint(targetPos), out hit, 1.0f, NavMesh.AllAreas))
                {
                    targetPos = go.transform.InverseTransformPoint(hit.position);
                }

                rigidbody.velocity = Tools.GetFlyToPosSpeed(new Vector2(1f, Random.Range(0.5f, 2f)), targetPos, -60f);
            }

            return go;
        }

        public static async void DropItemCustom(this RoleController target)
        {
            List<string> dropItemIdList = DropItemsDataAsset.Instance.GetDropItemIdList(target.RoleData.difficultyLevel, target.RoleData.Id);
            foreach (var itemId in dropItemIdList)
            {
                await target.DropItem(itemId);
            }
            
            // 掉落魂 add by TangJian 2019/3/13 12:34
            int soulCount = DropItemsDataAsset.Instance.GetDropSoulCount(target.RoleData.difficultyLevel, target.RoleData.Id);
            GameObject dropItemObject = await target.DropItem("Soul");
            if (dropItemObject != null)
            {
                DropItemController dropItemController = dropItemObject.GetComponent<DropItemController>();
                if (dropItemController != null)
                {
                    dropItemController.itemData.count = soulCount;
                }
            }
            
            
            // 5%的几率掉落身上的装备 add by TangJian 2019/3/22 15:40
            target.DropOwnerItems();
        }

        public static void ConsumeOtherItem(this RoleController target, int index)
        {
            target.RoleData.OtherItemList.RemoveAt(index);
        }

        public static void DropOwnerItems(this RoleController target)
        {
            if (Tools.RandomWithWeight(5, 95) == 0)
            {
                List<EquipPlaceType> equipDatas = new List<EquipPlaceType>()
                {
                    EquipPlaceType.Helmet,
                    EquipPlaceType.Necklace,
                    EquipPlaceType.Glove,
                    EquipPlaceType.Trousers,
                    EquipPlaceType.Shoe,
                    EquipPlaceType.Ring1,
                    EquipPlaceType.Ring2,
                };

                int index = Tools.RandomWithWeight(equipDatas, (data, i) =>
                {
                    EquipData equipData = target.RoleData.EquipData.GetEquipByPlace(data);   
                    if (equipData != null && !string.IsNullOrWhiteSpace(equipData.id))
                    {
                        return 1;
                    }

                    return 0;
                });

                if (index >= 0)
                {
                    target.RoleData.EquipData.DropEquipByPlace(equipDatas[index], target.SceneId, target.transform.position);
                    target.RoleData.EquipData.RemoveEquipByPlace(equipDatas[index]);
                }
            }
        }
        
        public static void JuageEquipPossessType(this RoleController target, WeaponData weaponData)
        {
            if (weaponData.possessType != PossessType.None && target._roleData.EquipData.MainHand.possessType == PossessType.Both_Hand)
            {
                target.DropEquipByPlace(EquipPlaceType.Weapon_Main);
                target.DropEquipByPlace(EquipPlaceType.Weapon_Secondry);
            }
            switch (weaponData.possessType)
            {
                case PossessType.Main_Hand:
                    if (target.HasEquipByPlace(EquipPlaceType.Weapon_Main))
                        target.DropEquipByPlace(EquipPlaceType.Weapon_Main);
                    target.AddEquip(weaponData);
                    break;
                case PossessType.Secondary_Hand:
                    if (target.HasEquipByPlace(EquipPlaceType.Weapon_Secondry))
                        target.DropEquipByPlace(EquipPlaceType.Weapon_Secondry);
                    target.AddEquip(weaponData);
                    break;
                case PossessType.Both_Hand:
                    target.DropEquipByPlace(EquipPlaceType.Weapon_Main);
                    target.DropEquipByPlace(EquipPlaceType.Weapon_Secondry);
                    target.AddEquip(weaponData);
                    break;
                case PossessType.Either_Hand:
                    if (!target.HasEquipByPlace(EquipPlaceType.Weapon_Main))
                    {
                        target.DropEquipByPlace(EquipPlaceType.Weapon_Main);
                        target.AddEquip(weaponData);
                    }
                    else
                    {
                        target.DropEquipByPlace(EquipPlaceType.Weapon_Secondry);
                        target.AddEquip(weaponData);
                    }
                    break;
                default:
                    throw new Exception("PossessType.None");
            }
        }

        public static bool HasEquipByPlace(this RoleController target, EquipPlaceType equipPlaceType)
        {
            var equipData = target._roleData.EquipData;
            var data = equipData.GetEquipByPlace(equipPlaceType);
            return data != null && string.IsNullOrEmpty(data.id) == false;
        }

        public static void DropEquipByPlace(this RoleController target, EquipPlaceType equipPlaceType)
        {
            var equipData = target._roleData.EquipData;

            var dropItem = equipData.GetEquipByPlace(equipPlaceType);
            equipData.SetEquipByPlace(equipPlaceType, null);

            target.DropItem(dropItem.id);
        }

        public static void AddEquip(this RoleController target,WeaponData weaponData)
        {
            switch (weaponData.possessType)
            {
                case PossessType.Main_Hand:
                    target._roleData.EquipData.SetEquipByPlace(EquipPlaceType.Weapon_Main,weaponData);
                    break;
                case PossessType.Secondary_Hand: 
                    target._roleData.EquipData.SetEquipByPlace(EquipPlaceType.Weapon_Secondry,weaponData);
                    break;
                case PossessType.Both_Hand:
                    target._roleData.EquipData.SetEquipByPlace(EquipPlaceType.Weapon_Main,weaponData);
                    break;
                case PossessType.Either_Hand:
                    if (!target.HasEquipByPlace(EquipPlaceType.Weapon_Main))
                    {
                        target._roleData.EquipData.SetEquipByPlace(EquipPlaceType.Weapon_Main,weaponData);
                    }
                    else
                    {
                        target._roleData.EquipData.SetEquipByPlace(EquipPlaceType.Weapon_Secondry,weaponData);
                    }
                    break;
                case PossessType.None:
                default:
                    throw new Exception("拾取错误！");
                    break;
            }
        }
        
    }
}