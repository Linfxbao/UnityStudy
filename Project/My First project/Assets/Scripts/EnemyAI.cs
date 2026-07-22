using UnityEngine;
using Pathfinding;


[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public Transform target;

    public float updateRate = 2f;

    private Seeker seeker;
    private Rigidbody2D rb;

    public Path path;
    public float speed = 300f;
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathIsEnded = false;

    public float nextWayPointDistance = 3f;

    private int currentWaypoint = 0;
    private bool warnedStaticBody = false;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        nextWayPointDistance = Mathf.Clamp(nextWayPointDistance, 0.05f, 0.5f);

        if (!TryResolveTarget())
        {
            return;
        }

        InvokeRepeating(nameof(RequestPath), 0f, 1f / Mathf.Max(updateRate, 0.1f));
    }

    void OnDisable()
    {
        CancelInvoke(nameof(RequestPath));
        StopMoving();
    }

    bool TryResolveTarget()
    {
        if (target != null)
        {
            return true;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("EnemyAI: target is null and no GameObject tagged 'Player' was found.");
            return false;
        }

        target = playerObj.transform;
        return true;
    }

    void RequestPath()
    {
        if (!TryResolveTarget())
        {
            return;
        }

        if (!seeker.IsDone())
        {
            return;
        }

        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        if (p.error || p.vectorPath == null || p.vectorPath.Count == 0)
        {
            return;
        }

        path = p;
        currentWaypoint = 0;
        pathIsEnded = false;
    }

    void FixedUpdate()
    {
        if (target == null && !TryResolveTarget())
        {
            StopMoving();
            return;
        }

        if (path == null || path.vectorPath == null || path.vectorPath.Count == 0)
        {
            StopMoving();
            return;
        }

        while (currentWaypoint < path.vectorPath.Count &&
               Vector2.Distance(rb.position, (Vector2)path.vectorPath[currentWaypoint]) <= nextWayPointDistance)
        {
            currentWaypoint++;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            pathIsEnded = true;
            StopMoving();
            return;
        }

        pathIsEnded = false;

        if (rb.bodyType == RigidbodyType2D.Static)
        {
            if (!warnedStaticBody)
            {
                Debug.LogWarning("EnemyAI: Rigidbody2D is Static. Change it to Dynamic or Kinematic so the enemy can move.");
                warnedStaticBody = true;
            }
            return;
        }

        warnedStaticBody = false;

        Vector2 nextWaypoint = (Vector2)path.vectorPath[currentWaypoint];
        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            nextWaypoint,
            GetMovementSpeed() * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);
    }

    float GetMovementSpeed()
    {
        return speed > 50f ? speed / 50f : speed;
    }

    void StopMoving()
    {
        if (rb == null || rb.bodyType != RigidbodyType2D.Dynamic)
        {
            return;
        }

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
}

