using System;
using UnityEngine;

namespace Tang
{
    public enum CoroutineStatus
    {
        Inactive = 0,
        Failure = 1,
        Success = 2,
        Running = 3
    }
    
    // 决策层 add by TangJian 2018/01/10 23:40:56
    [System.Serializable]
    public class RoleAIAction
    {
        public RoleAIAction()
        {
        }

        public RoleAIAction(string name, string actionId, float interval, float duration, Bounds bounds)
        {
            this.name = name;
            this.actionId = actionId;
            this.interval = interval;
            this.duration = duration;
            this.bounds = bounds;
        }
        [SerializeField] public float usetime;//上一次使用时间

        [SerializeField] public int weight = 1;//权重
        [SerializeField] public string name; // 名字 add by TangJian 2017/11/06 20:14:57
        [SerializeField] public string actionId; // 行为id add by TangJian 2018/01/15 16:16:47
        [SerializeField] public float interval; // 间隔时间 add by TangJian 2017/11/06 20:14:53
        [SerializeField] public float duration; // 持续时间 add by TangJian 2017/11/06 20:15:35
        [SerializeField] public Bounds bounds;
        
        [NonSerialized] public Color Color;
    }
    
}