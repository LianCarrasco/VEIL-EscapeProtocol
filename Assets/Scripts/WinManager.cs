using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinManager : MonoBehaviour
{
    public Button siguienteNivel;

    public string menuPrincipal = "MainMenu";

    void Start()
    {
        int nivelActual = PlayerPrefs.GetInt("NivelActual", 1);

        if (nivelActual >= 3)
        {
            siguienteNivel.gameObject.SetActive(false);
        }
    }

    public void IrMenu()
    {
        SceneManager.LoadScene(menuPrincipal);
    }

    public void SiguienteNivel()
    {
        int nivelActual = PlayerPrefs.GetInt("NivelActual", 1);

        if (nivelActual == 1)
            SceneManager.LoadScene("Level2");
        else if (nivelActual == 2)
            SceneManager.LoadScene("Level3");
    }
}
