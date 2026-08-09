using UnityEngine;

public class SoundManager : MonoBehaviour
{
   [SerializeField] private Transform placeSfxPrefab;
   [SerializeField] private Transform winSfxPrefab;
   [SerializeField] private Transform lossSfxPrefab;

    private void Start() {
        GameManager.Instance.OnPlacedObject += GameManager_OnPlacedObject;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e) {
        if (GameManager.Instance.GetLocalPlayerType() == e.winPlayerType) {
            Transform SfxTransform = Instantiate(winSfxPrefab);
            Destroy(SfxTransform.gameObject, 5f);
        } else {
            Transform SfxTransform = Instantiate(lossSfxPrefab);
            Destroy(SfxTransform.gameObject, 5f);
        }
    }

    private void GameManager_OnPlacedObject(object sender, System.EventArgs e) {
        Transform SfxTransform = Instantiate(placeSfxPrefab);
        Destroy(SfxTransform.gameObject, 5f);
    }

}
