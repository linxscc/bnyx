using UnityEngine;

namespace Tang
{
    public interface ITriggerDelegate : IEventDelegate
    {
        GameObject GetGameObject();
        GameObject gameObject { get; }

        Transform transform { get; }

        void OnTriggerIn(TriggerEvent evt);
        void OnTriggerOut(TriggerEvent evt);
        void OnTriggerKeep(TriggerEvent evt);

        // void OnCollisionIn(Collision other);
        // void OnCollisionOut(Collision other);
        // void OnCollisionKeep(Collision other);
    }
}