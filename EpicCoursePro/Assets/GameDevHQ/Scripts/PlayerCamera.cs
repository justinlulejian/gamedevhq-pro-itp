using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Player camera movement and rotation settings")]
    [SerializeField] 
    private Camera _camera;
    [SerializeField] 
    [Tooltip("How fast the camera zoom movement is in response to user mouse scroll wheel input.")]
    private float _zoomSpeed = 1000.0f;
    [SerializeField] 
    [Tooltip("How fast the camera rotates itself in response to user input.")]
    private float _panningSpeed = 100.0f;
    [SerializeField] 
    [Tooltip("How fast the camera move in response to user mouse input.")]
    private float _mouseMovementSpeed = 5f;
    [SerializeField] 
    [Tooltip("% away from the edge of the screen where mouse movement will trigger.")]
    private float _mouseMovementTriggerDistance = .05f;

    private void CalculateRotation()
    {
        // Invert with -1 so W goes up and S goes down. TODO(improvement): Allow user to specify.
        float verticalRotation = Input.GetAxis("Vertical") * _panningSpeed * Time.deltaTime * -1;
        float horizontalRotation = Input.GetAxis("Horizontal") * _panningSpeed * Time.deltaTime;
        
        // Pan camera horizontally and vertically if specified.
        transform.Rotate(verticalRotation, horizontalRotation, 0);
    }

    // Allow the scroll wheel and the mouse cursor to move the camera position RTS-style.
    private void CalculateMovement()
    {
        // Move the camera forwards and backwards on the scroll wheel.
        float zoomForwardBackwardMovement = (Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed *
                                             Time.deltaTime);
        transform.Translate(0, 0, zoomForwardBackwardMovement);

        // Mouse movement when cursor approaches end of screen. RTS-style.
        if (Input.mousePosition.y > (Screen.height * (1.0f - _mouseMovementTriggerDistance)))
        {
            transform.Translate(Vector3.up * (Time.deltaTime * _mouseMovementSpeed));
        }
        if (Input.mousePosition.y < (1.0f - _mouseMovementTriggerDistance))
        {
            transform.Translate(Vector3.down * (Time.deltaTime * _mouseMovementSpeed));
        }
        if (Input.mousePosition.x > (Screen.width * (1.0f - _mouseMovementTriggerDistance)))
        {
            transform.Translate(Vector3.right * (Time.deltaTime * _mouseMovementSpeed));
        }
        if (Input.mousePosition.x < (1.0f - _mouseMovementTriggerDistance))
        {
            transform.Translate(Vector3.left * (Time.deltaTime * _mouseMovementSpeed));
        }
    }
}
