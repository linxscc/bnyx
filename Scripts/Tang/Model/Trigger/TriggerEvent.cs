using UnityEngine;

namespace Tang
{
    public class TriggerEvent
    {
        public TriggerEventType type = TriggerEventType.Default;
        public TriggerController selfTriggerController;
        public TriggerController otherTriggerController;
        public Collider colider;
        public Vector3 colidePoint;
        public Object data;
    }
}
