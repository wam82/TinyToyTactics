using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    public GameObject tankProjectilePrefab;  // Cannon bomb projectile prefab
    public Transform cannon;              // Point from which the tank fires
    public Transform turret;
    public float detectionRadius;        // Detection range for the player
    // public float fireCooldown;            // Cooldown period between shots
    // public float projectileSpeed;        // Initial speed of the projectile

    private GameObject player;
    private float cooldownTimer = 0f;
    private bool canFire = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cooldownTimer = tankProjectilePrefab.GetComponent<EnemyProjectile>().fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        RotateFiringPointTowardsPlayer();
        
        // Check cooldown timer
        if (!canFire)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
                canFire = true;
            return;
        }

        // Check if player is within detection radius
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= detectionRadius && canFire)
        {
            Vector3 targetPosition = player.transform.position; // Capture snapshot of player's position
            FireAtPlayer();
            canFire = false;
            cooldownTimer = tankProjectilePrefab.GetComponent<EnemyProjectile>().fireRate; // Reset cooldown
        }
    }

    void FireAtPlayer()
    {
        Quaternion adjustedRotation = turret.rotation * Quaternion.Euler(-90f, 0f, 0f);
        GameObject bullet = Instantiate(tankProjectilePrefab, cannon.position - cannon.forward * 1.2f, adjustedRotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = -turret.forward * bullet.GetComponent<EnemyProjectile>().projectileSpeed;
        }
    }

    void RotateFiringPointTowardsPlayer()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = player.transform.position - turret.position;

        // Set y-component to 0 to ensure only horizontal rotation
        directionToPlayer.y = 0;

        // Calculate the target rotation to face the player
        Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer);

        // Apply the rotation to the firing point
        turret.rotation = targetRotation;
    }
}
