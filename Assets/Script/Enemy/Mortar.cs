using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Mortar : MonoBehaviour
{
    public GameObject mortarProjectilePrefab;  // Mortar bomb projectile prefab
    public Transform mortarLauncher;              // Point from which the mortar fires
    public float detectionRadius;        // Detection range for the player
    // public float fireCooldown;            // Cooldown period between shots
    // public float projectileSpeed;        // Initial speed of the projectile

    private GameObject player;
    private float cooldownTimer = 0f;
    private bool canFire = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Assuming the player has a "Player" tag
        cooldownTimer = mortarProjectilePrefab.GetComponent<EnemyProjectile>().fireRate;
    }

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
            {
                canFire = true;
            }
            return;
        }

        // Check if player is within detection radius
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= detectionRadius && canFire)
        {
            // Debug.Log("Player in radius");
            Vector3 targetPosition = player.transform.position; // Capture snapshot of player's position
            FireProjectile(targetPosition);
            canFire = false;
            cooldownTimer = mortarProjectilePrefab.GetComponent<EnemyProjectile>().fireRate; // Reset cooldown
        }
    }

    void FireProjectile(Vector3 targetPosition)
    {
        Debug.Log("Shooting...");
        // Instantiate projectile and set its initial position and rotation
        GameObject mortarProjectile = Instantiate(mortarProjectilePrefab, mortarLauncher.position + mortarLauncher.up * 0.8f, mortarLauncher.rotation);
        
        Rigidbody rb = mortarProjectile.GetComponent<Rigidbody>();
        
        // Calculate projectile trajectory to reach target with an arc
        if (rb != null)
        {
            Vector3 initialVelocity = CalculateArcVelocity(mortarLauncher.position, targetPosition, rb.GetComponent<EnemyProjectile>().projectileSpeed);
            rb.velocity = initialVelocity;
        }
    }

    Vector3 CalculateArcVelocity(Vector3 start, Vector3 end, float speed)
    {
        Vector3 direction = end - start;
        float horizontalDistance = new Vector3(direction.x, 0, direction.z).magnitude;
        float verticalDistance = direction.y;

        // Calculate the time to reach the target based on fixed projectile speed
        float time = horizontalDistance / speed;

        // Calculate horizontal and vertical components of the velocity
        Vector3 horizontalVelocity = new Vector3(direction.x, 0, direction.z).normalized * speed;
        float verticalVelocity = (verticalDistance / time) + (0.5f * Mathf.Abs(Physics.gravity.y) * time);

        // Return the combined velocity
        return horizontalVelocity + Vector3.up * verticalVelocity;
    }

    void RotateFiringPointTowardsPlayer()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = player.transform.position - mortarLauncher.position;

        // Set y-component to 0 to ensure only horizontal rotation
        directionToPlayer.y = 0;

        // Calculate the target y-axis rotation to face the player
        Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer);

        // Apply the target rotation to the y-axis only, keeping the original x-axis tilt
        mortarLauncher.rotation = Quaternion.Euler(-45f, targetRotation.eulerAngles.y, 0);
    }

}
