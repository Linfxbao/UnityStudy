using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    private AudioManager audioManager;
    public string mouseHoverSound = "ButtonHover";
    public string buttonPressSound = "ButtonPress";

    void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("Freak out! No audioManager found in the scene!");
        }
    }

    public void OnMouseOver()
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(mouseHoverSound);
        }
    }

    public void Quit()
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(buttonPressSound);
        }

        Debug.Log("Application quit!");
        Application.Quit();
    }

    public void Retry()
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(buttonPressSound);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
