using System;
using System.Collections.Generic;

namespace Tang
{
    [Serializable]
    public class BuffData
    {
        public string id; // buff id add by TangJian 2017/09/30 15:58:10
        public string name; // buff名 add by TangJian 2017/09/30 15:58:04
        public string icon; // 图标 add by TangJian 2017/11/18 18:50:46
        public int level = 1; // 等级, 层数 add by TangJian 2017/09/30 15:57:58

        public float duration; // 持续时间 add by TangJian 2017/09/30 15:58:20
        public float updateInterval; // 刷新间隔 add by TangJian 2017/10/09 20:42:09

        public AttrData attrData = new AttrData();

        public List<ActionData> buffBeginActions = new List<ActionData>();
        public List<ActionData> buffUpdateActions = new List<ActionData>();
        public List<ActionData> buffEndActions = new List<ActionData>();

        public Dictionary<BuffEventType, List<ActionData>> buffEvents = new Dictionary<BuffEventType, List<ActionData>>();

        public float beginTime; // 开始时间 add by TangJian 2017/09/30 15:58:17
        public float time = 0; // 从生效开始经过的时间 add by TangJian 2017/10/10 15:25:26
        public float updateTimes = 0; // 刷新的次数 add by TangJian 2017/10/10 15:25:44
        public string key; // key add by TangJian 2017/11/20 21:57:48

        public BuffData Clone()
        {
            return Tools.DepthClone(this);
        }
    }
}