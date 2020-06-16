


namespace Tang
{
    public class CustomKeyBinding : IKeyBinding
    {
        private KeyAction keyAction;

        public CustomKeyBinding(KeyAction keyAction)
        {
            this.keyAction = keyAction;
        }

        public KeyAction Key
        {
            get
            {
                return keyAction;
            }
            set
            {
                keyAction = value;
            }
        }
    }
}

