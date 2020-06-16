using UnityEngine;
using Spine.Unity;

namespace Tang
{
    public class arrowController : FlySkillController
    {
        //Rigidbody rigidbody;
        SkeletonAnimator skeletonAnimation;
        DamageController damageController;
        public float ang = 30f;

        public float Atk
        {
            set { damageController.damageData.atk = value; }
            get { return damageController.damageData.atk; }
        }
        private Vector3 speed = new Vector3();
        bool IsGrounded = false;
        bool speedset = true;
        public float GrivityAcceleration = 60f;

        public override void FlyTo(Vector3 pos)
        {
            Vector3 newSpeed = Tools.GetFlyToPosSpeed(new Vector2(1, 1), pos - transform.position + new Vector3(0, 1f, 0), -GrivityAcceleration);
            //Speed = Tools.GetFlyToPosSpeed(new Vector2(1, 1), pos - transform.position + new Vector3(0, 0.5f, 0), -GrivityAcceleration);
            float aas= Mathf.Atan2(newSpeed.z, newSpeed.x) * Mathf.Rad2Deg;
            Vector3 force = new Vector3();
            switch (Direction)
            {
                case Direction.Left:
                    if(aas < 180f - ang && aas > 0)
                    {
                        force = new Vector3(Mathf.Cos((180f - ang) * Mathf.Deg2Rad), 0, Mathf.Sin((180f - ang) * Mathf.Deg2Rad));
                    }
                    else if(aas > ang - 180f && aas < 0)
                    {
                        force = new Vector3(Mathf.Cos((180f + ang) * Mathf.Deg2Rad), 0, Mathf.Sin((180f + ang) * Mathf.Deg2Rad));
                    }
                    else
                    {
                        
                    }
                    break;
                case Direction.Right:
                    if (aas < -ang)
                    {
                        force = new Vector3(Mathf.Cos((-ang) * Mathf.Deg2Rad), 0, Mathf.Sin((-ang) * Mathf.Deg2Rad));
                    }
                    else if (aas > ang)
                    {
                        force = new Vector3(Mathf.Cos((ang) * Mathf.Deg2Rad), 0, Mathf.Sin((ang) * Mathf.Deg2Rad));
                    }
                    else
                    {

                    }
                    break;
            }
            if (force.x == 0 && force.z == 0)
            {
                Vector3 linshi = new Vector3(newSpeed.x, 0, newSpeed.z);
                if(linshi.magnitude> SkillData.speed.x)
                {
                    Speed = (linshi.normalized * SkillData.speed.x)+new Vector3(0,newSpeed.y,0);
                }
                else
                {
                    speed = newSpeed;
                }
                
            }
            else
            {
                Speed = (force.normalized * SkillData.speed.x) + new Vector3(0, SkillData.speed.y, 0);
            }
            //if(aas>)
            RefreshRenderer();
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
            Destroy(gameObject, 10);
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
            if (evt.colider.gameObject.layer == LayerMask.NameToLayer("Ground") || 
                evt.colider.gameObject.layer == LayerMask.NameToLayer("Wall") ||
                evt.colider.gameObject.tag == "BackWall")
            {
                IsGrounded = true;
                var sdsa = gameObject.transform.localEulerAngles.z * Mathf.Deg2Rad;
                Vector3 oldspeed = new Vector3(Mathf.Cos(sdsa), Mathf.Sin(sdsa), 0);

                damageController.gameObject.SetActive(false);
                MainRigidbody.freezeRotation = true;
                MainRigidbody.useGravity = false;
                speedset = false;

                Speed = Vector3.zero;
                MainRigidbody.isKinematic = true;
                gameObject.GetChild("Collider").GetChild("BoxCollider").SetActive(false);
                Vector3 ad = gameObject.transform.localPosition + oldspeed.normalized * 1f;
                gameObject.transform.DoLocalMove(ad, 0.05f);
            }
        }
        
        
        

        public override Direction Direction { set { direction = value; } get { return direction; } }

        private void LateUpdate()
        {
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