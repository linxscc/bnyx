using System.Collections.Generic;

namespace Tang
{
    public class TreasureBoxData : ItemData
    {
        public string ID= "";
        public string prefab= "";
        public List<DropItem> dropItemList = new List<DropItem>();
    }
}