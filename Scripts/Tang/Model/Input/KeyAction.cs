using UnityEngine;

namespace Tang
{
    [System.Serializable]
    public class KeyAction
    {

        public static KeyAction CreateAxisButtonPress(string axisName, float axisValueThreshold)
        {
            KeyAction keyAction = new KeyAction();
            keyAction.actionType = KeyActionType.AxisButtonPress;
            keyAction.axisName = axisName;
            keyAction.axisValueThreshold = axisValueThreshold;

            keyAction.axisValueMonitor = new ValueMonitor<bool>(() =>
                {
                    keyAction.axisValue = Input.GetAxis(keyAction.axisName);
//                    Debug.Log(axisName + "CreateAxisButtonPress = " + keyAction.axisValue);

                    return Mathf.Abs(keyAction.axisValue - keyAction.axisValueThreshold) < 0.001f;

                },
                (bool from, bool to) =>
                {
                    if (to)
                        keyAction.axisButtonPress = to;
                });

            return keyAction;
        }

        public static KeyAction CreateAxisButtonRelease(string axisName, float axisValueThreshold)
        {
            KeyAction keyAction = new KeyAction();
            keyAction.actionType = KeyActionType.AxisButtonRelease;
            keyAction.axisName = axisName;
            keyAction.axisValueThreshold = axisValueThreshold;

            keyAction.axisValueMonitor = new ValueMonitor<bool>(() =>
                {
                    keyAction.axisValue = Input.GetAxis(keyAction.axisName);
//                    Debug.Log(axisName + "CreateAxisButtonRelease = " + keyAction.axisValue);

                    return Mathf.Abs(keyAction.axisValue - keyAction.axisValueThreshold) < 0.001f;

                },
                (bool from, bool to) =>
                {
                    if (to)
                        keyAction.axisButtonRelease = to;
                });

            return keyAction;
        }

        public static KeyAction CreateButtonPress(string buttonName)
        {
            KeyAction keyAction = new KeyAction();
            keyAction.actionType = KeyActionType.ButtonPress;
            keyAction.buttonName = buttonName;
            return keyAction;
        }

        public static KeyAction CreateButtonRelease(string buttonName)
        {
            KeyAction keyAction = new KeyAction();
            keyAction.actionType = KeyActionType.ButtonRelease;
            keyAction.buttonName = buttonName;
            return keyAction;
        }

        public static KeyAction CreateAxis(string axisName)
        {
            KeyAction keyAction = new KeyAction();
            keyAction.actionType = KeyActionType.Axis;
            keyAction.axisName = axisName;

            //            keyAction.axisValueMonitor = new ValueMonitor<bool>(() =>
            //                {
            //                    keyAction.axisValue = Input.GetAxis(keyAction.axisName);
            //                    return keyAction.axisValue;
            //                    
            //                },
            //                (float from, float to) =>
            //                {
            //                    if (to == 0)
            //                    {
            //                        keyAction.axisButtonRelease = true;
            //
            //                    }
            //
            //                });

            return keyAction;
        }

        public static KeyAction CreateKeyPress(KeyCode keyCode)
        {
            KeyAction keyAction = new KeyAction();
            keyAction.actionType = KeyActionType.Press;
            keyAction.keyCode = keyCode;
            return keyAction;
        }

        public static KeyAction CreateKeyRelease(KeyCode keyCode)
        {
            KeyAction keyAction = new KeyAction();
            keyAction.actionType = KeyActionType.Release;
            keyAction.keyCode = keyCode;
            return keyAction;
        }

        public string axisName = "";
        public float axisValue = 0;
        public float oldAxisValue = -1;
        public float axisValueThreshold = 1;

        public bool axisEnable = false;
        public bool axisButtonPress = false;
        public bool axisButtonRelease = false;

        private ValueMonitor<bool> axisValueMonitor;

        public string buttonName;

        public KeyCode keyCode;
        public KeyActionType actionType;

        public KeyAction()
        {
        }

        public override int GetHashCode()
        {
            return (int)keyCode + (int)actionType * 1000;
        }

        public override bool Equals(object obj)
        {
            KeyAction keyAction = obj as KeyAction;
            if (keyAction == null)
            {
                return false;
            }
            if (GetHashCode() == keyAction.GetHashCode())
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return Tools.Obj2Json<KeyAction>(this);
        }

        public void Update()
        {
            if (axisValueMonitor != null)
            {
                axisValueMonitor.Update();
            }
        }
    }
}