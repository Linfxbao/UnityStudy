using UnityEngine;

public class Punch : MonoBehaviour
{
    [Header("拳击")]
    public Camera cam;
    public float punchDamage = 10f;
    public float punchRange = 0.3f;
    
    
    public float punchCharge = 15f;
    private float nextTimeToPunch = 0f;
    public Animator ani;

    public void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToPunch)
        {
            nextTimeToPunch = Time.time + 1 / punchCharge;

            Punching();
            ani.SetBool("Punch", true);
            ani.SetBool("Idle", false);
        } else
        {
            ani.SetBool("Punch", false);
            ani.SetBool("Idle", true);
        }
    }

    private void Punching()
    {
        RaycastHit hitInfo;

        

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, punchRange))
        {
            Debug.Log("Punch: " + hitInfo.transform.name);
            Health health = hitInfo.transform.GetComponent<Health>();
            Zombie1 zombie1 = hitInfo.transform.GetComponent<Zombie1>();
            Zombie2 zombie2 = hitInfo.transform.GetComponent<Zombie2>();
            Debug.LogError("Punching");
            if (health != null)
            {
                health.TakeDamage(punchDamage);

                //GameObject hitObject = Instantiate(effect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                //Destroy(hitObject, 1f);
            }
            else if (zombie1 != null)
            {
                zombie1.zombieHitDamage(punchDamage);

                //GameObject hitObject = Instantiate(effect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                //Destroy(hitObject, 1f);
            }
            else if (zombie2 != null)
            {
                zombie2.zombieHitDamage(punchDamage);
                
            }
        }
    }
}
