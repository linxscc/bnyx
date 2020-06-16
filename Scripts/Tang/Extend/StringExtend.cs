
using System.Collections.Generic;


namespace Tang
{
    public static class StringExtend
    {
        public static bool IsValid(this string target)
        {
            return target != null && target != "";
        }

        public static List<string> SplitRetainSeparator(this string target, char separator)
        {
            string[] strArray = target.Split(separator);
            List<string> strList = new List<string>();
            for (int i = 0; i < strArray.Length; i++)
            {
                strList.Add(strArray[i]);

                if (i < strArray.Length - 1)
                    strList.Add(separator.ToString());
            }
            return strList;
        }
    }
}