using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


namespace Tang
{
    public partial class SceneController : MonoBehaviour
    {
        public SceneData SceneData;

        GameObject _itemParent;
        public GameObject ItemParent
        {
            get
            {
                if (_itemParent == null)
                {
                    _itemParent = transform.Find("Items").gameObject;
                }
                return _itemParent;
            }
        }
        
        ValueMonitorPool _valueMonitorPool = new ValueMonitorPool();
        
        private void Start()
        {
            InitPortal();
        }

        private void Update()
        {
            _valueMonitorPool.Update();
        }

        public void ActiveScene()
        {
            gameObject.SetActive(true);
        }

        public void DeActiveScene()
        {
            gameObject.SetActive(false);
        }
        
        public void ObjectEnterWithWorldPosition(SceneObjectController sceneObjectController, Vector3 position)
        {
            ObjectEnterWithLocalPosition(sceneObjectController, transform.InverseTransformPoint(position));
        }
        
        public void ObjectEnterWithLocalPosition(SceneObjectController sceneObjectController, Vector3 localPosition)
        {
            sceneObjectController.CurrSceneController = this;
            sceneObjectController.SceneId = name;

            Transform andAutoAdd = transform.FindAndAutoAdd(sceneObjectController.PathInScene);
            sceneObjectController.transform.parent = andAutoAdd.transform;
            sceneObjectController.transform.localPosition = localPosition;
        }


        public void RoleEnterWithWorldPosition(RoleController role, Vector3 position)
        {
            EnterRoleController(role);
            ObjectEnterWithWorldPosition(role, position);
        }

        public void RoleEnterWithLocalPosition(RoleController role, Vector3 localPosition)
        {
            EnterRoleController(role);
            ObjectEnterWithLocalPosition(role, localPosition);
        }

        public void PlacementEnterWithLocalPosition(SceneObjectController sceneObjectController, Vector3 localPosition)
        {
            ObjectEnterWithLocalPosition(sceneObjectController, localPosition);
        }

        public void DropItemEnterWithLocalPosition(DropItemController dropItemController, Vector3 localPosition)
        {
            ObjectEnterWithLocalPosition(dropItemController, localPosition);
        }

        public void DropItemEnterWithLocalPosition(GameObject item, Vector3 localPosition)
        {
            if (item != null)
            {
                item.transform.parent = ItemParent.transform;
                item.transform.localPosition = localPosition;
            }
            else
            {
                Debug.Log("添加的物品为空!");
            }
        }

        //
        Dictionary<string, List<GameObject>> _areaListMap = new Dictionary<string, List<GameObject>>();
        public Vector3 GetAreaRandomPosition(string areaId)
        {
            List<GameObject> areaList = null;
            if (_areaListMap.ContainsKey(areaId))
            {
                areaList = _areaListMap[areaId];
            }
            else
            {
                areaList = new List<GameObject>();
                var roleAreas = base.gameObject.transform.Find("Areas/" + areaId);
                if (roleAreas != null)
                {
                    for (int i = 0; i < roleAreas.transform.childCount; i++)
                    {
                        var roleArea = roleAreas.GetChild(i).gameObject;
                        areaList.Add(roleArea);
                    }
                }
                _areaListMap.Add(areaId, areaList);
            }

            if (areaList != null)
            {
                if (areaList.Count > 0)
                {
                    int randomIndex = Tools.RandomWithWeight<GameObject>(areaList, (GameObject obj, int index) =>
                    {
                        int weight = (int)(obj.transform.localScale.x * obj.transform.localScale.z);
                        return weight;
                    });

                    if (randomIndex >= 0)
                    {
                        var currRoleArea = areaList[randomIndex];
                        var randomPosition = Tools.RandomPositionInCube(currRoleArea.transform.position, currRoleArea.transform.lossyScale);

                        randomPosition.y = currRoleArea.transform.position.y;
                        NavMeshHit hit;
                        Vector3 targetPos = transform.InverseTransformPoint(randomPosition);
                        if (NavMesh.SamplePosition(targetPos, out hit, 1.0f, NavMesh.AllAreas))
                        {
                            targetPos = hit.position;
                        }
                        return targetPos;
                    }
                    else
                    {
                        Debug.Log("error!!!");
                    }
                }
            }
            return new Vector3();
        }

