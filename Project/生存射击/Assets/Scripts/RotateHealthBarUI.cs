using UnityEngine;

public class RotateHealthBarUI : MonoBehaviour
{
    public Transform MainCam;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + MainCam.forward);
    }
}
