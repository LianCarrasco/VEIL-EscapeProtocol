using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinManager : MonoBehaviour
{
    public AudioSource clickSound;
    public GameObject botonSiguienteNivel;

    void Start()
    {
        int nivel = PlayerPrefs.GetInt("NivelCompletado");

        // Si terminó Level3, ocultar botón
        if (nivel == 3)
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
        if (clickSound != null)
            clickSound.Play();

        yield return new WaitForSeconds(0.4f);

        int nivel = PlayerPrefs.GetInt("NivelCompletado");

        if (nivel == 1)
        {
            SceneManager.LoadScene("Level2");
        }
        else if (nivel == 2)
        {
            SceneManager.LoadScene("Level3");
        }
    }


    public void IrAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
