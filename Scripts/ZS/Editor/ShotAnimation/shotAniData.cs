using UnityEngine;
using DG.Tweening;


namespace ZS
{
    public class shotAniData : ScriptableObject
    {
        public shotAniData(string name, Vector3 V1, Vector3 V2, Ease ea, float timer)
        {
            Name = name;
            StartPos = V1;
            EndPos = V2;
            ease = ea;
            ExecutionTime = timer;
        }
        public string Name;
        public Vector3 StartPos;
        public Vector3 EndPos;
        public Ease ease;
        public float ExecutionTime;

    }
   

   


}

