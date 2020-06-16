using UnityEngine;

namespace Tang
{
    public static class Matrix4x4Extensions
    {
        public static Matrix4x4 GetMove(this Matrix4x4 target, Vector3 vector3)
        {
            return target.GetMove(vector3.x, vector3.y, vector3.z);
        }

        public static Matrix4x4 GetMove(this Matrix4x4 target, float x, float y, float z)
        {
            Matrix4x4 matrix4X4 = new Matrix4x4();
            matrix4X4.SetRow(0, new Vector4(1, 0, 0, x));
            matrix4X4.SetRow(1, new Vector4(0, 1, 0, y));
            matrix4X4.SetRow(2, new Vector4(0, 0, 1, z));
            matrix4X4.SetRow(3, new Vector4(0, 0, 0, 1));

            return target * matrix4X4;
        }
        
        public static Matrix4x4 GetScale(this Matrix4x4 target, float x, float y, float z)
        {
            Matrix4x4 matrix4X4 = new Matrix4x4();
            matrix4X4.SetRow(0, new Vector4(x, 0, 0, 0));
            matrix4X4.SetRow(1, new Vector4(0, y, 0, 0));
            matrix4X4.SetRow(2, new Vector4(0, 0, z, 0));
            matrix4X4.SetRow(3, new Vector4(0, 0, 0, 1));
            
            return target * matrix4X4;
        }
    }
}