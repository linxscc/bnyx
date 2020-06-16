using UnityEngine;

namespace Tang
{
    [System.Serializable]
    public class SceneObjectData
    {
        public string name;
        public string sceneId = "Root";
        public Vector3 position;
    }

    public class SceneObjectController : MyMonoBehaviour
    {

        public string OldId;

        public string Id
        {
            get { return name; }
        }

        public string OldSceneId;

        private string sceneId;

        public string SceneId
        {
            get
            {
                if (string.IsNullOrEmpty(sceneId))
                {
                    if (CurrSceneController != null)
                    {
                        sceneId = CurrSceneController.name;
                    }
                    else
                    {
                        Debug.LogError(name + ": 不属于任何场景");
                        return "Root";
                    }
                }

                return sceneId;
            }
            set { sceneId = value; }
        }

        public SceneController CurrSceneController;
        public SceneObjectData SceneObjectData = new SceneObjectData();

        // 路径 add by TangJian 2019/1/8 15:08
        public virtual string PathInScene
        {
            get { return "SceneObjects"; }
        }

        public virtual void OnEnable()
        {
//            ObjectControllerManager.Instance.Add(ObjectData.sceneId + ObjectData.name,this);

            if ((string.IsNullOrEmpty(OldId) == false && string.IsNullOrEmpty(OldSceneId) == false) && (Id != OldId || SceneId != OldSceneId))
            {
                SceneObjectControllerManager.Instance.Remove(OldId, this);
                SceneObjectControllerManager.Instance.Remove(OldSceneId + ":" + OldId, this);
            
                SceneObjectControllerManager.Instance.Add(Id, this);
                SceneObjectControllerManager.Instance.Add(SceneId + ":" + Id, this);
                OldId = Id;
                OldSceneId = SceneId;
            }
        }

        public virtual void OnDisable()
        {
//            ObjectControllerManager.Instance.RemoveObjectController(this);
        }

        public virtual void Start()
        {
            SceneObjectControllerManager.Instance.Add(Id, this);
            SceneObjectControllerManager.Instance.Add(SceneId + ":" + Id, this);
            OldId = Id;
            OldSceneId = SceneId;
            
            SceneEventManager.Instance.ObjCreate(Id, SceneId);
        }

        public virtual void Awake()
        {
//            SceneObjectControllerManager.Instance.Add(Id, this);
//            SceneObjectControllerManager.Instance.Add(SceneId + ":" + Id, this);
//            OldId = Id;
//            OldSceneId = SceneId;
        }

        public virtual void OnDestroy()
        {
            SceneObjectControllerManager.Instance.Remove(OldId, this);
            SceneObjectControllerManager.Instance.Remove(OldSceneId + ":" + OldId, this);
            
            SceneObjectControllerManager.Instance.Remove(Id, this);
            SceneObjectControllerManager.Instance.Remove(SceneId + ":" + Id, this);
            
            SceneEventManager.Instance.ObjDestory(Id, SceneId);
        }
    }
}