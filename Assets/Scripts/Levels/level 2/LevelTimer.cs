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
        float progreso = elapsedTime / timeToWin;

        // 3 minutos reales = de 02:00 a 04:00
        int minutosTotales = Mathf.FloorToInt(120f + (progreso * 120f));

        int horas = minutosTotales / 60;
        int minutos = minutosTotales % 60;

        clockText.text = horas.ToString("00") + ":" + minutos.ToString("00");
    }

    private void WinLevel()
    {
        levelFinished = true;

        SceneManager.LoadScene(winSceneName);
    }
}

