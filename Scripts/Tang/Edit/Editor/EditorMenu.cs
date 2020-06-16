using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Spine.Unity;
using Tang;
using Tang.FrameEvent;


public class EditorMenu
{
    static public string GetTimeName()
    {
        return System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() +
               System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() +
               System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() +
               System.DateTime.Now.Millisecond.ToString();
    }

    static public Texture2D SaveRenderToPng(RenderTexture renderT, string folderName, string name)
    {
        int width = renderT.width;
        int height = renderT.height;
        Texture2D tex2d = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture.active = renderT;
        tex2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex2d.Apply();

        byte[] b = tex2d.EncodeToPNG();
        string sysPath = "c:/" + folderName;
        if (!Directory.Exists(sysPath))
            Directory.CreateDirectory(sysPath);
        FileStream file = File.Open(sysPath + "/" + name + GetTimeName() + ".png", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(b);
        file.Close();

        return tex2d;
    }
    
    [MenuItem("Assets/创建ScriptableObject/RoleQTEDataAsset")]
    public static void CreateRoleQTEDataAsset()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log(path);
        if (System.IO.Path.HasExtension(path))
        {
            Debug.LogError("请点击文件夹空白区域创建");
        }
        else
        {
            string fileName = path + "/" + "RoleQTEDataAsset.asset";
            RoleQTEDataAsset scriptableObject = ScriptableObject.CreateInstance<RoleQTEDataAsset>();
            AssetDatabase.CreateAsset(scriptableObject, fileName);
        }
    }
    
    [MenuItem("自定义/清除进度条")]
    static void ClearProgressBar()
    {
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("测试/test")]
    static void Test()
    {
        //Tang.editor.EditorCoroutineRunner.AddCoroutine("EditorAuto", Tang.editor.EditorAuto.WalkFiles());
        Camera lightCamera = Camera.main.gameObject.GetChild("Light").GetComponent<Camera>();
        SaveRenderToPng(lightCamera.targetTexture, "test", "test");
    }

    [MenuItem("测试/菜单")]
    static void TestMenu()
    {
        //Tang.editor.EditorCoroutineRunner.AddCoroutine("EditorAuto", Tang.editor.EditorAuto.WalkFiles());
        Camera lightCamera = Camera.main.gameObject.GetChild("Light").GetComponent<Camera>();
        SaveRenderToPng(lightCamera.targetTexture, "test", "test");
    }

