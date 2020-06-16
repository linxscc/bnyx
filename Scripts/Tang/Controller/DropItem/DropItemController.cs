using UnityEngine;
using DG.Tweening;
using System.IO;
using Spine;
using Spine.Unity;

namespace Tang
{
    public class DropItemController : SceneObjectController, ITriggerDelegate
    {
        [SerializeField] private bool isSpriteRender = false;

        [SerializeField] public ItemData itemData;
        public ItemData ItemData { get { return itemData; } set { itemData = value; } }

        // 物品id add by TangJian 2018/01/22 22:30:35
        [SerializeField] private string itemId;
        public string ItemId { get { return itemId; } set { itemId = value; } }

        // 物品数目 add by TangJian 2018/01/22 22:31:02
        [SerializeField] public int itemCount = 1;
        public int ItemCount { get { return itemCount; } set { itemCount = value; } }

        public bool IsSpriteRender { get { return isSpriteRender; } set { isSpriteRender = value; } }

        Sequence animSequence;

        PickUpMethod pickUpMethod = PickUpMethod.Interact;

        MovementController movementController = new MovementController();

        Rigidbody mainRigidbody;

        [SerializeField] float acceleratedSpeedScale = 50f;

        bool isFlying = false;

        private bool pickUpFinish = false;
        
        GameObject flyTarget;

        System.Action pickUpAction;

        private SkeletonAnimation _skeletonAnimation;
        
        FunctionPool _functionPool = new FunctionPool();
        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            
            mainRigidbody = GetComponent<Rigidbody>();
        }

