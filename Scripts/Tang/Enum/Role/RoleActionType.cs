namespace Tang
{
    public enum RoleActionType
    {
        None = 0,

        MoveUp_Begin,
        MoveUp_End,

        MoveDown_Begin,
        MoveDown_End,

        MoveLeft_Begin,
        MoveLeft_End,

        MoveRight_Begin,
        MoveRight_End,
        
        
        Move_Horizonal,
        Move_Vertical,

        WalkCut_Begin,
        WalkCut_End,
        
        // 轻重攻击 add by TangJian 2017/08/28 17:17:35
        Action1_Begin,
        Action1_End,

        Action2_Begin,
        Action2_End,

        Action3_Begin,
        Action3_End,

        Action4_Begin,
        Action4_End,

        Action5_Begin,
        Action5_End,

        Action6_Begin,
        Action6_End,

        Action7_Begin,
        Action7_End,

        Action8_Begin,
        Action8_End,

        Action9_Begin,
        Action9_End,
        
        // 跳跃 add by TangJian 2017/08/28 17:17:20
        Jump_Begin,
        Jump_End,

        // 翻滚 add by TangJian 2017/08/28 17:19:18
        Roll_Begin,
        Roll_End,

        //冲刺
        Rush_Begin,
        Rush_End,

        // 技能 add by TangJian 2017/08/28 20:57:24
        Skill1_Begin,
        Skill1_End,
        Skill2_Begin,
        Skill2_End,

        // 交互 add by TangJian 2017/08/28 17:20:25
        Interact_Begin,
        Interact_End,

        // 使用 add by tianjinpeng 2018/02/07 15:13:39
        Use_Begin,
        Use_End,

    }
}