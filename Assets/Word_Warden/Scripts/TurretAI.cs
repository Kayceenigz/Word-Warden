using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [Header("Setup")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float range = 20f;
    public float baseFireRate = 1.0f; // Shots per second

    private float nextFireTime;

    void Update()
    {
        // Calculate speed based on Global Upgrades
        // Example: Base 1.0 + (Level * 0.5)
        float currentFireRate = baseFireRate + (GameManager.Instance.speedUpgradeLevel * 0.5f);

        if (Time.time >= nextFireTime)
        {
            GameObject target = FindClosestEnemy();
            if (target != null)
            {
                Shoot(target);
                nextFireTime = Time.time + (1f / currentFireRate);
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        // Shoots raycast to the right to find zombies
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, range);

        // Tag check: Make sure your Zombie prefab is tagged "Enemy"
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    void Shoot(GameObject target)
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // Global Damage: Base 10 + (Level * 5)
            float currentDamage = 10f + (GameManager.Instance.damageUpgradeLevel * 5f);
            bullet.GetComponent<Projectile>().Setup(currentDamage);
        }
    }
}