using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public Camera cam;

    public float CurrentLeanInput { get; private set; }
    public float MoveSpeed { get; private set; }
    [Header("Skiing")]
    public float gravityForce = 20f;
    public float inputForce = 10f;
    public float gravityWeight = 0.6f;
    public float inputWeight = 0.4f;
    public float maxSpeed = 40f;
    public float minSpeed = 5f;
    public float turnSpeed = 3f;
    public float deceleration = 0.00001f;
    public float leanForce = 20f;
    public float leanInfluence = 0.5f;
    [Header("Ground Check")]
    public float groundCheckDistance = 1.2f;
    public LayerMask groundMask;

    private bool grounded;
    private RaycastHit slopeHit;
    private Vector3 inputDir;
    private Vector3 lateralDir;
    private Vector3 inputForceVec;
    private PlayerInputSystem controls;
    private Vector2 moveInput;

    private void Start()
    {
        rb.useGravity = false;
        rb.freezeRotation = true;
        controls = new PlayerInputSystem();
        controls.Enable();
    }

    private void FixedUpdate()
    {
        CheckGround();

        if (grounded)
        {
            moveInput = controls.Player.Move.ReadValue<Vector2>();
            CurrentLeanInput = moveInput.x;
            lateralDir = Vector3.ProjectOnPlane(orientation.right, slopeHit.normal).normalized;
            ApplySkiingForces();
            ApplyTurning();
            ClampSpeed();
            ApplyDeceleration();
        }
        else
        {
            rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);
        }
        print(rb.linearVelocity.magnitude);
    }

    void CheckGround()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, out slopeHit, groundCheckDistance, groundMask);
    }

    void ApplySkiingForces()
    {
        Vector3 slopeGravity = Vector3.ProjectOnPlane(Vector3.down * gravityForce, slopeHit.normal);
        
        inputDir = Vector3.ProjectOnPlane(orientation.forward, slopeHit.normal).normalized;


        inputForceVec = inputDir * inputForce * moveInput.y;

        float currentSpeed = rb.linearVelocity.magnitude;
        MoveSpeed = currentSpeed;
        Vector3 leanForceVec = lateralDir * leanForce * moveInput.x * (currentSpeed / maxSpeed);
        print(leanForceVec);
        Vector3 totalForce = (slopeGravity * gravityWeight) + (inputForceVec * inputWeight) + leanForceVec;
        rb.AddForce(totalForce, ForceMode.Acceleration);

    }

    void ApplyTurning()
    {
        if (rb.linearVelocity.magnitude < 0.1f) return;

        Vector3 targetDir = (inputDir + lateralDir * moveInput.x * leanInfluence).normalized;
        Vector3 newDir = Vector3.RotateTowards(rb.linearVelocity.normalized, targetDir, turnSpeed * Time.fixedDeltaTime, 0f);
        rb.linearVelocity = newDir * rb.linearVelocity.magnitude;
        
    }

    void ClampSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    void ApplyDeceleration()
    {
        if (controls.Player.Move.ReadValue<Vector2>().y == 0 && grounded)
        {
            float newSpeed = Mathf.MoveTowards(rb.linearVelocity.magnitude, 0f, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = rb.linearVelocity.normalized * newSpeed;
        }
    }
}
