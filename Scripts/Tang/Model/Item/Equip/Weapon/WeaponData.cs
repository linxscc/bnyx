namespace Tang
{
    [System.Serializable]
    public class WeaponData : EquipData
    {
        public string weaponAttachmentName;
        public string mainHandAttachmentName;
        public string offHandAttachmentName;
        
        public AtkPropertyType atkPropertyType = AtkPropertyType.physicalDamage;
        public DamageEffectType damageEffectType = DamageEffectType.Strike;
        
        public PossessType possessType;
        
        public bool IsOneHanded()
        {
            return equipType == EquipType.Swd || equipType == EquipType.Sswd;
        }
        
        public bool IsShield()
        {
            return equipType == EquipType.Shield;
        }
    }
}