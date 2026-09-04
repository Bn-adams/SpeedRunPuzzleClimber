using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(PlayerManager))]
public class PlayerInput : MonoBehaviour
{
    private PlayerManager _playerManager;


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
    private Vector2 dashStick;

    [SerializeField] private bool _leftStickInUse = true;

    // Dead zones
    private float triggerDeadZone = 0.1f;
    private float joystickDeadZone = 0.2f;

    // Viberation
    private bool L_hasVibrated;
    private bool R_hasVibrated;

    // -- Taken from PlayerManager
    private float armLength;


    [Header("Player Settings")]
    [SerializeField] private bool invertGrippingInput = true;
    [SerializeField] private float handMoveSpeed = 100;
    [SerializeField] private float maxVelocity = 25f;
    [SerializeField] private float maxLinearDampening = 1;
    public bool shouldRestartTimer = false;

    // Vibration
    private Coroutine GripVibrationCoroutine;
    [SerializeField] private bool vibrationEnabled = true;
    [SerializeField] private float vibrationDuration = 0.05f;
    [SerializeField] private float vibrationStrengthLowFrequency = 0.05f;
    [SerializeField] private float vibrationStrengthHighFrequency = 0.1f;

    [Header("Dash")]
    [SerializeField] private bool hasDashed = false;
    [SerializeField] private float dashPower = 10;
    [SerializeField] private float dashDuration = 1;

    [SerializeField] private float gripReleaseDash = 0.2f;



    [Header("Joystick Gripping Settings")]
    //[SerializeField] float forceMultiplier = 15f;
    [SerializeField] float downThreshold = -0.85f;        // Stick must be this downward to apply following settings
    [SerializeField] float singleHandUpwardBoost = 2f; // Force multiplier on y axis when going straight up
    [SerializeField] float doubleHandedUpwardBoost = 1.4f;
    [SerializeField] float horizontalDamping = 1;      // Force dampener on x axis when going straight up
    [SerializeField] float swingDampening = 0.98f;        // The rate which the x axis linear velocity multiplies by on fixed update


    [Header("Joystick Gripping Settings")]

    [SerializeField] float forceMultiplier = 10f;
    [SerializeField] float initialAccelerationBoost = 20f;
    [SerializeField] float boostFadeSpeed = 5f;
    [SerializeField] float maxSwingSpeed = 15f;


    [Header("Player stats")]

    [SerializeField] float speed;
    [SerializeField] float boost;
    [SerializeField] float acceleration;



    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
        armLength = _playerManager.armLength;
        _bodyRB = _playerManager.bodyRB;
        _handRB = _playerManager.handRB;
    }
    private void InitializeGamepad()
    {
        // Get the current gamepad
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.Log("No controller connected.");
            return;
        }

        // Read joystick values
        leftStick = gamepad.leftStick.ReadValue();
        rightStick = gamepad.rightStick.ReadValue();

        if (_leftStickInUse)
        {
            swingStick = leftStick;
            dashStick = rightStick;
        }
        else
        {
            swingStick = rightStick;
            dashStick = leftStick;
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
    private void Update()
    {
        InitializeGamepad();
        ControllerMovement(swingStick);
        GrippingLogic();
        DashInputDetection();
    }
    private void FixedUpdate()
    {
        GrippedHandMovement();
    }
    private void DashInputDetection()
    {
        if (rightStick != Vector2.zero && !hasDashed)
        {
            hasDashed = true;
            _bodyRB.AddForce(dashStick.normalized * dashPower, ForceMode.Impulse);
        }
        
        if (rightStick == Vector2.zero)
        {
            hasDashed = false;
        }
    }

    

    

    private void GrippedHandMovement()
    {
        if (!_playerManager.isGripping) return;

        GrippedBodyMovement(swingStick);
    }

    
    private void GrippedBodyMovement(Vector2 joyStick)
    {
        //// If stick is pushed downward
        //if ((invertGrippingInput && joyStick.y < downThreshold) || (!invertGrippingInput && joyStick.y > -downThreshold))
        //{

        //    joyStick.x *= horizontalDamping;

        //    // Gradually dampen swinging
        //    Vector3 bodyVelocity = _bodyRB.linearVelocity;
        //    bodyVelocity.x *= swingDampening;
        //    _bodyRB.linearVelocity = bodyVelocity;
        //}
        


        // Apply force
        //if (invertGrippingInput) _bodyRB.AddForce(-joyStick * forceMultiplier, ForceMode.Acceleration);
        //else _bodyRB.AddForce(joyStick * forceMultiplier, ForceMode.Acceleration);



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
        }
   

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
    private void GrippingLogic()
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
        if (!L_hasVibrated && vibrationEnabled)
        {
            L_hasVibrated = true;
            if (GripVibrationCoroutine != null) StopCoroutine(GripVibrationCoroutine);
            GripVibrationCoroutine = StartCoroutine(DoGripVibration());
        }
        _playerManager.isGripping = true;
        _handRB.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void OnGripRelease()
    {
        if (_playerManager.isGripping == true)
        {
            //Debug.Log(_bodyRB.linearVelocity);
            _bodyRB.AddForce(_bodyRB.linearVelocity * gripReleaseDash, ForceMode.Impulse);

            L_hasVibrated = false;
            _playerManager.isGripping = false;
            _handRB.constraints = RigidbodyConstraints.None;
        }
    }

    private IEnumerator DoGripVibration()
    {
        Gamepad.current.SetMotorSpeeds(vibrationStrengthLowFrequency, vibrationStrengthHighFrequency);
        yield return new WaitForSeconds(vibrationDuration);
        Gamepad.current.SetMotorSpeeds(0, 0);
    }

    

    private void Finish()
    {
        _playerManager.Finish();
    }
    
}
