using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static string ultimoNivel;

    public AudioSource audioSource;
    public AudioClip sonidoClick;

    public void Jugar()
    {
        audioSource.PlayOneShot(sonidoClick);
        Invoke("CargarNivel", 0.5f); // tiempo de espera
    }

    public void Salir()
    {
        audioSource.PlayOneShot(sonidoClick);
        Invoke("CerrarJuego", 0.5f); // tiempo de espera
    }

    void CargarNivel()
    {
        SceneManager.LoadScene("Level1");
    }

    void CerrarJuego()
    {
        Application.Quit();
    }
}
