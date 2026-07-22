using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIndicator : MonoBehaviour
{
    [SerializeField]
    private RectTransform healthBarRect;
    [SerializeField]
    private TextMeshProUGUI healthText;

    void Start()
    {
        if (healthBarRect == null)
        {
            Debug.LogError("Health bar RectTransform is not assigned.");
        }
        if (healthText == null)
        {
            Debug.LogError("Health text is not assigned.");
        }
    }

    public void SetHealth(int _cur, int _max)
    {
        float _value = (float)_cur / _max;

        healthBarRect.localScale = new(_value, healthBarRect.localScale.y, healthBarRect.localScale.z);

        healthText.text = _cur + "/" + _max + " HP";
    }

}
