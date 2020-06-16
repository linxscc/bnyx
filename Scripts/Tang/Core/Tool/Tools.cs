using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Reflection;

using System.Runtime.InteropServices;
using System.IO;
using DeepCopier;
using Newtonsoft.Json.Linq;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Tang
{
    public class Tools
    {
        private static int onlyId = 0;
        public static int getOnlyId()
        {
            return ++ onlyId;
        }

        public static void Destroy(UnityEngine.Object obj, float delayTime = 0)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Object.Destroy(obj, delayTime);
            }
            else
            {
                Object.DestroyImmediate(obj, true);
            }
#else
            Object.Destroy(obj, delayTime);            
#endif
        }

        public static T GetComponent<T>(GameObject gameObject, bool autoAdd) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                if (autoAdd)
                    component = gameObject.AddComponent<T>();
            }
            return component;
        }

        // 添加组件 add by TangJian 2017/07/18 16:05:36
        public static T AddComponent<T>(GameObject gameObject) where T : Component
        {
            RemoveComponent<T>(gameObject);
            return gameObject.AddComponent<T>();
        }

        // 移除
        public static void RemoveComponent<T>(GameObject gameObject) where T : Component
        {
            var components = gameObject.GetComponents<T>();
            foreach (var item in components)
            {
                Destroy(item);
            }
        }

        // 权重随机 add by TangJian 2019/3/8 16:09
        public static int RandomWithWeight(params int[] weights)
        {
            return RandomWithWeight<int>(weights.ToList(), (i, i1) => { return i; });
        }
        
        // 权重随机 add by TangJian 2019/3/8 16:09
        public static int RandomWithWeight(params float[] weights)
        {
            return RandomWithWeight<float>(weights.ToList(), (i, i1) => { return Mathf.FloorToInt(i); });
        }
        
        // 根据权重随机 add by TangJian 2017/08/03 20:22:12
        public static int RandomWithWeight<T>(List<T> list, System.Func<T, int, int> getWeightFunc)
        {
            if (list.Count == 1)
                return 0;

            bool isZeroList = true;

            int totalWeight = 0;
            List<int> weightList = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                int weight = getWeightFunc(item, i) * 1000;
                weightList.Add(weight);
                totalWeight += weight;

                if (weight > 0)
                {
                    isZeroList = false;
                }
            }

            if (isZeroList == false)
            {
                int randomValue = Random.Range(0, totalWeight + 1);
                for (int i = 0; i < weightList.Count; i++)
                {
                    randomValue -= weightList[i];
                    if (randomValue <= 0)
                    {
                        return i;
                    }
                }
            }

            // 没有结果 .. 
            Debug.Log("没有结果 .. ");
            return -1;
        }
        
        // 获得一个矩形区域内的一个随机位置 add by TangJian 2017/08/03 21:00:52
        public static Vector3 RandomPositionInCube(Vector3 position, Vector3 size)
        {
            var x = Random.Range(position.x - size.x / 2, position.x + size.x / 2);
            var y = Random.Range(position.y - size.y / 2, position.y + size.y / 2);
            var z = Random.Range(position.z - size.z / 2, position.z + size.z / 2);
            return new Vector3(x, y, z);
        }

        // 得到一个随机的Vector3 add by TangJian 2017/08/25 19:59:02
        public static Vector3 getRandomVector3(float scale, float minX = -1, float maxX = 1, float minY = -1, float maxY = 1, float minZ = -1, float maxZ = 1)
        {
            minX *= scale;
            maxX *= scale;

            minY *= scale;
            maxY *= scale;

            minZ *= scale;
            maxZ *= scale;

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            float z = Random.Range(minZ, maxZ); 

            return new Vector3(x, y, z);
        }

        public static List<KeyAction> currKeyActionList = new List<KeyAction>();
        // 得到按键状态 add by TangJian 2017/08/28 18:08:20

        public static JObject Obj2JObj<T>(T obj, bool ReferenceLoopHandlingIgnore = true)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            if (ReferenceLoopHandlingIgnore)
            {
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }
            return JObject.FromObject(obj, JsonSerializer.Create(settings));
        }

        public static string Obj2Json<T>(T obj, bool ReferenceLoopHandlingIgnore = true)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            if (ReferenceLoopHandlingIgnore)
            {
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }
            return Obj2Json<T>(obj, settings);
        }
        // 对象转Json add by TangJian 2017/08/21 20:23:36
        public static string Obj2Json<T>(T obj, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
        }

        public static T Json2Obj<T>(string jsonString, bool ReferenceLoopHandlingIgnore = true)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (ReferenceLoopHandlingIgnore)
            {
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }
            return JsonConvert.DeserializeObject<T>(jsonString, settings);
        }
        
        // json转对象 add by TangJian 2017/08/21 20:23:45
        public static T Json2Obj<T>(string jsonString, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(jsonString, settings);
        }

        public static string ResourcesPathToAbsolutePath(string resourcesPath)
        {
            return Application.dataPath + "/Resources/" + resourcesPath;
        }

        public static int GetLayerMask(List<string> layerNames)
        {
            int layerMask = 1;
            for (int i = 0; i < layerNames.Count; i++)
            {
                var layerName = layerNames[i];
                var layerIndex = LayerMask.NameToLayer(layerName);
                if (i == 0)
                {
                    layerMask = 1 << layerIndex;
                }
                else
                {
                    layerMask = layerMask | (1 << layerIndex);
                }
            }
            return layerMask;
        }

        public static GameObject GetChild(GameObject gameObject, string name, bool autoAdd = false)
        {
            var childTransform = gameObject.transform.Find(name);
            GameObject child = null;
            if (childTransform != null)
            {
                child = childTransform.gameObject;
            }
            else
            {
                if (autoAdd)
                {
                    child = new GameObject(name);
                    child.transform.parent = gameObject.transform;
                    child.transform.localPosition = Vector3.zero;
                }
            }
            return child;
        }


        // selection
        public static GameObject GetFirstSelectGameObject()
        {
#if UNITY_EDITOR
            if (Selection.gameObjects.Length > 0)
            {
                return Selection.gameObjects[0];
            }
#endif
            return null;
        }

        // 击中打印模块 add by TangJian 2017/09/29 17:42:12
        static string logString = "";
        public static void AddLog(string log)
        {
            logString += log;
        }
        public static void printLog()
        {
            Debug.Log(logString);
            logString = "";
        }


        // 赋值 add by TangJian 2017/09/30 16:32:23
        public static bool Assign<T>(out T o, T curr, T set)
        {
            bool ret = false;
            if (curr.Equals(set))
            {
                ret = false;
            }
            else
            {
                ret = true;
            }
            o = set;
            return ret;
        }

        // 使list长度等于count add by TangJian 2017/10/09 15:47:03
        public static void SetListCount<T>(List<T> list, int count, System.Func<T> newFunc)
        {
            if (list.Count == count)
            {
                return;
            }
            else if (list.Count < count)
            {
                for (int i = 0; i < count - list.Count; i++)
                {
                    list.Add(newFunc());
                }
            }
            else
            {
                list.RemoveRange(count, list.Count - count);
            }
        }

        public static void SetListCount<T>(List<T> list, int count) where T : new()
        {
            SetListCount(list, count, () => new T());
        }

        public static List<T> CloneList<T>(List<T> list) where T : System.ICloneable
        {
            List<T> ret = new List<T>();
            foreach (var item in list)
            {
                ret.Add((T)item.Clone());
            }
            return ret;
        }

        // 更新预制体 add by TangJian 2017/12/05 00:17:08
        public static GameObject UpdatePrefab(GameObject gameObject, string prefabPath)
        {
#if UNITY_EDITOR
            Object tmpPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
            if (tmpPrefab == null)
            {
                tmpPrefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
            }
            var ret = Tools.ReplacePrefab(gameObject, tmpPrefab, ReplacePrefabOptions.ReplaceNameBased);
            AssetDatabase.RenameAsset(prefabPath, Path.GetFileNameWithoutExtension(prefabPath)); // 处理windows下预制体名字大小写的问题 add by TangJian 2017/12/05 15:16:16
            return ret;
#else
    return null;
#endif
        }

        public static Object GetPrefabRoot(GameObject gameObject)
        {
#if UNITY_EDITOR
            Object parent = null;
            PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
            switch (prefabType)
            {
                case PrefabType.None:
                case PrefabType.Prefab:
                    parent = gameObject;
                    break;
                case PrefabType.DisconnectedPrefabInstance:
                case PrefabType.PrefabInstance:
                    parent = PrefabUtility.GetPrefabParent(gameObject);
                    break;
            }
            return parent;
#else
        return null;
#endif
        }

        // 获得预制体路径
        public static string GetPrefabPath(Object obj, bool withoutFileName = false)
        {
#if UNITY_EDITOR
            Object parent = null;
            PrefabType prefabType = PrefabUtility.GetPrefabType(obj);
            switch (prefabType)
            {
                // case PrefabType.None:
                case PrefabType.Prefab:
                    parent = obj;
                    break;
                case PrefabType.DisconnectedPrefabInstance:
                    parent = PrefabUtility.GetPrefabParent(obj);
                    break;
                case PrefabType.PrefabInstance:
                    parent = PrefabUtility.GetPrefabParent(obj);
                    break;
                default:
                    return null;
                // break;
            }
            if (parent)
            {
                string path = AssetDatabase.GetAssetPath(parent);
                if (withoutFileName == true)
                {
                    path = Path.GetDirectoryName(path);
                }
                return path;
            }
            return null;
#else
            return null;
#endif
        }


