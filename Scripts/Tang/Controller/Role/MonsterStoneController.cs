namespace Tang
{
    public class MonsterStoneController : RoleController
    {
        // public void Start()
        // {
        //     base.Start();

        //     roleModel.Hp = 200;
        //     roleModel.CurrHp = 200;
        //     roleModel.Atk = 30;
        // }

        // public override void refreshRoleAnim()
        // {

        // }

        // public override void OnEvtAtk()
        // {
        //     var triggerObject = triggerManager.createTrigger();
        //     var damageController = triggerObject.AddComponent<DamageController>();
        //     damageController.owner = gameObject;

        //     // 不渲染 add by TangJian 2017/07/07 22:40:39
        //     {
        //         var meshRenderer = triggerObject.GetComponent<MeshRenderer>();
        //         meshRenderer.enabled = false;
        //     }

        //     // 设置大小 add by TangJian 2017/07/07 22:40:40
        //     {
        //         var boxCollider = triggerObject.GetComponent<BoxCollider>();
        //         triggerObject.transform.localScale = new Vector3(2, 2, 2);
        //     }

        //     // 设置位置 add by TangJian 2017/07/07 22:34:48
        //     {
        //         triggerObject.transform.parent = gameObject.transform.parent;

        //         var x = gameObject.transform.localPosition.x;
        //         var y = gameObject.transform.localPosition.y;
        //         var z = gameObject.transform.localPosition.z;

        //         triggerObject.transform.localPosition = new Vector3(x + 1 * getDirectionInt(), y + 0.5f, z);
        //     }

        //     // 设置攻击方向 add by TangJian 2017/07/10 20:47:44
        //     {
        //         var clipInfoArray = animator.GetCurrentAnimatorClipInfo(0);
        //         var clipInfo = clipInfoArray[0];
        //         var name = clipInfo.clip.name;

        //         var damageData = new DamageData();
        //         damageController.DamageData = damageData;
        //         switch (name)
        //         {
        //             case "attack1":
        //                 {
        //                     var x = gameObject.transform.localPosition.x;
        //                     var y = gameObject.transform.localPosition.y;
        //                     var z = gameObject.transform.localPosition.z;

        //                     triggerObject.transform.localScale = new Vector3(4f, 2.8f, 2.0f);
        //                     triggerObject.transform.localPosition = new Vector3(x + (2f + 1.2f) * getDirectionInt(), y + 1.4f, z);
        //                 }

        //                 damageData.direction = new Vector3(getDirectionInt(), 0, 0);
        //                 damageData.force = new Vector3(getDirectionInt() * 10, -20, 0);
        //                 damageData.hitType = 2;
        //                 break;
        //             case "attack2":
        //                 {
        //                     var x = gameObject.transform.localPosition.x;
        //                     var y = gameObject.transform.localPosition.y;
        //                     var z = gameObject.transform.localPosition.z;

        //                     triggerObject.transform.localScale = new Vector3(5.0f, 1.8f, 2.0f);
        //                     triggerObject.transform.localPosition = new Vector3(x + (2.5f + 1.1f) * getDirectionInt(), y + 0.9f, z);
        //                 }

        //                 damageData.direction = new Vector3(getDirectionInt(), 0, 0);
        //                 damageData.force = new Vector3(getDirectionInt() * 5, 20, 0);
        //                 damageData.hitType = 1;
        //                 break;
        //         }

        //         damageData.atk = 30;
        //     }

        //     Destroy(triggerObject, 0.1f);
        // }
    }
}