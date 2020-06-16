using System;

namespace Tang
{

    public enum NB_EVENTTYPE
    {
        STATE_START = 1,
        STATE_END,
        STATE_UPDATE
    };
    
    public class NB_RESULT
    {
        public int ret;
        public string info;
    };

    
    public class NewBehaviour
    {
        public Action action;

        public NewBehaviour(string name , Action action)
        {
            this.action = action;
        }

        
        public virtual void Start()
        {
            action?.Invoke();
        }
        
        public virtual void OnBegin()
        {
            
        }

        public virtual void OnUpdate( float timeE)
        {
            
            //throw new NotImplementedException();
        }

        public virtual void OnEnd()
        {
            //throw new NotImplementedException();
        }

        public virtual NB_RESULT OnEvent(string stateName,NB_EVENTTYPE eventType,Object info,float time)
        {
            NB_RESULT ret = new NB_RESULT();
            ret.ret = 1;
            ret.info = "asd";
            return ret;
            //throw new NotImplementedException();
        }
    }


    public class IdleNewBehaviour:NewBehaviour
    {
        public IdleNewBehaviour(string name, Action action) : base(name, action)
        {
            this.action = action;
        }

        public override void Start()
        {
            
        }
        
        public override void OnBegin()
        {
            
        }

        public override void OnUpdate( float timeE)
        {
            
        }

        public override void OnEnd()
        {
            
        }

    }
    
    
    public class MoveNewBehaviour:NewBehaviour
    {
        public MoveNewBehaviour(string name, Action action) : base(name, action)
        {
            this.action = action;
        }

        public override void Start()
        {
            
        }
        
        public override void OnBegin()
        {
            
        }

        public override void OnUpdate( float timeE)
        {
            
        }

        public override void OnEnd()
        {
            
        }

    }
    public class ActionNewBehaviour:NewBehaviour
    {
        public ActionNewBehaviour(string name, Action action) : base(name, action)
        {
            this.action = action;
        }

        public override void Start()
        {
            
        }
        
        public override void OnBegin()
        {
            
        }

        public override void OnUpdate( float timeE)
        {
            
        }

//        public override void OnEvent(string stateName, NB_EVENTTYPE eventType, object info, float time)
//        {
//            base.OnEvent(stateName, eventType, info, time);
//        }
    }
    
    
}