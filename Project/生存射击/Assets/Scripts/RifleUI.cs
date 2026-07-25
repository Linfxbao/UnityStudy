using UnityEngine;
using UnityEngine.UI;

public class RifleUI : MonoBehaviour
{
    public Text ammoText;
    public Text magText;
    public static RifleUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateAmmoText(int presentAmmo)
    {
        ammoText.text = "Ammo: " + presentAmmo.ToString();
    }

    public void UpdateMagText(int presentMag)
    {
        magText.text = "Magzines: " + presentMag.ToString();
    }
}
