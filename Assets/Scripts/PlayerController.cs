using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float playerSpeed;
    [SerializeField] private float rotationSpeed = 10f;

    private Vector3 move;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteCounter;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    public bool isGrounded;


    [Header("Gravity")]
    [SerializeField] private float fallMultiplier = 2.5f;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(inputX, 0f, inputZ).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            move = camForward * inputZ + camRight * inputX;

            // Smooth rotation toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            move = Vector3.zero;
        }



        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;
   

        if (Input.GetKeyDown(KeyCode.Space)) jumpBufferCounter = jumpBufferTime;

        else jumpBufferCounter = Mathf.Max(jumpBufferCounter - Time.deltaTime, 0f);

        TryJump();

    }

    private void FixedUpdate()
    {
        rb.AddForce(move * playerSpeed, ForceMode.Acceleration);

        if (!isGrounded)
        {
            if (rb.linearVelocity.y < 0) // falling
                rb.AddForce(Vector3.up * Physics.gravity.y * (fallMultiplier - 1f), ForceMode.Acceleration);

            else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space)) // short hop if releasing early
                rb.AddForce(Vector3.up * Physics.gravity.y * (fallMultiplier / 2f), ForceMode.Acceleration);
        }
       
    }

    void TryJump()
    {
        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            Jump();
            jumpBufferCounter = 0;
            coyoteCounter = 0;
        }
    }

    void Jump()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


}
