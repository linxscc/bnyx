using System.Collections.Generic;

namespace Tang
{
    [System.Serializable]
    public class TriggerData
    {
        public TriggerType type = TriggerType.Default;
        public List<TriggerType> watchTypes;
    }
}