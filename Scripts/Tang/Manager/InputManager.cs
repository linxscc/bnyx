using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Dynamic;
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

#if UNITY_EDITOR

#endif

namespace Tang
{
    public class InputBinding
    {
        public InputBinding(List<IKeyBinding> keyBindingList, Action<List<IKeyBinding>> onInput)
        {
            this.keyBindingList = keyBindingList;
            this.onInput = onInput;
        }
        public List<IKeyBinding> keyBindingList = new List<IKeyBinding>();
        public Action<List<IKeyBinding>> onInput;

        public List<KeyAction> currKeyActionList = new List<KeyAction>();
        public List<IKeyBinding> currActiveKeyBindingList = new List<IKeyBinding>();

        public void Update()
        {
            currActiveKeyBindingList.Clear();

            foreach (var keyBinding in keyBindingList)
            {
                KeyAction keyAction = keyBinding.Key;
                KeyCode keyCode = keyAction.keyCode;
                KeyActionType keyActionType = keyAction.actionType;

                switch (keyActionType)
                {
                    case KeyActionType.Press:
                        if (Input.GetKeyDown(keyCode))
                        {
                            currActiveKeyBindingList.Add(keyBinding);
                        }
                        break;
                    case KeyActionType.Release:
                        if (Input.GetKeyUp(keyCode))
                        {
                            currActiveKeyBindingList.Add(keyBinding);
                        }
                        break;
                    case KeyActionType.ButtonPress:
                        if (Input.GetButtonDown(keyAction.buttonName))
                        {
                            currActiveKeyBindingList.Add(keyBinding);
                        }
                        break;
                    case KeyActionType.ButtonRelease:
                        if (Input.GetButtonUp(keyAction.buttonName))
                        {
                            currActiveKeyBindingList.Add(keyBinding);
                        }
                        break;
                    case KeyActionType.AxisButtonPress:
                        keyAction.Update();
                        if (keyAction.axisButtonPress)
                        {
                            keyAction.axisButtonPress = false;
                            currActiveKeyBindingList.Add(keyBinding);
                        }
                        break;
                    case KeyActionType.AxisButtonRelease:
                        keyAction.Update();
                        if (keyAction.axisButtonRelease)
                        {
                            keyAction.axisButtonRelease = false;
                            currActiveKeyBindingList.Add(keyBinding);
                        }
                        break;
                    case KeyActionType.Axis:
                        keyAction.axisValue = Input.GetAxis(keyAction.axisName);
                        if (Mathf.Abs(keyAction.axisValue) > 0 || (keyAction.axisValue != keyAction.oldAxisValue))
                        {
                            keyAction.oldAxisValue = keyAction.axisValue;
                            currActiveKeyBindingList.Add(keyBinding);
                        }
                        break;
                    default:
                        break;
                }
            }

            // foreach (var keyBinding in currActiveKeyBindingList)
            // {
            //     if (keyBinding.Key.actionType == KeyActionType.Press || keyBinding.Key.actionType == KeyActionType.Release)
            //         Debug.Log((keyBinding.Key.actionType == KeyActionType.Press ? "按下" : "松开") + " " + keyBinding.Key.keyCode);
            // }

            if (onInput != null)
            {
                if (currActiveKeyBindingList.Count > 0)
                    onInput(currActiveKeyBindingList);
            }
        }
    }

    public class InputState
    {
        public HashSet<KeyCode> KeyCodeList = new HashSet<KeyCode>();
        public Dictionary<string, InputBinding> inputBindingDic = new Dictionary<string, InputBinding>();
        public List<InputBinding> inputBindingList = new List<InputBinding>();

        public void AddInputBinding(string inputBindingName, InputBinding inputBinding)
        {
            if (inputBindingDic.ContainsKey(inputBindingName))
            {
                inputBindingDic.Remove(inputBindingName);
            }
            inputBindingDic.Add(inputBindingName, inputBinding);

            UpdateInputBindingList();
            UpdateKeyCodeList();
        }

        public void RemoveInputBinding(string inputBindingName)
        {
            if (inputBindingDic.ContainsKey(inputBindingName))
            {
                inputBindingDic.Remove(inputBindingName);
            }

            UpdateInputBindingList();
            UpdateKeyCodeList();
        }

        private void UpdateInputBindingList()
        {
            inputBindingList = inputBindingDic.Values.ToList();
        }

        private void UpdateKeyCodeList()
        {
            // 刷新绑定的按键集合 add by TangJian 2018/02/09 16:41:49
            KeyCodeList.Clear();

            foreach (var item in inputBindingDic)
            {
                InputBinding inputBinding = item.Value;
                foreach (var ikeyBinding in inputBinding.keyBindingList)
                {
                    KeyCodeList.Add(ikeyBinding.Key.keyCode);
                }
            }
        }
    }

