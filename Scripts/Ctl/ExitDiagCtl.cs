using System;
using UnityEngine;
using UnityEngine.UI;

internal class ExitDiagCtl:Singleton<ExitDiagCtl>
{
    private GameObject gameObject;
    private Button yesBtn;
    private Button noBtn;

    public event Action<UICtl.UIState> onClick;

    public void init(GameObject gb)
    {
        gameObject = gb;
        yesBtn = Util.Child(gameObject, "Yes").GetComponent<Button>();
        noBtn = Util.Child(gameObject, "No").GetComponent<Button>();

        yesBtn.onClick.AddListener(() =>
        {
            onClick(UICtl.UIState.EXIT_GAME);
        });

        noBtn.onClick.AddListener(() =>
        {
            onClick(UICtl.UIState.START_MENU);
        });
    }

    protected ExitDiagCtl()
    {
        
    }

}