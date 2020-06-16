using UnityEngine;
using FairyGUI;


namespace Tang
{
    public class MenuUIController : MyMonoBehaviour
    {
        GComponent menu;
        GButton button_tart;
        GButton button_config;
        GButton button_quit;

        void Start()
        {
            menu = GetComponent<UIPanel>().ui;

            button_tart = menu.GetChild("Button_Start").asButton;
            button_tart.GetChild("text").text = "开始游戏";
            button_tart.onClick.Add(() =>
            {
                Debug.Log("开始游戏");
                UnityEngine.SceneManagement.SceneManager.LoadScene("MapEditor");
            });


            button_config = menu.GetChild("Button_Config").asButton;
            button_config.GetChild("text").text = "设置";
            button_config.onClick.Add(() =>
                        {
                            Debug.Log("设置");
                            UnityEngine.SceneManagement.SceneManager.LoadScene("Config");
                        });

            button_quit = menu.GetChild("Button_Quit").asButton;
            button_quit.GetChild("text").text = "退出";
            button_quit.onClick.Add(() =>
            {
                Debug.Log("退出游戏");
                Application.Quit();
            });
        }
    }
}