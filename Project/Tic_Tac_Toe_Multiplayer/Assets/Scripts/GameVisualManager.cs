using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameVisualManager : NetworkBehaviour
{
    // 棋子区域大小
    private const float GRID_SIZE = 3.1f;

    // 棋子预制件
    [SerializeField]
    private Transform crossPrefab;
    [SerializeField]
    private Transform circlePrefab;

    // 胜利线条预制件
    [SerializeField]
    private Transform lineCompletePrefab;

    // 记录当前游戏界面中生成的所有UI对象(重置后需要删除的)
    private List<GameObject> visualGameObjectList;

    private void Awake() {
        visualGameObjectList = new List<GameObject>();
    }

    private void Start() {
        // 订阅事件
        GameManager.Instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPosition;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
    }

    // 删除visualGameObjectList中的对象，清空实现重置游戏的功能
    private void GameManager_OnRematch(object sender, System.EventArgs e) {
        
        if (!NetworkManager.Singleton.IsServer) {
            return;
        }

        foreach (GameObject visualGameObject in visualGameObjectList) {
            Destroy(visualGameObject);
        }

        visualGameObjectList.Clear();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e) {
        if (!NetworkManager.Singleton.IsServer) {
            return;
        }

        // 不同的胜利方法显示线条不同角度
        float eulerZ;
        switch (e.line.orientation) {
            default:
            case GameManager.Orientation.Horizontal:
                eulerZ = 0f;
                break;
            case GameManager.Orientation.Vertical:
                eulerZ = 90f;
                break;
            case GameManager.Orientation.DiagonalA:
                eulerZ = 45f;
                break;
            case GameManager.Orientation.DiagonalB:
                eulerZ = -45f;
                break;                
        }

        // 生成线条
        Transform lineCompleteTransform = 
        Instantiate(lineCompletePrefab,
                    GetGridWorldPosition(e.line.centerGridPosition.x, e.line.centerGridPosition.y),
                    Quaternion.Euler(0, 0, eulerZ)
                    );
        lineCompleteTransform.GetComponent<NetworkObject>().Spawn(true);

        visualGameObjectList.Add(lineCompleteTransform.gameObject);
    }

    private void GameManager_OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e) {
        Debug.Log("GameManager_OnClickedOnGridPosition");
        SpawnObjectRpc(e.x, e.y, e.playerType);
    }

    // 生成棋子预制件
    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, GameManager.PlayerType playerType) {
        Debug.Log("SpawnObject");
        Transform prefab;
        switch (playerType) {
            default:
            case GameManager.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;
                break;
            
        }
        Transform spawnedCrossTransform = Instantiate(prefab, GetGridWorldPosition(x, y), Quaternion.identity);
        // 将棋子作为网络对象在服务器上生成并同步给所有客户端
        spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);

        visualGameObjectList.Add(spawnedCrossTransform.gameObject);
    }

    private Vector2 GetGridWorldPosition(int x, int y) {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
