using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Player camera settings")]
    [SerializeField] 
    private Camera _camera;
    public float _zoomSpeed = 1000.0f;
    public float _panningSpeed = 100.0f;
    
    void Update()
    {
        // Invert with -1 so W goes up and S goes down. TODO(improvement): Allow user to specify.
        float verticalRotation = Input.GetAxis("Vertical") * _panningSpeed * Time.deltaTime * -1;
        float horizontalRotation = Input.GetAxis("Horizontal") * _panningSpeed * Time.deltaTime;
        float zoomForwardBackwardMovement = (Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed *
                                             Time.deltaTime);

        // Move the camera forwards and backwards on the scrollwheel.
        transform.Translate(0, 0, zoomForwardBackwardMovement);

        // Pan camera horizontally and vertically if specified.
        transform.Rotate(verticalRotation, horizontalRotation, 0);
    }
}
