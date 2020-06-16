using Spine.Unity;
using UnityEngine;

namespace Tang
{
    public interface IObjectController
    {
        Transform transform { get; }

        // 是否处于防守状态 add by TangJian 2019/5/10 16:17
        bool IsDefending { get; }
        
        // BOSS特殊霸体状态击退数值
        bool CanRebound { get; }
        
        // 普通状态击退抗性数值 add by TangJian 2019/5/10 16:18
        float RepellingResistance { get;}

        // 防守状态的击退抗性数值 add by TangJian 2019/5/10 16:18
        float DefenseRepellingResistance { get;}

        int GetDirectionInt();
        Vector3 Speed { get; set; }

        Animator MainAnimator { get; }
        SkeletonRenderer MainSkeletonRenderer { get; }

        float DefaultAnimSpeed { get; }

        float FrontZ { get; }
        float BackZ { get; }

        bool IsGrounded();
    }
}