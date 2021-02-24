using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector3 centerPt = new Vector3(0, 0, 0);
    public float radius = 5f;
    
    void Update()
    {
        // Get the new position for the object.
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        Debug.Log($"movement: {movement.ToString()}");
        Vector3 newPos = transform.position + movement;
        Debug.Log($"newPos: {newPos.ToString()}");
        // Calculate the distance of the new position from the center point. Keep the direction
        // the same but clamp the length to the specified radius.
        Vector3 offset = newPos - centerPt;
        Debug.Log($"offset: {offset.ToString()}");
        Debug.Log($"Vector3.ClampMagnitude(offset, radius: {Vector3.ClampMagnitude(offset, radius).ToString()}");
        transform.position = centerPt + Vector3.ClampMagnitude(offset, radius);
        Debug.Log($"transform.position changed: {transform.position.ToString()}");
        
    }
}
