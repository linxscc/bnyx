using UnityEngine;


namespace Tang
{
    public static class RoleControllerExtend
    {
        public static void MoveTo(this RoleController target, Vector3 pos)
        {
            Vector3 offset = pos - target.gameObject.transform.position;
            target.Move(offset);
        }

        public static int GetCurrAnimTagHash(this RoleController target)
        {
            return target.RoleAnimator.GetCurrentAnimatorStateInfo(0).tagHash;
        }

        public static bool CurrAnimIsTag(this RoleController target, string animTag)
        {
            return target.RoleAnimator.GetCurrentAnimatorStateInfo(0).IsTag(animTag);
        }

        public static void OpenPortal(this RoleController target, PortalController portal)
        {
            if (portal.IsOpen) // 门已经开了, 就不处理 add by TangJian 2018/10/17 20:05
            {
                portal.Close();
            }
            else if (portal.OpenKey.IsValid()) // 使用钥匙开门 add by TangJian 2018/10/17 20:38
            {
                int itemIndex = target.RoleData.OtherItemList.FindIndex((ItemData item_) =>
                {
                    if (item_.id == portal.PortalData.door.key)
                    {
                        return true;
                    }
                    return false;
                });

                if (itemIndex >= 0)
                {
                    portal.Open();
                    target.RoleData.RemoveOtherItemWithIndex(itemIndex);
                }
                else
                {
                    Debug.Log("身上没有钥匙" + portal.OpenKey);
                }
            }
            else // 不需要钥匙, 直接开门 add by TangJian 2018/10/17 20:08
            {
                portal.Open();
            }
        }

        public static void OperateJoystick(this RoleController target, JoystickController joystickController)
        {
            joystickController.State = joystickController.State % joystickController.StateCount;
        }
    }
}