


using System.Collections.Generic;


namespace Tang
{
    public class Cache
    {
        Dictionary<int, object> dic;

        public Cache()
        {
            dic = new Dictionary<int, object>();
        }

        public void Clear()
        {
            dic.Clear();
        }

        public bool TryGet(int hashCode, out object o)
        {
            return dic.TryGetValue(hashCode, out o);
        }

        public void Set(int hashCode, object o)
        {
            dic.Add(hashCode, o);
        }
    }
}