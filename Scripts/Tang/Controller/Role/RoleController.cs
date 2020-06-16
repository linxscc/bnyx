using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using System;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine.Serialization;
using ZS;


namespace Tang
{
    using FrameEvent;
    
    public enum RoleCollisionState
    {
        Default = 1,
        WithoutRole = 2,
        Climb = 3,
    }
    
    public partial class RoleController : SceneObjectController,ITriggerDelegate, IAnimEventDelegate, IHitAndMat, IObjectController, IRoleAction
    {
        public override string PathInScene
        {
            get { return "Roles"; }
        }

        public SceneController SceneController;

        // 角色控制器 add by TangJian 2017/07/13 23:33:16
        [SerializeField] private CharacterController _characterController;
        public CharacterController CharacterController { get { return _characterController; } }

        // 角色动画控制器 add by TangJian 2017/08/08 20:50:33
        private AnimController _roleAnimController;
        public AnimController RoleAnimController { get { return _roleAnimController; } }

        // animator 动画状态机 add by TangJian 2017/07/13 23:34:13
        protected Animator _animator;
        public Animator RoleAnimator { get { return _animator; } }

        // 能否碰撞 add by TangJian 2018/12/8 1:01
        private bool _canCollider = true;

        public Dictionary<string, bool> animatorParams = new Dictionary<string, bool>();

        // animatorStateInfo动画状态机当前状态信息 add by TangJian 2017/07/13 23:38:22
        private AnimatorStateInfo _currAnimatorStateInfo;
        private AnimatorStateInfo _nextAnimatorStateInfo;

        // 骨骼动画状态机 add by TangJian 2017/07/13 23:39:13         
        protected SkeletonAnimator skeletonAnimator;
        public SkeletonAnimator SkeletonAnimator { get { return skeletonAnimator; } set { skeletonAnimator = value; } }

        // 角色AI add by TangJian 2018/12/8 0:59
         [FormerlySerializedAs("_roleAiController")] public RoleBehaviorTree _roleBehaviorTree;
        public RoleBehaviorTree roleBehaviorTree { get { return _roleBehaviorTree = _roleBehaviorTree != null ? _roleBehaviorTree : gameObject.GetComponent<RoleBehaviorTree>(); } }

        // 行为树 add by TangJian 2018/12/8 0:59
        public BehaviorDesigner.Runtime.BehaviorTree BeHaviorTree { get { return roleBehaviorTree != null ? roleBehaviorTree.BehaviorTree : null; } }

        // 受伤事件 add by TangJian 2018/12/6 22:15
        public event Action<DamageData> OnHurtEvent;

        // 击中事件 add by TangJian 2018/12/6 22:15
        public event Action<DamageData> OnHitEvent;

        // 攻击事件 add by TangJian 2018/12/6 22:15
        public delegate void OnAtkDelegate(FrameEventInfo.RoleAtkFrameEventData roleAtkFrameEventData);
        public event OnAtkDelegate OnAtkEvent;

        // 是否拥有AI add by TangJian 2018/12/8 0:58
        public bool _withAI = false;
        public bool WithAI { set { _withAI = value; } get { return _withAI; } }

        private Renderer _animRenderer;

        // 角色状态机 add by TangJian 2018/05/08 17:31:00
        private FSM _fsm;

        // 增益控制器 add by TangJian 2017/10/09 19:57:13
        public BuffController _buffController = new BuffController();
        public BuffController BuffController { get { return _buffController; } }

        public RoleData _roleData = new RoleData(); // 角色数据 add by TangJian 2017/07/13 23:31:15
        public RoleData RoleData { get { return _roleData; } set { _roleData = value; } }

        // 默认动画速度 add by TangJian 2017/07/13 23:33:34
        private float _defaultAnimSpeed = 1.0f;

        public Animator MainAnimator
        {
            get => _animator;
        }

        public SkeletonRenderer MainSkeletonRenderer
        {
            get => SkeletonAnimator;
        }
        public float DefaultAnimSpeed { get { return _defaultAnimSpeed; } set { _defaultAnimSpeed = value; } }

        public float FrontZ => -0.1f;

        public float BackZ => 0.1f;

        // 跳跃速度 add by TangJian 2017/07/13 23:26:56        
        [SerializeField] public float JumpSpeed { get { return RoleData.JumpSpeed; } }

        // 当前受伤动画速度 add by TangJian 2018/12/8 0:50
        [SerializeField] protected float _currHurtAnimSpeed = 1;
        public float CurrHurtAnimSpeedScale { get { return _currHurtAnimSpeed; } set { _currHurtAnimSpeed = value; } }

        public static float GroundFrictionAcceleration = 60f;
        public static float GrivityAcceleration = 60f;

        public string DamageOnlyID;

        private RoleCollisionState collisionState = RoleCollisionState.Default;
        public RoleCollisionState CollisionState { set { collisionState = value; } get { return collisionState; } }

        // 是否浮空 add by TangJian 2018/12/8 0:49
        bool _isHovering = false;
        public bool IsHovering { set { _isHovering = value; } get { return _isHovering; } }

        // 是否无敌 add by TangJian 2018/12/8 0:48
        [SerializeField] private bool _isInvincible = false;
        public bool IsInvincible { set { _isInvincible = value; } get { return _isInvincible; } }

        // 摇杆 add by TangJian 2017/07/04 21:18:05
        protected Vector2 joystick;
        public Vector2 Joystick { get { return joystick; } }

        // 当前速度 add by TangJian 2017/07/10 21:26:14
        protected Vector3 speed = new Vector3(0, 0, 0);
        public Vector3 Speed { set { speed = value; } get { return speed; } }

        public int currjumpTimes = 0;
        public int jumpTimes { get { return RoleData.jumpTimes; } }

