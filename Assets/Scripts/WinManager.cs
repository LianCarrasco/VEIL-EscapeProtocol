using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class WinManager : MonoBehaviour
{
    public AudioSource clickSound;

    public GameObject botonSiguienteNivel;

    void Start()
    {
        // Si estamos en el nivel 3, ocultar botón siguiente nivel
        if (SceneManager.GetActiveScene().name == "Level3")
         {
             botonSiguienteNivel.SetActive(false);
         }
         else
         {
             botonSiguienteNivel.SetActive(true);
         }
    }

    public void SiguienteNivel()
    {
        StartCoroutine(CargarSiguienteNivel());
    }

    IEnumerator CargarSiguienteNivel()
    {
        clickSound.Play();

        yield return new WaitForSeconds(0.4f);

        string nivelActual = SceneManager.GetActiveScene().name;

        if (nivelActual == "Level1")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (nivelActual == "Level2")
        {
            SceneManager.LoadScene("Level3");
        }
    }


    public void IrAlMenu()
    {
        StartCoroutine(CargarMenu());
    }

    IEnumerator CargarMenu()
    {
        clickSound.Play();

        yield return new WaitForSeconds(0.4f);

        SceneManager.LoadScene("MainMenu");
    }
}
