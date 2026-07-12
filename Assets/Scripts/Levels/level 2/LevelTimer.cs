using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class LevelTimer : MonoBehaviour
{
    
    [Header("Timer")]
    [SerializeField] private TMP_Text clockText;
    [SerializeField] private float timeToWin = 180f;

    [Header("Scene")]
    [SerializeField] private string winSceneName = "Win";

    [SerializeField] private int numeroNivel = 1;

    private float elapsedTime = 0f;
    private bool levelFinished = false;

    private void Update()
    {
        if (levelFinished) return;

        elapsedTime += Time.deltaTime;

        UpdateClockText();

        if (elapsedTime >= timeToWin)
        {
            WinLevel();
        }
    }

    private void UpdateClockText()
    {
        int totalSeconds = Mathf.FloorToInt(elapsedTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        clockText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void WinLevel()
    {
        levelFinished = true;

        PlayerPrefs.SetInt("NivelActual", numeroNivel);

        SceneManager.LoadScene(winSceneName);
    }
}

