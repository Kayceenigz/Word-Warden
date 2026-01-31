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
        float currentFireRate = baseFireRate + (GameManager.Instance.speedUpgradeLevel * 0.5f);

        if (Time.time >= nextFireTime)
        {
            GameObject target = FindTarget();
            if (target != null)
            {
                Shoot(target);
                nextFireTime = Time.time + (1f / currentFireRate);
            }
        }
    }

    GameObject FindTarget()
    {
        // 1. Find all enemies in range
        // We use CircleCast or OverlapCircle to find things near the turret
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.right, range);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                EnemyController enemy = hit.collider.GetComponent<EnemyController>();

                // CRITICAL: Only shoot if the zombie is UNMASKED
                // (In your new logic, unmasked zombies die instantly, 
                // but this keeps the logic safe if you add health later)
                if (enemy != null && !enemy.isMasked)
                {
                    return hit.collider.gameObject;
                }
            }
        }
        return null;
    }

    void Shoot(GameObject target)
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Point bullet toward the target
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // Calculate damage based on upgrades
            float currentDamage = 10f + (GameManager.Instance.damageUpgradeLevel * 5f);

            // Get the projectile script and set it up
            Projectile projectile = bullet.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Setup(currentDamage);
            }
        }
    }

    // Visual aid in the editor to see the turret's range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * range);
    }
}