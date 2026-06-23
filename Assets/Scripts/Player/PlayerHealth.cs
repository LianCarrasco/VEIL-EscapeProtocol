using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private Slider barraVida;

    private int vidaActual;

    public int VidaActual
    {
        get { return vidaActual; }
    }

    public bool EstaMuerto
    {
        get { return vidaActual <= 0; }
    }

    private void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarBarra();
    }

    public void RecibirDaño(int daño)
    {
        if (daño <= 0 || EstaMuerto)
        {
            return;
        }

        vidaActual -= daño;

        if (vidaActual < 0)
        {
            vidaActual = 0;
        }

        ActualizarBarra();

        if (vidaActual <= 0)
        {
            Debug.Log("Jugador murió");
        }
    }

    private void ActualizarBarra()
    {
        if (barraVida != null)
        {
            barraVida.value = (float)vidaActual / vidaMaxima;
        }
    }
}