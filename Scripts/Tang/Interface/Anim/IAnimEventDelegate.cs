using Newtonsoft.Json.Linq;

namespace Tang
{
    using FrameEvent;
    interface IAnimEventDelegate
    {
        void OnAtk(FrameEventInfo.RoleAtkFrameEventData roleAtkFrameEventData);
        void OnSkill(FrameEventInfo.RoleSkillFrameEventData roleSkillFrameEventData);
        void OnPlayAnim(FrameEventInfo.PlayAnimFrameEventData playAnimFrameEventData);
        void OnCustom(FrameEventInfo.CustomFrameEventData customFrameEventData);
        void OnVariableRoleMoveSpeed(FrameEventInfo.VariableRoleMoveSpeedData variableRoleMoveSpeedData);
        void OnPlayAnimList(FrameEventInfo.FrameEventAnimList frameEventAnimList);
        void OnSetSpeedFrameEvent(FrameEventInfo.SetSpeedFrameEvent setSpeedFrameEvent);
        void OnPlayAnimEffect(FrameEventInfo.PlayAnimEffectFrameEventData playAnimFrameEventData);
        void OnSetAnimSpeedFrameEvent(FrameEventInfo.SetAnimSpeedFrameEvent setAnimSpeedFrameEvent);
        void OnMoveToTargetFrameEvent(FrameEventInfo.MoveToTargetFrameEvent moveToTargetFrameEvent);
        void OnCameraShakeFrameEvent(FrameEventInfo.CameraShakeFrameEvent cameraShakeFrameEvent);
        void OnSuperArmor(FrameEventInfo.SuperArmorFrameEvent superArmorFrameEvent);
        void OnSkillList(FrameEventInfo.RoleSkillListFrameEvent roleSkillListFrameEvent);
        void OnRoleAiFlyTo(FrameEventInfo.RoleAiFlyToFrameEventData roleAiFlyToFrameEventData);
        void OnRoleQte(FrameEventInfo.RoleQTE roleQte);
        void OnAnimProperty(FrameEventInfo.AnimProperty animProperty);

        void OnPlayAudio(FrameEventInfo.PlayAudio playAudio);

        void OnJObject(JObject jObject);

        void OnTread(FrameEventInfo.OnTread tread);
    }
}