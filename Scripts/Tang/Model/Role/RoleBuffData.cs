using System;
using System.Collections.Generic;
using UnityEngine;


namespace Tang
{
    [Serializable]
    public class RoleBuffData
    {
        public Dictionary<string, BuffData> buffDataDic = new Dictionary<string, BuffData>();

        public BuffData Add(string key, string buffId)
        {
            BuffData buffData = Get(key);
            if (buffData != null)
            {
                buffData.level += 1;
            }
            else
            {
                buffData = BuffManager.Instance.GetBuffData(buffId);
                Debug.Assert(buffData != null);
                Add(key, buffData);
            }
            return buffData;
        }

        public BuffData Add(string key, BuffData buffData)
        {
            BuffData oldBuffData = Get(key);
            if (oldBuffData != null)
            {
                oldBuffData.level += 1;
            }
            else
            {
                Debug.Assert(buffData != null);

                buffData.level = 1;
                buffData.key = key;
                buffDataDic.Add(key, buffData);
            }

            return buffData;
        }

        public void Remove(string key)
        {
            if (buffDataDic.ContainsKey(key))
            {
                buffDataDic.Remove(key);
            }
        }

        public BuffData Get(string key)
        {
            BuffData buffData;
            if (buffDataDic.TryGetValue(key, out buffData))
            {
                return buffData;
            }
            return null;
        }

        public bool HasBuff(string key)
        {
            return buffDataDic.ContainsKey(key);
        }
    }
}