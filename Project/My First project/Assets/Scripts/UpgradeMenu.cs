using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField]
    private Text healthText;
    
    [SerializeField]
    private Text speedText;

    [SerializeField]
    private float healthMultiplier = 1.3f;

    [SerializeField]
    private float moveSpeedMultiplier = 1.1f;
    
    [SerializeField]
    private int upgradeCost = 50;

    private PlayerStats stats;

    void OnEnable()
    {
        stats = PlayerStats.instance ?? FindFirstObjectByType<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("UpgradeMenu: PlayerStats instance was not found in the scene.");
            enabled = false;
            return;
        }

        UpdateValues();
    }

    void UpdateValues()
    {
        if (healthText == null || speedText == null || stats == null)
        {
            return;
        }

        healthText.text = "HEALTH: " + stats.maxHealth.ToString();
        speedText.text = "SPEED: " + stats.moveSpeed.ToString();
    }

    public void UpgradeHealth()
    {
        if (GameMaster.Money < upgradeCost)
        {
            AudioManager.instance.PlaySound("NoMoney");
            return;
        }

        stats.maxHealth = (int) (stats.maxHealth * healthMultiplier);
        GameMaster.Money -= upgradeCost;
        AudioManager.instance.PlaySound("Money");
        UpdateValues();
    }
    public void UpgradeSpeed()
    {
        if (GameMaster.Money < upgradeCost)
        {
            AudioManager.instance.PlaySound("NoMoney");
            return;
        }
        stats.moveSpeed = Mathf.Round(stats.moveSpeed * moveSpeedMultiplier);
        GameMaster.Money -= upgradeCost;
        AudioManager.instance.PlaySound("Money");
        UpdateValues();
    }
}
