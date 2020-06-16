
using Tang;

namespace ZS
{
    public class CaptionController:RoleController
    {
        public override bool Action1Begin()
        {
            _animator.SetTrigger("action1_begin");
            return true;
        }

        public override bool Action2Begin()
        {
            _animator.SetTrigger("action2_begin");
            return true;
            
        }

        public override bool Action3Begin()
        {
            _animator.SetTrigger("action3_begin");
            return true;
        }

        public override bool Action4Begin()
        {
            _animator.SetTrigger("action4_begin");
            return true;
        }
        
        public override bool IntoState1()
        { 
            _animator.SetFloat("p_state",0);
            return true;
        }
        public override bool IntoState3()
        { 
            _animator.SetInteger("action1_begin_again",1);
            return true;
        }
        public override bool ComeOutState3()
        {
            _animator.SetInteger("action1_begin_again",0);
            return true;
        }

        public override bool IntoState6()
        { 
            _animator.SetFloat("p_state",1);   
            _animator.SetTrigger("p1_to_p2");
            return true;
        }
        public override bool ComeOutState6()
        {
            return true;
        }
    }
}