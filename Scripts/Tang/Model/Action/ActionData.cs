using System;
using UnityEngine;

namespace Tang
{
    [Serializable]
    public class ActionData : ICloneable
    {
        public ActionType type;
        public int chance = 100;
        public string string1;
        public string string2;
        public string string3;
        public int int1;
        public int int2;
        public int int3;
        public float float1;
        public float float2;
        public float float3;
        public bool bool1;
        public bool bool2;
        public bool bool3;

        public Vector3 vector3_1;
        public Vector3 vector3_2;
        public Vector3 vector3_3;

        public object Clone()
        {
            ActionData ret = new ActionData();
            ret.type = type;
            ret.chance =chance;
            
            ret.string1 = string1;
            ret.string2 = string2;
            ret.string3 = string3;

            ret.int1 = int1;
            ret.int2 = int2;
            ret.int3 = int3;

            ret.float1 = float1;
            ret.float2 = float2;
            ret.float3 = float3;

            ret.bool1 = bool1;
            ret.bool2 = bool2;
            ret.bool3 = bool3;

            ret.vector3_1 = vector3_1;
            ret.vector3_2 = vector3_2;
            ret.vector3_3 = vector3_3;
            return ret;
        }
    }
}