using UnityEngine;

public class HealthBoost : MonoBehaviour
{
    public PlayerScript player;

    public float addHealth = 50f;
    public float radius = 2.5f;

    public AudioSource audioSource;
    public AudioClip healthBoostSound;

    public Animator ani;

    public GameObject pickUPTip;

    private void Update ()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            pickUPTip.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                ani.SetBool("Open", true);
                player.presentHealth = player.presentHealth + addHealth > player.health ? player.health : player.presentHealth + addHealth;
                player.healthBar.SetHealth(player.presentHealth);
                audioSource.PlayOneShot(healthBoostSound);

                pickUPTip.SetActive(false);
                Destroy(gameObject, 2f);
            }
        }
        else
        {
            pickUPTip.SetActive(false);
        }
    }
}