#if UNITY_EDITOR
        // 替换预制体 add by TangJian 2017/12/05 00:11:16
        public static GameObject ReplacePrefab(GameObject gameObject, Object targetObject, ReplacePrefabOptions replacePrefabOptions = ReplacePrefabOptions.ReplaceNameBased)
        {
            Object parent = null;
            PrefabType prefabType = PrefabUtility.GetPrefabType(targetObject);
            switch (prefabType)
            {
                case PrefabType.None:
                case PrefabType.Prefab:
                    parent = targetObject;
                    break;
                case PrefabType.DisconnectedPrefabInstance:
                case PrefabType.PrefabInstance:
                    parent = PrefabUtility.GetPrefabParent(gameObject);
                    break;
            }
            return PrefabUtility.ReplacePrefab(gameObject, parent, replacePrefabOptions);
        }
#endif

#if UNITY_EDITOR
        // 替换预制体 add by TangJian 2017/12/05 00:11:16
        public static void ReplacePrefab(string path, GameObject gameObject, ReplacePrefabOptions replacePrefabOptions = ReplacePrefabOptions.ReplaceNameBased)
        {
            GameObject targetObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            PrefabType prefabType = PrefabUtility.GetPrefabType(targetObject);
            if (prefabType == PrefabType.Prefab)
            {
                PrefabUtility.ReplacePrefab(gameObject, targetObject, replacePrefabOptions);
            }
            Debug.LogError("ReplacePrefab");
        }
