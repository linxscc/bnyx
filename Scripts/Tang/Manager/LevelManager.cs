using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Tang
{
    public static class LevelEvent
    {
        public static string ROLE_ENTER_ROOM = "ROLE_ENTER_ROOM";
    }

    public class RoleEnterRoomEvent
    {
        public static string ROLE_ENTER_ROOM = "ROLE_ENTER_ROOM";

        public RoleEnterRoomEvent(string roleId, string fromSceneId, string fromPortalId, string toSceneId,
            string toPortalId)
        {
            RoleId = roleId;
            FromSceneId = fromSceneId;
            FromPortalId = fromPortalId;
            ToSceneId = toSceneId;
            ToPortalId = toPortalId;
        }

        public override string ToString()
        {
            return "[" + RoleId + ":" + FromSceneId + ":" + FromPortalId + ":" + ToSceneId + ":" + ToPortalId + "]";
        }

        public string RoleId;
        public string FromSceneId;
        public string FromPortalId;
        public string ToSceneId;
        public string ToPortalId;
    }

    public class JoystickStateChangeEvent
    {
        public static string EventName = "JoystickStateChangeEvent";
        
        public JoystickStateChangeEvent(string sceneId, string comId, string stateName, int stateNameHash)
        {
            SceneId = sceneId;
            ComId = comId;
            StateName = stateName;
            StateNameHash = stateNameHash;
        }

        public string SceneId;
        public string ComId;
        public int StateNameHash;
        public string StateName;
    }

    public class LevelManager : MonoBehaviour
    {
        private static LevelManager instance;

        public static LevelManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<LevelManager>();
                }
                return instance;
            }
        }

        public async Task<LevelConfig> GetNewLevelConfig(string levelName)
        {
            LevelConfig newLevelConfig = new LevelConfig();

            // 房间 add by TangJian 2018/12/27 12:30
            newLevelConfig.rooms = await AssetManager.LoadJson<List<RoomConfig>>("Level/" + levelName + "/Rooms.json");
//                Tools.Json2Obj<List<RoomConfig>>(Tools.getJsonStringFromResource("Configs/Levels/" + levelName + "/Rooms"));

            // 房间连接 add by TangJian 2018/12/27 12:30
            for (int i = 1; i < 999; i++)
            {
                string pathString = await AssetManager.LoadString("Level/" + levelName + "/Path" + i + ".json");
                if (pathString == null)
                    break;
                PathConfig newPath = Tools.Json2Obj<PathConfig>(pathString);
                newLevelConfig.paths.Add(newPath);
            }

            // 房间怪物物品 add by TangJian 2018/12/27 12:31
            newLevelConfig.objectses =
                await AssetManager.LoadJson<List<RoomObjectesConfig>>("Level/" + levelName + "/Roles.json");

            // 读取场景事件 add by TangJian 2018/12/29 16:04
            newLevelConfig.sceneEvents =
                await AssetManager.LoadJson<SceneEvents>("Level/" + levelName + "/Events.json");

            return newLevelConfig;
        }
    }
}