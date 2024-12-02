using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullMovement : MonoBehaviour
{
    public float maxSpeed;
    public float acceleration;
    public float rotationSpeed;
    public float brakingForce;

    public float currentSpeed = 0f;

    public Transform hull;
    public Transform turret;

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // Apply natural deceleration when no key is pressed
            currentSpeed = Mathf.Max(currentSpeed - acceleration * Time.deltaTime, -maxSpeed);
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed = Mathf.Max(currentSpeed - acceleration * Time.deltaTime, -maxSpeed);
            }
            else if (currentSpeed <= 0)
            {
                currentSpeed = 0;
            }
        }

        // Move the tank hull forward
        transform.Translate(-hull.forward * currentSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            hull.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            hull.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}
