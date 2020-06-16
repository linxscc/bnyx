using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace Tang
{
    public class BuffManager : MyMonoBehaviour
    {
        private static BuffManager instance;
        public static BuffManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<BuffManager>();
                }
                return instance;
            }
        }

        private string buffMapFileName;
        public string BuffMapFileName { get { return buffMapFileName; } set { buffMapFileName = value; } }
        public List<BuffData> buffDataList = new List<BuffData>();
        public Dictionary<string, BuffData> buffDataDict = new Dictionary<string, BuffData>();

        void Start()
        {
            loadBuffData();
        }

        public async void loadBuffData()
        {
            string jsonString = (await AssetManager.LoadAssetAsync<TextAsset>("BuffMap")).text;
            buffDataDict = Tools.Json2Obj<Dictionary<string, BuffData>>(jsonString);
            buffDataList = buffDataDict.Values.ToList();
        }

        public BuffData GetBuffData(string buffId)
        {
            BuffData buffData;
            if (buffDataDict.TryGetValue(buffId, out buffData))
            {
                buffData = buffData.Clone();

                return buffData.Clone();
            }
            return null;
        }
    }
}