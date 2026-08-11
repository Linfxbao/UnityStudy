using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    // 设置游戏中的光标、指示当前落子方的箭头、玩家当前分数
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;
    [SerializeField] private GameObject crossYouTextMeshGameObject;
    [SerializeField] private GameObject circleYouTextMeshGameObject;
    [SerializeField] private TextMeshProUGUI playerCrossScoreTextMesh;
    [SerializeField] private TextMeshProUGUI playerCircleScoreTextMesh; 

    private void Awake() {
        crossArrowGameObject.SetActive(false);
        circleArrowGameObject.SetActive(false);
        crossYouTextMeshGameObject.SetActive(false);
        circleYouTextMeshGameObject.SetActive(false);

        playerCrossScoreTextMesh.text = "";
        playerCircleScoreTextMesh.text = "";
    }

    private void Start() {
        // 在游戏开始后，控制指示当前落子玩家的"YOU"字UI和光标显示
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        // 落子玩家改变后相应的UI改变时调用
        GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;
        // 分出胜负后更新玩家分数时调用
        GameManager.Instance.OnScoreChanged += GameManager_OnScoreChanged;
    }

    private void GameManager_OnScoreChanged(object sender, System.EventArgs e) {
        // 使用out获得当前双方的分数，转化为分数显示在页面上
        GameManager.Instance.GetScores(out int playerCrossScore, out int playerCircleScore);
        playerCrossScoreTextMesh.text = playerCrossScore.ToString();
        playerCircleScoreTextMesh.text = playerCircleScore.ToString();
    }

    private void GameManager_OnCurrentPlayablePlayerTypeChanged(object sender, System.EventArgs e) {
        UpdateCurrentArrow();
    }

    private void GameManager_OnGameStarted(object sender, System.EventArgs e) {
    // 根据玩家双方，设置他们自身的"YOU"显示
        if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Cross) {
            crossYouTextMeshGameObject.SetActive(true);
        } else {
            circleYouTextMeshGameObject.SetActive(true);
        }
        playerCrossScoreTextMesh.text = "";
        playerCircleScoreTextMesh.text = "";

        UpdateCurrentArrow();
    }

    // 根据当前落子玩家设置光标显示与不显示
    private void UpdateCurrentArrow() {
        if (GameManager.Instance.GetCurrentPlayablePlayerType() == GameManager.PlayerType.Cross) {
            crossArrowGameObject.SetActive(true);
            circleArrowGameObject.SetActive(false);
        } else {
            crossArrowGameObject.SetActive(false);
            circleArrowGameObject.SetActive(true);
        }
    }
}
