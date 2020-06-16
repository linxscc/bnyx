using UnityEngine;

/// <summary>
/// ��ҽű����£���һ����������ȥ��Ч�� by crz 2017.8.8
/// </summary>
namespace Tang
{
    public class CoinController : MonoBehaviour, ITriggerDelegate
    {
        GameObject player;
        private bool ifGet;

        public float getRange = 0.5f;
        public float moveSpeed = 0.5f;

        Rigidbody mainRigidbody;
        Animator animator;

        public GameObject GetGameObject() { return gameObject; }

        void Start()
        {
            mainRigidbody = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();

            ifGet = false;
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
        {
        }

        public void OnTriggerOut(TriggerEvent evt)
        { }

        public void OnTriggerKeep(TriggerEvent evt)
        {

        }

        public bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.ItemPickUp:
                    //���봥������Χ
                    Debug.Log("coming trigger evt.type = " + evt.GetType() + "evt.data = " + evt.Data);
                    if (evt.Data != null)
                    {
                        player = evt.Data as GameObject;
                        ifGet = true;
                    }
                    break;
                case EventType.DamageHit:
                    break;
            }
            return true;
        }

        //��Ӹ��·������ý��Ʈ���ɫ
        private void Update()
        {
            if (ifGet == true)
            {
                float distance = Vector3.Distance(gameObject.transform.position, player.transform.position);
                if (distance <= getRange)
                {
                    Destroy(gameObject);
                }
                else if (player != null)
                {
                    Debug.Log("is comming here? ----- crz ---- coin");
                    gameObject.transform.position += (player.transform.position - gameObject.transform.position).normalized * moveSpeed * Time.deltaTime;
                }
            }
        }

    }

}