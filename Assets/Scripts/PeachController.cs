using UnityEngine;

public class PeachController : MonoBehaviour
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
        animator = GetComponentInChildren<Animator>();
        originalScale = transform.localScale; // Guardar la escala original
    }

    void Update()
    {
        // --- Movimiento horizontal ---
        float move = 0f;

        if (Input.GetKey(KeyCode.H))
            move = -1f;  // Izquierda
        else if (Input.GetKey(KeyCode.K))
            move = 1f;   // Derecha

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // --- Salto (tecla U) ---
        if (Input.GetKeyDown(KeyCode.U) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
        bool running = move != 0;                    // True si se mueve
        animator.SetBool("running", running);
        
        if (move < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (move > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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
