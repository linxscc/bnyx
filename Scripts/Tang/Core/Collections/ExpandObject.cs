using System.Collections.Generic;
using System.Dynamic;


namespace Tang
{
    public class ExpandObject : Dictionary<string, object>
    {
        public void SetValue<T>(string name, T value)
        {
            this[name] = value;
        }

        public T GetObj<T>(string name, T defaultValue)
        {
            T t;
            if (TryGetValue<T>(name, out t))
            {
                return t;
            }

            return defaultValue;
        }

        public bool TryGetValue<T>(string name, out T value)
        {
            value = default(T);
            object outValue = null;
            if (base.TryGetValue(name, out outValue))
            {
                value = (T)outValue;
                return true;
            }
            return false;
        }

        public void SetObj<T>(string name, T obj)
        {
            this[name] = obj;
        }

        public bool TryGetObj<T>(string name, out T value) where T : class
        {
            value = default(T);
            object outValue = null;
            if (base.TryGetValue(name, out outValue))
            {
                value = outValue as T;
                return true;
            }
            return false;
        }
    }



    public class MyDynamicObject : DynamicObject
    {
        // The inner dictionary.
        private Dictionary<string, object> _Values = new Dictionary<string, object>();

        // Getting a property.
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_Values.TryGetValue(binder.Name, out result))
            {
                return true;
            }
            else
            {
                result = null;
                return true;
            }
        }

        // Setting a property.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _Values[binder.Name] = value;
            return true;
        }

        public bool Contains(string key)
        {
            return _Values.ContainsKey(key);
        }

        public Dictionary<string, object> Values
        {
            get { return this._Values; }
        }
    }
}