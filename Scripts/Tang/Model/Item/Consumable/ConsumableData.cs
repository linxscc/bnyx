namespace Tang
{
    public enum ConsumableUseType
    {
        deposit=0,
        ImmediateUse =1,
    }
    [System.Serializable]
    public class ConsumableData : ItemData
    {
        public ConsumableUseType consumableUseType = ConsumableUseType.deposit;
        public string buffId;
    }
}