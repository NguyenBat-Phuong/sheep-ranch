using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NPCWander : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float wanderRadius = 3f;
    public float changeDirectionTime = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveDirection;
    private Vector2 startPosition;
    private float timer;

    public Vector2 MoveDirection => moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        startPosition = rb.position;
        PickNewDirection();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            PickNewDirection();
    }

    void FixedUpdate()
    {
        if (moveDirection == Vector2.zero) return;

        Vector2 nextPos = rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime);

        // Quay đầu nếu đi ra ngoài vùng wanderRadius
        if (Vector2.Distance(nextPos, startPosition) > wanderRadius)
            PickNewDirection();
        else
            rb.MovePosition(nextPos);
    }

    void PickNewDirection()
    {
        // 30% xác suất đứng yên (cừu nghỉ ngơi)
        if (Random.value < 0.3f)
            moveDirection = Vector2.zero;
        else
            moveDirection = Random.insideUnitCircle.normalized;

        timer = changeDirectionTime + Random.Range(0f, 1.5f);
    }
}
