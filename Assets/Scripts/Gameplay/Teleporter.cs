using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Tooltip("Khoảng cách dịch chuyển khi teleport đi sang hướng ngược lại.")]
    public float teleportDistance = 4f;

    [Tooltip("Khoảng cách an toàn để tránh bị kích hoạt lại ngay sau khi teleport.")]
    public float safeMargin = 0.5f;

    [Tooltip("Thời gian chờ trước khi cho phép cùng một Rigidbody teleport lại.")]
    public float cooldown = 0.2f;

    private HashSet<Rigidbody2D> teleportedBodies = new HashSet<Rigidbody2D>();

    private void Awake()
    {
        // Chuyển tất cả collider thành trigger để chỉ dùng sự kiện chạm mà không cản vật lý.
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null)
            return;

        if (teleportedBodies.Contains(rb))
            return;

        if (other.GetComponent<Player>() == null && other.GetComponent<NPCWander>() == null)
            return;

        Vector2 localDirection = transform.InverseTransformPoint(other.transform.position);
        if (localDirection == Vector2.zero)
            localDirection = Vector2.up;

        if (Mathf.Abs(localDirection.x) > Mathf.Abs(localDirection.y))
            localDirection = new Vector2(Mathf.Sign(localDirection.x), 0f);
        else
            localDirection = new Vector2(0f, Mathf.Sign(localDirection.y));

        Vector2 targetPosition = (Vector2)transform.position - localDirection * (teleportDistance + safeMargin);
        Vector3 resultPosition = new Vector3(targetPosition.x, targetPosition.y, other.transform.position.z);

        rb.position = targetPosition;
        rb.velocity = Vector2.zero;
        other.transform.position = resultPosition;

        teleportedBodies.Add(rb);
        StartCoroutine(RemoveFromCooldown(rb));
    }

    private IEnumerator RemoveFromCooldown(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(cooldown);
        teleportedBodies.Remove(rb);
    }
}
