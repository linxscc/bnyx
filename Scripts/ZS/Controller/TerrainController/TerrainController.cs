using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using Tang;
using Event = Tang.Event;

public class TerrainController : SceneObjectController, ITriggerDelegate, IHitAndMat, IObjectController
{

    public MatType matType = MatType.rock_side;
    public override void Start()
    {
        base.Start();
        HitAndHurtDelegate = new HitAndHurtController(this);
    }
    public virtual bool OnEvent(Event evt) 
    { return true; }

    public virtual GameObject GetGameObject()
    { return gameObject;}
    
    public void OnTriggerIn(TriggerEvent evt)
    {
        
    }

    public void OnTriggerOut(TriggerEvent evt)
    {
        
    }

    public void OnTriggerKeep(TriggerEvent evt)
    {
        
    }

    public HitEffectType GetHitEffectType()
    {
        return HitEffectType.Other;
    }

    public virtual MatType GetMatType()
    {
        return matType;
    }

    public EffectShowMode EffectShowMode => EffectShowMode.FrontOrBack;

    public bool TryGetHitPos(out Vector3 hitPos)
    {
        hitPos = Vector3.zero;
        return true;
    }

    public float Damping => 1;

    public IHitAndHurtDelegate HitAndHurtDelegate { get; private set; }

    public virtual bool OnHit(DamageData damageData)
    {
        HitAndHurtDelegate.Hit(damageData);
        return true;
    }

    public virtual bool OnHurt(DamageData damageData)
    {
        HitAndHurtDelegate.Hurt(damageData);
        return true;
    }

    public string TeamId { get; }
    public bool IsDefending => false;
    public bool CanRebound { get; }

    [field: SerializeField]
    public float RepellingResistance { get; set; } = 1;

    public float DefenseRepellingResistance => RepellingResistance;
    public int GetDirectionInt()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 Speed { get; set; }
    public Animator MainAnimator { get; }
    public SkeletonRenderer MainSkeletonRenderer { get; }
    public float DefaultAnimSpeed { get; }
    public float FrontZ { get; }
    public float BackZ { get; }
    public bool IsGrounded()
    {
        return true;
    }
}
