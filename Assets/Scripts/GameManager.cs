using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static string ultimoNivel;

    public void Jugar()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Salir()
    {
        Application.Quit();
    }

    public static void GuardarNivelActual()
    {
        ultimoNivel = SceneManager.GetActiveScene().name;
        Debug.Log("Nivel guardado: " + ultimoNivel);
    }

    public void Reintentar()
    {
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

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
