using UnityEngine;
using TMPro;

public class LandingPadVisual : MonoBehaviour
{
    // 着陆平台倍率UI显示
    [SerializeField] private TextMeshPro scoreMultiplierTextMesh;

    private void Awake()
    {
        LandingPad landingPad = GetComponent<LandingPad>();
        scoreMultiplierTextMesh.text = "x" + landingPad.GetScoreMultiplier();
    }
}
