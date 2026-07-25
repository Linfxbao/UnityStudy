using UnityEngine;
using UnityEngine.SceneManagement;

public class Objective4 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            ObjectiveController.instance.obj4 = true;
            ObjectiveController.instance.GetObjectivesDone();

            SceneManager.LoadScene("MainScene");

            Destroy(gameObject, 2f);
        }
    }
}
