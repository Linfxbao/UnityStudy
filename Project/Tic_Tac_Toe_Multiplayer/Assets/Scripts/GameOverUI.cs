using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    // 游戏结束UI
    [SerializeField] private TextMeshProUGUI resultTextMesh;
    // 胜利、失败、平局字体颜色
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Color tieColor;
    // 再来一把按钮
    [SerializeField] private Button rematchButton;

    private void Awake() {
        // 按下按钮，执行重置函数
        rematchButton.onClick.AddListener(() => {
            GameManager.Instance.RematchRpc();
        });
    }

    private void Start() {
        // 订阅相关事件
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
        GameManager.Instance.OnGameTied += GameManager_OnGameTied;

        Hide();
    }

    // 当出现平局时显示文字为平局并设置字体颜色
    private void GameManager_OnGameTied(object sender, System.EventArgs e) {
        resultTextMesh.text = "TIE!";
        resultTextMesh.color = tieColor;
        Show();
    }

    private void GameManager_OnRematch(object sender, System.EventArgs e) {
        Hide();
    }

    // 当游戏结束时按照页面玩家是胜利或失败显示文字并设置字体颜色
    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e) {
        if (e.winPlayerType == GameManager.Instance.GetLocalPlayerType()) {
            resultTextMesh.text = "YOU WIN!";
            resultTextMesh.color = winColor;
        } else {
            resultTextMesh.text = "YOU LOSE!";
            resultTextMesh.color = loseColor;
        }
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
