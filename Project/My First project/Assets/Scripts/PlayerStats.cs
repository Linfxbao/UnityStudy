using UnityEngine;
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public int maxHealth = 200;

    public int _curHealth;
    public int curHealth
    {
        get { return _curHealth; }
        set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    public float healthRegenRate = 2f;

    public float moveSpeed = 5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        curHealth = maxHealth;
    }
}