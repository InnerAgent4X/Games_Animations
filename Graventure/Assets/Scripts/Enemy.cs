using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    // Path endpoints (a straight track between start and end)
    Vector3 pathStart;
    Vector3 pathEnd;

    [SerializeField]
    protected int health;

    [Header("Step movement along the track")]
    public float stepDistance = 1.0f;     // how far each incremental step goes
    public float stepDuration = 0.0f;    // how long the movement interpolation takes
    public float stepDelay;
    public float endReachThreshold = 0.05f; // when the end point is considered reached

    bool moving = false;

    // Initialize with absolute start and end positions (call after Instantiate)
    public void InitializePath(Vector3 start, Vector3 end)
    {
        pathStart = start;
        pathEnd = end;

        transform.position = pathStart;

        if (!moving)
        {
            moving = true;
            StartCoroutine(MoveAlongSegment());
        }
    }

    private void Start()
    {
        stepDelay = GameManager.Instance.levelSettings.EnemySpawnRate;
    }

    IEnumerator MoveAlongSegment()
    {
        // Safety: if start and end are the same, finish immediately
        if (Vector3.Distance(pathStart, pathEnd) <= endReachThreshold)
        {
            OnReachedEnd();
            yield break;
        }

        while (true)
        {
            if (GameManager.Instance.isYouWin)
            {
                Destroy(this.gameObject);
            }

            // remaining distance to end along the segment
            Vector3 closest = ClosestPointOnSegment(pathStart, pathEnd, transform.position);
            float remaining = Vector3.Distance(closest, pathEnd);
            if (remaining <= endReachThreshold)
            {
                transform.position = pathEnd;
                break;
            }

            // direction along the segment from current (closest) point toward end
            Vector3 dir = (pathEnd - closest).normalized;
            float moveDist = Mathf.Min(stepDistance, remaining);
            Vector3 target = transform.position + dir * moveDist;

            // Project target back onto the segment to ensure we stay constrained
            target = ClosestPointOnSegment(pathStart, pathEnd, target);

            // Face movement direction
            Vector3 faceDir = (target - transform.position);
            if (faceDir.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(faceDir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, 0.25f);
            }

            // Smooth interpolate
            float t = 0f;
            Vector3 from = transform.position;
            float duration = Mathf.Max(0.0001f, stepDuration);
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(from, target, t);
                yield return null;
            }

            // tiny pause between steps
            if (stepDelay > 0f) yield return new WaitForSeconds(stepDelay);
        }

        OnReachedEnd();
    }

    static Vector3 ClosestPointOnSegment(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 ab = b - a;
        float abLen2 = Vector3.Dot(ab, ab);
        if (abLen2 == 0f) return a;
        float t = Vector3.Dot(p - a, ab) / abLen2;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }

    void OnReachedEnd()
    {
        // Default: destroy enemy. Replace with damage to player or pooling return.
        if (GameManager.Instance.isGameOver == false) GameManager.Instance.HealthyBoy -= 1;
        Destroy(gameObject);
    }

    public void DealDamage()
    {
        health--;
        if (health <= 0)
        {
            GameManager.Instance.Score += 1;
            Destroy(gameObject);
        }
    }
}
