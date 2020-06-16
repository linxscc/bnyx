using UnityEngine;

namespace Tang
{
    public class GroundStabAnimController : MonoBehaviour
    {

        GroundStabController groundStab;

        void Start()
        {
            groundStab = GetComponentInParent<GroundStabController>();
        }

        /// <summary>
        /// 触发攻击事件
        /// </summary>
        void OnEvtAtk()
        {
//            groundStab.OnEvtAtk();
        }

    }
}