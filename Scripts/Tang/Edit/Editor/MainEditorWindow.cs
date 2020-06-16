using System;
using System.Collections.Generic;
using Boo.Lang;
using Tang.FrameEvent;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZS;

namespace Tang.Editor
{
    public enum MainEditorMode
    {
        Close = 0,
        Move,
        Dig
    }

    public class MainEditorWindow : EditorWindow
    {
        [MenuItem("Window/MainEditorWindow")]
        static void Init()
        {
            MainEditorWindow window = (MainEditorWindow)EditorWindow.GetWindow(typeof(MainEditorWindow));
            window.Show();
        }

        protected Vector2 mousePosition = Vector2.zero;
        protected SceneView sceneView;
        MainEditorMode mainEditorMode
        {
            set
            {
                PlayerPrefs.SetInt("mainEditorMode", (int)value);
            }

            get
            {
                return (MainEditorMode)PlayerPrefs.GetInt("mainEditorMode", (int)MainEditorMode.Close);
            }
        }

        Material holeBackMaterial;
        Material holeSideMaterial;
        Material holeBottomMaterial;

        Vector3 roleSize = new Vector3(1, 1, 1);

        ValueMonitorPool valueMonitorPool;

        void OnEnable()
        {
            // mainEditorMode = MainEditorMode.Close;


            //holeBackMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources_moved/Prefabs/SceneObject/Stage01/Hole/Back.mat");
            //holeSideMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources_moved/Prefabs/SceneObject/Stage01/Hole/Side.mat");
            //holeBottomMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources_moved/Prefabs/SceneObject/Stage01/Hole/Bottom.mat");



            //valueMonitorPool = new ValueMonitorPool();
            //valueMonitorPool.AddMonitor<int>(() =>
            //{
            //    return (int)mainEditorMode + (Application.isPlaying == true ? 10 : 20);
            //}, (int from, int to) =>
            //{
            //    sceneView = SceneView.lastActiveSceneView;

            //    SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUICallback_Move));
            //    SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUICallback_Dig));

            //    switch (mainEditorMode)
            //    {
            //        case MainEditorMode.Move:
            //            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUICallback_Move));
            //            break;
            //        case MainEditorMode.Dig:
            //            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUICallback_Dig));
            //            break;
            //    }
            //});
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
        }

        private float x = 1;
        private float y;
        private float z;

        private bool IsTest;

        private int Slectint = 0;
        private string[] Names = {"读取", "cunfang", " hh", "je"};

        private int Slect = 0;
        GUIStyle GetStyle(string styleName)
        {
            GUIStyle s = UnityEngine.GUI.skin.FindStyle(styleName);
            if (s == null)
                s = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            if (s == null)
            {
                Debug.Log("Missing " + styleName);
                s = new GUIStyle();
            }
            return s;
        }
        
        
        async void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUILayout.BeginHorizontal(GUILayout.Width(20));
                {
                    if (GUILayout.Button("Hhhh",GetStyle("label")))
                    {
                        Debug.Log("hhhh");
                    }                      
                }
                GUILayout.EndHorizontal();
                {
                    var guiMode = new GUIContent("Play");
                    Rect rMode = GUILayoutUtility.GetRect(guiMode, EditorStyles.toolbarDropDown);
                    if (EditorGUI.DropdownButton(rMode, guiMode, FocusType.Keyboard, EditorStyles.toolbarDropDown))
                    {
                        var menu = new GenericMenu();
                        for (int i = 0; i < Names.Length; i++)
                        {
                            var m = Names[i];
                            menu.AddItem(new GUIContent(m),false,()=>{Debug.Log(m);});
                        }
                        menu.DropDown(rMode);
                    }  
                }
            }
            
            GUILayout.EndHorizontal();

            GUILayout.BeginArea(new Rect(0, 20, 100, 100));
            GUILayout.BeginVertical(EditorStyles.boldLabel,GUILayout.Height(20));
            
