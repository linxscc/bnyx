using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Tang
{
    public class ExplodeItemController : MyMonoBehaviour, ITriggerDelegate
    {
        public List<string> explodeItemIdList = new List<string>();
        public List<GameObject> explodeItemList = new List<GameObject>();
        [SerializeField] TreasureBoxData treasureBoxData;
        public TreasureBoxData TreasureBoxData { get { return treasureBoxData; } set { treasureBoxData = value; } }
        void Start()
        {
            MeshCollider meshCollider = transform.GetComponentInChildren<MeshCollider>();

            GameObject DamageTarget = Tools.GetChild(gameObject, "DamageTarget", true);
            DamageTarget.layer = LayerMask.NameToLayer("Interaction");
            DamageTarget.tag = "DamageTarget";

            TriggerController triggerController = Tools.AddComponent<TriggerController>(DamageTarget);

            BoxCollider boxCollider = Tools.AddComponent<BoxCollider>(DamageTarget);
            DamageTarget.transform.localScale = meshCollider.bounds.size;
            boxCollider.transform.localPosition = new Vector3(0, DamageTarget.transform.localScale.y / 2, 0);
            boxCollider.isTrigger = true;

            Rigidbody rigidbody = Tools.GetComponent<Rigidbody>(DamageTarget, true);
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }

        void explode(Vector3 force)
        {
            foreach (var item in explodeItemIdList)
            {
                GameObject itemGameObject = GameObjectManager.Instance.Spawn(item);

                var bloodRigidbody = itemGameObject.GetComponent<Rigidbody>();
                var render = itemGameObject.GetComponentInChildren<MeshRenderer>();

                itemGameObject.transform.parent = gameObject.transform.parent;
                itemGameObject.transform.position = gameObject.transform.position + new Vector3(0, 1, 0) + Tools.getRandomVector3(0.2f);

                bloodRigidbody.AddForce(force.normalized * 100.0f + Tools.getRandomVector3(400));

                itemGameObject.DOFade(0, 2).OnComplete(() =>
                {
                    GameObjectManager.Instance.Despawn(itemGameObject);
                });
            }
        }

        void OnHurt(DamageData damageData)
        {
            explode(damageData.force);
            Destroy(gameObject);
            // switch (animator.GetInteger("state"))
            // {
            //     case 0:
            //         animator.SetInteger("state", 1);
            //         break;
            //     case 1:
            //         break;
            // }
        }
        public GameObject GetGameObject()
        {
            return gameObject;
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