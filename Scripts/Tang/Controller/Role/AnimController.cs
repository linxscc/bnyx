using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Tang
{
    using FrameEvent;
    
    public class PlayEffectData
    {
        public string name;
        public Vector3 pos;
    }


    public class AnimController : MonoBehaviour
    {
        IAnimEventDelegate animEventDelegate;
        Cache cache = new Cache();

        void Start()
        {
            animEventDelegate = GetComponentInParent<IAnimEventDelegate>();
        }

        void RoleAtk(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.RoleAtkFrameEventData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnAtk(eventData as FrameEventInfo.RoleAtkFrameEventData);
        }

        void RoleSkill(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.RoleSkillFrameEventData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnSkill(eventData as FrameEventInfo.RoleSkillFrameEventData);
        }
        
        async void PlayAnim(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData)) 
            {
                eventData = Tools.Json2Obj<FrameEventInfo.PlayAnimFrameEventData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnPlayAnim(eventData as FrameEventInfo.PlayAnimFrameEventData);
        }

        void Custom(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.CustomFrameEventData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnCustom(eventData as FrameEventInfo.CustomFrameEventData);
        }
        void VariableSpeed(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.VariableRoleMoveSpeedData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnVariableRoleMoveSpeed(eventData as FrameEventInfo.VariableRoleMoveSpeedData);
        }
        void PlayAnimList(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.FrameEventAnimList>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnPlayAnimList(eventData as FrameEventInfo.FrameEventAnimList);
        }

        void SetSpeed(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.SetSpeedFrameEvent>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnSetSpeedFrameEvent(eventData as FrameEventInfo.SetSpeedFrameEvent);
        }

        void PlayAnimEffect(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.PlayAnimEffectFrameEventData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnPlayAnimEffect(eventData as FrameEventInfo.PlayAnimEffectFrameEventData);
        }
        void SetAnimSpeed(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.SetAnimSpeedFrameEvent>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnSetAnimSpeedFrameEvent(eventData as FrameEventInfo.SetAnimSpeedFrameEvent);
        }
        void MoveToTarget(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.MoveToTargetFrameEvent>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnMoveToTargetFrameEvent(eventData as FrameEventInfo.MoveToTargetFrameEvent);
        }
        void CameraShake(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.CameraShakeFrameEvent>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnCameraShakeFrameEvent(eventData as FrameEventInfo.CameraShakeFrameEvent);
        }

        void SuperArmor(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.SuperArmorFrameEvent>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnSuperArmor(eventData as FrameEventInfo.SuperArmorFrameEvent);
        }
        void RoleSkillList(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.RoleSkillListFrameEvent>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnSkillList(eventData as FrameEventInfo.RoleSkillListFrameEvent);
        }
        void RoleAiFlyTo(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.RoleAiFlyToFrameEventData>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnRoleAiFlyTo(eventData as FrameEventInfo.RoleAiFlyToFrameEventData);
        }

        void RoleQTE(string eventString)
        {
            object eventData;
            if (!cache.TryGet(eventString.GetHashCode(), out eventData))
            {
                eventData = Tools.Json2Obj<FrameEventInfo.RoleQTE>(eventString, true);
                cache.Set(eventString.GetHashCode(), eventData);
            }
            animEventDelegate?.OnRoleQte(eventData as FrameEventInfo.RoleQTE);
        }

        void JObject(FrameEventData frameEventData)
        {
            var jObject = frameEventData.EventData;
            switch (frameEventData.name)
            {
                case "RoleAtk":
                    animEventDelegate?.OnAtk(jObject.ToObject<FrameEventInfo.RoleAtkFrameEventData>());
                    break;
                case "RoleSkill":
                    animEventDelegate?.OnSkill(jObject.ToObject<FrameEventInfo.RoleSkillFrameEventData>());
                    break;
                case "PlayAnim":
                    animEventDelegate?.OnPlayAnim(jObject.ToObject<FrameEventInfo.PlayAnimFrameEventData>());
                    break;
                case "Custom":
                    animEventDelegate?.OnCustom(jObject.ToObject<FrameEventInfo.CustomFrameEventData>());
                    break;
                case "VariableSpeed":
                    animEventDelegate?.OnVariableRoleMoveSpeed(jObject.ToObject<FrameEventInfo.VariableRoleMoveSpeedData>());
                    break;
                case "PlayAnimList":
                    animEventDelegate?.OnPlayAnimList(jObject.ToObject<FrameEventInfo.FrameEventAnimList>());
                    break;
                case "SetSpeed":
                    animEventDelegate?.OnSetSpeedFrameEvent(jObject.ToObject<FrameEventInfo.SetSpeedFrameEvent>());
                    break;
                case "PlayAnimEffect":
                    animEventDelegate?.OnPlayAnimEffect(jObject.ToObject<FrameEventInfo.PlayAnimEffectFrameEventData>());
                    break;
                case "SetAnimSpeed":
                    animEventDelegate?.OnSetAnimSpeedFrameEvent(jObject.ToObject<FrameEventInfo.SetAnimSpeedFrameEvent>());
                    break;
                case "MoveToTarget":
                    animEventDelegate?.OnMoveToTargetFrameEvent(jObject.ToObject<FrameEventInfo.MoveToTargetFrameEvent>());
                    break;
                case "CameraShake":
                    animEventDelegate?.OnCameraShakeFrameEvent(jObject.ToObject<FrameEventInfo.CameraShakeFrameEvent>());
                    break;
                case "SuperArmor":
                    animEventDelegate?.OnSuperArmor(jObject.ToObject<FrameEventInfo.SuperArmorFrameEvent>());
                    break;
                case "RoleSkillList":
                    animEventDelegate?.OnSkillList(jObject.ToObject<FrameEventInfo.RoleSkillListFrameEvent>());
                    break;
                case "RoleAiFlyTo":
                    animEventDelegate?.OnRoleAiFlyTo(jObject.ToObject<FrameEventInfo.RoleAiFlyToFrameEventData>());
                    break;
                case "AnimProperty":
                    animEventDelegate?.OnAnimProperty(jObject.ToObject<FrameEventInfo.AnimProperty>());
                    break;
                case "PlayAudio":
                    animEventDelegate?.OnPlayAudio(jObject.ToObject<FrameEventInfo.PlayAudio>());
                    break;
                case "OnTread":
                    animEventDelegate?.OnTread(jObject.ToObject<FrameEventInfo.OnTread>());
                    break;
                default:
                    animEventDelegate?.OnJObject(jObject);
                    break;
            }
        }
    }
}