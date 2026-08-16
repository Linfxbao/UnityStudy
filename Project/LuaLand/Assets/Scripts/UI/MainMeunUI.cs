using UnityEngine;
using UnityEngine.UI;

public class MainMeunUI : MonoBehaviour
{
    // 开始界面的按钮事件设置
    [SerializeField] private Button playBotton;
    [SerializeField] private Button quitBotton;

    private void Awake()
    {
        // ？为什么
        Time.timeScale = 1f;

        playBotton.onClick.AddListener(() =>
        {
            GameManager.ResetStaticData();
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        });

        quitBotton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void Start()
    {
        playBotton.Select();
    }
}
