using System;
using System.Collections;
using System.Collections.Generic;
using Tang.Animation;
using UnityEngine;

namespace Tang
{
    public class IdleForFewSecondsNewBehaviour : NewBehaviour
    {
        private NewBehaviour bh_Idle = new NewBehaviour("idle", () => { });

        private Action actionEnd;
        
        private float time = 5;
        private float timePassed = 0;
        public IdleForFewSecondsNewBehaviour(string name, Action action,Action actionEnd,float time) : base(name, action)
        {
            this.time = time;
            this.actionEnd = actionEnd;
        }

        public override void Start()
        {
            timePassed = time;
            action?.Invoke();
        }

//        public override void OnUpdate( float timeE , MESSAGEQUEUE)
//        {
//            timePassed -= timeE;
//            if (timePassed <= 0)
//            {
//                actionEnd?.Invoke();
//            }
//            //
//            MESSAGEQUEUE.add("xxx")
//        }
        
    }
    
    public class PrisonerAIController : MonoBehaviour 
    {
        public class RootBehaviour : NewBehaviour
        {
            //idle 没事就休息1~5秒
            //moveToPlayer 向玩家走过去 1~10秒
            //attackPlayerInRange 当玩家在范围内时攻击他
            
            public RootBehaviour(string name, Action action) : base(name, action)
            {
            }

//            public override void Start()
//            {
//                IdleForFewSecondsNewBehaviour idle4 = new IdleForFewSecondsNewBehaviour();
//                
//                idle4.Start();
//                
//                base.Start();
//            }

//            public override void OnEvent(string stateName, NB_EVENTTYPE eventType, object info, float time)
//            {
//                if (stateName == "idle" && eventType == NB_EVENTTYPE.STATE_START )
//                {
//                    
//                    
//                }
//                
//                base.OnEvent(stateName, eventType, info, time);
//                
//            }

            public override void OnUpdate(float timeE)
            {
                //bh_action1.OnUpdate(timeE);
                base.OnUpdate(timeE);
            }
        }
        
        // prisoner可以做的所有操作列表
        // Behaviour 移动  Action
        // Behaviour 攻击  A
        // Behaviour 跳
        // Behaviour 绕后

        // 控制器思考时机
        // onUpdate()
        //{
        //}

        // 控制器控制时机
        // onEvent()
        // {
        // }

        private RootBehaviour rootBehaviour;
        
        private NewBehaviour bh_Idle = new NewBehaviour("idle", () => { });
        private NewBehaviour bh_action1;

        private RoleController roleController;
        
        private void Start()
        {
            
            roleController = GetComponent<RoleController>();
            
            bh_action1 = new NewBehaviour(
                "action1",
                () => { roleController.Action1Begin(); });
            
            GetComponent<AnimatorStateTransmit>().OnStateEvents += UpdateMovement;
            
            //设置初始状态
            //TODO
            
            
            rootBehaviour.Start();
        }
        
        private void UpdateMovement(string stateName, AnimatorStateEventType eventType, Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex, float time)
        {
            

            System.Object info = (System.Object)stateInfo;
           

            NB_EVENTTYPE type;
            
            if (eventType == AnimatorStateEventType.OnStateEnter)
            {
                //if (stateInfo.IsTag("idle"))
                //{
                //    bh_action1.Start();
                //}
                type = NB_EVENTTYPE.STATE_START;
            }
            else if (eventType == AnimatorStateEventType.OnStateUpdate)
            {
                type = NB_EVENTTYPE.STATE_UPDATE;
            }
            else if (eventType == AnimatorStateEventType.OnStateExit)
            {
                type = NB_EVENTTYPE.STATE_END;
            }
            else
            {
                return;
            }
            
            rootBehaviour.OnEvent(stateName,type,info,time);
            
        }


        
       
        
        
//        private void Update() 
//        {
//            //思考
//            //bh_action1.OnUpdate();
//            
//            rootBehaviour.OnUpdate(1/60 , MESSAGEQUEUE);
//
//            if (MESSAGEQUEUE)
//            {
//                for (MESSAGEQUEUE)
//                {
//                    rootBehaviour.OnEvent();
//                    MESSAGEQUEUE--;
//                }
//            }
//            //检测状态机
//            
//        }


       

        public enum State
        {
            idle = 0,
            Move = 1,
            Action1 = 3
        }
    }
    
}