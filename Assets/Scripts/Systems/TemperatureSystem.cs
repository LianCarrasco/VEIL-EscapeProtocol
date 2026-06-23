using UnityEngine;
using UnityEngine.UI;

public class TemperatureSystem : MonoBehaviour
{
    [SerializeField] private float temperaturaActual = 30f;
    [SerializeField] private float temperaturaMaxima = 100f;
    [SerializeField] private float velocidadSubida = 3f;
    [SerializeField] private float limitePeligro = 75f;
    [SerializeField] private int dañoPorCalor = 5;
    [SerializeField] private PlayerHealth jugador;
    [SerializeField] private Slider barraTemperatura;

    private float contadorDaño;

    private void Update()
    {
        SubirTemperatura();
        AplicarDañoPorCalor();
        ActualizarBarra();
    }

    private void SubirTemperatura()
    {
        temperaturaActual += velocidadSubida * Time.deltaTime;

        if (temperaturaActual > temperaturaMaxima)
        {
            temperaturaActual = temperaturaMaxima;
        }
    }

    private void AplicarDañoPorCalor()
    {
        if (jugador == null)
        {
            return;
        }

        if (temperaturaActual >= limitePeligro)
        {
            contadorDaño += Time.deltaTime;

            if (contadorDaño >= 2f)
            {
                jugador.RecibirDaño(dañoPorCalor);
                contadorDaño = 0f;
            }
        }
        else
        {
            contadorDaño = 0f;
        }
    }

    public void EnfriarSistema()
    {
        temperaturaActual -= 25f;

        if (temperaturaActual < 0)
        {
            temperaturaActual = 0;
        }

        Debug.Log("Sistema enfriado");
    }

    private void ActualizarBarra()
    {
        if (barraTemperatura != null)
        {
            barraTemperatura.value = temperaturaActual / temperaturaMaxima;
        }
    }
}