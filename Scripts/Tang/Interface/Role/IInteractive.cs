using UnityEngine;

namespace Tang
{
    public interface IInteractive
    {
        void OnInteractEnter(GameObject gameObject);
        void OnInteractExit(GameObject gameObject);
        void OnInteractEvent(Event evt);
    }
}