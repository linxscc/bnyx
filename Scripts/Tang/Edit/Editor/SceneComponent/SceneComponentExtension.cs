


namespace Tang.Editor
{
    public static class SceneComponentExtension
    {
        public static void UnDoMoveBy(this SceneComponent target, int x, int y, int z)
        {
            target.MoveByGridPos(new UnityEngine.Vector3(x, y, z));
        }
    }
}