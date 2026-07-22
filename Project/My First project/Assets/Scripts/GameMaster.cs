using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    [SerializeField]
    private int maxLives = 3;

    private static int _remainingLives;
    public static int RemainingLives
    {
        get { return _remainingLives; }
    }

    public static int Money;

    [SerializeField]
    private int startingMoney;

    void Awake()
    {
        if (gm != null && gm != this)
        {
            Debug.LogWarning("GameMaster: duplicate instance detected.");
            return;
        }

        gm = this;
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 2;
    public Transform spawnPrefab;
    public string respawnCountDownSoundName = "RespawnCountDown";
    public string spawnSoundName = "Spawn";
    public string gameOverSoundName = "GameOver";

    public CameraShake cameraShake;

    [SerializeField]
    private GameObject gameOverUI;

    [SerializeField]
    private GameObject upgradeMenu;

    [SerializeField]
    private WaveSpawner waveSpawner;

    public delegate void UpgradeMenuCallback(bool active);
    public UpgradeMenuCallback onToggleUpgradeMenu;

    private AudioManager audioManager;

    void Start()
    {

        _remainingLives = maxLives;

        Money = startingMoney;

        if (cameraShake == null)
        {
            Debug.LogError("There is no camreaShake in GameMaster.cs!!!");
        }

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("Freak out! No AudioManager found in the scene!!");
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.uKey.wasPressedThisFrame)
        {

            ToggleUpgradeMenu();
            
        }
    }

    private void ToggleUpgradeMenu()
    {
        if (upgradeMenu == null)
        {
            Debug.LogError("GameMaster: upgradeMenu is not assigned.");
            return;
        }

        upgradeMenu.SetActive(!upgradeMenu.activeSelf);
        bool isUpgradeMenuActive = upgradeMenu.activeSelf;

        if (waveSpawner != null)
        {
            waveSpawner.enabled = !isUpgradeMenuActive;
        }

        onToggleUpgradeMenu?.Invoke(isUpgradeMenuActive);
    }

    public IEnumerator RespawnPlayer()
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(respawnCountDownSoundName);
        }

        yield return new WaitForSeconds(spawnDelay);

        if (upgradeMenu != null && upgradeMenu.activeSelf)
        {
            upgradeMenu.SetActive(false);

            if (waveSpawner != null)
            {
                waveSpawner.enabled = true;
            }

            onToggleUpgradeMenu?.Invoke(false);
        }

        if (audioManager != null)
        {
            audioManager.PlaySound(spawnSoundName);
        }

        if (playerPrefab == null || spawnPoint == null)
        {
            Debug.LogError("GameMaster: playerPrefab or spawnPoint is not assigned.");
            yield break;
        }

        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        if (spawnPrefab != null)
        {
            GameObject clone = Instantiate(spawnPrefab.gameObject, spawnPoint.position, spawnPoint.rotation);
            Destroy(clone, 3f);
        }

    }

    public void EndGame()
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(gameOverSoundName);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        else
        {
            Debug.LogError("GameMaster: gameOverUI is not assigned.");
        }
    }
    public static void KillPlayer(Player player)
    {
        if (gm == null)
        {
            Debug.LogError("GameMaster: static instance was not initialized.");
            return;
        }

        Destroy(player.gameObject);

        _remainingLives--;
        if (_remainingLives <= 0)
        {
            gm.EndGame();
        }
        else
        {
            gm.StartCoroutine(gm.RespawnPlayer());
        }
    }

    public static void KillEnemy(Enemy enemy)
    {
        if (gm == null)
        {
            Debug.LogError("GameMaster: static instance was not initialized.");
            return;
        }

        gm._KillEnemy(enemy);
    }

    public void _KillEnemy(Enemy _enemy)
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(_enemy.deathSoundName);
        }

        Money += _enemy.moneyDrop;
        if (audioManager != null)
        {
            audioManager.PlaySound("Money");
        }

        if (_enemy.deathParticles != null)
        {
            Transform _clone = (Transform)Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity);
            Destroy(_clone, 1f);
        }

        if (cameraShake != null)
        {
            cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLength);
        }

        Destroy(_enemy.gameObject);
    }
}
