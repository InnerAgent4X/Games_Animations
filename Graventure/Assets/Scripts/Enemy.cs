using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    // Path endpoints (a straight track between start and end)
    Vector3 pathStart;
    Vector3 pathEnd;

    public bool instantOnBeat = true;

    // Fields used for smooth interpolation between beats
    Vector3 beatFrom;
    Vector3 beatTo;
    int assignedBeat = -1;

    [Header("Step movement along the track")]
    public float stepDistance = 0.5f;     // how far each incremental step goes
    public float endReachThreshold = 0.05f; // when the end point is considered reached

    // Initialize with absolute start and end positions (call after Instantiate)
    public void InitializePath(Vector3 start, Vector3 end)
    {
        pathStart = start;
        pathEnd = end;

        transform.position = pathStart;
    }

    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBeat += UseBeat;
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBeat -= UseBeat;
        }
    }

    void UseBeat(int beatCount)
    {
        // Safety: if reached end or game finished, ignore
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isYouWin)
        {
            Destroy(this.gameObject);
            return;
        }

        // beat-driven movement is the only mode now

        // remaining distance to end along the segment (reuse same math as coroutine)
        Vector3 closest = ClosestPointOnSegment(pathStart, pathEnd, transform.position);
        float remaining = Vector3.Distance(closest, pathEnd);
        if (remaining <= endReachThreshold)
        {
            transform.position = pathEnd;
            OnReachedEnd();
            return;
        }

        // compute next target along the segment
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

        if (instantOnBeat)
        {
            // immediate step on beat
            transform.position = target;
        }
        else
        {
            // prepare interpolation for Update-driven smooth movement
            beatFrom = transform.position;
            beatTo = target;
            assignedBeat = beatCount;
        }
    }

    void Update()
    {
        // Smoothly interpolate between beatFrom and beatTo using GameManager's beat progress
        if (!instantOnBeat && assignedBeat >= 0 && GameManager.Instance != null)
        {
            // Use GameManager's beat progress (0..1) for smooth interpolation between beats
            float p = GameManager.Instance.GetBeatProgress();
            transform.position = Vector3.Lerp(beatFrom, beatTo, p);

            if (p >= 1f - 1e-4f)
            {
                assignedBeat = -1;
            }
        }
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


    public DamageType[] requiredSequence;

    private int currentIndex = 0;

    public void TakeDamage(DamageType attackType)
    {
        if (attackType != requiredSequence[currentIndex])
        {
            // Wrong attack type, reset sequence
            currentIndex = 0;
            GameManager.Instance.AttackPenalty();
            return;
        }

        currentIndex++;

        if (currentIndex >= requiredSequence.Length)
        {
            // Sequence completed, destroy enemy
            GameManager.Instance.Score += 1;
            Destroy(gameObject);
        }
    }
}