            for (int i = 0; i < Names.Length; i++)
            {
                GUIContent guiContent = new GUIContent(Names[i]);
                Rect rect = GUILayoutUtility.GetRect(guiContent,GetStyle("Label"));
                
                if (Slectint == i)
                {
                    EditorGUI.DrawRect(rect,new Color(0,1,1,0.5f));
                }
                if (GUI.Button(rect,Names[i],GetStyle("label")))
                {
                    if (Slectint != i)
                    {
                        Slectint = i;
                    }

                    if (UnityEngine.Event.current.button == 1)
                    {
                        Debug.Log(Names[i]);

                    }
                }
                
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
            
            
            GUILayout.BeginArea(new Rect(220, 20, 100, 100));
            GUILayout.BeginVertical(EditorStyles.label,GUILayout.Height(20));
            
            Slectint = GUILayout.SelectionGrid(Slectint, Names, 1);

            GUILayout.EndVertical();
            GUILayout.EndArea();
            
            GUILayout.BeginArea(new Rect(420, 20, 100, 100));
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("hjahhaha");
                GUILayout.Button("+", GetStyle("Label"), GUILayout.Width(20));
            }
            GUILayout.EndArea();


            
//            using (new EditorGUILayout.VerticalScope())
//            {
////                x = MyGUI.FloatFieldWithTitle("x", x);
////                y = MyGUI.FloatFieldWithTitle("y", y);
////                z = MyGUI.FloatFieldWithTitle("z", z);
////                
//                if (MyGUI.Button("Test"))
//                {
//                    AsyncOperationHandle handle;
//                    Addressables.ResourceManager.Dispose();
//                    
//                }
//            }



            //mainEditorMode = (MainEditorMode)MyGUI.EnumPopupWithTitle("编辑模式", mainEditorMode);


            //holeBackMaterial = EditorGUILayout.ObjectField(holeBackMaterial, typeof(Material), false) as Material;
            //holeSideMaterial = EditorGUILayout.ObjectField(holeSideMaterial, typeof(Material), false) as Material;
            //holeBottomMaterial = EditorGUILayout.ObjectField(holeBottomMaterial, typeof(Material), false) as Material;

            //if (MyGUI.Button("计算角度"))
            //{
            //    // Camera camera = Camera.main;

            //    // Vector3 beginScreenVec3 = new Vector3(0, 0);
            //    // Vector3 endScreenVec3 = new Vector3(100, 100);

            //    // Vector3 endWorldVec3 = camera.ScreenToWorldPoint(endScreenVec3);
            //    Debug.Log(Mathf.Atan(2) / Mathf.PI * 180);
            //    Debug.Log(Mathf.Atan(Mathf.Sqrt(3)) / Mathf.PI * 180);

            //    // Debug.Log(Mathf.Atan(2) / Mathf.PI * 180);


            //    // Debug.Log();
            //}


            //if (MyGUI.Button("生成材质"))
            //{
            //    foreach (var gameObject in Selection.gameObjects)
            //    {
            //        if (PrefabUtility.GetPrefabType(gameObject) == PrefabType.Prefab && gameObject.GetComponent<SceneComponent>())
            //        {
            //            Debug.Log("为" + gameObject.name + "生成材质");
            //            Tools.ModifyPrefab(gameObject, (GameObject modifyGameObject) =>
            //            {
            //                SceneComponent modifyTerrainComponent = modifyGameObject.GetComponent<SceneComponent>();
            //                modifyTerrainComponent.Editor_Build();
            //            });
            //        }
            //    }

            //    // foreach (SceneController sceneController in targets)
            //    // {

            //    //     string materialPath = Tools.GetPrefabPath(sceneController.gameObject, true) + "/" + Tools.GetPrefabRoot(sceneController.gameObject).name;
            //    //     Debug.Log("materialPath = " + materialPath);

            //    //     Tools.CreateFolder(materialPath);

            //    //     sceneController.gameObject.Recursive((GameObject go, int depth) =>
            //    //     {
            //    //         if (go.tag == "Ground" || go.tag == "BackWall" || go.tag == "FrontWall")
            //    //         {
            //    //             var ground = go.GetComponent<TerrainComponent>();
            //    //             ground.CreateMesh(materialPath + "/" + Tools.getOnlyId());
            //    //         }
            //    //     }, 1, 999);
            //    // }
            //}

            //if (MyGUI.Button("生成立方体"))
            //{
            //    GameObject cube = new GameObject();

            //    MeshFilter meshFilter = cube.AddComponentUnique<MeshFilter>();
            //    MeshRenderer meshRenderer = cube.AddComponentUnique<MeshRenderer>();


            //    {
            //        // 前
            //        // Vector3 p0 = new Vector3(0, 0, 0);
            //        // Vector3 p1 = new Vector3(1, 0, 0);
            //        // Vector3 p2 = new Vector3(1, 1, 0);
            //        // Vector3 p3 = new Vector3(0, 1, 0);

