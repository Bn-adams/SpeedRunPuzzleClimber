using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] private float grappleDistance = 20f;
    [SerializeField] private LayerMask _grappleMask;

    private SpringJoint grappleJoint;

    [SerializeField] private GameObject _player;

    private void Update()
    {
        Gamepad gamepad = Gamepad.current;

        if (gamepad == null)
            return;

        if (gamepad.aButton.wasPressedThisFrame)
        {
            TryGrapple();
        }
    }

    public void TryGrapple()
    {
        if (Gamepad.current == null)
            return;

        Vector2 stick = Gamepad.current.leftStick.ReadValue();

        Vector3 direction = new Vector3(
            stick.x,
            stick.y,
            0f
        ).normalized;

        // Don't grapple if the stick isn't being pushed
        if (direction == Vector3.zero)
            return;

        if (Physics.Raycast(
            transform.position,
            direction,
            out RaycastHit hit,
            grappleDistance,
            _grappleMask))
        {
            Grapple(hit);
        }
    }

    private void Grapple(RaycastHit hit)
    {
        if (grappleJoint != null)
            return;

        ConfigurableJoint joint = _player.gameObject.AddComponent<ConfigurableJoint>();

        joint.connectedBody = hit.rigidbody;
        joint.autoConfigureConnectedAnchor = false;

        // Anchor at the player
        joint.anchor = Vector3.zero;

        // Anchor at the point we hit
        joint.connectedAnchor = hit.point;

        // Allow movement, but limit how far we can move
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        float distance = Vector3.Distance(
            transform.position,
            hit.point
        );

        SoftJointLimit limit = joint.linearLimit;
        limit.limit = distance;
        joint.linearLimit = limit;
    }

    public void ReleaseGrapple()
    {
        if (grappleJoint != null)
        {
            Destroy(grappleJoint);
            grappleJoint = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (Gamepad.current == null)
            return;

        Vector2 stick = Gamepad.current.leftStick.ReadValue();

        Vector3 direction = new Vector3(
            stick.x,
            stick.y,
            0f
        ).normalized;

        if (direction == Vector3.zero)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawRay(
            transform.position,
            direction * grappleDistance
        );
    }
}