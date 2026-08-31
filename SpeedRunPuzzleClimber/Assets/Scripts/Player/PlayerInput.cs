using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private bool hasFinished = false;
    [SerializeField] private float handMoveSpeed = 100;
    [SerializeField] private float maxVelocity = 25f;
    [SerializeField] private float maxLinearDampening = 1;
    private bool shouldRestartTimer = false;

    // Vibration
    private Coroutine GripVibrationCoroutine;
    [SerializeField] private bool vibrationEnabled = true;
    [SerializeField] private float vibrationDuration = 0.05f;
    [SerializeField] private float vibrationStrengthLowFrequency = 0.05f;
    [SerializeField] private float vibrationStrengthHighFrequency = 0.1f;

    [Header("Joystick Gripping Settings")]
    [SerializeField] float forceMultiplier = 15f;
    [SerializeField] float downThreshold = -0.85f;        // Stick must be this downward to apply following settings
    [SerializeField] float singleHandUpwardBoost = 2f; // Force multiplier on y axis when going straight up
    [SerializeField] float doubleHandedUpwardBoost = 1.4f;
    [SerializeField] float horizontalDamping = 1;      // Force dampener on x axis when going straight up
    [SerializeField] float swingDampening = 0.98f;        // The rate which the x axis linear velocity multiplies by on fixed update

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

        leftTrigger = gamepad.leftTrigger.ReadValue();
        rightTrigger = gamepad.rightTrigger.ReadValue();

        leftShoulder = gamepad.leftShoulder.ReadValue();
        rightShoulder = gamepad.rightShoulder.ReadValue();

        //if (gamepad.buttonNorth.wasPressedThisFrame)
        //{
        //    SpawnPlayer();
        //}
        //if (gamepad.buttonEast.wasPressedThisFrame)
        //{
        //    currentCheckpoint = Vector2.zero;
        //    SpawnPlayer();
        //}
        //if (gamepad.buttonSouth.wasPressedThisFrame && hasFinished)
        //{
        //    currentCheckpoint = Vector2.zero;
        //    hasFinished = false;
        //    OpenMenu();
        //}
        //if (gamepad.startButton.wasPressedThisFrame) OpenMenu();
    }
    private void Update()
    {
        InitializeGamepad();
        ControllerMovement();
        GrippingLogic();
    }
    private void FixedUpdate()
    {
        GrippedHandMovement();
    }


    private void GrippedHandMovement()
    {
        if (!_playerManager.isGripping) return;

        GrippedBodyMovement(leftStick);
    }

    
    private void GrippedBodyMovement(Vector2 joyStick)
    {
        // If stick is pushed downward
        if ((invertGrippingInput && joyStick.y < downThreshold) || (!invertGrippingInput && joyStick.y > -downThreshold))
        {

            joyStick.x *= horizontalDamping;

            // Gradually dampen swinging
            Vector3 bodyVelocity = _bodyRB.linearVelocity;
            bodyVelocity.x *= swingDampening;
            _bodyRB.linearVelocity = bodyVelocity;
        }

        // Apply force
        if (invertGrippingInput) _bodyRB.AddForce(-joyStick * forceMultiplier, ForceMode.Acceleration);
        else _bodyRB.AddForce(joyStick * forceMultiplier, ForceMode.Acceleration);
    }
    // Move hand based on joystick input and handle gripping
    private void ControllerMovement()
    {
        if (!_playerManager.isGripping)
        {
            Vector3 L_WorldOffset = new Vector3(
                Mathf.Clamp(leftStick.x, -1, 1) * armLength,
                Mathf.Clamp(leftStick.y, -1, 1) * armLength,
                0f);

            Vector3 targetPos = L_WorldOffset + _bodyRB.transform.position;
            _handRB.transform.position = Vector3.MoveTowards(_handRB.transform.position, targetPos, handMoveSpeed * Time.deltaTime);
        }

        //if (!R_isGripping)
        //{
        //    Vector3 R_WorldOffset = new Vector3(
        //        Mathf.Clamp(rightStick.x, -1, 1) * armLength,
        //        Mathf.Clamp(rightStick.y, -1, 1) * armLength,
        //        0f);

        //    Vector3 targetPos = R_WorldOffset + R_shoulderPoint.transform.position;
        //    R_handRB.transform.position = Vector3.MoveTowards(R_handRB.transform.position, targetPos, handMoveSpeed * Time.deltaTime);
        //}
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
            if (_playerManager.canGripFinish) { OnGrip(); Finish(); }
            else if (_playerManager.canGripCheckpoint) { OnGrip(); SetCheckPoint(); }
            else if (_playerManager.canGripJug && !leftShoulderPressed) OnGrip();
            else if (_playerManager.canGripPocket && leftShoulderPressed) OnGrip();
            else if (!_playerManager.isRespawning) { OnLGripRelease(); }
        }
        else if (leftShoulderPressed || rightShoulderPressed)
        {
            if (_playerManager.canGripFinish) { OnGrip(); Finish(); }
            else if (_playerManager.canGripCheckpoint) { OnGrip(); SetCheckPoint(); }
            else if (_playerManager.canGripCrimp && !leftTriggerPressed) OnGrip();
            else if (!_playerManager.isRespawning) { OnLGripRelease(); }
        }
        else
        {
            OnLGripRelease();
        }
        
    }

    private void OnGrip()
    {
        if (_playerManager.isRespawning)
        {
            _playerManager.isRespawning = false;
            if (shouldRestartTimer)
            {
                //timerHandeler.timeElapsed = 0f;
                //timerHandeler.isTimerRunning = true;
                shouldRestartTimer = false;
            }
            _bodyRB.constraints = RigidbodyConstraints.None;
            _bodyRB.constraints = RigidbodyConstraints.FreezePositionZ;
            _bodyRB.constraints = RigidbodyConstraints.FreezeRotation;
        }
        //if (!L_hasVibrated && vibrationEnabled)
        //{
        //    L_hasVibrated = true;
        //    if (GripVibrationCoroutine != null) StopCoroutine(GripVibrationCoroutine);
        //    GripVibrationCoroutine = StartCoroutine(DoGripVibration());
        //}
        _playerManager.isGripping = true;
        _handRB.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void OnLGripRelease()
    {
        L_hasVibrated = false;
        _playerManager.isGripping = false;
        _handRB.constraints = RigidbodyConstraints.None;
    }
    
    //private IEnumerator DoGripVibration()
    //{
    //    Gamepad.current.SetMotorSpeeds(vibrationStrengthLowFrequency, vibrationStrengthHighFrequency);
    //    yield return new WaitForSeconds(vibrationDuration);
    //    Gamepad.current.SetMotorSpeeds(0, 0);
    //}
    
    
    
    


    private void Finish()
    {

    }
    private void SetCheckPoint()
    {

    }
}
