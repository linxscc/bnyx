using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public enum ColliderType
    {
        weakness = 1,
        collider = 2,
        Trigger = 3,
    }
    public enum FollowType
    {
        bone = 1,
        slot = 2,
    }
    [System.Serializable]
    public class WeaknessslotData
    {
        public string slotname;
        public float z;
        public TriggerMode triggerMode;
        public string tag;
        public string layer;
        public bool isTrigger;
        public string TriggerId;
    }
    [System.Serializable]
    public class WeaknessData
    {
        public float WeaknessHP;
        public float WeaknessHPMax;
        public string WeaknessName;
        public bool BoneFollow = false;
        public string BoneName;
        public ColliderType colliderType = ColliderType.weakness;
        public string CollidertagName;
        public string ColliderlayerName;
        public List<string> ComponentPathList=new List<string>();
        public List<WeaknessslotData> slotNameList = new List<WeaknessslotData>();
        public FollowType followType = FollowType.bone;
        public Vector3 center = new Vector3();
        public Vector3 size = new Vector3();
    }
}

