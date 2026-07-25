using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("场景")]
    public GameObject mainMenuUI;
    public GameObject selectCharacterUI;

    public void OnSelectCharacter()
    {
        selectCharacterUI.SetActive(true);
        mainMenuUI.SetActive(false);
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("ZombieLand");
    }

    public void OnQuitButton()
    {
        Debug.LogError("退出游戏");
        Application.Quit();
    }
}
