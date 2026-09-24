using UnityEngine;

public class MAttack : MonoBehaviour
{
    public float speed = 50f;
    public float distance = 2f;

    public DamageType damageType = DamageType.Magical;

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
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageType);
                Destroy(gameObject);
            }
            Destroy(gameObject);
        }
    }
}
