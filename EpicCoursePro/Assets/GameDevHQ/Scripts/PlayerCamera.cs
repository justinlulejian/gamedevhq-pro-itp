using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerCamera : MonoBehaviour
{
    private Camera _cam;
    
    [Header("Player camera movement and rotation settings")]
    [SerializeField]
    [Tooltip("How fast the camera zoom movement is in response to user mouse scroll wheel input.")]
    private float _zoomSpeed = 1000.0f;
    [SerializeField]
    [Tooltip("How fast the camera rotates itself in response to user input.")]
    private float _keyboardMovementSpeed = 5f;
    [SerializeField]
    [Tooltip("How fast the camera move in response to user mouse input.")]
    private float _mouseMovementSpeed = 5f;
    [SerializeField]
    [Tooltip("% away from the edge of the screen where mouse movement will trigger.")]
    private float _mouseMovementTriggerDistance = .05f;
    [SerializeField]
    [Tooltip("% away from the edge of the screen where mouse movement will trigger.")]
    private float _maximumHorizontalRotationDelta = 10f;
    [SerializeField]
    [Tooltip("% away from the edge of the screen where mouse movement will trigger.")]
    private float _maximumVerticalRotationDelta = 10f;
    [SerializeField]
    [Tooltip("% away from the edge of the screen where mouse movement will trigger.")]
    private float _maximumHorizontalMovementDelta = 10f;
    [SerializeField]
    [Tooltip("% away from the edge of the screen where mouse movement will trigger.")]
    private float _maximumVerticallMovementDelta = 10f;
    
    private Vector3 _cameraOriginalPosition;
    private Quaternion _cameraOriginalRotation;
    
    // TODO: Bounding, I think I should just bit the bullet and do basic static clamping and forget about using
    // clamp magnitude. I can look into something better later. Right now it moves correctly at least.
    private void Awake()
    {
        _cam = this.transform.GetComponent<Camera>();
        _cameraOriginalPosition = transform.parent.position;
        _cameraOriginalRotation = transform.parent.rotation;
        
        if (_cam == null)
        {
            Debug.LogError("Main camera was not found for player. Movement may be impacted.");
        }
        if (_cameraOriginalPosition == null)
        {
            Debug.LogError("Player camera's original position was not found. Movement may " +
                           "be impacted.");
        }
        if (_cameraOriginalRotation == null)
        {
            Debug.LogError("Player camera's original rotation was not found. Movement may " +
                           "be impacted.");
        }
    }

    private void Update()
    {
        Debug.Log($"_cameraOriginalPosition: {_cameraOriginalPosition.ToString()}");
        CalculateMovement();
    }


    // Allow the scroll wheel and the mouse cursor to move the camera position RTS-style.
    private void CalculateMovement()
    {
        Vector3 _movement = new Vector3();
        CalculateKeyboardMovement(ref _movement);
        CalculateZoomMovement(ref _movement);
        // CalculateMouseMovement(ref _movement);
        MoveWithBounds(ref _movement);
    }

    private void CalculateZoomMovement(ref Vector3 position)
    {
        // Move the camera forwards and backwards on the scroll wheel.
        float zoomForwardBackwardMovement = (Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed *
                                             Time.deltaTime);
        position += new Vector3(0, 0, zoomForwardBackwardMovement);
    }

    private void CalculateKeyboardMovement(ref Vector3 position)
    {
        // TODO: This is not so much vertical as it is hover forward, notate better?
        float verticalMovementSpeed = (
            Input.GetAxis("Vertical") * _keyboardMovementSpeed * Time.deltaTime);
        position +=  new Vector3(0, verticalMovementSpeed, verticalMovementSpeed);
        
        float horizontalMovementSpeed = (
            Input.GetAxis("Horizontal") * _keyboardMovementSpeed * Time.deltaTime);
        position += new Vector3(horizontalMovementSpeed, 0, 0);
    }

    // Mouse movement when cursor approaches end of screen. RTS-style.
    private void CalculateMouseMovement(ref Vector3 position)
    {
        
        if (Input.mousePosition.y > (Screen.height * (1.0f - _mouseMovementTriggerDistance)))
        {
            position += (Vector3.up * (Time.deltaTime * _mouseMovementSpeed));
        }
        if (Input.mousePosition.y < (1.0f - _mouseMovementTriggerDistance))
        {
            position += (Vector3.down * (Time.deltaTime * _mouseMovementSpeed));
        }
        if (Input.mousePosition.x > (Screen.width * (1.0f - _mouseMovementTriggerDistance)))
        {
            position += (Vector3.right * (Time.deltaTime * _mouseMovementSpeed));
        }
        if (Input.mousePosition.x < (1.0f - _mouseMovementTriggerDistance))
        {
            position += (Vector3.left * (Time.deltaTime * _mouseMovementSpeed));
        }
    }

    private void MoveWithBounds(ref Vector3 movePosition)
    {
        // Bounds: Zoom Z axis +40:-5 
        // Y axis: +7:-8 
        // X axis: +5:-6 (need to prevent it from going outside buildings when zoomed though...)
        _cam.transform.Translate(movePosition);
        Debug.Log($"Vector3.distance is now: {Vector3.Distance(_cameraOriginalPosition, transform.position).ToString()}");
    }
}
