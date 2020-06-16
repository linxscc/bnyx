using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using System.Threading.Tasks;

namespace Tang
{
    public partial class SceneManager : MyMonoBehaviour
    {
        private static SceneManager instance;
        public static SceneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<SceneManager>();
                }
                return instance;
            }
        }

        // 已经载入的场景 add by TangJian 2018/12/28 17:46
        private Dictionary<string, SceneController> scenes = new Dictionary<string, SceneController>();

        // 当前场景 add by TangJian 2018/12/28 17:46
        private SceneController currScene;
        public SceneController CurrScene { get { return currScene; } }

        // 当前场景Id add by TangJian 2018/12/28 17:46
        public string CurrSceneId { get { return currScene != null ? currScene.name : null; } }

        // 获得一个唯一的场景位置 add by TangJian 2017/08/03 22:40:38
        Vector3 onlyScenePosition = new Vector3();
        public Vector3 GetOnlyScenePosition()
        {
            onlyScenePosition += new Vector3(500, 0, 0);
            return onlyScenePosition;
        }

        // 场景根节点 add by TangJian 2018/12/28 17:45
        private GameObject sceneRoot;
        public GameObject SceneRoot { get { return sceneRoot = (sceneRoot == null ? GameObject.FindGameObjectWithTag("SceneParent") : sceneRoot); } }

        // 切换场景 add by TangJian 2018/12/28 17:46
        public bool SwitchToScene(string id)
        {
            if (ShowScene(id))
            {
                if (CurrSceneId != id)
                    HideScene(CurrSceneId);
                currScene = GetScene(id);
                return true;
            }
            return false;
        }

        // 显示场景 add by TangJian 2018/12/28 17:46
        public bool ShowScene(string id)
        {
            var sceneController = GetScene(id);
            if (sceneController != null)
            {
                sceneController.ActiveScene();
            }
            else
            {
                GetScene(id).gameObject.SetActive(true);
            }
            return true;
        }

        // 隐藏场景 add by TangJian 2018/12/28 17:46
        public void HideScene(string id)
        {
            var sceneController = GetScene(id);
            if (sceneController != null)
            {
                sceneController.DeActiveScene();
            }
        }

        public SceneController GetScene(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            SceneController sceneController;
            if (scenes.TryGetValue(id, out sceneController))
            {
                return sceneController;
            }
            return null;
        }

        public void ClearLoadedScene()
        {
            foreach (var scene in scenes)
            {
                GameObject.Destroy(scene.Value);
            }

            scenes.Clear();
        }

        public SceneEvents currSceneEvents;

        private Observer roleEnterRoomObserver;

        private List<Observer> observers;
        
        private void OnEnable()
        {
            RegisterEvents();
        }

        private void OnDisable()
        {
            UnRegisterEvents();
        }

        public IEnumerator LoadLevelConfig(LevelConfig levelConfig)
        {
            ClearLoadedScene();

            List<SceneData> sceneDataList = new List<SceneData>();
            PathConfig currPath = levelConfig.currPath;
            Debug.Assert(currPath != null);
           
            // 创建场景数据 add by TangJian 2018/12/26 15:31
            foreach (var roomData in levelConfig.rooms)
            {
                SceneData sceneData = new SceneData();
                sceneData.sceneId = roomData.id;
                sceneData.rawSceneId = roomData.RawId;
                sceneDataList.Add(sceneData);
            }

            // 创建场景连接 add by TangJian 2018/12/26 15:32
            foreach (var connectionData in currPath.connections)
            {
                string fromSceneId = connectionData.fromScene;
                string fromPortalId = connectionData.fromPortal;

                string toSceneId = connectionData.toScene;
                string toPortalId = connectionData.toPortal;

                SceneData fromSceneData = sceneDataList.Find((SceneData sceneData_) => { return fromSceneId == sceneData_.sceneId; });
                SceneData toSceneData = sceneDataList.Find((SceneData sceneData_) => { return toSceneId == sceneData_.sceneId; });

                Debug.Assert(fromSceneData!= null, "找不到场景" + fromSceneId);
                Debug.Assert(toSceneData!= null, "找不到场景" + toSceneId);

                // 添加门 add by TangJian 2018/12/26 15:42
                {
                    PortalData fromPortalData = new PortalData();
                    
                    fromPortalData.id = fromPortalId;
                    fromPortalData.sceneId = fromSceneId;

                    fromPortalData.toSceneId = toSceneId;
                    fromPortalData.toPortalId = toPortalId;

                    fromSceneData.portalDatas.Add(fromPortalData);    

                    PortalData toPortalData = new PortalData();
                    
                    toPortalData.id = toPortalId;
                    toPortalData.sceneId = toSceneId;

                    toPortalData.toSceneId = fromSceneId;
                    toPortalData.toPortalId = fromPortalId;
                    
                    toSceneData.portalDatas.Add(toPortalData);   
                }
            }

            // 向场景数据中添加怪物数据 add by TangJian 2018/12/27 12:42
            foreach (var objectData in levelConfig.objectses)
            {
                RoomObjectesConfig newRoomObjects = objectData;

                SceneData sceneData = sceneDataList.Find((SceneData sceneData_) => { return newRoomObjects.roomId == sceneData_.sceneId; });
                Debug.Assert(sceneData!=null, "找不到SceneData: " + newRoomObjects.roomId);

                for (int j = 0; j < newRoomObjects.objects.Count; j++)
                {
                    RoomObjectConfig newRoomObject = newRoomObjects.objects[j];

                    for (int k = 0; k < newRoomObject.count; k++)
                    {
                        RoleData roleData = new RoleData();
                        roleData.Id = newRoomObject.objectId;
                        roleData.Prefab = newRoomObject.objectId;
                        roleData.TeamId = "2";
                        roleData.locationData.areaId = newRoomObject.areaId;

                        sceneData.roleDatas.Add(roleData);
                    }
                }
            }
            
            // 设置场景事件 add by TangJian 2019/1/2 14:49
            InitSceneEvent(levelConfig.sceneEvents);
            
            int step = 0;
            int stepCount = sceneDataList.Count;
            
            // 创建场景 add by TangJian 2018/12/26 15:48
            for (int i = 0; i < sceneDataList.Count; i++)
            {
                SceneData sceneData = sceneDataList[i];

                var handle = CreateSceneAsync(sceneData, levelConfig.difficultyLevel);

                while (handle.IsCompleted == false)
                {
                    yield return (float)(step) / stepCount;
                }
                
                SceneController scene = handle.Result;
                
//                SceneController scene = CreateScene(sceneData, levelConfig.difficultyLevel);
                scenes.Add(sceneData.sceneId, scene);
                scene.gameObject.SetActive(false);
                yield return (float)(++step) / stepCount;
            }
        }

        public async Task<SceneController> CreateSceneAsync(SceneData sceneData, int difficultyLevel = 1)
        {
            // 新建场景
            Task<GameObject> task = AssetManager.LoadAssetAsync<GameObject>("Scenes/" + sceneData.rawSceneId + ".prefab");

            GameObject prefab = await task;
            
            Debug.Assert(prefab != null, "找不到场景预制体:" + "Prefabs / Scenes / " + sceneData.rawSceneId);
            GameObject scene = Instantiate(prefab);

            // Debug.Log(" 预制体id " + sceneData.rawSceneId);
            scene.transform.parent = SceneRoot.transform;
            scene.transform.localPosition = GetOnlyScenePosition();
            scene.name = sceneData.sceneId;
            SceneController sceneController = scene.GetComponent<SceneController>();
            sceneController.SceneData = sceneData;

            {
                // 添加怪物 add by TangJian 2017/08/03 21:35:51
                {
                    foreach (var roleData in sceneData.roleDatas)
                    {
                        roleData.locationData.position = sceneController.GetAreaRandomPosition(roleData.locationData.areaId);
                        roleData.difficultyLevel = difficultyLevel;
                        
                        sceneController.AddRole(roleData.Prefab, roleData.locationData.areaId);
                    }
                }
//
//                // 添加物品 add by TangJian 2017/08/22 19:45:51
//                {
//                    foreach (var itemData in sceneData.itemDatas)
//                    {
//                        var item = GameObjectManager.Instance.Create(itemData.id);
//                        DropItemController dropItemController = item.GetComponent<DropItemController>();
//                        itemData.position = sceneController.GetAreaRandomPosition("MonsterAreas");
//                        sceneController.DropItemEnterWithLocalPosition(dropItemController, itemData.position);
//                    }
//                }
//
//                // 添加放置物体 add by TangJian 2018/01/23 15:52:08
//                {
//                    foreach (var placementData in sceneData.PlacementDatas)
//                    {
//                        var placementObject = GameObjectManager.Instance.Create(placementData.id);
//                        SceneObjectController sceneObjectController = placementObject.GetComponent<SceneObjectController>();
//                        
//                        PlacementController treasureBoxController = placementObject.GetComponent<PlacementController>();
//                        treasureBoxController.Placementdata = placementData;
//
//                        placementData.position = sceneController.GetAreaRandomPosition("MonsterAreas");
//                        sceneController.PlacementEnterWithLocalPosition(sceneObjectController, placementData.position);
//
//                        ScenePlacementComponent sceneComponent = treasureBoxController.gameObject.GetComponent<ScenePlacementComponent>();
//                        if (sceneComponent != null)
//                        {
//                            sceneComponent.GridPos = placementObject.transform.localPosition;
//                        }
//                    }
//                }
//
                // 设置门数据 add by TangJian 2018/12/26 16:00
                for (int i = 0; i < sceneData.portalDatas.Count; i++)
                {
                    PortalData portalData = sceneData.portalDatas[i];

                    Transform portalTransform = scene.transform.Find("Portals/" + portalData.id);
                    Debug.Assert(portalTransform != null, "找不到门: " + sceneData.sceneId + ":" + portalData.id);

                    PortalController portalController = portalTransform.GetComponent<PortalController>();
                    portalController.PortalData = portalData;
                    portalController.State = 1;
                }

                // 为丢失NavMeshDataBuild
                {
                    foreach(var item in scene.GetComponents<UnityEngine.AI.NavMeshSurface>())
                    {
                        if (item.navMeshData == null)
                        {
                            item.BuildNavMesh();
                        }
                    }
                }
            }

            // 初始化
            // sceneController.InitData();


            return sceneController;
        }
        
        public async Task<SceneController> CreateScene(SceneData sceneData, int difficultyLevel = 1)
        {
            // 新建场景
            GameObject prefab = await AssetManager.LoadAssetAsync<GameObject>("Assets/Resources_moved/Prefabs/Scenes/" + sceneData.rawSceneId+".prefab");
            Debug.Assert(prefab != null, "找不到场景预制体:" + "Prefabs / Scenes / " + sceneData.rawSceneId);
            GameObject scene = Instantiate(prefab);

            // Debug.Log(" 预制体id " + sceneData.rawSceneId);
            scene.transform.parent = SceneRoot.transform;
            scene.transform.localPosition = GetOnlyScenePosition();
            scene.name = sceneData.sceneId;
            SceneController sceneController = scene.GetComponent<SceneController>();
            sceneController.SceneData = sceneData;

            {
                // 添加怪物 add by TangJian 2017/08/03 21:35:51
                {
                    foreach (var roleData in sceneData.roleDatas)
                    {
                        roleData.locationData.position = sceneController.GetAreaRandomPosition(roleData.locationData.areaId);
                        roleData.difficultyLevel = difficultyLevel;
                        sceneController.AddRole(roleData);
                    }
                }

                // 添加物品 add by TangJian 2017/08/22 19:45:51
                {
                    foreach (var itemData in sceneData.itemDatas)
                    {
                        var item = GameObjectManager.Instance.Create(itemData.id);
                        DropItemController dropItemController = item.GetComponent<DropItemController>();
                        itemData.position = sceneController.GetAreaRandomPosition("MonsterAreas");
                        sceneController.DropItemEnterWithLocalPosition(dropItemController, itemData.position);
                    }
                }

                // 添加放置物体 add by TangJian 2018/01/23 15:52:08
                {
                    foreach (var placementData in sceneData.PlacementDatas)
                    {
                        var placementObject = GameObjectManager.Instance.Create(placementData.id);
                        SceneObjectController sceneObjectController = placementObject.GetComponent<SceneObjectController>();
                        
                        PlacementController treasureBoxController = placementObject.GetComponent<PlacementController>();
                        treasureBoxController.Placementdata = placementData;

                        placementData.position = sceneController.GetAreaRandomPosition("MonsterAreas");
                        sceneController.PlacementEnterWithLocalPosition(sceneObjectController, placementData.position);

                        ScenePlacementComponent sceneComponent = treasureBoxController.gameObject.GetComponent<ScenePlacementComponent>();
                        if (sceneComponent != null)
                        {
                            sceneComponent.GridPos = placementObject.transform.localPosition;
                        }
                    }
                }

                // 设置门数据 add by TangJian 2018/12/26 16:00
                for (int i = 0; i < sceneData.portalDatas.Count; i++)
                {
                    PortalData portalData = sceneData.portalDatas[i];

                    Transform portalTransform = scene.transform.Find("Portals/" + portalData.id);
                    Debug.Assert(portalTransform != null, "找不到门: " + sceneData.sceneId + ":" + portalData.id);

                    PortalController portalController = portalTransform.GetComponent<PortalController>();
                    portalController.PortalData = portalData;
                    portalController.State = 1;
                }

                // 为丢失NavMeshDataBuild
                {
                    foreach(var item in scene.GetComponents<UnityEngine.AI.NavMeshSurface>())
                    {
                        if (item.navMeshData == null)
                        {
                            item.BuildNavMesh();
                        }
                    }
                }
            }

            // 初始化
            // sceneController.InitData();


            return sceneController;
        }

        public SceneController GetLoadedSceneController(string id)
        {
            var scene = GetScene(id);
            if (scene != null)
            {
                var sceneController = scene.GetComponent<SceneController>();
                if (sceneController != null)
                {
                    return sceneController;
                }
            }
            return null;
        }

        public Transform GetTransform(string objId, string sceneId)
        {
            Transform retTr = null;

            Transform sceneTf = SceneRoot.transform.Find(sceneId);
            sceneTf.Recursive((Transform tf, int depth) =>
            {
                if(tf.name == objId)
                    retTr = tf;
            }, 1, 999);
            
            return retTr;
        }

        public GameObject GetObject(string objId, string sceneId)
        {
            Transform tr = GetTransform(objId, sceneId);
            return tr != null ? tr.gameObject : null;
        }
        
        public T GetObjectComponent<T>(string objId, string sceneId) where T : class
        {
            Transform tf = GetTransform(objId, sceneId);
            if (tf != null)
            {
                return tf.GetComponent<T>();
            }

            return null;
        }
        
        // 物品进入场景 本地坐标 add by TangJian 2019/4/2 12:19
        public bool DropItemEnterSceneWithLocalPosition(SceneObjectController sceneObjectController, string sceneId, Vector3 localPosition)
        {
            return ObjectEnterSceneLocalPosition(sceneObjectController, sceneId, localPosition);
        }
        
        public bool DropItemEnterSceneWithWorldPosition(SceneObjectController sceneObjectController, string sceneId, Vector3 worldPosition)
        {
            return ObjectEnterSceneWithWorldPosition(sceneObjectController, sceneId, worldPosition);
        }
        
        // 角色进入场景 世界坐标 add by TangJian 2019/4/2 14:58
        public bool RoleEnterSceneWithLocalPosition(RoleController roleController, string sceneId,
            Vector3 localPosition)
        {
            var sceneController = GetScene(sceneId);
            sceneController.RoleEnterWithLocalPosition(roleController, localPosition);
            return true;
        }

        public bool RoleEnterSceneWithWorldPosition(RoleController roleController, string sceneId,
            Vector3 worldPosition)
        {
            var sceneController = GetScene(sceneId);
            sceneController.RoleEnterWithLocalPosition(roleController, sceneController.transform.InverseTransformPoint(worldPosition));
            return true;
        }

        // 进入场景方法 add by TangJian 2018/12/28 17:41
        public bool ObjectEnterSceneWithWorldPosition(SceneObjectController sceneObjectController, string sceneId, Vector3 position)
        {
            var sceneController = GetScene(sceneId);
            sceneController.ObjectEnterWithLocalPosition(sceneObjectController, sceneController.transform.InverseTransformPoint(position));
            return true;
        }
        
        public bool ObjectEnterSceneLocalPosition(SceneObjectController sceneObjectController, string sceneId, Vector3 localPosition)
        {
            var sceneController = GetScene(sceneId);
            sceneController.ObjectEnterWithLocalPosition(sceneObjectController, localPosition);
            return true;
        }

        public bool RoleEnterSceneByPortal(RoleController sceneObjectController, string sceneId, string portalId)
        {
            portalId = "Portals/" + portalId;
            var scene = GetScene(sceneId);
            if (scene && scene.transform.Find(portalId))
            {
                var sceneController = scene.GetComponent<SceneController>();
                if (sceneController != null)
                {
                    GameObject portalObject = scene.transform.Find(portalId).gameObject;
                    PortalController portalController = portalObject.GetComponent<PortalController>();

                    if (portalController != null)
                        RoleEnterSceneWithWorldPosition(sceneObjectController, sceneId,
                            portalController.ExitRoot.transform.position);

                    return true;
                }
            }
            return false;
        }

        public bool RoleEnterSceneByPortal(RoleController sceneObjectController, string fromSceneId, string fromPortalId, string toSceneId,
            string toPortalId)
        {
            if (RoleEnterSceneByPortal(sceneObjectController, toSceneId, toPortalId))
            {
                SceneEventManager.Instance.ObjEnterRoom(sceneObjectController.name, fromSceneId, fromPortalId, toSceneId,
                    toSceneId);
                return true;
            }
            return false;
        }

        public bool RoleEnterAndSwitchScene(RoleController sceneObjectController, string sceneId, string areaName)
        {
            if (SceneManager.Instance.SwitchToScene(sceneId))
            {
                var sceneController = SceneManager.Instance.GetLoadedSceneController(sceneId);
                return RoleEnterSceneWithWorldPosition(sceneObjectController, sceneId, sceneController.transform.TransformPoint(sceneController.GetAreaRandomPosition(areaName)));
            }
            return false;
        }

        private void LateUpdate()
        {
            SortRendererManager.Instance.Update();
        }
    }
}