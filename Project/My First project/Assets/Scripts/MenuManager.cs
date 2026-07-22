using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    [SerializeField]
    string hoverOverSound = "ButtonHover";

    [SerializeField]
    string pressButtonSound = "ButtonPress";

    AudioManager audioManager;
    void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audioManager found!");
        }
    }

    public void StartGame()
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(pressButtonSound);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("WE QUIT THE GAME!");
        Application.Quit();
    }
    public void OnMouseOver()
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(hoverOverSound);
        }
    }
}
