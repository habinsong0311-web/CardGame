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
        Debug.Log("타이틀 돌아가기 버튼 실행");
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
    public void BattleDeckSelectScene()
    {
        SceneManager.LoadScene("BattleDeckSelectScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
