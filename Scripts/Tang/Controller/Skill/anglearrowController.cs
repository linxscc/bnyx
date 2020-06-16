using UnityEngine;
using Spine.Unity;

namespace Tang
{
    public class anglearrowController : FlySkillController
    {
        //Rigidbody rigidbody;
        SkeletonAnimator skeletonAnimation;
        DamageController damageController;
        RoleController roleController;

        private Vector3 speed = new Vector3();
        bool IsGrounded = false;
        bool speedset = true;
        public float GrivityAcceleration = 20f;

        public override void FlyTo(Vector3 pos)
        {
            var offset = pos - transform.position;
            Speed = Tools.GetFlyToPosSpeed(new Vector2(1f, 0.5f), offset, -GrivityAcceleration);
//            Speed = offset.normalized * skillData.speed.x;
        }

        // 速度 add by TangJian 2018/05/09 14:47:36
        public override Vector3 Speed { set { speed = value; } get { return speed; } }

        // 设置飞行技能速度 add by TangJian 2018/04/13 15:53:39
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public override void Awake()
        {
            // 刚体 add by TangJian 2018/01/16 16:15:37
            base.Awake();
            // 动画 add by TangJian 2018/01/15 15:04:59
            skeletonAnimation = GetComponentInChildren<SkeletonAnimator>();
            //UseGravity = false;
            //MainRigidbody.isKinematic = true;
            // 伤害 add by TangJian 2018/01/15 15:05:04
            {
                damageController = GetComponentInChildren<DamageController>();
                var damageData = damageController.damageData;
                damageData.itriggerDelegate = this;
                damageData.force = Vector3.zero;
            }
        }

        void Start()
        {
//            Destroy(gameObject, 10);
//            if (((BaseSkillController) this).Owner != null)
//            {
//                roleController = ((BaseSkillController) this).Owner.GetComponent<RoleController>();
//                float yuanangle=roleController.RoleAnimator.GetFloat("MagicAngle");
//                Vector3 see = new Vector3();
//                switch (roleController.GetDirection())
//                {
//                    case Direction.Left:
//                        see = new Vector3(Mathf.Cos((180f-yuanangle)*Mathf.Deg2Rad), skillData.speed.y, Mathf.Sin((180f - yuanangle) * Mathf.Deg2Rad)).normalized*skillData.speed.x;
//                        break;
//                    case Direction.Right:
//                        see = new Vector3(Mathf.Cos((360f + yuanangle) * Mathf.Deg2Rad), skillData.speed.y, Mathf.Sin((360f + yuanangle) * Mathf.Deg2Rad)).normalized * skillData.speed.x;
//                        break;
//                }
//                Speed = see;
//            }
//            roleController.RoleAnimator.SetFloat("MagicAngle",0f);
        }


        // 触发器 add by TangJian 2017/07/31 16:07:33



        public override void InitDamage()
        {
            MainDamageController = gameObject.GetChild("Trigger").GetChild("Damage").GetComponent<DamageController>();
            Debug.Assert(MainDamageController != null);

            var damageData = MainDamageController.damageData;
            damageData.itriggerDelegate = this;
            damageData.force = Vector3.zero;
        }

        public override bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.DamageHit:
                    // 击中目标 add by TangJian 2018/01/15 15:05:39
                    damageController.gameObject.SetActive(false);
                    Destroy(gameObject);
                    break;
            }
            return true;
        }

        public override void OnTriggerIn(TriggerEvent evt)
        {
            if (evt.colider.gameObject.layer == LayerMask.NameToLayer("Ground") || evt.colider.gameObject.tag == "BackWall")
            {
                IsGrounded = true;
                var sdsa = gameObject.transform.localEulerAngles.z * Mathf.Deg2Rad;
                Vector3 oldspeed = new Vector3(Mathf.Cos(sdsa), Mathf.Sin(sdsa), 0);

                damageController.gameObject.SetActive(false);
                MainRigidbody.freezeRotation = true;
                MainRigidbody.useGravity = false;
                speedset = false;
                State = 2;

                Speed = Vector3.zero;
                MainRigidbody.isKinematic = true;
                gameObject.GetChild("Collider").GetChild("BoxCollider").SetActive(false);
                Vector3 ad = gameObject.transform.localPosition + oldspeed.normalized * 1f;
                gameObject.transform.DoLocalMove(ad, 0.05f);
            }
        }

        public override Direction Direction { set { direction = value; } get { return direction; } }

        public override void OnUpdate() 
        {
            base.OnUpdate();
            
            // 重力 add by TangJian 2017/07/10 21:28:06
            {
                Vector3 gravity = new Vector3(0, GrivityAcceleration, 0);

                if (IsGrounded && Speed.y < 0)
                {
                    Speed = new Vector3(Speed.x, 0f, Speed.z);
                }
                else
                {
                    Speed = Speed - gravity * Time.deltaTime;
                }
            }

            if (speedset)
            {
                transform.position = transform.position + (Speed * Time.deltaTime);
            }

            // 设置击退方向 add by TangJian 2018/12/10 21:43
            MainDamageData.targetMoveBy = Speed.normalized * MainDamageData.targetMoveBy.magnitude;

            RefreshRenderer();
        }

        public void RefreshRenderer()
        {
            if (MainRigidbody.freezeRotation == false)
            {
                Quaternion rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), Speed.normalized);
                transform.rotation = Quaternion.Euler(new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z));
            }
        }
    }
}