using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed;

    private Rigidbody rb;


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
    }
    
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        move = new Vector3(moveX, 0f, moveZ);

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

        if(!isGrounded && rb.linearVelocity.y < 1)
            rb.AddForce(Vector3.up * Physics.gravity.y * (fallMultiplier - 1f), ForceMode.Acceleration);
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
