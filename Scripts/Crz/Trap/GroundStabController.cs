using UnityEngine;

/// <summary>
/// 地刺控制
/// </summary>
namespace Tang
{
    public enum GroundStabStateType
    {
        Noloop = 0,
        loop = 1,
    }
    
    public class GroundStabController : PlacementController
    {
        public float attackTime; //攻击延迟
        public float downTime; //关闭延迟

        public float attackInterval; //

        public GroundStabStateType groundStabStateType;
        public float firststate;

        public bool first=true;
        public override object Data {
            get { return this; }
            set
            {
                GroundStabController newThis = value as GroundStabController;
                this.attackTime = newThis.attackTime;
                this.downTime = newThis.downTime;
                this.attackInterval = newThis.attackInterval;
                this.groundStabStateType = newThis.groundStabStateType;
                this.firststate = newThis.firststate;
            }
        }


        public override void Start()
        {
            InitAnimator();

            DamageController.gameObject.SetActive(false);

            var damageData = DamageController.damageData;
            damageData.itriggerDelegate = this;
            damageData.atk = Placementdata.atk;
            damageData.DamageDirectionType = DamageDirectionType.Radial;
            damageData.damageTimes = -1;
            //if (groundStabStateType == GroundStabStateType.loop)
            //{
            //    State = firststate;
            //}
            //else
            {
                State = 0;
            }
            GameManager.Instance.AddGrandstabWaitLoopData(GetInstanceID().ToString(), this);

        }


        public override void OnTriggerIn(TriggerEvent evt)
        {
            if (groundStabStateType == GroundStabStateType.Noloop)
            {
                if (evt.otherTriggerController.ITriggerDelegate != null
                    && evt.otherTriggerController.ITriggerDelegate.GetGameObject() != null
                    && (evt.otherTriggerController.ITriggerDelegate.GetGameObject().tag == "Role"
                        || evt.otherTriggerController.ITriggerDelegate.GetGameObject().tag == "Player")
                    &&evt.otherTriggerController.ITriggerDelegate.GetGameObject().GetComponent<RoleController>().RoleData.TeamId != Placementdata.teamId)
                {
                    DelayFunc("trapUp", () =>
                    {
                        State = 1;
                    }, attackTime);
                }
            }

        }


        public override void OnTriggerOut(TriggerEvent evt)
        {
            if (groundStabStateType == GroundStabStateType.Noloop)
            {
                if (evt.otherTriggerController.ITriggerDelegate == null
                    || evt.otherTriggerController.ITriggerDelegate.GetGameObject() == null
                    || evt.otherTriggerController.ITriggerDelegate.GetGameObject().tag != "Role"
                    || evt.otherTriggerController.ITriggerDelegate.GetGameObject().tag != "Player")
                {
                    DelayFunc("trapDown", () =>
                    {
                        State = 0;
                    }, downTime);
                }
            }
            

        }
        public void OnOfftoStab(Animator animator,float time,int state)
        {
            if (groundStabStateType == GroundStabStateType.loop)
            {
                DelayFunc("trapDownUp-" + animator.gameObject.GetInstanceID(), () =>
                {
                    animator.SetInteger("state", state);
                    if (first)
                        first = false;
                }, time);
            }
        }
        //添加动画帧实现
        //        public virtual void OnEvtAtk()
        //        {
        ////            Debug.Log("动画开始，触发伤害开启");
        ////            DamageObject.SetActive(true);
        //        }
        //
        //        public virtual void AnimDownEnd()
        //        {
        ////            Debug.Log("动画完全结束，触发伤害关闭");
        ////            DamageObject.SetActive(false);
        //        }

    }
}