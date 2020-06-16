using System.Collections.Generic;


namespace Tang
{
    public partial class RoleController
    {
        private void InitBuff()
        {
            // 增益控制器 add by TangJian 2017/10/09 20:12:56
            BuffController.SetRoleBuffData(RoleData.RoleBuffData);
            BuffController.RegisterBuffEvent(BuffEventType.OnBuffBegin, (BuffData buff, object[] objs) =>
            {
                OnBuffBegin(buff);
                return true;
            });
            BuffController.RegisterBuffEvent(BuffEventType.OnBuffUpdate, (BuffData buff, object[] objs) =>
            {
                OnBuffUpdate(buff);
                return true;
            });
            BuffController.RegisterBuffEvent(BuffEventType.OnBuffEnd, (BuffData buff, object[] objs) =>
            {
                OnBuffEnd(buff);
                return true;
            });
            BuffController.RegisterBuffEvent(BuffEventType.OnHurt, (BuffData buff, object[] objs) =>
            {
                if (buff.buffEvents.ContainsKey(BuffEventType.OnHurt))
                    ActionDataExecutor.ExecuteRoleActions(this, buff.buffEvents[BuffEventType.OnHurt]);
                return true;
            });

            BuffController.RegisterBuffEvent(BuffEventType.OnHit, (BuffData buff, object[] objs) =>
            {
                if (buff.buffEvents.ContainsKey(BuffEventType.OnHit))
                    ActionDataExecutor.ExecuteRoleActions(this, buff.buffEvents[BuffEventType.OnHit]);
                return true;
            });
        }

        void OnBuffBegin(BuffData buff)
        {
            List<ActionData> list = new List<ActionData>();
            if (buff.buffEvents.TryGetValue(BuffEventType.OnBuffBegin, out list))
            {
                ActionDataExecutor.ExecuteRoleActions(this, list);
            }

        }

        void OnBuffUpdate(BuffData buff)
        {
            for (int i = 0; i < buff.level; i++)
            {
                List<ActionData> list = new List<ActionData>();
                if (buff.buffEvents.TryGetValue(BuffEventType.OnBuffUpdate, out list))
                {
                    ActionDataExecutor.ExecuteRoleActions(this, list);
                }
            }
        }

        void OnBuffEnd(BuffData buff)
        {
            for (int i = 0; i < buff.level; i++)
            {
                List<ActionData> list = new List<ActionData>();
                if (buff.buffEvents.TryGetValue(BuffEventType.OnBuffEnd, out list))
                {
                    ActionDataExecutor.ExecuteRoleActions(this, list);
                }
                // buff.buffEvents.TryGetValue(BuffEventType.OnBuffEnd,out List<ActionData> liasd);               
            }
        }
    }
}