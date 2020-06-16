using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Tang
{
    public enum QteState
    {
        QteBegin = 1,
        QteRunning = 2,
        QteSuccess = 3,
        QteFailure = 4
    }

    public interface IQte
    {
        event Action OnStart;
        event Action OnUpdate;
        event Action OnBreak;
        event Action OnEnd;
        
        void Init(RoleController a, RoleController b);
        void Begin();
        void Hit();
        void LastHit();
        void Success();
        void Failure();
    }

    public class RoleQTEDataAsset : ScriptableObject
    {
        public static RoleQTEDataAsset s_RoleQTEDataAsset;

        public static RoleQTEDataAsset Instance
        {
            get
            {
                if (s_RoleQTEDataAsset == null)
                {
                    GetAsset();
                    s_RoleQTEDataAsset.Init();
                }

                return s_RoleQTEDataAsset;
            }
        }

        private static async void GetAsset()
        {
            s_RoleQTEDataAsset = await AssetManager.LoadAssetAsync<RoleQTEDataAsset>("Manager/RoleQTEDataAsset.asset");
        }
        [Serializable]
        public class RoleQTE
        {
            public string RoleIdAId;
            public string RoleIdBId;
            public string QteId;
            public string ScriptId;
        }

        public List<RoleQTE> RoleQteList = new List<RoleQTE>();
        private Dictionary<string, RoleQTE> RoleQteDic;

        public void Init()
        {
            RoleQteDic = RoleQteList.ToDictionary(item => (item.RoleIdAId + item.RoleIdBId + item.QteId), item => item);
        }

        public IQte Qte(RoleController a, RoleController b, string qteId)
        {
            RoleQTE roleQte = GetQte(a.RoleData.Id, b.RoleData.Id, qteId);

            if (roleQte != null)
            {
                string typeName = "Tang." + roleQte.ScriptId;
                Type type = Type.GetType(typeName);
                IQte iQte = a.gameObject.AddComponent(type) as IQte;
                Debug.Assert(iQte != null);
                iQte.Init(a ,b);
                return iQte;
            }
            return null;
        }

        public RoleQTE GetQte(string roleAId, string RoleBId, string qteId)
        {
            string key = roleAId + RoleBId + qteId;

            RoleQTE roleQte;
            if (RoleQteDic.TryGetValue(key, out roleQte))
            {
                return roleQte;
            }
            Debug.LogError("找不到qte: " + key);
            return null;
        }
    }
}