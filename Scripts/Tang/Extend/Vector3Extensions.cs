using UnityEngine;




namespace Tang
{
    public static class Vector2Extensions
    {
        public static Vector2 Mul(this Vector2 t1, Vector2 t2)
        {
            return new Vector2(t1.x * t2.x, t1.y * t2.y);
        }
    }

    public static class Vector3Extensions
    {
        public static Vector3 frontVec3 = new Vector3(1, 0, 0);

        #region 向量旋转

        public static Vector3 RotateFrontTo(this Vector3 target, Vector3 to)
        {
            Quaternion rotation = Quaternion.FromToRotation(frontVec3, to);
            return rotation * target;
        }

        public static Vector3 RotateFromTo(this Vector3 target, Vector3 from, Vector3 to)
        {
            Quaternion rotation = Quaternion.FromToRotation(from, to);
            return rotation * target;
        }
        public static Vector3 RandomMinMax(Vector3 min,Vector3 max)
        {
            return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        }

        public static Vector3 RotateToFront(this Vector3 target)
        {
            return target.RotateFromTo(target, frontVec3);
        }

        public static Vector3 RotateTo(this Vector3 target, Vector3 to)
        {
            return target.RotateFromTo(target, to);
        }
        
        // 限制角度在180以内 add by TangJian 2019/4/22 14:51
        public static Vector3 AngleLimit180(this Vector3 target)
        {
            return new Vector3(AngleLimit180(target.x), AngleLimit180(target.y), AngleLimit180(target.z));
        }

        public static float AngleLimit180(float angle)
        {
            angle = angle % 360f;
            if (angle > 180f)
            {
                angle = -(360f - angle);
            }

            return angle;
        }


        // 获得向量角度 add by TangJian 2019/4/22 14:38
        public static Vector3 GetAngle(this Vector3 target)
        {
            Vector3 angle = Quaternion.FromToRotation(Vector3.right, new Vector3(target.x, target.y, -target.z))
                .eulerAngles;
            return angle.AngleLimit180();
        }

        public static float CalcAngle(float x, float y)
        {
            float angle = 0;
            float absX = Mathf.Abs(x);
            float absY = Mathf.Abs(y);
            
            if (x > 0)
            {
                if (y > 0)
                {
                    angle = Mathf.Atan(absY / absX) * 180f / Mathf.PI;
                }
                else if (y < 0)
                {
                    angle = 360f - Mathf.Atan(absY / absX) * 180f / Mathf.PI;
                }
                else
                {
                    angle = 0;
                }
            }
            else if (x < 0)
            {
                if (y > 0)
                {
                    angle = 180f - Mathf.Atan(absY / absX) * 180f / Mathf.PI;
                }
                else if (y < 0)
                {
                    angle = 180f + Mathf.Atan(absY / absX) * 180f / Mathf.PI;
                }
                else
                {
                    angle = 180f;
                }
            }
            else
            {
                if (y > 0)
                {
                    angle = 90f;
                }
                else if (y < 0)
                {
                    angle = 270f;
                }
                else
                {
                    angle = 0;
                }
            }

            return angle;
        }

        public static float GetAngleX(this Vector3 target)
        {
            return CalcAngle(target.y, target.z);
//            return Quaternion.FromToRotation(Vector3.forward, new Vector3(0, target.y, -target.z)).eulerAngles.x;
        }

        public static float GetAngleY(this Vector3 target)
        {
            return CalcAngle(target.x, target.z);
            //            return Quaternion.FromToRotation(Vector3.right, new Vector3(target.x, 0, -target.z)).eulerAngles.y;
        }
        
        public static float GetAngleZ(this Vector3 target)
        {
            return CalcAngle(target.x, target.y);
//            return Quaternion.FromToRotation(Vector3.right, new Vector3(target.x, target.y, 0)).eulerAngles.z;
        }
        // 角度控制 add by TangJian 2019/4/22 13:58
        public static Vector3 AngleYLimit(this Vector3 target, float angleYMax)
        {
            var offset = target;
            offset.y *= -1;            
            Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, offset);
            var maxPos = Quaternion.Euler(new Vector3(0, -angleYMax, 0)) * Vector3.right;
            offset = offset.RotateTo(maxPos);
            return offset;
        }

        #endregion
        
        // 翻转 add by TangJian 2019/4/22 12:51
        public static Vector3 Flip(this Vector3 target, bool flip = true)
        {
            if (flip)
                return new Vector3(-target.x, target.y, target.z);
            return target;
        }

        public static Vector3 Mul(this Vector3 target, Vector3 value)
        {
            return new Vector3(target.x * value.x, target.y * value.y, target.z * value.z);
        }

        public static Vector3 Mul(this Vector3 target, float x, float y, float z)
        {
            Vector3 value = new Vector3(x, y, z);
            return new Vector3(target.x * value.x, target.y * value.y, target.z * value.z);
        }

        public static Vector3 MoveByToSpeed(this Vector3 target, float GrivityAcceleration = 60, float GroundFrictionAcceleration = 60)
        {
            Vector3 speed;
            speed.x = (target.x >= 0 ? 1 : -1) * Mathf.Sqrt(Mathf.Abs(2 * GroundFrictionAcceleration * target.x));
            speed.z = (target.z >= 0 ? 1 : -1) * Mathf.Sqrt(Mathf.Abs(2 * GroundFrictionAcceleration * target.z));

//            if (target.y < 0)
//            {
//                target.y *= -1;
//            }

            speed.y = target.y >= 0 ? Mathf.Sqrt(2 * GrivityAcceleration * target.y) : -Mathf.Sqrt(2 * GrivityAcceleration * -target.y);
            return speed;
        }
    }

    public static class RigidbodyExtend
    {
        public static Vector3 frontVec3 = new Vector3(1, 0, 0);

        public static void AddForceWithRotationFrontTo(this Rigidbody target, Vector3 force, ForceMode forceMode, Vector3 to)
        {
            target.AddForce(force.RotateFrontTo(to), forceMode);
        }
    }
}