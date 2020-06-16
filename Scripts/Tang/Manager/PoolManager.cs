using UnityEngine;
using PathologicalGames;

namespace Tang
{
    public class PoolManager : MyMonoBehaviour
    {
        private static PoolManager instance;
        public static PoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<PoolManager>();
                }
                return instance;
            }
        }

        SpawnPool pool;
        SpawnPool Pool
        {
            get
            {
                if (pool == null)
                {
                    pool = PathologicalGames.PoolManager.Pools.Create("MainPool");
                    // pool = gameObject.GetComponent<SpawnPool>();
                }
                return pool;
            }
        }

        public void AddPrefab(GameObject gameObject)
        {
            if (ContainKey(gameObject.name))
                return;
            
            if (Pool.GetPrefabPool(gameObject) == null)
            {
                PrefabPool prefabPool = new PrefabPool(gameObject.transform);
                //缓存池这个Prefab的最大保存数量
                prefabPool.preloadAmount = 40;
                prefabPool.preloadTime = true;
                prefabPool.preloadFrames = 1;
                

                //是否开启缓存池智能自动清理模式
                prefabPool.cullDespawned = true;
                //缓存池自动清理，但是始终保留几个对象不清理
                prefabPool.cullAbove = 10;

                //每过多久执行一遍自动清理(销毁)，单位是秒
                prefabPool.cullDelay = 5;
                //每次自动清理 个游戏对象
                prefabPool.cullMaxPerPass = 10;
                //是否开启实例的限制功能
                prefabPool.limitInstances = true;
                //限制缓存池里最大的Prefab的数量，它和上面的preloadAmount是有冲突的，如果同时开启则以limitAmout为准
                prefabPool.limitAmount = 40;
                //如果我们限制了缓存池里面只能有10个Prefab，如果不勾选它，那么你拿第11个的时候就会返回null。如果勾选它在取第11个的时候他会返回给你前10个里最不常用的那个
                prefabPool.limitFIFO = true;

                prefabPool.limitInstances = true;

//                Pool.CreatePrefabPool(prefabPool);
                Pool._perPrefabPoolOptions.Add(prefabPool);
                Pool.CreatePrefabPool(prefabPool);
            }
        }

        public bool ContainKey(string key)
        {
            return Pool.prefabPools.ContainsKey(key);
        }

        public GameObject Spawn(string name)
        {
            if (ContainKey(name) == false)
            {
                //return null;
//                var prefab = Resources.Load<GameObject>(name);
                var prefab = GameObjectManager.Instance.GetPrefab(name);
                while (!prefab.IsCompleted)
                {
                    
                }
                var go = prefab.Result;
                go.name = name;
                AddPrefab(go);
            }
            var trans = Pool.Spawn(name);
            if (trans != null)
            {
                return trans.gameObject;
            }
            return null;
        }

        public GameObject Spawn(string name, GameObject gameObject)
        {
            if (Pool.prefabPools.ContainsKey(name) == false)
            {
                //return null;
                var prefab = gameObject;
                prefab.name = name;
                AddPrefab(prefab);
            }
            var trans = Pool.Spawn(name);
            if (trans != null)
            {
                return trans.gameObject;
            }
            return null;
        }


        public void Despawn(GameObject gameObject, float delayTime = 0)
        {
            if (delayTime <= 0)
            {
                gameObject.transform.parent = pool.transform;
                this.Pool.Despawn(gameObject.transform);
            }
            else
            {
                DelayFunc("Despawn: " + gameObject.GetInstanceID(), (System.Action)(() =>
                {
                    gameObject.transform.parent = pool.transform;
                    this.Pool.Despawn(gameObject.transform);
                }), delayTime);
            }

        }
    }

    // public class SpawnPool
    // {
    //     Dictionary<string, GameObject> m_gameObjects;
    //     List<GameObject> m_gameObjectList;

    //     private int m_maxCount = 5;

    //     public GameObject getGameObject(string name)
    //     {
    //         GameObject gameObject = null;
    //         if (m_gameObjectList.Count > 0)
    //         {
    //             gameObject = m_gameObjectList[m_gameObjectList.Count - 1];
    //         }
    //         return gameObject;
    //     }

    //     public void addGameObject(GameObject gameObject)
    //     {
    //         m_gameObjectList.Insert(0, gameObject);
    //         trim();
    //     }

    //     private void trim()
    //     {
    //         if (m_gameObjectList.Count > m_maxCount)
    //         {
    //             for (int i = 0; i < m_gameObjectList.Count - m_maxCount; i++)
    //             {
    //                 GameObject gameObject = m_gameObjectList[m_gameObjectList.Count - 1];
    //                 m_gameObjectList.RemoveAt(m_gameObjectList.Count - 1);
    //                 GameObject.Destroy(gameObject);
    //             }
    //         }
    //     }
    // }

    // public class PoolManager : MonoBehaviour
    // {
    //     Dictionary<string, string> m_keyTranslators;

    //     Dictionary<string, SpawnPool> m_spawnPools;

    //     public GameObject create(string name)
    //     {
    //         return getGameObject(name);
    //     }

    //     public void remove(GameObject GameObject)
    //     {
    //         addGameObject(GameObject);
    //     }

    //     private GameObject getGameObject(string name)
    //     {
    //         SpawnPool spawnPool = null;
    //         GameObject gameObject = null;
    //         if (m_spawnPools.TryGetValue(name, out spawnPool))
    //         {
    //             gameObject = spawnPool.getGameObject(name);
    //         }
    //         else
    //         {
    //             gameObject = Resources.Load<GameObject>(name);
    //             m_keyTranslators.Add(gameObject.GetType().ToString(), name);
    //         }
    //         return gameObject;
    //     }

    //     private void addGameObject(GameObject gameObject)
    //     {
    //         string key = null;
    //         if (m_keyTranslators.TryGetValue(gameObject.GetType().ToString(), out key))
    //         {
    //             m_keyTranslators.TryGetValue(gameObject.GetType().ToString(), out key);
    //         }
    //         else
    //         {
    //             Debug.Log("对象没有缓存: " + gameObject.GetType().ToString());
    //             return;
    //         }

    //         SpawnPool spawnPool = null;
    //         if (m_spawnPools.TryGetValue(name, out spawnPool))
    //         {
    //             spawnPool.addGameObject(gameObject);
    //         }
    //         else
    //         {

    //         }
    //     }
    // }
}