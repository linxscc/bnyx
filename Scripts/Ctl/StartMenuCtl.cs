using System;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuCtl : Singleton<StartMenuCtl>
{
    private GameObject gameObject;
    private Button playBtn;
    private Button exitBtn;

    public event Action<UICtl.UIState> onClick;

    protected StartMenuCtl()
    {
    }

    public void init(GameObject gb)
    {
        gameObject = gb;
        playBtn = Util.Child(gameObject, "Play").GetComponent<Button>();
        exitBtn = Util.Child(gameObject, "Exit").GetComponent<Button>();
        
        playBtn.onClick.AddListener(()=>
        {
            onClick(UICtl.UIState.ENTER_GAME);
            //gameObject.SetActive(false);
        });

        exitBtn.onClick.AddListener(() =>
        {
            onClick(UICtl.UIState.EXIT_DIAG);
            //gameObject.SetActive(false);
        });
    }
}
