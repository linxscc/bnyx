using System.Collections.Generic;

namespace Tang
{
    public static class ListExtend
    {
        public static void AddList<T>(this List<T> target, List<T> list)
        {
            foreach (var item in list)
            {
                target.Add(item);
            }
        }
        public static void AddArray<T>(this List<T> target, T[] array)
        {
            foreach (var item in array)
            {
                target.Add(item);
            }
        }
        public static bool TryGet<T>(this List<T> target, int index, out T o)
        {
            if (target.CanFindIndex(index))
            {
                o = target[index];
                return true;
            }
            o = default(T);
            return false;
        }

        public static bool CanFindIndex<T>(this List<T> target, int index)
        {
            if (index >= 0 && target.Count > index)
            {
                return true;
            }
            return false;
        }

        public static List<T> MyToList<T>(this List<object> target) where T : class
        {
            List<T> retList = new List<T>();
            foreach (var item in target)
            {
                var retItem = item as T;
                if (retItem != null)
                {
                    retList.Add(retItem);
                }
            }
            return retList;
        }

        public static List<T> MyToList<T>(this object[] target) where T : class
        {
            List<T> retList = new List<T>();
            foreach (var item in target)
            {
                var retItem = item as T;
                if (retItem != null)
                {
                    retList.Add(retItem);
                }
            }
            return retList;
        }
    }
}