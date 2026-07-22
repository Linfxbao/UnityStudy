using UnityEngine;
using UnityEngine.InputSystem;

public class Weapoon : MonoBehaviour
{
    public GameObject firePoint;
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector3 mousePosition;
    private bool isAttacking = false;

    void Start() 
    {
        // 如果在 Inspector 里没有指定 firePoint，就尝试找一个名为 BulletTrail 的对象
        if (firePoint == null)
        {
            firePoint = GameObject.Find("BulletTrail");
        }

        if (firePoint != null)
        {
            rb = firePoint.GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !isAttacking)
        {
            mousePosition = Mouse.current.position.ReadValue();
            mousePosition.z = transform.position.z - Camera.main.transform.position.z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
            Attack(worldPos);
            isAttacking = true;
        }
    }

    private void Attack(Vector3 targetPos) 
    {
        if (firePoint == null) {
            return;
        }

        Vector3 direction = (targetPos - transform.position).normalized;
        
        // 生成新子弹，作为 firePoint 的子对象
        GameObject newBullet = Instantiate(firePoint, transform.position, Quaternion.identity, firePoint.transform);
        
        Rigidbody2D rbNew = newBullet.GetComponent<Rigidbody2D>();
        if (rbNew != null)
        {
            rbNew.linearVelocity = direction * speed;
        }
        else
        {
            // 如果没有 Rigidbody2D，则手动移动一次，以便能看到方向
            newBullet.transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        isAttacking = false;
    }
}
