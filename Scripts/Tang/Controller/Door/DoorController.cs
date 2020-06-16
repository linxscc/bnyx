using UnityEngine;
//using DG.Tweening;

namespace Tang
{
    public class DoorController : MyMonoBehaviour, ITriggerDelegate
    {
        DoorData doorData = new DoorData();
        public DoorData DoorData
        {
            get { return doorData; }
            set
            {
                doorData = value;
                if (doorData == null)
                    doorData = new DoorData();

                //if (doorData.state == DoorState.Opened)
                //{
                //    fsm.SendEvent("open");
                //}
                //else if (doorData.state == DoorState.Closed)
                //{
                //    fsm.SendEvent("close");
                //}
            }
        }

        private Animator _animator;
        
        void Start()
        {
            _animator = GetComponentInChildren<Animator>();
        }


        public bool IsOpened()
        {
            return true;
            //return fsm.CurrStateName == "opened";
        }

        public bool IsClosed()
        {
            return false;
            //return fsm.CurrStateName == "closed";
        }

        public void OnDoorOpened()
        {
            //fsm.SendEvent("openning2opened");
        }

        public void OnDoorClosed()
        {
            //fsm.SendEvent("closing2closed");
        }
        
        public bool OnEvent(Event evt)
        {
            //RoleController roleController = (evt.Data as GameObject).GetComponent<RoleController>();
            //if (evt.Type == EventType.Interact)
            //{
            //    if (fsm.CurrStateName == "closed")
            //    {
            //        if (doorData != null && doorData.key != null && doorData.key != "")
            //        {
            //            int itemIndex = roleController.RoleData.OtherItemList.FindIndex((ItemData id) =>
            //            {
            //                if (id.id == doorData.key)
            //                {
            //                    return true;
            //                }
            //                return false;
            //            });

            //            ItemData item = roleController.RoleData.OtherItemList[itemIndex];

            //            if (item != null)
            //            {
            //                Open();
            //                roleController.ConsumeOtherItem(itemIndex);
            //            }
            //        }
            //        else
            //        {
            //            Open();
            //        }
            //    }
            //    // else if (fsm.CurrStateName == "opened")
            //    // {
            //    //     fsm.SendEvent("close");
            //    // }
            //}
            return true;
        }

        public void Open()
        {
            //fsm.SendEvent("open");
        }

        public void Close()
        {
            //fsm.SendEvent("close");
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void OnTriggerIn(TriggerEvent evt)
        {

        }

        public void OnTriggerOut(TriggerEvent evt)
        {

        }

        public void OnTriggerKeep(TriggerEvent evt)
        {

        }
    }
}