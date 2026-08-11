using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake() {

        // 按下UI按钮后将其设置为主机或客户机
        startHostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Hide();
        });
        startClientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
