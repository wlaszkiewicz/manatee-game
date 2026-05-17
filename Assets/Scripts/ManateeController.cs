using UnityEngine;
using UnityEngine.InputSystem;

public class ManateeController : MonoBehaviour
{
    [Header("Swimming")]
    public float swimForce = 15f;
    public float verticalForce = 5f;
    public float maxSpeed = 6f;

    [Header("Water Feel")]
    public float linearDrag = 2.5f;
    public float angularDrag = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 80f;

    [Header("Boost")]
    public float boostMultiplier = 4f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDrag;
        rb.useGravity = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

 void FixedUpdate()
{
    var keyboard = Keyboard.current;
    if (keyboard == null) return;

    float forward = 0f;

    if (keyboard.wKey.isPressed) forward = 1f;
    if (keyboard.sKey.isPressed) forward = -1f;

    // full 3D camera direction including up/down
    Vector3 camForward = Camera.main.transform.forward;

    float boost = UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed ? boostMultiplier : 1f;
    Vector3 moveForce = camForward * forward * swimForce * boost;

    if (rb.linearVelocity.magnitude < maxSpeed)
        rb.AddForce(moveForce, ForceMode.Force);

    // rotate manatee to face movement direction smoothly
    if (forward != 0f)
    {
        Quaternion targetRot = Quaternion.LookRotation(camForward);
    rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.fixedDeltaTime));    }
}
}