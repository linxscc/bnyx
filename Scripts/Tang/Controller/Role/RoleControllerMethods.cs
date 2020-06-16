using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using ZS;


namespace Tang
{
    using FrameEvent;
    
    public partial class RoleController
    {
        public override void OnEnable()
        {
            base.OnEnable();
            
            RoleManager.Instance.AddRoleController(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            RoleManager.Instance.RemoveRoleController(this);
        }

        // 初始化属性监控池 add by TangJian 2017/11/20 18:09:57
        public virtual void InitValueMonitorPool()
        {
            // 监控装备刷新 add by TangJian 2017 / 11 / 20 18:11:11
            _valueMonitorPool.AddMonitor<string>((Func<string>)(() =>
            {
                return (string)(this.RoleData.EquipData.getMainHand<WeaponData>() != null ? this.RoleData.EquipData.getMainHand<WeaponData>().id : "");
            }), (string from, string to) =>
            {
                RefreshRoleAnim(null);
                RefreshAnimator();
            }, true);

            // 监控装备刷新 add by TangJian 2017 / 11 / 20 18:11:11
            _valueMonitorPool.AddMonitor<string>((Func<string>)(() =>
            {
                return (string)(this.RoleData.EquipData.getOffHand<WeaponData>() != null ? this.RoleData.EquipData.getOffHand<WeaponData>().id : "");
            }), (string from, string to) =>
            {
                RefreshRoleAnim(null);
                RefreshAnimator();
            }, true);
            
            // 监控装备刷新 add by TangJian 2017 / 11 / 20 18:11:11
            _valueMonitorPool.AddMonitor<string>((Func<string>)(() =>
            {
                return (string)(this.RoleData.EquipData.GetArmorData() != null ? this.RoleData.EquipData.GetArmorData().id : "");
            }), (string from, string to) =>
            {
                RefreshRoleAnim(null);
                // updateAnimatorController();
            }, true);
            
            // 监控血量最大值变化, 并且做出调整 add by TangJian 2019/3/13 10:09
            _valueMonitorPool.AddMonitor<float>(() => { return RoleData.FinalHpMax;}, (float from, float to) =>
            {
                RoleData.Hp = (RoleData.Hp + (to - from)).Range(1, int.MaxValue);
            });
            
            _valueMonitorPool.Update();
        }
        public void UpdateValueMonitorPool()
        {
            _valueMonitorPool.Update();
        }

        // 装备主手武器 add by TangJian 2017/11/20 22:47:46
        public virtual void EquipMainHandWeapon(WeaponData weaponData)
        {
            RoleData.EquipData.MainHand = weaponData;
        }

        public virtual void UnEquipMainHandWeapon()
        {
            RoleData.EquipData.MainHand = null;
        }

        // 装备副手武器 add by TangJian 2017/11/20 22:49:51
        public virtual void EquipOffHandWeapon(WeaponData weaponData)
        {
            RoleData.EquipData.OffHand = weaponData;
        }

        public virtual void UnEquipOffHandWeapon()
        {
            RoleData.EquipData.OffHand = null;
        }
        public virtual void UnEquipArmor()
        {
            RoleData.EquipData.armorData = null;
        }
        public virtual void UnEquip(EquipType equipType)
        {
            switch (equipType)
            {
                case EquipType.Necklace:
                    RoleData.EquipData.necklaceData = null;
                    break;
                case EquipType.Glove:
                    RoleData.EquipData.gloveData = null;
                    break;
                case EquipType.Trousers:
                    RoleData.EquipData.TrousersData = null;
                    break;
                case EquipType.Shoe:
                    RoleData.EquipData.shoeData = null;
                    break;
                case EquipType.Helmet:
                    RoleData.EquipData.HelmetData = null;
                    break;
            }
        }
        public virtual void UnEquipRing(int index)
        {
            switch (index)
            {
                case 0:
                    RoleData.EquipData.ring1Data = null;
                    break;
                case 1:
                    RoleData.EquipData.ring2Data = null;
                    break;
            }
        }
        public virtual void UnEquipnecklace()
        {
            RoleData.EquipData.necklaceData = null;
        }
        public virtual void UnEquipSoul()
        {
            RoleData.EquipData.SoulData = null;
        }

        // 装备饰品 add by TangJian 2017/11/20 21:51:54
        public virtual bool EquipDecoration(DecorationData decorationData)
        {
            if (RoleData.EquipData.DecorationDataList.Count < 4)
            {
                RoleData.EquipData.DecorationDataList.Add(decorationData);
                // 转杯添加buff add by TangJian 2017/11/20 21:55:28            
                BuffController.AddEquipBuff(decorationData.buffId, decorationData.buffId);
                return true;
            }
            return false;
        }

        public virtual void UnEquipDecoration(DecorationData decorationData)
        {
            RoleData.EquipData.DecorationDataList.Remove(decorationData);

            // 转杯添加buff add by TangJian 2017/11/20 21:55:28            
            BuffController.RemoveBuff(decorationData.id + "-" + decorationData.buffId);
        }

        // Use this for initialization
        public override void Start()
        {
            base.Start();

            // 初始化角色 add by TangJian 2019/4/17 17:19
            Init();
            
            // 初始化增益 add by TangJian 2018/12/8 1:30
            InitBuff();

            // 角色控制器 add by TangJian 2017/07/13 23:28:59
            _characterController = GetComponent<CharacterController>();

            // 初始化动画状态机 add by TangJian 2018/12/8 14:39
            InitAnimator();





            // 设置角色方向 add by TangJian 2018/12/8 14:39
            SetDirection(this.direction);


            InitValueMonitorPool();

            initDamageTarget();
            // 初始化角色状态机 add by TangJian 2018/05/08 17:32:12
            InitFSM();
            
            
            
            // 初始化攻击受击代理 add by TangJian 2019/5/10 16:23
            _hitAndHurtDelegate = new HitAndHurtController(this);
            
            // 初始化踩地面特效控制器
            _walkOnGroundEffectDelegate = new WalkOnGroundEffectController();

            InitHurtMode();
        }

        void Init()
        {
            MoveState = MoveState.Idle;
        }

        void initDamageTarget()
        {
            GameObject game=gameObject.GetChild("DamageTarget");
            if (game!=null)
            {
                TriggerController triggerController = game.GetComponent<TriggerController>();
                if (triggerController != null)
                {
                    string targetid = Tools.getOnlyId().ToString()+"-"+gameObject.name;
                    triggerController.id = targetid;
                }
            }
            
        }
        
        void InitFSM()
        {
            _fsm = new FSM();

            _fsm.SetCurrStateName("alive");

            // 活着的状态 add by TangJian 2018/05/08 17:42:04
            _fsm.AddState("alive", () =>
            {

            }, () => { }, () => { });


            // 死亡状态 add by TangJian 2018/05/08 17:41:54
            _fsm.AddState("dead", () =>
            {
                if (OnDying != null)
                    OnDying(this);
                
                SceneEventManager.Instance.ObjDying(name, CurrSceneController.name);
                
                // 爆装备 add by TangJian 2018/01/04 17:00:57
//                this.DropItemList();
                this.DropItemCustom();
                    
                DelayFunc("RoleDead", () =>
                {
                    Tools.Destroy(gameObject);
                }, 5.0f);
                
            }, () => { }, () => { });

            // 生命值小于等于0进入 dead 状态 add by TangJian 2018/05/08 17:37:43
            _fsm.AddEvent("aliveToDead", "alive", "dead", () => { return IsDead; });
            
            // 跌落到 -10m处, 角色死亡 add by TangJian 2019/3/14 11:24
            _fsm.AddEvent("aliveToDead", "alive", "dead", () =>
            {
                switch (FallInOption)
                {
                    case FallInOption.None:
                        break;
                    case FallInOption.Dying:
                        return MTransform.position.y < FallInHeight;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return false;
            }, () =>
            {
                    _roleData.Hp = 0;
            });
        }

        void InitHurtMode()
        {
            HurtModeController = GetComponent<HurtModeController>() ?? throw new Exception("InitHurtMode error");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            // 销毁的时候移除自身 add by TangJian 2019/4/2 15:31
            CurrSceneController.RemoveRoleController(this);
        }

        public override void OnFixedUpdate()
        {
            // 刷新角色是否站在地面 add by TangJian 2018/05/03 21:14:27
//            _characterController.Move(new Vector3(0, -0.01f, 0));
//            isGrounded = _characterController.isGrounded;
//            UpdateGround();
            //isGrounded = _characterController.isGrounded;

            // 刷新增益 add by TangJian 2017/10/09 20:17:27
            BuffController.Update(Time.deltaTime);
        }

        public override void OnUpdate()
        {
            
            
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
//                this.DropItemList();
                this.DropItemCustom();
            }

            UpdateAnimatorState();


            // refreshRoleAnim();

            // 刷新角色数据 add by TangJian 2017/08/23 15:04:50
            UpdateAttr();

            UpdateValueMonitorPool();

            if (_fsm != null)
            {
                _fsm.Update();
            }

            // 角色升级检测 add by TangJian 2019/3/6 22:09
            UpgradeTest();
            
            
            // 用于修复转正的时候, clipInfo切换不及时的问题 add by TangJian 2019/5/24 16:05
            RoleAnimator.Update(0);
            RoleAnimator.Update(0);
            RoleAnimator.Update(Time.deltaTime);
            skeletonAnimator.Update();
        }
        
        private RaycastHit[] _raycastHits = new RaycastHit[32];
        public void UpdateGround()
        {
            if (isGrounded == false)
            {
                Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
                for (int i = 0; i < Physics.RaycastNonAlloc(ray, _raycastHits,  0.3f); i++)
                {
                    var hit = _raycastHits[i];
                    if (!hit.collider.isTrigger && !Physics.GetIgnoreLayerCollision(gameObject.layer, hit.collider.gameObject.layer))
                    {
                        if (hit.collider.gameObject != gameObject)
                        {
                            var isOnPerson = hit.collider.gameObject.layer == gameObject.layer;
                            if (isOnPerson == false)
                            {
                                isGrounded = true;
                            }
                        }
                    }
                }
            }
        }

//        private void LateUpdate()
//        {
//            
//        }

        void OnGUI()
        {
        }

        float regainDelay = 1;
        private bool IsRoll = false;
        
        void UpdateAttr()
        {
            if (RoleData != null)
            {

                RoleData.locationData.position = transform.localPosition;

                if (RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("attack") 
                    || RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("jumpingAttack")
                    || RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("jump")
                    || RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("roll")
//                    || RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("run")
                    || RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("rush")
                    || RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("dun-idle")
                    || RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("dun-move")
                    || RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("dun-hurt"))
                {
                    regainDelay = 2;
                }
                else
                {
                    regainDelay -= Time.deltaTime;
                    if (regainDelay <= 0)
                    {
                        Vigor = (Vigor + 1f * Time.deltaTime).Range(0, VigorMax);
                    }
                }
                
                if (RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("roll")
                    && Vigor > 20 && !IsRoll)
                {
                    Vigor -= 20;
                    IsRoll = true;
                }

                if (!RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("roll"))
                {
                    IsRoll = false;
                }
                
                if (RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("rush")
                    && Vigor > 0)
                {
//                        Vigor -=(VigorMax / 3f) *Time.deltaTime;
                        Vigor = Vigor - 4f * Time.deltaTime;
                }
                if (animatorParams.ContainsKey("role_data_tili"))
                {
                    _animator.SetFloat("role_data_tili", Vigor);
                }
                
                if (RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("dun-idle")
                    || RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("dun-move")
                    || RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("dun-hurt"))
                {
                    if(Vigor > 0)
                    {
//                        Vigor -=(VigorMax / 3f) *Time.deltaTime;
                        Vigor = Vigor - 5f * Time.deltaTime;
                    }
                }
            }
        }

        public void UpgradeTest()
        {
            if (RoleData.exp >= RoleUpgradeDataAsset.Instance.GetExp(RoleData.level + 1))
            {
                int level = RoleUpgradeDataAsset.Instance.GetLevel(RoleData.exp);
                RoleData.level = level;

                RoleData.Hp = RoleData.FinalHpMax;
                Debug.Log(name + "升级到: " + RoleData.level);
                
                AnimManager.Instance.PlayAnimEffect("RoleUpgrade", transform.position, 0, false, Vector3.right, transform, 0);
            }
        }

        public virtual void SkillBegin(SkillData skillData)
        {
            currSkillData = skillData;
        }
        
        public virtual void SkillEnd(SkillData skillData)
        {
            currSkillData = skillData;
        }

        public virtual void Interact() { }

        public virtual void Use() { }


        public virtual void SetDirection(Direction direction)
        {
            var animObject = _roleAnimController.gameObject;
            this.direction = direction;
            if (animObject != null)
            {
                if (this.direction == Direction.Right)
                {
                    animObject.transform.localScale = new Vector3(Mathf.Abs(animObject.transform.localScale.x), animObject.transform.localScale.y, animObject.transform.localScale.z);
                }
                else
                {
                    animObject.transform.localScale = new Vector3(-Mathf.Abs(animObject.transform.localScale.x), animObject.transform.localScale.y, animObject.transform.localScale.z);
                }
            }
        }

        public virtual void SetDirectionInt(int directionInt)
        {
            if (directionInt > 0)
            {
                SetDirection(Direction.Right);
            }
            else if (directionInt < 0)
            {
                SetDirection(Direction.Left);
            }
        }

        public int GetDirectionInt()
        {
            return direction == Direction.Right ? 1 : -1;
        }

        // 属性操作 加属性 add by TangJian 2017/10/09 21:38:21
        public void AddAttr(AttrType attrType, float attrValue)
        {
            Debug.Log("角色" + name + "属性" + attrType + "增加" + attrValue);

            switch (attrType)
            {
                case AttrType.Atk:
                    RoleData.Atk += attrValue;
                    break;
                case AttrType.Def:
                    RoleData.Def += attrValue;
                    break;
                case AttrType.AtkSpeedScale:
                    RoleData.AtkSpeedScale += attrValue;
                    break;

                // 暴击率 add by TangJian 2017/12/21 15:11:06
                case AttrType.CriticalRate:
                    RoleData.CriticalRate += attrValue;
                    break;
                case AttrType.CriticalDamage:
                    RoleData.CriticalDamage += attrValue;
                    break;

                case AttrType.MoveSpeedScale:
                    RoleData.MoveSpeedScale += attrValue;
                    break;
                case AttrType.HpMax:
                {
                    RoleData.HpMax += attrValue;
                }
                    break;
                case AttrType.Hp:
                {
                    RoleData.Hp += attrValue;
                    if (RoleData.Hp <= 0)
                    {
                        _animator.SetBool("isDead", true);
                    }
                }
                    break;
                case AttrType.MpMax:
                    RoleData.MpMax += attrValue;
                    break;
                case AttrType.Mp:
                    RoleData.Mp += attrValue;
                    break;
                case AttrType.Tili:
                    RoleData.Tili += attrValue;
                    break;
                case AttrType.TiliMax:
                    RoleData.TiliMax += attrValue;
                    break;
                case AttrType.Vigor:
                    _roleData.FinalVigor += attrValue;
                    break;
            }
        }
        


        public virtual void Cast(SkillData skillData)
        {
        }

        public void OnPlayEffect(PlayEffectData addEffectData)
        {
            Debug.Log("public void OnPlayEffect(PlayEffectData addEffectData)");
            AnimManager.Instance.PlayEffect(gameObject, addEffectData.name, direction, addEffectData.pos);
        }
        protected float currVariableSpeed = 1;
        protected float Difference = 0;
        Dictionary<string, GameObject> currAnimDamageGameObjectDic = new Dictionary<string, GameObject>();
        // Dictionary<string,float> currVariableRoleMoveSpeedDic = new Dictionary<string, float>();

        List<DamageController> currAnimDamageControllerList = new List<DamageController>();

        public void AddCurrAnimDamageGameObject(DamageController damageController)
        {
            currAnimDamageControllerList.Add(damageController);
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
            foreach (var item in currAnimDamageControllerList)
            {
                DamageManager.Instance.Remove(item);
            }

            currAnimDamageControllerList.Clear();
        }
        Tweener VariableTweener;
        // bool Tweeneronoff=false;
        // float Starttime=0;
        float oldVariableSpeed = 0;
        // float duration=0;
        public void AddCurrVariableRoleMoveSpeed(float speed, float time)
        {
            // Starttime=Time.time;
            Debug.Log("起始值" + currVariableSpeed);
            if (VariableTweener != null)
            {
                VariableTweener.Kill();
            }
            if (time != 0)
            {
                VariableTweener = currVariableSpeed.DoFloat(speed - currVariableSpeed, time).OnComplete(() =>
                {
                    currVariableSpeed = speed;
                });
            }
            else
            {
                currVariableSpeed = speed;
            }

            oldVariableSpeed = currVariableSpeed;
        }
        public void RemoveCurrVariableRoleMoveSpeed(float time)
        {
            oldVariableSpeed = currVariableSpeed;
            if (VariableTweener != null)
            {
                VariableTweener.Kill();
            }
            if (time != 0)
            {
                VariableTweener = currVariableSpeed.DoFloat(currVariableSpeed - 1, time).OnComplete(() =>
                {
                    currVariableSpeed = 1;
                });
            }
            else
            {
                currVariableSpeed = 1;
            }
            // currVariableSpeed=1;

        }

        public void ClearCurrVariableRoleMoveSpeed()
        {
            // float sasd = Time.time - Starttime;
            Debug.Log("退出时值" + currVariableSpeed);
            if (VariableTweener != null)
            {
                VariableTweener.Kill();
            }
            // MoveSpeedDofloat.Kill();
            currVariableSpeed = 1;
            // currVariableRoleMoveSpeedDic.Clear();
        }

        // 施法 add by TangJian 2017/08/09 19:00:22
        public virtual void OnEvtSkill()
        {
            // 攻击帧事件 add by TangJian 2017/07/13 23:23:07            
            var clipInfoArray = _animator.GetCurrentAnimatorClipInfo(0);
            var clipInfo = clipInfoArray[0];
            var animName = clipInfo.clip.name;

            switch (animName)
            {
                case "staff_atk1":
                {
                    // 发射火球术
                    var fireBall = GameObjectManager.Instance.Spawn("FireBall");

                    fireBall.transform.parent = transform.parent;
                    fireBall.transform.localPosition = transform.localPosition + new Vector3(0, 1, 0);

                    FireBallController fireBallController = fireBall.GetComponent<FireBallController>();
                    fireBallController.speed = new Vector3(GetDirectionInt() * 10, 0, 0);

                    fireBallController.DamageController.damageData.owner = gameObject;
                    fireBallController.DamageController.damageData.itriggerDelegate = this;
                }
                    break;
            }
        }

        public float AnimMoveSpeedScale = 1f;
        //public float oldRepellingResistance = 0f;
        void RepellingResistanceCalculation(float RepellingResistance)
        {
            if (AnimMoveSpeedScale > (1 - RepellingResistance))
            {
                AnimMoveSpeedScale = 1 - RepellingResistance;
            }
        }

        // 被击中弱点 add by TangJian 2018/12/8 1:54
        public virtual void WeaknessOnHurt(DamageData damageData)
        {
            WeaknessData weakness = RoleData.WeaknessDataList.Find((WeaknessData weaknessData) =>
            {
                if (weaknessData.WeaknessName == damageData.WeaknessName)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
            if (weakness != null)
            {
                if (weakness.WeaknessHP > 0)
                {
                    weakness.WeaknessHP = weakness.WeaknessHP - damageData.atk;
                    if (weakness.WeaknessHP <= 0)
                    {
                        if (animatorParams.ContainsKey(weakness.WeaknessName))
                        {
                            _animator.SetBool(weakness.WeaknessName, true);
                        }
                    }
                }

            }
        }

        #region //判断对应武器死亡动画  2019.3.27

        private void WeaponByDieAni(WeaponType weaponTypeType,float hurtNum)
        {    
            if (hurtNum >= RoleData.FinalHpMax)
            {
                _animator.SetInteger("deadType", 2);
                _animator.SetBool("isDead", true);                                                          
            }
            else
            {
                {
                    switch (weaponTypeType)
                    {
                        case WeaponType.Lswd:
                            _animator.SetInteger("deadType", 1);
                            _animator.SetBool("isDead", true);
                            break;
                        default:
                            _animator.SetBool("isDead", true);
                            break;
                        
                    }
                }
            }
            
            
        }

        #endregion
        
        public bool HurtByPart(DamageData damageData, HurtPart hurtPart)
        {
            Direction oldDirection = GetDirection();


            if (hurtPart == null) return false;
//                return Hurt(damageData);
            
            // 扣血 add by TangJian 2017/07/14 18:02:17
            if (IsDefending
                && damageData.ignoreShield == false // 不无视防御 add by TangJian 2019/4/3 17:16
            
            )
            {
                //方向判断
                AnimationSwitch(damageData, false, true);
                
                //体力判断
                {
                    // 伤害计算和扣血 add by TangJian 2019/4/2 16:44
                    float HurtNum = this.DoLoseHp(damageData.atk,hurtPart.HurtRatio,false);
                    //百分比减伤
                    LessenHurt_Ratio(HurtNum);
                    
                    // 自身动画状态设置 add by TangJian 2019/4/2 16:44
                    {
                        if (animatorParams.ContainsKey("NoDefence"))
                        {
                            _animator.SetBool("NoDefence", true);
                        }
                    
                        _animator.SetInteger("Defence_hurt_type", 3);
                        AnimManager.Instance.WeaponCremaShake(FrameEventInfo.CameraShakeType.mediumshape);
                        if (RoleData.Hp <= 0)
                        {
                            WeaponByDieAni(damageData.WeapondeadType,HurtNum);
                    
                        }
                        else
                        {
                            _animator.SetTrigger("hurt");
                        }    
                    }
                    
                    // 额外效果 add by TangJian 2019/4/2 16:44
                    {
                        float angle = damageData.SpecialEffectRotation;
                        Bounds bounds = new Bounds(transform.position + RoleData.DefendBoundcenter, RoleData.DefendBoundsize);
                        Vector3 localPos = transform.InverseTransformPoint(damageData.collideBounds.center);
                        Vector3 posse = transform.position + localPos;
                        Vector3 overpos = bounds.ClosestPoint(posse);
                        Vector3 moveOrientation = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
                        moveOrientation.y = Mathf.Abs(moveOrientation.y);
                    }
                }
            }
            else
            {         
                // 伤害计算和扣血 add by TangJian 2019/4/2 16:44
                float HurtNum = this.DoLoseHp(damageData.atk,hurtPart.HurtRatio);
                
                #region //2019.3.29// 动画状态机设置
                {
                    // 设置无视防御状态 add by TangJian 2019/4/3 17:50
                    if (damageData.ignoreShield)
                    {
                        RoleAnimator.SetTrigger("ignore_shield");
                    }
                   
                    if (RoleData.Hp <= 0)
                    {
                        WeaponByDieAni(damageData.WeapondeadType,HurtNum);
                    }
                                                        
                    //地面血迹 2019.3.29
                    AnimManager.Instance.PlayAnimEffect("BloodSubface", transform.position, 0, damageData.direction.x < 0, Vector3.right, transform);

                    //判断是否需要转身 2019.4.4
                    bool IsneedTurnback = RoleData.NeedTurnBack;
                    if (IsneedTurnback)
                    {
                        if (damageData.hitType != (int)HitType.Fly)
                            IsneedTurnback = false;
                    }
                    AnimationSwitch(damageData, IsneedTurnback);                              
                    
                }
                #endregion 
                
               
                // 播放掉血动画
                if (HurtNum > 0)
                {
                    this.PlayLoseHpAnim(HurtNum, damageData.isCritical);    
                }

                // 使用代理实现受伤 add by TangJian 2019/5/10 17:56
                _hitAndHurtDelegate.Hurt(damageData, hurtPart);
            }
            
            // 霸体状态处理 add by TangJian 2018/12/8 14:56
            if (RoleData.IsSuperArmor)
            {
                SetDirection(oldDirection); // 霸体被攻击不转面 add by TangJian 2019/3/25 10:54
                _animator.SetBool("hurt", false);
            }
            else
            {
                OnHurtEvent?.Invoke(damageData);
            }
            
            //击退 add by TangJian 2018/12/8 14:55
            if (damageData.targetMoveBy.magnitude > 0.01f)
            {
                Speed = (damageData.targetMoveBy * (1f - (IsDefending ? RoleData.DefenseRepellingResistance : RoleData.RepellingResistance))).MoveByToSpeed();
            }

            // 触发受伤事件 add by TangJian 2018/05/14 20:09:38
            BuffController.TriggerBuffEvent(BuffEventType.OnHurt);
            return true;
        }
        
        private void LessenHurt_Ratio(float HurtNum)
        {
            float vigorConsume = 5;   
            //减伤百分比
            float lessenhurt = RoleData.FinalLessenHurt - RoleData.GetDefenceAttr(AttrType.LessenHurt);
            
            //计算百分比减伤数值
            float lessenhurtNum = lessenhurt * HurtNum;
            float Hpmax = RoleData.FinalHpMax;
            float vigor = RoleData.FinalVigor;
            //需要消耗精力值
            float vigor_lessenhurt = (lessenhurtNum / Hpmax) * vigorConsume;
            
            if (vigor> vigor_lessenhurt)
            {
                RoleData.FinalVigor -= vigor_lessenhurt;
            }
            else
            {
                HPReduce(HurtNum);
            }
            
        }

        private void HPReduce(float ReduceHp)
        {
            if (RoleData.Hp > 0)
            {
                RoleData.Hp -= ReduceHp;
            }
            
            this.PlayLoseHpAnim(ReduceHp, false);
        }
        // 受伤 add by TangJian 2018/12/8 1:54
        public virtual bool Hurt(DamageData damageData)
        {
            Direction oldDirection = GetDirection();
            
            // 扣血 add by TangJian 2017/07/14 18:02:17
            if (IsDefending
            
                && damageData.ignoreShield == false // 不无视防御 add by TangJian 2019/4/3 17:16
            
            )
            {
                //方向判断
                AnimationSwitch(damageData, false, true);
                
                //体力判断
                {
                    // 伤害计算和扣血 add by TangJian 2019/4/2 16:44
                    float HurtNum = this.DoLoseHp(damageData.atk);
                    
                    // 自身动画状态设置 add by TangJian 2019/4/2 16:44
                    {
                        if (animatorParams.ContainsKey("NoDefence"))
                        {
                            _animator.SetBool("NoDefence", true);
                        }
                    
                        _animator.SetInteger("Defence_hurt_type", 3);
                        AnimManager.Instance.WeaponCremaShake(FrameEventInfo.CameraShakeType.mediumshape);
                        if (RoleData.Hp <= 0)
                        {
                            WeaponByDieAni(damageData.WeapondeadType,HurtNum);
                    
                        }
                        else
                        {
                            _animator.SetTrigger("hurt");
                        }    
                    }
                    
                    // 额外效果 add by TangJian 2019/4/2 16:44
                    {
                        float angle = damageData.SpecialEffectRotation;
                        Bounds bounds = new Bounds(transform.position + RoleData.DefendBoundcenter, RoleData.DefendBoundsize);
                        Vector3 localPos = transform.InverseTransformPoint(damageData.collideBounds.center);
                        Vector3 posse = transform.position + localPos;
                        Vector3 overpos = bounds.ClosestPoint(posse);
                        Vector3 moveOrientation = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
                        moveOrientation.y = Mathf.Abs(moveOrientation.y);
                    }
                }
            }
            else
            {         
                // 伤害计算和扣血 add by TangJian 2019/4/2 16:44
                float HurtNum = this.DoLoseHp(damageData.atk);
                
                #region //2019.3.29// 动画状态机设置
                {
                    // 设置无视防御状态 add by TangJian 2019/4/3 17:50
                    if (damageData.ignoreShield)
                    {
                        RoleAnimator.SetTrigger("ignore_shield");
                    }
                   
                    if (RoleData.Hp <= 0)
                    {
                        WeaponByDieAni(damageData.WeapondeadType,HurtNum);
                    }
                                                        
                    //地面血迹 2019.3.29
                    AnimManager.Instance.PlayAnimEffect("BloodSubface", transform.position, 0, damageData.direction.x < 0, Vector3.right, transform);

                    //判断是否需要转身 2019.4.4
                    bool IsneedTurnback = RoleData.NeedTurnBack;
                    if (IsneedTurnback)
                    {
                        if (damageData.hitType != (int)HitType.Fly)
                            IsneedTurnback = false;
                    }
                    AnimationSwitch(damageData, IsneedTurnback);                              
                    
                }
                #endregion 
                
                // 额外动画
                {
                    // 播放掉血动画
                    if(HurtNum > 0)
                        this.PlayLoseHpAnim(HurtNum, damageData.isCritical);
                }
                
                // 使用代理实现受伤 add by TangJian 2019/5/10 17:56
                _hitAndHurtDelegate.Hurt(damageData);
            }
            
            // 霸体状态处理 add by TangJian 2018/12/8 14:56
            if (RoleData.IsSuperArmor)
            {
                SetDirection(oldDirection); // 霸体被攻击不转面 add by TangJian 2019/3/25 10:54
                _animator.SetBool("hurt", false);
            }
            else
            {
                if(OnHurtEvent != null)
                    OnHurtEvent(damageData);
            }
            
            //击退 add by TangJian 2018/12/8 14:55
            if (damageData.targetMoveBy.magnitude > 0.01f)
            {
                Speed = (damageData.targetMoveBy * (1f - (IsDefending ? RoleData.DefenseRepellingResistance : RoleData.RepellingResistance))).MoveByToSpeed();
            }

            // 触发受伤事件 add by TangJian 2018/05/14 20:09:38
            BuffController.TriggerBuffEvent(BuffEventType.OnHurt);
            return true;
        }

        public virtual bool QteHurt(Vector3 direction, Vector3 collidePoint, float atk)
        {
            // 扣血 add by TangJian 2017/07/14 18:02:17
            float damage = this.DoLoseHp(atk, 1,true);
            this.PlayLoseHpAnim(damage, false);

//            #region //死亡动画，小怪根据武器对应死亡动画 2019.3.28
//
////            if (RoleData.Hp <= 0)
////            {
////                WeaponDieAni(damageData);
////            }
//
//            #endregion
                  
            #region //地面血迹 2019.3.25
            {
                AnimManager.Instance.PlayAnimEffect("BloodSubface", transform.position, 0, direction.x < 0, Vector3.right, transform);
            }
            #endregion
                
            // 击中效果 add by TangJian 2018/12/8 14:59
            {
                float angle = 0;
                Vector3 moveOrientation = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
                moveOrientation.y = Mathf.Abs(moveOrientation.y);
                AnimManager.Instance.PlayAnimEffect("RoleHurtEffect", transform.position + transform.InverseTransformPoint(collidePoint), 0, direction.x < 0, moveOrientation, transform);
            }

            return true;
        }
        
        float AngleCorrection(float force, float Rotation)
        {
            float zhachu = 0f;
            if (force > 0)
            {
                zhachu = Rotation;
            }
            else
            {
                if (Rotation > 0 && Rotation <= 90)
                {
                    zhachu = Rotation + (90 - Rotation);
                }
                else if (Rotation > 90 && Rotation <= 180)
                {
                    zhachu = 180 - Rotation;
                }
                else if (Rotation > 180 && Rotation <= 270)
                {
                    zhachu = 360 - (Rotation - 180);
                }
                else if (Rotation > 270 && Rotation <= 360)
                {
                    zhachu = 180 + (360 + Rotation);
                }

            }
            return zhachu;
        }

        public virtual float HpCalculation(DamageData damageData, bool reduce = true)
        {
            float damage = this.DoLoseHp(damageData.atk, 1,reduce);
            this.PlayLoseHpAnim(damage, damageData.isCritical);
            return damage;
        }
        
        public virtual void AnimationSwitch(DamageData damageData, bool needTurnBack, bool NoSetHurt = false)
        {
            // 设置力度
            _animator.SetFloat("hurt_force_value", damageData.forceValue);
            
            bool mode = false;
            if (RoleData.DamageTargetNameList != null)
            {
                if (RoleData.DamageTargetNameList.Count != 0)
                {
                    if (RoleData.DamageTargetNameList.Contains(damageData.hitTarget.name))
                    {
                        mode = true;
                    }
                }
            }
            
            if (mode)
            {
                if (animatorParams.ContainsKey("hurt_type"))
                {
                    int sd =RoleData.DamageTargetNameList.IndexOf(damageData.hitTarget.name);
                    _animator.SetInteger("hurt_type", sd);
                    _animator.SetFloat("hurt_type", sd);
                }
                if (RoleData.Hp <= 0)
                {
                    _animator.SetBool("isDead", true);
                }
                else
                {
                    if (NoSetHurt) { }
                    else
                    {
                        switch (damageData.forceType)
                        {
                            case DamageForceType.light:
                                if (RoleData.Atklightforcetype == false)
                                {
                                    _animator.SetBool("hurt", true);
                                    _animator.SetTrigger("hurt");
                                }
                                break;
                            case DamageForceType.moderate:
                                if (RoleData.Atkmoderateforcetype == false)
                                {
                                    _animator.SetBool("hurt", true);
                                    _animator.SetTrigger("hurt");
                                }
                                break;
                            case DamageForceType.heavy:
                                if (RoleData.Atkheavyforcetype == false)
                                {
                                    _animator.SetBool("hurt", true);
                                    _animator.SetTrigger("hurt");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                if (GetDirectionInt() > 0)
                {
                    if (damageData.direction.x > 0)
                    {
                        _animator.SetInteger("hurt_direction", -1);
                        _animator.SetFloat("hurt_direction", -1);
                        if (needTurnBack)
                        {
                            _animator.SetInteger("hurt_direction", 1);
                            _animator.SetFloat("hurt_direction", 1);
                            SetDirection(GetDirection().Reverse());
                        }
                    }
                    else
                    {
                        _animator.SetInteger("hurt_direction", 1);
                        _animator.SetFloat("hurt_direction", 1);
                    }
                }
                else
                {
                    if (damageData.direction.x < 0)
                    {
                        _animator.SetInteger("hurt_direction", -1);
                        _animator.SetFloat("hurt_direction", -1);

                        if (needTurnBack)
                        {
                            _animator.SetInteger("hurt_direction", 1);
                            _animator.SetFloat("hurt_direction", 1);
                            SetDirection(GetDirection().Reverse());
                        }
                    }
                    else
                    {
                        _animator.SetInteger("hurt_direction", 1);
                        _animator.SetFloat("hurt_direction", 1);
                    }
                }

                // 攻击部位体现 add by TangJian 2017/07/10 20:59:29
                _animator.SetInteger("hurt_type", damageData.hitType);
                _animator.SetFloat("hurt_type", damageData.hitType);

                //　设置攻击状态 add by TangJian 2017/07/14 18:06:02
                if (RoleData.Hp <= 0)
                {
                    _animator.SetBool("isDead", true);
                }
                else
                {
                    if (NoSetHurt) { }
                    else
                    {
                        switch (damageData.forceType)
                        {
                            case DamageForceType.light:
                                if (RoleData.Atklightforcetype == false)
                                {
                                    _animator.SetBool("hurt", true);
                                    _animator.SetTrigger("hurt");
                                }
                                break;
                            case DamageForceType.moderate:
                                if (RoleData.Atkmoderateforcetype == false)
                                {
                                    _animator.SetBool("hurt", true);
                                    _animator.SetTrigger("hurt");
                                }
                                break;
                            case DamageForceType.heavy:
                                if (RoleData.Atkheavyforcetype == false)
                                {
                                    _animator.SetBool("hurt", true);
                                    _animator.SetTrigger("hurt");
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    //animator.SetBool("hurt", true);
                }
            }
        }

        public virtual bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
//                case EventType.DamageHurt:
//                    return OnHurt(evt.Data as DamageData);
//                // break;
//                case EventType.DamageHit:
//                    OnHit(evt.Data as DamageData);
//                    break;
                case EventType.ItemPickUp:
                    ItemData itemData = evt.Data as ItemData;
                    if (itemData.itemType == ItemType.Soul)
                    {
                        RoleData.Soul += itemData.count;
                        RoleData.exp += itemData.count;
                    }
                    else if (itemData.itemType == ItemType.Gold)
                    {
                        RoleData.Money += itemData.count;
                    }
                    break;
            }
            return true;
        }

        // 连击摄像机放大 add by tianjinpeng 2018/07/11 13:23:58
        protected void comboCameraScale()
        {
            if (combo < 6 && combo > 0)
            {
                AnimManager.Instance.CameraScaleUp(0.2f);
            }
            else if (combo == 0)
            {
                AnimManager.Instance.CameraScaleDelut();
            }
            else
            {

            }
        }

        private GameObject _damageTargetGameObject;
        public void SetDamageTargetEnable(bool b)
        {
            _damageTargetGameObject = _damageTargetGameObject != null ? _damageTargetGameObject : gameObject.GetChild("DamageTarget");
            _damageTargetGameObject.SetActive(b);
        }

        private GameObject _shadowGameObject;
        public void SetShadowEnable(bool b)
        {
            _shadowGameObject = _shadowGameObject != null ? _shadowGameObject : gameObject.GetChild("Shadow");
            _shadowGameObject.SetActive(b);
        }

        public void SetColliderEnable(bool b)
        {
            _canCollider = b;
            if (_canCollider == false)
            {
                gameObject.layer = LayerMask.NameToLayer("NoCollider");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Role");
            }
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public void Closejock()
        {
            joystick = Vector2.zero;
        }
        
        // 使用局部坐标设置指向位置 add by TangJian 2019/4/22 11:38
        public void SetLocalPointPosition(Vector3 localPos)
        {
            PointPosition = transform.TransformPoint(localPos);
        }
        
        // 使用世界坐标设置指向位置 add by TangJian 2019/4/22 11:40
        public void SetWorldPointPosition(Vector3 worldPos)
        {
            PointPosition = worldPos;
        }
        
        public HitEffectType GetHitEffectType()
        {
            return _roleData.HitEffectType;
        }

        public MatType GetMatType()
        {
            return _roleData.MatType;
        }
        
        public bool TryGetHitPos(out Vector3 hitPos)
        {
            return skeletonAnimator.GetBonePos("hurt_effect_pos", out hitPos);
        }
    }
}