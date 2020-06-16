using UnityEngine;

namespace Tang
{
    public class RoleInputModel
    {
        private Vector2 joystick; // 摇杆 add by TangJian 2017/07/04 21:18:05
        private bool attack;
        private bool jump;
        public bool interact;
        public Vector2 Joystick { get { return joystick; } set { joystick = value; } }
        public bool Attack { get { return attack; } set { attack = value; } }
        public bool Jump { get { return jump; } set { jump = value; } }
        public bool Interact { get { return interact; } set { interact = value; } }
    }
}
