using UnityEngine;
namespace Tang
{
    using FrameEvent;

    public class UnburiedHammerColliderController : MonoBehaviour
    {
        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
        RoleController roleController;
        Collider mainCollider;
        public string animname;
        void Start()
        {
            valueMonitorPool.Clear();
            roleController =gameObject.GetComponentInParent<RoleController>();
            roleController.OnAtkEvent += OnAtk;
            mainCollider = gameObject.GetComponent<Collider>();
            valueMonitorPool.AddMonitor<bool>(()=> 
            {
                return GetCurrAnimName(roleController.RoleAnimator)==animname;
            },(bool from,bool to)=> 
            {
                if (to)
                {
                    mainCollider.enabled = false;
                }
                else
                {
                    animname = "";
                    mainCollider.enabled = true;
                }
            });
        }
        
        void OnAtk(FrameEventInfo.RoleAtkFrameEventData roleAtkFrameEventData)
        {
            animname = GetCurrAnimName(roleController.RoleAnimator);
        }
        
        string GetCurrAnimName(Animator animator)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                var ci = clipInfo[0];
                var animName = ci.clip.name;
                return animName;
            }
            return "";
        }
        void Update()
        {
            valueMonitorPool.Update();
        }
    }
}

