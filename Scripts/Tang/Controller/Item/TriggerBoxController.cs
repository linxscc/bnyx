using UnityEngine;

namespace Tang
{
    public class TriggerBoxController : PlacementController
    {
        public string TargetName = "Player1";
        public float DelayTime = 5;
        private string coId;

        public override object Data
        {
            get
            {
                return this;
            }

            set
            {
                TriggerBoxController newThis = value as TriggerBoxController;
                transform.localScale = newThis.transform.localScale;
                transform.localRotation = newThis.transform.localRotation;
                TargetName = newThis.TargetName;
                DelayTime = newThis.DelayTime;
            }
        }

        public override void Start()
        {
            base.Start();
        }

        public override void OnTriggerIn(TriggerEvent evt)
        {
            if (evt.otherTriggerController != null)
            {
                if (TargetName == evt.otherTriggerController.ITriggerDelegate.gameObject.name)
                {
                    Debug.Log("目标进入触发器");

                    coId = TargetName + GetInstanceID();
                    DelayFunc(coId, () =>
                    {
                        SceneEventManager.Instance.ObjStateChange(Id, 1);
                    }, DelayTime);
                }
            }
        }

        public override void OnTriggerOut(TriggerEvent evt)
        {
            if (evt.otherTriggerController != null)
            {
                if (TargetName == evt.otherTriggerController.ITriggerDelegate.gameObject.name)
                {
                    coId = TargetName + GetInstanceID();
                    RemoveCoroutine(coId);
                }
            }
        }

        private void OnDrawGizmos()
        {
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(new Vector3(0, 0.5f, 0), new Vector3(1, 1, 1));
            Gizmos.matrix = oldMatrix;
        }
    }
}