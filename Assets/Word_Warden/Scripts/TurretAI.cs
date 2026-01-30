using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [Header("Combat Stats")]
    public float fireRate = 1.0f;
    public float damage = 10f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private float nextFireTime;

    void Update()
    {
        // GDD: Auto-shooting threats
        if (Time.time >= nextFireTime)
        {
            GameObject target = FindClosestEnemy();
            if (target != null)
            {
                Shoot(target);
                nextFireTime = Time.time + (1f / fireRate);
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        // Raycast or OverlapBox to find zombies in a line
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 20f);
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    void Shoot(GameObject target)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Projectile>().Setup(damage);
    }
}