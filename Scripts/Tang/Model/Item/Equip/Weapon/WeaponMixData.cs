using UnityEngine;


namespace Tang
{
    [System.Serializable]
    public class WeaponMixData
    {
        public WeaponMixData(EquipType mainHand, EquipType offHand)
        {
            this.mainHand = mainHand;
            this.offHand = offHand;
        }
        public EquipType mainHand = EquipType.None;
        public EquipType offHand = EquipType.None;

        public RuntimeAnimatorController runtimeAnimatorController;
    }
}