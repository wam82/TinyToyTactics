using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTrap : MonoBehaviour
{
    public float collisionDamage;
    
    // public float damageCooldown;
    //
    // private float lastTimeDamage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            float damage = collisionDamage;
            if (gameObject.GetComponent<Enemy>().isBuffed)
            {
                damage = collisionDamage * gameObject.GetComponent<Enemy>().damageMultiplier;
            }
            player.TakeDamage(damage);
            // lastTimeDamage = Time.time;
            // Debug.Log("Player hit a Tank Trap and took " + collisionDamage + " damage.");
        }
    }

    
}
