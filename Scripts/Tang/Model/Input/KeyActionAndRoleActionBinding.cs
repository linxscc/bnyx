using UnityEngine;

namespace Tang
{
    [SerializeField]
    public class KeyActionAndRoleActionBinding : IKeyBinding
    {
        private KeyAction keyAction;
        private RoleAction roleAction;

        public KeyActionAndRoleActionBinding(KeyAction keyAction, RoleAction roleAction)
        {
            this.keyAction = keyAction;
            this.roleAction = roleAction;
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
        public RoleAction Action
        {
            get
            {
                return roleAction;
            }

            set
            {
                roleAction = value;
            }
        }
    }
}