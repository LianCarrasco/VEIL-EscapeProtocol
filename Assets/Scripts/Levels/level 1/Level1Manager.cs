using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class Level1Manager : MonoBehaviour
{
    [Header("Tiempo del nivel")]
    [SerializeField] private float levelDuration = 180f;
    [SerializeField] private TMP_Text timeText;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialPanel;

    [Header("Tablet")]
    [SerializeField] private GameObject tabletPanel;
    [SerializeField] private GameObject noPowerMessagePanel;

    [Header("Tareas")]
    [SerializeField] private Level1TaskButton[] taskButtons;

    [Header("Temperatura")]
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private float maxTemperature = 100f;
    [SerializeField] private float temperatureRiseSpeed = 6f;
    [SerializeField] private float temperatureCoolSpeed = 25f;
    [SerializeField] private float minHeatEventTime = 8f;
    [SerializeField] private float maxHeatEventTime = 18f;

    [Header("Generador")]
    [SerializeField] private float minPowerFailTime = 15f;
    [SerializeField] private float maxPowerFailTime = 35f;
    [SerializeField] private float generatorRepairTime = 3f;
    [SerializeField] private Image generatorProgressImage;
    [SerializeField] private GameObject repairGeneratorButton;
    [SerializeField] private GameObject lightOffImage;

    private bool isRepairingGenerator;

    [Header("Estado")]
    [SerializeField] private bool levelStarted = false;

    private float currentTime;
    private float currentTemperature;
    private float nextHeatEventTimer;
    private float nextPowerFailTimer;

    private bool isTabletOpen;
    private bool isPowerOn = true;
    private bool isCoolingTemperature;
    private bool isLevelFinished;

    private Level1TaskButton currentChargingTask;

    private void Start()
    {
        currentTime = 0f;
        currentTemperature = 0f;

        ScheduleNextHeatEvent();
        ScheduleNextPowerFail();

        levelStarted = false;

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        if (lightOffImage != null)
        {
            lightOffImage.SetActive(false);
        }

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
        }

        if (repairGeneratorButton != null)
        {
            repairGeneratorButton.SetActive(false);
        }

        if (noPowerMessagePanel != null)
        {
            noPowerMessagePanel.SetActive(false);
        }
        
        if (generatorProgressImage != null)
        {
            generatorProgressImage.fillAmount = 0f;
            generatorProgressImage.gameObject.SetActive(false);
        }

        for (int i = 0; i < taskButtons.Length; i++)
        {
            taskButtons[i].Initialize(this);
        }

        UpdateTimeUI();
        UpdateTemperatureUI();
    }

    private void Update()
    {
        if (!levelStarted || isLevelFinished)
        {
            return;
        }

        UpdateLevelTimer();
        UpdateTemperatureSystem();
        UpdateGeneratorSystem();
    }

    public void StartLevelFromTutorial()
    {
        levelStarted = true;

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        Debug.Log("Nivel 1 iniciado después del tutorial.");
    }

    private void UpdateLevelTimer()
    {
        currentTime += Time.deltaTime;

        UpdateTimeUI();

        if (currentTime >= levelDuration)
        {
            LoseLevel("Se acabó el tiempo.");
        }
    }

    private void UpdateTemperatureSystem()
    {
        nextHeatEventTimer -= Time.deltaTime;

        if (nextHeatEventTimer <= 0f)
        {
            currentTemperature += temperatureRiseSpeed * Time.deltaTime;
        }

        if (isCoolingTemperature)
        {
            currentTemperature -= temperatureCoolSpeed * Time.deltaTime;

            if (currentTemperature <= 0f)
            {
                currentTemperature = 0f;
                isCoolingTemperature = false;
                ScheduleNextHeatEvent();
            }
        }

        if (currentTemperature >= maxTemperature)
        {
            currentTemperature = maxTemperature;
            LoseLevel("La temperatura llegó al máximo.");
        }

        UpdateTemperatureUI();
    }

    private void UpdateGeneratorSystem()
    {
        if (!isPowerOn)
        {
            return;
        }

        nextPowerFailTimer -= Time.deltaTime;

        if (nextPowerFailTimer <= 0f)
        {
            TurnPowerOff();
        }
    }

    private void ScheduleNextHeatEvent()
    {
        nextHeatEventTimer = Random.Range(minHeatEventTime, maxHeatEventTime);
    }

    private void ScheduleNextPowerFail()
    {
        nextPowerFailTimer = Random.Range(minPowerFailTime, maxPowerFailTime);
    }

    public void OpenTablet()
    {
        if (isLevelFinished)
        {
            return;
        }

        if (!isPowerOn)
        {
            ShowNoPowerMessage();
            return;
        }

        isTabletOpen = true;

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(true);
        }

        if (noPowerMessagePanel != null)
        {
            noPowerMessagePanel.SetActive(false);
        }
    }

    public void CloseTablet()
    {
        isTabletOpen = false;

        CancelCurrentChargingTask();

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
        }
    }

    public void TryStartTask(Level1TaskButton task)
    {
        if (isLevelFinished)
        {
            return;
        }

        if (!isTabletOpen)
        {
            return;
        }

        if (!isPowerOn)
        {
            return;
        }

        if (task.IsCompleted)
        {
            return;
        }

        if (currentChargingTask != null)
        {
            return;
        }

        currentChargingTask = task;
        task.StartCharge();
    }

    public void OnTaskCompleted(Level1TaskButton task)
    {
        if (currentChargingTask == task)
        {
            currentChargingTask = null;
        }

        CheckWinCondition();
    }

    private void CancelCurrentChargingTask()
    {
        if (currentChargingTask != null)
        {
            currentChargingTask.CancelCharge();
            currentChargingTask = null;
        }
    }

    public void UseTemperatureController()
    {
        if (isLevelFinished)
        {
            return;
        }

        isCoolingTemperature = true;
        Debug.Log("Controlador de temperatura activado.");
    }

    private void TurnPowerOff()
    {
        isPowerOn = false;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.GeneradorDanado();
        }

        CancelCurrentChargingTask();

        if (isTabletOpen)
        {
            isTabletOpen = false;

            if (tabletPanel != null)
            {
                tabletPanel.SetActive(false);
            }
        }

        if (repairGeneratorButton != null)
        {
            repairGeneratorButton.SetActive(true);
        }

        if (lightOffImage != null)
        {
            lightOffImage.SetActive(true);
        }

        Debug.Log("El generador se apagó.");
    }

    public void RepairGenerator()
    {
        if (isLevelFinished)
        {
            return;
        }

        if (isPowerOn)
        {
            Debug.Log("El generador ya está funcionando.");
            return;
        }

        if (isRepairingGenerator)
        {
            Debug.Log("El generador ya se está reparando.");
            return;
        }

        StartCoroutine(RepairGeneratorRoutine());
    }

    private IEnumerator RepairGeneratorRoutine()
    {
        isRepairingGenerator = true;

        if (generatorProgressImage != null)
        {
            generatorProgressImage.fillAmount = 0f;
            generatorProgressImage.gameObject.SetActive(true);
        }

        Debug.Log("Reparando generador...");

        float elapsedTime = 0f;

        while (elapsedTime < generatorRepairTime)
        {
            elapsedTime += Time.deltaTime;

            if (generatorProgressImage != null)
            {
                generatorProgressImage.fillAmount = elapsedTime / generatorRepairTime;
            }

            yield return null;
        }

        isPowerOn = true;
        isRepairingGenerator = false;

        if (repairGeneratorButton != null)
        {
            repairGeneratorButton.SetActive(false);
        }

        if (generatorProgressImage != null)
        {
            generatorProgressImage.fillAmount = 0f;
            generatorProgressImage.gameObject.SetActive(false);
        }

        if (lightOffImage != null)
        {
            lightOffImage.SetActive(false);
        }

        if (noPowerMessagePanel != null)
        {
            noPowerMessagePanel.SetActive(false);
        }

        ScheduleNextPowerFail();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.GeneradorReparado();
        }

        Debug.Log("Generador reparado. La luz volvió.");
    }

    private void ShowNoPowerMessage()
    {
        if (noPowerMessagePanel != null)
        {
            noPowerMessagePanel.SetActive(true);
        }

        Debug.Log("No hay luz. Repara el generador.");
    }

    private void CheckWinCondition()
    {
        for (int i = 0; i < taskButtons.Length; i++)
        {
            if (!taskButtons[i].IsCompleted)
            {
                return;
            }
        }

        WinLevel();
    }

    private void WinLevel()
    {
        isLevelFinished = true;

        Debug.Log("Nivel 1 completado.");

        PlayerPrefs.SetInt("NivelCompletado", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Win");
    }

    private void LoseLevel(string reason)
    {
        if (isLevelFinished)
        {
            return;
        }

        isLevelFinished = true;

        Debug.Log("Game Over: " + reason);
        ButtonSounds.GuardarNivelActual();
        SceneManager.LoadScene("GameOver");
    }

    private void UpdateTimeUI()
    {
        if (timeText == null)
        {
            return;
        }

        float gameMinutes = Mathf.Lerp(0f, 120f, currentTime / levelDuration);

        int hours = Mathf.FloorToInt(gameMinutes / 60f);
        int minutes = Mathf.FloorToInt(gameMinutes % 60f);

        timeText.text = hours.ToString("00") + ":" + minutes.ToString("00");
    }

    private void UpdateTemperatureUI()
    {
        if (temperatureSlider != null)
        {
            temperatureSlider.maxValue = maxTemperature;
            temperatureSlider.value = currentTemperature;
        }
    }
}
