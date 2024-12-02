using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    public float rotationSpeed = 5f;   // Controls how fast the turret rotates
    public float edgeMargin = 50f;     // Margin from screen edges to start rotating
    private float screenCenter = 0;        // Center position of the screen in pixels

    public Transform turret;
    void Start()
    {
        // Calculate the center of the screen
        screenCenter = Screen.width / 2f;

        Cursor.lockState = CursorLockMode.Confined;
        // Cursor.visible = false;
    }

    void FixedUpdate()
    {
        // Get the current mouse position on the X-axis (horizontal)
        float mouseX = Input.mousePosition.x;

        // Determine rotation direction based on mouse position
        if (mouseX < edgeMargin)
        {
            // Rotate left if mouse is within the left margin
            RotateTurret(-1);
        }
        else if (mouseX > Screen.width - edgeMargin)
        {
            // Rotate right if mouse is within the right margin
            RotateTurret(1);
        }
        else
        {
            // Stop rotation when mouse is not near the edges
            RotateTurret(0);
        }
    }

    // Method to rotate the turret
    private void RotateTurret(float direction)
    {
        // Only rotate if there's a valid direction (-1 for left, 1 for right)
        if (direction != 0)
        {
            turret.Rotate(Vector3.up * direction * rotationSpeed * Time.deltaTime);
        }
    }
}
