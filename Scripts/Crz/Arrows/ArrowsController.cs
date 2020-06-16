using UnityEngine;

/// <summary>
/// 弓箭控制器
/// </summary>
namespace Tang
{

    public class ArrowsController : MonoBehaviour, ITriggerDelegate
    {

        DamageController damageController;

        private Vector3 GritySpeed = new Vector3(0, -10);          //重力的速度向量，t时为0

        private Vector3 currentAngle;

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

        void Start()
        {
            currentAngle = Vector3.zero;

            damageController = GetComponentInChildren<DamageController>();
            var damageData = damageController.damageData;
            damageData.itriggerDelegate = this;

            damageData.DamageDirectionType = DamageDirectionType.Directional;           //朝向的伤害
            damageData.atk = 1;                                            //
            damageData.direction = new Vector3(1, 0, 0);
            damageData.force = new Vector3(5, 0, 0);
            damageData.hitType = 2;

            Destroy(gameObject, 5);
        }

        void Update()
        {

            speed += GritySpeed * Time.deltaTime;
            move(speed * Time.deltaTime);

            currentAngle.z = MathUtils.SpeedToDirection(speed);

            // Debug.Log("currentAngle = "+currentAngle);
            transform.eulerAngles = currentAngle;
        }

        void move(Vector3 moveVec3)
        {
            transform.position += moveVec3;
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
                    damageController.gameObject.SetActive(false);
                    Destroy(gameObject, 0);
                    break;
            }
            return true;
        }       
    }
}