using UnityEngine;
using FishNet.Object; 

public class PlayerMovement : NetworkBehaviour 
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    public Transform groundCheck;     // Titik acuan di kaki pemain
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;     // Layer khusus untuk lantai/platform

    [SerializeField] private Rigidbody2D rb;
    private bool isGrounded;
    private bool isFacingRight = true; 

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner) {
            transform.position = new Vector3(-7.5f, -2.3f, 0f);
        } 
        else {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    void Update()
    {
        // Cegah client lain menggerakkan karakter ini
        if (!IsOwner) 
            return;

        CekTanah();
        HandleMovement2D();
        HandleJump();
    }

    private void CekTanah()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }

    private void HandleMovement2D()
    {
        float moveX = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        if (moveX > 0 && !isFacingRight)
        {
            Flip();
        }

        else if (moveX < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void Flip()
    {
        // Ubah status menghadap
        isFacingRight = !isFacingRight;

        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}