        public GameObject AddRole(RoleData roleData)
        {
            var role = GameObjectManager.Instance.Create(roleData.Prefab);
            Debug.Assert(role, "没有角色[" + roleData.Prefab + "]");
            
            var roleController = role.GetComponent<RoleController>();
            roleController.RoleData.TeamId = "2";
            roleController.RoleData.Id = roleData.Id;
            roleController.RoleData.Prefab = roleData.Prefab;
            roleController.RoleData.locationData.areaId = roleData.locationData.areaId;
            
            RoleEnterWithLocalPosition(roleController, roleData.locationData.position);
            return role;
        }

        public async Task<RoleController> AddRole(string prefabId, string areaId)
        {
            var role = await AssetManager.InstantiateRole(prefabId);
            Debug.Assert(role, "没有角色[" + prefabId + "]");

            var roleController = role.GetComponent<RoleController>();
            roleController.RoleData.TeamId = "2";

            RoleEnterWithLocalPosition(roleController, GetAreaRandomPosition(areaId));
            return roleController;
        }

        private List<Bounds> GetMosnterRefreshArea()
        {
            Camera cameraparent = Camera.main;
            Vector3 CameraPos = cameraparent.transform.position;
            
            Camera camera = cameraparent.transform.Find("All").GetComponent<Camera>();
            var CameraHeight = camera.orthographicSize *2;
            var CameraWidth = CameraHeight * camera.aspect;

            var GroundLeftX = CameraPos.x - CameraWidth / 2;
            var GroundRightX = CameraPos.x + CameraWidth / 2;
            
            Vector3 center1 = new Vector3((2*GroundLeftX - 10f)/2,CameraPos.y, CameraPos.z);
            Vector3 size1 = new Vector3(10,2,CameraHeight/2);
            Bounds bounds1 = new Bounds(center1,size1);
           
            Vector3 center2 = new Vector3((2*GroundRightX + 10f)/2,CameraPos.y, CameraPos.z);
            Vector3 size2 = new Vector3(10,2,CameraHeight/2);
            Bounds bounds2 = new Bounds(center2,size2);
            
            return  new List<Bounds>{bounds1,bounds2};
        }

        private List<Vector3> GetMosnterRefreshPos(int count)
        {
            List<Bounds> boundses = GetMosnterRefreshArea();
            List<Vector3> RandomPos = new List<Vector3>();
            
            for (int item = 0; item < count; item++)
            {
                int index = Tools.RandomWithWeight(boundses, (bounds_, i) => { return (int)(bounds_.size.x * bounds_.size.z * 100); });
                Bounds bounds = boundses[index];
                Vector3 position = Tools.RandomPositionInCube(bounds.center, bounds.size);
                
                RandomPos.Add(position);
            }
            
            return RandomPos;
        }

        public  void RefreshMonster(string id,int count)
        {
            List<Vector3> vector3s = GetMosnterRefreshPos(count);
            foreach (var item in vector3s)
            {
                AddRole(id, item);
            }
        }

        public async void AddRole(string id, Vector3 pos)
        {
            var role = await AssetManager.InstantiateRole(id);
            Debug.Assert(role, "没有角色[" + id + "]");

            var roleController = role.GetComponent<RoleController>();
            roleController.RoleData.TeamId = "2";

            RoleEnterWithWorldPosition(roleController, GetRefreshPosition(pos));
            
        }

        public Vector3 GetRefreshPosition(Vector3 pos)
        {
            Vector3 refreshPosition;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pos, out hit, 10, NavMesh.AllAreas))
            {
                refreshPosition = hit.position;
            }
            else
            {
                if (NavMesh.FindClosestEdge(pos, out hit, NavMesh.AllAreas))
                {
                    refreshPosition = hit.position;                    
                }
                else
                {
                    throw new Exception("找不到边界");
                }
            }

            return refreshPosition;
        }
    }
}