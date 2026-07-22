using System.Collections.Generic;
using UnityEngine;

// 多层背景视差滚动：支持运行时动态注册新背景（配合 Tiling 生成的 buddy）。
public class Parallaxing : MonoBehaviour
{
    public Transform[] backgrounds; // 初始参与视差的背景列表（可在 Inspector 配置）
    public float smoothing = 1f; // 插值平滑系数

    private readonly List<Transform> runtimeBackgrounds = new(); // 运行时实际参与视差的背景（含动态新增）
    private readonly List<float> parallaxScales = new(); // 每层背景对应的视差系数
    private Transform cam; // 主相机 Transform
    private Vector3 previousCamPos; // 上一帧相机位置，用于计算位移增量

    void Awake()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            // 没有主相机会导致视差系统无法计算位移，直接禁用脚本
            Debug.LogError("Parallaxing requires a Main Camera in the scene.", this);
            enabled = false;
            return;
        }

        cam = mainCam.transform;
    }

    // 首帧前初始化：把 Inspector 中配置的背景注册到运行时列表
    void Start()
    {
        previousCamPos = cam.position;

        runtimeBackgrounds.Clear();
        parallaxScales.Clear();
        for (int i = 0; i < backgrounds.Length; i++)
        {
            RegisterBackground(backgrounds[i]);
        }
    }

    // 供外部（如 Tiling）在运行时注册新背景，避免新块不参与视差导致缝隙
    public void RegisterBackground(Transform background)
    {
        // 空对象或重复注册直接忽略
        if (background == null || runtimeBackgrounds.Contains(background))
        {
            return;
        }

        // 这里沿用 z 值映射视差强度的规则：z 越远，运动越慢
        runtimeBackgrounds.Add(background);
        parallaxScales.Add(background.position.z * -1f);
    }

    // 每帧根据“相机横向位移增量”推动各背景层
    void Update()
    {
        float camDeltaX = previousCamPos.x - cam.position.x;

        // 逆序遍历，便于在遍历过程中安全移除失效对象
        for (int i = runtimeBackgrounds.Count - 1; i >= 0; i--)
        {
            Transform background = runtimeBackgrounds[i];
            if (background == null)
            {
                // 背景被销毁时，同步移除对应系数
                runtimeBackgrounds.RemoveAt(i);
                parallaxScales.RemoveAt(i);
                continue;
            }

            float parallax = camDeltaX * parallaxScales[i];

            float backgroundTargetPosX = background.position.x + parallax;

            Vector3 backgroundTargetPos = new(backgroundTargetPosX, background.position.y, background.position.z);

            // 使用插值平滑移动，避免层间突兀抖动
            background.position = Vector3.Lerp(background.position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        // 记录相机当前位置，供下一帧计算增量
        previousCamPos = cam.position;
    }
}
