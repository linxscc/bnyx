using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class DataMap
    {
        Dictionary<string, object> objDic = new Dictionary<string, object>();

        public object this[string key] { get { return objDic[key]; } set { objDic[key] = value; } }

        public object getObj(string key)
        {
            return objDic[key];
        }
        public string getString(string key)
        {
            var ret = objDic[key];
            return ret as string;
        }
        public int getInt(string key)
        {
            var obj = getObj(key);
            if (obj != null)
            {
                if (obj.GetType() == typeof(int))
                {
                    return (int)obj;
                }
            }
            return 0;
        }
        public float getFloat(string key)
        {
            var obj = getObj(key);
            if (obj != null)
            {
                if (obj.GetType() == typeof(float))
                {
                    return (float)obj;
                }
            }
            return 0;
        }
        public Vector3 getVector3(string key)
        {
            var obj = getObj(key);
            if (obj != null)
            {
                if (obj.GetType() == typeof(Vector3))
                {
                    return (Vector3)obj;
                }
            }
            return new Vector3();
        }

        public void addObject(string key, object obj)
        {
            objDic.Add(key, obj);
        }
        public void addString(string key, string str)
        {
            addObject(key, str);
        }
        public void addInt(string key, int i)
        {
            addObject(key, i);
        }
        public void addFloat(string key, float f)
        {
            addObject(key, f);
        }
        public void addVector3(string key, Vector3 vec3)
        {
            addObject(key, vec3);
        }
    }
}