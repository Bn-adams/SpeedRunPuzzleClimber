using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(PlayerManager))]
public class PlayerInput : MonoBehaviour
{
    private PlayerManager _playerManager;

    private Camera _camera;

    [SerializeField] GameObject sparkHalo;
    [SerializeField] GameObject fireWhooshSFX;

    private Rigidbody _bodyRB;
    private Rigidbody _handRB;


    // Controller
    private float leftTrigger;
    private float rightTrigger;

    private float leftShoulder;
    private float rightShoulder;
    
    private Vector2 leftStick;
    private Vector2 rightStick;

    private Vector2 swingStick;

    [SerializeField] private bool _leftStickInUse = true;


    [SerializeField] private bool mouseInUse = false;

    // Mouse & keyboard
    private Vector2 mouseDelta;
    [SerializeField] private float mouseSensitivity = 0.01f;
    
    private Vector2 mouseHandOffset;

    [SerializeField] private float mouseGripSensitivity = 0.02f;

    // Dead zones
    private float triggerDeadZone = 0.1f;
    private float joystickDeadZone = 0.2f;

    // Viberation
    public bool hasVibrated;

    // -- Taken from PlayerManager
    private float armLength;


    [Header("Player Settings")]
    [SerializeField] private bool invertGrippingInput = true;
    [SerializeField] private float handMoveSpeed = 100;
    
    public bool shouldRestartTimer = false;

    // Vibration
    private Coroutine GripVibrationCoroutine;
    [SerializeField] private bool vibrationEnabled = true;
    [SerializeField] private float gripVibrationDuration = 0.05f;
    [SerializeField] private float gripVibrationStrengthLowFrequency = 0.05f;
    [SerializeField] private float gripVibrationStrengthHighFrequency = 0.1f;
    [SerializeField] private float decelerationVibrationDuration = 0.05f;
    [SerializeField] private float decelerationVibrationStrengthLowFrequency = 0.1f;
    [SerializeField] private float decelerationVibrationStrengthHighFrequency = 0.2f;


    [SerializeField] private float gripReleaseDash = 0.2f;

    [SerializeField] float swingDampening = 0.98f;        // The rate which the x axis linear velocity multiplies by on fixed update


    [Header("Joystick Gripping Settings")]
    private bool _gripped;
    [SerializeField] float forceMultiplier = 12f;
    [SerializeField] float initialAccelerationBoost = 50f;
    [SerializeField] float boostFadeSpeed = 15f;
    [SerializeField] float maxSwingSpeed = 25f;


    [Header("Player stats")]

    [SerializeField] float speed;
    [SerializeField] float boost;
    [SerializeField] float acceleration;

    [SerializeField] float LargestForce;

    private void Awake()
    {
        // removes the cursor, may have to tweak this when the ui is back
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


        _playerManager = GetComponent<PlayerManager>();
        _camera = Camera.main;
        armLength = _playerManager.armLength;
        _bodyRB = _playerManager.bodyRB;
        _handRB = _playerManager.handRB;
    }
    private void Update()
    {
        InitializeGamepad();
        InitializeMouse();

        if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            mouseInUse = true;
        }

