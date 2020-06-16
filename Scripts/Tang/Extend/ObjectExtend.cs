


using UnityEngine;


namespace Tang
{
    public static class ObjectExtend
    {
        // public static bool IsValid(this object target)
        // {
        //     return target != null;
        // }

        public static void DrawLine(this Texture2D texture, Vector3 fromPoint, Vector3 toPoint, Color color)
        {
            texture.SetPixel((int)fromPoint.x, (int)fromPoint.y, color);
            while (fromPoint != toPoint)
            {
                fromPoint = Vector3.Lerp(fromPoint, toPoint, 1f / (toPoint - fromPoint).magnitude);
                texture.SetPixel((int)fromPoint.x, (int)fromPoint.y, color);
            }
        }
    }
}