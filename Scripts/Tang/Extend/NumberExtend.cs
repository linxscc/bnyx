using System;






namespace Tang
{
    public static class NumberExtend
    {
        public static T Range<T>(this T target, T min, T max) where T : IComparable
        {
            T ret = target;
            if (target.CompareTo(min) < 0)
            {
                ret = min;
            }
            else if (target.CompareTo(max) > 0)
            {
                ret = max;
            }
            return ret;
        }

        public static bool ApproachTo(this float target, float value, double precision = 1E-05f)
        {
            return Math.Abs(target - value) <= precision;
        }
    }
}