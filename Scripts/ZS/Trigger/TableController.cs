using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Tang;
using UnityEngine;
using Event = Tang.Event;
using EventType = Tang.EventType;

public class TableController : PlacementController
{
    public override EffectShowMode EffectShowMode => EffectShowMode.ColliderPoint;

    public override bool OnHurt(DamageData damageData)
    {
        if (MainAnimator!=null)
        {
            MainAnimator.SetInteger("state",1);
        }
        AnimManager.Instance.PlayAnimEffect(Placementdata.deathEffect,gameObject.GetRendererBounds().center);
        HitAndHurtDelegate.Hurt(damageData);
        return true;
    }
}