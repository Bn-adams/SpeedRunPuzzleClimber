using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class JointManager : MonoBehaviour
{
    private PlayerManager _playerMovement;

    [SerializeField] private ConfigurableJoint _configurableJoint;


    [Header("Arm and joint settings")]
    private float armLength;
    [SerializeField] private float jointBreakingSensitivity = 0.99f;
    [SerializeField] private bool springEnabled = false;
    [SerializeField] private float jointSpring = 500f;
    [SerializeField] private float jointDamper = 80f;

    private void Awake()
    {
        _configurableJoint = GetComponent<ConfigurableJoint>();
        _playerMovement = GetComponent<PlayerManager>();
        armLength = _playerMovement.armLength;
    }

    private void Start()
    {
        CreateJoint();
    }

    public void JointChecking()
    {
        if (_playerMovement.isGripping)
        {
            float distance = Vector3.Distance(_playerMovement.bodyRB.position, _playerMovement.handRB.position);

            if (_configurableJoint == null && distance >= armLength)
            {
                CreateJoint();
            }

            if (_configurableJoint != null && distance < armLength * jointBreakingSensitivity)
            {
                DestroyJoint();
            }
        }
        else
        {
            if (_configurableJoint != null)
            {
                DestroyJoint();
            }
        }
    }


    private void CreateJoint()
    {
        _configurableJoint = _playerMovement.bodyRB.gameObject.AddComponent<ConfigurableJoint>();
        _configurableJoint.connectedBody = _playerMovement.handRB;

        // Prevent Unity from auto adjusting anchor positions
        _configurableJoint.autoConfigureConnectedAnchor = false;
        // L_shoulderPoint.position
        _configurableJoint.anchor = Vector3.zero;
        _configurableJoint.connectedAnchor = Vector3.zero;

        // Limit motion to simulate a rope/arm constraint
        _configurableJoint.xMotion = ConfigurableJointMotion.Limited;
        _configurableJoint.yMotion = ConfigurableJointMotion.Limited;
        _configurableJoint.zMotion = ConfigurableJointMotion.Limited;

        // Spring to arm to reduce jitering when swinging
        if (springEnabled)
        {
            SoftJointLimitSpring linearSpring = new SoftJointLimitSpring();
            linearSpring.spring = jointSpring;
            linearSpring.damper = jointDamper;
            _configurableJoint.linearLimitSpring = linearSpring;
        }

        SoftJointLimit linearLimit = new SoftJointLimit();
        linearLimit.limit = armLength; // arm can stretch this far
        _configurableJoint.linearLimit = linearLimit;
    }

    private void DestroyJoint()
    {
        Destroy(_configurableJoint);
        _configurableJoint = null;
    }
}
