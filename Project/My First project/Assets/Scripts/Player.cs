using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public int fallBoundary = -20;

    [Header("Optional: ")]
    [SerializeField]
    private StatusIndicator statusIndicator;

    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "Grunt";

    private AudioManager audioManager;

    private PlayerStats stats;

    void Start()
    {
        stats = PlayerStats.instance ?? FindFirstObjectByType<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("Player: PlayerStats instance was not found in the scene.");
            enabled = false;
            return;
        }

        stats.curHealth = stats.maxHealth;

        if (statusIndicator == null)
        {
            Debug.LogError("There is no status indicator!!!");
        }
        else
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }

        if (GameMaster.gm != null)
        {
            GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;
        }
        else
        {
            Debug.LogError("Player: GameMaster instance was not found in the scene.");
        }

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("Freak out! No audioManager found in scence!");
        }

        if (stats.healthRegenRate > 0f)
        {
            float regenInterval = 1f / stats.healthRegenRate;
            InvokeRepeating(nameof(RegenHealth), regenInterval, regenInterval);
        }
    }

    void RegenHealth()
    {
        if (stats == null)
        {
            return;
        }

        stats.curHealth += 1;

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }
    }

    void Update()
    {
        if (transform.position.y <= fallBoundary)
        {
            DamagePlayer(999999999);
        }
    }

    void OnUpgradeMenuToggle(bool active)
    {
        GetComponent<PlayerController>().enabled = !active;
       weapon _weapon = GetComponentInChildren<weapon>();
        if (_weapon != null)
        {
            _weapon.enabled = !active;
        }
    }
    void OnDestroy()
    {
        if (GameMaster.gm != null)
        {
            GameMaster.gm.onToggleUpgradeMenu -= OnUpgradeMenuToggle;
        }
    }

    public void DamagePlayer(int damage)
    {
        if (stats == null)
        {
            return;
        }

        stats.curHealth -= damage;

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }

        if (stats.curHealth <= 0)
        {
            if (audioManager != null)
            {
                audioManager.PlaySound(deathSoundName);
            }

            GameMaster.KillPlayer(this);
            return;
        }

        if (audioManager != null)
        {
            audioManager.PlaySound(damageSoundName);
        }
    }

}
