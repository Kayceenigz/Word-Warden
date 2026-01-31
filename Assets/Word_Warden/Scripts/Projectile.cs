using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private float damage;

    public void Setup(float dmg)
    {
        damage = dmg;
        // Self-destruct after 3 seconds so we don't leak memory
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Move right
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Deal damage to the zombie
            collision.GetComponent<EntityBase>()?.TakeDamage(damage);

            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}