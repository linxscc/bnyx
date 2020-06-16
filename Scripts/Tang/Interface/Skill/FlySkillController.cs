using UnityEngine;

namespace Tang
{
    public abstract class FlySkillController : SkillController
    {
        private float intensity;
        public virtual float Intensity { set { intensity = value; } get { return intensity; } }
        private Vector3 angleIntensity;
        public virtual Vector3 AngleIntensity { set { angleIntensity = value; } get { return angleIntensity; } }
        
        // 速度 add by TangJian 2018/05/09 14:47:36
        public override Vector3 Speed
        {
            set { MainRigidbody.velocity = value; }
            get
            {
                return MainRigidbody.velocity;
            }
        } // 设置飞行技能速度 add by TangJian 2018/04/13 15:53:39

        public virtual bool UseGravity
        {
            set
            {
                MainRigidbody.useGravity = value;
            }
            get
            {
                return MainRigidbody.useGravity;
            }
        } // 设置重力是否可用 add by TangJian 2018/04/13 15:53:41

        public override void Awake()
        {
            InitAnimator();
            
            InitSprite();

            InitRigidbody();
            
            InitCollider();
            
            InitDamage();
            
            InitHitAndHurt();

            Startup();
        }
        
        public virtual void OnHit()
        {
            // 击中目标 add by TangJian 2018/01/15 15:05:39
            MainDamageController.gameObject.SetActive(false);

            State = 2;

            Speed = Vector3.zero;
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
    }
}