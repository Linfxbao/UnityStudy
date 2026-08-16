using UnityEngine;

public class LandingPad : MonoBehaviour
{
    // 着陆平台
    [SerializeField] private int scoreMultiplier;

    // 获得平台上分数倍率
    public int GetScoreMultiplier()
    {
        return scoreMultiplier;
    }
}
