using UnityEngine;
using FairyGUI;


namespace Tang
{
    public class ConfigUIController : MyMonoBehaviour
    {
        GComponent ui;
        GButton button_back;

        void Start()
        {
            ui = GetComponent<UIPanel>().ui;

            button_back = ui.GetChild("Button_Back").asButton;
            button_back.GetChild("text").text = "返回菜单";
            button_back.onClick.Add(() =>
            {
                Debug.Log("返回菜单");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            });
        }
    }
}