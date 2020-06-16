using Newtonsoft.Json.Linq;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;




namespace Tang
{
    using FrameEvent;

    public abstract class PlacementController : SceneObjectController, ITriggerDelegate, IAnimEventDelegate, IData, IHitAndMat, IObjectController
    {
        public ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
        [SerializeField] tian.PlacementData placementData = null;

        public tian.PlacementData Placementdata { set { placementData = value; } get { return placementData; } }
        [SerializeField] private Animator mainAnimator; // 主动画状态机 add by TangJian 2018/04/13 15:59:32

        public bool IsDefending => false;
        public bool CanRebound { get; }
        public float RepellingResistance => 1;
        public float DefenseRepellingResistance => 1;
        public int GetDirectionInt()
        {
            throw new System.NotImplementedException();
        }

        public Vector3 Speed { get; set; }
        public Animator MainAnimator { set { mainAnimator = value; } get { return mainAnimator; } }
        public SkeletonRenderer MainSkeletonRenderer => _skeletonAnimator;
        public float DefaultAnimSpeed { get; }
        public float FrontZ
        {
            get
            {
                return (MainCollider.bounds.min.z - MainCollider.bounds.center.z) - 0.1f;
            }
        }

        public float BackZ
        {
            get
            {
                return (MainCollider.bounds.max.z - MainCollider.bounds.center.z) + 0.1f;
            }
        }

        public bool IsGrounded()
        {
            return true;
        }

        [SerializeField] private SkeletonAnimator _skeletonAnimator;

        public SkeletonAnimator MainSkeletonAnimator
        {
            get { return _skeletonAnimator; }
        }


        GameObject Image;
        DamageController damageController; //
        Rigidbody mainRigidbody = null;
        
        public Rigidbody MainRigidbody
        {
            get
            {
                if (mainRigidbody == null)
                {
                    mainRigidbody = GetComponent<Rigidbody>();
                }
                return mainRigidbody;
            }
        }
        
        // 主要的碰撞体 add by TangJian 2019/5/11 14:22
        private Collider mainCollider;

        public Collider MainCollider
        {
            get => mainCollider;
            set => mainCollider = value;
        }

        public GameObject DamageObject
        {
            get
            {
                // var Trigger = transform.GetChild("Trigger");
                var damageTransform = gameObject.GetChild("Damage");
                if (damageTransform != null)
                {
                    return damageTransform.gameObject;
                }
                else if (damageTransform == null)
                {
                    var Trigger = gameObject.GetChild("Trigger");
                    var Transform = Trigger.GetChild("Damage");
                    return Transform;
                }

                Debug.LogError("DamageObject == null");
                return null;
            }
        }

        public DamageController DamageController
        {
            get
            {
                if (damageController == null)
                {
                    damageController = DamageObject.GetComponent<DamageController>();
                }
                return damageController;
            }
        }

        private IHitAndHurtDelegate _hitAndHurtDelegate;
        public IHitAndHurtDelegate HitAndHurtDelegate => _hitAndHurtDelegate;

        public override void Start()
        {
            base.Start();

            InitCollider();
            InitAnimator();
            InitvalueMonitorPool();
            
            _hitAndHurtDelegate = new HitAndHurtController(this); 
            gameObject.RecursiveComponent<TriggerController>((controller, i) =>
                {
                    controller.id = Tools.getOnlyId().ToString();
                },2,99);
        }
        
        // 初始化Collider
        public virtual void InitCollider()
        {
            mainCollider = gameObject.GetChild("Collider").GetComponentInChildren<Collider>();
        }

        public virtual void InitAnimator()
        {
            _skeletonAnimator = GetComponentInChildren<SkeletonAnimator>();
            mainAnimator = GetComponentInChildren<Animator>();
        }
        public virtual void InitvalueMonitorPool()
        {
            valueMonitorPool.Clear();
        }

        public virtual GameObject GetGameObject() { return gameObject; }


        public virtual bool OnEvent(Event evt)
        {
            return true;
        }

        public virtual int State
        {
            set
            {
                if (mainAnimator != null)
                {
                    Debug.Log("设置 state = " + value);
                    mainAnimator.SetInteger("state", value);
                }

            }
            get
            {
                return mainAnimator.GetInteger("state");
            }
        }

        public virtual void OnTriggerIn(TriggerEvent evt)
        {
        }

        public virtual void OnTriggerKeep(TriggerEvent evt)
        {

        }

        public virtual void OnTriggerOut(TriggerEvent evt)
        {

        }

