namespace Tang
{
    public static class StaticMethod
    {
        public static Orientation Reverse(this Orientation target)
        {
            switch (target)
            {
                case Orientation.Up:
                    return Orientation.Down;
                case Orientation.Down:
                    return Orientation.Up;
                case Orientation.Left:
                    return Orientation.Right;
                case Orientation.Right:
                    return Orientation.Left;
            }
            return Orientation.Right;
        }
    }

    public enum Orientation
    {
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    public enum PortalType
    {
        None = 0,
        Entry = 1,
        Exit = 2
    }

    [System.Serializable]
    public class PortalData
    {
        public string id; // 传送门id add by TangJian 2017/08/02 20:34:58
        public string sceneId; // 所在场景id add by TangJian 2017/08/02 20:35:06
        public string toSceneId; // 要去的场景id add by TangJian 2017/08/02 20:35:16
        public string toPortalId; // 要取的场景的们的id add by TangJian 2017/08/02 20:35:30
        public int terrainIndex; // 地形位置 add by TangJian 2017/09/23 20:49:05
        public string terrainId; // 地形id add by TangJian 2017/09/23 20:48:19
        public string portalType = "LeftPortal"; // 传送门类型 add by TangJian 2017/09/27 20:21:04
        public Orientation orientation = Orientation.Right; // 传送门方向
        public PortalType pt = PortalType.None; // 传送门类型
        public DoorData door;
    }
}