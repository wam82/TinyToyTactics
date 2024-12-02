using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public float buffRange;

    public float damageBoost;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Find all colliders in range of the Tower's AoE
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, buffRange);
        foreach (Collider nearbyObject in hitColliders)
        {
            // Check if the nearby object has an "Enemy" tag and a component for damage handling
            Enemy enemy = nearbyObject.GetComponent<Enemy>();
            if (enemy != null && !enemy.isBuffed)
            {
                enemy.ApplyDamageBoost(damageBoost);
                enemy.isBuffed = true; // Mark as buffed to prevent reapplying the buff
            }
        }
    }
    
}
