using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private float damage;

    public void Setup(float dmg)
    {
        damage = dmg;
        // Self-destruct after 5 seconds to be safe
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Move right towards the incoming zombies
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Only interact with things tagged "Enemy"
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();

            // 2. STRICT CHECK: Only hit the zombie if it is UNMASKED
            // This prevents bullets from killing zombies before the player types the word.
            if (enemy != null && !enemy.isMasked)
            {
                enemy.TakeDamage(damage);

                // Optional: Instantiate a small hit effect/particle here

                // Destroy the bullet on impact
                Destroy(gameObject);
            }
        }
    }
}