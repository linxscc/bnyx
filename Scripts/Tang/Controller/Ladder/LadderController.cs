using UnityEngine;

namespace Tang
{
    public enum laddertype
    {
        Right=0,
        Left=1,
        Center=2,
    }
    public class LadderData
    {
        public laddertype laddertype;
    }
    public class LadderController : MonoBehaviour, ITriggerDelegate
    {
        int onlyid;
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        public laddertype laddertype;
        public bool OnEvent(Event evt)
        {
            return true;
        }

        public void OnTriggerIn(TriggerEvent evt)
        {
            if(evt.otherTriggerController.ITriggerDelegate.GetGameObject().tag == "Role"
                || evt.otherTriggerController.ITriggerDelegate.GetGameObject().tag == "Player")
            {
                RoleController humanController = evt.otherTriggerController.ITriggerDelegate.GetGameObject().GetComponent<RoleController>();
                if (humanController != null)
                {
                    humanController.AddClimbLadder(onlyid.ToString(),this);
                }
            }
        }

        public void OnTriggerKeep(TriggerEvent evt)
        {

        }

        public void OnTriggerOut(TriggerEvent evt)
        {
            if (evt.otherTriggerController.ITriggerDelegate.GetGameObject().tag == "Role"
                || evt.otherTriggerController.ITriggerDelegate.GetGameObject().tag == "Player")
            {
                RoleController humanController = evt.otherTriggerController.ITriggerDelegate.GetGameObject().GetComponent<RoleController>();
                if (humanController != null)
                {
                    humanController.RemoveClimbLadder(onlyid.ToString());
                }
            }
        }

        // Use this for initialization
        void Start ()
        {
            BoxCollider box = GetComponent<BoxCollider>();
            box.enabled = false;
            onlyid = Tools.getOnlyId();

        }
	
	    // Update is called once per frame
	    void Update () {
		
	    }
    }
}

