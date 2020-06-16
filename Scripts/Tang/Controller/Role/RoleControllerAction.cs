using System;
using UnityEngine;

namespace Tang
{
    public partial class RoleController
    {
        public virtual bool MoveBy(Vector2 vector2)
        {
            joystick.x = vector2.x;
            if (Math.Abs(joystick.x) > 0)
            {
                _animator.SetFloat("relative_speed_x", joystick.x * GetDirectionInt());
            }

                
            joystick.y = vector2.y;
            if (Math.Abs(joystick.y) > 0)
            {
                if (animatorParams.ContainsKey("relative_speed_y"))
                    _animator.SetFloat("relative_speed_y", joystick.y);
                if (canClimbLadder)
                {
                    ClimbLadder(joystick.y);
                }
            }
            return true;
        }
        
        public virtual bool Alt()
        {
            _animator.SetBool("Alt",true);
            _animator.SetInteger("Alt", 1);
            _animator.SetTrigger("Alt_begin");
            return true;
        }
        public virtual bool AltUp()
        {
            _animator.SetBool("Alt",true);
            _animator.SetInteger("Alt", 0);
            _animator.SetTrigger("Alt_end");
            return true;
        }

        private bool isAltPressed = false;

        public bool IsAltPressed => isAltPressed;

        public bool AltBegin()
        {
            _animator.SetInteger("Alt", 1);
            isRushing = false;
            isAltPressed = true;
            return true;
        }

        public bool AltEnd()
        {
            _animator.SetInteger("Alt", 0);
            isAltPressed = false;
            return true;
        }
        
        public bool WalkCutBegin()
        {
            _animator.SetBool("WalkCut", true);
            _animator.SetInteger("WalkCut_state", 1);
            _animator.SetTrigger("WalkCut_begin");
            return true;
        }
        
        public bool WalkCutEnd()
        {
            _animator.SetBool("WalkCut", true);
            _animator.SetInteger("WalkCut_state", 0);
            _animator.SetTrigger("WalkCut_end");
            return true;
        }

        public virtual bool KeyBoard1Begin()
        {
            _animator.SetBool("KeyBoard1", true);
            _animator.SetInteger("KeyBoard1_state", 1);
            _animator.SetTrigger("KeyBoard1_begin");
            return true;
        }
        public virtual bool KeyBoard1End()
        {
            _animator.SetBool("KeyBoard1", true);
            _animator.SetInteger("KeyBoard1_state", 0);
            _animator.SetTrigger("KeyBoard1_end");
            return true;
        }
        
        public virtual bool KeyBoard2Begin()
        {
            _animator.SetBool("KeyBoard2", true);
            _animator.SetInteger("KeyBoard2_state", 1);
            _animator.SetTrigger("KeyBoard2_begin");
            return true;
        }
        public virtual bool KeyBoard2End()
        {
            _animator.SetBool("KeyBoard2", true);
            _animator.SetInteger("KeyBoard2_state", 0);
            _animator.SetTrigger("KeyBoard2_end");
            return true;
        }
        
        public virtual bool KeyBoard3Begin()
        {
            _animator.SetBool("KeyBoard3", true);
            _animator.SetInteger("KeyBoard3_state", 1);
            _animator.SetTrigger("KeyBoard3_begin");
            return true;
        }
        public virtual bool KeyBoard3End()
        {
            _animator.SetBool("KeyBoard3", true);
            _animator.SetInteger("KeyBoard3_state", 0);
            _animator.SetTrigger("KeyBoard3_end");
            return true;
        }
        
        public virtual bool KeyBoard4Begin()
        {
            _animator.SetBool("KeyBoard3", true);
            _animator.SetInteger("KeyBoard3_state", 1);
            _animator.SetTrigger("KeyBoard3_begin");
            return true;
        }
        public virtual bool KeyBoard4End()
        {
            _animator.SetBool("KeyBoard4", true);
            _animator.SetInteger("KeyBoard4_state", 0);
            _animator.SetTrigger("KeyBoard4_end");
            return true;
        }
        
        public virtual bool KeyBoard5Begin()
        {
            _animator.SetBool("KeyBoard5", true);
            _animator.SetInteger("KeyBoard5_state", 1);
            _animator.SetTrigger("KeyBoard5_begin");
            return true;
        }
        public virtual bool KeyBoard5End()
        {
            _animator.SetBool("KeyBoard5", true);
            _animator.SetInteger("KeyBoard5_state", 0);
            _animator.SetTrigger("KeyBoard5_end");
            return true;
        }
        
        
        public virtual bool Action1Begin()
        {
            _animator.SetBool("action1", true);
            _animator.SetInteger("action1_state", 1);
            _animator.SetTrigger("action1_begin");
            return true;
        }

        public virtual bool Action1End()
        {
            _animator.SetBool("action1_end", true);
            _animator.SetInteger("action1_state", 0);
            _animator.SetTrigger("action1_end");
            return true;
        }

