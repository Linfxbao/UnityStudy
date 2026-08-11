using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 放置音效、胜利音效、失败音效
   [SerializeField] private Transform placeSfxPrefab;
   [SerializeField] private Transform winSfxPrefab;
   [SerializeField] private Transform lossSfxPrefab;

    private void Start() {
        // 订阅放置函数和游戏结束函数
        GameManager.Instance.OnPlacedObject += GameManager_OnPlacedObject;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e) {
        // 如果当前运行的页面是胜利方的页面则播放胜利音效，反之播放失败音效
        if (GameManager.Instance.GetLocalPlayerType() == e.winPlayerType) {
            Transform SfxTransform = Instantiate(winSfxPrefab);
            Destroy(SfxTransform.gameObject, 5f);
        } else {
            Transform SfxTransform = Instantiate(lossSfxPrefab);
            Destroy(SfxTransform.gameObject, 5f);
        }
    }

    // 放置棋子时播放放置音效
    private void GameManager_OnPlacedObject(object sender, System.EventArgs e) {
        Transform SfxTransform = Instantiate(placeSfxPrefab);
        Destroy(SfxTransform.gameObject, 5f);
    }

}
