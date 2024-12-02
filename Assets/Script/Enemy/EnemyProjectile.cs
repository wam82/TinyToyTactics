using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    
    public float fireRate;
    public float projectileSpeed;
    public float damage;

    public float area;

    private Rigidbody rb;
    
    private static Quaternion shellRotation = Quaternion.Euler(-90f, 0f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(transform.position);
        if (rb.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(-rb.velocity) * shellRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collision is with the ground or other surface
        if (other.gameObject.CompareTag("Ground"))
        {
            if (area != 0)
            {
                // Find all colliders in range of the Tower's AoE
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, area);
                foreach (Collider nearbyObject in hitColliders)
                {
                    // Check if the nearby object has an "Player" tag and a component for damage handling
                    Player player = nearbyObject.GetComponent<Player>();
                    if (player != null)
                    {
                        player.TakeDamage(damage);
                    }
        
                    Enemy enemy = nearbyObject.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                }
            }
            Debug.Log("Projectile hit the ground");
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            if (area != 0)
            {
                // Find all colliders in range of the Tower's AoE
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, area);
                foreach (Collider nearbyObject in hitColliders)
                {
                    // Check if the nearby object has an "Player" tag and a component for damage handling
                    Player player = nearbyObject.GetComponent<Player>();
                    if (player != null)
                    {
                        player.TakeDamage(damage);
                    }
        
                    Enemy enemy = nearbyObject.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                }
            }
            Destroy(gameObject);
            Debug.Log("Did damage to enemy");
        }
        
        if (other.gameObject.CompareTag("Player"))
        {
            if (area != 0)
            {
                // Find all colliders in range of the Tower's AoE
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, area);
                foreach (Collider nearbyObject in hitColliders)
                {
                    // Check if the nearby object has an "Player" tag and a component for damage handling
                    Player player = nearbyObject.GetComponent<Player>();
                    if (player != null)
                    {
                        player.TakeDamage(damage);
                    }
        
                    Enemy enemy = nearbyObject.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                }
            }
            Destroy(gameObject);
            Debug.Log("Did damage to player");
        }
    }
}
