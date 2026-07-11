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
    }

    public void Reintentar()
    {
        SceneManager.LoadScene(ultimoNivel);
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
