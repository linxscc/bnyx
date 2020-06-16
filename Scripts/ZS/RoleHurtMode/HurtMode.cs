using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZS
{
    [Serializable]
    public class HurtMode
    {
        public string Name;
        public List<HurtPart> HurtPartList = new List<HurtPart>();
    }
}


