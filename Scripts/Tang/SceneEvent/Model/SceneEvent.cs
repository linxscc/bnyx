using System.Collections.Generic;

namespace Tang
{
    public enum SceneEventType
    {
        ObjEnterRoom,
        ObjStateChange,
        ObjCreate,
        ObjDestroy,
        ObjDying
    }
    
    public class SceneEvent
    {
        public SceneEventType eventType;
        public List<Condition> conditions;
        public List<Result> results;
        public int times = 0;
        public int totalTimes = 1;
    }
}