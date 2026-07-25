using UnityEngine;

public class FootStepSound : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("音源")]
    public AudioClip[] footStepSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private AudioClip GetRandomFootStep()
    {
        return footStepSound[Random.Range(0, footStepSound.Length)];
    }

    private void Step()
    {
        AudioClip clip = GetRandomFootStep();
        audioSource.PlayOneShot(clip);
    }
}
