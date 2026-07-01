using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level1TaskButton : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int taskID;
    [SerializeField] private float chargeTime = 5f;

    [Header("Referencias UI")]
    [SerializeField] private Button button;
    [SerializeField] private Image progressImage;
    [SerializeField] private TMP_Text buttonText;

    [Header("Colores del botón")]
    [SerializeField] private Color pendingColor = new Color(0.8f, 0.1f, 0.1f);
    [SerializeField] private Color chargingColor = new Color(1f, 0.8f, 0.1f);
    [SerializeField] private Color completedColor = new Color(0.1f, 0.8f, 0.2f);

    private Image buttonImage;
    private float currentProgress;
    private bool isCharging;
    private bool isCompleted;
    private Coroutine chargeCoroutine;

    public int TaskID => taskID;
    public bool IsCharging => isCharging;
    public bool IsCompleted => isCompleted;

    private Level1Manager levelManager;

    public void Initialize(Level1Manager manager)
    {
        levelManager = manager;

        if (button != null)
        {
            buttonImage = button.GetComponent<Image>();
            button.onClick.AddListener(OnButtonPressed);
        }

        ResetVisual();
    }

    private void OnButtonPressed()
    {
        if (levelManager != null)
        {
            levelManager.TryStartTask(this);
        }
    }

    public void StartCharge()
    {
        if (isCompleted || isCharging)
        {
            return;
        }

        isCharging = true;
        currentProgress = 0f;

        if (buttonImage != null)
        {
            buttonImage.color = chargingColor;
        }

        if (progressImage != null)
        {
            progressImage.fillAmount = 0f;
            progressImage.gameObject.SetActive(true);
        }

        if (buttonText != null)
        {
            buttonText.text = "CARGANDO...";
        }

        if (chargeCoroutine != null)
        {
            StopCoroutine(chargeCoroutine);
        }

        chargeCoroutine = StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        while (currentProgress < chargeTime)
        {
            currentProgress += Time.deltaTime;

            float percent = currentProgress / chargeTime;

            if (progressImage != null)
            {
                progressImage.fillAmount = percent;
            }

            yield return null;
        }

        CompleteTask();
    }

    public void CancelCharge()
    {
        if (isCompleted)
        {
            return;
        }

        if (chargeCoroutine != null)
        {
            StopCoroutine(chargeCoroutine);
        }

        isCharging = false;
        currentProgress = 0f;

        ResetVisual();
    }

    private void CompleteTask()
    {
        isCharging = false;
        isCompleted = true;
        currentProgress = chargeTime;

        if (progressImage != null)
        {
            progressImage.fillAmount = 1f;
        }

        if (buttonImage != null)
        {
            buttonImage.color = completedColor;
        }

        if (buttonText != null)
        {
            buttonText.text = "ACTIVADO";
        }

        if (levelManager != null)
        {
            levelManager.OnTaskCompleted(this);
        }
    }

    private void ResetVisual()
    {
        if (progressImage != null)
        {
            progressImage.fillAmount = 0f;
            progressImage.gameObject.SetActive(true);
        }

        if (buttonImage != null)
        {
            buttonImage.color = pendingColor;
        }

        if (buttonText != null)
        {
            buttonText.text = "INICIAR";
        }

        if (button != null)
        {
            button.interactable = true;
        }
    }
}
