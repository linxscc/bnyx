namespace Tang
{
    public enum DamageDirectionType
    {
        Directional = 0, // 朝向的 add by TangJian 2017/07/31 16:25:13
        Radial = 1, // 放射状的 add by TangJian 2017/07/31 16:25:14
        RadialWithoutY = 2, // 放射状的 add by TangJian 2017/07/31 16:25:14
        DirectionalWithoutY = 3,
        UseSpeed = 4,
    }
    
    public enum DamageForceType
    {
        light,
        moderate,
        heavy
    }
    public enum RecordType
    {
        Record,
        NotRecord
    }
}