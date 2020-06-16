
namespace Tang
{
    [System.Serializable]
    public class DropItem
    {
        public int weight;
        public float chance;
        public GameObjectType objectType;
        public string itemId;
        public ItemType itemType;
        public int count = 1;
    }
}