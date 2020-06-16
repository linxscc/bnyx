using UnityEngine.Experimental.Input.Plugins.PlayerInput;

namespace Tang
{
    public interface IInputable
    {
        void OnInput(string name, InputValue inputValue);
    }
}