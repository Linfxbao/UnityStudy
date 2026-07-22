using UnityEngine;

// 2D 相机平滑跟随：支持目标偏移与阻尼过渡，减少画面抖动。
public class Camera2DFollow : MonoBehaviour
{
    public Transform target; // 相机要跟随的目标对象
    public float smoothTime = 0.3f; // SmoothDamp 的平滑时间（越大越“黏”）
    public Vector3 offset; // 相机与目标的世界坐标偏移量
    public float verticalLimit = 1f; // 可选：相机垂直位置的最小值，避免过度上下抖动

    private Vector3 currentVelocity = Vector3.zero; // SmoothDamp 内部需要的速度缓存
    //private Vector3 lastTargetPosition; // 上一帧目标位置，用于计算实际移动距离（可选）
    private float nextTimeToSearch = 0f; // 下一次搜索目标的时间，避免频繁查找


    // 首帧前初始化：如果未手动设置 offset，则采用当前相机与目标的相对位置
    void Start()
    {
        if (offset == Vector3.zero && target != null)
        {
            // 这样可以在编辑器中直接摆好初始机位，运行后保持该构图
            offset = transform.position - target.position;
        }
    }

    // 放在 LateUpdate：让相机跟随在角色移动之后执行，降低抖动感
    void LateUpdate()
    {
        

        // 没有目标时不执行跟随
        if (target == null) 
        {
            FindPlayer();
            return;
        }

        // 目标机位 = 目标位置 + 固定偏移
        Vector3 targetPosition = target.position + offset;

        // 平滑过渡到目标机位，避免镜头瞬移
        Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        newPos = new(newPos.x, Mathf.Clamp(newPos.y, verticalLimit, Mathf.Infinity), newPos.z); // 可选：限制相机垂直范围，避免过度上下抖动

        transform.position = newPos;
    }

    void FindPlayer()
    {
        if (nextTimeToSearch <= Time.time)
        {
            GameObject searchResult = GameObject.FindGameObjectWithTag("Player");
            if (searchResult != null)
            {
                target = searchResult.transform;
            }
            nextTimeToSearch = Time.time + 0.5f;
        }
    }
}
