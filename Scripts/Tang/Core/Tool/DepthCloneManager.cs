using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace Tang
{
    public class DepthCloneManager
    {
        public static DepthCloneManager instance;

        public static DepthCloneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DepthCloneManager();
                }
                return instance;
            }
        }

        private Dictionary<Type, ConstructorInfo[]> _constructorInfoses  = new Dictionary<Type, ConstructorInfo[]>();
        private Dictionary<Type, ConstructorInfo> _constructorInfo  = new Dictionary<Type, ConstructorInfo>();
        private Dictionary<Type, FieldInfo[]> _fieldInfoses  = new Dictionary<Type, FieldInfo[]>();
        private Dictionary<string, FieldInfo> _fieldInfos = new Dictionary<string, FieldInfo>();
        private Dictionary<FieldInfo, object> _fieldInfoAttributeFirstOrDefaultDic = new Dictionary<FieldInfo, object>();

        public ConstructorInfo[] GetConstructorInfoes(Type type,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            ConstructorInfo[] ret;
            if (_constructorInfoses.TryGetValue(type, out ret))
            {
            }
            else
            {
                ret = type.GetConstructors(bindingFlags);
                _constructorInfoses.Add(type, ret);
            }
            return ret;
        }

        public FieldInfo[] GetFields(Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            FieldInfo[] ret;
            if (_fieldInfoses.TryGetValue(type, out ret))
            {
            }
            else
            {
                ret = type.GetFields(bindingFlags);
                _fieldInfoses.Add(type, ret);
            }
            return ret;
        }

        public FieldInfo GetField(Type type, string fieldName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            string key = type.FullName + fieldName;
            FieldInfo ret;
            if (_fieldInfos.TryGetValue(key, out ret))
            {
            }
            else
            {
                ret = type.GetField(fieldName, bindingFlags);
                _fieldInfos.Add(key, ret);
            }
            return ret;
        }

        public object GetFirstOrDefaultAttribute(FieldInfo fieldInfo, Type attributeType, bool inherit = false)
        {
            object ret;
            if (_fieldInfoAttributeFirstOrDefaultDic.TryGetValue(fieldInfo, out ret))
            {
            }
            else
            {
                ret = fieldInfo.GetCustomAttributes(typeof(NonSerializedAttribute), false).FirstOrDefault();
                _fieldInfoAttributeFirstOrDefaultDic.Add(fieldInfo, ret);
            }
            return ret;
        }

        public object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public T DepthClone<T>(Object sourceObj) where T : class
        {
            Type sourceType = sourceObj.GetType();

            object targetObj = CreateInstance(sourceType);

            CloneWork(sourceType, sourceObj, targetObj);
            
            return (T)targetObj;
        }

        public void CloneWork(Type sourceType, object sourceObj, object targetObj)
        {
            FieldInfo[] fieldInfos = GetFields(sourceType);
            
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];
                if (fieldInfo == null)
                {
                    Console.WriteLine(fieldInfo.Name + " 没找到");
                    continue;
                }
                
                // 不需要序列化的对象 add by TangJian 2019/5/10 21:44
                if(GetFirstOrDefaultAttribute(fieldInfo, typeof(NonSerializedAttribute)) is NonSerializedAttribute)
                {
                    
                }
                else
                {
                    if (fieldInfo.FieldType.IsClass && fieldInfo.FieldType.FullName != "System.String")
                    {
                        var _src = fieldInfo.GetValue(sourceObj);
                        if (_src != null)
                        {
                            object _des = null;
                            if (_src is IList srcList)
                            {
                                IList destList = Activator.CreateInstance(fieldInfo.FieldType, srcList.Count) as IList;
                                Debug.Assert(destList != null);
                                        
                                for (int j = 0; j < srcList.Count; j++)
                                {
                                    destList.Add(DepthClone<object>(srcList[j]));
                                }

                                _des = destList;
                            }
                            else
                            {
                                _des = CreateInstance(fieldInfo.FieldType);
                                // 拷贝类 add by TangJian 2019/5/10 22:12
                                CloneWork(_src.GetType(), _src, _des);
                            }
                                    
                            fieldInfo.SetValue(targetObj, _des);
                        }
                        else
                        {
                            fieldInfo.SetValue(targetObj, null);
                        }
                    }
                    else
                    {
                        // 拷贝基本数据类型 add by TangJian 2019/5/10 22:12
                        fieldInfo.SetValue(targetObj, fieldInfo.GetValue(sourceObj));   
                    }
                }
            }
        }
    }
}