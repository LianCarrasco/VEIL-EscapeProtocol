using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonSounds : MonoBehaviour
{
    public AudioSource clickSound;

    public static void GuardarNivelActual()
    {
        string nivelActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("UltimoNivel", nivelActual);
        PlayerPrefs.Save();

        Debug.Log("Nivel guardado: " + nivelActual);
    }

    public void Reintentar()
    {
        StartCoroutine(CargarNivel());
    }

    IEnumerator CargarNivel()
    {
        clickSound.Play();

        yield return new WaitForSeconds(0.4f);

        string ultimoNivel = PlayerPrefs.GetString("UltimoNivel", "");

        Debug.Log("Reintentando nivel: " + ultimoNivel);

        if (!string.IsNullOrEmpty(ultimoNivel))
        {
            SceneManager.LoadScene(ultimoNivel);
        }
        else
        {
            Debug.LogError("No hay nivel guardado.");
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