        public virtual bool Action2Begin()
        {
            _animator.SetBool("action2", true);
            _animator.SetInteger("action2_state", 1);
            _animator.SetTrigger("action2_begin");
            return true;
        }

        public virtual bool Action2End()
        {
            _animator.SetBool("action2_end", true);
            _animator.SetInteger("action2_state", 0);
            _animator.SetTrigger("action2_end");
            return true;
        }

        public virtual bool Action3Begin()//L
        {
            _animator.SetBool("action3", true);
            _animator.SetInteger("action3_state", 1);
            _animator.SetTrigger("action3_begin");
            return true;
        }

        public virtual bool Action3End()
        {
            _animator.SetBool("action3", true);
            _animator.SetInteger("action3_state",0);
            _animator.SetTrigger("action3_end");
            return true;
        }

        public virtual bool Action4Begin()//I
        {
            _animator.SetBool("action4", true);
            _animator.SetInteger("action4_state", 1);
            _animator.SetTrigger("action4_begin");
            return true;
        }

        public virtual bool Action4End()
        {
            _animator.SetBool("action4", true);
            _animator.SetInteger("action4_state",0);
            _animator.SetTrigger("action4_end");
            return true;
        }
        
        public virtual bool Action5Begin()//G
        {
            _animator.SetBool("action5", true);
            _animator.SetInteger("action5_state", 1);
            _animator.SetTrigger("action5_begin");
            return true;
        }

        public virtual bool Action5End()
        {
            _animator.SetBool("action5", true);
            _animator.SetInteger("action5_state", 0);
            _animator.SetTrigger("action5_end");
            return true;
        }

        public virtual bool Action6Begin()//T
        {
            _animator.SetBool("action6", true);
            _animator.SetInteger("action6_state", 1);
            _animator.SetTrigger("action6_begin");
            return true;
        }

        public virtual bool Action6End()
        {
            _animator.SetBool("action6", true);
            _animator.SetInteger("action6_state", 0);
            _animator.SetTrigger("action6_end");
            return true;
        }

        public virtual bool Action7Begin()//Y
        {
            _animator.SetBool("action7", true);
            _animator.SetInteger("action7_state", 1);
            _animator.SetTrigger("action7_begin");
            return true;
        }

        public virtual bool Action7End()
        {
            _animator.SetBool("action7", true);
            _animator.SetInteger("action7_state", 0);
            _animator.SetTrigger("action7_end");
            return true;
        }

        public virtual bool Action8Begin()//U
        {
            _animator.SetBool("action8", true);
            _animator.SetInteger("action8_state", 1);
            _animator.SetTrigger("action8_begin");
            return true;
        }

        public virtual bool Action8End()
        {
            _animator.SetBool("action8", true);
            _animator.SetInteger("action8_state", 0);
            _animator.SetTrigger("action8_end");
            return true;
        }

        public virtual bool Action9Begin()//O
        {
            _animator.SetBool("action9", true);
            _animator.SetInteger("action9_state", 1);
            _animator.SetTrigger("action9_begin");
            return true;
        }

        public virtual bool Action9End()
        {
            _animator.SetBool("action9", true);
            _animator.SetInteger("action9_state", 0);
            _animator.SetTrigger("action9_end");
            return true;
        }

        public virtual bool Action10Begin()//P
        {
            _animator.SetBool("action10", true);
            _animator.SetInteger("action10_state", 1);
            _animator.SetTrigger("action10_begin");
            return true;
        }

        public virtual bool Action10End()
        {
            _animator.SetBool("action10", true);
            _animator.SetInteger("action10_state", 0);
            _animator.SetTrigger("action10_end");
            return true;
        }

        public virtual bool IntoState1()
        {
            _animator.SetInteger("action_jump", 1);
            return true;
        }

        public virtual bool ComeOutState1()
        {
            throw new NotImplementedException();
        }

        public virtual bool IntoState2()
        {
            return true;
        }
        public virtual bool ComeOutState2()
        {
            return true;
        }
        public virtual bool IntoState3()
        {
            if (RoleData.FinalVigor > 20)
            {
                _animator.SetInteger("action_roll", 1);
            }
            return true;
        }

        public virtual bool ComeOutState3()
        {
            throw new NotImplementedException();
        }

        public virtual bool IntoState4()
        {
            throw new NotImplementedException();
        }

        public virtual bool ComeOutState4()
        {
            throw new NotImplementedException();
        }

        public virtual bool IntoState5()
        {
            throw new NotImplementedException();
        }

        public virtual bool ComeOutState5()
        {
            throw new NotImplementedException();
        }

        public virtual bool IntoState6()
        {
            throw new NotImplementedException();
        }

        public virtual bool ComeOutState6()
        {
            throw new NotImplementedException();
        }

        public bool IntoRush()
        {
            isRushing = true;
            return true;
        }

        public bool ComeOutRush()
        {
            isRushing = false;
            return true;
        }
    }
}