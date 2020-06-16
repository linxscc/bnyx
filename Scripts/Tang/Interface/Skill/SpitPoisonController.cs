using UnityEngine;

namespace Tang
{
    public class SpitPoisonController : FlySkillController
    {
        public override void Awake()
        {
            base.Awake();

            // 禁用伤害 add by TangJian 2018/05/08 21:26:42
            //MainDamageController.gameObject.SetActive(false);
        }

        public override void OnFinished()
        {
            Destroy(gameObject);
        }

        public override void OnHit()
        {
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Debug.Log("设置状态!!!!!!");

                // 冻结z方向旋转 add by TangJian 2018/05/08 20:04:58
                MainRigidbody.freezeRotation = true;
                transform.eulerAngles = Vector3.zero;
                Speed = Vector3.zero;
                MainRigidbody.isKinematic = true;

                State = 2;
            }
        }
        public override void InitDamage()
        {

        }
        public override void OnTriggerIn(TriggerEvent evt)
        {
        }

        public override void OnUpdate()
        {
            transform.eulerAngles = new Vector3(0, 0, MathUtils.SpeedToDirection(Speed));
        }
    }
}

