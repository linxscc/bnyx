using System;
using UnityEngine;

namespace Tang
{
    public class SimpleTriggerController : MonoBehaviour
    {
        public event Action<Collider> OnTriggerIn;
        public event Action<Collider> OnTriggerOut;
        
        private void OnTriggerEnter(Collider other)
        {
            if (TagTriggerManager.Instance.FindTirggerBool(this.gameObject.tag, other.gameObject.tag))
            {
                if(OnTriggerIn != null)
                    OnTriggerIn(other); 
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (TagTriggerManager.Instance.FindTirggerBool(this.gameObject.tag, other.gameObject.tag))
            {
                if (OnTriggerOut != null)
                    OnTriggerOut(other);
            }
        }
    }
}