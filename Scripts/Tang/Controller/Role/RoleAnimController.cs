using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Tang
{
    using FrameEvent;
    
    public class RoleAnimController : MonoBehaviour
    {
        RoleController roleController;
        public AnimData animData;

        Cache cache = new Cache();

        void Start()
        {
            roleController = GetComponentInParent<RoleController>();
        }



        void RoleAtk(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.RoleAtkFrameEventData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            roleController.OnAtk(eventData as FrameEventInfo.RoleAtkFrameEventData);
        }

        void RoleSkill(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.RoleSkillFrameEventData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            roleController.OnSkill(eventData as FrameEventInfo.RoleSkillFrameEventData);
        }

        void PlayAnim(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.PlayAnimFrameEventData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            roleController.OnPlayAnim(eventData as FrameEventInfo.PlayAnimFrameEventData);
        }
        void VariableSpeed(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.VariableRoleMoveSpeedData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            roleController.OnVariableRoleMoveSpeed(eventData as FrameEventInfo.VariableRoleMoveSpeedData);
        }
        void PlayAnimList(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.FrameEventAnimList>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            roleController.OnPlayAnimList(eventData as FrameEventInfo.FrameEventAnimList);
        }

        public AnimAttackEventData getAnimAttackEventData(string name)
        {
            foreach (var animAttackEventData in animData.animAttackEventDatas)
            {
                if (animAttackEventData.animName == name)
                {
                    return animAttackEventData;
                }
            }
            return null;
        }
    }
}