            //        Vector3 p0 = new Vector3(0, 0, 0);
            //        Vector3 p1 = new Vector3(2, 0, 0);
            //        Vector3 p2 = new Vector3(2, 1, 0);
            //        Vector3 p3 = new Vector3(0, 1, 0);

            //        // 后
            //        Vector3 p4 = new Vector3(0, 0, 1);
            //        Vector3 p5 = new Vector3(1, 0, 1);
            //        Vector3 p6 = new Vector3(1, 1, 1);
            //        Vector3 p7 = new Vector3(0, 1, 1);

            //        // 上
            //        Vector3 P8 = p3;
            //        Vector3 p9 = p2;
            //        Vector3 p10 = p6;
            //        Vector3 p11 = p7;

            //        // 下
            //        Vector3 p12 = p0;
            //        Vector3 p13 = p1;
            //        Vector3 p14 = p5;
            //        Vector3 p15 = p4;

            //        // 左
            //        Vector3 p16 = p4;
            //        Vector3 p17 = p0;
            //        Vector3 p18 = p3;
            //        Vector3 p19 = p7;

            //        // 右
            //        Vector3 p20 = p1;
            //        Vector3 p21 = p5;
            //        Vector3 p22 = p6;
            //        Vector3 p23 = p2;

            //        Mesh mesh = new Mesh();

            //        mesh.vertices = new Vector3[24]{
            //            p0, p1, p2, p3,
            //            p4, p5, p6, p7,

            //            P8, p9, p10, p11,
            //            p12, p13, p14, p15,

            //            p16, p17, p18, p19,
            //            p20, p21, p22, p23,

            //         };

            //        mesh.triangles = new int[3 * 2 * 6]
            //        {
            //            0, 2, 1,
            //            0, 3, 2,
            //            4, 5, 6,
            //            4, 6, 7,

            //            8, 10, 9,
            //            8, 11, 10,
            //            12, 13, 14,
            //            12, 14, 15,

            //            16, 18, 17,
            //            16, 19, 18,
            //            20, 22, 21,
            //            20, 23, 22,
            //        };

            //        mesh.uv = new Vector2[]
            //        {
            //            // 正
            //            new Vector2(0, 0),
            //            new Vector2(1, 0),
            //            new Vector2(1, 1),
            //            new Vector2(0, 1),

            //            // 反
            //            new Vector2(0, 0),
            //            new Vector2(1, 0),
            //            new Vector2(1, 1),
            //            new Vector2(0, 1),

            //            //上
            //            new Vector2(0, 0),
            //            new Vector2(1, 0),
            //            new Vector2(1, 1),
            //            new Vector2(0, 1),

            //            //下
            //            new Vector2(0, 0),
            //            new Vector2(1, 0),
            //            new Vector2(1, 1),
            //            new Vector2(0, 1),

            //            //左
            //            new Vector2(0, 0),
            //            new Vector2(1, 0),
            //            new Vector2(1, 1),
            //            new Vector2(0, 1),

            //            //右
            //            new Vector2(0, 0),
            //            new Vector2(1, 0),
            //            new Vector2(1, 1),
            //            new Vector2(0, 1),
            //        };


            //        Color[] colors = new Color[24];
            //        for (int i = 0; i < colors.Length; i++)
            //        {
            //            colors[i] = Color.white;
            //        }
            //        mesh.colors = colors;

            //        Vector3[] normals = new Vector3[24];
            //        for (int i = 0; i < normals.Length; i++)
            //        {
            //            normals[i] = new Vector3(1, 1, 1);
            //        }
            //        mesh.normals = normals;

            //        meshFilter.mesh = mesh;
            //    }

            //    Material material = new Material(Shader.Find("Unlit/Transparent Cutout"));
            //    material.mainTexture = Resources.Load<Texture2D>("Prefabs/SceneObject/Stage01/Textures/3d/Wall");
            //    material.renderQueue = 2000;

            //    meshRenderer.material = material;
            //}

            //if (MyGUI.Button("为GameObject生成寻路障碍"))
            //{
            //    foreach (var go in Selection.gameObjects)
            //    {
            //        var navMeshObstacle = go.AddComponentUnique<UnityEngine.AI.NavMeshObstacle>();

            //        Bounds bounds = go.GetColliderBounds();

            //        navMeshObstacle.size = bounds.size;

            //        navMeshObstacle.center = new Vector3(bounds.center.x - go.transform.position.x, bounds.center.y - go.transform.position.y, bounds.center.z - go.transform.position.z);
            //        navMeshObstacle.carving = true;
            //    }
            //}

