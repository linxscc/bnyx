
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZS
{
    [Serializable]
    public class HurtPart
    {
        public string Name;
        public Bounds Bounds;
        public MatType MatType;
        public float HurtRatio; 
        public bool IsFollowSlot;
        public string FollowSlotName;
    }
}
