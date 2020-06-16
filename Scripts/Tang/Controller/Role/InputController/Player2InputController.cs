using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Basic.Math;
using UnityEngine;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

namespace Tang
{
    public class Player2InputController : InputController , IInputable
    {
        private IRoleAction roleAction;
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            roleAction = GetComponent<IRoleAction>();

            InputManager.Instance.Subscript("Gaming", new BindingInputAction("player2", (s, value) =>
            {
                OnInput(s,value);
            }));
            
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            InputManager.Instance.UnSubscript("Gaming","player2");
        }

        public void OnInput(string name,InputValue inputValue)
        {
            switch (name)
            {
                case "Alt":
                    if (inputValue.Get<float>() > 0.5)
                    {
                        roleAction.AltBegin();
                    }
                    else
                    {
                        roleAction.AltEnd();
                    }
                    break;
                case "Move":
                    Vector2 vector2 = inputValue.Get<Vector2>();
                    roleAction.MoveBy(vector2);
                    break;
                case "WalkCut":
                    if (inputValue.Get<float>() > 0.5)
                    {
                        roleAction.WalkCutBegin();
                    }
                    else
                    {
                        roleAction.WalkCutEnd();
                    }
                    break;
                case "Action1":
                    if (inputValue.Get<float>() > 0.5)
                    {
                        roleAction.Action1Begin();
                    }
                    else
                    {
                        roleAction.Action1End();
                    }
                    break;
                case "Action2" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.Action2Begin();
                    }
                    else
                    {
                        roleAction.Action2End();
                    }
                    break;
                case "Action3" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.Action3Begin();
                    }
                    else
                    {
                        roleAction.Action3End();
                    }
                    break;
                case "Action4" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.Action4Begin();
                    }
                    else
                    {
                        roleAction.Action4End();
                    }
                    break;
                
                case "Action5" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.Action5Begin();
                    }
                    else
                    {
                        roleAction.Action5End();
                    }
                    break;
                
                case "Action6" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.Action6Begin();
                    }
                    else
                    {
                        roleAction.Action6End();
                    }
                    break;
                
                case "Action7" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.Action7Begin();
                    }
                    else
                    {
                        roleAction.Action7End();
                    }
                    break;
                
                case "Action8" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.Action8Begin();
                    }
                    else
                    {
                        roleAction.Action8End();
                    }
                    break;
                
                case "Action9" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.Action9Begin();
                    }
                    else
                    {
                        roleAction.Action9End();
                    }
                    break;
                
                case "Action10" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.Action10Begin();
                    }
                    else
                    {
                        roleAction.Action10End();
                    }
                    break;
                case "Jump" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.IntoState1();
                    }
                    break;
                case "Rush" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.IntoRush();
                    }
                    else
                    {
                        roleAction.ComeOutRush();
                    }
                    break;
                case "Interact":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.IntoState2();
                    }
                    break;
                case "Roll":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.IntoState3();
                    }
                    break;
                case "Use":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.IntoState4();
                    }
                    break;
                
                case "KeyBoard1" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.KeyBoard1Begin();
                    }
                    else
                    {
                        roleAction.KeyBoard1End();
                    }
                    break;
                case "KeyBoard2" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.KeyBoard2Begin();
                    }
                    else
                    {
                        roleAction.KeyBoard2End();
                    }
                    break;
                case "KeyBoard3" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.KeyBoard3Begin();
                    }
                    else
                    {
                        roleAction.KeyBoard3End();
                    }
                    break;
                case "KeyBoard4" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.KeyBoard4End();
                    }
                    else
                    {
                        roleAction.KeyBoard4Begin();
                    }
                    break;
                case "KeyBoard5" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.KeyBoard5Begin();
                    }
                    else
                    {
                        roleAction.KeyBoard5End();
                    }
                    break;
            }
        }
    }
};