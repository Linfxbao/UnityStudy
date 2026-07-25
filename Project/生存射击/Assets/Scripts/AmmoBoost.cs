using UnityEngine;

public class AmmoBoost : MonoBehaviour
{
    public Rifle rifle;

    public int addAmmo = 5;
    public float radius = 2.5f;

    public AudioSource audioSource;
    public AudioClip ammoBoostSound;

    public Animator ani;

    public GameObject pickUPTip;

    private void Update()
    {
        if (Vector3.Distance(transform.position, rifle.transform.position) < radius)
        {
            pickUPTip.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                ani.SetBool("Open", true);
                rifle.mag += addAmmo;
                RifleUI.instance.UpdateMagText(rifle.mag);

                audioSource.PlayOneShot(ammoBoostSound);

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
