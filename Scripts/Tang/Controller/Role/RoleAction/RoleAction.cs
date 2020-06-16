using UnityEngine;

namespace Tang
{
    [SerializeField]
    public class RoleAction : IInputAction
    {
        public string id;
        public RoleActionType roleActionType = RoleActionType.None;
        public RoleAction(RoleActionType roleActionType)
        {
            this.roleActionType = roleActionType;
        }

        public override string ToString()
        {
            return Tools.Obj2Json<RoleAction>(this);
        }
    }
}