using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraZoom2D : MonoBehaviour
{
    // 设置CinemachineCamera相关参数
    private const float NORMAL_ORTHOGRAPHIC_SIZE = 10f;

    public static CinemachineCameraZoom2D Instance {  get; private set; }

    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private float zoomSpeed = 2f;

    private float targetOrthographicSize = 10f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);
    }

    public void SetTargetOrthographicSize(float targetOrthographicSize)
    {
        this.targetOrthographicSize = targetOrthographicSize;
    }

    public void SetNormalOrthographicSize()
    {
        SetTargetOrthographicSize(NORMAL_ORTHOGRAPHIC_SIZE);
    }
}
