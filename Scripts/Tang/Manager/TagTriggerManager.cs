using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class TagTriggerManager : MyMonoBehaviour
    {
        private static TagTriggerManager instance;
        public static TagTriggerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<TagTriggerManager>();
                }
                return instance;
            }
        }

        private string buffMapFileName;
        //public string BuffMapFileName { get { return buffMapFileName; } set { buffMapFileName = value; } }
        //public List<BuffData> buffDataList = new List<BuffData>();
        //public Dictionary<string, BuffData> buffDataDict = new Dictionary<string, BuffData>();
        public string TagTirggerListPath = "Resources_moved/Scripts/Tagtrigger/TagTirggerList.json";
        public string path = "Resources_moved/Scripts/Tagtrigger/";
        string TagTirggerListDataPath { get { return "Manager/TagTriggerListData.asset"; } }
        bool[,] TagTirggerList;
        List<string> taglist = new List<string>();
        public TagTriggerListData tagTriggerListData;
        void OnEnable()
        {
            //loadBuffData();
            //loadTagTriggerList();
        }
        public void Start()
        {
            Debug.Log("tagTriggerListData = " + tagTriggerListData);

            loadTagTriggerList();
        }
        public async void loadTagTriggerList()
        {
            tagTriggerListData = await AssetManager.LoadAssetAsync<TagTriggerListData>(TagTirggerListDataPath);
            if (tagTriggerListData != null&& tagTriggerListData.TagList != null&&tagTriggerListData.tagTriggerBoolList!=null)
            {
                int co=tagTriggerListData.TagList.Count;
                TagTirggerList = new bool[co, co];
                for (int t=0;t< tagTriggerListData.tagTriggerBoolList.Count;t++)
                {
                    for (int xt=0;xt< tagTriggerListData.tagTriggerBoolList[t].list.Count; xt++)
                    {
                        TagTirggerList[t, xt] = tagTriggerListData.tagTriggerBoolList[t].list[xt];
                    }
                }
                taglist = tagTriggerListData.TagList;
            }
            else
            {
                string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + TagTirggerListPath);
                TagTirggerList = Tools.Json2Obj<bool[,]>(jsonString);
                string jsonlist = Tools.ReadStringFromFile(Application.dataPath + "/" + path + "TagList.json");
                taglist = Tools.Json2Obj<List<string>>(jsonlist);
            }
            
        }
        public bool FindTirggerBool(string stringa,string stringb)
        {
            if (taglist.Contains(stringa) && taglist.Contains(stringb))
            {
                int inta = taglist.IndexOf(stringa);
                int intb = taglist.IndexOf(stringb);
                return TagTirggerList[inta, intb];
            }
            else
            {
                return false;
            }
        }
    }
    //[System.Serializable]
    //public class TagTriggerListData : ScriptableObject
    //{
    //    public List<string> TagList;
    //    public List<TagTriggerBoolList> tagTriggerBoolList=new List<TagTriggerBoolList>();
    //}
    [System.Serializable]
    public class TagTriggerBoolList
    {
        public List<bool> list = new List<bool>();
    }
}