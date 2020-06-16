namespace Tang
{
    public class BoneArcherController : RoleController
    {
        public override void SkillBegin(SkillData skillData)
        {
            base.SkillBegin(skillData);
            switch (skillData.id)
            {
                case "attack1":
                    {
                        _animator.SetBool("action1", true);
                    }
                    break;
            }
        }

        public override void SkillEnd(SkillData skillData)
        {
            base.SkillEnd(skillData);
            switch (skillData.id)
            {
                case "attack1":
                    {

                    }
                    break;
            }
        }

        // public override void OnEvtAtk()
        // {
        //     switch (currSkillData.id)
        //     {
        //         case "attack1":
        //             {
        //                 // 射箭
		// 				{
		// 					var arrow = GameObjectManager.Instance.createGameObject("Arrows");

		// 					arrow.transform.parent = transform.parent;
		// 					arrow.transform.localPosition = transform.localPosition + new Vector3(getDirectionInt() * 0.2f, 1.4f, 0);

		// 					ArrowsController arrowController = arrow.GetComponent<ArrowsController>();
		// 					arrowController.speed = new Vector3(getDirectionInt() * 20, 8, 2);

		// 					arrowController.DamageController.damageData.owner = gameObject;
		// 					arrowController.DamageController.damageData.itriggerDelegate = this;
		// 				}    

		// 				{
		// 					var arrow = GameObjectManager.Instance.createGameObject("Arrows");

		// 					arrow.transform.parent = transform.parent;
		// 					arrow.transform.localPosition = transform.localPosition + new Vector3(getDirectionInt() * 0.2f, 1.4f, 0);

		// 					ArrowsController arrowController = arrow.GetComponent<ArrowsController>();
		// 					arrowController.speed = new Vector3(getDirectionInt() * 20, 8, 0);

		// 					arrowController.DamageController.damageData.owner = gameObject;
		// 					arrowController.DamageController.damageData.itriggerDelegate = this;
		// 				}    

		// 				{
		// 					var arrow = GameObjectManager.Instance.createGameObject("Arrows");

		// 					arrow.transform.parent = transform.parent;
		// 					arrow.transform.localPosition = transform.localPosition + new Vector3(getDirectionInt() * 0.2f, 1.4f, 0);

		// 					ArrowsController arrowController = arrow.GetComponent<ArrowsController>();
		// 					arrowController.speed = new Vector3(getDirectionInt() * 20, 8, -2);

		// 					arrowController.DamageController.damageData.owner = gameObject;
		// 					arrowController.DamageController.damageData.itriggerDelegate = this;
		// 				}    
        //             }
        //             break;
        //     }
        // }
    }
}