        public async override void Start()
        {
            base.Start();
            
            if (ItemData.renderType == RenderType.Image)
            {
                gameObject.layer = LayerMask.NameToLayer("CollideWithGround");
                bool sdsa = true;
                var itemData = ItemManager.Instance.getItemDataById<ItemData>(ItemId);
                mainRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                if (ItemData.itemType == ItemType.Equip)
                {
                    sdsa = false;
                    var itemDatad = ItemManager.Instance.getItemDataById<EquipData>(ItemId);
                    if (itemDatad.imgGround == ImgGround.ground)
                    {
                        Texture2D texture = await AssetManager.LoadAssetAsync<Texture2D>(Definition.TextureAssetPrefix + "Icon/" + itemDatad.groundImgId + ".png");
                        Texture2D texture2d = texture;
                        float width = texture2d.width / 100f;
                        float height = texture2d.height / 100f;
                        MeshFilter meshFilter = gameObject.GetChild("Renderer").GetComponentInChildren<MeshFilter>();
                        MeshRenderer meshRenderer = gameObject.GetChild("Renderer").GetComponentInChildren<MeshRenderer>();
                        Material blackMaterial;
                        // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
                        {
                            // Shader shader = Shader.Find("Custom/Unlit/Transparent");
                            // Shader shader = Shader.Find("Spine/W_Sprite");
                            Shader shader = Shader.Find("Custom/Unlit/Transparent");

                            {
                                Mesh mesh = new Mesh();

                                // 为网格创建顶点数组
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

                            material.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                            material.SetFloat("_ZWrite", 1);

                            meshRenderer.sortingOrder = 0;
                            meshRenderer.material = material;
                            // meshRenderer.material.color=Color.black;
                            blackMaterial = material;
                        }

                        // 根据渲染设置碰撞区域 add by TangJian 2017/11/18 17:32:18
                        {
                            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

                            Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);
                            bounds.Encapsulate(meshRenderer.bounds);
                            bounds.center = gameObject.transform.InverseTransformPoint(bounds.center);

                            boxCollider.center = new Vector3(0, 0.5f, 0); ;
                            boxCollider.size = new Vector3(bounds.size.x, 1f, bounds.size.z);

                            boxCollider.enabled = true;

                            BoxCollider InteractBoxCollider = gameObject.GetChild("Interact").GetComponent<BoxCollider>();
                            InteractBoxCollider.center = boxCollider.center;
                            InteractBoxCollider.size = boxCollider.size + new Vector3(1, 1, 1);
                        }

                        float scale = 1.25f;

                        gameObject.transform.localScale = new Vector3(1, 1, 1) * scale;

                        // 影子设置 add by TangJian 2017/12/20 15:44:55
                        // {
                        //     BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

                        GameObject shadow = gameObject.GetChild("Shadow");
                        shadow.SetActive(false);

                        // ShadowProjector shadowProjector=shadow.GetComponent<ShadowProjector>();
                        // shadowProjector._Material=blackMaterial;
                        // shadowProjector.UVRect=new Rect(0f,0.5f,0.5f,0.5f);
                        // Bounds blackbound = new Bounds(gameObject.transform.position, Vector3.zero);
                        // blackbound.Encapsulate(meshRenderer.bounds);
                        // blackbound.center = gameObject.transform.InverseTransformPoint(blackbound.center);

                        // shadowProjector.UVRect=new Rect(0f,0.5f,0.5f,0.5f);


                        // shadowProjector.AutoSizeOpacity=true;
                        // shadowProjector.AutoSOMaxScaleMultiplier=0.5f;
                        // shadowProjector.ShadowSize = blackbound.size.x;
                        // ShadowProjector shadowProjector = shadow.GetComponent<ShadowProjector>();
                        // shadow.transform.localPosition = new Vector3(0, 0, 0);
                        // Projector Projector=shadow.GetComponent<Projector>();
                        // blackMaterial.color=Color.black;
                        // Projector.material=blackMaterial;
                        //     shadowController.shadowSize = boxCollider.size.x * gameObject.transform.localScale.x;
                        //     shadowController.shadowSize *= 0.6f;
                        // }

                    }
                    else
                    {
                        sdsa = true;
                    }
                }
                if (itemData.icon.IsValid() && sdsa)
                {
                    Texture2D texture = await AssetManager.LoadAssetAsync<Texture2D>(Definition.TextureAssetPrefix + "Icon/" + itemData.icon + ".png");
                    
                    float width = texture.width / 100f;
                    float height = texture.height / 100f;

                    MeshFilter meshFilter = gameObject.GetChild("Renderer", true).GetComponentInChildren<MeshFilter>();
                    MeshRenderer meshRenderer = gameObject.GetChild("Renderer", true).GetComponentInChildren<MeshRenderer>();
                    gameObject.GetChild("Renderer").AddComponent<SortRenderer>();
                    // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
                    {
                        // Shader shader = Shader.Find("Custom/Unlit/Transparent");
                        // Shader shader = Shader.Find("Spine/W_Sprite");
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

                            for (int i = 0; i < vertices.Length; i++)
                            {
                                vertices[i].y += height / 2;
                            }

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

                        material.SetFloat("_Cutoff", 0.2f);
                        material.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                        material.SetFloat("_ZWrite", 1);
                        //material.SetFloat("_ZTest", 1);

                        meshRenderer.material = material;
                    }

                    // 根据渲染设置碰撞区域 add by TangJian 2017/11/18 17:32:18
                    {
                        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

                        Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);
                        bounds.Encapsulate(meshRenderer.bounds);
                        bounds.center = gameObject.transform.InverseTransformPoint(bounds.center);

                        boxCollider.center = bounds.center;
                        boxCollider.size = bounds.size;

                        BoxCollider InteractBoxCollider = gameObject.GetChild("Interact").GetComponent<BoxCollider>();
                        InteractBoxCollider.center = boxCollider.center;
                        InteractBoxCollider.size = boxCollider.size + new Vector3(1, 1, 1);
                    }

                    float scale = 1.25f;

                    gameObject.transform.localScale = new Vector3(1, 1, 1) * scale;

                    // 影子设置 add by TangJian 2017/12/20 15:44:55
                    {
                        // BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

                        GameObject shadow = gameObject.GetChild("Shadow");
                        ShadowProjector shadowProjector = shadow.GetComponent<ShadowProjector>();
                        // shadowProjector.UVRect=new Rect(0f,0.5f,0.5f,0.5f);

                        // shadowProjector.UVRect=new Rect(0f,0.5f,0.5f,0.5f);
                        // Material material=(Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Standard Assets/Effects/Projectors/Materials/ShadowProjector.mat",typeof(Material));
                        // shadowProjector._Material=material;

                        // shadowProjector.AutoSizeOpacity=true;
                        // shadowProjector.AutoSOMaxScaleMultiplier=0.5f;
                        shadowProjector.ShadowSize = width * 0.6f;
                        // shadowProjector.AutoSizeOpacity=true;
                        // shadowProjector.AutoSOMaxScaleMultiplier=0.5f;
                        // ShadowController shadowController = shadow.GetComponent<ShadowController>();
                        // shadow.transform.localPosition = new Vector3(0, 0, 0);
                        // shadowController.shadowSize = boxCollider.size.x * gameObject.transform.localScale.x;
                        // shadowController.shadowSize *= 0.6f;
                    }
                }

                if (animSequence == null && sdsa)
                {
                    animSequence = DOTween.Sequence();
                    // 播放动画 add by TangJian 2017/12/25 15:43:55            
                    animSequence.Append(gameObject.GetChild("Renderer").transform.DOLocalMoveY(0.2f, 2f));
                    animSequence.Append(gameObject.GetChild("Renderer").transform.DOLocalMoveY(0f, 2f)).OnComplete(() =>
                    {
                        animSequence.Restart();
                    });
                }
            }
            else if (ItemData.renderType == RenderType.Anim)
            {
                GameObject animObject = gameObject.GetChild("Renderer").GetChild("anim", true);
                _skeletonAnimation = animObject.AddComponentIfNone<SkeletonAnimation>();
                _skeletonAnimation.skeletonDataAsset = await AssetManager.LoadAssetAsync<SkeletonDataAsset>(ItemData.anim);
                _skeletonAnimation.Initialize(true);
                _skeletonAnimation.state.SetAnimation(0, itemData.idleAnim, true);
//                _functionPool.AddFunc(() =>
//                {
//                    if (_skeletonAnimation != null && _skeletonAnimation.state != null)
//                    {
//                        _skeletonAnimation.state.SetAnimation(0, itemData.idleAnim, true);
//                        return true;
//                    }
//
//                    return false;
//                });
            }

