using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
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

    // These were established with what visually made sense to prevent looking beyond the map.
    // TODO(improvement): Try to create a dynamic bounds that can be set in inspector.
    private float _cameraXPosMin = -65;
    private float _cameraXPosMax = -40;
    private float _cameraYPosMin = 5f;
    private float _cameraYPosMax = 29f;
    private float _cameraZPosMin = -2.3f;
    private float _cameraZPosMax = 7f;
    private float _cameraFovMin = 8f;
    private float _cameraFovMax = 35f;
    
    private Vector3 _cameraOriginalPosition;
    
    private void Awake()
    {
        _cam = transform.GetComponent<Camera>();
        _cameraOriginalPosition = transform.parent.position;
        
        if (_cam == null)   
        {
            Debug.LogError("Main camera was not found for player. Movement may be impacted.");
        }
        if (_cameraOriginalPosition == null)
        {
            Debug.LogError("Player camera's original position was not found. Movement may " +
                           "be impacted.");
        }
    }

    private void Update()
    {
        CalculateMovement();
    }


    // Allow the scroll wheel and the mouse cursor to move the camera position RTS-style.
    private void CalculateMovement()
    {
        Vector3 _movement = new Vector3();
        CalculateKeyboardMovement(ref _movement);
        CalculateZoomMovement(ref _movement);
        CalculateMouseMovement(ref _movement);
        MoveWithBounds(ref _movement);
    }

    private void CalculateZoomMovement(ref Vector3 position)
    {
        // Move the camera forwards and backwards on the scroll wheel.
        float zoomForwardBackwardMovement = (Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed *
                                             Time.deltaTime);
        _cam.fieldOfView =  Mathf.Clamp(
            _cam.fieldOfView - zoomForwardBackwardMovement, _cameraFovMin, _cameraFovMax);
    }

    private void CalculateKeyboardMovement(ref Vector3 position)
    {
        float forwardBackwardMoveSpeed = (
            Input.GetAxis("Vertical") * _keyboardMovementSpeed * Time.deltaTime);
        position +=  new Vector3(0, forwardBackwardMoveSpeed, forwardBackwardMoveSpeed);
        
        float horizontalMovementSpeed = (
            Input.GetAxis("Horizontal") * _keyboardMovementSpeed * Time.deltaTime);
        position += new Vector3(horizontalMovementSpeed, 0, 0);
    }

    // Mouse movement when cursor approaches end of screen. RTS-style.
    private void CalculateMouseMovement(ref Vector3 position)
    {
        if (Input.mousePosition.y > (Screen.height * (1.0f - _mouseMovementTriggerDistance)))
        {
            position += ((Vector3.forward + Vector3.up) * (Time.deltaTime * _mouseMovementSpeed));
        }
        if (Input.mousePosition.y < (1.0f - _mouseMovementTriggerDistance))
        {
            position += ((Vector3.back + Vector3.down) * (Time.deltaTime * _mouseMovementSpeed));
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
    
    private bool IsMovePositionOutOfBounds(ref Vector3 movePosition)
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition + movePosition;
        if (newPosition.x < _cameraXPosMin || newPosition.x > _cameraXPosMax)
        {
            return true;
        }
        if (newPosition.y < _cameraYPosMin || newPosition.y > _cameraYPosMax)
        {
            return true;
        }
        if (newPosition.z < _cameraZPosMin || newPosition.z > _cameraZPosMax)
        {
            return true;
        }
        return false;
    }

    private void MoveWithBounds(ref Vector3 movePosition)
    {
        _cam.transform.Translate(movePosition);
        if (IsMovePositionOutOfBounds(ref movePosition))
        {
            Vector3 currentPosition = transform.position;
            currentPosition.x = Mathf.Clamp(currentPosition.x, _cameraXPosMin, _cameraXPosMax);
            currentPosition.y = Mathf.Clamp(currentPosition.y, _cameraYPosMin, _cameraYPosMax);
            currentPosition.z = Mathf.Clamp(currentPosition.z, _cameraZPosMin, _cameraZPosMax);
            transform.position = currentPosition;
        }
    }
}
