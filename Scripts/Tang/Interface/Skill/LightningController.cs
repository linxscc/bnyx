using UnityEngine;

namespace Tang
{
    // 静止的技能控制器
    public class LightningController : StaticSkillController
    {
        public override void TowardTo(Vector3 pos)
        {
            var offset = pos - transform.position;
            offset.y = 0;

            transform.rotation = Quaternion.FromToRotation(Vector3.right, offset);
        }


        public override bool OnHit(DamageData damageData)
        {
            if (base.OnHit(damageData))
            {
                HitAndHurtDelegate.Hit(damageData);
                DelayFunc("SkillDestroy", () =>
                {
                    MainDamageController.transform.gameObject.SetActive(false);
                }, 0.01f);
                return true;
            }
            return false;
        }
    }
}