using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Trap,
        Tank,
        Mortar, 
        Tower
    }
    public EnemyType type;
    public float maxHealth;
    public float currentHealth;
    public bool isBuffed = false;
    public float damageMultiplier; // Adjusted by Tower buff
    public HealthBar healthBar;
    public float scoreValue;
    

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        
        if(healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }    
    }

    // Function to apply a damage boost from the Tower
    public void ApplyDamageBoost(float boostMultiplier)
    {
        damageMultiplier = boostMultiplier;
        Debug.Log(gameObject.name + " is buffed with a damage multiplier of " + damageMultiplier);
    }

    // Function to take damage with current damage multiplier
    public void TakeDamage(float damage)
    {
        float adjustedDamage = damage;
        if (isBuffed)
        {
            adjustedDamage = damage * damageMultiplier;
        }
        
        currentHealth -= adjustedDamage;
        // Debug.Log(gameObject.name + " took " + adjustedDamage + " damage. Current Health: " + currentHealth);
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle death (e.g., play animations, drop loot)
        Debug.Log(gameObject.name + " has been destroyed!");
        if (gameObject.name.Equals("Tower"))
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, gameObject.GetComponent<TowerBehaviour>().buffRange);
            foreach (Collider nearbyObject in hitColliders)
            {
                // Check if the nearby object has an "Enemy" tag and a component for damage handling
                Enemy enemy = nearbyObject.GetComponent<Enemy>();
                if (enemy != null && enemy.isBuffed)
                {
                    enemy.RemoveBuff();
                    enemy.isBuffed = true; // Mark as buffed to prevent reapplying the buff
                }
            }
        }
        OverlayManager.Instance.AddScore(scoreValue);
        Destroy(gameObject);
    }

    // Function to reset the buff state if the Tower effect is removed or Tower is destroyed
    public void RemoveBuff()
    {
        isBuffed = false;
        damageMultiplier = 1f; // Reset multiplier to normal
        Debug.Log(gameObject.name + " has lost the damage buff.");
    }
}
