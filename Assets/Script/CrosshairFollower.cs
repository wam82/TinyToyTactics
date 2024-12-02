using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairFollower : MonoBehaviour
{
    public RectTransform crosshair; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPoint;
        
        // Convert the mouse position from screen space to canvas space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(crosshair.parent.GetComponent<RectTransform>(), mousePosition, null, out worldPoint);

        // Set the crosshair position
        crosshair.localPosition = worldPoint;
    }
}
