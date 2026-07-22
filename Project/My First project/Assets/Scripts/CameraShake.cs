using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Camera mainCam;

    float shakeAmount = 0;
    private Vector3 originalLocalPosition = Vector3.zero;

    void Awake()
    {
        mainCam = Camera.main;
        if (mainCam != null)
        {
            originalLocalPosition = mainCam.transform.localPosition;
        }
    }

    public void Shake(float amt, float length)
    {
        if (!EnsureCamera())
        {
            return;
        }

        shakeAmount = amt;
        originalLocalPosition = mainCam.transform.localPosition;

        CancelInvoke(nameof(DoShake));
        CancelInvoke(nameof(StopShake));
        InvokeRepeating(nameof(DoShake), 0f, 0.02f);
        Invoke(nameof(StopShake), length);
    }

    void DoShake()
    {
        if (!EnsureCamera())
        {
            return;
        }

        if (shakeAmount > 0)
        {
            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;
            mainCam.transform.localPosition = originalLocalPosition + new Vector3(offsetX, offsetY, 0f);
        }

    }

    void StopShake()
    {
        CancelInvoke(nameof(DoShake));

        if (EnsureCamera())
        {
            mainCam.transform.localPosition = originalLocalPosition;
        }
    }

    bool EnsureCamera()
    {
        if (mainCam != null)
        {
            return true;
        }

        mainCam = Camera.main;
        if (mainCam != null)
        {
            originalLocalPosition = mainCam.transform.localPosition;
            return true;
        }

        return false;
    }

}