            InitPickUpMethod();
        }

        void InitPickUpMethod()
        {
            pickUpMethod = itemData.pickUpMethod;
            switch (pickUpMethod)
            {
                case PickUpMethod.Interact:
                    break;
                case PickUpMethod.FlyIn:
                    // movementController.Init(transform.position, (Vector3 position) => { transform.position = position; });
                    break;
                default:
                    Debug.LogError("未知捡取方式");
                    break;
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            _functionPool.CallFuncs();
            
            if (pickUpFinish == false && pickUpMethod == PickUpMethod.FlyIn)
            {
                if (flyTarget != null)
                {
                    // 跟踪玩家 add by TangJian 2018/01/24 20:01:41                
                    Vector3 acceleratedSpeed = (flyTarget.transform.position - gameObject.transform.position).normalized * acceleratedSpeedScale;
                    mainRigidbody.AddForce(acceleratedSpeed);

                    if ((flyTarget.transform.position - gameObject.transform.position).magnitude <= 1)
                    {
                        pickUpAction();
                        RemoveSelf();
                    }
                }
            }

            if (transform.localPosition.magnitude > 500f)
            {
                RemoveSelf();
            }
        }
        
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool OnEvent(Event evt)
        {
            return true;
        }

        public void OnTriggerIn(TriggerEvent evt)
        {
            if (pickUpMethod == PickUpMethod.FlyIn)
            {
                if (isFlying == false)
                {
                    GameObject otherObject = evt.otherTriggerController.ITriggerDelegate.GetGameObject();
                    if (otherObject.name == "Player1")
                    {
                        flyTarget = otherObject;

                        pickUpAction = () =>
                        {
                            Event e = new Event();
                            e.Type = EventType.ItemPickUp;
                            e.Data = itemData;
                            evt.otherTriggerController.ITriggerDelegate.OnEvent(e);
                            pickUpFinish = true;
                        };
                    }
                }
            }
        }
        public void RemoveSelf()
        {
            if (ItemData != null)
            {
                if (ItemData.renderType == RenderType.Anim)
                {
                    if (ItemData.destoryAnim != null)
                    {
                        _skeletonAnimation.gameObject.layer = LayerMask.NameToLayer("Effect");
                        _skeletonAnimation.state.SetAnimation(0, ItemData.destoryAnim, false);
                        _skeletonAnimation.state.End += (TrackEntry trackEntry) =>
                        {
                            Destroy(gameObject);
                        };
                        return;
                    }
                }
            }
            
            Destroy(gameObject);
        }
        
        public void OnTriggerOut(TriggerEvent evt)
        {

        }
        
        public void OnTriggerKeep(TriggerEvent evt)
        {

        }
    }
}