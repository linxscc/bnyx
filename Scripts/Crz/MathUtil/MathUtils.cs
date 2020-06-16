using UnityEngine;

namespace Tang
{
    public class MathUtils
    {

        /// <summary>
        /// 速度转角度
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static float SpeedToDirection(Vector3 speed)
        {
            float z = 0;
            //计算旋转角
            if (speed.x >= 0)
            {
                z = Mathf.Atan((speed.y) / speed.x) * Mathf.Rad2Deg;
            }
            else if (speed.x < 0)
            {
                z = Mathf.Atan((speed.y) / speed.x) * Mathf.Rad2Deg + 180;
            }

            if (float.IsNaN(z))
            {
                Debug.Log("z == float.NaN");
                return 0;
            }
            return z;
        }
    }

}
