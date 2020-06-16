namespace Tang
{
    public class JavelinSoldierController : RoleController
    {
        // public override void OnEvtAtk()
        // {
        //     // 攻击帧事件 add by TangJian 2017/07/13 23:23:07            
        //     var clipInfoArray = animator.GetCurrentAnimatorClipInfo(0);
        //     var clipInfo = clipInfoArray[0];
        //     var animName = clipInfo.clip.name;

        //     if (animName == "attack2")
        //     {
        //         GameObject javelinObject = GameObjectManager.Instance.Create("Javelin");
        //         JavelinController javelinController = javelinObject.GetComponent<JavelinController>();

        //         javelinController.Owner = gameObject;
        //         javelinController.Atk = roleData.Atk;
        //         javelinController.TeamId = roleData.TeamId;                

        //         javelinController.transform.position = transform.position + new Vector3(0, 1, 0);
        //         javelinController.Speed = new Vector3(35 * getDirectionInt(), 0, 0);
        //     }
        //     else
        //     {
        //         base.OnEvtAtk();
        //     }
        // }
    }
}