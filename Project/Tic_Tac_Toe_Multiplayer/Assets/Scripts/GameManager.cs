using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    // 创建单例模式
    public static GameManager Instance { get; private set;}

    // 设置事件
    // 点击网格位置
    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    // 开始游戏
    public event EventHandler OnGameStarted;
    // 重置场景
    public event EventHandler OnRematch;
    // 出现平局
    public event EventHandler OnGameTied;
    // 游戏胜利/失败
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs : EventArgs {
        public Line line;
        public PlayerType winPlayerType;
    }
    // 切换落子方
    public event EventHandler OnCurrentPlayablePlayerTypeChanged;
    // 事件参数类
    public class OnClickedOnGridPositionEventArgs : EventArgs {
        public int x;
        public int y;
        public PlayerType playerType;
    }
    // 分数变化
    public event EventHandler OnScoreChanged;
    //放置棋子音效事件
    public event EventHandler OnPlacedObject;

    
    // 玩家状态
    public enum PlayerType {
        None,
        Cross,
        Circle,
    }

    // 连成线的方式：水平、垂直、对角线
    public enum Orientation {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB,
    }

    // 棋子位置；行、列、对角线三个棋子的中心位置；连线角度
    public struct Line/* : INetworkSerializable*/ {
        public List<Vector2Int> gridVector2IntList;
        public Vector2Int centerGridPosition;
        public Orientation orientation;

        // public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        //     serializer.SerializeValue(ref centerGridPosition);
        //     serializer.SerializeValue(ref orientation);
        //     serializer.SerializeValue(ref gridVector2IntList);
        // }
    }

    // 当前用户所执棋子
    private PlayerType localPlayerType;
    // 现在落子的用户所执棋子
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();
    // 记录棋盘当前状况
    private PlayerType[,] playerTypeArray;
    // 胜利的可能排列
    private List<Line> lineList;
    // 玩家分数
    private NetworkVariable<int> playerCrossScore = new NetworkVariable<int>();
    private NetworkVariable<int> playerCircleScore = new NetworkVariable<int>();


    private void Awake() {
        // 初始化单例
        if (Instance != null) {
            Debug.LogError("More than one GameManager instance!");
        }
        Instance = this;

        playerTypeArray = new PlayerType[3, 3];

        // 设置胜利的可能
        lineList = new List<Line>{
            // 垂直
            new Line {
                gridVector2IntList = new List<Vector2Int> {new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2)},
                centerGridPosition = new Vector2Int(0, 1),
                orientation = Orientation.Vertical,
            },
            new Line {
                gridVector2IntList = new List<Vector2Int> {new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2)},
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.Vertical,
            },
            new Line {
                gridVector2IntList = new List<Vector2Int> {new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2)},
                centerGridPosition = new Vector2Int(2, 1),
                orientation = Orientation.Vertical,
            },
            // 水平
            new Line {
                gridVector2IntList = new List<Vector2Int> {new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0)},
                centerGridPosition = new Vector2Int(1, 0),
                orientation = Orientation.Horizontal,
            },
            new Line {
                gridVector2IntList = new List<Vector2Int> {new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1)},
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.Horizontal,
            },
            new Line {
                gridVector2IntList = new List<Vector2Int> {new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2)},
                centerGridPosition = new Vector2Int(1, 2),
                orientation = Orientation.Horizontal,
            },

            // 对角线
            new Line {
                gridVector2IntList = new List<Vector2Int> {new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2)},
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.DiagonalA,
            },
            new Line {
                gridVector2IntList = new List<Vector2Int> {new Vector2Int(2, 0), new Vector2Int(1, 1), new Vector2Int(0, 2)},
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.DiagonalB,
            },
        };
    }

    // 客户端连接成功并完成同步后执行
    public override void OnNetworkSpawn() {
        Debug.Log("OnNetworkSpawn: " + NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == 0) {
            localPlayerType = PlayerType.Cross;
        } else {
            localPlayerType = PlayerType.Circle;
        }

        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }

        currentPlayablePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) => {
             OnCurrentPlayablePlayerTypeChanged?.Invoke(this, EventArgs.Empty);
        };

        playerCrossScore.OnValueChanged += (int preScore, int newScore) => {
            OnScoreChanged?.Invoke(this, EventArgs.Empty);
        };
        playerCircleScore.OnValueChanged += (int preScore, int newScore) => {
            OnScoreChanged?.Invoke(this, EventArgs.Empty);
        };
    }

    // 当前有两个玩家后开始游戏
    private void NetworkManager_OnClientConnectedCallback(ulong obj) {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2) {
            // 开始游戏
            currentPlayablePlayerType.Value = PlayerType.Cross;

            TriggerOnGameStartedRpc();
        }

    }

    // OnGameStarted事件触发
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc() {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    // 点击区域后调用， 因为该逻辑只在服务器中调用，因此不用广播到客户端
    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType) {
        Debug.Log("ClickOnGridPosition: " + x + "," + y);
        
        //判断点击的用户是否是当前落棋的玩家
        if (playerType != currentPlayablePlayerType.Value) {
            return;
        }

        // 判断点击的区域是否是空的
        if (playerTypeArray[x, y] != PlayerType.None) {
            return;
        }

        // 设置当前位置为相应的棋子
        playerTypeArray[x, y] = playerType;
        // 播放音效
        TriggerOnPlacedObjectRpc();

        // 设置OnClickedOnGridPosition事件参数对象
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs {
            x = x,
            y = y,
            playerType = playerType,
        });

        // 切换落子玩家
        switch (currentPlayablePlayerType.Value) {
            default:
            case PlayerType.Cross:
                currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }
        // 检测是否胜利
        TestWinner();
    }

    // 音效
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnPlacedObjectRpc() {
        OnPlacedObject?.Invoke(this, EventArgs.Empty);
    }

    // 判断三个棋子是否都是同一类型
    private bool TestWinnerLine(Line line) {
        return TestWinnerLine(
            playerTypeArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y],
            playerTypeArray[line.gridVector2IntList[1].x, line.gridVector2IntList[1].y],
            playerTypeArray[line.gridVector2IntList[2].x, line.gridVector2IntList[2].y]
        );
    }

    private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType) {
        return aPlayerType != PlayerType.None && aPlayerType == bPlayerType && bPlayerType == cPlayerType;
    }

    private void TestWinner() {
        for (int i = 0; i < lineList.Count; i++) {
            Line line = lineList[i];
            // 若有胜利方，则设置胜利方，并加分
            if (TestWinnerLine(line)) {
                Debug.Log("Winner!");
                currentPlayablePlayerType.Value = PlayerType.None; 
                PlayerType winPlayerType = playerTypeArray[line.centerGridPosition.x, line.centerGridPosition.y];
                switch (winPlayerType) {
                    default:
                    case PlayerType.Cross:
                        playerCrossScore.Value++;
                        break;
                    case PlayerType.Circle:
                        playerCircleScore.Value++;
                        break;
                }
                TriggerOnGameWinRpc(i, winPlayerType);
                return;
            }
        }

        // 判断是否是平局
        bool hasTie = true;
        for (int x = 0; x < playerTypeArray.GetLength(0); x++) {
            for (int y = 0; y < playerTypeArray.GetLength(1); y++) {
                if (playerTypeArray[x, y] == PlayerType.None) {
                    hasTie = false;
                    break;
                }
            }
        }

        if (hasTie) {
            TriggerOnGameTiedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameTiedRpc() {
        OnGameTied?.Invoke(this, EventArgs.Empty);
    }

    // 传递胜利方信息
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex, PlayerType winPlayerType) {
        Line line = lineList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs {
            line = line,
            winPlayerType = winPlayerType,
        });
    }

    // 重置游戏
    [Rpc(SendTo.Server)]
    public void RematchRpc() {
        // 设置当前棋盘为空
        for (int x = 0; x < playerTypeArray.GetLength(0); x++) {
            for (int y = 0; y < playerTypeArray.GetLength(1); y++) {
                playerTypeArray[x, y] = PlayerType.None;
            }
        }
        currentPlayablePlayerType.Value = PlayerType.Cross;

        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnRematchRpc() {
        OnRematch?.Invoke(this, EventArgs.Empty);
    }

    public PlayerType GetLocalPlayerType() {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayablePlayerType() {
        return currentPlayablePlayerType.Value;
    }

    // 返回当前二者分数， out规定参数必须在函数中赋值
    public void GetScores(out int playerCrossScore, out int playerCircleScore) {
        playerCrossScore = this.playerCrossScore.Value;
        playerCircleScore = this.playerCircleScore.Value;
    }

}
