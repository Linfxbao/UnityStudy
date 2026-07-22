



``` csharp

using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    private Vector3 offset;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("缺少跟随对象");
        }

        offset = transform.position - player.position;

    }

    void Update()
    {
        transform.position = player.position + offset;
    }
}


```


``` csharp

using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform Cam;

    void Start()
    {
        if (Cam == null)
        {
            Debug.LogError("缺少镜头对象");
        }
    }

    void Update()
    {
        transform.LookAt(Cam);
    }
}


```