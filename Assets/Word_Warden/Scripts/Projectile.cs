using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private float damage;

    public void Setup(float dmg) => damage = dmg;

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        // Destroy if off screen
        if (transform.position.x > 15f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().TakeDamage(damage);
            Destroy(gameObject); // Bullet impact
        }
    }
}