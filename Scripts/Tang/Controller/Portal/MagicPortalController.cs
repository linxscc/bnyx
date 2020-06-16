using UnityEngine;

namespace Tang
{
    public class MagicPortalController : MonoBehaviour, ITriggerDelegate
    {
        private Animator _animator;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }


        public bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.DamageHurt:
                    break;
                case EventType.DamageHit:
                    break;
                case EventType.Interact:
                    GameStart.Instance.ReloadNextLevel(GameManager.Instance.Player1.RoleData);
                    break;
            }
            return true;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void OnTriggerIn(TriggerEvent evt)
        {
//            GameObject gameObject = evt.otherTriggerController.ITriggerDelegate.gameObject;
//            if (gameObject.name == "Player1")
//            {
//                GameStart.Instance.ReloadNextLevel(GameManager.Instance.Player1.RoleData);
//            }
        }

        public void OnTriggerOut(TriggerEvent evt)
        {
          
        }

        public void OnTriggerKeep(TriggerEvent evt)
        {
           
        }
    }
}