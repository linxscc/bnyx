using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tang;
using Tang.FrameEvent;

public class WallController : TerrainController
{
    
    public override bool OnHurt(DamageData damageData)
    {
        HitAndHurtDelegate.Hurt(damageData);
        return true;
    }
    
}
 