        public virtual async void Drop()
        {
            foreach (var item in placementData.dropItemList)
            {
                if (item.chance > Random.Range(0, 99))
                {
                    GameObject go = await AssetManager.InstantiateDropItem(item.itemId);
                    DropItemController dropItemController = go.GetComponent<DropItemController>();
                    SceneManager.Instance.DropItemEnterSceneWithWorldPosition(dropItemController, SceneId,
                        gameObject.transform.position + new Vector3(0, 1, 0));
                    
                    Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        //rigidbody.velocity = new Vector3(Random.Range(0f, 0.5f), Random.Range(0f, 3f), Random.Range(0f, 0.5f)).MoveByToSpeed();
                        Vector3 targetPos = new Vector3(Random.Range(-2f, 2f), -1f, Random.Range(-2f, 2f));
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(go.transform.TransformPoint(targetPos), out hit, 1.0f, NavMesh.AllAreas))
                        {
                            targetPos = go.transform.InverseTransformPoint(hit.position);
                        }
                        rigidbody.velocity = Tools.GetFlyToPosSpeed(new Vector2(1f, Random.Range(0.5f, 2f)), targetPos, -60f);
                    }
                }
            }
        }

        public virtual void OnAtk(FrameEventInfo.RoleAtkFrameEventData roleAtkFrameEventData)
        {
            string damageControllerId = roleAtkFrameEventData.id + GetInstanceID();
            string damageId = roleAtkFrameEventData.useCustomFrameEventBatch
                ? damageControllerId + MainSkeletonAnimator.CustomFrameEventBatchId
                : damageControllerId + MainSkeletonAnimator.FrameEventBatchId;


            FrameEventMethods.OnSkillAtk(roleAtkFrameEventData, damageControllerId, damageId,
                Placementdata.teamId, MainSkeletonAnimator, this, transform.rotation, DamageEffectType.Strike,
                AtkPropertyType.magicalDamage, 1);
        }

        public virtual void OnSkill(FrameEventInfo.RoleSkillFrameEventData roleSkillFrameEventData)
        {

        }

        public virtual void OnPlayAnim(FrameEventInfo.PlayAnimFrameEventData PlayAnimFrameEventData)
        {

        }

        public void OnCustom(FrameEventInfo.CustomFrameEventData customFrameEventData)
        {
            switch (customFrameEventData.eventType)
            {
                case FrameEventInfo.CustomFrameEventData.EventType.AddCustomFrameEventBatchId:
                    MainSkeletonAnimator.CustomFrameEventBatchId++;
                    break;
                case FrameEventInfo.CustomFrameEventData.EventType.DestroySelf:
                    Destroy(gameObject);
                    break;
            }
        }

        public override void OnUpdate()
        {
            valueMonitorPool.Update();
        }

        public void OnVariableRoleMoveSpeed(FrameEventInfo.VariableRoleMoveSpeedData variableRoleMoveSpeedData)
        {
        }

        public void OnPlayAnimList(FrameEventInfo.FrameEventAnimList frameEventAnimList)
        {
            throw new System.NotImplementedException();
        }

        public void OnSetSpeedFrameEvent(FrameEventInfo.SetSpeedFrameEvent setSpeedFrameEvent)
        {
            throw new System.NotImplementedException();
        }

        public void OnPlayAnimEffect(FrameEventInfo.PlayAnimEffectFrameEventData playAnimFrameEventData)
        {
            Vector3 pos = new Vector3(playAnimFrameEventData.pos.x, playAnimFrameEventData.pos.y, playAnimFrameEventData.pos.z);
            AnimManager.Instance.PlayAnimEffect(playAnimFrameEventData.id, transform.position + pos, 0, false, Vector3.right, transform);
        }

        public void OnSetAnimSpeedFrameEvent(FrameEventInfo.SetAnimSpeedFrameEvent setAnimSpeedFrameEvent)
        {
            throw new System.NotImplementedException();
        }

        public void OnMoveToTargetFrameEvent(FrameEventInfo.MoveToTargetFrameEvent moveToTargetFrameEvent)
        {
            throw new System.NotImplementedException();
        }

        public void OnCameraShakeFrameEvent(FrameEventInfo.CameraShakeFrameEvent cameraShakeFrameEvent)
        {
            throw new System.NotImplementedException();
        }

        public void OnSuperArmor(FrameEventInfo.SuperArmorFrameEvent superArmorFrameEvent)
        {
            throw new System.NotImplementedException();
        }

        public void OnSkillList(FrameEventInfo.RoleSkillListFrameEvent roleSkillListFrameEvent)
        {
            throw new System.NotImplementedException();
        }
        public void OnRoleAiFlyTo(FrameEventInfo.RoleAiFlyToFrameEventData roleAiFlyToFrameEventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnRoleQte(FrameEventInfo.RoleQTE roleQte)
        {
            throw new System.NotImplementedException();
        }

        public void OnAnimProperty(FrameEventInfo.AnimProperty animProperty)
        {
            throw new System.NotImplementedException();
        }

        public void OnPlayAudio(FrameEventInfo.PlayAudio playAudio)
        {
            throw new System.NotImplementedException();
        }

        public void OnJObject(JObject jObject)
        {
            throw new System.NotImplementedException();
        }

        public void OnTread(FrameEventInfo.OnTread onTread)
        {
            Debug.Log("Tread!54");
        }

        public virtual object Data { get; set; }
        public HitEffectType GetHitEffectType()
        {
            return placementData.hitEffectType;
        }

        public MatType GetMatType()
        {
            var a = placementData.matType;
            return placementData.matType;
        }

        public virtual EffectShowMode EffectShowMode => EffectShowMode.FrontOrBack;

        public bool TryGetHitPos(out Vector3 hitPos)
        {
            hitPos = Vector3.zero;
            return false;
        }

        public float Damping => 0;

        public virtual bool OnHit(DamageData damageData)
        {
            _hitAndHurtDelegate.Hit(damageData);
            return true;
        }

        public virtual bool OnHurt(DamageData damageData)
        {
            _hitAndHurtDelegate.Hurt(damageData);
            return true;
        }

        public string TeamId => placementData.teamId;
    }
}