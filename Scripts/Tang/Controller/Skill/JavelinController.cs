using UnityEngine;
using Spine.Unity;

namespace Tang
{
    public class JavelinController : FlySkillController
    {
        //Rigidbody rigidbody;
        SkeletonAnimator skeletonAnimation;
        DamageController damageController;

        public float distance = 15f;
        public float speedy;
        public float ang = 30f;
        JavelinController()
        {
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public override void Awake()
        {
            // 刚体 add by TangJian 2018/01/16 16:15:37
            base.Awake();
            // 动画 add by TangJian 2018/01/15 15:04:59
            skeletonAnimation = GetComponentInChildren<SkeletonAnimator>();

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
            //roleAIController = owner.GetComponent<RoleAIController>();
            //if (roleAIController != null)
            //{
            //    if (roleAIController.TargetController != null)
            //    {
            //        Vector3 force = new Vector3(roleAIController.TargetController.transform.position.x - transform.position.x, 0, roleAIController.TargetController.transform.position.z - transform.position.z);
            //        if (Vector3.Magnitude(roleAIController.TargetController.transform.position - transform.position) <= skillData.speed.x)
            //        {
            //            float dir = Vector3.Magnitude(roleAIController.TargetController.transform.position - transform.position);
            //            float end = roleAIController.TargetController.RunSpeed * (dir / skillData.speed.x);
            //            force = new Vector3(force.x + (roleAIController.TargetController.Speed.normalized.x * dir), force.y, force.z + (roleAIController.TargetController.Speed.normalized.x * dir));
            //        }
            //        float fl = roleAIController.TargetController.transform.position.z - transform.position.z;
            //        float sdsa = Mathf.Atan2(force.z, force.x) * Mathf.Rad2Deg;
            //        switch (Direction)
            //        {
            //            case Direction.Left:
            //                if (sdsa < 180f - ang && sdsa > 0)
            //                {
            //                    force = new Vector3(Mathf.Cos((180f - ang) * Mathf.Deg2Rad), 0, Mathf.Sin((180f - ang) * Mathf.Deg2Rad));
            //                }
            //                else if (sdsa > ang - 180f && sdsa < 0)
            //                {
            //                    force = new Vector3(Mathf.Cos((180f + ang) * Mathf.Deg2Rad), 0, Mathf.Sin((180f + ang) * Mathf.Deg2Rad));
            //                }
            //                else
            //                {

            //                }
            //                break;
            //            case Direction.Right:
            //                if (sdsa < -ang)
            //                {
            //                    force = new Vector3(Mathf.Cos((-ang) * Mathf.Deg2Rad), 0, Mathf.Sin((-ang) * Mathf.Deg2Rad));
            //                }
            //                else if (sdsa > ang)
            //                {
            //                    force = new Vector3(Mathf.Cos((ang) * Mathf.Deg2Rad), 0, Mathf.Sin((ang) * Mathf.Deg2Rad));
            //                }
            //                else
            //                {

            //                }
            //                break;
            //        }
            //        Speed = (force.normalized * skillData.speed.x) + new Vector3(0, skillData.speed.y, 0);
            //    }
            //}
        }

        public override void FlyTo(Vector3 pos)
        {
            Speed = Tools.GetFlyToPosSpeed(new Vector2(1, 1), pos - transform.position, -30f);
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
            if (evt.colider.gameObject.layer == LayerMask.NameToLayer("Ground") || evt.colider.gameObject.tag == "BackWall" || evt.colider.gameObject.layer == LayerMask.NameToLayer("Placement"))
            {
                var sdsa = gameObject.transform.localEulerAngles.z * Mathf.Deg2Rad;
                Vector3 oldspeed = new Vector3(Mathf.Cos(sdsa), Mathf.Sin(sdsa), 0);
                //Debug.Log("hhh");
                MainRigidbody.freezeRotation = true;
                MainRigidbody.useGravity = false;
                damageController.gameObject.SetActive(false);
                damageController.enabled = false;
                //transform.eulerAngles = Vector3.zero;
                Speed = Vector3.zero;
                MainAnimator.speed = 0f;
                MainRigidbody.isKinematic = true;
                gameObject.GetChild("Collider").GetChild("BoxCollider").SetActive(false);
                Vector3 ad = gameObject.transform.localPosition + oldspeed.normalized * 1f;
                gameObject.transform.DoLocalMove(ad, 0.05f);
                //delayFunc("Destory", () =>
                //{
                //    Destroy(gameObject);
                //}, 0.5f);
                //Destroy(gameObject);
            }
        }
        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.tag == "BackWall" || other.gameObject.layer == LayerMask.NameToLayer("Placement"))
            {
                // 冻结z方向旋转 add by TangJian 2018/05/08 20:04:58
                var sdsa = gameObject.transform.localEulerAngles.z * Mathf.Deg2Rad;
                Vector3 oldspeed = new Vector3(Mathf.Cos(sdsa), Mathf.Sin(sdsa), 0);
                MainRigidbody.freezeRotation = true;
                MainRigidbody.useGravity = false;
                damageController.gameObject.SetActive(false);
                damageController.enabled = false;
                MainAnimator.speed = 0f;
                //transform.eulerAngles = Vector3.zero;
                Speed = Vector3.zero;
                MainRigidbody.isKinematic = true;
                gameObject.GetChild("Collider").GetChild("BoxCollider").SetActive(false);
                Vector3 ad = gameObject.transform.localPosition + oldspeed.normalized * 1f;

                gameObject.transform.DoLocalMove(ad, 0.05f);
                //delayFunc("Destory", () =>
                //{
                //    Destroy(gameObject);
                //}, 0.5f);
            }
        }
        public override Direction Direction
        {
            set
            {
                direction = value;
            }
            get { return direction; }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (MainRigidbody.freezeRotation == false)
            {
                if (MainRigidbody.isKinematic == false)
                {
                    if (transform.localScale.x >= 0)
                    {
                        Quaternion rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), Speed.normalized);
                        MainRigidbody.rotation = Quaternion.Euler(new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z));
                    }
                    else
                    {
                        Quaternion rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), Speed.normalized);
                        MainRigidbody.rotation = Quaternion.Euler(new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z));
                    }
                }
                //{
                //    Matrix4x4.Translate(Speed);
                //}
            }
        }
    }
}