namespace Tang
{
    public enum AttrType
    {
        Hp = 1,
        Mp = 2,

        HpMax = 3,
        MpMax = 4,

        Atk = 5,
        AtkMin = 6,

        AtkMax = 7,
        MagicalMin = 8,
        MagicalMax = 9,

        Def = 10,
//        AtkSpeed = 11,
//        MoveSpeed = 12,

        CriticalRate = 13,
        CriticalDamage = 14,

        Tili = 15, // 体力 add by TangJian 2018/01/17 15:11:27
        TiliMax = 16, // 体力最大值 add by TangJian 2018/01/17 15:11:36

        WalkSpeed = 17,
        RunSpeed = 18,

        AtkSpeedScale = 19,
        MoveSpeedScale = 20,

        // 韧性 add by TangJian 2018/12/12 12:14
        Poise = 21,

        //韧性最大值 add by TangJian 2018/12/12 12:15
        PoiseMax = 22,

        // 削韧 add by TangJian 2018/12/12 12:15
        PoiseCut = 23,

        // 韧性倍率 add by TangJian 2018/12/12 14:54
        PoiseScale = 24,
        
        //质量
        Mass=25,
        
        // 当前血量百分比 add by TangJian 2019/3/23 14:58
        HpPercent = 26,
        
        // 霸体 add by TangJian 2019/3/23 15:42
        SuperArmor = 27,
        
        // 冲刺速度 add by TangJian 2019/4/16 18:47
        RushSpeed = 28,
        
        // 精力
        Vigor = 29,
        VigorMax = 30,
        
        // 力量
        Strength = 31,
        
        // 重量
        Weight = 32,
        
        LessenHurt = 33
        
    }
}