            //roleSize = EditorGUILayout.Vector3Field("角色大小", roleSize);
            //if (MyGUI.Button("生成角色结构"))
            //{
            //    GameObject roleObject = Selection.activeGameObject;
            //    Undo.RecordObject(roleObject, "生成角色结构");

            //    RoleController roleController = roleObject.GetComponent<RoleController>();
            //    if (roleController != null)
            //    {
            //        GameObject animObject = roleObject.GetChild("Anim");
            //        Bounds animObjectBounds = animObject.GetRendererBounds(1, 1);

            //        // 调整角色控制器的碰撞区域大小 add by TangJian 2018/05/04 16:43:18
            //        {
            //            CharacterController characterController = roleObject.GetComponent<CharacterController>();
            //            characterController.radius = roleSize.x / 2;
            //            characterController.height = 0;
            //            characterController.center = new Vector3(0, characterController.radius, 0);

            //            characterController.slopeLimit = 30;
            //            characterController.stepOffset = 0.1f;
            //        }

            //        // 调整受击区域大小 add by TangJian 2018/05/04 16:51:34
            //        {
            //            GameObject damageTargetObject = roleObject.GetChild("DamageTarget", true);
            //            damageTargetObject.layer = LayerMask.NameToLayer("Interaction");
            //            damageTargetObject.tag = "DamageTarget";

            //            damageTargetObject.transform.localPosition = new Vector3(0, roleSize.y / 2f, 0);
            //            damageTargetObject.transform.localScale = new Vector3(roleSize.x, roleSize.y, roleSize.z);

            //            // 添加刚体 add by TangJian 2018/05/04 17:19:12
            //            Rigidbody rigidbody = damageTargetObject.AddComponentUnique<Rigidbody>();
            //            rigidbody.useGravity = false;
            //            rigidbody.isKinematic = true;

            //            // 触发器 add by TangJian 2018/05/04 16:54:07
            //            BoxCollider boxCollider = damageTargetObject.AddComponentUnique<BoxCollider>();
            //            boxCollider.size = new Vector3(1, 1, 1);
            //            boxCollider.center = Vector3.zero;
            //            boxCollider.isTrigger = true;

            //            // 触发器控制器 add by TangJian 2018/05/04 16:54:36
            //            TriggerController triggerController = damageTargetObject.AddComponentUnique<TriggerController>();
            //            triggerController.triggerData.type = TriggerType.DamageTarget;
            //            triggerController.triggerData.watchTypes = new List<TriggerType> { TriggerType.Damage };
            //        }

            //        // 调整交互区域大小 add by TangJian 2018/05/04 17:00:31
            //        {
            //            GameObject oldInteractionObject = roleObject.GetChild("InteractiveCube");
            //            if (oldInteractionObject != null)
            //                DestroyImmediate(oldInteractionObject);

            //            GameObject interactionObject = roleObject.GetChild("Interaction", true);

            //            interactionObject.transform.localPosition = new Vector3(0, roleSize.y / 2f, 0);
            //            interactionObject.transform.localScale = new Vector3(roleSize.x, roleSize.y, roleSize.z);

            //            // 触发器 add by TangJian 2018/05/04 16:54:07
            //            BoxCollider boxCollider = interactionObject.AddComponentUnique<BoxCollider>();
            //            boxCollider.size = new Vector3(1, 1, 1);
            //            boxCollider.center = Vector3.zero;
            //            boxCollider.isTrigger = true;

            //            // 触发器控制器 add by TangJian 2018/05/04 16:54:36
            //            TriggerController triggerController = interactionObject.AddComponentUnique<TriggerController>();
            //            triggerController.triggerData.type = TriggerType.Role;
            //            triggerController.needKeepingGameObject = true;
            //        }

            //        // 影子大小 add by TangJian 2018/05/04 17:04:26
            //        {
            //            GameObject shadowObject = roleObject.GetChild("Shadow", true);

            //            ShadowController shadowController = shadowObject.GetComponent<ShadowController>();
            //            shadowController.shadowSize = roleSize.x * 0.8f;

            //            // GameObject childShadowObject = shadowObject.GetChild("Shadow");
            //            // childShadowObject.transform.localScale = new Vector3(roleSize.x, roleSize.y, roleSize.z);
            //        }
            //    }
            //}

