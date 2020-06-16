using UnityEngine;

namespace Tang
{
    public class TriggerControllerGroup : MonoBehaviour
    {
        private void Start()
        {
            transform.RecursiveComponent<TriggerController>((TriggerController tc, int depth) =>
            {
                tc.id = tc.id + GetInstanceID();
            }, 1, 999);
        }
    }
}
