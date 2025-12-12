using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Tiempo de juego")]
    public float tiempoInicial = 30f;
    private float tiempoRestante;
    public TMP_Text textoTemporizador;

    [Header("Sonidos")]
    public AudioSource audioSource; // Sonido tic-tac final
    public AudioSource musicaJuego; // Música principal del juego

    [Header("Puntajes de jugadores")]
    public TMP_Text textoPuntajeJugador1;
    public TMP_Text textoPuntajeJugador2;
    public TMP_Text textoPuntajeJugador3;
    public TMP_Text textoPuntajeJugador4;

    [Header("Texto del ganador o empate")]
    public TMP_Text textoGanador;

    // Variables de puntaje
    private int puntajeJugador1 = 0;
    private int puntajeJugador2 = 0;
    private int puntajeJugador3 = 0;
    private int puntajeJugador4 = 0;

    private bool juegoActivo = true;
    private bool sonidoReproducido = false;

    [Header("Botón de reinicio")]
    public GameObject botonReiniciar;

    void Start()
    {
        tiempoRestante = tiempoInicial;

        if (textoTemporizador != null)
            textoTemporizador.text = Mathf.CeilToInt(tiempoRestante).ToString();

        // Asegurarse de que el texto del ganador esté oculto al inicio
        if (textoGanador != null)
            textoGanador.gameObject.SetActive(false);

        // 🎵 Iniciar música de fondo
        if (musicaJuego != null)
            musicaJuego.Play();
    }

    void Update()
    {
        if (!juegoActivo) return;

        tiempoRestante -= Time.deltaTime;
        textoTemporizador.text = Mathf.CeilToInt(tiempoRestante).ToString();

        // Cambiar color del temporizador cuando quedan menos de 5 segundos
        if (tiempoRestante <= 5)
        {
            float t = Mathf.PingPong(Time.time * 5f, 1f);
            textoTemporizador.color = Color.Lerp(Color.white, Color.red, t);
        }
        else
        {
            textoTemporizador.color = Color.white;
        }

        // Reproducir sonido tic-tac en los últimos 3 segundos
        if (tiempoRestante <= 3 && !sonidoReproducido)
        {
            if (audioSource != null)
                audioSource.Play();
            sonidoReproducido = true;
        }

        // Fin del tiempo
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            textoTemporizador.text = "¡FIN!";
            textoTemporizador.color = Color.yellow;
            TerminarJuego();
        }
    }

    // 🔹 Sumar puntos a los jugadores
    public void SumarPunto(int jugadorID)
    {
        if (!juegoActivo) return;

        switch (jugadorID)
        {
            case 1:
                puntajeJugador1++;
                textoPuntajeJugador1.text = puntajeJugador1.ToString();
                break;
            case 2:
                puntajeJugador2++;
                textoPuntajeJugador2.text = puntajeJugador2.ToString();
                break;
            case 3:
                puntajeJugador3++;
                textoPuntajeJugador3.text = puntajeJugador3.ToString();
                break;
            case 4:
                puntajeJugador4++;
                textoPuntajeJugador4.text = puntajeJugador4.ToString();
                break;
        }
    }

    // 🔹 Lógica cuando se termina el juego
    private void TerminarJuego()
    {
        juegoActivo = false;

        // Detener movimientos de todos los Rigidbody2D
        Rigidbody2D[] cuerpos = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
        foreach (var rb in cuerpos)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Desactivar controladores de personajes
        MonoBehaviour[] controladores = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var ctrl in controladores)
        {
            if (ctrl.GetType().Name.Contains("Controller") || ctrl.GetType().Name.Contains("Player"))
                ctrl.enabled = false;
        }

        // Desactivar notas musicales
        NotaMusical[] notas = FindObjectsByType<NotaMusical>(FindObjectsSortMode.None);
        foreach (var n in notas)
            n.enabled = false;

        // Mostrar ganador o empate
        MostrarResultadoFinal();

        // 🎵 Detener la música 3 segundos después del fin
        if (musicaJuego != null)
            StartCoroutine(DetenerMusicaDespuesDe(3f));
    }

    // 🔹 Mostrar ganador o empate
    private void MostrarResultadoFinal()
    {
        if (textoGanador == null) return;

        textoGanador.gameObject.SetActive(true);

        int[] puntajes = { puntajeJugador1, puntajeJugador2, puntajeJugador3, puntajeJugador4 };
        string[] nombres = { "Luigi", "Mario", "Peach", "Toad" };

        int maxPuntaje = Mathf.Max(puntajes);
        string ganadores = "";

        // Buscar cuántos jugadores empataron
        int cantidadGanadores = 0;
        for (int i = 0; i < puntajes.Length; i++)
        {
            if (puntajes[i] == maxPuntaje)
            {
                cantidadGanadores++;
                if (ganadores != "") ganadores += " y ";
                ganadores += nombres[i];
            }
        }

        // Mostrar mensaje según resultado
        if (cantidadGanadores > 1)
        {
            textoGanador.text = "¡Empate entre " + ganadores + "!";
            textoGanador.color = Color.cyan;
        }
        else
        {
            textoGanador.text = "¡Gana " + ganadores + "!";
            textoGanador.color = Color.yellow;
        }
        //GameObject boton = GameObject.Find("BotonReiniciar");
        if (botonReiniciar != null)
            botonReiniciar.SetActive(true);

    }

    // 🔹 Detener música después de X segundos
    private IEnumerator DetenerMusicaDespuesDe(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        musicaJuego.Stop();
    }
    public void ReiniciarJuego()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
