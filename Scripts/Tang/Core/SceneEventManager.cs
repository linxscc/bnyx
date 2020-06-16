using System.Collections.Generic;
using Tang.Interfaces;

namespace Tang
{
    public class SceneEventManager
    {
        private static SceneEventManager s_sceneEventManager;

        public static SceneEventManager Instance
        {
            get
            {
                if (s_sceneEventManager == null)
                {
                    s_sceneEventManager = new SceneEventManager();
                }

                return s_sceneEventManager;
            }
        }

        public Dictionary<string, List<IObserver>> observerses = new Dictionary<string, List<IObserver>>();
        
        public virtual void SendNotification(string notificationName, object body = null, string type = null)
        {
            List<IObserver> observers;
            if (observerses.TryGetValue(notificationName, out observers))
            {
                for (int i = 0; i < observers.Count; i++)
                {
                    observers[i].NotifyObserver(new Notification(notificationName, body, type));
                }
            }
        }
        
        // 角色创建 add by TangJian 2019/1/9 15:33
        public delegate void ObjCreateDelegate(string objId, string roomId);
        public event ObjCreateDelegate ObjCreateDelegates;
        public void ObjCreate(string objId, string roomId)
        {
            if (ObjCreateDelegates != null)
                ObjCreateDelegates(objId, roomId);
        }
        
        // 销毁对象 add by TangJian 2019/1/9 15:46
        public delegate void ObjDestoryDelegate(string objId, string roomId);
        public event ObjDestoryDelegate ObjDestoryDelegates;
        public void ObjDestory(string objId, string roomId)
        {
            if (ObjDestoryDelegates != null)
                ObjDestoryDelegates(objId, roomId);
        }
        
        // 角色死亡 角色死亡 add by TangJian 2019/1/7 16:24
        public delegate void ObjDyingDelegate(string objId, string roomId);
        public event ObjDyingDelegate ObjDyingDelegates;
        public void ObjDying(string objId, string roomId)
        {
            if (ObjDyingDelegates != null)
                ObjDyingDelegates(objId, roomId);
        }
        
        // 角色进入房间 add by TangJian 2019/1/3 23:34
        public delegate void ObjEnterRoomDelegate(string objId, string fromRoomId, string fromPortalId, string toRoomId,
            string toPortalId);
        public event ObjEnterRoomDelegate ObjEnterRoomDelegates;
        public void ObjEnterRoom(string objId, string fromRoomId, string fromPortalId, string toRoomId,
            string toPortalId)
        {
            if (ObjEnterRoomDelegates != null)
                ObjEnterRoomDelegates(objId, fromRoomId, fromPortalId, toRoomId, toPortalId);
        }
        
        // 物体状态变化 add by TangJian 2019/1/3 23:34
        public delegate void ObjStateChangeDelegate(string objId, int toState);
        public event ObjStateChangeDelegate ObjStateChangeDelegates;
        public void ObjStateChange(string objId, int state)
        {
            if (ObjStateChangeDelegates != null)
                ObjStateChangeDelegates(objId, state);
        }
    }
}