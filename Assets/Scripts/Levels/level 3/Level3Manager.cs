using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CameraRoomBackground
{
    public string roomID;
    public Sprite backgroundSprite;
}

public class Level3Manager : MonoBehaviour
{
    private Dictionary<Level3Enemy, bool> enemyCameraVisibility = new Dictionary<Level3Enemy, bool>();
    [Header("Tiempo")]
    [SerializeField] private float levelDuration = 180f;
    [SerializeField] private TMP_Text timeText;

    [Header("Batería")]
    [SerializeField] private Slider batterySlider;
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float tabletBatteryDrainPerSecond = 0.75f;
    [SerializeField] private float closedDoorDrainPerSecond = 1.50f;

    [Header("Tablet")]
    [SerializeField] private GameObject tabletPanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject cameraPanel;
    [SerializeField] private TMP_Text cameraRoomText;
    [SerializeField] private GameObject soundButtonObject;
    [SerializeField] private string spawnRoomID = "Spawn";

    [Header("Fondos de cámara")]
    [SerializeField] private Image cameraBackgroundImage;
    [SerializeField] private CameraRoomBackground[] cameraBackgrounds;


    [Header("Puertas")]
    [SerializeField] private RectTransform leftDoorVisual;
    [SerializeField] private RectTransform rightDoorVisual;
    [SerializeField] private float doorAnimationTime = 0.6f;

    [SerializeField] private Vector2 leftDoorOpenPosition;
    [SerializeField] private Vector2 leftDoorClosedPosition;

    [SerializeField] private Vector2 rightDoorOpenPosition;
    [SerializeField] private Vector2 rightDoorClosedPosition;

    private bool isLeftDoorMoving;
    private bool isRightDoorMoving;
    private Coroutine leftDoorCoroutine;
    private Coroutine rightDoorCoroutine;

    [Header("Luces")]
    [SerializeField] private GameObject leftLightVisual;
    [SerializeField] private GameObject rightLightVisual;
    [SerializeField] private GameObject leftEnemyInHallwayVisual;
    [SerializeField] private GameObject rightEnemyInHallwayVisual;

    [Header("Generador")]
    [SerializeField] private GameObject generatorAlertPanel;
    [SerializeField] private Image generatorRepairProgressImage;
    [SerializeField] private float generatorRepairTime = 4f;
    [SerializeField] private float generatorDeathTime = 10f;

    [Header("Enemigos")]
    [SerializeField] private Level3Enemy[] enemies;
    [SerializeField] private float hallwayAttackTime = 7f;

    [Header("Muerte visual")]
    [SerializeField] private float jumpscareDuration = 1.5f;

    private float currentTime;
    private float currentBattery;
    private float generatorDamageTimer;

    private bool isTabletOpen;
    private bool isLeftDoorClosed;
    private bool isRightDoorClosed;
    private bool isLeftLightOn;
    private bool isRightLightOn;
    private bool isGeneratorDamaged;
    private bool isRepairingGenerator;
    private bool isLevelFinished;
    private bool isJumpscareActive;

    private string currentCameraRoomID;
    private Level3Enemy enemyAtGenerator;

    public float HallwayAttackTime => hallwayAttackTime;

    private void Start()
    {
        currentTime = 0f;
        currentBattery = maxBattery;

        if (leftDoorVisual != null)
        {
            leftDoorVisual.anchoredPosition = leftDoorOpenPosition;
        }

        if (rightDoorVisual != null)
        {
            rightDoorVisual.anchoredPosition = rightDoorOpenPosition;
        }

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
        }

        if (cameraPanel != null)
        {
            cameraPanel.SetActive(false);
        }

        if (mapPanel != null)
        {
            mapPanel.SetActive(true);
        }

        if (generatorAlertPanel != null)
        {
            generatorAlertPanel.SetActive(false);
        }

        if (generatorRepairProgressImage != null)
        {
            generatorRepairProgressImage.fillAmount = 0f;
            generatorRepairProgressImage.gameObject.SetActive(false);
        }

        if (leftEnemyInHallwayVisual != null)
        {
            leftEnemyInHallwayVisual.SetActive(false);
        }

