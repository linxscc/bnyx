//#define POOL_ENABLE

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR

#endif




namespace Tang
{
    public class GameObjectManager : MonoBehaviour
    {
        [System.Serializable]
        public class Prefab
        {
            public Prefab(string key, GameObject prefab)
            {
                this.key = key;
                this.prefab = prefab;
            }

            public string key;
            public GameObject prefab;
        }

        [System.Serializable]
        public class PrefabPath
        {
            public PrefabPath(string key, string path)
            {
                this.key = key;
                this.path = path;
            }

            public string key;
            public string path; 
        }

        private static GameObjectManager instance;
        public static GameObjectManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<GameObjectManager>();
                }
                return instance;
            }
        }

        [HideInInspector] public Dictionary<string, GameObject> prefabDic = new Dictionary<string, GameObject>();
        public Dictionary<string, string> prefabPathDic = new Dictionary<string, string>();
        public List<Prefab> prefabs = new List<Prefab>();
        public List<PrefabPath> PrefabPathList = new List<PrefabPath>();
        
        private void OnEnable()
        {

        }

        private void Awake()
        {
            foreach (var item in PrefabPathList)
            {
                string itemKey = item.key.ToLower();
                if (prefabPathDic.ContainsKey(itemKey))
                {
                    Debug.Log("预制体重名：" + itemKey);
                }
                else
                {
                    prefabPathDic.Add(item.key.ToLower(), item.path);
                }
            }
        }

        void Start()
        {
#if POOL_ENABLE
            foreach (var item in prefabDic)
            {
                PoolManager.Instance.AddPrefab(item.Value);
            }
#endif
        }

        // 
        public Task<GameObject>   GetPrefab(string name)
        {
            name = name.ToLower(); // 转为小写 add by TangJian 2018/11/29 23:19

            string prefabPath;

            if (prefabPathDic.TryGetValue(name, out prefabPath))
            {
                return  AssetManager.LoadAssetAsync<GameObject>(prefabPath);
            }

            Debug.LogError("找不到预制体: " + name);
            return null;
        }
        public GameObject Create(string name, GameObject gameObject = null)
        {
            name = name.ToLower(); // 转为小写 add by TangJian 2018/11/29 23:19

            GameObject retGo;

            if (gameObject != null)
            {
                retGo = Instantiate(gameObject);
            }
            else
            {
                var go =  GetPrefab(name);
                while (!go.IsCompleted)
                {
                    
                }
                retGo = Instantiate( go.Result);
            }
            retGo.name = retGo.name.Replace("(Clone)", "");
            return retGo;
        }
        
        public async Task<GameObject> SpawnAsync(string name)
        {
#if POOL_ENABLE
            return PoolManager.Instance.Spawn(name);
#else
            return await AssetManager.Instantiate(name);
#endif
        }
        
        public GameObject Spawn(string name)
        {
            name = name.ToLower(); // 转为小写 add by TangJian 2018/11/29 23:19

#if POOL_ENABLE
            return PoolManager.Instance.Spawn(name);
#else
            return Create(name);
#endif
        }

        public GameObject Spawn(string name, GameObject gameObject)
        {
            name = name.ToLower(); // 转为小写 add by TangJian 2018/11/29 23:19

#if POOL_ENABLE
            return PoolManager.Instance.Spawn(name, gameObject);
#else
            return Create(name, gameObject);
#endif
        }

        public void Despawn(GameObject gameObject, float delayTime = 0)
        {
#if POOL_ENABLE
            PoolManager.Instance.Despawn(gameObject, delayTime);
#else
            Tools.Destroy(gameObject);
#endif
        }
    }
}