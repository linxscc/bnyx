using System;
using System.Collections.Generic;
using UnityEngine;


namespace Tang
{
    public class buffIdData
    {
        public string id;
        public float probability;

    }
    public enum BuffEventType
    {
        OnBuffBegin = 1, // 增益开始 add by TangJian 2018/05/14 19:22:18
        OnBuffUpdate = 2, // 增益刷新 add by TangJian 2018/05/14 19:22:23
        OnBuffEnd = 3, // 增益结束 add by TangJian 2018/05/14 19:22:27

        OnAttack = 4, // 攻击时 add by TangJian 2018/05/14 19:23:12
        OnHurt = 5, // 受伤时 add by TangJian 2018/05/14 19:23:18
        OnHit = 6, // 击中 add by TangJian 2018/05/15 10:31:11
    }

    [System.Serializable]
    public class BuffController
    {
        public RoleBuffData roleBuffData = new RoleBuffData();

        // 其他的事件注册 add by TangJian 2018/05/14 19:20:53
        private Dictionary<BuffEventType, Func<BuffData, object[], bool>> buffEventDic = new Dictionary<BuffEventType, Func<BuffData, object[], bool>>();

        public void SetRoleBuffData(RoleBuffData roleBuffData)
        {
            this.roleBuffData = roleBuffData;
        }

        public void AddEquipBuff(string buffKey, string buffId)
        {
            BuffData buffData = roleBuffData.Add(buffKey, buffId);

            // 永久 add by TangJian 2017/11/20 22:08:11
            buffData.duration = -1;

            if (buffData != null)
            {
                TriggerBuffEvent(BuffEventType.OnBuffBegin, null);
            }
            else
            {
                Debug.LogError("没有buff:" + buffId);
            }
        }

        public void AddPermanentBuff(string key, BuffData buffData)
        {
            buffData = roleBuffData.Add(key, buffData);

            buffData.duration = -1; // 永久 add by TangJian 2018/12/12 14:13

            if (buffData != null)
            {
                TriggerBuffEvent(BuffEventType.OnBuffBegin, null);
            }
            else
            {
                Debug.LogError("没有buff:" + key);
            }
        }

        public void AddBuff(string buffId)
        {
            string buffKey = buffId;

            BuffData buffData = roleBuffData.Add(buffKey, buffId);
            if (buffData != null)
            {
                TriggerBuffEvent(BuffEventType.OnBuffBegin, null);
            }
            else
            {
                Debug.LogError("没有buff:" + buffId);
            }
        }

        public void RemoveBuff(string buffId)
        {
            roleBuffData.Remove(buffId);
        }

        public bool HasBuff(string buffId)
        {
            return roleBuffData.buffDataDic.ContainsKey(buffId);
        }

        // 注册事件 add by TangJian 2018/05/14 19:25:39
        public void RegisterBuffEvent(BuffEventType buffEventType, Func<BuffData, object[], bool> func)
        {
            if (buffEventDic.ContainsKey(buffEventType))
            {
                buffEventDic.Remove(buffEventType);
            }
            buffEventDic.Add(buffEventType, func);
        }

        // 触发事件 add by TangJian 2018/05/14 19:27:44
        public void TriggerBuffEvent(BuffEventType buffEventType, params object[] objects)
        {
            Func<BuffData, object[], bool> func;
            if (buffEventDic.TryGetValue(buffEventType, out func))
            {
                foreach (var item in roleBuffData.buffDataDic)
                {
                    func(item.Value, objects);
                }
            }
        }

        List<string> needRemoveIdList = new List<string>();
        public void Update(float deltaTime)
        {
            needRemoveIdList.Clear();

            foreach (var item in roleBuffData.buffDataDic)
            {
                var buff = item.Value;

                if (buff.time >= (buff.updateTimes + 1) * buff.updateInterval)
                {
                    buff.updateTimes++;
                    TriggerBuffEvent(BuffEventType.OnBuffUpdate, null);
                }

                buff.time += deltaTime;

                if (buff.duration >= 0 && buff.time >= buff.duration)
                {
                    TriggerBuffEvent(BuffEventType.OnBuffEnd, null);
                    needRemoveIdList.Add(buff.key);
                }
            }

            for (int i = needRemoveIdList.Count - 1; i >= 0; i--)
            {
                RemoveBuff(needRemoveIdList[i]);
            }
        }
    }
}