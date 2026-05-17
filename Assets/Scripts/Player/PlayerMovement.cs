using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 12f;
    public float jumpForce = 7f;
    public float groundDistance = 0.5f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public float externalDecay = 8f;
    private Rigidbody rb;
    private Vector2 movementInput;
    private bool isGrounded;

    private Vector3 externalVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckGround();
        ApplyExternalDecay();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void OnMovement(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void MovePlayer()
    {
        Vector3 moveDirection =
            transform.right * movementInput.x +
            transform.forward * movementInput.y;

        moveDirection.Normalize();

        Vector3 targetVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            rb.linearVelocity.y,
            moveDirection.z * moveSpeed
        );

        rb.linearVelocity = targetVelocity + externalVelocity;
    }

    void ApplyExternalDecay()
    {
        externalVelocity = Vector3.Lerp(
            externalVelocity,
            Vector3.zero,
            externalDecay * Time.deltaTime
        );
    }

    public void AddExternalVelocity(Vector3 velocity)
    {
        externalVelocity += velocity;
    }
}