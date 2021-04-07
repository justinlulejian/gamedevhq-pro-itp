using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerCamera : MonoSingleton<PlayerCamera>
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

    // Movement 
    // These were established with what visually made sense to prevent looking beyond the map.
    // TODO(improvement): Try to create a dynamic bounds that can be set in inspector.
    private const float CameraXPosMin = -65;
    private const float CameraXPosMax = -40;
    private const float CameraYPosMin = 5f;
    private const float CameraYPosMax = 29f;
    private const float CameraZPosMin = -2.3f;
    private const float CameraZPosMax = 7f;
    private const float CameraFovMin = 8f;
    private const float CameraFovMax = 35f;
    // Mouse camera movement.
    private int _mouseCamMovementForwardTrigger;
    private int _mouseCamMovementBackwardTrigger;
    private int _mouseCamMovementLeftTrigger;
    private int _mouseCamMovementRightTrigger;
    private Vector3 _moveForward = Vector3.forward + Vector3.up;
    private Vector3 _moveBackward = Vector3.back + Vector3.down;
    
    
    private Vector3 _cameraOriginalPosition;

    public Camera GetPlayerCamera()
    {
        return _cam;
    }
    
    protected override void Awake()
    {
        base.Awake();
        _cam = transform.GetComponent<Camera>();
        _cameraOriginalPosition = transform.parent.position;
        _cam.depthTextureMode = DepthTextureMode.Depth;

        _mouseCamMovementForwardTrigger = Mathf.RoundToInt(
            Screen.height * (1.0f - _mouseMovementTriggerDistance));
        _mouseCamMovementBackwardTrigger = Mathf.RoundToInt(
            Screen.height * _mouseMovementTriggerDistance);
        _mouseCamMovementRightTrigger = Mathf.RoundToInt(
            Screen.width * (1.0f - _mouseMovementTriggerDistance));
        _mouseCamMovementLeftTrigger = Mathf.RoundToInt(
            Screen.width * _mouseMovementTriggerDistance);
        
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
        // CalculateMovement();
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
                                             Time.unscaledDeltaTime);
        _cam.fieldOfView =  Mathf.Clamp(
            _cam.fieldOfView - zoomForwardBackwardMovement, CameraFovMin, CameraFovMax);
    }

    private void CalculateKeyboardMovement(ref Vector3 position)
    {
        // GetAxisRaw otherwise it's zero with GetAxis when Time.timeScale is 0.
        float forwardBackwardMoveSpeed = (
            Input.GetAxisRaw("Vertical") * _keyboardMovementSpeed * Time.unscaledDeltaTime);
        position +=  new Vector3(0, forwardBackwardMoveSpeed, forwardBackwardMoveSpeed);
        float horizontalMovementSpeed = (
            Input.GetAxisRaw("Horizontal") * _keyboardMovementSpeed * Time.unscaledDeltaTime);
        position += new Vector3(horizontalMovementSpeed, 0, 0);
    }

    // Mouse movement when cursor approaches end of screen. RTS-style.
    private void CalculateMouseMovement(ref Vector3 position)
    {
        if (CheckFloatInIntRange(
            Input.mousePosition.y, _mouseCamMovementForwardTrigger, Screen.height))
        {
            position += (_moveForward * (Time.unscaledDeltaTime * _mouseMovementSpeed));
        }
        if (CheckFloatInIntRange(
            Input.mousePosition.y, 0, _mouseCamMovementBackwardTrigger))
        {
            position += (_moveBackward * (Time.unscaledDeltaTime * _mouseMovementSpeed));
        }
        if (CheckFloatInIntRange(
            Input.mousePosition.x, _mouseCamMovementRightTrigger, Screen.width))
        {
            position += (Vector3.right * (Time.unscaledDeltaTime * _mouseMovementSpeed));
        }
        if (CheckFloatInIntRange(
            Input.mousePosition.x, 0, _mouseCamMovementLeftTrigger))
        {
            position += (Vector3.left * (Time.unscaledDeltaTime * _mouseMovementSpeed));
        }
    }

    // Check rounded float value to int is in range [min, max) (excludes max).
    private bool CheckFloatInIntRange(float value, int min, int max)
    {
        int intValue = Mathf.RoundToInt(value);
        return intValue >= min && intValue < max;
    }
    
    private bool IsMovePositionOutOfBounds(ref Vector3 movePosition)
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition + movePosition;
        if (newPosition.x < CameraXPosMin || newPosition.x > CameraXPosMax)
        {
            return true;
        }
        if (newPosition.y < CameraYPosMin || newPosition.y > CameraYPosMax)
        {
            return true;
        }
        if (newPosition.z < CameraZPosMin || newPosition.z > CameraZPosMax)
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
            currentPosition.x = Mathf.Clamp(currentPosition.x, CameraXPosMin, CameraXPosMax);
            currentPosition.y = Mathf.Clamp(currentPosition.y, CameraYPosMin, CameraYPosMax);
            currentPosition.z = Mathf.Clamp(currentPosition.z, CameraZPosMin, CameraZPosMax);
            transform.position = currentPosition;
        }
    }
}
