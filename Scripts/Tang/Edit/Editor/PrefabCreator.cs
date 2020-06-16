using UnityEngine;

using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using ZS;

namespace Tang
{
    public class PrefabCreator
    {
        public static async Task<GameObject> CreateDropItem(string itemId)
        {
            var itemObject = new GameObject();
            itemObject.layer = LayerMask.NameToLayer("All");
            itemObject.tag = "Item";

            // 掉落物品控制器 add by TangJian 2017/12/20 15:25:25
            {
                DropItemController dropItemController = itemObject.AddComponentUnique<DropItemController>();
                dropItemController.IsSpriteRender = true;
                dropItemController.ItemId = itemId;
            }

            // 碰撞器 add by TangJian 2017/12/20 15:25:16
            {
                BoxCollider boxCollider = itemObject.AddComponentUnique<BoxCollider>();
            }

            // 刚体 add by TangJian 2017/12/20 15:24:52
            {
                Rigidbody rigidbody = itemObject.AddComponentUnique<Rigidbody>();
                rigidbody.useGravity = true;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            // 渲染节点 add by TangJian 2017/12/20 15:24:44
            {
                var rendererObject = itemObject.GetChild("Renderer", true);
                rendererObject.AddComponentUnique<MeshFilter>();
                rendererObject.AddComponentUnique<MeshRenderer>();
            }

            // 影子 add by TangJian 2017/12/20 15:25:45
            {
                GameObject Shadow = GameObject.Instantiate( await AssetManager.LoadAssetAsync<GameObject>("Assets/Resources_moved/Prefabs/Shadow/Shadow.prefab"));
                Shadow.transform.parent = itemObject.transform;
                Shadow.name = "Shadow";
            }

            // 交互节点 add by TangJian 2017/12/20 15:25:35
            {
                var interactObject = itemObject.GetChild("Interact", true);
                interactObject.layer = LayerMask.NameToLayer("Interaction");
                interactObject.tag = "Interaction";

                BoxCollider boxCollider = interactObject.AddComponentUnique<BoxCollider>();
                boxCollider.isTrigger = true;

                TriggerController triggerController = interactObject.AddComponentUnique<TriggerController>();
            }
            return itemObject;
        }

        public static GameObject CreateDropItem(ItemData itemData)
        {
            var itemObject = new GameObject();
            itemObject.layer = LayerMask.NameToLayer("SceneComponent");
            itemObject.tag = "Item";

            itemObject.AddComponent<SceneDropItemComponent>();

            // 掉落物品控制器 add by TangJian 2017/12/20 15:25:25
            {
                DropItemController dropItemController = itemObject.AddComponentUnique<DropItemController>();
                dropItemController.IsSpriteRender = true;
                dropItemController.ItemId = itemData.id;
                dropItemController.ItemData = itemData;
            }

            // 碰撞器 add by TangJian 2017/12/20 15:25:16
            {
                BoxCollider boxCollider = itemObject.AddComponentUnique<BoxCollider>();
            }

            // 刚体 add by TangJian 2017/12/20 15:24:52
            {
                Rigidbody rigidbody = itemObject.AddComponentUnique<Rigidbody>();
                rigidbody.useGravity = true;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            // 渲染节点 add by TangJian 2017/12/20 15:24:44
            {
                var rendererObject = itemObject.GetChild("Renderer", true);
                rendererObject.AddComponentUnique<MeshFilter>();
                rendererObject.AddComponentUnique<MeshRenderer>();
            }

            // 影子 add by TangJian 2017/12/20 15:25:45
            {
                GameObject Shadow = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources_moved/Prefabs/Shadow/Shadow.prefab"));
                // Shadow.layer=LayerMask.NameToLayer("Role");
                Shadow.transform.parent = itemObject.transform;
                Shadow.name = "Shadow";
                // ShadowProjector shadowProjector=Shadow.AddComponentUnique<ShadowProjector>();
                // shadowProjector.UVRect=new Rect(0f,0.5f,0.5f,0.5f);
                // Material material=(Material)AssetDatabase.LoadAssetAtPath("Assets/Standard Assets/Effects/Projectors/Materials/ShadowProjector.mat",typeof(Material));
                // shadowProjector._Material=material;
                // shadowProjector.AutoSizeOpacity=true;
                // shadowProjector.AutoSOMaxScaleMultiplier=0.5f;
                // shadowProjector.ShadowSize = createRoleData.size.x * 0.8f;
            }

            switch (itemData.pickUpMethod)
            {
                case PickUpMethod.Interact:
                    // 交互节点 add by TangJian 2017/12/20 15:25:35
                {
                    var interactObject = itemObject.GetChild("Interact", true);
                    interactObject.layer = LayerMask.NameToLayer("Interaction");
                    interactObject.tag = "Interaction";

                    BoxCollider boxCollider = interactObject.AddComponentUnique<BoxCollider>();
                    boxCollider.isTrigger = true;

                    TriggerController triggerController = interactObject.AddComponentUnique<TriggerController>();
                }
                    break;
                case PickUpMethod.FlyIn:
                    // 交互节点 add by TangJian 2017/12/20 15:25:35
                {
                    var interactObject = itemObject.GetChild("Interact", true);
                    interactObject.layer = LayerMask.NameToLayer("Interaction");
                    interactObject.tag = "Interaction";

                    interactObject.transform.localScale = interactObject.transform.localScale * 10;

                    BoxCollider boxCollider = interactObject.AddComponentUnique<BoxCollider>();
                    boxCollider.isTrigger = true;

                    TriggerController triggerController = interactObject.AddComponentUnique<TriggerController>();
                }
                    break;
                default:
                    Debug.LogError("未知PickUpMethod");
                    break;
            }

            return itemObject;
        }

        // 创建场景图片渲染节点 add by TangJian 2017/12/20 17:06:02
        public static GameObject Create2DItemRenderObject(string icon, GameObject gameObject = null)
        {
            if (gameObject == null)
                gameObject = new GameObject();

            Texture2D texture = Resources.Load<Texture2D>(icon);
            float width = texture.width / 100f;
            float height = texture.height / 100f;

            MeshFilter meshFilter = gameObject.GetComponentInChildren<MeshFilter>();
            MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

            Shader shader = Shader.Find("Custom/Unlit/Transparent");
            {
                Mesh mesh = new Mesh();

                // 为网格创建顶点数组
                Vector3[] vertices = new Vector3[4]{
                    new Vector3(width / 2, height / 2, 0),
                    new Vector3(-width / 2, height / 2, 0),
                    new Vector3(width / 2, -height / 2, 0),
                    new Vector3(-width / 2, -height / 2, 0)
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
                meshFilter.mesh = mesh;
            }
            Material material = new Material(shader);
            material.mainTexture = texture;
            material.renderQueue = 3000;

            meshRenderer.material = material;

            return gameObject;
        }
        //创建场景组件add by tianjinpeng 2018/05/28 11:02:16
        public static GameObject CreatePlacement(tian.PlacementConfig placementConfig, string path, float imgScale)
        {
            GameObject gameobject = new GameObject();

            GameObject renderer = gameobject.GetChild("Renderer", true);
            GameObject collider = gameobject.GetChild("Collider", true);
            GameObject trigger = gameobject.GetChild("Trigger", true);
            {//设置Rigidbody参数
                Rigidbody rigidbody = gameobject.AddComponentUnique<Rigidbody>();
                SetPlacementRigidbody(rigidbody, placementConfig.AddRigidbody);
            }

            SetPlacementRender(placementConfig, renderer, imgScale, path);//设置Renderer下的gameobject与参数

            switch (placementConfig.placementType)
            {
                case tian.PlacementType.TreasureBox:
                {
                    GameObject Damagecollider = SetPlacementTriggercollider(placementConfig, trigger);//设置trigger下的参数

                    TreasureBoxController TreasureBox = gameobject.AddComponentUnique<TreasureBoxController>();
                    TreasureBox.Placementdata = placementConfig.placementData;

                    GameObject Interact = SetPlacementTriggercollider(placementConfig, trigger, true, "Interact", "Interaction", "Interaction");

                    Rigidbody TreasureBoxrigidbody = gameobject.AddComponentUnique<Rigidbody>();
                    TreasureBoxrigidbody.useGravity = false;
                    TreasureBoxrigidbody.isKinematic = true;
                }
                    break;
                case tian.PlacementType.trap:
                {
                    GameObject Damagecollider = SetPlacementTriggercollider(placementConfig, trigger, true, "Interact", "Interaction", "Interaction");//设置trigger下的参数

                    switch (placementConfig.trapType)
                    {
                        case tian.TrapType.GroundStab:
                            GroundStabController groundstabController = gameobject.AddComponentUnique<GroundStabController>();
                            groundstabController.groundStabStateType = (GroundStabStateType)placementConfig.trapType;
                            groundstabController.attackTime = placementConfig.float1;
                            groundstabController.downTime = placementConfig.float2;
                            groundstabController.firststate = placementConfig.float3;
                            groundstabController.Placementdata = placementConfig.placementData;
                            break;
                    }
                }
                    break;
                case tian.PlacementType.bucket:
                {
                    GameObject gocollider = SetPlacementCoillder(placementConfig, collider);//设置Collider下的Collider参数
                    GameObject Damagecollider = SetPlacementTriggercollider(placementConfig, trigger);//设置trigger下的参数
                    WoodenBucketController WoodenBucketController = gameobject.AddComponentUnique<WoodenBucketController>();
                    WoodenBucketController.Placementdata = placementConfig.placementData;
                }
                    break;
                case tian.PlacementType.Ladder:
                {
                    GameObject Damagecollider = SetPlacementTriggercollider(placementConfig, trigger, true, "LadderInteraction", "Interaction");//设置trigger下的参数
                    LadderController ladderController = gameobject.AddComponentUnique<LadderController>();
                    ladderController.laddertype = placementConfig.Laddertype;
                    gameobject.tag = "Ladder";
                }
                    break;
                case tian.PlacementType.FenceDoor:
                {
                    GameObject gocollider = SetPlacementCoillder(placementConfig, collider);//设置Collider下的Collider参数
                    GameObject Damagecollider = SetPlacementTriggercollider(placementConfig, trigger, true, "Interaction", "Interaction");//设置trigger下的参数
                    BoxCollider Damagebox = Damagecollider.GetComponent<BoxCollider>();
                    FenceDoorController fenceDoorController = gameobject.AddComponentUnique<FenceDoorController>();
                    Vector3 rotation = new Vector3();
                    switch (placementConfig.Laddertype)
                    {
                        case laddertype.Center:
                            rotation = new Vector3(0, 0, 0);
                            break;
                        case laddertype.Left:
                            rotation = new Vector3(0, -Mathf.Atan(2) / Mathf.PI * 180, 0);
                            break;
                        case laddertype.Right:
                            rotation = new Vector3(0, Mathf.Atan(2) / Mathf.PI * 180, 0);
                            break;
                    }

                    gocollider.transform.localRotation = Quaternion.Euler(rotation);
                    Damagecollider.transform.localRotation = Quaternion.Euler(rotation);
                    //Damagecollider.tag = "Interaction";
                    Damagebox.size = new Vector3(placementConfig.size.x, placementConfig.size.y,
                        placementConfig.size.z * 1.5f);
                }
                    break;
                case tian.PlacementType.SceneDecoration:
                    GameObject Gocollider = SetPlacementCoillder(placementConfig, collider);//设置Collider下的Collider参数
                    GameObject damagecollider = SetPlacementTriggercollider(placementConfig, trigger);//设置trigger下的参数
                    TableController tablecontroller = gameobject.AddComponentUnique<TableController>();
                    tablecontroller.Placementdata = placementConfig.placementData;
                    break;
                case tian.PlacementType.Joystick:
                {
                    gameobject.AddComponentUnique<JoystickController>();

                    GameObject gocollider = SetPlacementCoillder(placementConfig, collider);//设置Collider下的Collider参数
                    GameObject Damagecollider = SetPlacementTriggercollider(placementConfig, trigger, true, "Interaction", "Interaction");//设置trigger下的参数
                    BoxCollider Damagebox = Damagecollider.GetComponent<BoxCollider>();
                    Damagebox.size = new Vector3(1, 1, 1);
                }
                    break;
                case tian.PlacementType.TriggerBox:
                {
                    gameobject.AddComponentUnique<TriggerBoxController>();

                    GameObject gocollider = SetPlacementCoillder(placementConfig, collider);//设置Collider下的Collider参数
                    GameObject Damagecollider = SetPlacementTriggercollider(placementConfig, trigger, true, "Interaction", "Interaction");//设置trigger下的参数
                    BoxCollider Damagebox = Damagecollider.GetComponent<BoxCollider>();
                    Damagebox.size = new Vector3(1, 1, 1);
                }
                    break;
                default:
                {
                    GameObject gocollider = SetPlacementCoillder(placementConfig, collider);//设置Collider下的Collider参数
                }
                    break;
            }
            
            CreatePlentCollider(gameobject, gameobject.tag, "SceneComponent", placementConfig.colliderSizeType, placementConfig.size, placementConfig.boundsPosition, placementConfig.colliderBounds, true);
            ScenePlacementComponent scenePlacementComponent = gameobject.AddComponentUnique<ScenePlacementComponent>();
            scenePlacementComponent.data.hasGravity = placementConfig.AddRigidbody;
            if (placementConfig.placementShake)
            {
                ShakeController shakeController = gameobject.AddComponentUnique<ShakeController>();
                shakeController.Placementdata = placementConfig.placementData;
            }

            string prefabFile = path + "/" + placementConfig.id + ".prefab";

            scenePlacementComponent.filePath = prefabFile;

            Tools.UpdatePrefab(gameobject, prefabFile);

            return gameobject;
        }
        public static GameObject CreateSceneDecoration(SceneDecorationEditorwindowData data, string path, float imgscale)
        {
            GameObject Gobject = new GameObject();
            MeshFilter meshfilter = Gobject.AddComponentUnique<MeshFilter>();
            MeshRenderer meshRenderer = Gobject.AddComponentUnique<MeshRenderer>();
            MeshCollider meshcollider = Gobject.AddComponentUnique<MeshCollider>();
            // SortRenderer sortRenderer = Gobject.AddComponentUnique<SortRenderer>();


            // AssetImporter assetImporter = AssetImporter.GetAtPath(data.texturePath);
            // TextureImporter textureimporter = (TextureImporter)assetImporter;
            // TextureImporterNPOTScale saa = textureimporter.npotScale;


            // textureimporter.npotScale = TextureImporterNPOTScale.None;
            // assetImporter = (AssetImporter)textureimporter;
            // AssetDatabase.ImportAsset(data.texturePath);
            byte[] bytes = File.ReadAllBytes(data.texturePath);

            Texture2D texture2d = new Texture2D(0, 0);
            texture2d.LoadImage(bytes);
            Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(data.texturePath, typeof(Texture2D));

            // (AssetImporter)textureimporter.AssetImporter.SaveAndReimport();
            // Image image=(Image)AssetDatabase.LoadAssetAtPath(data.texturePath, typeof(Image));   * (((float)System.Math.Sqrt(3) / 3f) * 2))
            float width = (texture2d.width / 100f);
            float height = ((texture2d.height * (((float)System.Math.Sqrt(3) / 3f) * 2)) / 100f);
            if (data.scaleType == tian.ScaleType.Alone)
            {
                width = width * data.AloneScale;
                height = height * data.AloneScale;
            }
            else
            {
                width = width * imgscale;
                height = height * imgscale;
            }


            Debug.Log("id:" + data.id + "  " + "width:" + texture2d.width + " " + "height:" + texture2d.height);
            // textureimporter.npotScale = saa;
            // assetImporter = (AssetImporter)textureimporter;
            // assetImporter.SaveAndReimport();
            // Material frist = (Material)AssetDatabase.LoadAssetAtPath(path + "/Rander/Image/first.mat", typeof(Material));
            Shader shader = Shader.Find("Custom/Unlit/Transparent");
            Material material = new Material(shader);
            // material.shader=shader;
            material.renderQueue = 2450;
            material.SetFloat("_IsScale", 0);
            Mesh mesh = new Mesh();
            switch (data.sceneDecorationPosition)
            {
                case SceneDecorationPosition.Positive:
                {
                    SortRenderer sortRenderer = Gobject.AddComponentUnique<SortRenderer>();
                    Vector3[] vertices = new Vector3[4]
                    {
                        new Vector3(width / 2, height / 2, 0),
                        new Vector3(-width / 2, height / 2, 0),
                        new Vector3(width / 2, -height / 2, 0),
                        new Vector3(-width / 2, -height / 2, 0)
                    };


                    for (int i = 0; i < vertices.Length; i++)
                    {
                        vertices[i].y += height / 2;
                    }

                    mesh.vertices = vertices;

                    // 通过顶点为网格创建三角形
                    int[] triangles = new int[2 * 3]
                    {
                        0, 3, 1, 0, 2, 3
                    };
                    mesh.triangles = triangles;

                    mesh.uv = new Vector2[]
                    {
                        new Vector2(1, 1),
                        new Vector2(0, 1),
                        new Vector2(1, 0),
                        new Vector2(0, 0)
                    };
                    AssetDatabase.CreateAsset(mesh, path + "/Rander/Image/" + data.id + ".asset");
                    meshfilter.mesh = mesh;
                    material.mainTexture = texture;
                    material.SetFloat("_IsScale", 0);
                    AssetDatabase.CreateAsset(material, path + "/Rander/Image/" + data.id + ".mat");
                    meshRenderer.material = material;
                    meshcollider.sharedMesh = mesh;
                }
                    break;
                case SceneDecorationPosition.LeftSide:
                {
                    SortRenderer sortRenderer = Gobject.AddComponentUnique<SortRenderer>();
                    Vector3[] vertices = new Vector3[4]
                    {
                        new Vector3(width / 2, height / 2, 0),
                        new Vector3(-width / 2, height / 2, 0),
                        new Vector3(width / 2, -height / 2, 0),
                        new Vector3(-width / 2, -height / 2, 0)
                    };


                    for (int i = 0; i < vertices.Length; i++)
                    {
                        vertices[i].y += height / 2;
                    }

                    mesh.vertices = vertices;

                    // 通过顶点为网格创建三角形
                    int[] triangles = new int[2 * 3]
                    {
                        0, 3, 1, 0, 2, 3
                    };
                    mesh.triangles = triangles;

                    mesh.uv = new Vector2[]
                    {
                        new Vector2(1, 1),
                        new Vector2(0, 1),
                        new Vector2(1, 0),
                        new Vector2(0, 0)
                    };
                    AssetDatabase.CreateAsset(mesh, path + "/Rander/Image/" + data.id + ".asset");
                    meshfilter.mesh = mesh;
                    material.mainTexture = texture;
                    material.SetFloat("_IsScale", 0);
                    material.SetFloat("_ZWrite", 0);
                    AssetDatabase.CreateAsset(material, path + "/Rander/Image/" + data.id + ".mat");
                    meshRenderer.material = material;
                    meshcollider.sharedMesh = mesh;
                    // meshRenderer.sortingOrder = -999;
                }

                    break;
                case SceneDecorationPosition.RightSide:
                {
                    SortRenderer sortRenderer = Gobject.AddComponentUnique<SortRenderer>();
                    Vector3[] vertices = new Vector3[4]
                    {
                        new Vector3(width / 2, height / 2, 0),
                        new Vector3(-width / 2, height / 2, 0),
                        new Vector3(width / 2, -height / 2, 0),
                        new Vector3(-width / 2, -height / 2, 0)
                    };


                    for (int i = 0; i < vertices.Length; i++)
                    {
                        vertices[i].y += height / 2;
                    }

                    mesh.vertices = vertices;

                    // 通过顶点为网格创建三角形
                    int[] triangles = new int[2 * 3]
                    {
                        0, 3, 1, 0, 2, 3
                    };
                    mesh.triangles = triangles;

                    mesh.uv = new Vector2[]
                    {
                        new Vector2(1, 1),
                        new Vector2(0, 1),
                        new Vector2(1, 0),
                        new Vector2(0, 0)
                    };
                    AssetDatabase.CreateAsset(mesh, path + "/Rander/Image/" + data.id + ".asset");
                    meshfilter.mesh = mesh;
                    material.mainTexture = texture;
                    material.SetFloat("_IsScale", 0);
                    material.SetFloat("_ZWrite", 0);
                    AssetDatabase.CreateAsset(material, path + "/Rander/Image/" + data.id + ".mat");
                    meshRenderer.material = material;
                    meshcollider.sharedMesh = mesh;
                    // meshRenderer.sortingOrder = -999;
                }

                    break;
                case SceneDecorationPosition.ground:
                {
                    SortRenderer sortRenderer = Gobject.AddComponentUnique<SortRenderer>();
                    Vector3[] vertices = new Vector3[4]
                    {
                        new Vector3(width / 2, 0.1f, (height*(float)System.Math.Sqrt(3)) / 2),
                        new Vector3(-width / 2, 0.1f, (height*(float)System.Math.Sqrt(3)) / 2),
                        new Vector3(width / 2, 0.1f, (-height*(float)System.Math.Sqrt(3))/ 2),
                        new Vector3(-width / 2, 0.1f, (-height*(float)System.Math.Sqrt(3)) / 2)
                    };


                    // for (int i = 0; i < vertices.Length; i++)
                    // {
                    //     vertices[i].y += height / 2;
                    // }

                    mesh.vertices = vertices;

                    // 通过顶点为网格创建三角形
                    int[] triangles = new int[2 * 3]
                    {
                        0, 3, 1, 0, 2, 3
                    };
                    mesh.triangles = triangles;

                    mesh.uv = new Vector2[]
                    {
                        new Vector2(1, 1),
                        new Vector2(0, 1),
                        new Vector2(1, 0),
                        new Vector2(0, 0)
                    };
                    AssetDatabase.CreateAsset(mesh, path + "/Rander/Image/" + data.id + ".asset");
                    meshfilter.mesh = mesh;
                    material.mainTexture = texture;
                    material.SetFloat("_IsScale", 0);
                    material.SetFloat("_ZWrite", 0);
                    AssetDatabase.CreateAsset(material, path + "/Rander/Image/" + data.id + ".mat");
                    meshRenderer.material = material;
                    meshcollider.sharedMesh = mesh;
                    // meshRenderer.sortingOrder = -999;
                }
                    break;
                default:
                    break;
            }

            Gobject.AddComponentUnique<SceneDecorationComponent>();
            Gobject.layer = LayerMask.NameToLayer("SceneComponent");
            //Gobject.tag = "SceneComponent";
            return Gobject;
        }
        //创建人物预制体
        public static GameObject CreateRole(CreateRoleData createRoleData, string prefabPath, List<RoleAIAction> roleAIActionList = null, List<WeaknessData> weaknessDatalist = null)
        {
            GameObject Gobject = new GameObject();
            GameObject anim = Gobject.GetChild("Anim", true);
            GameObject interactive = Gobject.GetChild("InteractiveCube", true);
            GameObject shadow = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources_moved/Prefabs/Shadow/RoleShadow.prefab"));
            shadow.transform.parent = Gobject.transform;
            shadow.name = "Shadow";

            //--------添加角色控制器----------add by tianjinpeng 2018/06/14 15:43:28
            CharacterController characterController = Gobject.AddComponentUnique<CharacterController>();
            characterController.slopeLimit = 60f;
            characterController.stepOffset = 0.5f;
            characterController.height = createRoleData.size.y;
            characterController.radius = createRoleData.size.x / 2;
            characterController.center = new Vector3(0, createRoleData.size.y / 2, 0);
            
            //-----------人物控制器----------- add by tianjinpeng 2018/06/14 15:43:20
            RoleController roleController;
            switch (createRoleData.roleControllerType)
            {
                case RoleControllerType.RoleController:
                    roleController = Gobject.AddComponentUnique<RoleController>();
                    break;
                case RoleControllerType.HumanController:
                    roleController = Gobject.AddComponentUnique<HumanController>();
                    break;
                case RoleControllerType.BaronController:
                    roleController = Gobject.AddComponentUnique<BaronController>();
                    break;
                case RoleControllerType.CaptionController:
                    roleController = Gobject.AddComponentUnique<CaptionController>();
                    break;
                case RoleControllerType.PrisonerController:
                    roleController = Gobject.AddComponentUnique<PrisonerController>();
                    Gobject.AddComponentUnique<PrisonerAIController>();
                    break;
                default:
                    roleController = Gobject.AddComponentUnique<RoleController>();
                    break;
            }

            roleController.RoleData = createRoleData.roleData;

            // 是否有转身动画
            roleController.WithTrunBackAnim = createRoleData.WithTrunBackAnim;
            
            //-----------AI控制器--------------add by tianjinpeng 2018/06/14 15:43:24
            RoleBehaviorTree roleBehaviorTree = Gobject.AddComponentUnique<RoleBehaviorTree>();
            RoleNavMeshAgent roleNavMeshAgent = Gobject.AddComponentUnique<RoleNavMeshAgent>();
            roleBehaviorTree.agentTypeID = createRoleData.agentTypeID;
            roleBehaviorTree.aiScripName = createRoleData.AIname;
            if (roleAIActionList != null && roleAIActionList.Count != 0)
            {
                roleBehaviorTree.actionList = roleAIActionList;
            }
            if (createRoleData.withAI)
            {
                roleController.WithAI = createRoleData.withAI;
                BehaviorDesigner.Runtime.BehaviorTree behaviorTree = Gobject.AddComponentUnique<BehaviorDesigner.Runtime.BehaviorTree>();
                behaviorTree.ExternalBehavior = (BehaviorDesigner.Runtime.ExternalBehavior)AssetDatabase.LoadAssetAtPath(createRoleData.AIPath, typeof(BehaviorDesigner.Runtime.ExternalBehavior));
                // behaviorTree.SetVariableValue("Anim",anim);
                behaviorTree.RestartWhenComplete = true;
            }
            //-----------AI寻路障碍-----------add by tianjinpeng 2018/06/14 15:43:31
            UnityEngine.AI.NavMeshObstacle navMeshObstacle = Gobject.AddComponentUnique<UnityEngine.AI.NavMeshObstacle>();
            navMeshObstacle.size = createRoleData.size;
            navMeshObstacle.center = new Vector3(0, createRoleData.size.y / 2, 0);
            navMeshObstacle.enabled = false;

            //添加血条预设物
            if (createRoleData.IsMonsterHP)
            {
                Gobject.AddComponentUnique<MonsterHPController>();
            }
            
            //-----------添加动画相关组件--------------add by tianjinpeng 2018/06/14 15:43:35
            MeshFilter meshFilter = anim.AddComponentUnique<MeshFilter>();
            MeshRenderer meshRenderer = anim.AddComponentUnique<MeshRenderer>();
            Animator animator = anim.AddComponentUnique<Animator>();
            Spine.Unity.SkeletonAnimator skeletonAnimator = anim.AddComponentUnique<Spine.Unity.SkeletonAnimator>();

            skeletonAnimator.skeletonDataAsset = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(createRoleData.SkeletonDataAssetPath, typeof(Spine.Unity.SkeletonDataAsset));
            animator.runtimeAnimatorController = (UnityEditor.Animations.AnimatorController)AssetDatabase.LoadAssetAtPath(createRoleData.AnimControllerPath, typeof(UnityEditor.Animations.AnimatorController));
            anim.AddComponentUnique<SortRenderer>();
//            AnimController animController = anim.AddComponentUnique<AnimController>();
            skeletonAnimator.initialSkinName = createRoleData.SkinName;


            //----------添加受击区域相关组件------------add by tianjinpeng 2018/06/14 15:45:41
            if (createRoleData.damageTargetSwitch)
            {
                GameObject damageTarget = Gobject.GetChild("DamageTarget", true);

                Rigidbody rigidbody = damageTarget.AddComponentUnique<Rigidbody>();
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;

                BoxCollider DamageboxCollider = damageTarget.AddComponentUnique<BoxCollider>();
                DamageboxCollider.isTrigger = true;
                DamageboxCollider.size = createRoleData.size;
                DamageboxCollider.center = new Vector3(0, createRoleData.size.y / 2, 0);

                TriggerController DamageTrigger = damageTarget.AddComponentUnique<TriggerController>();

                damageTarget.layer = LayerMask.NameToLayer("Interaction");
                damageTarget.tag = "DamageTarget";
            }


            //---------------添加交互区域相关组件-----------------add by tianjinpeng 2018/06/14 15:46:57
            BoxCollider InteractiveBoxCollider = interactive.AddComponentUnique<BoxCollider>();
            InteractiveBoxCollider.isTrigger = true;
            InteractiveBoxCollider.center = new Vector3(0, createRoleData.size.y / 2, 0);
            InteractiveBoxCollider.size = createRoleData.size;
            TriggerController InteractiveTrigger = interactive.AddComponentUnique<TriggerController>();
            InteractiveTrigger.needKeepingGameObject = false;
            InteractiveTrigger.gameObject.layer = LayerMask.NameToLayer("Interaction");

            Rigidbody interactiveRigidbody = interactive.AddComponent<Rigidbody>();
            interactiveRigidbody.useGravity = false;
            interactiveRigidbody.isKinematic = true;
            // InteractiveTrigger.triggerData.type=TriggerType.Role;

            //--------添加弱点------------add by tianjinpeng 2018/11/14 10:25:20
            if (weaknessDatalist != null && weaknessDatalist.Count != 0)
            {
                foreach (var weaknessData in weaknessDatalist)
                {
                    GameObject gameObject = Gobject.GetChild(weaknessData.WeaknessName, true);
                    gameObject.AddComponent<TriggerControllerGroup>();
                    if (weaknessData.BoneFollow)
                    {
                        switch (weaknessData.followType)
                        {
                            case FollowType.bone:
                            {
                                Spine.Unity.BoneFollower boneFollower = gameObject.AddComponentUnique<Spine.Unity.BoneFollower>();
                                boneFollower.SkeletonRenderer = anim.GetComponent<Spine.Unity.SkeletonRenderer>();
                                boneFollower.boneName = weaknessData.BoneName;
                            }
                                break;
                            case FollowType.slot:
                            {
                                if (weaknessData.slotNameList.Count == 1)
                                {
                                    BoxCollider collider = gameObject.GetComponent<BoxCollider>();

                                    SlotFollower slotFollower = gameObject.AddComponentUnique<SlotFollower>();
                                    slotFollower.slotName = weaknessData.slotNameList[0].slotname;
                                    collider.size = new Vector3(1, 1, weaknessData.slotNameList[0].z);
                                    slotFollower.skeletonAnimator = skeletonAnimator;
                                }
                                else if (weaknessData.slotNameList.Count > 1)
                                {
                                    foreach (WeaknessslotData weaknessslot in weaknessData.slotNameList)
                                    {
                                        GameObject game = gameObject.GetChild(weaknessslot.slotname, true);
                                        BoxCollider collider = game.AddComponentIfNone<BoxCollider>();
                                        SlotFollower slotFollower = game.AddComponentUnique<SlotFollower>();
                                        slotFollower.slotName = weaknessslot.slotname;
                                        slotFollower.skeletonAnimator = skeletonAnimator;
                                        collider.isTrigger = weaknessslot.isTrigger;
                                        if (weaknessslot.isTrigger)
                                        {
                                            TriggerController trigger = game.AddComponentIfNone<TriggerController>();
                                            trigger.id = weaknessslot.TriggerId;
                                        }
                                        collider.size = new Vector3(1, 1, weaknessslot.z);
                                        game.tag = weaknessslot.tag;
                                        game.layer = LayerMask.NameToLayer(weaknessslot.layer);
                                    }
                                }
                            }
                                break;
                        }

                    }
                    BoxCollider boxCollider = gameObject.AddComponentUnique<BoxCollider>();
                    //boxCollider.isTrigger = true;
                    boxCollider.center = weaknessData.center;
                    boxCollider.size = weaknessData.size;
                    //TriggerController trigger = gameObject.AddComponentUnique<TriggerController>();
                    //trigger.triggerData.type = TriggerType.DamageTarget;
                    //Rigidbody rig= gameObject.AddComponentUnique<Rigidbody>();
                    //rig.useGravity = false;
                    //rig.isKinematic = true;
                    switch (weaknessData.colliderType)
                    {
                        case ColliderType.weakness:
                            TriggerController trigger = gameObject.AddComponentUnique<TriggerController>();
                            Rigidbody rig = gameObject.AddComponentUnique<Rigidbody>();
                            rig.useGravity = false;
                            rig.isKinematic = true;
                            boxCollider.isTrigger = true;
                            gameObject.layer = LayerMask.NameToLayer("Interaction");
                            gameObject.tag = "Weakness";
                            break;
                        case ColliderType.collider:
                            foreach (string path in weaknessData.ComponentPathList)
                            {
                                MonoScript monoScript = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                                gameObject.AddComponent(monoScript.GetClass());
                            }
                            boxCollider.isTrigger = false;
                            gameObject.layer = LayerMask.NameToLayer(weaknessData.ColliderlayerName);
                            gameObject.tag = weaknessData.CollidertagName;
                            break;
                        case ColliderType.Trigger:
                            foreach (string path in weaknessData.ComponentPathList)
                            {
                                MonoScript monoScript = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                                gameObject.AddComponent(monoScript.GetClass());
                            }
                            Rigidbody rigi = gameObject.AddComponentUnique<Rigidbody>();
                            rigi.useGravity = false;
                            rigi.isKinematic = true;
                            boxCollider.isTrigger = true;
                            gameObject.layer = LayerMask.NameToLayer(weaknessData.ColliderlayerName);
                            gameObject.tag = weaknessData.CollidertagName;
                            break;
                    }
                }
            }

            //--------------添加影子---------------add by tianjinpeng 2018/06/14 15:57:07
            ShadowProjector shadowProjector = shadow.GetComponent<ShadowProjector>();
            // shadowProjector.UVRect=new Rect(0f,0.5f,0.5f,0.5f);
            // Material material=(Material)AssetDatabase.LoadAssetAtPath("Assets/FastShadowProjector/Materials/BlobShadowAtlas.mat",typeof(Material));
            // shadowProjector._Material=material;
            shadowProjector.ShadowSize = createRoleData.size.x * 0.6f;
            shadowProjector.AutoSizeOpacity = true;
//            shadowProjector.AutoSORaycastLayer = LayerMask.NameToLayer("Ground");
            
            if (createRoleData.shadowParameterOnOff)
            {
                shadowProjector.AutoSOMaxScaleMultiplier = createRoleData.ShadowMaxScale;
                shadowProjector.AutoSOCutOffDistance = createRoleData.ShadowCutoffDistance;
            }
            else
            {
                shadowProjector.AutoSOMaxScaleMultiplier = 0.5f;
                shadowProjector.AutoSOCutOffDistance = 10f;
            }
            
//             添加受击区域
            var hurtModesObject = Gobject.GetChild("HurtModesObject", true);
            foreach (var hurtMode in createRoleData.HurtModes)
            {
                var hurtModeObject = hurtModesObject.GetChild(hurtMode.Name, true);

                foreach (var hurtPart in hurtMode.HurtPartList)
                {
                    var hurtPartObject = hurtModeObject.GetChild(hurtPart.Name, true);

                    {
                        GameObject damageTarget = hurtPartObject;

                        Rigidbody rigidbody = damageTarget.AddComponentUnique<Rigidbody>();
                        rigidbody.isKinematic = true;
                        rigidbody.useGravity = false;

                        BoxCollider DamageboxCollider = damageTarget.AddComponentUnique<BoxCollider>();
                        DamageboxCollider.isTrigger = true;
                        DamageboxCollider.size = hurtPart.Bounds.size;
                        DamageboxCollider.center = hurtPart.Bounds.center;

                        TriggerController DamageTrigger = damageTarget.AddComponentUnique<TriggerController>();

                        damageTarget.layer = LayerMask.NameToLayer("Interaction");
                        damageTarget.tag = "DamageTarget";
                    }
                }
            }

            HurtModeController hurtModeController = Gobject.AddComponent<HurtModeController>();
            for (int i = 0; i < createRoleData.HurtModes.Count; i++)
            {
                hurtModeController.HurtModes.Add(createRoleData.HurtModes[i]);
            }
            
            //-----------------添加层级-----------------add by tianjinpeng 2018/06/14 17:02:17
            Gobject.layer = LayerMask.NameToLayer("Role");

            Gobject.tag = "Role";
            anim.layer = LayerMask.NameToLayer("Role");
            interactive.layer = LayerMask.NameToLayer("Interaction");
            interactive.tag = "Interaction";
            shadow.layer = LayerMask.NameToLayer("Role");
            Tools.UpdatePrefab(Gobject, prefabPath + createRoleData.Id + ".prefab");

            return Gobject;
        }

        // 从Texture生成GameObject add by TangJian 2018/11/21 17:42
        //public static GameObject GenrateGameObjectFromTexture(Texture2D texture2D)
        //{
        //    GameObject newGo = new GameObject();

        //    MeshRenderer renderer = newGo.AddComponent<MeshRenderer>();

        //    renderer

        //    return newGo;
        //}
        
        public static Rigidbody SetPlacementRigidbody(Rigidbody target, bool AddRigidbody)
        {
            Rigidbody rigidbody = target;


            // 物件不能旋转 add by TangJian 2018/12/18 16:13
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            if (AddRigidbody)
            {
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
            else
            {
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
            }
            return rigidbody;
        }
        public static void SetPlacementRender(tian.PlacementConfig placementConfig, GameObject renderer, float imgScale, string path)
        {
            switch (placementConfig.randerChoice)
            {
                case tian.RanderChoice.Image:
                {
                    GameObject image = renderer.GetChild("Image", true);

                    byte[] bytes = File.ReadAllBytes(placementConfig.ImagePath);

                    Texture2D texture2d = new Texture2D(0, 0);

                    try
                    {
                        texture2d.LoadImage(bytes);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }

                    Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(placementConfig.ImagePath, typeof(Texture2D));


                    float width = (texture2d.width / 100f);
                    float height = (texture2d.height  / 100f);
                    if (placementConfig.scaleType == tian.ScaleType.Alone)
                    {
                        width = width * placementConfig.AloneScale;
                        height = height * placementConfig.AloneScale;
                    }
                    else
                    {
                        width = width * imgScale;
                        height = height * imgScale;
                    }

                    switch (placementConfig.colliderSizeType)
                    {
                        case tian.ColliderSizeType.semiautomatic:
                            if (placementConfig.NoSortRenderPos == false)
                            {
                                GameObject sortRenderPosObject = image.GetChild("SortRenderPos", true);
                                sortRenderPosObject.transform.localPosition = new Vector3(0, 0, placementConfig.size.z);
                            }
                            break;
                        case tian.ColliderSizeType.Manually:
                            placementConfig.size = new Vector3(width, height, placementConfig.size.z);
                            if (placementConfig.NoSortRenderPos == false)
                            {
                                GameObject sortRenderPosObject = image.GetChild("SortRenderPos", true);
                                sortRenderPosObject.transform.localPosition = new Vector3(0, 0, placementConfig.size.z);
                            }
                            break;
                        case tian.ColliderSizeType.TextureRect:
                        {
                            Rect frontRect = new Rect(
                                new Vector2(placementConfig.frontRect.position.x * texture2d.width / 100f
                                    , placementConfig.frontRect.position.y * texture2d.height / 100f * 1.1547005383792515290182975610039f)
                                , new Vector2(placementConfig.frontRect.size.x * texture2d.width / 100f
                                    , placementConfig.frontRect.size.y * texture2d.height / 100f * 1.1547005383792515290182975610039f));

                            Rect topRect = new Rect(
                                new Vector2(placementConfig.topRect.position.x * texture2d.width / 100f
                                    , placementConfig.topRect.position.y * texture2d.height / 100f * 1.1547005383792515290182975610039f)
                                , new Vector2(placementConfig.topRect.size.x * texture2d.width / 100f
                                    , placementConfig.topRect.size.y * texture2d.height / 100f * 1.1547005383792515290182975610039f));

                            // placementConfig.size = new Vector3(frontRect.width, frontRect.height, topRect.height);

                            placementConfig.colliderBounds.center =
                                new Vector3(-(texture2d.width / 100f) / 2 + frontRect.position.x + frontRect.width / 2f,

                                    frontRect.height / 2f,

                                    (frontRect.y * 2f) + (topRect.height * 2f) / 2f
                                );

                            placementConfig.colliderBounds.size = new Vector3(frontRect.size.x, frontRect.size.y, topRect.size.y * 2);


                            // image.AddComponentUnique<SortRenderer>();
                            if (placementConfig.NoSortRenderPos == false)
                            {
                                GameObject sortRenderPosObject = image.GetChild("SortRenderPos", true);
                                sortRenderPosObject.transform.localPosition = new Vector3(0, 0, placementConfig.colliderBounds.max.z);
                            }
                                    
                        }
                            break;
                    }

                    Debug.Log("id:" + placementConfig.id + "  " + "width:" + texture2d.width + " " + "height:" + texture2d.height);
                    // textureimporter.npotScale = saa;
                    // assetImporter = (AssetImporter)textureimporter;
                    // assetImporter.SaveAndReimport();


                    MeshFilter meshfilte = image.AddComponentUnique<MeshFilter>();
                    MeshRenderer meshRenderer = image.AddComponentUnique<MeshRenderer>();

                    image.AddComponentUnique<SortRenderer>();

                    Shader shader = Shader.Find("Custom/Unlit/Transparent");
                    {
                        Mesh mesh = new Mesh();

                        // 为网格创建顶点数组
                        Vector3[] vertices = new Vector3[4]
                        {
                            new Vector3(width / 2, height, 0),
                            new Vector3(-width / 2, height, 0),
                            new Vector3(width / 2, 0, 0),
                            new Vector3(-width / 2, 0, 0)
                        };
                        switch (placementConfig.sceneDecorationPosition)
                        {
                            case SceneDecorationPosition.Positive:
                                vertices = new Vector3[4]
                                {
                                    new Vector3(width / 2, height, 0),
                                    new Vector3(-width / 2, height, 0),
                                    new Vector3(width / 2, 0, 0),
                                    new Vector3(-width / 2, 0, 0)
                                };
                                break;
                            case SceneDecorationPosition.ground:
                                height = ((texture2d.height * (((float)System.Math.Sqrt(3) / 3f) * 2)) / 100f);
                                vertices = new Vector3[4]
                                {
                                    new Vector3(width / 2, 0 , (height*(float)System.Math.Sqrt(3))),
                                    new Vector3(-width / 2, 0 , (height*(float)System.Math.Sqrt(3))),
                                    new Vector3(width / 2, 0, 0),
                                    new Vector3(-width / 2, 0, 0)
                                };
                                break;
                            case SceneDecorationPosition.LeftSide:
                                vertices = new Vector3[4]
                                {
                                    new Vector3(width / 2, height, 0),
                                    new Vector3(-width / 2, height, 0),
                                    new Vector3(width / 2, 0, 0),
                                    new Vector3(-width / 2, 0, 0)
                                };
                                break;
                            case SceneDecorationPosition.RightSide:
                                vertices = new Vector3[4]
                                {
                                    new Vector3(width / 2, height, 0),
                                    new Vector3(-width / 2, height, 0),
                                    new Vector3(width / 2, 0, 0),
                                    new Vector3(-width / 2, 0, 0)
                                };
                                break;
                            default:
                                break;
                        }

                        // for (int i = 0; i < vertices.Length; i++)
                        // {
                        //     vertices[i].z = vertices[i].z - height / 2;
                        // }

                        mesh.vertices = vertices;

                        // 通过顶点为网格创建三角形
                        int[] triangles = new int[2 * 3]
                        {
                            0, 3, 1, 0, 2, 3
                        };
                        mesh.triangles = triangles;

                        mesh.uv = new Vector2[]
                        {
                            new Vector2(1, 1),
                            new Vector2(0, 1),
                            new Vector2(1, 0),
                            new Vector2(0, 0)
                        };

                        AssetDatabase.CreateAsset(mesh, path + "/Render/Image/" + placementConfig.id + ".asset");
                        meshfilte.mesh = mesh;
                    }
                        
                    var setMaterial = AssetDatabase.LoadAssetAtPath<Material>(placementConfig.materialPath);
                    if (setMaterial == null)
                    {
                        Material material = new Material(shader);
                        material.mainTexture = texture;
                        material.renderQueue = 3000;

                        material.SetFloat("_ZWrite", 0);
                        material.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                        material.SetFloat("_IsScale", 0);
                            
                        AssetDatabase.CreateAsset(material, path + "/Render/Image/" + placementConfig.id + ".mat");
                        meshRenderer.material = material;
                            
                        meshRenderer.transform.localScale = new Vector3(1, 1.1547005383792515290182975610039f, 1);
                    }
                    else
                    {
                        meshRenderer.material = setMaterial;
                        meshRenderer.transform.localScale = new Vector3(1, 1.1547005383792515290182975610039f, 1);
                    }
                }
                    break;
                case tian.RanderChoice.Anim:

                    GameObject anim = renderer.GetChild("Anim", true);

                    MeshFilter meshfilter = anim.AddComponentUnique<MeshFilter>();
                    MeshRenderer MeshRenderer = anim.AddComponentUnique<MeshRenderer>();

                    Animator animator = anim.AddComponentUnique<Animator>();

                    Spine.Unity.SkeletonAnimator skeletonAnimator = anim.AddComponentUnique<Spine.Unity.SkeletonAnimator>();

                    anim.AddComponentUnique<SortRenderer>();

                    skeletonAnimator.skeletonDataAsset = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(placementConfig.SkeletonDataAssetpath, typeof(Spine.Unity.SkeletonDataAsset));

                    animator.runtimeAnimatorController = (UnityEditor.Animations.AnimatorController)AssetDatabase.LoadAssetAtPath(placementConfig.AnimatorControllerpath, typeof(UnityEditor.Animations.AnimatorController));

                    // 帧事件分发器 add by TangJian 2018/12/18 16:51
                    anim.AddComponentUnique<AnimController>();
                    break;
                default:
                    break;
            }

        }
        public static GameObject SetPlacementCoillder(tian.PlacementConfig placementConfig, GameObject collider, string name = "boxc")
        {
            GameObject gocollider = collider.GetChild(name, true);

            string Layer = "";
            switch (placementConfig.colliderlayer)
            {
                case tian.ColliderLayer.Placement:
                    Layer = "Placement";
                    break;
                case tian.ColliderLayer.SmallPlacement:
                    Layer = "SmallPlacement";
                    break;
                case tian.ColliderLayer.Ground:
                    Layer = "Ground";
                    break;
                case tian.ColliderLayer.Wall:
                    Layer = "Wall";
                    break;
                default:
                    break;
            }
            CreatePlentCollider(gocollider, "Placement", Layer, placementConfig.colliderSizeType, placementConfig.size, placementConfig.boundsPosition, placementConfig.colliderBounds, false);
            if (placementConfig.NoNavMeshObstacle == false)
            {
                UnityEngine.AI.NavMeshObstacle navMeshObstacl = gocollider.AddComponentUnique<UnityEngine.AI.NavMeshObstacle>();
                SetNavMeshObstacle(navMeshObstacl, placementConfig.colliderSizeType, placementConfig.size, placementConfig.boundsPosition, placementConfig.colliderBounds);
            }
            

            return gocollider;
        }
        public static GameObject SetPlacementTriggercollider(tian.PlacementConfig placementConfig, GameObject trigger, bool AddTriggerController = true, string name = "DamageTarget", string tag = "DamageTarget", string layer = "Interaction")
        {
            GameObject Damagecollider = trigger.GetChild(name, true);

            CreatePlentCollider(Damagecollider, tag, layer, placementConfig.colliderSizeType, placementConfig.size, placementConfig.boundsPosition, placementConfig.colliderBounds, true);
            if (AddTriggerController)
            {
                TriggerController triggerController = Damagecollider.AddComponentUnique<TriggerController>();
            }
            //支持位移碰撞
            if (placementConfig.placementShake)
            {
                GameObject ShakeCollider = trigger.GetChild("ShakeTarget", true);
                CreatePlentCollider(ShakeCollider,"Interaction",layer, placementConfig.colliderSizeType, placementConfig.size, placementConfig.boundsPosition, placementConfig.colliderBounds, true);
                if (AddTriggerController)
                {
                    TriggerController triggerController = ShakeCollider.AddComponentUnique<TriggerController>();
                }
            }
            return Damagecollider;
        }
        public static void CreatePlentCollider(GameObject gameObject, string tag, string layer, tian.ColliderSizeType colliderSizeType, Vector3 size, Vector3 boundsPosition, Bounds colliderBounds, bool isTrigger)
        {
            gameObject.layer = LayerMask.NameToLayer(layer);
            gameObject.tag = tag;

            BoxCollider boxCollider = gameObject.AddComponentUnique<BoxCollider>();
            boxCollider.isTrigger = isTrigger;
            if (colliderSizeType == tian.ColliderSizeType.TextureRect)
            {
                boxCollider.size = colliderBounds.size;
                boxCollider.center = colliderBounds.center;
            }
            else if (colliderSizeType == tian.ColliderSizeType.Manually)
            {
                boxCollider.size = size;
                boxCollider.center = boundsPosition;
            }
            else
            {
                boxCollider.size = size;
                boxCollider.center = new Vector3(0, size.y / 2, 0);
            }
        }
        public static void SetNavMeshObstacle(UnityEngine.AI.NavMeshObstacle navMeshObstacle, tian.ColliderSizeType colliderSizeType, Vector3 size, Vector3 boundsPosition, Bounds colliderBounds)
        {
            if (colliderSizeType == tian.ColliderSizeType.TextureRect)
            {
                navMeshObstacle.size = colliderBounds.size;
                navMeshObstacle.center = colliderBounds.center;
            }
            else if (colliderSizeType == tian.ColliderSizeType.Manually)
            {
                navMeshObstacle.size = size;
                navMeshObstacle.center = boundsPosition;
            }
            else
            {
                navMeshObstacle.size = size;
                navMeshObstacle.center = new Vector3(0, size.y / 2, 0);
            }
            navMeshObstacle.carving = true;
        }
        public static GameObject GenrateGameObjectFromSprite(Sprite sprite)
        {
            GameObject newGo = new GameObject();

            SpriteRenderer spriteRenderer = newGo.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;

            return newGo;
        }
    }
}