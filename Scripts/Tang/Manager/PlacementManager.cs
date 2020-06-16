using System.Collections.Generic;
using UnityEngine;


namespace Tang
{
    public class PlacementManager : MonoBehaviour
    {
        private static PlacementManager instance;
        public static PlacementManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<PlacementManager>();
                }
                return instance;
            }
        }
        
        public Dictionary<string, tian.PlacementData> roleDataDict = new Dictionary<string, tian.PlacementData>();

        void Awake()
        {
            loadPlacementData();
        }

        public async void loadPlacementData()
        {
            roleDataDict = Tools.Json2Obj<Dictionary<string, tian.PlacementData>>((await AssetManager.LoadAssetAsync<TextAsset>("PlacementDatas")).text);
        }

        public tian.PlacementData getPlacementDataById(string id)
        {
            if (roleDataDict.ContainsKey(id))
            {
                var data = roleDataDict[id];
                return Tools.DepthClone(data);
            }
            return null;
        }
    }

}