#endif

        // 修改prefab add by TangJian 2017/10/11 23:52:27
        public static void ModifyPrefab(GameObject gameObject, System.Action<GameObject> modifyFunc = null)
        {
#if UNITY_EDITOR
            GameObject instance = null;
            Object parent = null;
            PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
            switch (prefabType)
            {
                case PrefabType.Prefab:
                    instance = PrefabUtility.InstantiatePrefab(gameObject) as GameObject;
                    parent = gameObject;
                    break;
                case PrefabType.DisconnectedPrefabInstance:
                case PrefabType.PrefabInstance:
                    instance = gameObject;
                    parent = PrefabUtility.GetPrefabParent(gameObject);
                    break;
            }

            if (instance != null && parent != null)
            {
                try
                {
                    if (modifyFunc != null)
                        modifyFunc(instance);

                    PrefabUtility.ReplacePrefab(instance, parent, ReplacePrefabOptions.ConnectToPrefab);
                }
                finally
                {
                    if (prefabType == PrefabType.Prefab)
                        Destroy(instance);
                }
            }
#endif
        }

        public static float Range(float value, float min, float max)
        {
            value = Mathf.Max(value, min);
            value = Mathf.Min(value, max);
            return value;
        }

        public static string GetMemoryAddress(object o) // 获取引用类型的内存地址方法    
        {
            GCHandle h = GCHandle.Alloc(o, GCHandleType.WeakTrackResurrection);

            System.IntPtr addr = GCHandle.ToIntPtr(h);

            return "0x" + addr.ToString("X");
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        public static float AngleMirror(float angle)
        {
            // if (angle > 0 && angle <= 90)
            // {
            //     angle = 90 + (90 - angle);
            // }
            // else if (angle > 90 && angle <= 180)
            // {
            //     angle = 90 - (angle - 90);
            // }
            // else if (angle > 180 && angle <= 270)
            // {
            //     angle = 270 + (270 - angle);
            // }
            // else if (angle > 270 && angle <= 360)
            // {
            //     angle = 270 - (angle - 270);
            // }
            return 180 - angle;
        }

        public static string ReadStringFromFile(string fileName)
        {
            if (File.Exists(fileName))
                return File.ReadAllText(fileName);
            else
                return null;
        }

        public static void WriteStringFromFile(string fileName, string contents)
        {
            if(fileName != null)
                File.WriteAllText(fileName, contents);
        }

        // // 查找目录下的文件 add by TangJian 2017/11/17 15:41:43
        // public static IList<string> GetFilesInDir(string dirPath)
        // {

        // }

        public static List<string> GetFileListInFolder(string folder, string suffix = null)
        {
            List<string> files = new List<string>();
            WalkFiles((string fileName) =>
            {
                files.Add(fileName);
            }, folder, suffix);
            return files;
        }

        public static void WalkFiles(System.Action<string> action, string folder, string suffix = null)
        {
            foreach (string path in Directory.GetFiles(folder))
            {
                if (suffix == null ||
                    Path.GetExtension(path) == suffix)
                {
                    action(GetStandardPath(path));
                }
            }

            if (Directory.GetDirectories(folder).Length > 0)  //遍历所有文件夹  
            {
                foreach (string path in Directory.GetDirectories(folder))
                {
                    WalkFiles(action, path, suffix);
                }
            }
        }

        public static List<string> GetFilesInFolder(string folder, string suffix = null)
        {
            List<string> files = new List<string>();
            GetFilesInFolder(folder, (string path) =>
            {
                return suffix == null || Path.GetExtension(path) == suffix;
            }, ref files);
            return files;
        }

        private static void GetFilesInFolder(string folder, System.Func<string, bool> shouldCollect, ref List<string> files)
        {
            foreach (string path in Directory.GetFiles(folder))
            {
                if (shouldCollect(path))
                {
                    files.Add(path);
                }
            }

            if (Directory.GetDirectories(folder).Length > 0)  //遍历所有文件夹  
            {
                foreach (string path in Directory.GetDirectories(folder))
                {
                    GetFilesInFolder(path, shouldCollect, ref files);
                }
            }
        }

        public static List<string> GetFoldersInFolder(string folder, int depthMin = 2, int depthMax = 2)
        {
            List<string> folders = new List<string>();

            RecursiveFolder(folder, (string path, int depth) =>
            {
                folders.Add(path);
            }, 0, depthMin, depthMax);

            return folders;
        }

        private static void RecursiveFolder(string folder, System.Action<string, int> action, int depth = 0, int depthMin = 0, int depthMax = 99)
        {
            depth++;

            if (Directory.GetDirectories(folder).Length > 0)  //遍历所有文件夹  
            {
                foreach (string path in Directory.GetDirectories(folder))
                {
                    RecursiveFolder(path, action, depth, depthMin, depthMax);
                }

                if (depth >= depthMin && depth <= depthMax)
                    action(folder, depth);
            }

            depth--;
        }

        public static string GetStandardPath(string path)
        {
            return path.Replace('\\', '/').Replace("//", "/");
        }

        // public static List<GameObject> GetAllPrefabInFolder(string folder)
        // {
        //     List<GameObject> prefabList = new List<GameObject>();

        //     List<string> filePaths = Tools.GetFilesInFolder(folder, ".prefab");
        //     foreach (var filePath in filePaths)
        //     {
        //         var fileName = Tools.GetStandardPath(filePath);
        //         var pos = fileName.IndexOf("Resources/") + "Resources/".Length;
        //         fileName = fileName.Substring(pos);
        //         fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
        //         GameObject gameObject = Resources.Load<GameObject>(fileName);
        //         prefabList.Add(gameObject);
        //     }
        //     return prefabList;
        // }
        public static List<GameObject> GetAllPrefabInFolder(string folder)
        {
            List<GameObject> prefabList = new List<GameObject>();

            List<string> filePaths = Tools.GetFilesInFolder(folder, ".prefab");
            foreach (var filePath in filePaths)
            {
                var fileName = SysPathToAssetPath(Tools.GetStandardPath(filePath));
                GameObject gameObject = AssetManager.LoadAssetAtPath<GameObject>(fileName);
                prefabList.Add(gameObject);
            }
            return prefabList;
        }

#if UNITY_EDITOR

        public static List<System.Collections.Generic.KeyValuePair<string, GameObject>> GetAllPrefabAndPathInFolder(string folder)
        {
            List<System.Collections.Generic.KeyValuePair<string, GameObject>> prefabList = new List<System.Collections.Generic.KeyValuePair<string, GameObject>>();

            List<string> filePaths = Tools.GetFilesInFolder(folder, ".prefab");
            foreach (var filePath in filePaths)
            {
                var fileName = SysPathToAssetPath(Tools.GetStandardPath(filePath));
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(fileName);

                System.Collections.Generic.KeyValuePair<string, GameObject> pair = new System.Collections.Generic.KeyValuePair<string, GameObject>(fileName, gameObject);
                prefabList.Add(pair);
            }
            return prefabList;
        }
#endif

        public static void CreateFolder(string path)
        {
            path = GetStandardPath(path);

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '/')
                {
                    string path_ = path.Substring(0, i);
                    Debug.Log("创建目录: " + path_);
                    Directory.CreateDirectory(path_);
                }
            }
            Debug.Log("创建目录: " + path);
            Directory.CreateDirectory(path);
        }

        public static void CreateAssetFolder(string path)
        {
            path = Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")) + "/" + path;

            path = GetStandardPath(path);

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '/')
                {
                    Directory.CreateDirectory(path.Substring(0, i));
                }
            }
            Directory.CreateDirectory(path);
        }

        public static string AssetPathToSysPath(string assetPath)
        {
            return Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")) + "/" + assetPath;
        }

        public static string SysPathToAssetPath(string sysPath)
        {
            return GetStandardPath(sysPath.Substring(Application.dataPath.IndexOf("Assets")));
        }

        public static void testst(GameObject gameObject)
        {
            MeshFilter mf = gameObject.GetComponent<MeshFilter>();

            //顶点数组转顶点容器
            List<Vector3> verticeList = new List<Vector3>();
            int verticeCount = mf.mesh.vertices.Length;
            for (int verticeIndex = 0; verticeIndex < verticeCount; ++verticeIndex)
            {
                verticeList.Add(mf.mesh.vertices[verticeIndex]);
            }
            //三角形数组转三角形容器
            List<int> triangleList = new List<int>();
            int triangleCount = mf.mesh.triangles.Length;
            for (int triangleIndex = 0; triangleIndex < triangleCount; ++triangleIndex)
            {
                triangleList.Add(mf.mesh.triangles[triangleIndex]);
            }
            //uv坐标数组转uv坐标容器
            List<Vector2> uvList = new List<Vector2>();
            int uvCount = mf.mesh.uv.Length;
            for (int uvIndex = 0; uvIndex < uvCount; ++uvIndex)
            {
                uvList.Add(mf.mesh.uv[uvIndex]);
            }
            //顶点颜色数组转顶点颜色容器
            List<Vector3> normalList = new List<Vector3>();
            int normalCount = mf.mesh.normals.Length;
            for (int normalIndex = 0; normalIndex < normalCount; ++normalIndex)
            {
                normalList.Add(mf.mesh.normals[normalIndex]);
            }

            //检查每个三角面，是否存在两个顶点连接正好在直线上
            for (int triangleIndex = 0; triangleIndex < triangleList.Count;)
            {
                int trianglePoint0 = triangleList[triangleIndex];
                int trianglePoint1 = triangleList[triangleIndex + 1];
                int trianglePoint2 = triangleList[triangleIndex + 2];

                Vector3 point0 = verticeList[trianglePoint0];
                Vector3 point1 = verticeList[trianglePoint1];
                Vector3 point2 = verticeList[trianglePoint2];

                float planeY = 0.3f;
                //0-1，1-2相连线段被切割
                if ((point0.y - planeY) * (point1.y - planeY) < 0 && (point1.y - planeY) * (point2.y - planeY) < 0)
                {
                    //截断0-1之间的顶点
                    float k01 = (point1.y - point0.y) / (planeY - point0.y);
                    float newPointX01 = (point1.x - point0.x) / k01 + point0.x;
                    float newPointZ01 = (point1.z - point0.z) / k01 + point0.z;
                    Vector3 newPoint0_1 = new Vector3(newPointX01, planeY, newPointZ01);
                    verticeList.Add(newPoint0_1);
                    //uv
                    if (uvList.Count > 0)
                    {
                        Vector2 uv0 = uvList[trianglePoint0];
                        Vector2 uv1 = uvList[trianglePoint1];
                        float newUV_x = (uv1.x - uv0.x) / k01 + uv0.x;
                        float newUV_y = (uv1.y - uv0.y) / k01 + uv0.y;
                        uvList.Add(new Vector2(newUV_x, newUV_y));
                    }
                    //法向量
                    Vector3 normalX0 = normalList[trianglePoint0];
                    Vector3 normalX1 = normalList[trianglePoint1];
                    Vector3 normalX2 = normalList[trianglePoint2];
                    float newNoramlX01 = (normalX1.x - normalX0.x) / k01 + normalX0.x;
                    float newNoramlY01 = (normalX1.y - normalX0.y) / k01 + normalX0.y;
                    float newNoramlZ01 = (normalX1.z - normalX0.z) / k01 + normalX0.z;
                    normalList.Add(new Vector3(newNoramlX01, newNoramlY01, newNoramlZ01));
                    //截断1-2之间的顶点
                    float k12 = (point2.y - point1.y) / (planeY - point1.y);
                    float newPointX12 = (point2.x - point1.x) / k12 + point1.x;
                    float newPointZ12 = (point2.z - point1.z) / k12 + point1.z;
                    Vector3 newPoint1_2 = new Vector3(newPointX12, planeY, newPointZ12);
                    verticeList.Add(newPoint1_2);
                    if (uvList.Count > 0)
                    {
                        Vector2 uv1 = uvList[trianglePoint1];
                        Vector2 uv2 = uvList[trianglePoint2];
                        float newUV_x = (uv2.x - uv1.x) / k12 + uv1.x;
                        float newUV_y = (uv2.y - uv1.y) / k12 + uv1.y;
                        uvList.Add(new Vector2(newUV_x, newUV_y));
                    }
                    //法向量
                    float newNoramlX12 = (normalX2.x - normalX1.x) / k12 + normalX1.x;
                    float newNoramlY12 = (normalX2.y - normalX1.y) / k12 + normalX1.y;
                    float newNoramlZ12 = (normalX2.z - normalX1.z) / k12 + normalX1.z;
                    normalList.Add(new Vector3(newNoramlX12, newNoramlY12, newNoramlZ12));

                    int newVerticeCount = verticeList.Count;
                    //插入顶点索引，以此构建新三角形
                    triangleList.Insert(triangleIndex + 1, newVerticeCount - 2);
                    triangleList.Insert(triangleIndex + 2, newVerticeCount - 1);

                    triangleList.Insert(triangleIndex + 3, newVerticeCount - 1);
                    triangleList.Insert(triangleIndex + 4, newVerticeCount - 2);

                    triangleList.Insert(triangleIndex + 6, trianglePoint0);
                    triangleList.Insert(triangleIndex + 7, newVerticeCount - 1);
                }
                //1-2，2-0相连线段被切割
                else if ((point1.y - planeY) * (point2.y - planeY) < 0 && (point2.y - planeY) * (point0.y - planeY) < 0)
                {
                    //截断1-2之间的顶点
                    float k12 = (point2.y - point1.y) / (planeY - point1.y);
                    float newPointX12 = (point2.x - point1.x) / k12 + point1.x;
                    float newPointZ12 = (point2.z - point1.z) / k12 + point1.z;
                    Vector3 newPoint1_2 = new Vector3(newPointX12, planeY, newPointZ12);
                    verticeList.Add(newPoint1_2);
                    if (uvList.Count > 0)
                    {
                        Vector2 uv1 = uvList[trianglePoint1];
                        Vector2 uv2 = uvList[trianglePoint2];
                        float newUV_x = (uv2.x - uv1.x) / k12 + uv1.x;
                        float newUV_y = (uv2.y - uv1.y) / k12 + uv1.y;
                        uvList.Add(new Vector2(newUV_x, newUV_y));
                    }
                    //法向量
                    Vector3 normalX0 = normalList[trianglePoint0];
                    Vector3 normalX1 = normalList[trianglePoint1];
                    Vector3 normalX2 = normalList[trianglePoint2];
                    float newNoramlX12 = (normalX2.x - normalX1.x) / k12 + normalX1.x;
                    float newNoramlY12 = (normalX2.y - normalX1.y) / k12 + normalX1.y;
                    float newNoramlZ12 = (normalX2.z - normalX1.z) / k12 + normalX1.z;
                    normalList.Add(new Vector3(newNoramlX12, newNoramlY12, newNoramlZ12));

                    //截断0-2之间的顶点
                    float k02 = (point2.y - point0.y) / (planeY - point0.y);
                    float newPointX02 = (point2.x - point0.x) / k02 + point0.x;
                    float newPointZ02 = (point2.z - point0.z) / k02 + point0.z;
                    Vector3 newPoint0_2 = new Vector3(newPointX02, planeY, newPointZ02);
                    verticeList.Add(newPoint0_2);
                    //uv
                    if (uvList.Count > 0)
                    {
                        Vector2 uv0 = uvList[trianglePoint0];
                        Vector2 uv2 = uvList[trianglePoint2];
                        float newUV_x = (uv2.x - uv0.x) / k02 + uv0.x;
                        float newUV_y = (uv2.y - uv0.y) / k02 + uv0.y;
                        uvList.Add(new Vector2(newUV_x, newUV_y));
                    }
                    //法向量
                    float newNoramlX02 = (normalX1.x - normalX0.x) / k02 + normalX0.x;
                    float newNoramlY02 = (normalX1.y - normalX0.y) / k02 + normalX0.y;
                    float newNoramlZ02 = (normalX1.z - normalX0.z) / k02 + normalX0.z;
                    normalList.Add(new Vector3(newNoramlX02, newNoramlY02, newNoramlZ02));

                    int newVerticeCount = verticeList.Count;
                    //插入顶点索引，以此构建新三角形

                    //{0}
                    //{1}
                    triangleList.Insert(triangleIndex + 2, newVerticeCount - 2);

                    triangleList.Insert(triangleIndex + 3, newVerticeCount - 1);
                    triangleList.Insert(triangleIndex + 4, newVerticeCount - 2);
                    //{2}

                    triangleList.Insert(triangleIndex + 6, newVerticeCount - 1);
                    triangleList.Insert(triangleIndex + 7, trianglePoint0);
                    triangleList.Insert(triangleIndex + 8, newVerticeCount - 2);
                }
                //0-1，2-0相连线段被切割
                else if ((point0.y - planeY) * (point1.y - planeY) < 0 && (point2.y - planeY) * (point0.y - planeY) < 0)
                {
                    //截断0-1之间的顶点
                    float k01 = (point1.y - point0.y) / (planeY - point0.y);
                    float newPointX01 = (point1.x - point0.x) / k01 + point0.x;
                    float newPointZ01 = (point1.z - point0.z) / k01 + point0.z;
                    Vector3 newPoint0_1 = new Vector3(newPointX01, planeY, newPointZ01);
                    verticeList.Add(newPoint0_1);
                    //uv
                    if (uvList.Count > 0)
                    {
                        Vector2 uv0 = uvList[trianglePoint0];
                        Vector2 uv1 = uvList[trianglePoint1];
                        float newUV_x = (uv1.x - uv0.x) / k01 + uv0.x;
                        float newUV_y = (uv1.y - uv0.y) / k01 + uv0.y;
                        uvList.Add(new Vector2(newUV_x, newUV_y));
                    }
                    //法向量
                    Vector3 normalX0 = normalList[trianglePoint0];
                    Vector3 normalX1 = normalList[trianglePoint1];
                    Vector3 normalX2 = normalList[trianglePoint2];
                    float newNoramlX01 = (normalX1.x - normalX0.x) / k01 + normalX0.x;
                    float newNoramlY01 = (normalX1.y - normalX0.y) / k01 + normalX0.y;
                    float newNoramlZ01 = (normalX1.z - normalX0.z) / k01 + normalX0.z;
                    normalList.Add(new Vector3(newNoramlX01, newNoramlY01, newNoramlZ01));

                    //截断0-2之间的顶点
                    float k02 = (point2.y - point0.y) / (planeY - point0.y);
                    float newPointX02 = (point2.x - point0.x) / k02 + point0.x;
                    float newPointZ02 = (point2.z - point0.z) / k02 + point0.z;
                    Vector3 newPoint0_2 = new Vector3(newPointX02, planeY, newPointZ02);
                    verticeList.Add(newPoint0_2);
                    //uv
                    if (uvList.Count > 0)
                    {
                        Vector2 uv0 = uvList[trianglePoint0];
                        Vector2 uv2 = uvList[trianglePoint2];
                        float newUV_x = (uv2.x - uv0.x) / k02 + uv0.x;
                        float newUV_y = (uv2.y - uv0.y) / k02 + uv0.y;
                        uvList.Add(new Vector2(newUV_x, newUV_y));
                    }
                    //法向量
                    float newNoramlX02 = (normalX1.x - normalX0.x) / k02 + normalX0.x;
                    float newNoramlY02 = (normalX1.y - normalX0.y) / k02 + normalX0.y;
                    float newNoramlZ02 = (normalX1.z - normalX0.z) / k02 + normalX0.z;
                    normalList.Add(new Vector3(newNoramlX02, newNoramlY02, newNoramlZ02));

                    int newVerticeCount = verticeList.Count;
                    //插入顶点索引，以此构建新三角形

                    //{0}
                    triangleList.Insert(triangleIndex + 1, newVerticeCount - 2);
                    triangleList.Insert(triangleIndex + 2, newVerticeCount - 1);

                    triangleList.Insert(triangleIndex + 3, newVerticeCount - 2);
                    //{1}
                    //{2}

                    triangleList.Insert(triangleIndex + 6, trianglePoint2);
                    triangleList.Insert(triangleIndex + 7, newVerticeCount - 1);
                    triangleList.Insert(triangleIndex + 8, newVerticeCount - 2);
                }
                //只有0-1被切
                else if ((point0.y - planeY) * (point1.y - planeY) < 0)
                {
                    Debug.Log("只有01被切");
                }
                //只有1-2被切
                else if ((point1.y - planeY) * (point2.y - planeY) < 0)
                {
                    Debug.Log("只有12被切");
                }
                //只有2-0被切
                else if ((point2.y - planeY) * (point0.y - planeY) < 0)
                {
                    Debug.Log("只有02被切");
                }
                triangleIndex += 3;
            }

            //筛选出切割面两侧的顶点索引
            List<int> triangles1 = new List<int>();
            List<int> triangles2 = new List<int>();
            for (int triangleIndex = 0; triangleIndex < triangleList.Count; triangleIndex += 3)
            {
                int trianglePoint0 = triangleList[triangleIndex];
                int trianglePoint1 = triangleList[triangleIndex + 1];
                int trianglePoint2 = triangleList[triangleIndex + 2];

                Vector3 point0 = verticeList[trianglePoint0];
                Vector3 point1 = verticeList[trianglePoint1];
                Vector3 point2 = verticeList[trianglePoint2];
                //切割面
                float planeY = 0.3f;
                if (point0.y > planeY || point1.y > planeY || point2.y > planeY)
                {
                    triangles1.Add(trianglePoint0);
                    triangles1.Add(trianglePoint1);
                    triangles1.Add(trianglePoint2);
                }
                else
                {
                    triangles2.Add(trianglePoint0);
                    triangles2.Add(trianglePoint1);
                    triangles2.Add(trianglePoint2);
                }
            }

            //缝合切口
            //for (int verticeIndex = verticeCount; verticeIndex < verticeList.Count - 2; ++verticeIndex)
            //{
            //    triangles1.Add(verticeIndex + 2);
            //    triangles1.Add(verticeIndex);
            //    triangles1.Add(verticeCount);

            //    triangles2.Add(verticeCount);
            //    triangles2.Add(verticeIndex);
            //    triangles2.Add(verticeIndex + 2);
            //}


            mf.mesh.vertices = verticeList.ToArray();
            mf.mesh.triangles = triangles1.ToArray();
            if (uvList.Count > 0)
            {
                mf.mesh.uv = uvList.ToArray();
            }
            mf.mesh.normals = normalList.ToArray();

            //分割模型
            GameObject newModel = new GameObject("New Model");
            MeshFilter meshFilter = newModel.AddComponent<MeshFilter>();
            meshFilter.mesh.vertices = mf.mesh.vertices;
            meshFilter.mesh.triangles = triangles2.ToArray();
            meshFilter.mesh.uv = mf.mesh.uv;
            meshFilter.mesh.normals = mf.mesh.normals;
            Renderer newRenderer = newModel.AddComponent<MeshRenderer>();
            newRenderer.material = gameObject.GetComponent<MeshRenderer>().material;
        }

        public static T RandomObject<T>(params T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static float GetDegree(Vector2 pos)
        {
            float degree = 0;
            if (pos.x > 0)
            {
                degree = Mathf.Atan2(pos.y, pos.x);
            }
            else if (pos.x < 0)
            {
                degree = Mathf.Atan2(pos.y, pos.x);
            }
            else
            {
            }
            degree = degree / Mathf.PI * 180;
            return degree;
        }

        public static float GetDegree(float x, float y)
        {
            return GetDegree(new Vector2(x, y));
        }
        public static Vector3 BezierCurve(Vector3 P0, Vector3 P1, Vector3 P2, float t)
        {
            Vector3 B = Vector3.zero;
            float t1 = (1 - t) * (1 - t);
            float t2 = t * (1 - t);
            float t3 = t * t;
            B = P0 * t1 + 2 * t2 * P1 + t3 * P2;
            //B.y = P0.y*t1 + 2*t2*P1.y + t3*P2.y;  
            //B.z = P0.z*t1 + 2*t2*P1.z + t3*P2.z;  
            return B;

        }

        public static T SerializeDepthClone<T>(T obj) where T : class, new()
        {
            if (obj == null)
                return null;
            
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            return (T)formatter.Deserialize(ms);
        }

        public static byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;
            
            //内存实例  
            MemoryStream ms = new MemoryStream();
            //创建序列化的实例  
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);//序列化对象，写入ms流中    
            byte[] bytes = ms.GetBuffer();
            return bytes;
        }
        public static object DeserializeObject(byte[] bytes)
        {
            object obj = null;
            if (bytes == null)
                return obj;
            //利用传来的byte[]创建一个内存流  
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            obj = formatter.Deserialize(ms);//把内存流反序列成对象    
            ms.Close();
            return obj;
        }

        sealed class Vector3SerializationSurrogate : System.Runtime.Serialization.ISerializationSurrogate
        {

            // Method called to serialize a Vector3 object
            public void GetObjectData(System.Object obj,
                System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            {

                Vector3 v3 = (Vector3)obj;
                info.AddValue("x", v3.x);
                info.AddValue("y", v3.y);
                info.AddValue("z", v3.z);
                // Debug.Log(v3);
            }

            // Method called to deserialize a Vector3 object
            public System.Object SetObjectData(System.Object obj,
                System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context,
                System.Runtime.Serialization.ISurrogateSelector selector)
            {

                Vector3 v3 = (Vector3)obj;
                v3.x = (float)info.GetValue("x", typeof(float));
                v3.y = (float)info.GetValue("y", typeof(float));
                v3.z = (float)info.GetValue("z", typeof(float));
                obj = v3;
                return obj;   // Formatters ignore this return value //Seems to have been fixed!
            }
        }
        // 寻路路径根据距离切片
        public static List<Vector3> GetPathList(Vector3[] PathList, float distance)
        {
            List<Vector3> newpathlist = new List<Vector3>();
            if (PathList != null && PathList.Length != 0)
            {
                for (int i = 1; i < PathList.Length; i++)
                {
                    float Pathdistance = Vector3.Magnitude(PathList[i - 1] - PathList[i]);
                    int num = (int)Mathf.Floor(Pathdistance / distance);
                    if (Pathdistance > distance)
                    {
                        List<Vector3> LinePointList = GetLinePointList(PathList[i - 1], PathList[i], num);
                        foreach (var item in LinePointList)
                        {
                            newpathlist.Add(item);
                        }
                    }
                    else
                    {
                        newpathlist.Add(PathList[i]);
                    }
                }
            }
            return newpathlist;
        }
        public static List<Vector3> GetLinePointList(Vector3 beginPos, Vector3 endPos, int num)
        {
            List<Vector3> list = new List<Vector3>();
            for (int i = 0; i < num; i++)
            {
                Vector3 retPos = GetLinePoint(beginPos, endPos, num, i);
                list.Add(retPos);
            }
            return list;
        }
        public static Vector3 GetLinePoint(Vector3 beginPos, Vector3 endPos, int num, int index)
        {
            Vector3 offset = endPos - beginPos;
            return beginPos + (offset / num) * (index);
        }
        
        public static T DepthClone<T>(T t) where T : class
        {
            return Copier.Copy<T>(t);
        }

        public static Mesh SaveMesh(string meshName, Mesh mesh)
        {
#if UNITY_EDITOR
            Mesh asset = UnityEngine.Object.Instantiate<Mesh>(mesh);
            asset.name = meshName;
            AssetDatabase.CreateAsset(asset, meshName + ".asset");
            AssetDatabase.SaveAssets();
            return (Mesh)AssetDatabase.LoadAssetAtPath(meshName + ".asset", typeof(Mesh));
#else
        return null;
#endif
        }

        public static float Vradian(Vector3 from, Vector3 to)
        {
            float radian;
            float x = from.x - to.x;
            float longl = (from - to).magnitude;
            float cos = x / longl;
            radian = Mathf.Acos(cos) + Mathf.PI;

            return radian;
        }
        public static GameObject CreateCube(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float h, Material backWall, Material sideWall, Material bottom)
        {
            GameObject cube = new GameObject();

            MeshFilter meshFilter = cube.AddComponentUnique<MeshFilter>();
            MeshRenderer meshRenderer = cube.AddComponentUnique<MeshRenderer>();

            {
                // 上
                Vector3 p4 = p0 + new Vector3(0, -h, 0);
                Vector3 p5 = p1 + new Vector3(0, -h, 0);
                Vector3 p6 = p2 + new Vector3(0, -h, 0);
                Vector3 p7 = p3 + new Vector3(0, -h, 0);

                // 前
                Vector3 P8 = p4;
                Vector3 p9 = p5;
                Vector3 p10 = p1;
                Vector3 p11 = p0;

                // 后
                Vector3 p12 = p7;
                Vector3 p13 = p6;
                Vector3 p14 = p2;
                Vector3 p15 = p3;

                // 左
                Vector3 p16 = p7;
                Vector3 p17 = p4;
                Vector3 p18 = p0;
                Vector3 p19 = p3;

                // 右
                Vector3 p20 = p6;
                Vector3 p21 = p5;
                Vector3 p22 = p1;
                Vector3 p23 = p2;

                Mesh mesh = new Mesh();

                mesh.vertices = new Vector3[24]{
                    p0, p1, p2, p3,
                    p4, p5, p6, p7,

                    P8, p9, p10, p11,
                    p12, p13, p14, p15,

                    p16, p17, p18, p19,
                    p20, p21, p22, p23,

                };

                mesh.subMeshCount = 3;

                // 前后
                mesh.SetTriangles(new int[3 * 2 * 2] {
                    8, 10, 9,
                    8, 11, 10,

                    12, 13, 14,
                    12, 14, 15,
                }, 0);

                // 左右
                mesh.SetTriangles(new int[3 * 2 * 2] {
                    16, 18, 17,
                    16, 19, 18,

                    20, 21, 22,
                    20, 22, 23,
                }, 1);

                // 上下
                // mesh.SetTriangles(new int[3 * 2 * 2] {
                //         0, 2, 1,
                //         0, 3, 2,

                //         4, 5, 6,
                //         4, 6, 7,
                //     }, 2);


                // 前后
                // mesh.SetTriangles(new int[3 * 2 * 2] {
                //     8, 10, 9,
                //     8, 11, 10,
                //     12, 13, 14,
                //     12, 14, 15,
                // }, 0);

                // mesh.triangles = new int[3 * 2 * 6]
                // {
                //         0, 2, 1,
                //         0, 3, 2,
                //         4, 5, 6,
                //         4, 6, 7,

                //         8, 10, 9,
                //         8, 11, 10,
                //         12, 13, 14,
                //         12, 14, 15,

                //         16, 18, 17,
                //         16, 19, 18,
                //         20, 22, 21,
                //         20, 23, 22,
                // };

                mesh.uv = new Vector2[]
                {
                    // 下
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),

                    // 上
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),

                    //前
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),

                    //后
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),

                    //左
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),

                    //右
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                };


                Color[] colors = new Color[24];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = Color.white;
                }
                mesh.colors = colors;

                Vector3[] normals = new Vector3[24];
                for (int i = 0; i < normals.Length; i++)
                {
                    normals[i] = new Vector3(1, 1, 1);
                }
                mesh.normals = normals;

                meshFilter.mesh = mesh;
            }

            Material[] materials = new Material[] {
                backWall,
                sideWall,
                // bottom,
            };

            meshRenderer.materials = materials;

            return cube;
        }

        public static void TTT(GameObject gameObject)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            // Debug.Log("meshFilter.sharedMesh.subMeshCount = " + meshFilter.sharedMesh.subMeshCount);
            // Debug.Log("meshRenderer.sharedMaterials.Length = " + meshRenderer.sharedMaterials.Length);


            List<Material> materialList = new List<Material>();
            foreach (var item in meshRenderer.sharedMaterials)
            {
                item.name = item.name.Replace("(Instance)", "").Replace(" ", "");

                int findIndex = materialList.FindIndex((Material m) =>
                {
                    if (m.name == item.name)
                    {
                        return true;
                    }
                    return false;
                });

                if (findIndex >= 0)
                {
                }
                else
                {
                    materialList.Add(item);
                }
            }

            // Debug.Log("meshRenderer.sharedMaterials.Length = " + meshRenderer.sharedMaterials.Length);
            // Debug.Log("materialSet.Count = " + materialList.Count);

            List<List<int>> trianglesList = new List<List<int>>();
            foreach (var item in materialList)
            {
                trianglesList.Add(new List<int>());
            }

            // 重新分配
            for (int i = 0; i < meshFilter.sharedMesh.subMeshCount; i++)
            {
                string materialName = meshRenderer.sharedMaterials[i].name;

                for (int j = 0; j < materialList.Count; j++)
                {
                    if (materialList[j].name == materialName)
                    {
                        trianglesList[j].AddRange(meshFilter.sharedMesh.GetTriangles(i).ToList<int>());
                    }
                }
            }

            for (int i = 0; i < trianglesList.Count; i++)
            {
                meshFilter.sharedMesh.SetTriangles(trianglesList[i].ToArray(), i);
            }

            meshFilter.sharedMesh.subMeshCount = materialList.Count;
            meshRenderer.sharedMaterials = materialList.ToArray();
        }


        // 合并模型 add by TangJian 2018/04/25 19:51:36
        // public static CombineModel(GameObject parent)
        // {
        //     //获取MeshRender;  
        //     MeshRenderer[] meshRenders = parent.GetComponentsInChildren<MeshRenderer>();

        //     //材质;  
        //     Material[] mats = new Material[meshRenders.Length];
        //     for (int i = 0; i < meshRenders.Length; i++)
        //     {
        //         mats[i] = meshRenders[i].sharedMaterial;
        //     }

        //     //合并Mesh;  
        //     MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();

        //     CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        //     for (int i = 0; i < meshFilters.Length; i++)
        //     {
        //         combine[i].mesh = meshFilters[i].sharedMesh;
        //         combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        //         meshFilters[i].gameObject.SetActive(false);
        //     }

        //     MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        //     MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        //     mf.mesh = new Mesh();
        //     mf.mesh.CombineMeshes(combine, false);
        //     gameObject.SetActive(true);
        //     mr.sharedMaterials = mats;
        // }


        public static T Load<T>(string path) where T : new()
        {
            string jsonStr = Tools.ReadStringFromFile(path);
            if (jsonStr == null || jsonStr == "")
            {
                return default(T);
            }
            else
            {
                T data = Tools.Json2Obj<T>(jsonStr);
                if (data == null)
                {
                    data = default(T);
                }
                return data;
            }
        }

        public static void Save<T>(T data, string path) where T : new()
        {
            string jsonStr = Tools.Obj2Json<T>(data);
            if (jsonStr != null && jsonStr != "")
            {
                Tools.WriteStringFromFile(path, jsonStr);
            }
        }


        public Vector3 V3RotateAround(Vector3 source, Vector3 axis, float angle)
        {
            Quaternion q = Quaternion.AngleAxis(angle, axis);// 旋转系数
            return q * source;// 返回目标点
        }

        public static void SaveTextureToFile(Texture2D texture, string fileName)
        {
            var bytes = texture.EncodeToPNG();
            var file = File.Open(fileName, FileMode.Create);
            var binary = new BinaryWriter(file);
            binary.Write(bytes);
            file.Close();
        }




        public static Vector3 GetFlyToPosSpeed(Vector2 speedNormal, Vector3 targetPos, float g = -60f)
        {
            Vector3 speed = speedNormal;
            Vector3 targetPos_ = targetPos.RotateFromTo(new Vector3(targetPos.x, 0, targetPos.z), new Vector3(1, 0, 0));

            float b = speed.y / speed.x;
            float a = (-(b * targetPos_.x) / (targetPos_.x * targetPos_.x));

            float x1 = (-b - (float)Mathf.Sqrt(b * b)) / (2f * a);
            float midX = x1 / 2f;
            float maxY = a * midX * midX + b * midX;

            speed.x = Mathf.Sqrt((-(g * x1) / b) / 2f);

            speed.y = speed.x * b;

            //Debug.Log("a = " + a);
            //Debug.Log("b = " + b);
            //Debug.Log("x1 = " + x1);
            //Debug.Log("speed = " + speed);

            return speed.RotateFromTo(new Vector3(1, 0, 0), new Vector3(targetPos.x, 0, targetPos.z));
        }

        public static bool TryGetFlyToPosSpeed(Vector2 speedNormal, Vector3 targetPos, out Vector3 speed, float g = -60f)
        {
            speed = speedNormal;
            Vector3 targetPos_ = targetPos.RotateFromTo(new Vector3(targetPos.x, 0, targetPos.z), new Vector3(1, 0, 0));

            float b = speed.y / speed.x;
            float a = -((targetPos_.x - targetPos_.y) / (targetPos_.x * targetPos_.x));

            if (b >= 1 || g >= 0)
            {
                return false;
            }

            float x1 = (-b - (float)Mathf.Sqrt(b * b)) / (2 * a);
            float midX = x1 / 2f;
            float maxY = a * midX * midX + b * midX;

            speed.x = Mathf.Sqrt((-(g * x1) / b) / 2f);

            speed.y = speed.x * b;

            //Debug.Log("a = " + a);
            //Debug.Log("b = " + b);
            //Debug.Log("x1 = " + x1);
            //Debug.Log("speed = " + speed);

            speed = speed.RotateFromTo(new Vector3(1, 0, 0), new Vector3(targetPos.x, 0, targetPos.z));
            return true;
        }

        public static void ExecuteCoroutine(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            { }
        }
        
        public static int GetTimestamp()
        {
//            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return System.DateTime.Now.Millisecond;
        }
        
        public static void SaveOneData<T>(Dictionary<string ,T> keyValuePairs,string path)
        {
            Dictionary<string, string> pathDic = new Dictionary<string, string>();
            List<string> pathlist = new List<string>();
            pathlist = Tools.GetFileListInFolder(path, ".json");
            foreach (var pathitem in pathlist)
            {
                string name = Path.GetFileNameWithoutExtension(pathitem);
                pathDic.Add(name, pathitem);
            }
            foreach (var item in keyValuePairs)
            {
                bool Write = true;
                string tojsonstring = Tools.Obj2Json(item.Value, true);
                if (pathDic.ContainsKey(item.Key))
                {
                    string jsonst = Tools.ReadStringFromFile(pathDic[item.Key]);
                    if (jsonst == tojsonstring)
                    {
                        Write = false;
                    }
                }
                if (Write)
                {
                    Tools.WriteStringFromFile(path +"/"+ item.Key + ".json", tojsonstring);
                }
            }
        }
        public static Dictionary<string ,T> LoadOneData<T>(string path)
        {
            Dictionary<string, T> keyValuePairs = new Dictionary<string, T>();
            List<string> pathlist = new List<string>();
            
#if UNITY_EDITOR

            pathlist = Tools.GetFileListInFolder(path, ".json");
            T Data;
            foreach (var item in pathlist)
            {
                string jsonString = Tools.ReadStringFromFile(item);
                Data = Tools.Json2Obj<T>(jsonString, true);
                if (Data != null)
                {
                    keyValuePairs.Add(Path.GetFileNameWithoutExtension(item), Data);
                }
            }

#else

            string key = "Resources";
            int beginIndex = path.IndexOf(key);
            int endIndex = path.Length - 1;

            beginIndex = beginIndex < 0 ? 0 : beginIndex + key.Length + 1;
            endIndex = endIndex < 0 ? path.Length - 1 : endIndex;

            string newpath = path.Substring(beginIndex, endIndex - beginIndex);
            TextAsset[] objects = Resources.LoadAll<TextAsset>(newpath);
            T Data;
            foreach (var item in objects)
            {
                string jsonString = item.text;
                Data = Tools.Json2Obj<T>(jsonString, true);
                if(Data != null)
                {
                    keyValuePairs.Add(item.name, Data);
                }
            }

#endif
            return keyValuePairs;
        }
        
        public static void NewAnim(GameObject anim,SkillData skillData)
        {
            Animator animator = anim.GetComponent<Animator>();
            Spine.Unity.SkeletonAnimator skeletonAnimator = anim.GetComponent<Spine.Unity.SkeletonAnimator>();

            var skeletonAnimatorRenderer = skeletonAnimator.GetComponent<Renderer>();
            skeletonAnimatorRenderer.enabled = false;

            animator.runtimeAnimatorController =
                AssetManager.LoadAssetAtPath<RuntimeAnimatorController>(skillData.AnimControllerPath);

            skeletonAnimator.skeletonDataAsset =
                AssetManager.LoadAssetAtPath<Spine.Unity.SkeletonDataAsset>(skillData.SkeletonDataAssetPath);

            skeletonAnimator.initialSkinName = skillData.SkinName;
            skeletonAnimator.Initialize(true);
        }
    }


    public class TestJsonConverter : JsonConverter
    {
        public List<System.Type> typeList;

        public override bool CanConvert(System.Type objectType)
        {
            for (int i = 0; i < typeList.Count; i++)
            {
                System.Type type = typeList[i];
                if (type.IsAssignableFrom(objectType))
                    return true;
            }
            return false;
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var constructor = objectType.GetConstructor(new System.Type[] { });

            var value = constructor.Invoke(new object[] { });
            if (value == null)
            {
                throw new JsonSerializationException("No object created.");
            }

            serializer.Populate(reader, value);
            return value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }
        
        
    }
}