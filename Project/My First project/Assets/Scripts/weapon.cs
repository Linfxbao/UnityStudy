using UnityEngine;
using UnityEngine.InputSystem;

public class weapon : MonoBehaviour
{
    public Transform MuzzleFlashPrefab;

    public float fireRate = 0;
    public int Damage = 10;
    public float shotDistance = 15f;
    public LayerMask whatToHit;
    public Transform BulletTrailPrefab;
    public Transform HitPrefab;

    public float camShakeAmt = 0.05f;
    public float camShakeLength = 0.1f;
    CameraShake camShake;

    public string weaponShootSound = "DefaultShot";
    AudioManager audioManager;

    float timeToSpawnEffect = 0;
    public float effectSpawnRate = 10;

    private float timeToFire = 0;
    private Transform firePoint;
    private Camera mainCam;


    void Awake()
    {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("There is no firePoint!");
        }
    }

    void Start()
    {
        mainCam = Camera.main;

        if (GameMaster.gm != null)
        {
            camShake = GameMaster.gm.GetComponent<CameraShake>();
        }

        if (camShake == null)
        {
            Debug.LogError("There is no camShake!!!!");
        }

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("Freak out! No audioManager found in scence!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (fireRate == 0)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Shoot();
            }
        } else
        {
            if (Mouse.current.leftButton.isPressed && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        if (firePoint == null || Mouse.current == null)
        {
            return;
        }

        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null)
            {
                return;
            }
        }

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 mousePosition = new(mouseWorld.x, mouseWorld.y);
        Vector2 firePointPosition = new(firePoint.position.x, firePoint.position.y);
        Vector2 shotDirection = mousePosition - firePointPosition;

        if (shotDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Vector2 shotDirectionNormalized = shotDirection.normalized;
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, shotDirectionNormalized, shotDistance, whatToHit); 
        
        Debug.DrawLine(firePointPosition, firePointPosition + shotDirectionNormalized * shotDistance, Color.cyan);
        if (hit.collider != null)
        {
            Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DamageEnemy(Damage);
                //Debug.Log("we hit" + hit.collider.name + "and did " + Damage + "damage");
            }
        }

        if (Time.time >= timeToSpawnEffect)
        {
            Vector3 hitPos;
            Vector3 hitNormal;

            if (hit.collider == null)
            {
                hitPos = firePoint.position + (Vector3)(shotDirectionNormalized * shotDistance);
                hitNormal = new(9999, 9999, 9999);
            }
            else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }

            hitPos.z = firePoint.position.z;

            Effect(hitPos, hitNormal);
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void Effect(Vector3 hitPos, Vector3 hitNormal)
    {
        Transform trail = (Transform)Instantiate(BulletTrailPrefab, firePoint.position, firePoint.rotation);
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        if (lr != null)
        {
            //SET POSITIONS
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);

        }

        Destroy(trail.gameObject, 0.04f);

        if (hitNormal != new Vector3(9999, 9999, 9999))
        {
            Transform hitParticle = Instantiate(HitPrefab, hitPos, Quaternion.FromToRotation(Vector3.right, hitNormal));
            Destroy(hitParticle.gameObject, 1f);
        }

        Transform clone = (Transform)Instantiate(MuzzleFlashPrefab, firePoint.position, firePoint.rotation);
        clone.parent = firePoint;
        float size = Random.Range(0.3f, 0.9f);
        clone.localScale = new Vector3(size, size, size);

        Destroy(clone.gameObject, 0.02f);

        if (camShake != null)
        {
            camShake.Shake(camShakeAmt, camShakeLength);
        }

        if (audioManager != null)
        {
            audioManager.PlaySound(weaponShootSound);
        }
    }
}
