using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class RoleInputData
    {
        public Vector2 joystick; // 摇杆 add by TangJian 2017/07/04 21:18:05
        public bool action1_begin;
        public bool action2_begin;
        public bool jump_begin;
        public bool interact_begin;
        public bool skill1;
        public bool roll_begin;

        public List<RoleAction> actionList = new List<RoleAction>();
        public List<IKeyBinding> keyBindingList;

        public Vector2 Joystick { get { return joystick; } set { joystick = value; } }
        public bool Action1 { get { return action1_begin; } set { action1_begin = value; } }
        public bool Action2 { get { return action2_begin; } set { action2_begin = value; } }
        public bool Jump { get { return jump_begin; } set { jump_begin = value; } }
        public bool Interact { get { return interact_begin; } set { interact_begin = value; } }
        public bool Skill1 { get { return skill1; } set { skill1 = value; } }
    }
}
