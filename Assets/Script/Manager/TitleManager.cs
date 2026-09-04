using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("BattleField");
    }
    public void ReturnTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
    public void DeckSelectScene()
    {
        SceneManager.LoadScene("DeckSelectScene");
    }
    public void DeckEditorScene()
    {
        SceneManager.LoadScene("DeckEditorScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
