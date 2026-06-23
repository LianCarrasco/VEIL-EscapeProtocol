using UnityEngine;
using UnityEngine.UI;

public class VentilationSystem : MonoBehaviour
{
    [SerializeField] private float aireActual = 100f;
    [SerializeField] private float velocidadPerdida = 2f;
    [SerializeField] private float limitePeligro = 20f;
    [SerializeField] private int dañoPorFaltaDeAire = 5;
    [SerializeField] private PlayerHealth jugador;
    [SerializeField] private Slider barraAire;

    private float contadorDaño;

    private void Update()
    {
        ReducirAire();
        AplicarDañoPorFaltaDeAire();
        ActualizarBarra();
    }

    private void ReducirAire()
    {
        aireActual -= velocidadPerdida * Time.deltaTime;

        if (aireActual < 0)
        {
            aireActual = 0;
        }
    }

    private void AplicarDañoPorFaltaDeAire()
    {
        if (jugador == null)
        {
            return;
        }

        if (aireActual <= limitePeligro)
        {
            contadorDaño += Time.deltaTime;

            if (contadorDaño >= 2f)
            {
                jugador.RecibirDaño(dañoPorFaltaDeAire);
                contadorDaño = 0f;
            }
        }
        else
        {
            contadorDaño = 0f;
        }
    }

    public void RestaurarVentilacion()
    {
        aireActual += 30f;

        if (aireActual > 100f)
        {
            aireActual = 100f;
        }

        Debug.Log("Ventilación restaurada");
    }

    private void ActualizarBarra()
    {
        if (barraAire != null)
        {
            barraAire.value = aireActual / 100f;
        }
    }
}