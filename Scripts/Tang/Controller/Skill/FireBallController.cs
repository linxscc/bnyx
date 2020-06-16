using UnityEngine;

namespace Tang
{
    public class FireBallController : MonoBehaviour, ITriggerDelegate
    {
        Animator animator;
        DamageController damageController;
        public DamageController DamageController
        {
            get
            {
                if (damageController == null)
                {
                    damageController = GetComponentInChildren<DamageController>();
                }
                return damageController;
            }
        }
        public Vector3 speed = new Vector3();

        Direction direction = Direction.Right;

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();

            damageController = GetComponentInChildren<DamageController>();
            var damageData = damageController.damageData;
            // damageData.owner = gameObject;
            damageData.itriggerDelegate = this;

            Destroy(gameObject, 10);
        }



        void Update()
        {
            switch (animator.GetInteger("state"))
            {
                case 0:
                    {
                        if (speed.x > 0)
                        {
                            setDirection(Direction.Right);
                        }
                        else if (speed.x < 0)
                        {
                            setDirection(Direction.Left);
                        }
                        move(speed * Time.deltaTime);
                    }
                    break;
                case 1:
                    {
                        if (speed.x > 0)
                        {
                            setDirection(Direction.Right);
                        }
                        else if (speed.x < 0)
                        {
                            setDirection(Direction.Left);
                        }
                        move(speed * Time.deltaTime);
                    }
                    break;
                case 2:
                    break;
            }
        }

        public virtual void setDirection(Direction direction)
        {
            this.direction = direction;
            if (this.direction == Direction.Right)
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            else
            {
                transform.rotation = new Quaternion(0, 180, 0, 0);
            }
        }

        void move(Vector3 moveVec3)
        {
            transform.localPosition += moveVec3;
        }
        // 触发器 add by TangJian 2017/07/31 16:07:33
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
        public bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.DamageHit:
                    animator.SetInteger("state", 2);
                    damageController.gameObject.active = false;
                    Destroy(gameObject, 1);
                    break;
            }
            return true;
        }
    }
}