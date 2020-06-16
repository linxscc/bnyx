using UnityEngine;

namespace Tang
{
    public class ItemController : MonoBehaviour, ITriggerDelegate
    {
        Rigidbody mainRigidbody;
        Animator animator;
        public GameObject GetGameObject() { return gameObject; }

        void Start()
        {
            mainRigidbody = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
        }

        void updateAnimatorState()
        {
            animator.GetInteger("state");
        }

        void OnHurt(DamageData damageData)
        {
            switch (animator.GetInteger("state"))
            {
                case 0:
                    animator.SetInteger("state", 1);
                    break;
                case 1:
                    break;
            }
        }
        public void OnTriggerIn(TriggerEvent evt)
        { }

        public void OnTriggerOut(TriggerEvent evt)
        { }
        public void OnTriggerKeep(TriggerEvent evt)
        {

        }
        public bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.DamageHurt:
                    OnHurt(evt.Data as DamageData);
                    break;
                case EventType.DamageHit:
                    break;
            }
            return true;
        }

    }
}