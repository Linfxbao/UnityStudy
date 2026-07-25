using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("武器")]
    public GameObject weapon;
    public GameObject pickUpWeapon;
    public Punch punch;

    [Header("玩家")]
    public PlayerScript player;
    private float radius = 2.5f;

    public GameObject lightobj;

    private void Awake()
    {
        weapon.SetActive(false);
        punch.enabled = true;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ObjectiveController.instance.obj1 = true;
                ObjectiveController.instance.GetObjectivesDone();
                Destroy(lightobj, 2f);
                punch.enabled = false;
                weapon.SetActive(true);
                pickUpWeapon.SetActive(false);
            }
        }
    }
}
