using System.Collections.Generic;
using Tang.Animation;
using UnityEngine;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;
using Debug = System.Diagnostics.Debug;

namespace Tang
{
    public class Player1InputController : InputController, IInputable
    {
        private IRoleAction roleAction;

        void OnEnable()
        {
            roleAction = GetComponent<IRoleAction>();
            
            InputManager.Instance.Subscript("Gaming", new BindingInputAction("player1", (s, value) =>
            {
                if (DebugManager.Instance.debugData.useKeyboard)
                {
                    OnInput(s, value);
                }
                else
                {
                    OnXboxInput(s,value);
                }
            }));
            
            InputManager.Instance.SwitchActions("Gaming");
            
            
            GetComponentInChildren<AnimatorStateTransmit>().OnStateEvents += UpdateMovement;
        }

        public virtual void UpdateMovement(string stateName, AnimatorStateEventType eventType, Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex, float time)
        {
            
        }

        void OnDisable()
        {
            InputManager.Instance.UnSubscript("Gaming", "player1");
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
            }
        }

        public void OnXboxInput(string name, InputValue inputValue)
        {
            switch (name)
            {
                case "XBoxMove":
                    Vector2 vector2 = inputValue.Get<Vector2>();
                    roleAction.MoveBy(vector2);
                    
                    break;
                case "XBoxAction1":
                    float f = inputValue.Get<float>();
                    if (f > 0.5)
                    {
                        roleAction.Action1Begin();
                    }
                    else
                    {
                        roleAction.Action1End();
                    }
                    break;
                case "XBoxAction2" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.Action2Begin();
                    }
                    else
                    {
                        roleAction.Action2End();
                    }
                    break;
                case "XBoxAction3" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.IntoState1(); 
                    }
                    break;
                case "XBoxAction4":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.IntoState3();
                    }
                    break;
                case "XBoxAction5":
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.IntoState2(); 
                    }
                    break;
                case "XBoxRush" :
                    if (inputValue.Get<float>()>0.5f)
                    {
                        roleAction.IntoRush();
                    }
                    else
                    {
                        roleAction.ComeOutRush();
                    }
                    break;
            } 
        }
    }
}