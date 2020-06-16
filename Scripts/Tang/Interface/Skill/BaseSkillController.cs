using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace Tang
{
    using FrameEvent;

    public abstract partial class BaseSkillController : MyMonoBehaviour, ISkillController, ITriggerDelegate,
        IFinishAble, IAnimEventDelegate, IAnimDelegate, IHitAndMat, IObjectController
    {
        // 渲染节点
        public GameObject RendererObject
        {
            get => gameObject.GetChild("Renderer");
        }

        // 碰撞节点
        public GameObject ColliderObject
        {
            get => gameObject.GetChild("Collider");
        }

        // 触发器根节点
        public GameObject TriggerObject
        {
            get => gameObject.GetChild("Trigger");
        }

        [SerializeField] private Rigidbody mainRigidbody; // 主刚体 add by TangJian 2018/04/13 16:02:02

        public virtual Rigidbody MainRigidbody
        {
            set { mainRigidbody = value; }
            get { return mainRigidbody; }
        }

        // 主动画状态机 add by TangJian 2018/04/13 15:59:32
        [SerializeField] private Animator mainAnimator;

        public virtual Animator MainAnimator
        {
            set { mainAnimator = value; }
            get { return mainAnimator; }
        }

        [SerializeField] private DamageController mainDamageController;

        public virtual DamageController MainDamageController
        {
            set { mainDamageController = value; }
            get { return mainDamageController; }
        }

        private Collider mainDamageCollider;
        public Collider MainDamageCollider
        {
            get
            {
                if (mainDamageCollider == null)
                    mainDamageCollider = MainDamageController.GetComponent<Collider>();
                return mainDamageCollider;
            }
        }

        public virtual DamageData MainDamageData
        {
            set
            {
                mainDamageController.damageData = value;
            }
            get
            {
                return mainDamageController.damageData;
            }
        } // 设置伤害 add by TangJian 2018/04/13 15:54:52
        
        // 骨骼动画状态机 add by TangJian 2019/4/20 11:49
        private SkeletonAnimator skeletonAnimator;
        public virtual SkeletonAnimator MainSkeletonAnimator { set { skeletonAnimator = value; } get { return skeletonAnimator; } }
        
        // 精灵 add by TangJian 2019/4/20 14:36
        public Sprite Sprite { get; set; }

        // 精灵渲染器 add by TangJian 2019/4/20 14:38
        public SpriteRenderer SpriteRenderer { get; set; }

        // 碰撞体 add by TangJian 2019/4/20 11:52
        public Collider MainCollider { get; set; }

        // 存在时间 add by TangJian 2019/4/20 11:49
        private float survivalTime;
        public virtual float SurvivalTime { set { survivalTime = value; } get { return survivalTime; } }

        // 技能数据 add by TangJian 2019/4/20 11:49
        public SkillData SkillData { private set; get; }

        // 拥有者 add by TangJian 2019/4/20 11:50
        public GameObject owner;
        public GameObject Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        
        // 朝向 add by TangJian 2018/05/09 14:47:30
        [SerializeField] public Direction direction = Direction.Right;
        public virtual Direction Direction
        {
            set
            {
                direction = value; 
            }
            get { return direction; }
        }

        public virtual Vector3 Speed { get; set; }

        // 朝向整型 add by TangJian 2019/4/20 11:50
        public virtual int DirectionInt { set { Direction = value > 0 ? Direction.Right : Direction.Left; } get { return Direction == Direction.Right ? 1 : -1; } }
        public string DamageOnlyID;
       
        // 队伍编号 add by TangJian 2018/05/09 14:47:59

        public string TeamId { set; get; } = "1";

        // 伤害管理列表 add by TangJian 2019/4/20 11:51
        Dictionary<string, GameObject> currAnimDamageGameObjectDic = new Dictionary<string, GameObject>();
        
        
        // 技能初始化接口 add by TangJian 2019/4/20 11:51
        public virtual async void InitSkill(SkillData skillData)
        {
            SkillData = skillData;

            // tag
            gameObject.tag = SkillData.tagName;

            
            // 攻击
            {
                if (skillData.onoffDamage)
                {
                    if (MainDamageController.GetComponent<BoxCollider>() is BoxCollider boxCollider)
                    {
                        boxCollider.size = skillData.DamageColliderSize;
                        boxCollider.center = skillData.DamageColliderCenter;
                    }
                    
                    MainDamageData.teamId = TeamId;
                    MainDamageData.DamageId = Tools.getOnlyId().ToString() + "-" + skillData.id;
                    // 攻击力 add by TangJian 2018/04/13 20:11:39
                    MainDamageData.atk = skillData.atk;

                    // 击退效果 add by TangJian 2018/04/13 20:12:18
                    MainDamageData.targetMoveBy = new Vector3(skillData.angleIntensity.x * DirectionInt, skillData.angleIntensity.y, skillData.angleIntensity.z).normalized * skillData.Intensity;
                    MainDamageData.DamageDirectionType = skillData.DamageDirectionType;
                    MainDamageData.direction = new Vector3(DirectionInt, 0, 0);
                    MainDamageData.forceType = skillData.damageForceType;
                    MainDamageData.damageTimes = -1;
                    MainDamageData.poiseCut = skillData.poiseCut;

                    MainDamageData.owner = gameObject;

                    // 赋值打击效果类型 add by TangJian 2019/6/5 17:24
                    MainDamageData.HitEffectType = skillData.HitEffectType;
                }
                else
                {
                    MainDamageController.enabled = false;
                }
            }
            
            // 刚体初始化 add by TangJian 2019/4/20 11:47
            {
                
                if (MainRigidbody is Rigidbody rigidbody)
                {
                    if (skillData.withRigidbody)
                    {
                        rigidbody.useGravity = skillData.useGravity;
                        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        rigidbody.centerOfMass = skillData.focuspos;
                    }
                    else
                    {
                        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                    }
                }    
            }
                
            // 碰撞体初始化 add by TangJian 2019/4/20 11:47
            {
                if (ColliderObject.GetChild("BoxCollider", true).GetComponent<BoxCollider>() is BoxCollider boxCollider)
                {
                    if (skillData.onoffCollider)
                    {
                        boxCollider.center = skillData.colliderCenter;
                        boxCollider.size = skillData.colliderSize;
                        boxCollider.gameObject.tag = skillData.CollidertagName;
                        boxCollider.gameObject.layer = LayerMask.NameToLayer(skillData.ColliderlayerName);
                    }
                    else
                    {
                        boxCollider.gameObject.SetActive(false);
                    }  
                }
            }

            // 触发器初始化 add by TangJian 2019/4/20 12:13
            {
                var target = TriggerObject.GetChild("Target");
                if (skillData.onoffTirgger)
                {
                    target.tag = skillData.tagName;
                    target.layer = LayerMask.NameToLayer(skillData.layerName);
                    TriggerController triggerController = target.GetComponent<TriggerController>();
                    triggerController.triggerMode = skillData.triggerMode;
                    BoxCollider TirggerCollider = target.GetComponent<BoxCollider>();
                    TirggerCollider.center = skillData.TirggercolliderCenter;
                    TirggerCollider.size = skillData.TirggercolliderSize;
                }
                else
                {
                    target.SetActive(false);
                }
            }
            
            var anim = mainAnimator.gameObject;
            // 渲染初始化 add by TangJian 2019/4/20 11:41
            switch (skillData.rendererType)
            {
                case RendererType.SkeletonAnimator:
                {
                    Animator animator = anim.GetComponent<Animator>();
                    Spine.Unity.SkeletonAnimator skeletonAnimator = anim.GetComponent<Spine.Unity.SkeletonAnimator>();

                    var skeletonAnimatorRenderer = skeletonAnimator.GetComponent<Renderer>();
                    skeletonAnimatorRenderer.enabled = false;
                    
                    DelayFunc(() => { skeletonAnimatorRenderer.enabled = true; }, 0.000001f);

                    animator.runtimeAnimatorController =
                        await AssetManager.LoadAssetAsync<RuntimeAnimatorController>(skillData.AnimControllerPath);

                    skeletonAnimator.skeletonDataAsset =
                        await AssetManager.LoadAssetAsync<Spine.Unity.SkeletonDataAsset>(skillData.SkeletonDataAssetPath);

                    skeletonAnimator.initialSkinName = skillData.SkinName;
                    skeletonAnimator.Initialize(true);
                }
                    break;
                case RendererType.Sprite:
                    Sprite = await AssetManager.LoadAssetAsync<Sprite>(skillData.SpritePath);
                    SpriteRenderer.sprite = Sprite;
                    break;
                case RendererType.Skeleton:
                    GameObject obj = new GameObject("Anim_SkeletonAnimation");
                    obj.transform.parent = anim.transform;
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    
                    Spine.Unity.SkeletonAnimation skeletonAnimation = obj.AddComponent<Spine.Unity.SkeletonAnimation>();
                    skeletonAnimation.skeletonDataAsset = await AssetManager.LoadAssetAsync<Spine.Unity.SkeletonDataAsset>(skillData.SkeletonPath);
                    
                    skeletonAnimation.Initialize(true);
                    Spine.TrackEntry TrackEntry = skeletonAnimation.state.SetAnimation(0, skillData.SkeletonClipName, true);
                    TrackEntry.TimeScale = 1;
                    
                    anim.gameObject.SetActive(true);
                    break;
                case RendererType.Anim:
                    AnimManager.Instance.PlayAnimEffect(skillData.AnimName, transform.position);
                    Destroy(gameObject);
                    break;
                default:
                    Debug.LogError("没有处理渲染");
                    break;
            }
            
            // 影子
            GameObject shadow = RendererObject.GetChild("Shadow");
            if (skillData.shadow)
            {
                ShadowProjector shadowProjector = shadow.GetComponent<ShadowProjector>();
                shadow.transform.localScale = skillData.shadowScale;
                shadowProjector.AutoSOCutOffDistance = skillData.CutOffDistance;
                shadowProjector.AutoSOMaxScaleMultiplier = skillData.MaxScaleMultpler;
            }
            else
            {
                shadow.SetActive(false);
            }
        }
        
        private List<int> ignoreList;

        public virtual void SetIgnoreList(List<int> ignoreList)
        {
            this.ignoreList = ignoreList;
        }

        public virtual List<int> GetIgnoreList()
        {
            return ignoreList;
        }

        public virtual void InitDamage()
        {
            MainDamageController = gameObject.GetChild("Trigger").GetChild("Damage").GetComponent<DamageController>();
            Debug.Assert(MainDamageController != null);

            var damageData = MainDamageController.damageData;
            damageData.itriggerDelegate = this;
            damageData.force = Vector3.zero;
        }
        
        // 初始化碰撞体 add by TangJian 2019/4/20 11:54
        public virtual void InitCollider()
        {
            MainCollider = ColliderObject.GetComponentInChildren<Collider>();
        }
        
        public virtual void InitRigidbody()
        {
            MainRigidbody = gameObject.GetComponent<Rigidbody>();
            Debug.Assert(MainRigidbody != null);
        }
        
        public void Cast()
        {
            throw new System.NotImplementedException();
        }

        // 构造函数之后调用 add by TangJian 2018/05/09 15:13:30
        public virtual void Awake()
        {
            InitAnimator();
            
            InitSprite();

            InitRigidbody();
            
            InitCollider();
            
            InitDamage();
            
            InitHitAndHurt();

            Startup();
        }

        // 初始化动画状态机 add by TangJian 2018/05/09 15:13:42
        public virtual void InitAnimator()
        {
            mainAnimator = gameObject.GetChild("Renderer").GetChild("Anim").GetComponent<Animator>();
            Debug.Assert(mainAnimator != null);
            MainSkeletonAnimator = gameObject.GetChild("Renderer").GetChild("Anim").GetComponent<SkeletonAnimator>();
            Debug.Assert(MainSkeletonAnimator != null);
        }
        
        // 初始化精灵 add by TangJian 2019/4/20 14:39
        public virtual void InitSprite()
        {
            SpriteRenderer = RendererObject.GetChild("SpriteRenderer", true).AddComponentIfNone<SpriteRenderer>();
        }

        // 设置动画状态 add by TangJian 2018/05/09 15:13:57
        public virtual int State
        {
            set
            {
                Debug.Log("设置 state = " + value);
                mainAnimator.SetInteger("state", value);
            }
            get
            {
                return mainAnimator.GetInteger("state");
            }
        }

        // 开始 add by TangJian 2018/05/09 15:14:02
        public virtual void Startup()
        {
            State = 1;
        }
        
        public virtual void WaitForDestroy()
        {
            DelayFunc("Destory", () =>
            {
                Destroy(gameObject);
            }, survivalTime);
        }

        public virtual void OnFinished()
        {
            ClearCurrAnimDamageGameObject();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        public virtual void InitPos(Vector3 V3)
        {
            gameObject.transform.position = V3;
        }

        // 触发器相关方法 add by TangJian 2018/05/09 15:17:11
        public virtual GameObject GetGameObject()
        {
            return gameObject;
        }
        // gameObject.setactive(false)时调用的方法 add by tianjinpeng 2018/08/06 14:23:28
        public virtual void OnDisable()
        {
            Tools.Destroy(this.gameObject);
        }
        public virtual void OnTriggerIn(TriggerEvent evt)
        {
        }

        public virtual void OnTriggerOut(TriggerEvent evt)
        {
        }

        public virtual void OnTriggerKeep(TriggerEvent evt)
        {
        }

        public virtual bool OnEvent(Event evt)
        {
            return true;
        }

        // 动画事件方法 add by TangJian 2018/05/09 15:16:44
        public virtual void OnAtk(FrameEventInfo.RoleAtkFrameEventData roleAtkFrameEventData)
        {
            string damageControllerId = roleAtkFrameEventData.id + GetInstanceID();
            string damageId = roleAtkFrameEventData.useCustomFrameEventBatch
                ? damageControllerId + MainSkeletonAnimator.CustomFrameEventBatchId
                : damageControllerId + MainSkeletonAnimator.FrameEventBatchId;

            FrameEventMethods.OnSkillAtk(roleAtkFrameEventData, damageControllerId, damageId,
                TeamId, MainSkeletonAnimator, this, transform.rotation, DamageEffectType.Strike,
                AtkPropertyType.magicalDamage,1,0,0,0,0,Owner.GetComponent<RoleController>().RoleData.FinalPoiseCut, Owner.GetComponent<RoleController>().RoleData.FinalMass);
        }
        public void AddCurrAnimDamageGameObject(string id, GameObject go)
        {
            currAnimDamageGameObjectDic.Add(id, go);
        }

        public void RemoveCurrAnimDamageGameObject(string id)
        {
            GameObject go;
            if (currAnimDamageGameObjectDic.TryGetValue(id, out go))
            {
                Destroy(go);
                currAnimDamageGameObjectDic.Remove(id);
            }
        }

        public void ClearCurrAnimDamageGameObject()
        {
            foreach (var item in currAnimDamageGameObjectDic)
            {
                Destroy(item.Value);
            }
            currAnimDamageGameObjectDic.Clear();
        }

        public void OnSkill(FrameEventInfo.RoleSkillFrameEventData roleSkillFrameEventData)
        {
        }

        public void OnPlayAnim(FrameEventInfo.PlayAnimFrameEventData PlayAnimFrameEventData)
        {
        }

        public virtual void FlyTo(Vector3 pos)
        {
        }

        public virtual void TowardTo(Vector3 pos)
        {
           
        }

        public void SetToSetupPose()
        {
            MainAnimator.gameObject.GetComponent<SkeletonAnimator>().skeleton.SetToSetupPose();
        }

        public void OnCustom(FrameEventInfo.CustomFrameEventData customFrameEventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnVariableRoleMoveSpeed(FrameEventInfo.VariableRoleMoveSpeedData variableRoleMoveSpeedData)
        {
            throw new System.NotImplementedException();
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
            Vector3 pos = new Vector3(playAnimFrameEventData.pos.x * GetDirectionInt(), playAnimFrameEventData.pos.y, playAnimFrameEventData.pos.z);
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

        public void OnTread(FrameEventInfo.OnTread tread)
        {
            throw new System.NotImplementedException();
        }
    }

    
    
    public abstract partial class BaseSkillController
    {
        private IHitAndHurtDelegate _hitAndHurtDelegate;
        public IHitAndHurtDelegate HitAndHurtDelegate => _hitAndHurtDelegate;

        protected void InitHitAndHurt()
        {
            _hitAndHurtDelegate = new HitAndHurtController(this);
        }

        public HitEffectType GetHitEffectType()
        {
            return SkillData.HitEffectType;
        }

        public MatType GetMatType()
        {
            return MatType.Wood;
        }

        public EffectShowMode EffectShowMode => EffectShowMode.ColliderPoint;

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

        public bool OnHurt(DamageData damageData)
        {
            _hitAndHurtDelegate.Hurt(damageData);
            return true;
        }

        public bool IsDefending => false;
        public bool CanRebound { get; }
        public float RepellingResistance => 0;
        public float DefenseRepellingResistance => 0;
        
        public int GetDirectionInt()
        {
            return direction == Direction.Right ? 1 : -1;
        }

        public SkeletonRenderer MainSkeletonRenderer => skeletonAnimator;
        public float DefaultAnimSpeed => 1;
        public float FrontZ => 0.1f;
        public float BackZ => -0.1f;
        public bool IsGrounded()
        {
            return false;
        }
    }
}