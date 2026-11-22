using System.Collections;
using UnityEngine;

public class ReactiveSwords : MonoBehaviour
{
    Transform target;
    PlayerMovement playerMovement;
    Coroutine fireRoutine;

    [SerializeField] int damage;
    [SerializeField] float speed = 10f;            // average speed over the whole movement
    [SerializeField] float fireDelay = 0.5f;
    [SerializeField] float overshootDistance = 3f; // how far past the player's position to travel

    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 720f;   // degrees per second while tracking the player
    [SerializeField] float rotationOffset = 0f;    // degrees; 0 = sprite points right, -90 = sprite points up

    // Ease-in curve: starts slow, accelerates toward the end.
    [SerializeField]
    AnimationCurve easeCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 0f),
        new Keyframe(1f, 1f, 2f, 2f));

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerMovement.dashAction += Fire;
    }

    private void OnDestroy()
    {
        if (playerMovement != null)
            playerMovement.dashAction -= Fire;
    }

    public void Fire()
    {
        if (fireRoutine != null) return; // already firing

        target = GameObject.FindGameObjectWithTag("Player").transform;
        fireRoutine = StartCoroutine(FireCoroutine());
    }

    IEnumerator FireCoroutine()
    {
        // Wind-up: turn toward the player each frame.
        float timer = 0f;
        while (timer < fireDelay)
        {
            timer += Time.deltaTime;
            RotateTowardTarget(rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // Lock on: face the target exactly before launching.
        RotateTowardTarget(360f);

        Vector3 start = transform.position;
        Vector2 direction = (target.position - start).normalized;

        Vector3 end = target.position + (Vector3)(direction * overshootDistance);
        end.z = start.z;

        float distance = Vector3.Distance(start, end);
        float duration = speed > 0f ? distance / speed : 0f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.LerpUnclamped(start, end, easeCurve.Evaluate(t));
            yield return null;
        }

        transform.position = end;
        fireRoutine = null;
    }

    void RotateTowardTarget(float maxDegrees)
    {
        Vector2 dir = target.position - transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + rotationOffset;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegrees);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
        }
    }
}