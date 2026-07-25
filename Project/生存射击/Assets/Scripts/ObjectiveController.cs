using UnityEngine;
using UnityEngine.UI;

public class ObjectiveController : MonoBehaviour
{
    [Header("任务")]
    public Text objective1;
    public Text objective2;
    public Text objective3;
    public Text objective4;

    [HideInInspector]
    public bool obj1 = false;
    [HideInInspector]
    public bool obj2 = false;
    [HideInInspector]
    public bool obj3 = false;
    [HideInInspector]
    public bool obj4 = false;

    public static ObjectiveController instance;

    private void Awake()
    {
        instance = this;
    }

    public void GetObjectivesDone()
    {
        if (obj1)
        {
            objective1.color = Color.green;
        }

        if (obj2)
        {
            objective2.color = Color.green;
        }

        if (obj3)
        {
            objective3.color = Color.green;
        }

        if (obj4)
        {
            objective4.color = Color.green;
        }
    }
}