    [MenuItem("测试/生成1x1的网格")]
    static void Create1x1Mesh()
    {
        Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/NormalMesh.asset");

        if (mesh == null)
        {
            mesh = new Mesh();
            AssetDatabase.CreateAsset(mesh, "Assets/NormalMesh.asset");
        }

        mesh.vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0),
        };

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            mesh.vertices[i] = mesh.vertices[i] - new Vector3(0.5f, 0.5f, 0);
        }

        mesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
        };

        mesh.triangles = new int[]
        {
            0, 1, 2, 0, 2, 3
        };

        mesh.normals = new Vector3[]
        {
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
        };

        EditorUtility.SetDirty(mesh);

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Assets/自定义/从Sprite生成GameObject")]
    static void GenrateGameObjectFromSprite()
    {
        Object[] objects = Selection.objects;
        string label = "GenerateSprites";

        try
        {
            for (int i = 0; i < objects.Length; i++)
            {
                Object obj = objects[i];
                string fileName = AssetDatabase.GetAssetPath(obj);
                string path = fileName.Substring(0, fileName.LastIndexOf('/'));

                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(fileName);

                EditorUtility.DisplayProgressBar("从Sprite生成GameObject", i + "/" + objects.Length, (float)i / objects.Length);

                if (sprite != null)
                {
                    path = path + "/" + label;

                    Tang.Tools.CreateFolder(Tang.Tools.AssetPathToSysPath(path));

                    path = path + "/" + sprite.name;

                    GameObject go = PrefabCreator.GenrateGameObjectFromSprite(sprite);
                    PrefabUtility.CreatePrefab(path + ".prefab", go);
                    GameObject.DestroyImmediate(go);
                    //AssetDatabase.CreateAsset(gameObject, path);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    [MenuItem("自定义/为所有SkeletonDataAsset添加帧事件")]
    static void AddFrameEventForAllSkeletonAsset()
    {
        List<string> files = Tang.Tools.GetFileListInFolder(Application.dataPath, ".asset");

        for (int i = 0; i < files.Count; i++)
        {
            string fileName = files[i];
            EditorUtility.DisplayProgressBar("为所有SkeletonDataAsset添加帧事件", fileName, (float)i / files.Count);

            SkeletonDataAsset skeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(Tang.Tools.SysPathToAssetPath(fileName));
            if (skeletonDataAsset != null)
            {
                string jsonPath = JsonFileTools.GetAniEventPath(skeletonDataAsset);

                TextAsset textAsset =
                    AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
                if (textAsset != null)
                    skeletonDataAsset.TextAsset = textAsset;

                EditorUtility.SetDirty(skeletonDataAsset);
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("自定义/重新计算所有mesh")]
    static void RecalculateAllMesh()
    {
        List<string> files = Tang.Tools.GetFileListInFolder(Application.dataPath, ".asset");

        for (int i = 0; i < files.Count; i++)
        {
            string fileName = files[i];
            EditorUtility.DisplayProgressBar("重新计算所有mesh", fileName, (float)i / files.Count);

            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(Tang.Tools.SysPathToAssetPath(fileName));
            if (mesh != null)
            {
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
            }
        }

        AssetDatabase.SaveAssets();

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/自定义/生成图片网格和材质")]
    static void CreateImageMeshAndMaterialMenuItem()
    {
        foreach (Object texture in Selection.objects)
        {
            string fileName = AssetDatabase.GetAssetPath(texture);
            string path = fileName.Substring(0, fileName.LastIndexOf('/'));
            
            Texture2D texture2D = texture as Texture2D;
            if (texture2D)
                CreateImageMeshAndMaterial(texture2D, path);
        }
    }

    static void CreateImageMeshAndMaterial(Texture2D texture2D, string path)
    {
        float width = texture2D.width / 100f;
        float height = texture2D.height / 100f;
        Material blackMaterial;
        // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
        {
            Mesh mesh = new Mesh();

            // 为网格创建顶点数组
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(width / 2, (height*(float)System.Math.Sqrt(3)) / 2, 0),
                new Vector3(-width / 2, (height*(float)System.Math.Sqrt(3)) / 2, 0),
                new Vector3(width / 2, (-height*(float)System.Math.Sqrt(3))/ 2, 0),
                new Vector3(-width / 2, (-height*(float)System.Math.Sqrt(3)) / 2, 0)
            };

            mesh.vertices = vertices;

            // 通过顶点为网格创建三角形
            int[] triangles = new int[2 * 3]{
                0, 3, 1, 0, 2, 3
            };
            mesh.triangles = triangles;

            mesh.uv = new Vector2[]{
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(0, 0)
            };
            
            AssetDatabase.CreateAsset(mesh, path + "/" + texture2D.name + "_mesh" + ".asset");
        }
    }


    [MenuItem("自定义/刷新所有场景组件路径")]
    static void RefreshAllSceneComponentPath()
    {
        RefreshAllSceneComponentPath(Application.dataPath + "/Resources_moved/Prefabs");
    }


    static void RefreshAllSceneComponentPath(string path)
    {
        //try
        //{
        //    EditorUtility.DisplayProgressBar("刷新场景组件路径", "搜索预制体文件", 0);
        //    int layer = LayerMask.NameToLayer("SceneComponent");

        //    List<string> files = Tang.Tools.GetFileListInFolder(path, ".prefab");

        //    for (int i = 0; i < files.Count; i++)
        //    {
        //        string fileName = files[i];
        //        EditorUtility.DisplayProgressBar("刷新场景组件路径", fileName, (float)i / files.Count);

        //        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Tang.Tools.SysPathToAssetPath(fileName));
        //        if (prefab.layer == layer)
        //        {
        //            Tang.SceneComponent sceneComponent = prefab.GetComponent<Tang.SceneComponent>();
        //            if (sceneComponent != null)
        //            {
        //                Tang.Tools.ModifyPrefab(prefab, (GameObject go) =>
        //                {
        //                    Tang.SceneComponent sceneComponent_ = go.GetComponent<Tang.SceneComponent>();
        //                    sceneComponent_.filePath = Tang.Tools.GetPrefabPath(prefab);
        //                });
        //            }
        //        }
        //    }
        //}
        //finally
        //{
        //    EditorUtility.ClearProgressBar();
        //}
    }

    [MenuItem("自定义/生成所有场景组件模板")]
    static void GenerateTemplateSceneComponents()
    {
        string path = "Assets/Resources_moved/Prefabs/Terrains/Template";
        GenerateTemplateSceneComponents(path);
        RefreshAllSceneComponentPath(Tang.Tools.AssetPathToSysPath(path));
    }

    static void GenerateTemplateSceneComponents(string path)
    {
        GenerateTemplateGrounds(path);
        GenerateTemplateWalls(path);
        GenerateTemplateSlopes(path);
        GenerateTemplateSlopWalls(path);
    }

    [MenuItem("自定义/生成场景地面组件模板")]
    static void GenerateTemplateGrounds()
    {
        string path = "Assets/Resources_moved/Prefabs/Terrains/Template";
        GenerateTemplateGrounds(path);
        RefreshAllSceneComponentPath(Tang.Tools.AssetPathToSysPath(path));
    }

    static void GenerateTemplateGrounds(string path)
    {
        // 创建所有的地面模板 add by TangJian 2018/11/1 18:02
        {
            for (int x = 1; x <= 9; x++)
            {
                for (int y = 1; y <= 9; y++)
                {
                    EditorUtility.DisplayProgressBar("生成地面模板", x + "x" + y, (float)x * (float)y / 8.0f * 8.0f);

                    // 侧边地面 add by TangJian 2018/11/1 20:10
                    {
                        string prefabName = "Ground_Side_" + x + "x" + y;
                        string folder = "Ground_Side";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";


                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        Ground sceneComponent = go.AddComponentIfNone<Ground>();
                        sceneComponent.groundData.groundType = GroundType.Left;
                        sceneComponent.groundData.rows = y;
                        sceneComponent.groundData.cols = x;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }

                    // 中间地面 add by TangJian 2018/11/1 20:11
                    {
                        string prefabName = "Ground_Center_" + x + "x" + y;
                        string folder = "Ground_Center";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        Ground sceneComponent = go.AddComponentIfNone<Ground>();
                        sceneComponent.groundData.groundType = GroundType.Center;
                        sceneComponent.groundData.rows = y;
                        sceneComponent.groundData.cols = x;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("自定义/生成场景墙组件模板")]
    static void GenerateTemplateWalls()
    {
        string path = "Assets/Resources_moved/Prefabs/Terrains/Template";
        GenerateTemplateWalls(path);
        RefreshAllSceneComponentPath(Tang.Tools.AssetPathToSysPath(path));
    }

    static void GenerateTemplateWalls(string path)
    {
        // 生成墙壁模板 add by TangJian 2018/11/1 18:56
        {
            for (int x = 1; x <= 9; x++)
            {
                for (int y = 1; y <= 9; y++)
                {
                    EditorUtility.DisplayProgressBar("生成墙壁模板", x + "x" + y, (float)x * (float)y / 8.0f * 8.0f);

                    // 侧墙 add by TangJian 2018/11/1 20:13
                    {
                        string prefabName = "Wall_Side_Back_" + x + "x" + y;
                        string folder = "Wall_Side_Back";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.DestoryChildren();


                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        SceneSideWallComponent sceneComponent = go.AddComponentIfNone<SceneSideWallComponent>();
                        sceneComponent.wallData.widthZ = x;
                        sceneComponent.wallData.widthY = y;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }

                    {
                        string prefabName = "Wall_Side_Front_" + x + "x" + y;
                        string folder = "Wall_Side_Front";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.DestoryChildren();

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        SceneSideWallComponent sceneComponent = go.AddComponentIfNone<SceneSideWallComponent>();
                        sceneComponent.wallData.widthZ = x;
                        sceneComponent.wallData.widthY = y;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }

                    // 正墙 add by TangJian 2018/11/1 20:13
                    {
                        string prefabName = "Wall_Center_Back_" + x + "x" + y;
                        string folder = "Wall_Center_Back";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.DestoryChildren();

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        SceneCenterWallComponent sceneComponent = go.AddComponentIfNone<SceneCenterWallComponent>();
                        sceneComponent.wallData.widthX = x;
                        sceneComponent.wallData.widthY = y;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }

                    {
                        string prefabName = "Wall_Center_Front_" + x + "x" + y;
                        string folder = "Wall_Center_Front";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.DestoryChildren();

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        SceneCenterWallComponent sceneComponent = go.AddComponentIfNone<SceneCenterWallComponent>();
                        sceneComponent.wallData.widthX = x;
                        sceneComponent.wallData.widthY = y;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("自定义/生成场景斜坡件模板")]
    static void GenerateTemplateSlopes()
    {
        string path = "Assets/Resources_moved/Prefabs/Terrains/Template";
        GenerateTemplateSlopes(path);
        RefreshAllSceneComponentPath(Tang.Tools.AssetPathToSysPath(path));
    }

    static void GenerateTemplateSlopes(string path)
    {
        // 生成斜坡模板 add by TangJian 2018/11/1 20:55
        {
            for (int x = 1; x <= 9; x++)
            {
                for (int y = 1; y <= 9; y++)
                {
                    EditorUtility.DisplayProgressBar("生成斜坡模板", x + "x" + y, (float)x * (float)y / 8.0f * 8.0f);

                    // 侧边地面 add by TangJian 2018/11/1 20:10
                    {
                        string prefabName = "Slop_" + x + "x" + y;
                        string folder = "Slop";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        SceneSlopeComponent sceneComponent = go.AddComponentIfNone<SceneSlopeComponent>();
                        sceneComponent.slopData.widthZ = x;
                        sceneComponent.slopData.length = y;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("自定义/生成场景斜墙件模板")]
    static void GenerateTemplateSlopWalls()
    {
        string path = "Assets/Resources_moved/Prefabs/Terrains/Template";
        GenerateTemplateSlopWalls(path);
        RefreshAllSceneComponentPath(Tang.Tools.AssetPathToSysPath(path));
    }

    static void GenerateTemplateSlopWalls(string path)
    {
        // 生成斜墙模板 add by TangJian 2018/11/1 20:55
        {
            for (int x = 1; x <= 9; x++)
            {
                for (int y = 1; y <= 9; y++)
                {
                    EditorUtility.DisplayProgressBar("生成鞋墙模板", x + "x" + y, (float)x * (float)y / 8.0f * 8.0f);

                    // 侧边地面 add by TangJian 2018/11/1 20:10
                    {
                        string prefabName = "SlopWall_Back_" + x + "x" + x + "x" + y;
                        string folder = "SlopWall_Back";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        SceneSlopeWallComponent sceneComponent = go.AddComponentIfNone<SceneSlopeWallComponent>();
                        sceneComponent.slopeData.slopeX = x;
                        sceneComponent.slopeData.slopeY = x;
                        sceneComponent.slopeData.widthY = y;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }

                    // 侧边地面 add by TangJian 2018/11/1 20:10
                    {
                        string prefabName = "SlopWall_Front_" + x + "x" + x + "x" + y;
                        string folder = "SlopWall_Front";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        SceneSlopeWallComponent sceneComponent = go.AddComponentIfNone<SceneSlopeWallComponent>();
                        sceneComponent.slopeData.slopeX = x;
                        sceneComponent.slopeData.slopeY = x;
                        sceneComponent.slopeData.widthY = y;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }


    [MenuItem("自定义/生成场景墙顶模板")]
    static void GenerateTemplateWallTop()
    {
        string path = "Assets/Resources_moved/Prefabs/Terrains/Template";
        GenerateTemplateWallTop(path);
        RefreshAllSceneComponentPath(Tang.Tools.AssetPathToSysPath(path));
    }

    static void GenerateTemplateWallTop(string path)
    {
        // 生成墙顶模板 add by TangJian 2018/11/1 20:55
        {
            for (int x = 1; x <= 9; x++)
            {
                for (int y = 1; y <= 9; y++)
                {
                    EditorUtility.DisplayProgressBar("生成墙顶模板", x + "x" + y, (float)x * (float)y / 8.0f * 8.0f);

                    // 侧边地面 add by TangJian 2018/11/1 20:10
                    {
                        string prefabName = "WallTop_Center_" + x + "x" + y;
                        string folder = "WallTop_Center";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        SceneCenterWallTopComponent sceneComponent = go.AddComponentUnique<SceneCenterWallTopComponent>();
                        sceneComponent.wallData.widthX = x;
                        sceneComponent.wallData.widthZ = y;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }

                    // 侧边地面 add by TangJian 2018/11/1 20:10
                    {
                        string prefabName = "WallTop_Side_" + x + "x" + y;
                        string folder = "WallTop_Side";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        SceneSideWallTopComponent sceneComponent = go.AddComponentUnique<SceneSideWallTopComponent>();
                        sceneComponent.wallData.widthX = y;
                        sceneComponent.wallData.widthZ = x;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }
                }
            }
        }

        for (int x = 1; x <= 9; x++)
        {
            EditorUtility.DisplayProgressBar("生成墙顶模板", x.ToString(), (float)x / 9);

            // 斜墙顶部  add by TangJian 2018/11/1 20:10
            {
                string prefabName = "SlopeWallTop_" + x + "x" + x + "x" + 1;
                string folder = "SlopeWallTop";

                string prefabFilePath = path + "/" + folder;
                Tang.Tools.CreateAssetFolder(prefabFilePath);
                string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                if (go == null)
                {
                    go = new GameObject();
                    PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                }
                else
                {
                    go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                }

                go.layer = LayerMask.NameToLayer("SceneComponent");
                go.name = prefabName;

                SceneSlopeWallTopComponent sceneComponent = go.AddComponentUnique<SceneSlopeWallTopComponent>();
                sceneComponent.wallData.slopeX = x;
                sceneComponent.wallData.slopeY = x;
                sceneComponent.wallData.widthZ = 1;

                sceneComponent.Editor_Build();

                PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                Object.DestroyImmediate(go);
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }


    [MenuItem("自定义/生成场景三角墙模板")]
    static void GenerateTemplateTriangleWall()
    {
        string path = "Assets/Resources_moved/Prefabs/Terrains/Template";
        GenerateTemplateTriangleWall(path);
        RefreshAllSceneComponentPath(Tang.Tools.AssetPathToSysPath(path));
    }

    static void GenerateTemplateTriangleWall(string path)
    {
        // 生成墙顶模板 add by TangJian 2018/11/1 20:55
        {
            for (int x = 1; x <= 9; x++)
            {
                for (int y = 1; y <= 9; y++)
                {
                    EditorUtility.DisplayProgressBar("生成三角墙模板", x + "x" + y, (float)x * (float)y / 8.0f * 8.0f);

                    // 侧边地面 add by TangJian 2018/11/1 20:10
                    {
                        string prefabName = "TriangleWall_" + x + "x" + y;
                        string folder = "TriangleWall";

                        string prefabFilePath = path + "/" + folder;
                        Tang.Tools.CreateAssetFolder(prefabFilePath);
                        string prefabFileName = path + "/" + folder + "/" + prefabName + ".prefab";

                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFileName);
                        if (go == null)
                        {
                            go = new GameObject();
                            PrefabUtility.CreatePrefab(prefabFileName, go, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        else
                        {
                            go = PrefabUtility.InstantiatePrefab(go as GameObject) as GameObject;
                        }

                        go.layer = LayerMask.NameToLayer("SceneComponent");
                        go.name = prefabName;

                        SceneTriangleWallComponent sceneComponent = go.AddComponentIfNone<SceneTriangleWallComponent>();
                        sceneComponent.triangleWallData.widthX = x;
                        sceneComponent.triangleWallData.widthY = y;

                        sceneComponent.Editor_Build();

                        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go));

                        Object.DestroyImmediate(go);
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }
    [MenuItem("自定义/烘培所有场景")]
    static void BuildSceneNavagnet()
    {
        AssetDatabase.Refresh();

        List<string> files = Tang.Tools.GetFileListInFolder(Application.dataPath+ "/Resources_moved/Prefabs/Scenes/Tang", ".prefab");

        for (int i = 0; i < files.Count; i++)
        {
            string path = Tang.Tools.SysPathToAssetPath(files[i]);
            GameObject game = AssetManager.LoadAssetAtPath<GameObject>(path);
            if (game != null && game.GetComponent<SceneController>() != null)
            {
                Tang.Editor.EditorCoroutineSequenceRunner.AddCoroutine(buildnavmesh(game));
            }
        }

        //UnityEditor.AI.NavMeshAssetManager.instance.overUpdate = () =>
        //{
        //    AssetDatabase.SaveAssets();
        //    EditorUtility.ClearProgressBar();
        //};
        //EditorUtility.ClearProgressBar();
    }
    static System.Collections.IEnumerator buildnavmesh(GameObject gameObject)
    {
        GameObject game = PrefabUtility.InstantiatePrefab(gameObject) as GameObject;

        Debug.Log("烘培: " + game.name);

        List<UnityEngine.AI.NavMeshSurface> navMeshSurfaces = new List<UnityEngine.AI.NavMeshSurface>();
        for (int e = 0; e < UnityEngine.AI.NavMesh.GetSettingsCount(); e++)
        {
            bool build = true;
            UnityEngine.AI.NavMeshBuildSettings navMeshBuildSettings = UnityEngine.AI.NavMesh.GetSettingsByIndex(e);
            foreach (var item in game.GetComponents<UnityEngine.AI.NavMeshSurface>())
            {
                if (item.agentTypeID == navMeshBuildSettings.agentTypeID)
                {
                    build = false;
                    break;
                }
            }

            if (build)
            {
                UnityEngine.AI.NavMeshSurface navMeshSurface = game.AddComponent<UnityEngine.AI.NavMeshSurface>();
                LayerMask layerMask = new LayerMask();
                layerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Terrain");
                navMeshSurface.agentTypeID = navMeshBuildSettings.agentTypeID;
                navMeshSurface.collectObjects = UnityEngine.AI.CollectObjects.Children;
                navMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
                navMeshSurface.layerMask = layerMask;
                navMeshSurfaces.Add(navMeshSurface);
                //UnityEditor.AI.NavMeshAssetManager.instance.StartBakingSurfaces(gameObject.GetComponents<UnityEngine.AI.NavMeshSurface>());
            }
        }

        if (navMeshSurfaces.Count != 0)
        {
            UnityEditor.AI.NavMeshAssetManager.instance.StartBakingSurfaces(navMeshSurfaces.ToArray());
            EditorUtility.SetDirty(game);

            //UnityEditor.AI.NavMeshAssetManager.instance.overUpdate = () =>
            //{
            //    AssetDatabase.SaveAssets();
            //    Tang.Tools.Destroy(game);
            //};


            while (true)
            {
                if (UnityEditor.AI.NavMeshAssetManager.instance.GetBakeOperationsCout() > 0)
                {
                    yield return 0;
                }
                else
                {
                    Debug.Log("保存烘培: " + game.name);

                    PrefabUtility.SaveAsPrefabAsset(game, AssetDatabase.GetAssetPath(gameObject));
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    Tang.Tools.Destroy(game);
                    break;
                }
            }
        }
        else
        {
            Debug.Log("保存烘培: " + gameObject.name);
            Tang.Tools.Destroy(game);
        }
    }
}