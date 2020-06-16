using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    [System.Serializable]
    public class Properties : MonoBehaviour
    {
        public List<PropertyString> propertyStrings = new List<PropertyString>();
        public List<PropertyFloat> propertyFloats = new List<PropertyFloat>();        

        public void addFloat(string key, float value)
        {
            propertyFloats.Add(new PropertyFloat(key, value));
        }
        public float getFloat(string key)
        {
            foreach (var property in propertyFloats)
            {
                if (key == property.key)
                {
                    return property.value;
                }
            }
            return 0;
        }
        public void addString(string key, string value)
        {
            propertyStrings.Add(new PropertyString(key, value));
        }
        public string getString(string key)
        {
            foreach (var property in propertyStrings)
            {
                if (key == property.key)
                {
                    return property.value;
                }
            }
            return "";
        }
    }
}