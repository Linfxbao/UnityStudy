using System;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
   
    private static int totalScore = 0;
    private static int levelNumber = 1;

    // 游戏完成后再次游玩
    public static void ResetStaticData()
    {
        levelNumber = 1;
        totalScore = 0;
    }

    // 游戏场景预制件列表，用于切换游戏场景
    [SerializeField] private List<GameLevel> gameLevelList;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;

    private int score;
    private float time;
    private bool isTimerActive;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        score = 0;
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnStateChanged += Lander_OnStateChanged;

        GameInput.Instance.OnMenuButtonPressed += GameInput_OnMenuButtonPressed;
        LoadCurrentLevel();
    }

    // 主页面按钮
    private void GameInput_OnMenuButtonPressed(object obj, System.EventArgs e)
    {
        PauseUnPauseGame();
    }

    // 玩家一控制，角色游戏对象状态就改变，此时将摄像机视角切换为游戏对象
    private void Lander_OnStateChanged(object sender, Lander.OnStateChangedEventArgs e)
    {
        isTimerActive = e.state == Lander.State.Normal;

        if (e.state == Lander.State.Normal)
        {
            cinemachineCamera.Target.TrackingTarget = Lander.Instance.transform;
            CinemachineCameraZoom2D.Instance.SetNormalOrthographicSize();

        }
    }

    private void Update()
    {
        if (isTimerActive)
        {
            time += Time.deltaTime;
        }
    }

    // 加载当前关卡场景预制件，同时初始化摄像机视角
    private void LoadCurrentLevel()
    {
        GameLevel gameLevel = GetGameLevel();
        GameLevel spawnedGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);
        Lander.Instance.transform.position = spawnedGameLevel.GetLanderStartPosition();
        cinemachineCamera.Target.TrackingTarget = spawnedGameLevel.GetCameraStartTargetTransform();
        CinemachineCameraZoom2D.Instance.SetTargetOrthographicSize(spawnedGameLevel.GetZoomedOutOrthographicSize());
    }

    // 获得下一个游戏关卡场景
    private GameLevel GetGameLevel()
    {
        foreach (GameLevel gameLevel in gameLevelList)
        {
            if (gameLevel.GetLevelNumber() == levelNumber)
            {
                return gameLevel;
            }
        }
        return null;
    }

    // 角色成功着陆加分
    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AddScore(e.score);
    }

    // 角色拾取金币加分
    private void Lander_OnCoinPickup(object sender, System.EventArgs e)
    {
        AddScore(500);
    }

    // 加分
    public void AddScore(int addScoreAmount)
    {
        score += addScoreAmount;
        // Debug.Log(score);
    }

    // 获取游戏时间
    public float GetTime()
    {
        return time;
    }

    // 获取当前关卡分数
    public int GetScore()
    {
        return score;
    }

    // 获取总分
    public int GetTotalScore()
    {
        return totalScore;
    }

    // 加载下一个关卡，若已是最后一关，则加载GameOverScene场景
    public void GoToNextLevel()
    {
        levelNumber++;
        totalScore += score;
        if (GetGameLevel() == null)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameOverScene);
        } else
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        }

    }

    // 重试，重新加载当前关卡
    public void RetryLevel()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }

    // 游戏中按下暂停键
    private void PauseUnPauseGame()
    {
        if (Time.timeScale == 1f)
        {
            PauseGame();
        } else
        {
            UnPauseGame();
        }
    }

    // 暂停游戏
    public void PauseGame()
    {
        Time.timeScale = 0f;
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }

    // 继续游戏
    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        OnGameUnPaused?.Invoke(this, EventArgs.Empty);
    }
}
