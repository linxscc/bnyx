





using UnityEngine;


namespace Tang
{
    public class NewFireBallSkillController : FlySkillController
    {
        public override void InitDamage()
        {
            //MainDamageController = gameObject.GetChild("Trigger").GetChild("Damage").GetComponent<DamageController>();
            //Debug.Assert(MainDamageController != null);

            //var damageData = MainDamageController.damageData;
            //damageData.itriggerDelegate = this;
            //damageData.force = Vector3.zero;
            
        }
        public void Start()
        {
            //MainDamageController.damageData.poiseCut = owner.GetComponent<RoleController>().RoleData.FinalPoiseCut;
        }
        public override Vector3 Speed
        {
            set
            {
                MainRigidbody.velocity = value * 3;
            }
            get
            {
                return MainRigidbody.velocity;
            }
        }
        public override Direction Direction
        {
            set{ direction = value; }
            get { return direction; }
        }
        public override void OnTriggerIn(TriggerEvent evt)
        {
            base.OnTriggerIn(evt);
            if( evt.colider.gameObject.layer == LayerMask.NameToLayer("Ground") || evt.colider.gameObject.tag == "BackWall" || evt.colider.gameObject.layer == LayerMask.NameToLayer("Placement"))
            {
                //MainDamageController.gameObject.SetActive(false);
                State = 2;
                Speed = Vector3.zero;
                MainRigidbody.isKinematic = true;
                MainRigidbody.freezeRotation = true;
                MainRigidbody.useGravity = false;
                //AnimManager.Instance.PlayAnimEffect("fireball_boom", transform.position);
                Vector3 posf = transform.position - Owner.transform.position;
                SkillManager.Instance.UseSkill("fireball_boom", Owner.transform, TeamId, Direction, new Vector3(Mathf.Abs(posf.x),posf.y,posf.z));
                Destroy(gameObject);

            }
            else
            {
                if (evt.otherTriggerController.ITriggerDelegate != null)
                {
                    if (evt.otherTriggerController.ITriggerDelegate.gameObject != null)
                    {
                        if(evt.otherTriggerController.ITriggerDelegate.gameObject.layer == LayerMask.NameToLayer("Role")|| evt.otherTriggerController.ITriggerDelegate.gameObject.layer == LayerMask.NameToLayer("SceneComponent"))
                        {
                            State = 2;
                            Speed = Vector3.zero;
                            MainRigidbody.isKinematic = true;
                            MainRigidbody.freezeRotation = true;
                            MainRigidbody.useGravity = false;
                            //AnimManager.Instance.PlayAnimEffect("fireball_boom", transform.position);
                            Vector3 posf = transform.position - Owner.transform.position;
                            SkillManager.Instance.UseSkill("fireball_boom", Owner.transform, TeamId, Direction, new Vector3(Mathf.Abs(posf.x), posf.y, posf.z));
                            Destroy(gameObject);
                        }
                    }
                
                }
            }
            
            
        }
        public override void OnHit()
        {
            //base.OnHit();
            MainRigidbody.isKinematic = true;
            MainRigidbody.freezeRotation = true;
            MainRigidbody.useGravity = false;
            Vector3 posf = transform.position - Owner.transform.position;
            SkillManager.Instance.UseSkill("fireball_boom", Owner.transform, TeamId, Direction, new Vector3(Mathf.Abs(posf.x), posf.y, posf.z));
            Destroy(gameObject);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (MainRigidbody.freezeRotation == false)
            {
                if (MainRigidbody.isKinematic == false)
                {
                    if (transform.localScale.x >= 0)
                    {
                        Quaternion rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), Speed.normalized);
                        MainRigidbody.rotation = Quaternion.Euler(new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z));
                    }
                    else
                    {
                        Quaternion rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), Speed.normalized);
                        MainRigidbody.rotation = Quaternion.Euler(new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z));
                    }
                }
            }
        }
    }
}