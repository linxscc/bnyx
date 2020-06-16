namespace Tang
{
    [System.Serializable]
    public class DropItemData : PlacementData
    {
        public ItemType type;
        public string id;
        public int count = 1;
    }
}