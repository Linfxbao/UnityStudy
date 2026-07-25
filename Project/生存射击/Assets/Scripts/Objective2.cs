using UnityEngine;

public class Objective2 : MonoBehaviour
{
    public GameObject Object4;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectiveController.instance.obj2 = true;
            ObjectiveController.instance.GetObjectivesDone();

            Object4.SetActive(true);
            Destroy(gameObject, 2f);
        }
    }
}
