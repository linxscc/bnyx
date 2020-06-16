using System;




namespace Tang
{
    public enum SoulType
    {
        None = 0,
        time = 1,
        NewRoom = 2
    }
    [Serializable]
    public class SoulData : EquipData
    {
        
        public string skillId = "";        
        public SoulCharging soulCharging= new SoulCharging();
        
    }
    public class SoulCharging
    {
        public int currNewRoomChargingcount = 0;
        public float usetime = 0;
        public float cd =0;
        public int int1 = 0;
        public int Chargingcount = 0;
        public SoulType soulType=SoulType.None;
        public bool CanUse()
        {
            float time=UnityEngine.Time.time;
            bool bld=true;
            switch (soulType)
            {
                case SoulType.time:
                if (time-usetime>cd||usetime==0)
                {
                    bld= true;
                }else
                {
                    bld= false;
                }
                break;
                case SoulType.NewRoom:
                if (currNewRoomChargingcount>=Chargingcount)
                {
                    bld= true;
                }else
                {
                    bld= false;
                }
                break;

                default:
                    bld= true;
                break;
            }
            return bld;
        }
        public void Reset()
        {
            switch (soulType)
            {
                case SoulType.time:
                    usetime=UnityEngine.Time.time;
                break;
                default:
                break;
            }
        }
    }
}