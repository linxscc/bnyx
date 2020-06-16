using System.Collections.Generic;
using UnityEngine;
using System;



namespace Tang
{
    public partial class SceneManager
    {
        void RegisterEvents()
        {
            SceneEventManager.Instance.ObjEnterRoomDelegates += OnObjEnterRoom;
            SceneEventManager.Instance.ObjStateChangeDelegates += OnObjectStateChange;
            SceneEventManager.Instance.ObjDyingDelegates += OnObjDying;
            SceneEventManager.Instance.ObjCreateDelegates += OnObjCreate;
            SceneEventManager.Instance.ObjDestoryDelegates += OnObjDestroy;
        }

        void UnRegisterEvents()
        {
            SceneEventManager.Instance.ObjEnterRoomDelegates -= OnObjEnterRoom;
            SceneEventManager.Instance.ObjStateChangeDelegates -= OnObjectStateChange;
            SceneEventManager.Instance.ObjDyingDelegates -= OnObjDying;
            SceneEventManager.Instance.ObjCreateDelegates -= OnObjCreate;
            SceneEventManager.Instance.ObjDestoryDelegates -= OnObjDestroy;
        }
        
        // 对象创建 add by TangJian 2019/1/9 15:38
        void OnObjCreate(string objId, string roomId)
        {
            Debug.Log("创建对象:" + roomId + ":" + objId);

            string key = roomId + ":" + objId;
            
            List<SceneObjectController> sceneObjectControllers = SceneObjectControllerManager.Instance.Get(key);
            if (sceneObjectControllers != null)
            {
                currSceneEvents.SetInt("objCount", sceneObjectControllers.Count);
            }
            
            currSceneEvents.DoConditionsAndResults(SceneEventType.ObjCreate);
        }
        
        // 对象销毁 add by TangJian 2019/1/9 15:44
        void OnObjDestroy(string objId, string roomId)
        {
            Debug.Log("销毁对象:" + roomId + ":" + objId);
            
            currSceneEvents.SetString("objId", objId);
            currSceneEvents.SetString("roomId", roomId);
            
            string key = roomId + ":" + objId;
            
            List<SceneObjectController> sceneObjectControllers = SceneObjectControllerManager.Instance.Get(key);
            currSceneEvents.SetInt("objCount", sceneObjectControllers != null ? sceneObjectControllers.Count : 0);
            
            currSceneEvents.DoConditionsAndResults(SceneEventType.ObjDestroy);
        }
        
        // 角色死亡 add by TangJian 2019/1/7 16:32
        void OnObjDying(string objId, string roomId)
        {
            Debug.Log("对象死亡:" + roomId + ":" + objId);
            
//            GameObject.Find(objId).GetComponent<RemoveCharacterController>().enabled = false;
//            Debug.Log("成功移除" + objId + "的CharacterController！");
            
            currSceneEvents.SetString("objId", objId);
            currSceneEvents.SetString("roomId", roomId);

            string key = roomId + ":" + objId;
            
            List<SceneObjectController> sceneObjectControllers = SceneObjectControllerManager.Instance.Get(key);
            if (sceneObjectControllers != null)
            {
                currSceneEvents.SetInt("objCount", sceneObjectControllers.Count);
            }
            
            currSceneEvents.DoConditionsAndResults(SceneEventType.ObjDying);
        }

        // 角色进入房间事件 add by TangJian 2019/1/4 0:04
        void OnObjEnterRoom(string objId, string fromRoomId, string fromPortalId, string toRoomId,
            string toPortalId)
        {
            if (currSceneEvents == null) return;
           
            currSceneEvents.SetString("objId", objId);
            currSceneEvents.SetString("roomId", CurrSceneId);
            currSceneEvents.SetString("fromRoomId", fromRoomId);
            currSceneEvents.SetString("fromPortalId", fromPortalId);
            currSceneEvents.SetString("toRoomId", toRoomId);
            currSceneEvents.SetString("toPortalId", toPortalId);

            currSceneEvents.DoConditionsAndResults(SceneEventType.ObjEnterRoom);
        }
        
        // 物体状态变化事件 add by TangJian 2019/1/4 0:04
        void OnObjectStateChange(string objId, int state)
        {
            if (currSceneEvents == null) return;
            
            currSceneEvents.SetString("objId", objId);
            currSceneEvents.SetInt("state", state);
            
            currSceneEvents.DoConditionsAndResults(SceneEventType.ObjStateChange);
        }
        
        public void InitSceneEvent(SceneEvents sceneEvents)
        {
            currSceneEvents = sceneEvents;

            currSceneEvents.ResultDelegates += (Result result) =>
            {
                switch (result.resultType)
                {
                    case ResultType.AddRole:
                    {
                        string roleId = (string) result.parameters[0];

                        int count = Convert.ToInt32(result.parameters[1]);
                        string roomId = (string) result.parameters[2];
                        string areaId = (string) result.parameters[3];

                        SceneController sceneController = GetScene(roomId);
                        Debug.Assert(sceneController != null, "找不到场景:" + roomId);

                        if (sceneController)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                sceneController.AddRole(roleId, areaId);
                            }
                        }
                    }
                        break;
                    case ResultType.SetObjState:
                    {
                        string objId = (string)result.parameters[0];
                        int state = Convert.ToInt32(result.parameters[1]);

                        SceneObjectController sceneObjectController = SceneObjectControllerManager.Instance.GetFirst(objId);
                        IInteractable interactable = sceneObjectController.GetComponent<IInteractable>();
                        
//                        IInteractable interactable =  GetObjectComponent<IInteractable>(objId, roomId);
                        interactable.State = state;
                    }
                        break;
                    case ResultType.GamePass:
                    {
                        GameManager.Instance.GamePass();
                    }
                        break;
                    default:
                        break;
                }
            };
        }
    }
}