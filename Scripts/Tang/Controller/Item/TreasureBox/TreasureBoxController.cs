namespace Tang
{
    public class TreasureBoxController : PlacementController
    {
        public bool IsOpened()
        {
            return State == 2;
        }

        public override bool OnHurt(DamageData damageData)
        {
            switch (State)
            {
                case 0:
                    State = 1;
                    MainRigidbody.AddForce(damageData.force * 50);
                    HitAndHurtDelegate.Hurt(damageData);
                    break;
                case 1:
                    break;
                case 2:
                    State = 3;
                    MainRigidbody.AddForce(damageData.force * 50);
                    HitAndHurtDelegate.Hurt(damageData);
                    break;
            }
            return true;
        }

        public override bool OnEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.Interact:
                    if(State == 0)
                    {
                        State = 2;
                        Drop();
                    }
                    break;
            }
            return true;
        }
    }
}