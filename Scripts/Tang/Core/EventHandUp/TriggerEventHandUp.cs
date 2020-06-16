using UnityEngine;

namespace Tang
{
    public class TriggerEventHandUp : MonoBehaviour
    {
        GameObject parent;
        void Start()
        {
            parent = transform.parent.gameObject;
        }

        protected void OnTriggerEnter(Collider other)
        {
            parent.SendMessage("OnTriggerEnter", other);
            // SendMessageUpwards("OnTriggerEnter", other);
        }

        protected void OnTriggerExit(Collider other)
        {
            parent.SendMessage("OnTriggerExit", other);
            // SendMessageUpwards("OnTriggerExit", other);
        }

    }
}