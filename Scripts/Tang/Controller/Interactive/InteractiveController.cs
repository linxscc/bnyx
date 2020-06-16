using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class InteractiveController : MonoBehaviour
    {
        IInteractive interactive;
        protected Dictionary<object, InteractiveController> interactiveControllers = new Dictionary<object, InteractiveController>();
        protected List<InteractiveController> interactiveControllerRemoveList = new List<InteractiveController>();

        void Start()
        {
            interactive = GetComponentInParent<IInteractive>();
        }

        void Update()
        {
            foreach (var interactiveController in interactiveControllerRemoveList)
            {
                if (interactiveControllers.ContainsKey(interactiveController))
                {
                    interactiveControllers.Remove(interactiveController);
                }
            }
            interactiveControllerRemoveList.Clear();
        }

        protected void addInteractiveController(InteractiveController interactiveController)
        {
            if (interactiveControllers.ContainsKey(interactiveController) == false)
            {
                interactiveControllers.Add(interactiveController, interactiveController);
            }
        }

        protected void removeInteractiveController(InteractiveController interactiveController)
        {
            interactiveControllerRemoveList.Add(interactiveController);
        }

        protected void OnTriggerEnter(Collider other)
        {
            InteractiveController interactiveController = other.gameObject.GetComponent<InteractiveController>();
            if (interactiveController)
            {
                addInteractiveController(interactiveController);
                if (interactive != null)
                {
                    interactive.OnInteractEnter(other.gameObject.transform.parent.gameObject);
                }
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            InteractiveController interactiveController = other.gameObject.GetComponent<InteractiveController>();
            if (interactiveController)
            {
                removeInteractiveController(interactiveController);
                if (interactive != null)
                {
                    interactive.OnInteractExit(other.gameObject.transform.parent.gameObject);
                }
            }
        }

        public void sendEvent(Event evt)
        {
            if (interactive != null)
            {
                interactive.OnInteractEvent(evt);
            }

        }

        public GameObject getFirstGameObject()
        {
            foreach (var item in interactiveControllers)
            {
                if (!item.Key.Equals(null))
                {
                    return item.Value.gameObject.transform.parent.gameObject;
                }
                else
                {
                    removeInteractiveController(item.Value);
                }
                break;
            }
            return null;
        }

        public List<GameObject> getGameObjectList()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (var item in interactiveControllers)
            {
                if (!item.Key.Equals(null))
                {
                    list.Add(item.Value.gameObject.transform.parent.gameObject);
                }
                else
                {
                    removeInteractiveController(item.Value);
                }
            }
            return null;
        }
    }
}