    public partial class InputManager : MonoBehaviour
    {
        private static InputManager instance;
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<InputManager>();
                }
                return instance;
            }
        }

        private string inputStateName;
        private string InputStateName
        {
            get
            {
                if (inputStateName == null)
                    inputStateName = "";
                return inputStateName;
            }

            set
            {
                inputStateName = value;
            }
        }

        private Dictionary<string, InputState> inputStateDic = new Dictionary<string, InputState>();

        private Dictionary<string, InputState> InputStateDic
        {
            get
            {
                if (inputStateDic == null)
                    inputStateDic = new Dictionary<string, InputState>();
                return inputStateDic;
            }
        }


        // 添加输入绑定 add by TangJian 2018/02/09 16:26:17
        public void AddInputBinding(string inputStateName, string inputBindingName, InputBinding inputBinding)
        {
            if (InputStateDic.ContainsKey(inputStateName) == false)
            {
                InputStateDic.Add(inputStateName, new InputState());
            }

            InputState inputState;
            if (InputStateDic.TryGetValue(inputStateName, out inputState))
            {
                inputState.AddInputBinding(inputBindingName, inputBinding);
            }
        }

        public void RemoveInputBinding(string inputStateName, string inputBindingName)
        {
            InputState inputState;
            if (InputStateDic.TryGetValue(inputStateName, out inputState))
            {
                inputState.RemoveInputBinding(inputBindingName);
            }
        }

        // 切换输入状态 add by TangJian 2018/02/09 16:26:25
        public void SetInputState(string inputStateName)
        {
            this.InputStateName = inputStateName;
        }

        public InputState GetCurrInputState()
        {
            InputState ret;
            if (InputStateDic.TryGetValue(InputStateName, out ret))
                return ret;
            else
                return null;
        }

        void Start()
        {
            // AddInputBinding("normal", "test", new InputBinding(new List<IKeyBinding>()
            // {
            //     // new KeyActionAndRoleActionBinding(new KeyAction(KeyCode.A, KeyActionType.Press), new RoleAction(RoleActionType.Action1_Begin)),
            //     // new KeyActionAndRoleActionBinding(new KeyAction(KeyCode.A, KeyActionType.Release), new RoleAction(RoleActionType.Action1_End))
            // },
            // (List<IKeyBinding> keyBindingList) =>
            // {
            //     foreach (KeyActionAndRoleActionBinding keyBinding in keyBindingList)
            //     {
            //         switch (keyBinding.Action.roleActionType)
            //         {
            //             case RoleActionType.Action1_Begin:
            //                 Debug.Log("RoleActionType.Action1_Begin");
            //                 break;
            //             case RoleActionType.Action1_End:
            //                 Debug.Log("RoleActionType.Action1_End");
            //                 break;
            //         }
            //     }
            // }));
            // SetInputState("normal");
        }

        void UpdateCurrInputState()
        {
            InputState inputState = GetCurrInputState();
            if (inputState != null)
            {
                foreach (var inputBinding in inputState.inputBindingList)
                {
                    inputBinding.Update();
                }
            }
        }

        void Update()
        {
            UpdateCurrInputState();
        }

        public InputActionAsset ActionAsset;
        private PlayerInput playerInput;
        
        private void Awake()
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
            playerInput.actions = ActionAsset;
            SwitchActions(ActionMapName);
        }

        Dictionary<string, List<BindingInputAction>> ActionBindingDic = new Dictionary<string, List<BindingInputAction>>();
        private string ActionMapName = "Gaming";
        
        
        public void SwitchActions(string name)
        {
            playerInput.SwitchActions(name);
            ActionMapName = name;
            ActionAsset.Disable();
            ActionAsset.Enable();
        }

        public void Subscript(string name, BindingInputAction action)
        {
            UnSubscript(name, action.ID);
            
            List<BindingInputAction> actions;
            if (ActionBindingDic.TryGetValue(name, out actions))
            {
                actions.Add(action);
            }
            else
            {
                ActionBindingDic.Add(name, new List<BindingInputAction>(){action});
            }
        }
        
        public void UnSubscript(string name, string actionId)
        {
            List<BindingInputAction> actions;
            if (ActionBindingDic.TryGetValue(name, out actions))
            {
                int index = actions.FindIndex(action => { return action.ID == actionId; });
                if(index >= 0)
                    actions.RemoveAt(index);
            }
        }
        
        void Emit(string actionName, InputValue inputValue)
        {
            List<BindingInputAction> actions;
            if (ActionBindingDic.TryGetValue(ActionMapName, out actions))
            {
                foreach (var action in actions)
                {
                    action.Invoke(actionName, inputValue);
                }
            }
        }
        

        void OnXBoxMove(InputValue inputValue)
        {
            Emit("XBoxMove", inputValue);
        }
        void OnXBoxAction1(InputValue inputValue)
        {
            Emit("XBoxAction1", inputValue);
        }

        void OnXBoxAction2(InputValue inputValue)
        {
            Emit("XBoxAction2", inputValue);
        }
        void OnXBoxRush(InputValue inputValue)
        {
            Emit("XBoxRush", inputValue);
        }
        void OnXBoxAction3(InputValue inputValue)
        {
            Emit("XBoxAction3", inputValue);
        }

        void OnXBoxAction4(InputValue inputValue)
        {
            Emit("XBoxAction4", inputValue);
        }
        void OnXBoxAction5(InputValue inputValue)
        {
            Emit("XBoxAction5", inputValue);
        }

        
        
        void OnMove(InputValue inputValue)
        {
            Emit("Move", inputValue);
        }

        void OnWalkCut(InputValue inputValue)
        {
            Emit("WalkCut", inputValue);
        }

        void OnAlt(InputValue inputValue)
        {
            Emit("Alt",inputValue);
        }
        
        void OnAction1(InputValue inputValue)
        {
            Emit("Action1", inputValue);
        }

        void OnAction2(InputValue inputValue)
        {
            Emit("Action2", inputValue);
        }

        void OnRush(InputValue inputValue)
        {
            Emit("Rush", inputValue);
        }

        void OnJump(InputValue inputValue)
        {
            Emit("Jump", inputValue);
        }

        void OnInteract(InputValue inputValue)
        {
            Emit("Interact", inputValue);
        }

        void OnRoll(InputValue inputValue)
        {
            Emit("Roll", inputValue);
        }
        void OnUse(InputValue inputValue)
        {
            Emit("Use", inputValue);
        }
        
        
        void OnUI_C(InputValue inputValue)
        {
            Emit("UI_C", inputValue);
        }
        void OnUI_Q(InputValue inputValue)
        {
            Emit("UI_Q", inputValue);
        }
        void OnUI_E(InputValue inputValue)
        {
            Emit("UI_E", inputValue);
        }
        void OnUI_B(InputValue inputValue)
        {
            Emit("UI_B", inputValue);
        }
        void OnUI_R(InputValue inputValue)
        {
            Emit("UI_R", inputValue);
        }
        void OnUI_J(InputValue inputValue)
        {
            Emit("UI_J", inputValue);
        }
        void OnUI_K(InputValue inputValue)
        {
            Emit("UI_K", inputValue);
        }
        void OnUI_U(InputValue inputValue)
        {
            Emit("UI_U", inputValue);
        }
        void OnUI_W(InputValue inputValue)
        {
            Emit("UI_W", inputValue);
        }
        void OnUI_A(InputValue inputValue)
        {
            Emit("UI_A", inputValue);
        }
        void OnUI_S(InputValue inputValue)
        {
            Emit("UI_S", inputValue);
        }
        void OnUI_D(InputValue inputValue)
        {
            Emit("UI_D", inputValue);
        }

        void OnG(InputValue inputValue)
        {
            Emit("G", inputValue);
        }

        void OnI(InputValue inputValue)
        {
            Emit("I", inputValue);
        }

        void OnO(InputValue inputValue)
        {
            Emit("O", inputValue);
        }

        void OnP(InputValue inputValue)
        {
            Emit("P", inputValue);
        }

        void OnM(InputValue inputValue)
        {
            Emit("M", inputValue);
        }

        void OnN(InputValue inputValue)
        {
            Emit("N", inputValue);
        }

        void OnAction3(InputValue inputValue)
        {
            Emit("Action3", inputValue);
        }
        void OnAction4(InputValue inputValue)
        {
            Emit("Action4", inputValue);
        }
        void OnAction5(InputValue inputValue)
        {
            Emit("Action5", inputValue);
        }
        void OnAction6(InputValue inputValue)
        {
            Emit("Action6", inputValue);
        }
        void OnAction7(InputValue inputValue)
        {
            Emit("Action7", inputValue);
        }
        void OnAction8(InputValue inputValue)
        {
            Emit("Action8", inputValue);
        }
        void OnAction9(InputValue inputValue)
        {
            Emit("Action9", inputValue);
        }
        void OnAction10(InputValue inputValue)
        {
            Emit("Action10", inputValue);
        }


        void OnKeyBoard1(InputValue inputValue)
        {
            Emit("KeyBoard1", inputValue);
        }
        void OnKeyBoard2(InputValue inputValue)
        {
            Emit("KeyBoard2", inputValue);
        }
        void OnKeyBoard3(InputValue inputValue)
        {
            Emit("KeyBoard3", inputValue);
        }
        void OnKeyBoard4(InputValue inputValue)
        {
            Emit("KeyBoard4", inputValue);
        }
        void OnKeyBoard5(InputValue inputValue)
        {
            Emit("KeyBoard5", inputValue);
        }


        void OnUI_Return(InputValue inputValue)
        {
            Emit("UI_Return", inputValue);
        }
        void OnUI_Escape(InputValue inputValue)
        {
            Emit("UI_Escape", inputValue);
        }
        void OnUI_Space(InputValue inputValue)
        {
            Emit("UI_Space", inputValue);
        }
        void OnUI_Anykey(InputValue inputValue)
        {
            Emit("UI_UI_Anykey", inputValue);
        }

        
    }

    public struct BindingInputAction
    {
        public string ID;
        public Action<string, InputValue> action;

        public BindingInputAction(string id, Action<string, InputValue> action)
        {
            this.ID = id;
            this.action = action;
        }

        public void Invoke(string actionName, InputValue inputValue)
        {
            action?.Invoke(actionName, inputValue);
        }
    }
}