        // 连击 add by tianjinpeng 2018/07/11 13:23:32
        int combo = 0;
        public int ComBo { get { return combo; } set { combo = value; } }
        public Vector3 FirstClimbPos;
        protected Direction direction = Direction.Right; // 

        // 游戏状态控制 add by TangJian 2017/07/19 14:29:52

        private bool isDead = false;
        public bool IsDead { get { return RoleData.FinalHp <= 0; } }

        public GameObject GetGameObject() { return gameObject; }

        // 技能相关 add by TangJian 2017/08/28 21:21:14
        public SkillData currSkillData;

        public RoleAttackType currRoleAttackType = RoleAttackType.None;

        protected ValueMonitorPool _valueMonitorPool = new ValueMonitorPool();

        // 是否防御 add by TangJian 2018/12/8 1:06
        private bool _isDefending = false;
        public bool IsDefending { get { return _isDefending; } set { _isDefending = value; } }


        private bool canRebound = true;
        public bool CanRebound
        {
            get => canRebound;
            set => canRebound = value;
        }

        public float RepellingResistance {
            get => _roleData.RepellingResistance;
            set => _roleData.RepellingResistance = value;
        }

        public float DefenseRepellingResistance
        {
            get => _roleData.DefenseRepellingResistance;
            set => _roleData.DefenseRepellingResistance = value;
        }

        public EffectShowMode EffectShowMode => EffectShowMode.FrontOrBack;
        public float Damping => _roleData.Damping;

        // 队伍Id add by TangJian 2019/5/10 15:22
        public string TeamId => _roleData.TeamId;
        
        // 得到当前攻击速度 add by TangJian 2018/01/15 11:44:04
        public float AtkSpeed => RoleData.FinalAtkSpeed;
        
        // 当前瞄准方向 add by TangJian 2019/4/22 10:56
        [SerializeField] private Vector3 PointPosition;
        
        
        // 当前是否移动 add by TangJian 2019/4/16 21:40
        public bool IsMoving = false;
        
        // 是否走路中 add by TangJian 2019/4/16 23:14
        public bool IsWalking = false;
        
        // 是否跑步中 add by TangJian 2019/4/16 23:14
        public bool IsRunning = false;
        
        // 是否冲刺中 add by TangJian 2019/4/16 23:13
        public bool isRushing = false;
        
        // 运动状态 add by TangJian 2019/4/16 21:25
        public MoveState MoveState = MoveState.Idle;
        
        // 走路速度 add by TangJian 2018/12/8 1:10
        public float WalkSpeed { get { return RoleData.FinalWalkSpeed; } }

        // 跑步速度 add by TangJian 2018/12/8 1:10
        public float RunSpeed { get { return RoleData.FinalRunSpeed; } }
        
        // 冲刺速度 add by TangJian 2019/4/16 18:38
        public float RushSpeed { get { return RoleData.FinalRushSpeed; } }
        
        // 交互行为
        private System.Action<GameObject> onInteractAction;
        private System.Action pickupConsumable;

        public Action PickupConsumable { get { return pickupConsumable = pickupConsumable != null ? pickupConsumable : () => { }; } set { pickupConsumable = value; } }
        public Action<GameObject> OnInteractAction { get { return onInteractAction = onInteractAction != null ? onInteractAction : (GameObject go) => { }; } set { onInteractAction = value; } }


        public event Action<RoleController> OnDying;
        
        // qte add by TangJian 2019/4/4 11:55
        public bool IsQte = false;
//        public RoleController QteTarget;
        public IQte MyQte;
        public IQte HisQte;

        // 攻击受击代理 add by TangJian 2019/5/10 16:22
        private IHitAndHurtDelegate _hitAndHurtDelegate;

        [SerializeField] private HurtModeController HurtModeController;

        public FallInOption FallInOption => roleData.FallInOption;
        public float FallInHeight => roleData.FallInHeight;
        
        // 属性
        
        public float Hp
        {
            get => _roleData.FinalHp;
            set => _roleData.Hp = value;
        }

        public float HpMax => _roleData.HpMax;

        public float Mp
        {
            get => _roleData.FinalMp;
            set => _roleData.Mp = value;
        }

        public float MpMax => _roleData.MpMax;
        
        // 精力
        public float Vigor
        {
            get => _roleData.FinalVigor;
            set => _roleData.FinalVigor = value;
        }
        
        // 精力最大值
        public float VigorMax
        {
            get => _roleData.FinalVigorMax;
        }

        public float FinalHp => _roleData.FinalHp;
        public float FinalHpmax => _roleData.FinalHpMax;
        public int Exp => _roleData.exp;
        public int Level => _roleData.level;
        public float FinalAtk => _roleData.FinalAtk;
        public float FinalDef => _roleData.FinalDef;
        public float FinalMovingSpeed => _roleData.FinalMoveSpeedScale;
        public float FinalCritical => _roleData.FinalCriticalRate;
        public float FinalCriticalDamage => _roleData.FinalCriticalDamage;

        public string ID => _roleData.Id;
        public RoleData roleData => _roleData;
        
        public float Money
        {
            get => _roleData.Money;
        }
        public float Soul => _roleData.Soul;
        public string SoulIcon => _roleData.EquipData.SoulData.icon;

        public List<ConsumableData> ConsumableItemList => _roleData.ConsumableItemList;
        public int CurrConsumableItemIndex => _roleData.CurrConsumableItemIndex;
        
        // Tweener
        private Tweener tweenerDoMoveBy;
        
        // 是否有转身动画
        public bool WithTrunBackAnim = true;

        public IWalkOnGroundEffectController _walkOnGroundEffectDelegate;
    }

    public enum FallInOption
    {
        None = 0,
        Dying = 1
    }
}