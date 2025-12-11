using UnityEngine;

public class MarioController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private Vector3 originalScale;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
        originalScale = transform.localScale; 
    }

    void Update()
    {
        // --- Movimiento horizontal ---
        float move = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            move = -1f;  // Izquierda
        else if (Input.GetKey(KeyCode.RightArrow))
            move = 1f;   // Derecha

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // --- Salto (flecha arriba) ---
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
        bool running = move != 0;                   
        animator.SetBool("running", running);
        
        if (move < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (move > 0)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }
}
