using UnityEngine;

namespace Tang
{
    [System.Serializable]
    public class AnimAttackEventData
    {
        public string animName;
        // 位置 add by TangJian 2017/07/24 15:13:54
        public Vector3 pos = new Vector3(0, 0, 0);
        // 大小 add by TangJian 2017/07/24 15:13:48
        public Vector3 size = new Vector3(1, 1, 1);
        // 攻击力道 add by TangJian 2017/07/24 15:18:05
        public Vector3 force = new Vector3(1, 0, 0);
        public Vector3 targetMoveBy = new Vector3(0, 0, 0);
        // 攻击方向 add by TangJian 2017/07/24 15:29:10
        public Direction direction = Direction.Right;
        // 攻击类型 add by TangJian 2017/07/24 15:29:17
        public HitType hitType = HitType.Down;
        // 攻击力道 add by TangJian 2017/07/24 15:29:56
        public float atk = 1;
        // 体力消耗 add by TangJian 2018/01/17 15:09:46
        public float tiliCost = 10;
    }
}