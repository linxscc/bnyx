using System.Collections.Generic;
using UnityEngine;
//using System.Data;
using System;

namespace Tang
{
    public class RoleUpgradeDataAsset : ScriptableObject
    {
        public static RoleUpgradeDataAsset s_RoleUpgradeDataAsset;

        public static RoleUpgradeDataAsset Instance
        {
            get
            {
                if (s_RoleUpgradeDataAsset == null)
                {
                   
                }
                return s_RoleUpgradeDataAsset;
            }
        }
//        [RuntimeInitializeOnLoadMethod]
        public static async void GetAsset()
        {
            s_RoleUpgradeDataAsset = await AssetManager.LoadAssetAsync<RoleUpgradeDataAsset>("Manager/RoleUpgradeDataAsset.asset");
        }
        
        [System.Serializable]
        public class LevelAndExp
        {
            public int level = 1;
            public int exp = 0;

            public LevelAndExp(int level, int exp)
            {
                this.level = level;
                this.exp = exp;
            }
        }

        public List<LevelAndExp> levelAndExpList = new List<LevelAndExp>();

        private Cache _expToLevelCache = new Cache();
        private Cache _levelToExpCache = new Cache();
        
        public void Clear()
        {
            levelAndExpList.Clear();
            _expToLevelCache.Clear();
            _levelToExpCache.Clear();
        }

        public void AddItem(int lv, int exp)
        {
            levelAndExpList.Add(new LevelAndExp(lv, exp));
        }

        public int GetLevel(int exp)
        {
            object level;
            if (_expToLevelCache.TryGet(exp, out level))
            {
                Debug.Assert(level != null);
                return Convert.ToInt32(level);
            }
            else
            {
                for (int i = levelAndExpList.Count - 1; i < levelAndExpList.Count; i--)
                {
                    var levelAndExp = levelAndExpList[i];
                    if (exp >= levelAndExp.exp)
                    {
                        _expToLevelCache.Set(exp, levelAndExp.level);
                        return levelAndExp.level;
                    }
                }
            }
            
            return 1;
        }

        public int GetExp(int level)
        {
            object exp;
            if (_levelToExpCache.TryGet(level, out exp))
            {
                Debug.Assert(exp != null);
                return Convert.ToInt32(exp);
            }
            else
            {
                LevelAndExp levelAndExp = levelAndExpList.Find((LevelAndExp _levelAndExp) =>
                {
                    if (_levelAndExp.level == level)
                    {
                        return true;
                    }

                    return false;
                });

                if (levelAndExp != null)
                {
                    _levelToExpCache.Set(level, levelAndExp.exp);
                    return levelAndExp.exp;
                }
                else
                {
                    _levelToExpCache.Set(level, 9999999);
                    return 9999999;
                }
            }
            return 19999999;
        }

        public int GetLevelMax()
        {
            return levelAndExpList[levelAndExpList.Count - 1].level;
        }
        
        public int GetLevelMaxExp()
        {
            return levelAndExpList[levelAndExpList.Count - 1].exp;
        }
    }
}