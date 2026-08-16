using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    // 游戏关卡全部结束后显示页面以及按钮、总得分
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI scoreTextMesh;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        scoreTextMesh.text = "FINAL SCORE: " + GameManager.Instance.GetTotalScore().ToString();

        mainMenuButton.Select();
    }
}
