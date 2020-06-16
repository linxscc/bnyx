using UnityEngine;
using UnityEngine.Assertions;

public class UICtl : Singleton<UICtl>
{
    private GameObject gameObject;

    public GameObject m_ExitDiag;
    public GameObject m_StartMenu;
    private UIState curUIState = UIState.START_MENU;

    public void init(GameObject gb)
    {
        Assert.IsNotNull(gb);
        this.gameObject = gb;
        m_StartMenu = Util.Child(this.gameObject, "StartMenu");
        m_ExitDiag = Util.Child(this.gameObject, "ExitDiag");

        Assert.IsNotNull(m_StartMenu);
        Assert.IsNotNull(m_ExitDiag);

        StartMenuCtl.Instance.init(m_StartMenu);
        StartMenuCtl.Instance.onClick += switchUIState;

        ExitDiagCtl.Instance.init(m_ExitDiag);
        ExitDiagCtl.Instance.onClick += switchUIState;
        this.switchUIState(UIState.START_MENU);
    }

    private void exitGame()
    {
        Debug.Log("exitGame");
        Application.Quit();
    }

    private void showStartMenu()
    {
        Debug.Log("showStartMenu");
        m_StartMenu.SetActive(true);
        m_ExitDiag.SetActive(false);
    }

    private void enterGame()
    {
        Debug.Log("enterGame");
        GameCtl.Instance.switchGameState(GameCtl.GameState.PLAY);
    }

    private void showExitDiag()
    {
        Debug.Log("showExitDiag");
        m_ExitDiag.SetActive(true);
        m_StartMenu.SetActive(false);
    }

    public void switchUIState(UIState state)
    {
        this.curUIState = state;
        switch (state)
        {
            case UIState.START_MENU:
                showStartMenu();
                break;
            case UIState.EXIT_DIAG:
                showExitDiag();
                break;
            case UIState.ENTER_GAME:
                enterGame();
                break;
            case UIState.EXIT_GAME:
                exitGame();
                break;
            default:
                break;
        }
    }

    protected UICtl() { }

    public enum UIState
    {
        START_MENU,
        EXIT_DIAG,
        ENTER_GAME,
        EXIT_GAME
    }

}

