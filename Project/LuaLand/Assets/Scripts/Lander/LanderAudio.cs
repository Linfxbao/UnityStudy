using UnityEngine;

public class LanderAudio : MonoBehaviour
{
    // 角色移动时音效
    [SerializeField] private AudioSource thrusterAudioSource;

    private Lander lander;

    private void Awake()
    {
        lander = GetComponent<Lander>();
    }

    private void Start()
    {
        lander.OnBeforeForce += Lander_OnBeforeForce;
        lander.OnUpForce += Lander_OnUpForce;
        lander.OnLeftForce += Lander_OnLeftForce;
        lander.OnRightForce += Lander_OnRightForce;

        SoundManager.Instance.OnSoundVolumeChanged += SoundManager_OnSoundVolumeChanged;
        
        // 防止切换关卡后音量被重置
        thrusterAudioSource.volume = SoundManager.Instance.GetSoundVolumeNormalized();

        thrusterAudioSource.Pause();
    }

    // 音效音量改变时调用
    private void SoundManager_OnSoundVolumeChanged(object sender, System.EventArgs e)
    {
        thrusterAudioSource.volume = SoundManager.Instance.GetSoundVolumeNormalized();
    }

    // 向左移动时，当前已经在播放的话就不重复播放
    private void Lander_OnLeftForce(object sender, System.EventArgs e)
    {
        if (!thrusterAudioSource.isPlaying)
        {
            thrusterAudioSource.Play();
        }
    }

    // 向右移动时
    private void Lander_OnRightForce(object sender, System.EventArgs e)
    {
        if (!thrusterAudioSource.isPlaying)
        {
            thrusterAudioSource.Play();
        }
    }

    // 向上移动时
    private void Lander_OnUpForce(object sender, System.EventArgs e)
    {
        if (!thrusterAudioSource.isPlaying)
        {
            thrusterAudioSource.Play();
        }
    }

    // 在移动前停止音效
    private void Lander_OnBeforeForce(object sender, System.EventArgs e)
    {
        thrusterAudioSource.Pause();
    }
}