        if (mouseInUse)
        {
            MouseMovement();
            MouseGrippingLogic();
        }
        else
        {
            ControllerMovement(swingStick);
            ControllerGrippingLogic();
        }
       
    }
    private void FixedUpdate()
    {
        GrippedHandMovement();
    }

    private void GrippedHandMovement()
    {
        if (!_playerManager.isGripping) return;

        if (mouseInUse)
        {
            MouseGripppedMovement();
        }
        else
        {
            ControllerGrippedMovement(swingStick);
        }
    }
    private void InitializeGamepad()
    {
        // Get the current gamepad
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            mouseInUse = true;
            return;
        }

        // Read joystick values
        leftStick = gamepad.leftStick.ReadValue();
        rightStick = gamepad.rightStick.ReadValue();

        if (leftStick != Vector2.zero)
        {
            mouseInUse = false;
            _leftStickInUse = true;
        }
        if (rightStick != Vector2.zero)
        {
            mouseInUse = false;
            _leftStickInUse = false;
        }

        if (_leftStickInUse)
        {
            swingStick = leftStick;
        }
        else
        {
            swingStick = rightStick;
        }

        leftTrigger = gamepad.leftTrigger.ReadValue();
        rightTrigger = gamepad.rightTrigger.ReadValue();

        leftShoulder = gamepad.leftShoulder.ReadValue();
        rightShoulder = gamepad.rightShoulder.ReadValue();

        if (gamepad.buttonNorth.wasPressedThisFrame)
        {
            _playerManager.spawnManager.SpawnPlayer();
        }
        if (gamepad.buttonEast.wasPressedThisFrame)
        {
            _playerManager.spawnManager.currentCheckpoint = Vector2.zero;
            _playerManager.spawnManager.SpawnPlayer();
        }
        if (gamepad.buttonSouth.wasPressedThisFrame && _playerManager.hasFinished)
        {
            _playerManager.spawnManager.currentCheckpoint = Vector2.zero;
            _playerManager.hasFinished = false;
            _playerManager.OpenMenu();
        }
        if (gamepad.startButton.wasPressedThisFrame) _playerManager.OpenMenu();
    }

    private void InitializeMouse()
    {
        if (Mouse.current == null) return;

        mouseDelta = Mouse.current.delta.ReadValue();

        if (Mouse.current.forwardButton.wasPressedThisFrame)
        {
            _playerManager.spawnManager.SpawnPlayer();
        }
        if (Mouse.current.backButton.wasPressedThisFrame)
        {
            _playerManager.spawnManager.currentCheckpoint = Vector2.zero;
            _playerManager.spawnManager.SpawnPlayer();
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            
            _playerManager.OpenMenu();
        }
        if (Mouse.current.leftButton.wasPressedThisFrame && !Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void MouseMovement()
    {
        if (_playerManager.isGripping)
            return;

        Vector2 movement = mouseDelta * mouseSensitivity;

        mouseHandOffset += movement;

        mouseHandOffset = Vector2.ClampMagnitude(
            mouseHandOffset,
            armLength
        );

        Vector3 targetPos =
            _bodyRB.transform.position +
            new Vector3(
                mouseHandOffset.x,
                mouseHandOffset.y,
                0f
            );

        _handRB.transform.position = targetPos;
    }
    

    // Move hand based on joystick input and handle gripping
    private void ControllerMovement(Vector2 stick)
    {

        if (!_playerManager.isGripping)
        {
            Vector3 L_WorldOffset = new Vector3(
                Mathf.Clamp(stick.x, -1, 1) * armLength,
                Mathf.Clamp(stick.y, -1, 1) * armLength,
                0f);

            Vector3 targetPos = L_WorldOffset + _bodyRB.transform.position;
            _handRB.transform.position = Vector3.MoveTowards(_handRB.transform.position, targetPos, handMoveSpeed * Time.deltaTime);
        }

    }
    private void MouseGripppedMovement()
    {

        Vector2 forceDirection = Mouse.current.delta.ReadValue() * mouseGripSensitivity;

        if (invertGrippingInput)
            forceDirection = -forceDirection;

        forceDirection = Vector2.ClampMagnitude(forceDirection, 1f);

        Debug.Log(forceDirection.magnitude);

        speed = _bodyRB.linearVelocity.magnitude;

        boost = Mathf.Clamp01(
            1f - speed / boostFadeSpeed
        );

        acceleration =
            forceMultiplier +
            initialAccelerationBoost * boost;

        if (speed < maxSwingSpeed)
        {
            _bodyRB.AddForce(
                forceDirection * acceleration,
                ForceMode.Acceleration
            );
            if (LargestForce < (forceDirection * acceleration).magnitude)
            {
                LargestForce = (forceDirection * acceleration).magnitude;
                Debug.Log($"Input: {forceDirection.magnitude} | Acceleration: {acceleration} | Force: {(forceDirection * acceleration).magnitude}");
            }
        }
    }

    private void ControllerGrippedMovement(Vector2 joyStick)
    {
        if (joyStick == Vector2.zero)
        {
            _bodyRB.linearVelocity *= swingDampening;
        }

        Vector2 forceDirection =
            invertGrippingInput ? -joyStick : joyStick;

        speed = _bodyRB.linearVelocity.magnitude;

        boost = Mathf.Clamp01(
            1f - speed / boostFadeSpeed
        );

        acceleration =
            forceMultiplier +
            initialAccelerationBoost * boost;

        if (speed < maxSwingSpeed)
        {
            _bodyRB.AddForce(
                forceDirection * acceleration,
                ForceMode.Acceleration
            );

            if (LargestForce < (forceDirection * acceleration).magnitude)
            {
                LargestForce = (forceDirection * acceleration).magnitude;
                Debug.Log($"Input: {forceDirection.magnitude} | Acceleration: {acceleration} | Force: {(forceDirection * acceleration).magnitude}");
            }
        }
    }
    private void MouseGrippingLogic()
    {
        if (Mouse.current == null)
            return;

        bool leftMouse = Mouse.current.leftButton.isPressed;
        bool rightMouse = Mouse.current.rightButton.isPressed;

        

        if (leftMouse)
        {
            if (_playerManager.CanGripFinish) { OnGrip(); Finish(); }
            else if (_playerManager.CanGripCheckpoint){OnGrip();_playerManager.spawnManager.SetCheckPoint();}
            else if (_playerManager.CanGripJug && !rightMouse){ OnGrip(); }
            else if (!_playerManager.isRespawning){ OnGripRelease();}
        }
        else if (rightMouse)
        {
            if (_playerManager.CanGripFinish) { OnGrip(); Finish(); }
            else if (_playerManager.CanGripCheckpoint) { OnGrip(); _playerManager.spawnManager.SetCheckPoint(); }
            else if (_playerManager.CanGripCrimp && !leftMouse) OnGrip();
            else if (!_playerManager.isRespawning) { OnGripRelease(); }
        }
        else
        {
            OnGripRelease();
        }
    }
    private void ControllerGrippingLogic()
    {
        bool leftTriggerPressed = leftTrigger >= triggerDeadZone;
        bool leftShoulderPressed = leftShoulder >= triggerDeadZone;
        bool rightTriggerPressed = rightTrigger >= triggerDeadZone;
        bool rightShoulderPressed = rightShoulder >= triggerDeadZone;

        // Left Hand Grip Logic
        if (leftTriggerPressed || rightTriggerPressed)
        {
            if (_playerManager.CanGripFinish) { OnGrip(); Finish(); }
            else if (_playerManager.CanGripCheckpoint) { OnGrip(); _playerManager.spawnManager.SetCheckPoint(); }
            else if (_playerManager.CanGripJug && !leftShoulderPressed) OnGrip();
            else if (_playerManager.CanGripPocket && leftShoulderPressed) OnGrip();
            else if (!_playerManager.isRespawning) { OnGripRelease(); }
        }
        else if (leftShoulderPressed || rightShoulderPressed)
        {
            if (_playerManager.CanGripFinish) { OnGrip(); Finish(); }
            else if (_playerManager.CanGripCheckpoint) { OnGrip(); _playerManager.spawnManager.SetCheckPoint(); }
            else if (_playerManager.CanGripCrimp && !leftTriggerPressed) OnGrip();
            else if (!_playerManager.isRespawning) { OnGripRelease(); }
        }
        else
        {
            OnGripRelease();
        }
        
    }

    private void OnGrip()
    {

        mouseDelta = Vector2.zero;
        if (_playerManager.isRespawning)
        {
            _playerManager.isRespawning = false;
            if (shouldRestartTimer)
            {
                _playerManager.timerManager.timeElapsed = 0f;
                _playerManager.timerManager.isTimerRunning = true;
                shouldRestartTimer = false;
            }
            _bodyRB.constraints = RigidbodyConstraints.None;
            _bodyRB.constraints = RigidbodyConstraints.FreezePositionZ;
            _bodyRB.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if (!_gripped)
        {
            _gripped = true;
            if (_playerManager.CanGripFinish)
            {

            }
            else if (_playerManager.CanGripCheckpoint)
            {

            }
            else if (_playerManager.CanGripJug)
            {
                // Particles 
                _playerManager.particleManager.InstantiateSparkHalo(_handRB.transform.position);

                // Sound FX
                Instantiate(fireWhooshSFX);
            }
            else
            {
                _gripped = false;
                return;
            }
            _playerManager.isGripping = true;
            _handRB.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Vibration
        if (!hasVibrated && vibrationEnabled)
        {
            hasVibrated = true;
            if (GripVibrationCoroutine != null) StopCoroutine(GripVibrationCoroutine);
            GripVibrationCoroutine = StartCoroutine(DoGripVibration());
        }
        
    }
    private void OnGripRelease()
    {
        mouseDelta = Vector2.zero;

        mouseHandOffset = _handRB.transform.position - _bodyRB.transform.position;

        if (_playerManager.isGripping == true)
        {
            _playerManager.dyno.StartDyno();


            _bodyRB.AddForce(_bodyRB.linearVelocity * gripReleaseDash, ForceMode.Impulse);

            hasVibrated = false;
            _playerManager.isGripping = false;
            _gripped = false;
            _handRB.constraints = RigidbodyConstraints.None;
        }
    }

    private IEnumerator DoGripVibration()
    {
        if (!vibrationEnabled || mouseInUse) yield break;
        Gamepad.current.SetMotorSpeeds(gripVibrationStrengthLowFrequency, gripVibrationStrengthHighFrequency);
        yield return new WaitForSeconds(gripVibrationDuration);
        Gamepad.current.SetMotorSpeeds(0, 0);
    }

    public IEnumerator DoDecelerationVibration()
    {
        if (!vibrationEnabled || mouseInUse) yield break;
        Gamepad.current.SetMotorSpeeds(decelerationVibrationStrengthLowFrequency, decelerationVibrationStrengthHighFrequency);
        yield return new WaitForSeconds(decelerationVibrationDuration);
        Gamepad.current.SetMotorSpeeds(0, 0);
    }


    private void Finish()
    {
        _playerManager.Finish();
    }
    
}
