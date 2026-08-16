using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [Header("相关参数")]
    // 第几关、角色从哪里开始游玩、摄像机观察位置、摄像机视野大小
    [SerializeField] private int levelNumber;
    [SerializeField] private Transform landerStartPositionTransform;
    [SerializeField] private Transform cameraStartTargetTransform;
    [SerializeField] private float zoomedOutOrthographicSize;

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public Vector3 GetLanderStartPosition ()
    {
        return landerStartPositionTransform.position;
    }

    public Transform GetCameraStartTargetTransform()
    {
        return cameraStartTargetTransform;
    }

    public float GetZoomedOutOrthographicSize()
    {
        return zoomedOutOrthographicSize;
    }
}
