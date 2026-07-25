using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectCharacterController : MonoBehaviour
{
    [Header("场景")]
    public GameObject mainMenuUI;
    public GameObject selectCharacterUI;

    public void OnSelectBack()
    {
        selectCharacterUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void OnSelectCharacter1()
    {
        SceneManager.LoadScene("ZombieLand");
    }

    public void OnSelectCharacter2()
    {
        SceneManager.LoadScene("ZombieLand 1");
    }

    public void OnSelectCharacter3()
    {
        SceneManager.LoadScene("ZombieLand 2");
    }
}
