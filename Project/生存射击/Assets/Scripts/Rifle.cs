using System.Collections;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    public PlayerScript player;
    public Transform hand;

    [Header("射击物")]
    public Camera cam;

    public float giveDamageOf = 10f;
    public float shootingRange = 100f;

    public float fireCharge = 15f;
    private float nextTimeToShoot = 0f;

    [Header("动画器")]
    public Animator ani;
    public GameObject rifleUI;

    [Header("装弹")]
    public int mag = 10;
    public int maxAmmo = 32;
    public float reloadingTime = 1.3f;
    private bool setReloading = false;
    private int presentAmmo;


    [Header("开火")]
    public ParticleSystem muzzleSpark;
    public GameObject woodEffect;
    public GameObject goreEffect;

    public GameObject ammoUI;
    public AudioClip shootSound;
    public AudioClip ReloadSound;
    public AudioSource audioSource;

    private void Awake()
    {
        transform.SetParent(hand);
        presentAmmo = maxAmmo;
    }

    private void OnEnable()
    {
        rifleUI.SetActive(true);
    }

    private void Update()
    {
        if (setReloading)
        {
            return;
        }

        if (presentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToShoot)
        {
            ani.SetBool("Fire", true);
            ani.SetBool("Idle", false);

            nextTimeToShoot = Time.time + 1f / fireCharge;
            Shoot();
        }
        else if(Input.GetButton("Fire2") && Input.GetButton("Fire1")) 
        {
            ani.SetBool("Idle", false);
            ani.SetBool("IdleAim", true);
            ani.SetBool("FireWalk", true);
            ani.SetBool("Walk", true);
            ani.SetBool("Reloading", false);
        }
        else if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            ani.SetBool("Idle", false);
            ani.SetBool("FireWalk", true);
        }
        else
        {
            ani.SetBool("Fire", false);
            ani.SetBool("Idle", true);
            ani.SetBool("FireWalk", false);
        }
    }

    private void Shoot()
    {
        if (mag == 0)
        {
            StartCoroutine(Warning());
            return;
        }

        presentAmmo--;

        if (presentAmmo == 0)
        {
            mag--;
        }

        RifleUI.instance.UpdateAmmoText(presentAmmo);
        RifleUI.instance.UpdateMagText(mag);


        audioSource.PlayOneShot(shootSound);
        muzzleSpark.Play();
        RaycastHit hitInfo;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootingRange))
        {
            Debug.Log(hitInfo.transform.name);
            Health health = hitInfo.transform.GetComponent<Health>();
            Zombie1 zombie1 = hitInfo.transform.GetComponent<Zombie1>();
            Zombie2 zombie2 = hitInfo.transform.GetComponent<Zombie2>();
            if (health != null)
            {
                health.TakeDamage(giveDamageOf);

                GameObject woodGo = Instantiate(woodEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(woodGo, 1f);
            }
            else if (zombie1 != null)
            {
                zombie1.zombieHitDamage(giveDamageOf);
                GameObject goreGo = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(goreGo, 1f);
            }
            else if (zombie2 != null)
            {
                zombie2.zombieHitDamage(giveDamageOf);
                GameObject goreGo = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(goreGo, 1f);
            }
        }
    }

    IEnumerator Reload()
    {
        player.playerSpeed = 0f;
        player.playerSprint = 0f;
        setReloading = true;
        Debug.Log("Reloading...");
        ani.SetBool("Reloading", true);
        audioSource.PlayOneShot(ReloadSound);

        yield return new WaitForSeconds(reloadingTime);

        ani.SetBool("Reloading", false);
        presentAmmo = maxAmmo;
        player.playerSpeed = 2.5f;
        player.playerSprint = 5f;
        setReloading = false;

        RifleUI.instance.UpdateAmmoText(presentAmmo);
        RifleUI.instance.UpdateMagText(mag);
    }

    IEnumerator Warning()
    {
        ammoUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        ammoUI.SetActive(false);
    }
}
