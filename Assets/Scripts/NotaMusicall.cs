using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(AudioSource))]
public class NotaMusical : MonoBehaviour
{
    [Header("Flotación horizontal")]
    public float amplitudHorizontal = 2f;    
    public float velocidadFlotacion = 1f;    
    private float xPosInicial;               

    [Header("Rebote / Impulso")]
    public float fuerzaDisparo = 10f;        
    public float arrastreInicial = 0.3f;     
    public float arrastreSuave = 2.5f;       
    public float tiempoReanudarFlotacion = 1.5f; 

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool flotando = true;
    private bool enImpulso = false;
    private GameObject ultimaSuperficieTocada;
    private float tiempo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // Configuración física
        rb.gravityScale = 0.3f;
        rb.linearDamping = arrastreSuave;
        rb.freezeRotation = true; 
        rb.angularDamping = 0f;

        // Configuración del sonido
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        xPosInicial = transform.position.x;
    }

    void Update()
    {
        if (flotando && !enImpulso)
        {
            tiempo += Time.deltaTime * velocidadFlotacion;
            float desplazamientoX = Mathf.Sin(tiempo) * amplitudHorizontal;
            transform.position = new Vector3(xPosInicial + desplazamientoX, transform.position.y, transform.position.z);
        }
        // Flotación normal
        if (flotando && !enImpulso)
        {
            tiempo += Time.deltaTime * velocidadFlotacion;
            float desplazamientoX = Mathf.Sin(tiempo) * amplitudHorizontal;
            transform.position = new Vector3(xPosInicial + desplazamientoX, transform.position.y, transform.position.z);
        }

        // Si se sale del rango visible, reubicarla al centro
        if (Mathf.Abs(transform.position.x) > 12 || Mathf.Abs(transform.position.y) > 7)
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = new Vector3(0, 0, 0); 
            xPosInicial = transform.position.x;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        //  Evita rebotar dos veces con la misma superficie
        if (collision.gameObject == ultimaSuperficieTocada)
            return;

        ultimaSuperficieTocada = collision.gameObject;

        //  Sonido del impacto
        if (audioSource != null)
            audioSource.Play();

        if (collision.contacts.Length > 0)
        {
            // Dirección opuesta al impacto
            Vector2 direccion = (collision.contacts[0].normal * -1f + Random.insideUnitCircle * 0.1f).normalized;

            rb.linearVelocity = Vector2.zero; 
            rb.AddForce(direccion * fuerzaDisparo, ForceMode2D.Impulse);

            StartCoroutine(ImpulsoTemporal());
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
       
        if (collision.gameObject == ultimaSuperficieTocada)
            ultimaSuperficieTocada = null;
    }

    private System.Collections.IEnumerator ImpulsoTemporal()
    {
        enImpulso = true;
        flotando = false;

        rb.linearDamping = arrastreInicial; 
        yield return new WaitForSeconds(0.3f);

        rb.linearDamping = arrastreSuave; 
        yield return new WaitForSeconds(tiempoReanudarFlotacion);

        rb.linearVelocity = Vector2.zero;
        flotando = true;
        enImpulso = false;

        // Reinicia la posición base de flotación
        xPosInicial = transform.position.x;
        tiempo = 0f;
    }
}
