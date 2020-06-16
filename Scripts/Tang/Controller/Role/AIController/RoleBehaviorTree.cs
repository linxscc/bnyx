using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tang
{
    public class RoleBehaviorTree : MonoBehaviour
    {
        private RoleController selfController;
        private RoleNavMeshAgent roleNavMeshAgent;
        private BehaviorDesigner.Runtime.BehaviorTree behaviorTree;
        public BehaviorDesigner.Runtime.BehaviorTree BehaviorTree => behaviorTree;
        
        private RoleController targetController; 
        public  RoleController TargetController { set => targetController = value; get => targetController; }
        public RoleController SelfController { set { selfController = value; } get { return selfController; } }
        
        public List<Transform> teammates = new List<Transform>();
        public List<Transform> enemys = new List<Transform>();
        
        public int agentTypeID = 0;
        public string aiScripName = "DefaultAI";
        
        // 角色行为 add by TangJian 2017/11/06 20:39:15
        public List<RoleAIAction> actionList = new List<RoleAIAction>()
        {
            new RoleAIAction("attack", "action1", 0, 0, new Bounds(new Vector3(0.5f, 0, 0), new Vector3(8, 1, 2)))
        };
        
        private RoleAIAction currAction;
        public RoleAIAction CurrAction
        {
            set => currAction = value;
            get => currAction;
        }
        
        
        public bool NeedTurnBack => selfController.WithTrunBackAnim;

        private void OnEnable()
        {
            Init();
            InitEvent();
        }

        private void Init()
        {
            selfController = GetComponent<RoleController>();
            roleNavMeshAgent = GetComponent<RoleNavMeshAgent>();
            behaviorTree = gameObject.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        }
        
        private void InitEvent()
        {
            selfController.OnHitEvent -= OnHit;
            selfController.OnHitEvent += OnHit;

            selfController.OnHurtEvent -= OnHurt;
            selfController.OnHurtEvent += OnHurt;
        }
        
        private void OnDisable()
        {
            if (selfController != null)
            {
                selfController.OnHitEvent -= OnHit;
                selfController.OnHurtEvent -= OnHurt;

                selfController.OnHitEvent += (DamageData damageData) => { };
            }
        }
        
        private void OnHit(DamageData damageData)
        {
            if (!behaviorTree) return;
            behaviorTree.SetValue("hit", true);

            behaviorTree.SetValue("HitTimes", behaviorTree.GetValue<int>("HitTimes") + 1);
                
            // 击中对象的tag名称记录
            List<string> hitTargetTagList = behaviorTree.GetValue<List<string>>("hitTargetTagList");

            hitTargetTagList?.Add(damageData.hitGameObject.tag);
        }

        private void OnHurt(DamageData damageData)
        {
            if (behaviorTree == null) return;
            behaviorTree.SetValue("hurt", true);

            behaviorTree.SetValue("HurtTimes", behaviorTree.GetValue<int>("HurtTimes") + 1);
                
            // 受击时间点记录
            var HurtTimeList = behaviorTree.GetValue<List<float>>("HurtTimeList");
            if (HurtTimeList != null)
            {
                HurtTimeList.Add(Time.time);
                if (HurtTimeList.Count > 10)
                {
                    HurtTimeList.RemoveAt(0);
                }
            }
                
            // 记录被什么tag击中
            List<string> hurtByTagList = behaviorTree.GetValue<List<string>>("hurtByTagList");
            List<string> hurtPartNameList = behaviorTree.GetValue<List<string>>("hurtPartNameList");

            if (hurtByTagList == null || hurtPartNameList == null) return;
            hurtByTagList.Add(damageData.owner.tag);
            hurtPartNameList.Add(damageData.HurtPart == null ? "" : damageData.HurtPart.Name);
        }
        
        public void Update()
        {
            GetTeam();
            if (behaviorTree != null)
            {
                float time = Time.time;

                behaviorTree.SetValue("Time", time);

                float hurtTime = behaviorTree.GetValue<float>("HurtTime");
                behaviorTree.SetValue("ElapsedTimeFromLastHurt", time - hurtTime);

                
                
                behaviorTree.SetValue("Teammates", teammates);
                behaviorTree.SetValue("Enemys", enemys);
                behaviorTree.SetValue("TargetTransformList", enemys); 
            }
        }
        
        private void GetTeam()
        {
            RoleController[] roleControllers = gameObject.transform.parent.GetComponentsInChildren<RoleController>();
                
            // 获取周围的角色列表 add by TangJian 2019/3/14 23:11
            teammates = new List<Transform>();
            enemys = new List<Transform>();
                
            foreach (var item in roleControllers)
            {
                // 不获取自己 add by TangJian 2019/3/14 23:12
                if (selfController.GetInstanceID() != item.GetInstanceID())
                {
                    if (item._roleData.TeamId == selfController._roleData.TeamId)
                    {
                        teammates.Add(item.transform);
                    }
                    else
                    {
                        enemys.Add(item.transform);
                    }
                }
            }
        }

        public IEnumerator Wait(float waitTime)
        {   
            while (waitTime > 0)
            {
                waitTime -= UnityEngine.Time.deltaTime;
                yield return null;
            }
        }
        
        public IEnumerator MovePath( Vector3 desPosition, float speed, Vector3[] path = null)
        {
            while (!selfController.IsGrounded())
            {
                yield return CoroutineStatus.Running;
            }
            
            if (path == null)
                path = roleNavMeshAgent.CalculPath(desPosition);

            if (path.Length <= 0)
            {
                yield return CoroutineStatus.Failure;
            }

            if (path.Length > 0)
            {
                for (int currIndex = 1; currIndex < path.Length; currIndex++)
                {
                    var position = path[currIndex];
                    IEnumerator moveTo = MoveTo(position, speed);

                    while (true)
                    {
                        if (moveTo.MoveNext())
                        {
                            yield return CoroutineStatus.Running;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        
        
        
        
        public IEnumerator MoveTo( Vector3 pos, float speed)
        {
            float beginTime = Time.time;
            float currTime = Time.time;

            Vector3 beginPos = selfController.transform.position;
            Vector3 endPos = pos;
            Vector3 offset = endPos - beginPos;
            //offset.y = 0f;

            float duration = offset.magnitude / speed;
            float speedPerFrame = speed / 60f;

            while (true)
            {
                if (
                    selfController.CurrAnimStateIsTag("idle")
                    || selfController.CurrAnimStateIsTag("run") 
                    || selfController.CurrAnimStateIsTag("move")
                    || selfController.CurrAnimStateIsTag("nocontrol")
                )
                {
                    var elapsedTime = Time.time - beginTime;
                    var percent = Tools.Range(elapsedTime / duration, 0f, 1f);

                    Vector3 gravity = new Vector3(0, 60f, 0);
                    if (selfController.IsGrounded() && selfController.Speed.y < 0)
                    {
                        //Speed = new Vector3(Speed.x, -2.0f, Speed.z);
                    }
                    else
                    {
                        beginPos = beginPos - gravity * Time.deltaTime;
                    }
                    
                    Vector3 toPos = beginPos + offset * percent;
                    Vector3 moveToPos = toPos;
                    
                    Ray ray = new Ray();
                    if (selfController.IsGrounded() && selfController.Speed.y > 0)
                    {
                        float gap = 0.05f;
                        ray.origin = transform.position + new Vector3(toPos.x * gap, 1, toPos.z * gap);
                        ray.direction = new Vector3(0, -1, 0);

                        if (Physics.Raycast(ray, out var hitInfo, 2, Tools.GetLayerMask(new List<string>() { "Ground" })))
                        {
                            moveToPos = transform.InverseTransformPoint(hitInfo.point);
                            toPos = toPos.magnitude * moveToPos.normalized;
                        }
                    }
                    
                    selfController.MoveTo(toPos);

                    int direction = 0;
                    if (offset.x > 0)
                    {
                        direction = 1;
                    }
                    else if (offset.x < 0)
                    {
                        direction = -1;
                    }
                    
                    if (percent >= 1f)
                    {
                        ForwardToTarget(direction);
                        break;
                    }
                    else
                    {
                        ForwardToTarget(direction);
                        yield return CoroutineStatus.Running;
                    }

                }
                else
                {
                    break;
                }
            }
        }
       
        public void ForwardToTarget(int direction = 0)
        {
            if (TargetController != null)
            {
                var selfPosition = selfController.transform.position;
                var targetPosition = TargetController.transform.position;
                int directionInt = (targetPosition.x - selfPosition.x) > 0 ? 1 : -1;

                if (directionInt * selfController.GetDirectionInt() < 0 )
                {
                    if (NeedTurnBack)
                    {
                        selfController.RoleAnimator.SetBool("is_turnback", true);
                    }
                    else
                    {
                        selfController.SetDirectionInt(directionInt);  
                    }
                }
            }
            else
            {
                if (direction * selfController.GetDirectionInt() < 0 )
                {
                    if (NeedTurnBack)
                    {
                        selfController.RoleAnimator.SetBool("is_turnback", true);
                    }
                    else
                    {
                        selfController.SetDirectionInt(direction);  
                    }
                }
            }

        }
        
        
         // 判断action是否能够攻击到目标 add by TangJian 2017/11/08 12:41:00
        public bool ActionCanAttackTarget(RoleAIAction roleAIAction, int directionInt)
        {
            directionInt = directionInt > 0 ? 1 : (directionInt < 0 ? -1 : 0);

            var selfPosition = selfController.transform.position;
            var targetPosition = targetController.transform.position;

            var actionBounds = new Bounds(roleAIAction.bounds.center, roleAIAction.bounds.size);
            actionBounds.center = new Vector3(actionBounds.center.x * directionInt + selfController.transform.position.x, actionBounds.center.y + selfController.transform.position.y, actionBounds.center.z + selfController.transform.position.z);

            // 绘制action覆盖区域 add by TangJian 2017/11/08 12:43:51
            DebugManager.Instance.AddDrawGizmos(gameObject.GetInstanceID() + "ActionCanAttackTarget", () =>
            {
                Gizmos.DrawCube(actionBounds.center, actionBounds.size);
            });

            bool a = actionBounds.Contains(targetPosition);
            return actionBounds.Contains(targetPosition);
        }

        // 得到action可以攻击到目标的位置 add by TangJian 2017/11/08 12:40:45
        public Vector3 GetAttackTargetPositionWithAction(RoleAIAction roleAIAction, Direction direction)
        {
            var selfPosition = selfController.transform.position;
            var targetPosition = targetController.transform.position;

            Vector3 retPosition = targetPosition;

            switch (direction)
            {
                case Direction.Right:
                    retPosition.x = targetPosition.x + Random.Range(roleAIAction.bounds.min.x, roleAIAction.bounds.max.x) * 1f;
                    break;
                case Direction.Left:
                    retPosition.x = targetPosition.x - Random.Range(roleAIAction.bounds.min.x, roleAIAction.bounds.max.x) * 1f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            // z方向随机偏移 add by TangJian 2018/01/13 13:48:03
            retPosition.z += Random.Range(-roleAIAction.bounds.size.z / 2, roleAIAction.bounds.size.z / 2);

            //retPosition.y = 0;
            return retPosition;
        }

        
        public RoleAIAction NextAction()
        {
            int dsa = Tools.RandomWithWeight<RoleAIAction>(actionList, (RoleAIAction curr, int index) =>
            {
                int weight = curr.weight;
                return weight;
            });
            currAction = actionList[dsa];

            return currAction;
        }

        public RoleAIAction GetAIAction(string name)
        {
            return actionList.Find((RoleAIAction roleAIAction) => roleAIAction.name == name);
        }
        
        
        // 判断是否能使用该action add by TangJian 2017/11/08 12:40:31
        public bool CanDoAction(RoleAIAction roleAIAction)
        {
            if (Time.time - roleAIAction.usetime >= roleAIAction.interval || roleAIAction.usetime == 0)
            {
                if (selfController == null || targetController == null) return false;
                var currAnimatorStateInfo = selfController.RoleAnimator.GetCurrentAnimatorStateInfo(0);
                if (!currAnimatorStateInfo.IsTag("idle") && !currAnimatorStateInfo.IsTag("run") &&
                    !currAnimatorStateInfo.IsTag("move")) return false;
                var selfPosition = selfController.transform.position;
                var targetPosition = targetController.transform.position;
                int directionInt = (int)(targetPosition.x - selfPosition.x);
                return ActionCanAttackTarget(roleAIAction, directionInt);
            }
            else
            {
                return false;
            }

        }
        
        // 使用action add by TangJian 2017/11/08 12:40:17
        public void DoAction(RoleAIAction roleAIAction)
        {
            var selfPosition = selfController.transform.position;
            var targetPosition = targetController.transform.position;
            int directionInt = (targetPosition.x - selfPosition.x) > 0 ? 1 : -1;

            // 设置角色朝向 add by TangJian 2018/01/11 23:01:46
            selfController.SetDirectionInt(directionInt);
            roleAIAction.usetime = Time.time;
            
            selfController.RoleAnimator.SetBool(roleAIAction.actionId, true);
            selfController.RoleAnimator.SetTrigger(roleAIAction.actionId);

            Debug.Log("actionId:" + roleAIAction.actionId);
        }
        
        public void FinishAction(RoleAIAction roleAIAction)
        {
            selfController.RoleAnimator.SetBool(roleAIAction.actionId, false);
        }

        public void DoAction(string actionId)
        {
            selfController.RoleAnimator.SetBool(actionId, true);
        }
        
        public Vector3 GetDoActionPos(RoleAIAction action, Direction direction)
        {
            if (TargetController.GetDirection() == Direction.Right)
            {
                if (direction == Direction.Right)
                {
                    return GetAttackTargetPositionWithAction(action, Direction.Right);
                }
                else
                {
                    return GetAttackTargetPositionWithAction(action, Direction.Left);
                }
            }
            else
            {
                if (direction == Direction.Right)
                {
                    return GetAttackTargetPositionWithAction(action, Direction.Left);
                }
                else
                {
                    return GetAttackTargetPositionWithAction(action, Direction.Right);
                }
            }
        }
        
        // 播放行为树连击列表 add by tianjinpeng 2018/08/14 11:24:08
        public IEnumerator PlayActionList( List<string> actionnamelist)
        {
            // bool onoff=false;
            // IEnumerator ienumerator;
            for (int i = 0; i < actionnamelist.Count; i++)
            {
                RoleAIAction curr = actionList.Find((RoleAIAction roleAIAction) =>
                {
                    return actionList[i].name == roleAIAction.name;
                });
                if (curr != null)
                {
                    if (Time.time - curr.usetime >= curr.interval || curr.usetime == 0)
                    {
                        CurrAction = curr;
                        DoAction(CurrAction);
                    }
                }
                yield return new UnityEngine.WaitForSeconds(5f);
            }
        }
        
        
        // 巡逻 add by tianjinpeng 2018/08/09 16:26:19
        public IEnumerator Patrol(float speed)
        {
            float sdfsa = Random.Range(-1000f, 1000f) / 1000 * 10f;
            Vector3 moveByPos = new Vector3(Random.Range(-1000f, 1000f) / 1000 * 10f, 0, Random.Range(-1000f, 1000f) / 1000 * 10f);
            Vector3 moveToPos = transform.position + moveByPos;
            IEnumerator movepath = MovePath(moveToPos, speed);
            while (true)
            {
                if (movepath.MoveNext())
                {
                    yield return CoroutineStatus.Running;
                }
                else
                {
                    break;
                }
            }
        }
        
        
         // 环形巡逻 add by tianjinpeng 2018/08/13 10:44:13
        public IEnumerator RingPatrolsloop( float mindist, float maxdist, float speed)
        {
            int retryTimes = 0;

            RingPatrolsloopBegin:
            
            float radius = Random.Range(mindist, maxdist);
            // float idletime=Random.Range(minidletime,maxidletime);
            float radian = Random.Range(0f, Mathf.PI * 2);
            Vector3 mpos = new Vector3(Mathf.Cos(radian) * radius, 0, Mathf.Sin(radian) * radius);

            Vector3 moveToPos = selfController.transform.position + mpos;
            Vector3[] sda;
            int tryCalcPath = roleNavMeshAgent.TryCalcPathNew(moveToPos, out sda);
            if (tryCalcPath == 2)
            {
                if (retryTimes <= 3)
                {
                    retryTimes++;
                    goto RingPatrolsloopBegin;
                }
            }else if(tryCalcPath == 0)
            {

            }
            else if(tryCalcPath == 1)
            {
                IEnumerator movepath = MovePath(moveToPos, speed, sda);

                while (true)
                {
                    if (movepath.MoveNext())
                    {
                        yield return CoroutineStatus.Running;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        // 保持位置在目标的环形区域内 add by tianjinpeng 2018/08/13 10:45:10
        public IEnumerator KeepDistance( float minKeepDist, float maxKeepDist, float minangle, float maxangle, float speed)
        {
            float begisandex = 0;
            bool tryCalcPath = false;
            bool onfo;
            keepDistanceBegin:
            float keepdist;
            if ((selfController.transform.position - TargetController.transform.position).magnitude < minKeepDist
                || (selfController.transform.position - TargetController.transform.position).magnitude > maxKeepDist)
            {
                onfo = true;
                if (minKeepDist == maxKeepDist)
                {
                    keepdist = minKeepDist;
                }
                else
                {
                    keepdist = Random.Range(minKeepDist, maxKeepDist);
                }
                float radian;

                if (minangle == 0 && maxangle == 0)
                {
                    radian = Tools.Vradian(TargetController.transform.position, selfController.transform.position);
                }
                else
                {
                    float currradian = Tools.Vradian(TargetController.transform.position, selfController.transform.position);
                    float minangleradian = (Mathf.PI / 180f) * minangle;
                    float maxangleradian = (Mathf.PI / 180f) * maxangle;
                    radian = Random.Range(currradian + minangleradian, currradian + maxangleradian);
                }
                // radian=Random.Range(0f,Mathf.PI*2);
                Vector3 mpos = new Vector3(Mathf.Cos(radian) * keepdist, 0, Mathf.Sin(radian) * keepdist);


                Vector3 moveToPos = TargetController.transform.position + mpos;
                Vector3[] sda;
                tryCalcPath = roleNavMeshAgent.TryCalcPath(moveToPos, out sda);
                IEnumerator movepath;
                if (tryCalcPath == false)
                {
                    begisandex++;
                    if (begisandex < 30)
                    {
                        goto keepDistanceBegin;
                    }
                    else
                    {
                        Debug.Log("次数超过30");
                        goto keepDistanceEnd;

                    }
                }
                movepath = MovePath(moveToPos, speed, sda);
                while (tryCalcPath)
                {
                    if (movepath.MoveNext())
                    {
                        yield return CoroutineStatus.Running;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                onfo = false;
            }
            keepDistanceEnd:
            if (onfo)
            {
                if (tryCalcPath)
                {
                    yield return CoroutineStatus.Success;
                }
                else
                {
                    yield return CoroutineStatus.Failure;
                }
            }




        }

        // 移动到对象 add by TangJian 2017/11/08 12:39:49
        public  IEnumerator MoveToTarget( float speed)
        {
            // MoveToTarget_Begin:
            Vector3 selfBeginPostion = selfController.transform.position;
            Vector3 targetBeginPosition = TargetController.transform.position;

            var i_moveTo = MovePath(targetBeginPosition, speed);
            while (true)
            {
                if (i_moveTo.MoveNext())
                {
                    yield return CoroutineStatus.Running;
                }
                else
                {
                    break;
                }
            }
        }
        
        public IEnumerator MoveToPos( Vector3 pos, float speed)
        {
            Vector3[] path;
            IEnumerator i_moveTo = null;
            if (roleNavMeshAgent.TryCalcPath(pos, out path))
            {
                i_moveTo = MovePath(pos, speed, path);
            }
            else
            {
                yield return CoroutineStatus.Failure;
            }
            if (i_moveTo != null)
            {
                while (i_moveTo.MoveNext())
                {
                    if (i_moveTo.MoveNext())
                    {
                        yield return CoroutineStatus.Running;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        // 移动到玩家斜后方 add by TangJian 2017/11/13 14:51:33
        public IEnumerator MoveToTargetBack( RoleAIAction roleAIAction, float speed, bool isFront = false, float distanceZ = 0)
        {
            var retryTimes = 0;
            MoveToTargetBack_Begin:
            retryTimes++;

            var selfToTargetPosition = TargetController.transform.position - selfController.transform.position;

            Direction direction = Direction.Right.FloatToDirection(selfToTargetPosition.x);

            var targetDirection = TargetController.GetDirection();
            if (isFront)
                targetDirection = targetDirection.Reverse();

            var targetBeginPosition = TargetController.transform.position;

            var targetFrontPosition = GetAttackTargetPositionWithAction(roleAIAction, targetDirection);
            var targetBackPosition = GetAttackTargetPositionWithAction(roleAIAction, targetDirection.Reverse());

            var targetRightPosition = GetAttackTargetPositionWithAction(roleAIAction, direction);
            var targetLeftPosition = GetAttackTargetPositionWithAction(roleAIAction, direction.Reverse());

            Vector3[] path;

            Vector3 stepPosition = Vector3.zero;

            // 如果目标背面朝着自己 add by TangJian 2017/11/13 17:10:55
            if (direction == targetDirection)
            {
                // 看走上还是走下 add by TangJian 2017/11/13 16:58:44
                if (roleNavMeshAgent.TryCalcPath(targetBackPosition, out path))
                {
                    stepPosition = targetBackPosition;
                }
                else if (roleNavMeshAgent.TryCalcPath(targetFrontPosition, out path))
                {
                    stepPosition = targetFrontPosition;
                }

                if (path.Length > 0)
                {
                    var i_moveto = MovePath(targetBackPosition, speed, path);
                    while (i_moveto.MoveNext())
                    {
                        if ((TargetController.transform.position - targetBeginPosition).magnitude > 1.5f
                        )//|| isFront ? targetDirection != targetController.GetDirection().Reverse() : targetDirection != targetController.GetDirection())
                        {
                            Debug.Log("玩家移动太远重新寻路1");
                            goto MoveToTargetBack_Begin;
                        }

                        yield return CoroutineStatus.Running;
                    }
                }
            }
            else
            {
                var targetFrontUpPosition = new Vector3(targetFrontPosition.x + Random.Range(-1f, 1f), targetFrontPosition.y, targetFrontPosition.z + distanceZ);
                var targetFrontDownPosition = new Vector3(targetFrontPosition.x + Random.Range(-1f, 1f), targetFrontPosition.y, targetFrontPosition.z - distanceZ);

                var targetBackUpPosition = new Vector3(targetBackPosition.x + Random.Range(-1f, 1f), targetBackPosition.y, targetBackPosition.z + distanceZ);
                var targetBackDownPosition = new Vector3(targetBackPosition.x + Random.Range(-1f, 1f), targetBackPosition.y, targetBackPosition.z - distanceZ);

                // 第一步, 移动到玩家正面上方或者下方 add by TangJian 2017/11/13 17:04:58
                {
                    if (selfToTargetPosition.y > 0)
                    {
                        if (roleNavMeshAgent.TryCalcPath(targetFrontDownPosition, out path))
                        {
                            stepPosition = targetFrontDownPosition;
                        }
                        else if (roleNavMeshAgent.TryCalcPath(targetFrontUpPosition, out path))
                        {
                            stepPosition = targetFrontUpPosition;
                        }
                    }
                    else
                    {
                        if (roleNavMeshAgent.TryCalcPath(targetFrontUpPosition, out path))
                        {
                            stepPosition = targetFrontUpPosition;
                        }
                        else if (roleNavMeshAgent.TryCalcPath(targetFrontDownPosition, out path))
                        {
                            stepPosition = targetFrontDownPosition;
                        }
                    }

                    if (path.Length > 0)
                    {
                        var i_moveto = MovePath(stepPosition, speed, path);
                        while (i_moveto.MoveNext())
                        {
                            if ((TargetController.transform.position - targetBeginPosition).magnitude > 1.5f
                            )//|| isFront ? targetDirection != targetController.GetDirection().Reverse() : targetDirection != targetController.GetDirection())
                            {
                                Debug.Log("玩家移动太远重新寻路2");
                                goto MoveToTargetBack_Begin;
                            }

                            yield return CoroutineStatus.Running;
                        }
                    }
                }

                // 第二步 绕到玩家后方 add by TangJian 2017/11/13 17:27:27
                {
                    if (selfToTargetPosition.y > 0)
                    {
                        if (roleNavMeshAgent.TryCalcPath(targetBackDownPosition, out path))
                        {
                            stepPosition = targetBackDownPosition;
                        }
                        else if (roleNavMeshAgent.TryCalcPath(targetBackUpPosition, out path))
                        {
                            stepPosition = targetBackUpPosition;
                        }
                    }
                    else
                    {
                        if (roleNavMeshAgent.TryCalcPath(targetBackUpPosition, out path))
                        {
                            stepPosition = targetBackUpPosition;
                        }
                        else if (roleNavMeshAgent.TryCalcPath(targetBackDownPosition, out path))
                        {
                            stepPosition = targetBackDownPosition;
                        }
                    }

                    if (path.Length > 0)
                    {
                        var i_moveto = MovePath(stepPosition, speed, path);
                        while (i_moveto.MoveNext())
                        {
                            if ((TargetController.transform.position - targetBeginPosition).magnitude > 1.5f
                            )//|| isFront ? targetDirection != targetController.GetDirection().Reverse() : targetDirection != targetController.GetDirection())
                            {
                                Debug.Log("玩家移动太远重新寻路3");
                                goto MoveToTargetBack_Begin;
                            }

                            yield return CoroutineStatus.Running;
                        }
                    }
                }

                // 第三步 移动到玩家背后 add by TangJian 2017/11/13 17:27:27
                {
                    if (roleNavMeshAgent.TryCalcPath(targetBackPosition, out path))
                    {
                        stepPosition = targetBackUpPosition;
                    }

                    if (path.Length > 0)
                    {
                        var i_moveto = MovePath(stepPosition, speed, path);
                        while (i_moveto.MoveNext())
                        {
                            if ((TargetController.transform.position - targetBeginPosition).magnitude > 1.5f
                            )//|| isFront ? targetDirection != targetController.GetDirection().Reverse() : targetDirection != targetController.GetDirection())
                            {
                                Debug.Log("玩家移动太远重新寻路4");
                                goto MoveToTargetBack_Begin;
                            }

                            yield return CoroutineStatus.Running;
                        }
                    }
                }
            }

            yield return CoroutineStatus.Success;
        }

        public IEnumerator MoveToCanDoAction( RoleAIAction roleAIAction, float speed)
        {
            MoveToCanDoAction_Begin:
            Vector3 selfBeginPostion = selfController.transform.position;
            Vector3 targetBeginPosition = TargetController.transform.position;

            var selfToTargetPosition = TargetController.transform.position - selfController.transform.position;
            Direction direction = Direction.Right.FloatToDirection(selfToTargetPosition.x);
            var targetRightPosition = GetAttackTargetPositionWithAction(roleAIAction, direction);
            var targetLeftPosition = GetAttackTargetPositionWithAction(roleAIAction, direction.Reverse());

            Vector3[] path;
            Vector3 stepPosition = Vector3.zero;
            if (roleNavMeshAgent.TryCalcPath(targetLeftPosition, out path))
            {
                stepPosition = targetLeftPosition;
            }
            else if (roleNavMeshAgent.TryCalcPath(targetRightPosition, out path))
            {
                stepPosition = targetRightPosition;
            }
            if (ActionCanAttackTarget(roleAIAction, selfController.GetDirectionInt()))
            {
                yield return CoroutineStatus.Success;
            }
            else
            {
                var i_moveTo = MovePath(stepPosition, speed);
                while (i_moveTo.MoveNext())
                {
                    if ((TargetController.transform.position - targetBeginPosition).magnitude > 1.5f)//|| isFront ? targetDirection != targetController.GetDirection().Reverse() : targetDirection != targetController.GetDirection())
                    {
                        Debug.Log("玩家移动太远重新寻路1");
                        goto MoveToCanDoAction_Begin;
                    }
                    var sPosition = TargetController.transform.position - selfController.transform.position;
                    Direction dir = Direction.Right.FloatToDirection(sPosition.x);
                    if (direction != dir)
                    {
                        Debug.Log("玩家移动朝向重新寻路1");
                        goto MoveToCanDoAction_Begin;
                    }
                    yield return CoroutineStatus.Running;
                }
                yield return CoroutineStatus.Success;
            }
            yield return CoroutineStatus.Failure;
        }


        public IEnumerator PatrolTime(float time, float speed)
        {
            float fristtime = Time.time;
            PatrolTimeBegin:
            IEnumerator patrol = Patrol(speed);
            while (true)
            {
                if (patrol.MoveNext())
                {
                    yield return CoroutineStatus.Running;
                    if (Time.time - fristtime >= time)
                    {
                        break;
                    }
                }
                else
                {
                    if (Time.time - fristtime < time)
                    {
                        yield return CoroutineStatus.Running;
                        goto PatrolTimeBegin;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }


        public IEnumerator JumpToTargetBackDoAction( RoleAIAction currAction, float distance)
        {
            JumpToTargetBackDoActionBegin:
            Vector3 selfBeginPos = selfController.transform.position;
            Vector3 targetBeginPos = TargetController.transform.position;
            Vector3 selfToTargetPos = targetBeginPos - selfBeginPos;
            Vector3 desPos = targetBeginPos;
            if (selfToTargetPos.x * TargetController.GetDirectionInt() > 0)
            {
                desPos = GetDoActionPos(currAction, Direction.Right);
            }
            else
            {
                desPos = GetDoActionPos(currAction, Direction.Left);
            }
            Vector3[] jumppathlist = roleNavMeshAgent.JumpPathdian(desPos, distance);
            IEnumerator<string> jumpPath = JumpPath(distance, jumppathlist);
            while (jumpPath.MoveNext())
            {
                if (jumpPath.Current != null)
                {
                    if (jumpPath.Current == "JumpToFinished" && Vector3.Magnitude(targetBeginPos - TargetController.transform.position) > 2)
                    {
                        if (selfController.RoleAnimator.GetBool("isGrounded"))
                        {
                            goto JumpToTargetBackDoActionBegin;
                        }
                    }
                }
                yield return CoroutineStatus.Running;
            }
            // IEnumerator jumppath=JumpPath();
        }


        public IEnumerator<string> JumpPath( float distance, Vector3[] path = null)
        {
            if (path != null)
            {
                for (int i = 1; i < path.Length; i++)
                {
                    Vector3 position = path[i];
                    float waittime = Time.time;
                    ForwardToTarget();
                    IEnumerator jumpto = JumpTo(position, selfController.RoleData.MoveSpeedScale, distance, waittime);
                    do
                    {
                        if (jumpto.MoveNext())
                        {
                            yield return "JumpToFinished";
                        }
                        else
                        {
                            yield return "CoroutineStatus.Running";
                        }
                    } while (jumpto.MoveNext());
                }
            }

        }

        public IEnumerator JumpTo( Vector3 pos, float speed, float distance, float waittime)
        {
            bool booljump = true;
            float beginTime = Time.time;
            float currTime = Time.time;

            Vector3 beginPos = selfController.transform.position;
            Vector3 endPos = pos;
            Vector3 offset = endPos - beginPos;
            offset.y = 0f;

            float duration = offset.magnitude / speed;
            float speedPerFrame = speed / 60f;

            float elapsedTime = 0;
            float percent = 0;
            float loopTimes = 0;

            while (true)
            {
                loopTimes = loopTimes + 1;
                if (selfController.CurrAnimStateIsTag("idle") || selfController.CurrAnimStateIsTag("run") || selfController.CurrAnimStateIsTag("move") || selfController.CurrAnimStateIsTag("accelerateRoll"))
                {
                    if (booljump)
                    {
                        elapsedTime = Time.time - beginTime;
                        percent = Tools.Range(elapsedTime / duration, 0f, 1f);

                        Vector3 currPos = beginPos + offset * percent;
                        Vector3 cloneposition = new Vector3(selfController.transform.position.x, 0, selfController.transform.position.z);
                        Vector3 sda = selfController.transform.InverseTransformPoint(new Vector3(0, GetParaCurveLinePointY(distance, distance * percent), 0));

                        Vector3 moveByPos = currPos - cloneposition;
                        moveByPos.y = sda.y;
                        Debug.Log("moveByPos" + moveByPos);
                        selfController.Move(moveByPos);
                    }
                    if (percent >= 1)
                    {
                        ForwardToTarget();
                        selfController.RoleAnimator.SetBool("is_moving", false);
                        booljump = false;
                        if (Time.time - waittime > 1.2f)
                        {
                            break;
                        }
                        else
                        {
                            ForwardToTarget();
                            yield return CoroutineStatus.Running;
                        }
                    }
                    else
                    {
                        selfController.RoleAnimator.SetBool("is_moving", true);
                        yield return CoroutineStatus.Running;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        
        
        public float GetParaCurveLinePointY(float Pathdistance, float distance)
        {
            float x = Pathdistance / 2;
            float y = Pathdistance / 2;
            float a = y / (Mathf.Pow(x, 2) - Pathdistance * x);
            float LinePointY = a * (Mathf.Pow(distance, 2) - Pathdistance * distance);
            return LinePointY;
        }
        
        
    }
}