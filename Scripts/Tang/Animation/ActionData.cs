using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace Tang.Anim
{
    public enum ActionType
    {
        // MoveTo = 1,
        MoveBy = 2,
        FadeTo = 3,
        // FadeBy = 4,
        Sequence = 5,
        Parallel = 6,

        ScaleTo = 7,

        MulColorTo = 8,
        AddColorTo = 9,
        AddForce = 10,
        AddTorque = 11,
        AddRotation = 12,
        Curve = 13,
        Path = 14,
    }
    
    public enum RandomType
    {
        Random = 1,
        Fix = 2,
    }

    [System.Serializable]
    public class ActionData
    {
        public ActionType actionType;

        public float duration;
        public float randomDurationFrom = 0;
        public float randomDurationTo = 0;

        public bool isNormalizedrandom = false;

        public Vector3 pos;
        public Vector3 rotation;

        public Vector3 randomPosFrom;
        public Vector3 randomPosTo;
        public Vector3 randomRotationFrom;
        public Vector3 randomRotationTo;
        public RotateMode rotateMode;

        public Vector3 Force;
        public Vector3 randomForceMin;
        public Vector3 randomForceMax;
        public ForceMode ForceMode;
        public RandomType RandomType;

        public Vector3 scale;
        public Vector3 randomScaleFrom;
        public Vector3 randomScaleTo;

        public float width;
        public float height;
        public float t;

        public PathType pathType;
        public PathMode pathMode;
        public int resolution = 10;

        public Vector4 color = Color.white;

        public Vector4 mulColor = Color.white;
        public Vector4 addColor = Color.black;

        public float alpha;

        public Ease ease = Ease.Linear;

        public List<ActionData> actionList = new List<ActionData>();

        public List<Vector3> vector3List = new List<Vector3>();
    }

}