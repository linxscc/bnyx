using UnityEngine;
namespace Tang
{
    public class UnburiedHammerController : TriggerController, ITriggerDelegate
    {
        public GameObject GetGameObject()
        {
            return this.gameObject;
        }
        public override ITriggerDelegate ITriggerDelegate
        {
            get
            {
                return GetComponent<ITriggerDelegate>();
            }
        }

        public bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.DamageHurt:
                    return OnHurt(evt.Data as DamageData);
                    // break;

            }
            return true;
            //throw new System.NotImplementedException();
        }
        bool OnHurt(DamageData damageData)
        {
            if (damageData.teamId != GetComponentInParent<RoleController>().RoleData.TeamId)
            {
                float angle = damageData.SpecialEffectRotation;
                Vector3 moveOrientation = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
                DebugManager.Instance.AddDrawGizmos("rolehurthammer" + gameObject.GetInstanceID(), () =>
                {
                    Gizmos.DrawSphere(damageData.collidePoint, 0.5f);
                });
                AnimManager.Instance.PlayAnimEffect("RoleHurtEffect", new Vector3(damageData.collidePoint.x, damageData.collidePoint.y, transform.position.z - 0.1f), 0, damageData.direction.x < 0, moveOrientation, transform);
                return true;
            }
            else
            {
                return false;
            }

        }
        public void OnTriggerIn(TriggerEvent evt)
        {
            //throw new System.NotImplementedException();
        }

        public void OnTriggerKeep(TriggerEvent evt)
        {
            //throw new System.NotImplementedException();
        }

        public void OnTriggerOut(TriggerEvent evt)
        {
            //throw new System.NotImplementedException();
        }




    }
}

