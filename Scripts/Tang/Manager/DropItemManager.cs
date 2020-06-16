using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class DropItemManager : MonoBehaviour
    {
        public List<Sprite> Sprites;
        GameObject dropItem;

        void Start()
        {
            GetDropItem();
        }

        private async void GetDropItem()
        {
            dropItem = await AssetManager.LoadAssetAsync<GameObject>("Assets/Resources_moved/Prefabs/DropItem/DropItem.prefab");
        }
        public Sprite getSprite(string key)
        {
            foreach (var sprite in Sprites)
            {
                if (sprite.name == key)
                {
                    return sprite;
                }
            }
            return null;
        }

        public GameObject createWeapon(string weaponId)
        {
            return null;
        }
    }
}
