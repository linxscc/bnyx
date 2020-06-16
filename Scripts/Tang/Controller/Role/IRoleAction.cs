
using UnityEngine;

namespace Tang
{
    public interface IRoleAction
    {
        bool Alt();
        bool AltUp();

        bool IsAltPressed { get; }

        bool AltBegin();
        bool AltEnd();

        bool WalkCutBegin();
        bool WalkCutEnd();
        
        

        bool MoveBy(Vector2 vector2);
        bool IntoRush();
        bool ComeOutRush();
        bool Action1Begin();
        bool Action1End();
        bool Action2Begin();
        bool Action2End();
        bool Action3Begin();
        bool Action3End();
        bool Action4Begin();
        bool Action4End();
        bool Action5Begin();
        bool Action5End();
        bool Action6Begin();
        bool Action6End();
        bool Action7Begin();
        bool Action7End();
        bool Action8Begin();
        bool Action8End();
        bool Action9Begin();
        bool Action9End();
        bool Action10Begin();
        bool Action10End();
        bool IntoState1();
        bool ComeOutState1();
        bool IntoState2();
        bool ComeOutState2();
        bool IntoState3();
        bool ComeOutState3();
        bool IntoState4();
        bool ComeOutState4();
        bool IntoState5();
        bool ComeOutState5();
        bool IntoState6();
        bool ComeOutState6();
        
        bool KeyBoard1Begin();
        bool KeyBoard1End();
        bool KeyBoard2Begin();
        bool KeyBoard2End();
        bool KeyBoard3Begin();
        bool KeyBoard3End();
        bool KeyBoard4Begin();
        bool KeyBoard4End();
        bool KeyBoard5Begin();
        bool KeyBoard5End();
        
        
    }
}