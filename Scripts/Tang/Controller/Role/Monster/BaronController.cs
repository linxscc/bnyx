using System.Collections;
using UnityEngine;

namespace Tang
{
    public class BaronController : RoleController
    {
        
        public override bool Action1Begin()
        {
            _animator.SetTrigger("action3_begin");
            return true;
        }

        public override bool Action2Begin()
        {
            _animator.SetTrigger("action3_2_begin");
            return true;
        }

        public override bool Action3Begin()
        {
            _animator.SetTrigger("action3_3_begin");
            return true;
        }

        public override bool Action4Begin()
        {
            _animator.SetTrigger("action1_begin");
            return true;
        }

        public override bool IntoState1()
        {
            _animator.SetTrigger("action2_begin");
            return true;
        }

        public override bool IntoState2()
        {
            _animator.SetTrigger("isdodge");
            return true;
        }
        public override bool IntoState3()
        {
            _animator.SetTrigger("Isweak");
            _animator.SetInteger("weak_state",0);
            return true;
        }
        public override bool ComeOutState3()
        {
            _animator.SetInteger("weak_state",2);
            return true;
        }
        
        public override bool IntoState4()
        {
            _animator.SetTrigger("parry_attack");
            return true;
        }
        public override bool IntoState5()
        {
            _animator.SetTrigger("parry");
            return true;
        }
        
        public override bool IntoState6()
        { 
            _animator.SetInteger("defence_state",1);            
            return true;
        }

        public override bool ComeOutState6()
        { 
            _animator.SetInteger("defence_state",2);
            return true;
        }

        public override bool OnHurt(DamageData damageData)
        {
            return base.OnHurt(damageData);
        }
    }
}