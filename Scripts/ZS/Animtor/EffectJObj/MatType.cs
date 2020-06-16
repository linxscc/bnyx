using System.Collections;
using System.Collections.Generic;
using Tang;
using UnityEngine;

public enum MatType 
{
    Wood = 0,
    Rock = 1,
    Skull = 2,
    Body = 3,
    Iron = 4,
    Other = 5,
    
    wood_side =6,
    wood_mid = 7,
    wood_ground=8,
    wood_stairs=9,
    rock_side=10,
    rock_mid=11,
    rock_ground=12,
    rock_stairs=13,
    iron_side=14,
    iron_mid=15,
    iron_ground=16,
    iron_stairs=17,
    other_side=18,
    other_mid=19,
    other_ground=20,
    other_stairs=21,
    
    water_mid=22,
    water_side=23,
    water_ground=24

    
}

public enum HitEffectType
{
    Other = 0,
    swd = 1,
    lswd = 2,
    bow = 3,
    staves = 4,
    hammer = 5
}

public enum EffectShowMode
{
    ColliderPoint = 0, // 特效显示在碰撞点位置 add by TangJian 2019/5/11 16:04
    FrontOrBack = 1 // 特效显示在物体前面或者后面 add by TangJian 2019/5/11 16:04
}

public interface HitAble
{
    // 攻击类型
    HitEffectType GetHitEffectType();
    
    bool OnHit(DamageData damageData);
}

public interface HurtAble
{
    // 受击类型
    MatType GetMatType();
    
    // 伤害显示位置模式
    EffectShowMode EffectShowMode { get; }
    
    // 获得打击点
    bool TryGetHitPos(out Vector3 hitPos);
    
    // 阻尼 add by TangJian 2019/5/13 10:55
    float Damping { get; }
    
    bool OnHurt(DamageData damageData);
}

public interface IHitAndMat : HitAble, HurtAble
{
    string TeamId { get; }
    
    GameObject gameObject { get; }
    Transform transform { get; }
}