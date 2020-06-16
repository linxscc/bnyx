using System.Collections.Generic;

namespace Tang
{
    [System.Serializable]
    public class GameData
    {
        public bool useKeyboard;
        public PlayerData player1Data;
        public List<SceneData> sceneDatas;
        public List<RoleData> roleDatas;
    }
}