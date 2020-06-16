//#define TANG_DEBUG





namespace Tang
{
    public static class Definition
    {
        public static bool Debug = false;

    
        public static string SkillListPath = "Resources_moved/Scripts/Skill";
        public static string SpineAssetPath = "Assets/Resources_moved/Spine";
        
        public static string BehaviorTreePath = "Assets/Behavior Designer/AI";
        
        public static string BothAssetPath = "Assets/Resources_moved/Scripts/BothEffectAnim";
        public static string HitTerrainAssetPath = "Assets/Resources_moved/Scripts/HitTerrainEffect";
        public const string ExcelPathData = "Assets/Resources_moved/Scripts/ExcelPathData";

        public const string AudioAssetPath = "Assets/Resources_moved/Scripts/AudioAsset";

        public static string RoleAssetPrefix = "Roles/Monster/";

        public static string DropItemAssetPrefix = "Assets/Resources_moved/Prefabs/DropItem/";

        public static string TextureAssetPrefix = "Assets/Resources_moved/Textures/";

        public static string IconPath = TextureAssetPrefix + "/Icon";
        
        public static readonly float SideWallAngle = 63.43495f;

        public static string WalkOnGroundEffectFile = "Assets/Resources_moved/Scripts/WalkOnGroundEffect";
        public static string WalkOnGroundEffectFileName = "WalkOnGroundEffect";
    }
}