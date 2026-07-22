using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    private int nextWave = 0;
    public int NextWave
    {
        get { return nextWave + 1; } 
    }

    public Transform[] spawnPoints;

    private SpawnState state = SpawnState.COUNTING;
    public SpawnState State
    {
        get { return state; }
    }

    [SerializeField]
    private float enemySearchInterval = 0.2f;

    private float searchCountDown;

    public float timeBetweenWave = 5f;
    private float waveCountdown;
    public float WaveCountDown
    {
        get { return Mathf.Max(0f, waveCountdown); }
    }

    void Start()
    {
        waveCountdown = timeBetweenWave;
        searchCountDown = Mathf.Max(0.05f, enemySearchInterval);

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No Spawn Point referenced!!!!");
        }

    }

    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            } else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        } else
        {
            waveCountdown = Mathf.Max(0f, waveCountdown - Time.deltaTime);
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed!");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWave;

        nextWave++;

        if (nextWave >= waves.Length)
        {
            nextWave = 0;
            Debug.Log("All waves completed!");
        }
    }

    bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;

        if (searchCountDown <= 0f)
        {
            searchCountDown = Mathf.Max(0.05f, enemySearchInterval);
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        state = SpawnState.SPAWNING;

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        state = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(Transform _enemy)
    {
        
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, _sp.rotation);
        
    }
}
