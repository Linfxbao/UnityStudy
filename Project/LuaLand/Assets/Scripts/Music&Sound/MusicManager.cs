using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance {  get; private set; }

    // 游戏音乐最大值
    private const int MUSIC_VOLUME_MAX = 10;
    // 当前游戏音乐时间，确保切换场景时不从头开始
    private static float musicTime;
    // 游戏音乐声音大小
    private static int musicVolume = 4;

    private AudioSource musicAudioSource;

    public event EventHandler OnMusicVolumeChanged;

    private void Awake()
    {
        Instance = this;
        musicAudioSource = GetComponent<AudioSource>();

        musicAudioSource.time = musicTime;
    }

    private void Start()
    {
        musicAudioSource.volume = GetMusicVolumeNormalized();
    }

    private void Update()
    {
        musicTime = musicAudioSource.time;
    }

    // 改变游戏音乐音量
    public void ChangeMusicVolume()
    {
        musicVolume = (musicVolume + 1) % MUSIC_VOLUME_MAX;
        musicAudioSource.volume = GetMusicVolumeNormalized();
        OnMusicVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    // 获取游戏音乐音量
    public int GetMusicVolume()
    {
        return musicVolume;
    }

    // 将音量归一化
    public float GetMusicVolumeNormalized()
    {
        return ((float)musicVolume) / MUSIC_VOLUME_MAX;
    }
}
