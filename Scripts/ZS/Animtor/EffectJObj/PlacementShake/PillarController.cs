using System.Collections;
using System.Collections.Generic;
using Tang;
using UnityEngine;
using ZS;

public class PillarController : PlacementController
{
    GameObject gameObject;
    GameObject hitObject;

    public override void Start()
    {
        base.Start();
        gameObject = GameObject.Find("Bos001");
    }

    public GameObject HitObject
    {
        get => hitObject;
        set => hitObject = value;
    }
    
    public override bool OnHurt(DamageData damageData)
    {
        if (damageData.owner.name == "Bos001(Clone)")
        {
            Hurt();   
        }
        return true;
    }

    private void Hurt()
    {
        if (MainAnimator)
        { 
            MainAnimator.SetInteger("state",1);
        }
    }
}