        if (rightEnemyInHallwayVisual != null)
        {
            rightEnemyInHallwayVisual.SetActive(false);
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].Initialize(this);
            enemyCameraVisibility[enemies[i]] = false;
        }

        UpdateTimeUI();
        UpdateBatteryUI();
        UpdateLightVisuals();
    }

    private void Update()
    {
        if (isLevelFinished)
        {
            return;
        }

        UpdateLevelTimer();
        UpdateBatteryDrain();
        UpdateGeneratorDamageTimer();
        UpdateEnemies();
        UpdateEnemyHallwayVisuals();

        if (isTabletOpen && cameraPanel != null && cameraPanel.activeSelf)
        {
            UpdateCameraEnemyVisuals();
        }
    }

    private void UpdateLevelTimer()
    {
        currentTime += Time.deltaTime;

        UpdateTimeUI();

        if (currentTime >= levelDuration)
        {
            WinGame();
        }
    }

    public void StartEnemyJumpscare(Level3Enemy enemy, string reason)
    {
        if (isLevelFinished || isJumpscareActive)
        {
            return;
        }

        StartCoroutine(EnemyJumpscareRoutine(enemy, reason));
    }

    private IEnumerator EnemyJumpscareRoutine(Level3Enemy enemy, string reason)
    {
        isJumpscareActive = true;
        isLevelFinished = true;

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
        }

        if (enemy.JumpscareVisual != null)
        {
            enemy.JumpscareVisual.SetActive(true);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.Screamer();
        }

        Debug.Log("Jumpscare de " + enemy.EnemyName);

        yield return new WaitForSeconds(jumpscareDuration);

        Debug.Log("Game Over: " + reason);
        ButtonSounds.GuardarNivelActual();
        SceneManager.LoadScene("GameOver");
    }

    private void UpdateCameraBackground(string roomID)
    {
        if (cameraBackgroundImage == null)
        {
            return;
        }

        for (int i = 0; i < cameraBackgrounds.Length; i++)
        {
            if (cameraBackgrounds[i].roomID == roomID)
            {
                cameraBackgroundImage.sprite = cameraBackgrounds[i].backgroundSprite;
                cameraBackgroundImage.enabled = true;
                return;
            }
        }

        cameraBackgroundImage.sprite = null;
        cameraBackgroundImage.enabled = false;

        Debug.Log("No hay fondo asignado para la cámara: " + roomID);
    }

    private bool CanUseDoors()
    {
        if (IsBatteryEmpty())
        {
            return false;
        }

        if (isGeneratorDamaged)
        {
            return false;
        }

        return true;
    }

    private void UpdateBatteryDrain()
    {
        if (IsBatteryEmpty())
        {
            return;
        }

        if (isTabletOpen)
        {
            ConsumeBattery(tabletBatteryDrainPerSecond * Time.deltaTime);
        }

        if (isLeftDoorClosed)
        {
            ConsumeBattery(closedDoorDrainPerSecond * Time.deltaTime);
        }

        if (isRightDoorClosed)
        {
            ConsumeBattery(closedDoorDrainPerSecond * Time.deltaTime);
        }

        if (IsBatteryEmpty())
        {
            ForcePowerOffActions();
        }
    }

    public bool IsDoorClosed(DoorSide side)
    {
        if (side == DoorSide.Left)
        {
            return isLeftDoorClosed;
        }

        if (side == DoorSide.Right)
        {
            return isRightDoorClosed;
        }

        return false;
    }

    private void UpdateCameraEnemyVisuals()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].CameraVisual == null)
                continue;

            bool shouldShow = enemies[i].IsInRoom(currentCameraRoomID);

            bool wasVisible = enemyCameraVisibility[enemies[i]];

            if (shouldShow != wasVisible)
            {

                enemyCameraVisibility[enemies[i]] = shouldShow;
            }

            enemies[i].CameraVisual.SetActive(shouldShow);
        }
    }

    private void HideAllCameraEnemyVisuals()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].CameraVisual != null)
            {
                enemies[i].CameraVisual.SetActive(false);
            }
        }
    }

    private void UpdateGeneratorDamageTimer()
    {
        if (!isGeneratorDamaged || isJumpscareActive)
        {
            return;
        }

        generatorDamageTimer -= Time.deltaTime;

        if (generatorDamageTimer <= 0f)
        {
            StartEnemyJumpscare(enemyAtGenerator, "El animatrónico del generador mató al jugador.");
        }
    }



    private void UpdateEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].Tick(Time.deltaTime);
        }
    }
    
    private void ConsumeBattery(float amount)
    {
        currentBattery -= amount;

        if (currentBattery < 0f)
        {
            currentBattery = 0f;
        }

        UpdateBatteryUI();
    }

    private bool IsBatteryEmpty()
    {
        return currentBattery <= 0f;
    }

    private void ForcePowerOffActions()
    {
        isTabletOpen = false;

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
        }

        ForceOpenDoors();

        Debug.Log("La batería se agotó. Las puertas se abrieron y ya no puedes usar tablet ni puertas.");
    }

    public void OpenTablet()
    {
        if (isLevelFinished)
        {
            return;
        }

        if (isGeneratorDamaged)
        {
            Debug.Log("No puedes usar la tablet mientras el generador esté dañado.");
            return;
        }

        if (IsBatteryEmpty())
        {
            Debug.Log("No hay batería para abrir la tablet.");
            return;
        }

        isTabletOpen = true;

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(true);
        }

        if (mapPanel != null)
        {
            mapPanel.SetActive(true);
        }

        if (cameraPanel != null)
        {
            cameraPanel.SetActive(false);
        }
    }

    public void CloseTablet()
    {
        isTabletOpen = false;

        HideAllCameraEnemyVisuals();

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
        }
    }

    public void OpenCameraRoom(string roomID)
    {
        if (!isTabletOpen)
        {
            return;
        }

        if (IsBatteryEmpty())
        {
            CloseTablet();
            return;
        }

        currentCameraRoomID = roomID;

        if (mapPanel != null)
        {
            mapPanel.SetActive(false);
        }

        if (cameraPanel != null)
        {
            cameraPanel.SetActive(true);
        }

        if (cameraRoomText != null)
        {
            cameraRoomText.text = "Cámara: " + roomID;
        }

        if (soundButtonObject != null)
        {
            soundButtonObject.SetActive(roomID != spawnRoomID);
        }
        UpdateCameraBackground(roomID);
        UpdateCameraEnemyVisuals();

        Debug.Log("Viendo cámara de habitación: " + roomID);
    }

    public void BackToMap()
    {
        if (!isTabletOpen)
        {
            return;
        }

        HideAllCameraEnemyVisuals();

        if (mapPanel != null)
        {
            mapPanel.SetActive(true);
        }

        if (cameraPanel != null)
        {
            cameraPanel.SetActive(false);
        }
    }

    public void PlaySoundInCurrentCamera()
    {
        if (!isTabletOpen)
        {
            return;
        }

        if (string.IsNullOrEmpty(currentCameraRoomID))
        {
            return;
        }

        if (currentCameraRoomID == spawnRoomID)
        {
            Debug.Log("No se puede usar sonido en la habitación Spawn.");
            return;
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].CanMoveToRoomBySound(currentCameraRoomID))
            {
                enemies[i].MoveToRoomBySound(currentCameraRoomID);
                Debug.Log("Sonido usado en: " + currentCameraRoomID);
                return;
            }
        }

        Debug.Log("Sonido usado en " + currentCameraRoomID + ", pero ningún animatrónico puede ir a esa habitación.");
    }

    public void ToggleLeftDoor()
    {
        if (isLevelFinished)
        {
            return;
        }

        if (!CanUseDoors())
        {
            Debug.Log("No puedes usar la puerta izquierda ahora.");
            return;
        }

        if (isLeftDoorMoving)
        {
            return;
        }

        leftDoorCoroutine = StartCoroutine(AnimateLeftDoor());
    }

    public void ToggleRightDoor()
    {
        if (isLevelFinished)
        {
            return;
        }

        if (!CanUseDoors())
        {
            Debug.Log("No puedes usar la puerta derecha ahora.");
            return;
        }

        if (isRightDoorMoving)
        {
            return;
        }

        rightDoorCoroutine = StartCoroutine(AnimateRightDoor());
    }

    private IEnumerator AnimateLeftDoor()
    {
        isLeftDoorMoving = true;

        bool closing = !isLeftDoorClosed;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.Puerta();
        }

        Vector2 startPosition = leftDoorVisual.anchoredPosition;
        Vector2 targetPosition = closing ? leftDoorClosedPosition : leftDoorOpenPosition;

        if (closing)
        {
            isLeftDoorClosed = true;
        }

        float elapsedTime = 0f;

        while (elapsedTime < doorAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            float percent = elapsedTime / doorAnimationTime;

            leftDoorVisual.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, percent);

            yield return null;
        }

        leftDoorVisual.anchoredPosition = targetPosition;

        if (!closing)
        {
            isLeftDoorClosed = false;
        }
        else
        {
            PushBackEnemyInHallway(DoorSide.Left);
        }

        isLeftDoorMoving = false;
    }

    private IEnumerator AnimateRightDoor()
    {
        isRightDoorMoving = true;

        bool closing = !isRightDoorClosed;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.Puerta();
        }

        Vector2 startPosition = rightDoorVisual.anchoredPosition;
        Vector2 targetPosition = closing ? rightDoorClosedPosition : rightDoorOpenPosition;

        if (closing)
        {
            isRightDoorClosed = true;
        }

        float elapsedTime = 0f;

        while (elapsedTime < doorAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            float percent = elapsedTime / doorAnimationTime;

            rightDoorVisual.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, percent);

            yield return null;
        }

        rightDoorVisual.anchoredPosition = targetPosition;

        if (!closing)
        {
            isRightDoorClosed = false;
        }
        else
        {
            PushBackEnemyInHallway(DoorSide.Right);
        }

        isRightDoorMoving = false;
    }

    private void ForceOpenDoors()
    {
        ForceOpenLeftDoor();
        ForceOpenRightDoor();
    }

    private void ForceOpenLeftDoor()
    {
        if (leftDoorVisual == null)
        {
            return;
        }

        if (leftDoorCoroutine != null)
        {
            StopCoroutine(leftDoorCoroutine);
        }

        leftDoorCoroutine = StartCoroutine(ForceOpenLeftDoorRoutine());
    }

    private void ForceOpenRightDoor()
    {
        if (rightDoorVisual == null)
        {
            return;
        }

        if (rightDoorCoroutine != null)
        {
            StopCoroutine(rightDoorCoroutine);
        }

        rightDoorCoroutine = StartCoroutine(ForceOpenRightDoorRoutine());
    }

    private IEnumerator ForceOpenLeftDoorRoutine()
    {
        isLeftDoorMoving = true;
        isLeftDoorClosed = false;

        Vector2 startPosition = leftDoorVisual.anchoredPosition;
        Vector2 targetPosition = leftDoorOpenPosition;

        float elapsedTime = 0f;

        while (elapsedTime < doorAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            float percent = elapsedTime / doorAnimationTime;

            leftDoorVisual.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, percent);

            yield return null;
        }

        leftDoorVisual.anchoredPosition = targetPosition;
        isLeftDoorMoving = false;
    }

    private IEnumerator ForceOpenRightDoorRoutine()
    {
        isRightDoorMoving = true;
        isRightDoorClosed = false;

        Vector2 startPosition = rightDoorVisual.anchoredPosition;
        Vector2 targetPosition = rightDoorOpenPosition;

        float elapsedTime = 0f;

        while (elapsedTime < doorAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            float percent = elapsedTime / doorAnimationTime;

            rightDoorVisual.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, percent);

            yield return null;
        }

        rightDoorVisual.anchoredPosition = targetPosition;
        isRightDoorMoving = false;
    }

    public void ToggleLeftLight()
    {
        if (isGeneratorDamaged)
        {
            Debug.Log("No puedes usar la linterna.");
            return;
        }

        isLeftLightOn = !isLeftLightOn;

        if (isLeftLightOn)
        {
            bool enemyInHallway = false;

            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].IsInDoorHallway(DoorSide.Left))
                {
                    enemyInHallway = true;
                    break;
                }
            }

            if (AudioManager.instance != null)
            {
                if (enemyInHallway)
                    AudioManager.instance.LinternaAnimatronico();
                else
                    AudioManager.instance.LinternaNormal();
            }
        }

        UpdateLightVisuals();
    }

    public void ToggleRightLight()
    {
        if (isGeneratorDamaged)
        {
            Debug.Log("No puedes usar la linterna.");
            return;
        }

        isRightLightOn = !isRightLightOn;

        if (isRightLightOn)
        {
            bool enemyInHallway = false;

            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].IsInDoorHallway(DoorSide.Right))
                {
                    enemyInHallway = true;
                    break;
                }
            }

            if (AudioManager.instance != null)
            {
                if (enemyInHallway)
                    AudioManager.instance.LinternaAnimatronico();
                else
                    AudioManager.instance.LinternaNormal();
            }
        }

        UpdateLightVisuals();
    }

    private void PushBackEnemyInHallway(DoorSide side)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].IsInDoorHallway(side))
            {
                enemies[i].RetreatFromHallway();
            }
        }
    }

    public void OnEnemyEnteredHallway(Level3Enemy enemy, DoorSide side)
    {
        Debug.Log(enemy.EnemyName + " está en el pasillo de la puerta " + side);

        if (IsDoorClosed(side))
        {
            Debug.Log(enemy.EnemyName + " encontró la puerta cerrada y retrocedió.");
            enemy.RetreatFromHallway();
            UpdateEnemyHallwayVisuals();
            return;
        }

        UpdateEnemyHallwayVisuals();
    }

    private void UpdateEnemyHallwayVisuals()
    {
        bool enemyInLeftHallway = false;
        bool enemyInRightHallway = false;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].IsInDoorHallway(DoorSide.Left))
            {
                enemyInLeftHallway = true;
            }

            if (enemies[i].IsInDoorHallway(DoorSide.Right))
            {
                enemyInRightHallway = true;
            }
        }

        if (leftEnemyInHallwayVisual != null)
        {
            leftEnemyInHallwayVisual.SetActive(isLeftLightOn && enemyInLeftHallway);
        }

        if (rightEnemyInHallwayVisual != null)
        {
            rightEnemyInHallwayVisual.SetActive(isRightLightOn && enemyInRightHallway);
        }
    }

    public void OnEnemyDamagedGenerator(Level3Enemy enemy)
    {
        if (isGeneratorDamaged)
        {
            return;
        }

        enemyAtGenerator = enemy;
        isGeneratorDamaged = true;
        generatorDamageTimer = generatorDeathTime;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.GeneradorDañado();
        }

        ForceOpenDoors();
        CloseTablet();
        isLeftLightOn = false;
        isRightLightOn = false;
        UpdateLightVisuals();

        if (generatorAlertPanel != null)
        {
            generatorAlertPanel.SetActive(true);
        }

        Debug.Log("Generador dañado. Las puertas se abrieron. Tienes " + generatorDeathTime + " segundos para repararlo.");
    }

    public void RepairGenerator()
    {
        if (!isGeneratorDamaged)
        {
            Debug.Log("El generador no está dañado.");
            return;
        }

        if (isRepairingGenerator)
        {
            Debug.Log("Ya estás reparando el generador.");
            return;
        }

        StartCoroutine(RepairGeneratorRoutine());
    }

    private IEnumerator RepairGeneratorRoutine()
    {
        isRepairingGenerator = true;

        if (generatorRepairProgressImage != null)
        {
            generatorRepairProgressImage.fillAmount = 0f;
            generatorRepairProgressImage.gameObject.SetActive(true);
        }

        float elapsedTime = 0f;

        while (elapsedTime < generatorRepairTime)
        {
            elapsedTime += Time.deltaTime;

            if (generatorRepairProgressImage != null)
            {
                generatorRepairProgressImage.fillAmount = elapsedTime / generatorRepairTime;
            }

            yield return null;
        }

        isRepairingGenerator = false;
        isGeneratorDamaged = false;

        if (generatorRepairProgressImage != null)
        {
            generatorRepairProgressImage.fillAmount = 0f;
            generatorRepairProgressImage.gameObject.SetActive(false);
        }

        if (generatorAlertPanel != null)
        {
            generatorAlertPanel.SetActive(false);
        }

        if (enemyAtGenerator != null)
        {
            enemyAtGenerator.RetreatAfterGeneratorRepair();
            enemyAtGenerator = null;
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.GeneradorReparado();
        }

        Debug.Log("Generador reparado.");
    }

    private void UpdateLightVisuals()
    {
        if (leftLightVisual != null)
        {
            leftLightVisual.SetActive(isLeftLightOn);
        }

        if (rightLightVisual != null)
        {
            rightLightVisual.SetActive(isRightLightOn);
        }

        UpdateEnemyHallwayVisuals();
    }

    private void UpdateTimeUI()
    {
        if (timeText == null)
        {
            return;
        }

        float gameMinutes = Mathf.Lerp(240f, 360f, currentTime / levelDuration);

        int hours = Mathf.FloorToInt(gameMinutes / 60f);
        int minutes = Mathf.FloorToInt(gameMinutes % 60f);

        timeText.text = hours.ToString("00") + ":" + minutes.ToString("00");
    }

    private void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.maxValue = maxBattery;
            batterySlider.value = currentBattery;
        }
    }

    private void WinGame()
    {
        isLevelFinished = true;

        Debug.Log("Ganaste el juego.");

        PlayerPrefs.SetInt("NivelCompletado", 3);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Win");
    }

    public void LoseLevel(string reason)
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
}
