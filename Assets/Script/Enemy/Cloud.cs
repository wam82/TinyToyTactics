using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float baseSpeed;

    public float damage;

    public float cooldown;

    public Player player;

    private float playerMaxSpeed;

    private float currentSpeed;

    private float lastTimeDamaged;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        playerMaxSpeed = player.GetComponent<HullMovement>().maxSpeed;
        currentSpeed = baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        baseSpeed = Mathf.Clamp(baseSpeed, baseSpeed, playerMaxSpeed / 2);
        currentSpeed = (playerMaxSpeed - Mathf.Abs(player.GetComponent<HullMovement>().currentSpeed)) / 2 + baseSpeed;
        
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, currentSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(damage);
            lastTimeDamaged = Time.time;   
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (Time.time > lastTimeDamaged + cooldown)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                player.TakeDamage(damage);
                lastTimeDamaged = Time.time;
            }
        }
    }
}
