using System;
using UnityEngine;
using System.Collections.Generic;







namespace tian
{
    public enum ColliderSizeType
    {
        semiautomatic = 0,
        Manually = 1,
        TextureRect = 2,
    }
    public enum ScaleType
    {
        Alone = 0,
        Whole = 1,
    }
    public enum ColliderLayer
    {
        SmallPlacement = 0,
        Placement = 1,
        Ground = 2,
        Wall = 3
    }
    public enum HporCount
    {
        None = 0,
        Hp = 1,
        count = 2,
    }
    public enum PlacementType
    {
        None = 0,
        TreasureBox = 1,
        trap = 2,
        bucket = 3,
        Ladder = 4,
        FenceDoor = 5,
        SceneDecoration = 6,
        Joystick = 7,
        TriggerBox = 8,
    }
    public enum RanderChoice
    {
        None = 0,
        Image = 1,
        Anim = 2,

    }
    public enum TrapType
    {
        None = 0,
        GroundStab = 1,
    }

    public enum RendererOrientation
    {
        Screen = 0,
        Up = 1
    }

    public class PlacementConfig
    {
        // public UnityEditor.Animations.AnimatorController AnimatorController;
        public ColliderSizeType colliderSizeType = ColliderSizeType.semiautomatic;
        public string AnimatorControllerpath;
        public string SkeletonDataAssetpath;
        public string ImagePath;
        public string tagstring;

        public bool NoSortRenderPos = false;
        public bool NoNavMeshObstacle = false;
        public bool AddRigidbody = false;
        public ColliderLayer colliderlayer = ColliderLayer.Placement;
        public Tang.SceneDecorationPosition sceneDecorationPosition = Tang.SceneDecorationPosition.Positive;
        public RanderChoice randerChoice = RanderChoice.None;

        public Vector3 boundsPosition = Vector3.zero;

        public Vector3 size = Vector3.zero;

        public Rect frontRect;
        public Rect topRect;
        public Bounds colliderBounds;

        public float AloneScale = 1;

        public TrapType trapType = TrapType.None;
        public int firstState = 0;
        public int trapState = 0;
        public float float1 = 0;
        public float float2 = 0;
        public float float3 = 0;
        public ScaleType scaleType = ScaleType.Whole;

        public Tang.laddertype Laddertype = Tang.laddertype.Right;


        public string materialPath;

        public PlacementData placementData = new PlacementData();
        public PlacementType placementType { get { return placementData.placementType; } set { placementData.placementType = value; } }
        public string id { get { return placementData.id; } set { placementData.id = value; } }
        public string prefab { get { return placementData.prefab; } set { placementData.prefab = value; } }
        public int atk { get { return placementData.atk; } set { placementData.atk = value; } }
        public Vector3 position { get { return placementData.position; } set { placementData.position = value; } }
        // public List<Tang.DropItem> dropItemList { get { return placementData.dropItemList; } set { placementData.dropItemList = value; } }

        // 渲染节点朝向 add by TangJian 2018/12/18 15:52
        public RendererOrientation RendererOrientation = RendererOrientation.Screen;
        
        
        
        // 材质类型
        public MatType matType {
            get
            {
                return placementData.matType;
            }

            set { placementData.matType = value; }
        }

        public HitEffectType hitEffectType
        {
            get
            {
                return placementData.hitEffectType;
            }

            set { placementData.hitEffectType = value; }            
        }
        
        public bool placementShake =false;
    }

    [Serializable]
    public class PlacementData
    {
        public PlacementType placementType = PlacementType.None;
        public string id = "";
        public string prefab = "";
        public int atk = 10;
        public Vector3 position;
        public List<Tang.DropItem> dropItemList = new List<Tang.DropItem>();
        public string deathEffect = null;
        public HporCount hporCount = HporCount.None;
        public float atkhp = 0;
        public int atkcount = 0;
        public string teamId = "3";
        public int initialState = 0;
        public MatType matType = MatType.Body;
        public HitEffectType hitEffectType = HitEffectType.lswd;
    }
}