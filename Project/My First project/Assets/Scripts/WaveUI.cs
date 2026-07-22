using UnityEngine;
using UnityEngine.UI;
public class WaveUI : MonoBehaviour
{
    [SerializeField]
    WaveSpawner spawner;

    [SerializeField]
    Animator waveAnimator;

    [SerializeField]
    Text waveCountDownText;

    [SerializeField]
    Text waveCountText;

    private WaveSpawner.SpawnState previousState;

    void Start()
    {
        if (spawner == null)
        {
            Debug.LogError("There is no spawner in WaveUI!!!");
            this.enabled = false;
        }
        if (waveAnimator == null)
        {
            Debug.LogError("There is no waveAnimator in WaveUI!!!");
            this.enabled = false;
        }
        if (waveCountDownText == null)
        {
            Debug.LogError("There is no waveCountDown in WaveUI!!!");
            this.enabled = false;
        }
        if (waveCountText == null)
        {
            Debug.LogError("There is no waveCountText in WaveUI!!!");
            this.enabled = false;
        }
    }

    void Update()
    {
        switch(spawner.State)
        {
            case WaveSpawner.SpawnState.COUNTING:
                UpdateCountDownUI();
                break;
            case WaveSpawner.SpawnState.SPAWNING:
                UpdateSpawningUI();
                break;
        }

        previousState = spawner.State;
    }

    void UpdateCountDownUI()
    {
        if (previousState != WaveSpawner.SpawnState.COUNTING)
        {
            waveAnimator.SetBool("WaveInComing", false);
            waveAnimator.SetBool("WaveCountDown", true);
            //Debug.Log("Counting");
        }
        waveCountDownText.text = Mathf.CeilToInt(spawner.WaveCountDown).ToString();
    }
    void UpdateSpawningUI()
    {
        if (previousState != WaveSpawner.SpawnState.SPAWNING)
        {
            waveAnimator.SetBool("WaveCountDown", false);
            waveAnimator.SetBool("WaveInComing", true);
            waveCountText.text = spawner.NextWave.ToString();
            Debug.Log("Spawning");
        }
        
    }
}
