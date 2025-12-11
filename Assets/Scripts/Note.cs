using UnityEngine;

public class Note : MonoBehaviour
{
    [Header("Movimiento")]
    public float fallSpeed = 2f;   
    public float destroyY = -6f;   

    void Update()
    {
        // Mover la nota hacia abajo
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}

