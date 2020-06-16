using UnityEngine;

namespace Tang
{
    public class HammerController : FlySkillController
    {
        // public override void Awake()
        // {
        //     base.Awake();
        //     var player1 = GameObject.Find("Player1");
        //     InitPos(new Vector3(player1.transform.position.x, 0, player1.transform.position.z));
        // }
        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
        
        BoxCollider mainCollider;
        
        bool IsGrounded = false;
        bool faii = true;
        public float GrivityAcceleration = 1000f;
        Vector3 speed = new Vector3();
        public override Vector3 Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = value;
            }
        }
        BehaviorDesigner.Runtime.BehaviorTree behaviorTree;
        private void Start()
        {
            valueMonitorPool.Clear();
            base.Awake();
            var player1 = GameObject.Find("Player1");
            mainCollider = gameObject.GetChild("Collider").GetChild("BoxCollider").GetComponent<BoxCollider>();
            RoleBehaviorTree roleBehaviorTree=Owner.GetComponent<RoleBehaviorTree>();
            if (roleBehaviorTree!=null)
            {
                Vector3 initvector3 = roleBehaviorTree.TargetController.transform.position;
                if(roleBehaviorTree.TargetController != null)
                {
                    InitPos(new Vector3(initvector3.x, transform.position.y, initvector3.z));
                }
                else
                {
                    InitPos(new Vector3(transform.position.x+(roleBehaviorTree.SelfController.GetDirectionInt()*10), transform.position.y, transform.position.z));
                }
               
                behaviorTree= Owner.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
                behaviorTree.SetVariableValue("hammerpos", transform.position);
                behaviorTree.SetVariableValue("hammer", gameObject);
            }
            //delayFunc(()=> 
            //{
            //    MainAnimator.speed = 0f;
            //},0.4f);
            valueMonitorPool.AddMonitor<int>(()=> 
            {
                if (roleBehaviorTree != null)
                {
                    if (roleBehaviorTree.TargetController != null)
                    {
                        Vector3 v3 = roleBehaviorTree.TargetController.transform.position;
                        return v3.x - transform.position.x > 0 ? 1 : -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
                
            },(int from, int to)=> 
            {
                if (to != 0)
                {
                    MainDamageData.direction= new Vector3(to, 0, 0);
                    MainDamageData.targetMoveBy = new Vector3(AngleIntensity.x * to, AngleIntensity.y, AngleIntensity.z).normalized * Intensity;
                }
            });
        }
        public override void OnTriggerIn(TriggerEvent evt)
        {
            if (evt.colider.gameObject.layer == LayerMask.NameToLayer("Ground") || evt.colider.gameObject.tag == "BackWall")
            {
                // 冻结z方向旋转 add by TangJian 2018/05/08 20:04:58
                MainRigidbody.freezeRotation = true;
                faii = false;
                MainRigidbody.useGravity = false;
                MainRigidbody.isKinematic = true;
                MainDamageController.gameObject.SetActive(false);
                IsGrounded = false;
                //collider.gameObject.layer = LayerMask.NameToLayer("SceneComponent");
                MainAnimator.SetTrigger("Trigger");
                //transform.eulerAngles = Vector3.zero;
                behaviorTree.SetVariableValue("hammerpos", transform.position);
                Speed = Vector3.zero;
                MainRigidbody.isKinematic = true;
            }
        }
        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.tag == "BackWall")
            {
                // 冻结z方向旋转 add by TangJian 2018/05/08 20:04:58
                MainRigidbody.freezeRotation = true;
                MainRigidbody.useGravity = false;
                MainRigidbody.isKinematic = true;
                faii = false;
                MainDamageController.gameObject.SetActive(false);
                IsGrounded = false;
                //collider.gameObject.layer = LayerMask.NameToLayer("SceneComponent");
                MainAnimator.SetTrigger("Trigger");
                //transform.eulerAngles = Vector3.zero;
                behaviorTree.SetVariableValue("hammerpos", transform.position);
                Speed = Vector3.zero;
                MainRigidbody.isKinematic = true;
            }
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            valueMonitorPool.Update();
            if (faii)
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
                        Speed = Speed - gravity * Time.fixedDeltaTime;
                    }
                }
                transform.localPosition = transform.localPosition + (Speed * Time.fixedDeltaTime);
            }
            
        }
        public override bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.DamageHurt:
                    return OnHurt(evt.Data as DamageData);
                    // break;

            }
            return true;
        }
        bool OnHurt(DamageData damageData)
        {
            float angle = damageData.SpecialEffectRotation;
            Vector3 moveOrientation = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
            AnimManager.Instance.PlayAnimEffect("RoleHurtEffect", transform.position + transform.InverseTransformPoint(damageData.collidePoint), 0, damageData.direction.x < 0, moveOrientation, transform);
            return true;
        }
        public void destroy()
        {
            Destroy(gameObject);
        }

    }
}

