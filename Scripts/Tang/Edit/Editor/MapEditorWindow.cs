using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tang.Editor
{
    public class TTTTTT : ScriptableObject
    {
        public string aaa = "adfadsfasdf";
        public List<string> strList = new List<string>();
    }

    public class MapEditorWindow : EditorWindow
    {
        [MenuItem("Window/地图编辑器")]
        static void Init()
        {
            MapEditorWindow window = (MapEditorWindow)EditorWindow.GetWindow(typeof(MapEditorWindow));
            window.Show();
            window.titleContent = new GUIContent("地图编辑器");
        }

        SceneController currSceneController
        {
            set
            {
                MapEditorManager.CurrSceneController = value;
            }
            get
            {
                return MapEditorManager.CurrSceneController;
            }
        }

        int currSelectedSceneIndex
        {
            set
            {
                MapEditorManager.CurrSelectedSceneIndex = value;
            }
            get
            {
                return MapEditorManager.CurrSelectedSceneIndex;
            }
        }

        bool isEditing
        {
            set
            {
                MapEditorManager.IsEditing = value;
            }
            get
            {
                return MapEditorManager.IsEditing;
            }
        }

        ValueMonitorPool valueMonitorPool;

        void OnEnable()
        {
            valueMonitorPool = new ValueMonitorPool();
            valueMonitorPool.AddMonitor<bool>(() =>
                {
                    return isEditing;
                },
                (bool from, bool to) =>
                {
                    if (to)
                    {
                        SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.RemoveAll(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUICallback_Move));
                        SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUICallback_Move));
                    }
                    else
                    {
                        SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.RemoveAll(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUICallback_Move));
                    }
                });

            EditorUpdateRunner.AddUpdateIfNot("MapEditorManager", () =>
            {
                EditorUpdate();
            });
        }

        void OnDisable()
        {
        }

        float gravity = 30f;

        void OnGUI()
        {
            if (MyGUI.Button(isEditing ? "编辑中" : "未编辑"))
            {
                isEditing = !isEditing;
            }

            // 当前编辑的场景 add by TangJian 2018/11/21 11:51
            {

                List<SceneController> sceneControllers = MapEditorManager.GetCurrSceneControllerList();
                string[] sceneOptions = new string[sceneControllers.Count];
                for (int i = 0; i < sceneControllers.Count; i++)
                {
                    sceneOptions[i] = sceneControllers[i].name;
                }
                currSelectedSceneIndex = MyGUI.PopupWithTitle("当前编辑场景", currSelectedSceneIndex, sceneOptions);

                if (sceneControllers.CanFindIndex(currSelectedSceneIndex))
                    currSceneController = sceneControllers[currSelectedSceneIndex];
            }

            if (MyGUI.Button("计算角度"))
            {
                // Camera camera = Camera.main;

                // Vector3 beginScreenVec3 = new Vector3(0, 0);
                // Vector3 endScreenVec3 = new Vector3(100, 100);

                // Vector3 endWorldVec3 = camera.ScreenToWorldPoint(endScreenVec3);
                Debug.Log(Mathf.Atan(2) / Mathf.PI * 180);
                Debug.Log(Mathf.Atan(Mathf.Sqrt(3)) / Mathf.PI * 180);

                // Debug.Log(Mathf.Atan(2) / Mathf.PI * 180);


                // Debug.Log();
            }

            gravity = MyGUI.FloatFieldWithTitle("gravity", gravity);

            if (MyGUI.Button("Test"))
            {

                var dict = Tools.GetFileListInFolder("Assets");
                
                List<string> retList = new List<string>();

                object suo = new object();
                
                retList.Clear();
                int begin = Tools.GetTimestamp();
                dict.AsParallel().ForAll(pair =>
                {
                    for (int i = 0; i < 100000; i++)
                    {
                    }
                });
                
                Debug.Log("duration1 = " + (Tools.GetTimestamp() - begin).ToString());
                
                retList.Clear();
                begin = Tools.GetTimestamp();
                Parallel.ForEach(dict, pair =>
                {
                    for (int i = 0; i < 100000; i++)
                    {
                    }
                });
                
                Debug.Log("duration2 = " + (Tools.GetTimestamp() - begin).ToString());
                
                retList.Clear();
                begin = Tools.GetTimestamp();
                foreach (var pair in dict)
                {
                    for (int i = 0; i < 100000; i++)
                    {
                    }
                }
                
                Debug.Log("duration3 = " + (Tools.GetTimestamp() - begin).ToString());
            }

            valueMonitorPool.Update();
        }

        void EditorUpdate()
        {
            if (MapEditorManager.NeedRepaint)
            {
                MapEditorManager.NeedRepaint = false;
                Repaint();
            }
        }

        void SceneComponentUnDoMoveBy(GameObject[] gos, int x, int y, int z)
        {
            foreach (var item in gos)
            {
                SceneComponent terrainComponent = item.GetComponent<SceneComponent>();
                if (terrainComponent != null)
                {
                    Undo.RecordObject(terrainComponent, "UnDo" + terrainComponent.name);
                    terrainComponent.MoveByGridPos(new Vector3(x, y, z));
                }
            }
            // sceneView.LookAt(Selection.activeGameObject.transform.position);
        }

        void SceneComponentUnDoOffsetBy(GameObject[] gos, int x, int y, int z)
        {
            foreach (var item in gos)
            {
                SceneComponent terrainComponent = item.GetComponent<SceneComponent>();
                if (terrainComponent != null)
                {
                    Undo.RecordObject(terrainComponent, "UnDo" + terrainComponent.name);
                    terrainComponent.Offset += new Vector3(x, y, z) * 0.01f;
                    terrainComponent.MoveByGridPos(Vector3.zero);
                }
            }
            // sceneView.LookAt(Selection.activeGameObject.transform.position);
        }

        bool MouseTest(Vector2 mousePoint, out GameObject pointGameObject, int layer)
        {
            pointGameObject = null;

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePoint);

            Vector2 size = new Vector2(100, 100);
            GameObject[] gameObjects = HandleUtility.PickRectObjects(new Rect(0, 0, Screen.width, Screen.height), false);
            float distance = 99999999;
            foreach (var go in gameObjects)
            {
                if (go.layer == layer)
                {
                    Collider collider = go.GetComponent<Collider>();
                    if (collider != null)
                    {
                        RaycastHit hitInfo;
                        if (collider.Raycast(ray, out hitInfo, distance))
                        {
                            distance = hitInfo.distance;
                            pointGameObject = go;
                        }
                    }
                }
            }
            return pointGameObject != null;
        }

        int popupIndex = 0;

        // 地面编辑模式 add by TangJian 2018/02/28 16:29:11
        public void OnSceneGUICallback_Move(SceneView sceneView)
        {
            UnityEngine.Event e = UnityEngine.Event.current;
            // Debug.Log("e.type = " + e.type);

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            // 选中场景组件
            {
                if (e.type == UnityEngine.EventType.MouseDown)
                {
                    if (e.button == 0)
                    {
                        GameObject activeGameObject1;
                        if (MouseTest(e.mousePosition, out activeGameObject1, LayerMask.NameToLayer("SceneComponent")))
                        {
                            if (e.control)
                            {

                                List<UnityEngine.Object> objects = Selection.objects.ToList();

                                int index = objects.FindIndex((UnityEngine.Object o) =>
                                {
                                    return o.GetInstanceID() == activeGameObject1.GetInstanceID();
                                });

                                if (index < 0)
                                {
                                    objects.Add(activeGameObject1);
                                }
                                else
                                {
                                    objects.RemoveAt(index);
                                }

                                Selection.objects = objects.ToArray();
                                // Selection.activeGameObject = hitInfo.collider.gameObject
                            }
                            else
                            {
                                //Debug.Log(hitInfo.collider.gameObject.name);
                                Selection.activeGameObject = null;
                                Selection.activeGameObject = activeGameObject1;
                                // sceneView.LookAt(Selection.activeGameObject.GetColliderBounds().center);
                            }
                        }
                        e.Use();
                    }
                    return;
                }
            }


            var activeGameObject = Selection.activeGameObject;
            if (activeGameObject != null)
            {
                if (activeGameObject.layer == LayerMask.NameToLayer("SceneComponent"))// activeGameObject.tag == "Ground" || activeGameObject.tag == "BackWall" || activeGameObject.tag == "FrontWall")
                {
                    // 锁定位置 add by TangJian 2018/02/28 16:40:31
                    {
                        // 在鼠标不按下, 而且按下的不是右键的情况下 add by TangJian 2018/10/25 22:18
                        if (e.type != UnityEngine.EventType.MouseDown && e.button != 1)
                        {
                            sceneView.orthographic = true;
                            sceneView.rotation = Quaternion.Euler(30, 0, 0);

                            // 调整摄像机视距 add by TangJian 2018/10/25 22:17
                            sceneView.camera.farClipPlane = 1000;
                            sceneView.camera.nearClipPlane = -1000;
                        }

                        // sceneView.LookAt(activeGameObject.transform.position);
                    }

                    switch (e.type)
                    {
                        //case UnityEngine.EventType.ExecuteCommand:
                        //    break;
                        case UnityEngine.EventType.KeyDown:
                            if (!e.control)
                            {
                                switch (e.keyCode)
                                {
                                    case KeyCode.Q:
                                        SceneComponentUnDoMoveBy(Selection.gameObjects, 0, -1, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.E:
                                        SceneComponentUnDoMoveBy(Selection.gameObjects, 0, 1, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.UpArrow:
                                    case KeyCode.W:
                                        SceneComponentUnDoMoveBy(Selection.gameObjects, 0, 0, 1);
                                        e.Use();
                                        break;
                                    case KeyCode.DownArrow:
                                    case KeyCode.S:
                                        SceneComponentUnDoMoveBy(Selection.gameObjects, 0, 0, -1);
                                        e.Use();
                                        break;
                                    case KeyCode.LeftArrow:
                                    case KeyCode.A:
                                        SceneComponentUnDoMoveBy(Selection.gameObjects, -1, 0, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.RightArrow:
                                    case KeyCode.D:
                                        SceneComponentUnDoMoveBy(Selection.gameObjects, 1, 0, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.T:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, 0, 0, 100);
                                        e.Use();
                                        break;
                                    case KeyCode.G:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, 0, 0, -100);
                                        e.Use();
                                        break;
                                    case KeyCode.F:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, -100, 0, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.H:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, 100, 0, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.R:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, 0, -100, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.Y:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, 0, 100, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.I:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, 0, 0, 1);
                                        e.Use();
                                        break;
                                    case KeyCode.K:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, 0, 0, -1);
                                        e.Use();
                                        break;
                                    case KeyCode.J:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, -1, 0, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.L:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, 1, 0, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.U:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, 0, -1, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.O:
                                        SceneComponentUnDoOffsetBy(Selection.gameObjects, 0, 1, 0);
                                        e.Use();
                                        break;
                                    case KeyCode.Delete:
                                        foreach (var item in Selection.gameObjects)
                                        {
                                            Undo.DestroyObjectImmediate(item);
                                        }
                                        e.Use();
                                        break;
                                    case KeyCode.Z: // 找到下方可以摆放的组件的位置 add by TangJian 2018/8/16 15:40
                                        ScenePlacementComponent scenePlacementComponent = Selection.activeGameObject.GetComponent<ScenePlacementComponent>();
                                        if (scenePlacementComponent != null)
                                        {
                                            var scenePlacementColliderBounds = scenePlacementComponent.gameObject.GetChild("Collider").GetColliderBounds();
                                            Ray ray = new Ray();
                                            ray.origin = new Vector3(scenePlacementColliderBounds.center.x, scenePlacementColliderBounds.min.y, scenePlacementColliderBounds.center.z);
                                            ray.direction = Vector3.down;
                                            RaycastHit raycastHit;
                                            if (Physics.Raycast(ray, out raycastHit, Tools.GetLayerMask(new List<string>() { "Terrain" })))
                                            {
                                                Debug.Log("找到摆放的位置");

                                                Vector3 localPos = scenePlacementComponent.transform.parent.InverseTransformPoint(raycastHit.point);
                                                scenePlacementComponent.Offset = new Vector3(scenePlacementComponent.Offset.x, localPos.y - Mathf.Floor(localPos.y), scenePlacementComponent.Offset.z);
                                                scenePlacementComponent.MoveToGridPos(new Vector3(scenePlacementComponent.GridPos.x, localPos.y, scenePlacementComponent.GridPos.z));
                                            }
                                            else
                                            {
                                                Debug.Log("没有找到摆放的位置");
                                            }

                                        }
                                        e.Use();
                                        break;

                                    case KeyCode.C: // 替换组件 add by TangJian 2018/10/31 15:55

                                        GenericMenu menu = new GenericMenu();

                                        string firstComponentType = null;
                                        for (int i = 0; i < Selection.gameObjects.Length; i++)
                                        {
                                            GameObject go = Selection.gameObjects[i];
                                            SceneComponent sc = go.GetComponent<SceneComponent>();

                                            if (i == 0)
                                            {
                                                firstComponentType = sc.ComponentType;
                                            }
                                            else
                                            {
                                                if (firstComponentType != sc.ComponentType)
                                                {
                                                    firstComponentType = null;
                                                    break;
                                                }
                                            }
                                        }

                                        if (firstComponentType != null)
                                        {
                                            List<KeyValuePair<string, GameObject>> gameObjects = Tools.GetAllPrefabAndPathInFolder(Application.dataPath + "/Resources_moved/Prefabs");

                                            foreach (var gameObject in gameObjects)
                                            {
                                                if (gameObject.Value != null)
                                                {
                                                    SceneComponent sceneComponent = gameObject.Value.GetComponent<SceneComponent>();
                                                    if (sceneComponent != null)
                                                    {
                                                        if (firstComponentType == sceneComponent.ComponentType)
                                                        {
                                                            menu.AddItem(new GUIContent("更换组件/" + gameObject.Key.Substring(gameObject.Key.IndexOf("Prefabs"))), false, () =>
                                                            {
                                                                GameObject[] selectGameObjects = Selection.gameObjects;
                                                                List<GameObject> newSelectGameObjects = new List<GameObject>();

                                                                for (int i = 0; i < selectGameObjects.Length; i++)
                                                                {
                                                                    GameObject go = selectGameObjects[i];
                                                                    SceneComponent sc = go.GetComponent<SceneComponent>();

                                                                    SceneComponent newSceneComponent = sc.Editor_UpdateToPrefab(gameObject.Value);
                                                                    newSelectGameObjects.Add(newSceneComponent.gameObject);

                                                                    Undo.RegisterCreatedObjectUndo(newSceneComponent.gameObject, newSceneComponent.name);
                                                                }

                                                                Selection.objects = newSelectGameObjects.ToArray();
                                                            });
                                                        }
                                                    }
                                                }
                                            }

                                            menu.ShowAsContext();
                                        }
                                        e.Use();
                                        break;
                                    default:
                                        //e.Use();
                                        break;
                                }
                            }
                            break;
                    }
                }
            }

            // 绘制辅助线 add by TangJian 2018/10/25 22:19
            {
                {
                    float height = 9f * 2f;
                    float width = height / 9f * 16f;

                    Vector3 p1 = new Vector3(-1, -1, 10).Mul(new Vector3(width, height, 1) / 2f);
                    Vector3 p2 = new Vector3(-1, 1, 10).Mul(new Vector3(width, height, 1) / 2f);
                    Vector3 p3 = new Vector3(1, 1, 10).Mul(new Vector3(width, height, 1) / 2f);
                    Vector3 p4 = new Vector3(1, -1, 10).Mul(new Vector3(width, height, 1) / 2f);

                    p1 = sceneView.camera.transform.TransformPoint(p1);
                    p2 = sceneView.camera.transform.TransformPoint(p2);
                    p3 = sceneView.camera.transform.TransformPoint(p3);
                    p4 = sceneView.camera.transform.TransformPoint(p4);

                    //Handles.DrawAAConvexPolygon(p1, p2, p3, p4);

                    Handles.DrawLine(p1, p2);
                    Handles.DrawLine(p2, p3);
                    Handles.DrawLine(p3, p4);
                    Handles.DrawLine(p4, p1);

                    Handles.Label(p2, "角色视框: 1倍");
                }

                {
                    float factor = 1.2f;

                    float height = 9f * 2f;
                    float width = height / 9f * 16f;

                    Vector3 p1 = new Vector3(-1, -1, 10).Mul(new Vector3(width, height, 1) / 2f).Mul(factor, factor, 1);
                    Vector3 p2 = new Vector3(-1, 1, 10).Mul(new Vector3(width, height, 1) / 2f).Mul(factor, factor, 1);
                    Vector3 p3 = new Vector3(1, 1, 10).Mul(new Vector3(width, height, 1) / 2f).Mul(factor, factor, 1);
                    Vector3 p4 = new Vector3(1, -1, 10).Mul(new Vector3(width, height, 1) / 2f).Mul(factor, factor, 1);


                    Vector3 p5 = p1 * 100f;
                    Vector3 p6 = p2 * 100f;
                    Vector3 p7 = p3 * 100f;
                    Vector3 p8 = p4 * 100f;



                    //Vector3 p1x = p1 + new Vector3(-9999, 0, 0);
                    //Vector3 p1y = p1 + new Vector3(0, -9999, 0);

                    //Vector3 p2x = p2 + new Vector3(-9999, 0, 0);
                    //Vector3 p2y = p2 + new Vector3(0, 9999, 0);

                    //Vector3 p3x = p3 + new Vector3(9999, 0, 0);
                    //Vector3 p3y = p3 + new Vector3(0, 9999, 0);

                    //Vector3 p4x = p4 + new Vector3(9999, 0, 0);
                    //Vector3 p4y = p4 + new Vector3(0, -9999, 0);

                    p1 = sceneView.camera.transform.TransformPoint(p1);
                    p2 = sceneView.camera.transform.TransformPoint(p2);
                    p3 = sceneView.camera.transform.TransformPoint(p3);
                    p4 = sceneView.camera.transform.TransformPoint(p4);

                    p5 = sceneView.camera.transform.TransformPoint(p5);
                    p6 = sceneView.camera.transform.TransformPoint(p6);
                    p7 = sceneView.camera.transform.TransformPoint(p7);
                    p8 = sceneView.camera.transform.TransformPoint(p8);


                    //p1x = sceneView.camera.transform.TransformPoint(p1x);
                    //p1y = sceneView.camera.transform.TransformPoint(p1y);

                    //p2x = sceneView.camera.transform.TransformPoint(p2x);
                    //p2y = sceneView.camera.transform.TransformPoint(p2y);

                    //p3x = sceneView.camera.transform.TransformPoint(p3x);
                    //p3y = sceneView.camera.transform.TransformPoint(p3y);

                    //p4x = sceneView.camera.transform.TransformPoint(p4x);
                    //p4y = sceneView.camera.transform.TransformPoint(p4y);

                    Handles.DrawLine(p1, p2);
                    Handles.DrawLine(p2, p3);
                    Handles.DrawLine(p3, p4);
                    Handles.DrawLine(p4, p1);



                    ////下
                    Handles.color = new Color(0, 0, 0, 0.2f);
                    Handles.DrawAAConvexPolygon(p1, p5, p6, p2);
                    Handles.DrawAAConvexPolygon(p2, p6, p7, p3);
                    Handles.DrawAAConvexPolygon(p3, p7, p8, p4);
                    Handles.DrawAAConvexPolygon(p4, p8, p5, p1);


                    ////下左
                    //Handles.color = new Color(0, 0, 0, 0.5f);
                    //Handles.DrawAAConvexPolygon(p1y, p1y + p1x, p2x, p2);

                    ////左
                    //Handles.color = new Color(0, 0, 0, 0.5f);
                    //Handles.DrawAAConvexPolygon(p1, p1x, p2x, p2);

                    ////上
                    //Handles.color = new Color(0, 0, 0, 0.5f);
                    //Handles.DrawAAConvexPolygon(p2, p2y, p3y, p3);

                    ////右
                    //Handles.color = new Color(0, 0, 0, 0.5f);
                    //Handles.DrawAAConvexPolygon(p3, p3x, p4x, p4);

                    Handles.color = Color.white;
                    Handles.Label(p2, "角色视框: 1.2倍");
                }
            }
        }
    }
}