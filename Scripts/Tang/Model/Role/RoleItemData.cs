using System;
using System.Collections.Generic;
using UnityEngine;


namespace Tang
{
    [Serializable]
    public class RoleItemData
    {
        [SerializeField] private int focusIndex = 0;
        [SerializeField] private List<ItemData> itemList = new List<ItemData>();

        public int FocusIndex { get { return focusIndex; } set { focusIndex = value; } }
        public List<ItemData> ItemList { get { return itemList; } set { itemList = value; } }
    }
}