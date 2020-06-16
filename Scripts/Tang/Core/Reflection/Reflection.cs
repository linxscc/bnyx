using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR

#endif

namespace Tang
{
    public class Reflection
    {
        static Reflection instance;
        static public Reflection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Reflection();
                }
                return instance;
            }
        }

        BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        Dictionary<string, string> baseTypeMap = new Dictionary<string, string>()
        {
            {"Camera", "UnityEngine.Camera"},
#if UNITY_EDITOR
            {"EditorWindow", "UnityEditor.EditorWindow"},
            {"EditorUtility", "UnityEditor.EditorUtility"},
            {"Editor", "UnityEditor.Editor"},
            {"SceneViewPicking", "UnityEditor.SceneViewPicking"},
            {"StageNavigationManager", "UnityEditor.SceneManagement.StageNavigationManager"},            
            {"PrefabUtility", "UnityEditor.PrefabUtility"},            
#endif
        };

        Dictionary<string, Assembly> assemblyMap = new Dictionary<string, Assembly>();
        Dictionary<string, Type> typeMap = new Dictionary<string, Type>();
        Dictionary<string, MethodInfo> methodInfoMap = new Dictionary<string, MethodInfo>();
        Dictionary<string, PropertyInfo> propertyInfoyMap = new Dictionary<string, PropertyInfo>();
        Dictionary<string, FieldInfo> fieldInfoyMap = new Dictionary<string, FieldInfo>();

        // 添加类型 add by TangJian 2017/10/09 15:35:19
        public bool AddType(Type type)
        {
            if (typeMap.ContainsKey(type.Name) == false)
            {
                typeMap.Add(type.Name, type);
                return true;
            }
            return false;
        }

        Reflection()
        {
//            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly != null)
                {
                    try
                    {
                        // 初始化assemblyMap
                        foreach (var item in baseTypeMap)
                        {
                            Type t = assembly.GetType(item.Value);
                            if (t != null)
                            {
                                assemblyMap[item.Key] = Assembly.GetAssembly(t);
                                Debug.Log("Reflection Success: " + item.Key);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }

            // 初始化反射类型
            foreach (var item in assemblyMap)
            {
                foreach (var type in item.Value.GetTypes())
                {
                    typeMap[type.Name] = type;
                }
            }
        }

        public Type GetTypeWithName(string typeName)
        {
            Type type;
            if (typeMap.TryGetValue(typeName, out type))
            {
                return type;
            }
            return null;
        }

        public MethodInfo GetMethodInfo(string typeName, string methodName)
        {
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            MethodInfo methodInfo;
            methodInfoMap.TryGetValue(typeName + methodName, out methodInfo);
            if (methodInfo == null)
            {
                Type type = GetTypeWithName(typeName);
                if (type != null)
                {
                    methodInfo = type.GetMethods(flags).Where(t => t.Name == methodName).FirstOrDefault();
                    methodInfoMap.Add(typeName + methodName, methodInfo);
                }
            }
            if (methodInfo == null)
                Debug.Log("没有方法 typeName = " + typeName + ", methodName = " + methodName);
            return methodInfo;
        }

        public PropertyInfo GetPropertyInfo(string typeName, string propertyName)
        {
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            PropertyInfo property = null;
            propertyInfoyMap.TryGetValue(typeName + propertyName, out property);
            if (property == null)
            {
                Type type = GetTypeWithName(typeName);
                if (type != null)
                {
                    property = type.GetProperties(flags).Where(t => t.Name == propertyName).FirstOrDefault();
                    propertyInfoyMap.Add(typeName + propertyName, property);
                }
            }
            if (property == null)
                Debug.Log("没有方法 typeName = " + typeName + ", propertyName = " + propertyName);
            return property;
        }

        public object Invoke(string typeName, string funcName, object obj, object[] parameters)
        {
            var methodInfo = GetMethodInfo(typeName, funcName);
            if (methodInfo != null)
            {
                return methodInfo.Invoke(obj, parameters);
            }
            return null;
        }

        public object GetPropertyInfoValue(string typeName, string propertyName, object obj, object[] parameters)
        {
            var propertyInfo = GetPropertyInfo(typeName, propertyName);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj, parameters);
            }
            return null;
        }

        public PropertyInfo[] GetPropertyInfos(string typeName, Type propertyType, object obj, object[] parameters)
        {
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
            Type type = GetTypeWithName(typeName);
            if (type != null)
            {
                propertyInfoList = type.GetProperties(flags).Where(t => t.GetType() == propertyType).ToList();
            }

            return propertyInfoList.ToArray();
        }

        public FieldInfo GetFieldInfo(string typeName, string fieldInfoName)
        {
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            FieldInfo fieldInfo = null;
            fieldInfoyMap.TryGetValue(typeName + fieldInfoName, out fieldInfo);
            if (fieldInfo == null)
            {
                Type type = GetTypeWithName(typeName);
                if (type != null)
                {
                    fieldInfo = type.GetFields(flags).Where(t => t.Name == fieldInfoName).FirstOrDefault();
                    fieldInfoyMap.Add(typeName + fieldInfoName, fieldInfo);
                }
            }
            if (fieldInfo == null)
                Debug.Log("没有方法 typeName = " + typeName + ", propertyName = " + fieldInfoName);
            return fieldInfo;
        }

        public object GetFieldInfoValue(string typeName, string fieldInfoName, object obj)
        {
            var fieldInfo = GetFieldInfo(typeName, fieldInfoName);
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }
            return null;
        }

        public void SetFieldInfoValue(string typeName, string fieldInfoName, object obj, object value)
        {
            var fieldInfo = GetFieldInfo(typeName, fieldInfoName);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
            }
        }
    }
}