using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed; // Speed of the cannon shell
    public float damage; // Damage dealt by cannon shell
    public float range; // Max range of the projectile (we'll use this in some way for destruction)
    public float fireRate; // Time in seconds between each shot (cooldown)
    public float area;
    private Vector3 spawnPosition;
    private static Quaternion shellRotation = Quaternion.Euler(-90f, 0f, 0f);

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        // Debug.Log(area);
    }

    // Update is called once per frame
    void Update()
    {
        // float distanceTravelled = Vector3.Distance(spawnPosition, transform.position);
        if (rb.velocity != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(-rb.velocity) * shellRotation;
        }

        
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (area != 0)
        {
            // Debug.Log("Area is not zero");
            // Find all colliders in range of the Tower's AoE
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, area);
            foreach (Collider nearbyObject in hitColliders)
            {
                // Check if the nearby object has an "Enemy" tag and a component for damage handling
                Enemy enemy = nearbyObject.GetComponent<Enemy>();
                // Debug.Log("Found enemy: " + enemy.name);
                if (enemy != null)
                {
                    Debug.Log("Did area damage");
                    enemy.TakeDamage(damage);
                }
            }
            Destroy(gameObject);
        }
        // Calculate distance traveled from spawn position to collision point
        float distanceTravelled = Vector3.Distance(spawnPosition, transform.position);

        // Check if the collision is with the ground or other surface
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Projectile hit the ground at a distance of: " + distanceTravelled.ToString("F2") + " units from spawn");
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
            // Debug.Log("Did damage to enemy");
        }
    }
}