            //if (MyGUI.Button("初始化scenedecoration"))
            //{
            //    foreach (var item in Selection.gameObjects)
            //    {
            //        MeshRenderer mesh;
            //        if (item.GetComponent<MeshRenderer>() == null)
            //        {
            //            item.AddComponent<MeshRenderer>();
            //        }
            //        if (item.GetComponent<SceneDecorationComponent>() == null)
            //        {
            //            item.AddComponent<Tang.SceneDecorationComponent>();
            //        }
            //        if (item.GetComponent<MeshCollider>() == null)
            //        {
            //            item.AddComponent<MeshCollider>();
            //        }

            //        item.layer = LayerMask.NameToLayer("SceneComponent");

            //        mesh = item.GetComponent<MeshRenderer>();
            //        // Material share= mesh.sharedMaterial;
            //        foreach (var share in mesh.sharedMaterials)
            //        {
            //            share.shader = Shader.Find("Custom/Unlit/Transparent1");

            //            share.SetFloat("_ZWrite", 1f);
            //            share.SetFloat("_IsScale", 0);
            //            share.renderQueue = 3000;
            //        }



            //    }
            //}

            //if (MyGUI.Button("生成mesh保存到本地"))
            //{
            //    var width = 1;
            //    var height = 1;

            //    Mesh mesh = new Mesh();

            //    // 为网格创建顶点数组
            //    Vector3[] vertices = new Vector3[4]{
            //                    new Vector3(0, 0, 0),
            //                    new Vector3(width, 0, 0),
            //                    new Vector3(width, height, 0),
            //                    new Vector3(0, height , 0)
            //                };

            //    mesh.vertices = vertices;

            //    // 通过顶点为网格创建三角形
            //    int[] triangles = new int[2 * 3]{
            //                    0, 2, 1, 0, 3, 2
            //                };
            //    mesh.triangles = triangles;

            //    mesh.uv = new Vector2[]{
            //                    new Vector2(0, 0),
            //                    new Vector2(1, 0),
            //                    new Vector2(1, 1),
            //                    new Vector2(0, 1)
            //                };

            //    // AssetDatabase.

            //    var fileName = "Assets/mesh.asset";
            //    AssetDatabase.CreateAsset(mesh, fileName);
            //}


            //if (MyGUI.Button("test"))
            //{
            //    float angle = 225;
            //    Vector3 moveOrientation = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;

            //    // Vector3 moveOrientation = Quaternion.AngleAxis(0, Vector3.back) * Vector3.right;

            //    Debug.Log("angle = " + angle);
            //    Debug.Log("moveOrientation = " + moveOrientation);


            //    // string str = "";

            //    // Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //    // foreach (var assembly in assemblies)
            //    // {
            //    //     Type[] types = assembly.GetTypes();
            //    //     foreach (var item in types)
            //    //     {
            //    //         str += item.Name + "\n";
            //    //     }
            //    // }
            //    // Debug.Log(str);

            //    // byte[] bytes = File.ReadAllBytes("D:/WorkSpace/bnyx_new/Assets/Resources_moved/Textures/Placement/20180611/xiangzizu.png");

            //    // Texture2D texture2d = new Texture2D(0, 0);
            //    // texture2d.LoadImage(bytes);

            //    // Debug.Log("texture2d.width = " + texture2d.width);
            //    // Debug.Log("texture2d.height = " + texture2d.height);     


            //    // EditorCoroutineRunner.AddCoroutineIfNot("Test", Test());


            //    // MethodDelegate<int> t3 = CSScript.CreateFunc<int>(@"int Sum(int a, int b)
            //    //                          {
            //    //                              return a+b;
            //    //                          }");

            //    // Debug.Log(t3(1, 2));


            //    // var t1 = CSScript.LoadCode(@"using System;
            //    //                      public class Script
            //    //                      {
            //    //                          public int Sum(int a, int b)
            //    //                          {
            //    //                              return a+b;
            //    //                          }
            //    //                      }");

            //    // MethodDelegate t2 = t1.GetStaticMethod("Sum");
            //    // Debug.Log(t2(new object[] { 1, 2 }));

            //    // var script = CSScript.LoadMethod(@"void SayHello(string greeting)
            //    //                        {
            //    //                            Console.WriteLine(greeting);
            //    //                        }");


            //    // MethodDelegate t = script.GetStaticMethod("SayHello");
            //    // t(new object[] { "123" });
            //}


            //GameEditorSettings.Instance.frontDoorOffset = MyGUI.Vector3WithTitle("前景墙图片偏移:", GameEditorSettings.Instance.frontDoorOffset);
            //GameEditorSettings.Instance.Save();

            //valueMonitorPool.Update();
        }
    }
}