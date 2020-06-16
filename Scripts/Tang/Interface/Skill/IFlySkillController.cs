

using UnityEngine;


namespace Tang
{
    public interface IFlySkillController
    {
        Vector3 Speed { set; get; } // 设置飞行技能速度 add by TangJian 2018/04/13 15:53:39
        bool GravityEnabled { set; get; } // 设置重力是否可用 add by TangJian 2018/04/13 15:53:41
        DamageData DamageData { set; get; } // 设置伤害 add by TangJian 2018/04/13 15:54:52
    }
}