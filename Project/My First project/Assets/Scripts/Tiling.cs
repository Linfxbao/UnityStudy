using UnityEngine;

// 水平无限平铺：当相机接近边缘时，自动在左右生成相邻背景块。
public class Tiling : MonoBehaviour
{
    public int offsetX = 2; // 提前生成阈值，避免相机到边缘才补图而露底
    public bool reverseScale = false; // 新块是否镜像（常用于减少重复感）

    // 当前块左右是否已有相邻块，防止重复生成
    private bool hasARightBuddy = false;
    private bool hasALeftBuddy = false;

    private float spriteWidth = 0f; // 当前精灵在世界坐标下的宽度
    private Camera cam; // 主相机
    private Transform myTransform; // 当前物体缓存
    private Parallaxing parallaxing; // 视差系统，用于注册动态新块

    void Awake()
    {
        cam = Camera.main;
        myTransform = transform;
        // 场景中只有一个 Parallaxing 时可直接查找
        parallaxing = FindFirstObjectByType<Parallaxing>();
    }

    // 首帧前初始化精灵宽度
    void Start()
    {
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        if (sRenderer == null)
        {
            // 没有 SpriteRenderer 就无法计算平铺间距
            Debug.LogError("Tiling requires a SpriteRenderer component.", this);
            enabled = false;
            return;
        }

        // 使用世界空间宽度（包含缩放），避免拼接出现大间隙
        spriteWidth = sRenderer.bounds.size.x;
    }

    // 每帧检查相机是否接近左右边缘，按需补充 buddy
    void Update()
    {
        if (!hasALeftBuddy || !hasARightBuddy)
        {
            // 正交相机可见半宽 = orthographicSize * 宽高比
            float camHorizontalExtend = cam.orthographicSize * Screen.width / Screen.height;

            // 计算“当前块可见区域边界”在世界坐标中的位置
            float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth / 2) - camHorizontalExtend;
            float edgeVisiblePositionLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtend;

            // 相机靠近右边缘，补右块
            if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && !hasARightBuddy)
            {
                MakeNewBuddy(1);
                hasARightBuddy = true;
            }
            // 相机靠近左边缘，补左块
            else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && !hasALeftBuddy)
            {
                MakeNewBuddy(-1);
                hasALeftBuddy = true;
            }
        }
    }

    // rightOrLeft: 1 表示右侧，-1 表示左侧
    private void MakeNewBuddy(int rightOrLeft)
    {
        // 按当前块宽度做整块偏移，保证无缝拼接
        Vector3 newPosition = new(myTransform.position.x + spriteWidth * rightOrLeft, myTransform.position.y, myTransform.position.z);

        //Transform newBuddy = Instantiate(myTransform, newPosition, myTransform.rotation) as Transform;

        Transform newBuddy = (Transform)Instantiate(myTransform, newPosition, myTransform.rotation);

        if (reverseScale)
        {
            // 仅翻转 X 轴，实现左右镜像
            newBuddy.localScale = new(newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
        }

        // 与当前块放到同一父节点下，保持层级整洁
        newBuddy.parent = myTransform.parent;

        if (parallaxing != null)
        {
            // 新生成块加入视差系统，避免“原块在动、新块不动”导致缝隙
            parallaxing.RegisterBackground(newBuddy);
        }

        if (rightOrLeft > 0)
        {
            newBuddy.GetComponent<Tiling>().hasALeftBuddy = true;
        }
        else
        {
            newBuddy.GetComponent<Tiling>().hasARightBuddy = true;
        }
    }
}
