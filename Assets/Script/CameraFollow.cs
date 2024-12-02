using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform tankHull;  // Reference to the tank's hull (main body)
    public Transform turret;    // Reference to the tank's turret (for rotating with it)

    public Vector3 offset;  // Position offset from the tank
    public float rotationSpeed = 5f;  // Speed of camera rotation around the tank
    public float followSpeed = 10f;  // Speed of camera movement (how quickly it follows the tank)

    private Quaternion targetRotation;

    private Vector3 cameraPosition;

    public float sharkDuration;

    public float sharkMagnitude;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initially set the camera's rotation to match the turret's rotation
        targetRotation = Quaternion.Euler(0, turret.eulerAngles.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Follow the tank's position smoothly
        Vector3 targetPosition = tankHull.position + targetRotation * offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate the camera to match the turret's rotation
        targetRotation = Quaternion.Euler(0, turret.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        transform.LookAt(tankHull);
        cameraPosition = transform.position;
    }

    public void TriggerShark()
    {
        StartCoroutine(Shark());
    }
    
    private IEnumerator Shark() {
        float elapsedTime = 0f;

        while(elapsedTime < sharkDuration) {
            float x = Random.Range(-1f, 1f) * sharkMagnitude;
            float y = Random.Range(-1f, 1f) * sharkMagnitude;

            transform.localPosition = cameraPosition + new Vector3(x, y, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = cameraPosition;
    }
}
