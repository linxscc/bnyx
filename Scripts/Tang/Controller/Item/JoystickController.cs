namespace Tang
{
    public class JoystickController : PlacementController, IInteractable
    {
        public override void Start()
        {
            base.Start();
        }

        public int StateCount = 2;
        
        public override int State
        {
            set
            {
                if (MainAnimator != null)
                {
//                    Debug.Log("设置 State = " + value);
                    MainAnimator.SetInteger("State", value);
                }
            }
            
            get
            {
                return MainAnimator.GetInteger("State");
            }
        }

        public bool CanInteract()
        {
            return true;
        }

        public void Interact()
        {
            State = (State + 1) % StateCount;
        }
    }
}