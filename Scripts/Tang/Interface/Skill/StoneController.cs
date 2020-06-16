using UnityEngine;

namespace Tang
{
	public class StoneController : FlySkillController 
	{
		public override Vector3 Speed
        {
            set
            {
                MainRigidbody.velocity = value * 3;

                if (value.magnitude != 0)
                    gameObject.transform.localEulerAngles = new Vector3(0, Tools.GetDegree(MainRigidbody.velocity.x, MainRigidbody.velocity.y), 0);
            }
            get
            {
                return MainRigidbody.velocity;
            }
        }
		public override void Awake()
        {
            base.Awake();

            MainRigidbody.useGravity = true;
            MainRigidbody.freezeRotation = true;

        }
		public override void OnHit()
		{
			MainDamageController.gameObject.SetActive(false);
		}
        public override bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.DamageHit:
                    OnHit();
                    break;
            }
            return true;
        }
		void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground")||other.gameObject.layer == LayerMask.NameToLayer("SceneComponent"))
            {
                Debug.Log("设置状态!!!!!!");
				MainDamageController.gameObject.SetActive(false);
                // 冻结z方向旋转 add by TangJian 2018/05/08 20:04:58
                MainRigidbody.freezeRotation = true;
                // transform.eulerAngles = Vector3.zero;
                Speed = Vector3.zero;
                MainRigidbody.isKinematic = true;

                State = 2;
            }
        }


	}
}

