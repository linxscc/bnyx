using System.Collections;
using System.Collections.Generic;
using Tang;
using UnityEngine;

public class RoleInteraction
{
    public string hitType;
    public string bodyType;
    public string hurtEffect;
}

namespace ZS
{
    public class HitAndMatData
    {
        public string HitEffectId;
        public string HurtEffectId;
        
        public HitAndMatData(string hitEffectId, string hurtEffectId)
        {
            this.HitEffectId = hitEffectId;
            this.HurtEffectId = hurtEffectId;
        }
    }

    public class GroundEffect
    {
        public string groundEffect;

        public GroundEffect(string groundEffect)
        {
            this.groundEffect = groundEffect;
        }
    }

    public class EffectAnim
    {
        public string id;
        public string AnimName;
        public string Anim1;
        public string Anim2;
        public string Anim3;
        public string Anim4;
        public string Anim5;
        public modeType mode;
        public AnimType _AnimType;
    }

    public enum modeType
    {
        random = 0
    }

    public enum AnimType
    {
        None=0,
        spine=1,
        effect
    }
    
   
}
