using System.Collections.Generic;
using UnityEngine;


namespace Tang
{
    public class GamingPlayerUIController : MyMonoBehaviour
    {
        private RectTransform rectTransform;
        private GameObject player;
        private RoleController playerController;
        public GameObject Player
        {
            get { return player; }
            set { player = value; Init(); }
        }

        void Init()
        {
            if (player)
            {
                rectTransform = GetComponent<RectTransform>();
                playerController = player.GetComponent<RoleController>();

                StateInit();
                ItemInit();
            }
        }

        void OnGUI()
        {
            if (playerController)
            {
                StateUpdate();
                ItemUpdate();
            }
        }

        // StateUI add by TangJian 2017/10/24 18:39:53
        BarUIController hpBarController;
        BarUIController mpBarController;

        void StateInit()
        {
            hpBarController = Tools.GetChild(gameObject, "State/Hp").GetComponent<BarUIController>();
            mpBarController = Tools.GetChild(gameObject, "State/Hp").GetComponent<BarUIController>();
        }

        public void SetHp(float cur, float max)
        {
            hpBarController.SetValue(cur, max);
        }
        public void SetMp(float cur, float max)
        {
            mpBarController.SetValue(cur, max);
        }

        void StateUpdate()
        {
            SetHp(playerController.RoleData.Hp, playerController.RoleData.FinalHpMax);
        }

        // ItemUI add by TangJian 2017/10/24 17:08:40
        private List<GameObject> itemList = new List<GameObject>();
        private int itemFocusIndex = 0;
        private SimpleFSM itemFSM;
        private GameObject test;
        private Vector3 itemPosition;

        void ItemInit()
        {
            var itemObject = gameObject.GetChild("Item");
            for (int i = 0; i < 2; i++)
            {
                var item = itemObject.GetChild("item" + i, true);
                UGUIGO ug = item.AddComponent<UGUIGO>();
                ug.SetShowObject(GameObjectManager.Instance.Spawn("Armor-1"));
                ug.Offset = new Vector2(0, -0.73f);
                ug.Scale = new Vector2(8, 8);
                ug.Show();

                var width = 128;

                item.transform.parent = itemObject.transform;
                item.transform.localPosition = new Vector3(width / 2 + i * width, 0, 0);
            }
        }

        void ItemUpdate()
        {
        }
    }
}