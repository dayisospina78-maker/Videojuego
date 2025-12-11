using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Tiempo de juego")]
    public float tiempoRestante = 30f;
    public Text textoTemporizador;

    [Header("Puntajes de jugadores")]
    public Text textoPuntajeJugador1;
    public Text textoPuntajeJugador2;
    public Text textoPuntajeJugador3;
    public Text textoPuntajeJugador4;

    private int puntajeJugador1 = 0;
    private int puntajeJugador2 = 0;
    private int puntajeJugador3 = 0;
    private int puntajeJugador4 = 0;

    private bool juegoActivo = true;

    void Update()
    {
        if (!juegoActivo) return;

        tiempoRestante -= Time.deltaTime;
        textoTemporizador.text = Mathf.CeilToInt(tiempoRestante).ToString();

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            textoTemporizador.text = "�FIN!";
            TerminarJuego();
        }
    }

    public void SumarPunto(int jugadorID)
    {
        if (!juegoActivo) return;

        switch (jugadorID)
        {
            case 1: puntajeJugador1++; textoPuntajeJugador1.text = puntajeJugador1.ToString(); break;
            case 2: puntajeJugador2++; textoPuntajeJugador2.text = puntajeJugador2.ToString(); break;
            case 3: puntajeJugador3++; textoPuntajeJugador3.text = puntajeJugador3.ToString(); break;
            case 4: puntajeJugador4++; textoPuntajeJugador4.text = puntajeJugador4.ToString(); break;
        }
    }

    private void TerminarJuego()
    {
        juegoActivo = false;

        Rigidbody2D[] cuerpos = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
        foreach (var rb in cuerpos)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        MonoBehaviour[] controladores = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var ctrl in controladores)
        {
            if (ctrl.GetType().Name.Contains("Controller") || ctrl.GetType().Name.Contains("Player"))
                ctrl.enabled = false;
        }

        NotaMusical[] notas = FindObjectsByType<NotaMusical>(FindObjectsSortMode.None);
        foreach (var n in notas)
            n.enabled = false;
    }
}
