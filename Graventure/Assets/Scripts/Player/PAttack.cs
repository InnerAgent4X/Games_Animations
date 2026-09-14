using UnityEngine;

public class PAttack : MonoBehaviour
{
    public float speed = 50f;
    public float distance = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= distance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PhysicalEnemy"))
        {
            // Call Enemy's TakeDamage method
            other.GetComponent<PhysicalEnemies>().DealDamage();
            Destroy(gameObject);
        }
        else if (other.CompareTag("MagicEnemy"))
        {
            Destroy(gameObject);
        }
    }
}
