using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tang.FrameEvent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Tang
{
    public class SkillManager : MyMonoBehaviour
    {
        private static SkillManager instance;
        public static SkillManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<SkillManager>();
                }
                return instance;
            }
        }
        private string skillDataAssetFile{ get{ return "Manager/SkillManagerDataAsset.asset"; } }
        private string skillDatalistAssetFile  { get{ return "Manager/SkillListManagerDataAsset"; } }
        private Dictionary<string, SkillData> skillDataMap = new Dictionary<string, SkillData>();
        private Dictionary<string, SkillListSaveData> skillListSaveDataDic = new Dictionary<string, SkillListSaveData>();
        //public SkillListManagerDataAsset skillListManagerDataAsset;
        public SkillManagerDataAsset SkillManagerDataAsset;
        void Start()
        {
            load();
        }

        async void load()
        {
            
            SkillManagerDataAsset = await AssetManager.LoadAssetAsync<SkillManagerDataAsset>(skillDataAssetFile);
           
            string textString = (await AssetManager.LoadAssetAsync<TextAsset>("SkillDatas")).text;
            skillListSaveDataDic = Tools.Json2Obj<Dictionary<string, SkillListSaveData>>(textString);
            if (SkillManagerDataAsset != null&& SkillManagerDataAsset.skillListSaveDatas.Count!=0)
            {
                skillDataMap = SkillManagerDataAsset.skillListSaveDatas.ToDictionary(item => item.id.ToLower(), item => item);
            }
            else
            {
                string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + "Resources_moved/Scripts/Skill/Skill.json");
                skillDataMap = Tools.Json2Obj<Dictionary<string, SkillData>>(jsonString).ToDictionary(pair => pair.Key.ToLower(), pair => pair.Value);
            }

            var a = 1;
            //skillListSaveDataDic = Tools.getObjectFromResource<Dictionary<string, SkillListSaveData>>("Scripts/Skill/SkillList");
        }

        public SkillData GetSkillData(string id)
        {
            SkillData ret;
            return skillDataMap.TryGetValue(id.ToLower(), out ret) ? ret : null;
        }
        public SkillListSaveData GetSkillListSaveData(string id)
        {
            SkillListSaveData skillListSaveData;
            return skillListSaveDataDic.TryGetValue(id, out skillListSaveData) ? skillListSaveData : null;
        }

        private void HasSkillGroup(SkillData skillData, Transform owner
            , string TeamId, Direction direction, Vector3 SkillFrameEventpos)
        {
            if (skillData.IsMorePlay&&skillData.PlayCount>1)
            {
                float interval = IntervalTime(skillData.DurationTime, skillData.PlayCount);
                this.DelayToDo(interval, (() =>
                {
                    StartCoroutine(WaitSkillGroup(interval, skillData.PlayCount-1,
                        skillData, owner, TeamId, direction,SkillFrameEventpos));
                }));
            }
        }
       
        private IEnumerator WaitSkillGroup(float interval,int count,SkillData skill,Transform owner, string TeamId
            , Direction direction,Vector3 SkillFrameEventpos)
        {
            for (int i = 0; i < count; i++)
            {
                UseSkill(skill,owner,TeamId,direction,SkillFrameEventpos,FrameEventInfo.RoleSkillFrameEventData.SkillActionType.Default,false);
                float time = 0;
                while (time < interval)
                {
                    time += UnityEngine.Time.deltaTime;
                    yield return null;
                }
            }
            yield return null;
        }
        
        private float IntervalTime(float duration,int count)
        {
            return count / duration;
        }
        
        public ISkillController UseSkill(string id, Transform owner, string TeamId
            , Direction direction, Vector3 SkillFrameEventpos
            , FrameEventInfo.RoleSkillFrameEventData.SkillActionType skillActionType = FrameEventInfo.RoleSkillFrameEventData.SkillActionType.Default)
        {
            if (GetSkillData(id) is SkillData skill)
            {
                ISkillController retSkillController = UseSkill(skill, owner, TeamId, direction, SkillFrameEventpos, skillActionType);
                
                // 自己释放的技能忽略自己
                retSkillController.SetIgnoreList(new List<int>(){owner.gameObject.GetHashCode()});
                
                return retSkillController;
            }
            Debug.Log("找不到skilldata");
            return null;

        }

        public ISkillController UseSkill(SkillData skillData, Transform owner
            , string TeamId, Direction direction, Vector3 SkillFrameEventpos,
            FrameEventInfo.RoleSkillFrameEventData.SkillActionType skillActionType = FrameEventInfo.RoleSkillFrameEventData.SkillActionType.Default,bool IsFirst = true)
        {
            if (skillData.IsPhasesSkill)
            {
                UseSkillPhases(skillData, owner, TeamId, direction, SkillFrameEventpos);
                return null;
            }
            if (IsFirst) HasSkillGroup(skillData, owner, TeamId, direction, SkillFrameEventpos);

            switch (skillData.type)
            {
                case SkillGroupType.None:
                    return UseSkill_Single(skillData, owner, TeamId, direction, SkillFrameEventpos);
                case SkillGroupType.Random:
                    return UseSkillRandom(skillData, owner, TeamId, direction, SkillFrameEventpos);
                case SkillGroupType.Sequence:
                    UseSkillSequence(skillData, owner, TeamId, direction, SkillFrameEventpos);
                    break;
                default:
                    Debug.LogError("SkillGroupType类型"+skillData.type);
                    break;
            }

            return null;
        }
        
        public ISkillController UseSkillRandom(SkillData skillData, Transform owner
            , string TeamId, Direction direction, Vector3 SkillFrameEventpos)
        {
            int Index = Random.Range(0, skillData.skillDatas.Count);
            var item = skillData.skillDatas[Index];
            skillData = item;
            
            return UseSkill_Single(skillData, owner, TeamId, direction, SkillFrameEventpos);
        }
        
        public void UseSkillSequence(SkillData skillData, Transform owner
            , string TeamId, Direction direction, Vector3 SkillFrameEventpos)
        {
            GameObject go = new GameObject();
            go.transform.parent = owner.parent;
            go.transform.localPosition = new Vector3(owner.localPosition.x,owner.localPosition.y,owner.localPosition.z);
            
            if (skillData.skillDatas[0].IsRandomTrans)
            {
                var x = Random.Range(skillData.skillDatas[0].MinRandomVector3.x, skillData.skillDatas[0].MaxRandomVector3.x+0.001f);
                var y = Random.Range(skillData.skillDatas[0].MinRandomVector3.y, skillData.skillDatas[0].MaxRandomVector3.y+0.001f);
                var z = Random.Range(skillData.skillDatas[0].MinRandomVector3.z, skillData.skillDatas[0].MaxRandomVector3.z+0.001f);
                go.transform.localPosition = new Vector3(x,y,z)+new Vector3(owner.localPosition.x,owner.localPosition.y,owner.localPosition.z);
            }
            
            Transform trans = go.transform;
            for (int i = 0; i < skillData.skillDatas.Count; i++)
            {
                var skill = skillData.skillDatas[i];
                        
                DelayFunc(() =>
                {
                    UseSkill(skill,trans,TeamId,direction,SkillFrameEventpos);
                }, skill.DelayTime);
            }
        }

        public void UseSkillPhases(SkillData skillData, Transform owner
            , string TeamId, Direction direction, Vector3 SkillFrameEventpos)
        {
            UseSkill_Single(skillData.FrontSkillData,owner,TeamId,direction,SkillFrameEventpos);
            DelayFunc(() =>
            {
                UseSkill_Single(skillData.CentreSkillData,owner,TeamId,direction,SkillFrameEventpos);
            }, skillData.FrontSkillData.AnimTime);
            
            
            
        }
        
        
        public ISkillController UseSkill_Single(SkillData skillData, Transform owner
            , string TeamId, Direction direction, Vector3 SkillFrameEventpos)
        {
            GameObject gameobject = GameObjectManager.Instance.Spawn("SkillPrefab");
            
            if (string.IsNullOrEmpty(skillData.componentTypeName)) return null;
            System.Type type = System.Type.GetType(skillData.componentTypeName);
            ISkillController skillController = gameobject.AddComponent(type) as ISkillController;
            
            SetSkillController(skillData,gameobject,ref skillController ,owner,TeamId,direction,SkillFrameEventpos);

            return skillController;
        }

        private void SetSkillController(SkillData skillData,GameObject gameobject,ref ISkillController skillController,Transform owner
            , string TeamId, Direction direction, Vector3 SkillFrameEventpos)
        {
            if (string.IsNullOrEmpty(skillData.id))return;
                // 基本参数
            skillController.TeamId = TeamId;
            skillController.Owner = owner.gameObject;
            
            // 设置位置 add by TangJian 2019/4/20 12:25
            
            gameobject.transform.parent = skillData.parentType == FrameEventInfo.ParentType.Parent ? owner.parent : owner;
            gameobject.transform.localPosition = owner.transform.localPosition + new Vector3(skillData.pos.x * getDirectionInt(direction), skillData.pos.y, skillData.pos.z) + new Vector3(SkillFrameEventpos.x * getDirectionInt(direction), SkillFrameEventpos.y, SkillFrameEventpos.z);
        
            // 设置方向 add by TangJian 2019/4/20 12:25
            skillController.Direction = direction;
        
            // 速度 add by TangJian 2018/04/13 20:10:56
            skillController.Speed = new Vector3(skillData.speed.x * getDirectionInt(direction), skillData.speed.y, skillData.speed.z);
            
        
            skillController.InitSkill(skillData);
        }
        
        
        
        public void useSkillList(string id, Direction direction, Transform target, string teamid)
        {
            SkillListSaveData skillListSaveData = GetSkillListSaveData(id);
            if (skillListSaveData != null)
            {
                useSkillList(skillListSaveData.roleSkillListFrameEventData, direction, target, teamid);
            }
            else
            {
                Debug.Log("找不到SkillListSaveData");
            }
        }
        
        public void useSkillList(RoleSkillListFrameEventData roleSkillListFrameEvent, Direction direction, Transform target, string teamid)
        {
            RunSkillList(roleSkillListFrameEvent, () =>
            {

            }, new Vector3(0, 0, 0), direction, target, teamid);
        }
        public void RunSkillList(RoleSkillListFrameEventData roleSkillListFrameEvent, System.Action onComplete, Vector3 pos, Direction direction = Direction.Right, Transform target = null, string teamid = "")
        {
            Vector3 add = PosCalculation(roleSkillListFrameEvent.postype, pos, roleSkillListFrameEvent.pos1, roleSkillListFrameEvent.pos2, 1);
            float time = TimeCalculation(roleSkillListFrameEvent.timetype, roleSkillListFrameEvent.time1, roleSkillListFrameEvent.time2);
            if (time == 0f)
            {
                switch (roleSkillListFrameEvent.RoleSkillListType)
                {
                    case RoleSkillListType.sequence:
                        RunSequence(roleSkillListFrameEvent.roleSkillListFrameEvents, onComplete, 0, add, direction, target, teamid);
                        break;
                    case RoleSkillListType.parallel:
                        RunParallel(roleSkillListFrameEvent.roleSkillListFrameEvents, onComplete, add, direction, target, teamid);
                        break;
                    case RoleSkillListType.random:
                        RunRandom(roleSkillListFrameEvent.roleSkillListFrameEvents, onComplete, add, direction, target, teamid);
                        break;
                    case RoleSkillListType.SkillList:
                        RunSkillListaction(roleSkillListFrameEvent.skillListDatas, onComplete, add, direction, target, teamid);
                        break;
                    default:
                        onComplete();
                        break;
                }
            }
            else
            {
                DelayFunc(() =>
                    {
                        switch (roleSkillListFrameEvent.RoleSkillListType)
                        {
                            case RoleSkillListType.sequence:
                                RunSequence(roleSkillListFrameEvent.roleSkillListFrameEvents, onComplete, 0, add, direction, target, teamid);
                                break;
                            case RoleSkillListType.parallel:
                                RunParallel(roleSkillListFrameEvent.roleSkillListFrameEvents, onComplete, add, direction, target, teamid);
                                break;
                            case RoleSkillListType.random:
                                RunRandom(roleSkillListFrameEvent.roleSkillListFrameEvents, onComplete, add, direction, target, teamid);
                                break;
                            case RoleSkillListType.SkillList:
                                RunSkillListaction(roleSkillListFrameEvent.skillListDatas, onComplete, add, direction, target, teamid);
                                break;
                            default:
                                onComplete();
                                break;
                        }
                    }
                    , time);
            }
        }
        public void RunSequence(List<RoleSkillListFrameEventData> roleSkillListFrameEvents, System.Action onComplete, int index, Vector3 pos, Direction direction = Direction.Right, Transform target = null, string teamid = "")
        {

            if (roleSkillListFrameEvents.Count <= 0)
            {
                onComplete();
                return;
            }
            RunSkillList(roleSkillListFrameEvents[index], () =>
            {
                index++;
                if (index < roleSkillListFrameEvents.Count)
                {
                    RunSequence(roleSkillListFrameEvents, onComplete, index, pos, direction, target, teamid);
                }
                else
                {
                    onComplete();
                }
            }, pos, direction, target, teamid);
        }
        public void RunParallel(List<RoleSkillListFrameEventData> roleSkillListFrameEvents, System.Action onComplete, Vector3 pos, Direction direction = Direction.Right, Transform target = null, string teamid = "")
        {

            if (roleSkillListFrameEvents.Count <= 0)
            {
                onComplete();
                return;
            }
            Vector3 vector = pos;
            int completeCount = 0;
            for (int i = 0; i < roleSkillListFrameEvents.Count; i++)
            {
                RunSkillList(roleSkillListFrameEvents[i], () =>
                {
                    completeCount++;
                    if (completeCount == roleSkillListFrameEvents.Count)
                    {
                        onComplete();
                    }
                }, vector, direction, target, teamid);
            }
        }
        public void RunRandom(List<RoleSkillListFrameEventData> roleSkillListFrameEvents, System.Action onComplete, Vector3 pos, Direction direction = Direction.Right, Transform target = null, string teamid = "")
        {
            if (roleSkillListFrameEvents.Count <= 0)
            {
                onComplete();
                return;
            }
            int index = Tools.RandomWithWeight(roleSkillListFrameEvents, (RoleSkillListFrameEventData ar, int i) =>
            {
                return ar.weight;
            });
            RunSkillList(roleSkillListFrameEvents[index], () =>
            {
                onComplete();
            }, pos, direction, target, teamid);
        }
        public void RunSkillListaction(List<SkillListData> SkillListDatas, System.Action onComplete, Vector3 pos, Direction direction = Direction.Right, Transform target = null, string teamid = "")
        {
            if (SkillListDatas.Count <= 0)
            {
                onComplete();
                return;
            }
            string onlyid = Tools.getOnlyId().ToString();
            for (int i = 0; i < SkillListDatas.Count; i++)
            {
                SkillListData currskillListData = SkillListDatas[i];
                float time = TimeCalculation(SkillListDatas[i].timetype, SkillListDatas[i].time1, SkillListDatas[i].time2);
                Vector3 add = PosCalculation(SkillListDatas[i].postype, pos, SkillListDatas[i].pos1, SkillListDatas[i].pos2, 1);
                if (time == 0f)
                {
                    UseSkill(SkillListDatas[i].id, target, teamid, direction, add);
                }
                else
                {
                    DelayFunc(onlyid + "-" + i.ToString() + "-" + SkillListDatas[i].id, () =>
                    {
                        UseSkill(currskillListData.id, target, teamid, direction, add);
                    }, time);
                }

            }
            onComplete();
        }
        public float TimeCalculation(useskilltype timetype, float time1, float time2)
        {
            float time = 0;
            switch (timetype)
            {
                case useskilltype.fixeda:
                    time = time1;
                    break;
                case useskilltype.random:
                    time = Random.Range(time1, time2);
                    break;
            }
            return time;
        }
        public Vector3 PosCalculation(useskilltype postype, Vector3 pos, Vector3 pos1, Vector3 pos2, int dir)
        {
            Vector3 add = new Vector3();
            switch (postype)
            {
                case useskilltype.fixeda:
                    add = pos + new Vector3(pos1.x * dir, pos1.y, pos1.z); ;
                    break;
                case useskilltype.random:
                    Vector3 sdas = Vector3Extensions.RandomMinMax(pos1, pos2);
                    add = pos + new Vector3(sdas.x * dir, sdas.y, sdas.z);
                    break;
            }
            return add;
        }
        public async Task NewAnim(GameObject anim,SkillData skillData)
        {
            Animator animator = anim.GetComponent<Animator>();
            Spine.Unity.SkeletonAnimator skeletonAnimator = anim.GetComponent<Spine.Unity.SkeletonAnimator>();

            var skeletonAnimatorRenderer = skeletonAnimator.GetComponent<Renderer>();
            skeletonAnimatorRenderer.enabled = false;
            DelayFunc(() =>
            {
                skeletonAnimatorRenderer.enabled = true;
            }, 0.00001f);

            animator.runtimeAnimatorController =
                await AssetManager.LoadAssetAsync<RuntimeAnimatorController>(skillData.AnimControllerPath);

            skeletonAnimator.skeletonDataAsset =
                await AssetManager.LoadAssetAsync<Spine.Unity.SkeletonDataAsset>(skillData.SkeletonDataAssetPath);

            skeletonAnimator.initialSkinName = skillData.SkinName;
            skeletonAnimator.Initialize(true);
        }
        
        public async void NewAnimation(GameObject Rander,SkillData skillData)
        {
            GameObject Animation = Rander.GetChild("Animation", true);
            Spine.Unity.SkeletonAnimation skeletonAnimation = Animation.AddComponent<Spine.Unity.SkeletonAnimation>();
            skeletonAnimation.skeletonDataAsset= await AssetManager.LoadAssetAsync<Spine.Unity.SkeletonDataAsset>(skillData.SkeletonDataAssetPath);
            var skeletonAnimationRenderer = skeletonAnimation.GetComponent<Renderer>();
            skeletonAnimationRenderer.enabled = false;
            DelayFunc(() =>
            {
                skeletonAnimationRenderer.enabled = true;
            }, 0.00001f);
            skeletonAnimation.initialSkinName= skillData.SkinName;
            skeletonAnimation.Initialize(true);
            Spine.TrackEntry TrackEntry = skeletonAnimation.state.SetAnimation(0, skillData.AnimationName, true);
            Animation.AddComponent<SortRenderer>();
        }
        
        public int getDirectionInt(Direction direction)
        {
            return direction == Direction.Right ? 1 : -1;
        }
    }

}