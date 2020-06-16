using UnityEngine;
using UnityEngine.Assertions;

public class GameCtl : Singleton<GameCtl>
{
    private GameObject gameObject;
  
    public GameObject m_Game;
    public GameObject m_UI;
    public UICtl m_UICtl;

    private GameState curGameState = GameState.START;
    public void init(GameObject app) 
    {
        Assert.IsNotNull(app);
        gameObject = app;
        m_Game = Util.Child(this.gameObject, "Game");
        m_UI = Util.Child(this.gameObject, "Camera/UI");

        Assert.IsNotNull(m_Game);
        Assert.IsNotNull(m_UI);

        UICtl.Instance.init(m_UI);

        switchGameState(GameState.START);
    }

    private void startGame()
    {
        Util.Log("startGame");
        m_Game.SetActive(false);
        m_UI.SetActive(true);
    }

    private void playGame()
    {
        Util.Log("playGame");
        m_Game.SetActive(true);
        m_UI.SetActive(false);
    }

    public void switchGameState(GameState state)
    {
        this.curGameState = state;
        switch (state)
        {
            case GameState.START:
                startGame();
                break;
            case GameState.PAUSE:
                break;
            case GameState.PLAY:
                playGame();
                break;
            default:
                break;
        }
    }

    protected GameCtl()
    {
     
    }

    public enum GameState
    {
        START,
        PAUSE,
        PLAY
    }
}
