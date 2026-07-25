using System.Collections;
using UnityEngine;

public class ZombieSpawn : MonoBehaviour
{
    [Header("")]
    public GameObject zombiePrefab;
    public Transform zombieSpawnPos;
    private float repeatCycle = 1f;

    public GameObject warningUI;

    public AudioClip damageZoneSound;
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InvokeRepeating(nameof(EnemySpawner), 1f, repeatCycle);
            audioSource.PlayOneShot(damageZoneSound);
            StartCoroutine(Warning());
            Destroy(gameObject, 10f);
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void EnemySpawner()
    {
        GameObject m_obj = Instantiate(zombiePrefab, zombieSpawnPos.position, zombieSpawnPos.rotation);
        m_obj.SetActive(true);
    }

    IEnumerator Warning()
    {
        warningUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        warningUI.SetActive(false);
    }
}
