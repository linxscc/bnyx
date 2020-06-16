namespace Tang
{
    // 剑: swd
    // 巨剑: lswd
    // 钝器: blunt
    // 太刀: katana
    // 匕首: sswd
    public enum WeaponType
    {
        #region //无武器状态 2019.3.27
        None = 0,        
        #endregion
        
        Swd = 1, // 剑 add by TangJian 2017/08/09 11:34:17
        Lswd = 2, // 巨剑 add by TangJian 2017/08/09 11:34:16
        blunt = 3, // 钝器 add by TangJian 2017/08/09 11:34:28
        Katana = 4, // 太刀 add by TangJian 2017/08/09 11:34:40
        Sswd = 5, // 匕首 add by TangJian 2017/08/09 11:35:02
        Saxe = 6, // 短斧
		Shield = 7 // 盾
    }
}