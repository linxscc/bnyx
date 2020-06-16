namespace Tang
{
    public class SwordManController : RoleController
    {
        //     public void Start()
        //     {
        //         base.Start();
        //         roleModel.Atk = 10;
        //     }

        //     public override void OnEvtAtk()
        //     {
        //         var triggerObject = triggerManager.createTrigger();
        //         var damageController = triggerObject.AddComponent<DamageController>();
        //         damageController.owner = gameObject;

        //         // 不渲染 add by TangJian 2017/07/07 22:40:39
        //         {
        //             var meshRenderer = triggerObject.GetComponent<MeshRenderer>();
        //             meshRenderer.enabled = false;
        //         }

        //         // 设置大小 add by TangJian 2017/07/07 22:40:40
        //         {
        //             var boxCollider = triggerObject.GetComponent<BoxCollider>();
        //             triggerObject.transform.localScale = new Vector3(3 * triggerObject.transform.localScale.x, 2 * triggerObject.transform.localScale.y, 2 * triggerObject.transform.localScale.z);
        //         }

        //         // 设置位置 add by TangJian 2017/07/07 22:34:48
        //         {
        //             triggerObject.transform.parent = gameObject.transform.parent;

        //             var x = gameObject.transform.localPosition.x;
        //             var y = gameObject.transform.localPosition.y;
        //             var z = gameObject.transform.localPosition.z;

        //             triggerObject.transform.localPosition = new Vector3(x + 1 * getDirectionInt(), y + 0.5f, z);
        //         }

        //         // 设置攻击方向 add by TangJian 2017/07/10 20:47:44
        //         {
        //             var clipInfoArray = animator.GetCurrentAnimatorClipInfo(0);
        //             var clipInfo = clipInfoArray[0];
        //             var name = clipInfo.clip.name;

        //             var damageData = new DamageData();
        //             damageController.DamageData = damageData;
        //             switch (name)
        //             {
        //                 case "lswd-attack1":
        //                     damageData.direction = new Vector3(getDirectionInt(), 0, 0);
        //                     damageData.force = new Vector3(getDirectionInt() * 1, 2, 0);
        //                     damageData.hitType = 1;
        //                     break;
        //                 case "lswd-attack2":
        //                     damageData.direction = new Vector3(getDirectionInt(), 0, 0);
        //                     damageData.force = new Vector3(getDirectionInt() * 3, 2, 0);
        //                     damageData.hitType = 5;
        //                     break;
        //                 case "lswd-attack3":
        //                     damageData.direction = new Vector3(getDirectionInt(), 0, 0);
        //                     damageData.force = new Vector3(getDirectionInt() * 3, 0, 0);
        //                     damageData.hitType = 1;
        //                     break;
        //                 case "lswd-attack4":
        //                     damageData.direction = new Vector3(getDirectionInt(), 0, 0);
        //                     damageData.force = new Vector3(getDirectionInt() * 10, 2, 0);
        //                     damageData.hitType = 3;
        //                     break;
        //                 default:
        //                     damageData.direction = new Vector3(getDirectionInt(), 0, 0);
        //                     damageData.force = new Vector3(getDirectionInt() * 1, 5, 0);
        //                     damageData.hitType = 1;
        //                     break;
        //             }

        //             // 攻击力赋值 add by TangJian 2017/07/14 17:53:07
        //             {
        //                 damageData.atk = roleModel.Atk;
        //             }
        //         }

        //         Destroy(triggerObject, 0.1f);
        //     }
    }
}