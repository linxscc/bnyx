





using UnityEngine;


namespace Tang
{
    public class FireBallSkillController : FlySkillController
    {
        public override void InitDamage()
        {
            MainDamageController = gameObject.GetChild("Trigger").GetChild("Damage").GetComponent<DamageController>();
            Debug.Assert(MainDamageController != null);

            var damageData = MainDamageController.damageData;
            damageData.itriggerDelegate = this;
            damageData.force = Vector3.zero;
        }
        public override Vector3 Speed
        {
            set
            {
                MainRigidbody.velocity = value * 3;
                //if (value.magnitude != 0)
                //    gameObject.transform.localEulerAngles = new Vector3(0, 0, Tools.GetDegree(MainRigidbody.velocity.x, MainRigidbody.velocity.y));
            }
            get
            {
                return MainRigidbody.velocity;
            }
        }
        //private Direction direction = Direction.Right;
        //public override Direction Direction
        //{
        //    set {direction = value; }get { return direction; }
        //}
        public override void OnTriggerIn(TriggerEvent evt)
        {
            base.OnTriggerIn(evt);
            if(evt.colider.gameObject.layer == LayerMask.NameToLayer("Ground") || evt.colider.gameObject.tag == "BackWall" || evt.colider.gameObject.layer == LayerMask.NameToLayer("Placement"))
            {
                MainDamageController.gameObject.SetActive(false);
                State = 2;
                Speed = Vector3.zero;
            }
        }
    }
}