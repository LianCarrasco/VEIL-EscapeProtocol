using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public enum AttackView
{
    None,
    Left,
    Right,
    Back,
    Systems
}
[System.Serializable]
public class Animatronic
{
    public string name;
    public Color sensorColor;

    [Header("Tramo comun (camino compartido)")]
    public int[] route;
    public AttackView attackView; // Usado si NO tiene bifurcacion (ej: Green)
    public float moveTime = 8f;

    [Header("Bifurcacion (opcional)")]
    [Tooltip("Si esta activo, al terminar el tramo comun el animatronico elige aleatoriamente entre el Camino A y el Camino B")]

    public bool hasBranch = false;
    [Tooltip("Continuacion de camaras despues del tramo comun, para el Camino A")]

    public int[] routeContinuationA;

    public AttackView attackViewA = AttackView.None;
    [Tooltip("Continuacion de camaras despues del tramo comun, para el Camino B")]

    public int[] routeContinuationB;

    public AttackView attackViewB = AttackView.None;
    [Range(0f, 1f)]
    [Tooltip("Probabilidad de tomar el Camino A. El resto de probabilidad es para el Camino B")]

    public float branchChanceA = 0.5f;

    [Header("Ataque")]
    [Tooltip("Segundos que tiene el jugador para reaccionar una vez que este animatronico empieza a atacar, antes de morir")]

    public float timeToReact = 6f;

    [Header("Screamer propio")]
    [Tooltip("Panel/Imagen que se muestra si ESTE animatronico es el que te mata")]

    public GameObject screamerPanel;
    [Tooltip("Sonido que se reproduce si ESTE animatronico es el que te mata")]

    public AudioClip screamerSound;
    [Header("Visual de 'asomado' en la puerta")]
    [Tooltip("Sprite/GameObject que se activa cuando este animatronico esta atacando desde la vista IZQUIERDA")]
    public GameObject peekVisualLeft;
    [Tooltip("Sprite/GameObject que se activa cuando este animatronico esta atacando desde la vista DERECHA")]
    public GameObject peekVisualRight;
    [Tooltip("Sprite/GameObject que se activa cuando este animatronico esta atacando desde la vista TRASERA")]
    public GameObject peekVisualBack;

    [HideInInspector] public bool takingBranch = false;
    [HideInInspector] public bool usingBranchA = false;
    [HideInInspector] public int branchIndex = 0;
    [HideInInspector] public int routeIndex = 0;
    [HideInInspector] public float moveTimer = 0f;
    [HideInInspector] public float attackTimer = 0f;
    [HideInInspector] public bool attacking = false;
}

public class AnimatronicManager : MonoBehaviour
{
    private AudioManager audioManager;

    [Header("Vista actual del jugador")]
    [SerializeField] private ViewController viewController;

    [Header("Animatronicos")]
    [SerializeField] private Animatronic greenAnimatronic;
    [SerializeField] private Animatronic blueAnimatronic;

    [Header("Tablet - botones Camara 1 a 9")]
    [SerializeField] private Button[] cameraButtons;

    [Header("Reglas")]
    [SerializeField] private string gameOverSceneName = "GameOver";

    [Header("Victoria")]
    [SerializeField] private float surviveTime = 180f;
    [SerializeField] private string winSceneName = "Win";

    private float timer;
    private bool gameEnded = false;

    [Header("Screamer / Muerte (compartido)")]
    [SerializeField] private float screamerDuration = 1.5f;
    [SerializeField] private AudioSource screamerAudioSource;
    [SerializeField] private GameObject playerCanvasRoot;

    [Header("Cooldown de Sonido")]
    [SerializeField] private float soundCooldown = 15f;
    [SerializeField] private Button soundButton;
    private float soundCooldownTimer = 0f;
    private bool soundOnCooldown = false;

    private int selectedCamera = 1;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color selectedColor = new Color(1f, 1f, 0f, 1f);

    private bool isDying = false;

    private void Start()
    {
        audioManager = AudioManager.instance;

        timer = surviveTime;

        RefreshTabletSensors();

        if (greenAnimatronic.screamerPanel != null)
        {
            greenAnimatronic.screamerPanel.SetActive(false);
        }

        if (blueAnimatronic.screamerPanel != null)
        {
            blueAnimatronic.screamerPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameEnded)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            WinGame();
            return;
        }

        if (isDying) return;

        UpdateAnimatronic(greenAnimatronic);
        UpdateAnimatronic(blueAnimatronic);
        RefreshTabletSensors();
        UpdateSoundCooldown();
        if (viewController != null)
        {
            RoomView currentView = viewController.CurrentView;
            UpdatePeekVisuals(greenAnimatronic, currentView);
            UpdatePeekVisuals(blueAnimatronic, currentView);
        }
    }

    private void UpdatePeekVisuals(Animatronic anim, RoomView currentView)
    {
        SetActiveSafe(anim.peekVisualLeft, false);
        SetActiveSafe(anim.peekVisualRight, false);
        SetActiveSafe(anim.peekVisualBack, false);

        if (!anim.attacking) return;

        bool viewMatches =
        (anim.attackView == AttackView.Left && currentView == RoomView.Left) ||
        (anim.attackView == AttackView.Right && currentView == RoomView.Right) ||
        (anim.attackView == AttackView.Back && currentView == RoomView.Back);

        if (!viewMatches) return;

        switch (anim.attackView)
        {
            case AttackView.Left:
                SetActiveSafe(anim.peekVisualLeft, true);
                break;
            case AttackView.Right:
                SetActiveSafe(anim.peekVisualRight, true);
                break;
            case AttackView.Back:
                SetActiveSafe(anim.peekVisualBack, true);
                break;
        }
    }

    private void SetActiveSafe(GameObject obj, bool active)
    {
        if (obj != null)
        {
            obj.SetActive(active);
        }
    }

    private void PlayFootstepSound(AttackView view)
    {
        if (audioManager == null)
            return;

        switch (view)
        {
            case AttackView.Left:
                audioManager.PasosIzquierda();
                break;

            case AttackView.Right:
                audioManager.PasosDerecha();
                break;

            case AttackView.Back:
                audioManager.PasosAtras();
                break;
        }
    }

    private void UpdateAnimatronic(Animatronic anim)
    {
        if (anim.attacking)
        {
            anim.attackTimer += Time.deltaTime;

            if (anim.attackTimer >= anim.timeToReact)
            {
                TriggerDeath(anim);
            }

            return;
        }
        anim.moveTimer += Time.deltaTime;

        if (anim.moveTimer >= anim.moveTime)
        {
            anim.moveTimer = 0f;
            MoveForward(anim);
        }
    }

    private void MoveForward(Animatronic anim)
    {
        // Si está recorriendo una rama
        if (anim.takingBranch)
        {
            int[] branch = anim.usingBranchA ?
                anim.routeContinuationA :
                anim.routeContinuationB;

            if (branch == null || branch.Length == 0)
            {
                Debug.LogWarning(anim.name + " no tiene una ruta de rama válida.");
                anim.attacking = true;
                anim.attackTimer = 0f;
                return;
            }

            // Si todavía hay cámaras por recorrer
            if (anim.branchIndex < branch.Length - 1)
            {
                anim.branchIndex++;
                return;
            }

            // Llegó al final de la rama
            anim.attacking = true;
            anim.attackTimer = 0f;

            anim.attackView = anim.usingBranchA ?
                anim.attackViewA :
                anim.attackViewB;

            PlayFootstepSound(anim.attackView);

            return;
        }

        // Movimiento normal del camino común
        anim.routeIndex++;

        // Llegó al final del tramo común
        if (anim.routeIndex >= anim.route.Length - 1)
        {
            // Si tiene bifurcación decide ahora
            if (anim.hasBranch)
            {
                anim.takingBranch = true;

                anim.usingBranchA =
                    Random.value < anim.branchChanceA;

                anim.branchIndex = 0;

                Debug.Log(anim.name +
                (anim.usingBranchA ?
                " eligió Camino A (Back)" :
                " eligió Camino B (Left)"));

                return;
            }

            // Si no tiene bifurcación ataca normal
            anim.attacking = true;
            anim.attackTimer = 0f;

            PlayFootstepSound(anim.attackView);
        }
    }

    public void UseFlashlight(RoomView currentView) 
    {
        if (isDying) return;

        ScareIfVisible(greenAnimatronic, currentView);
        ScareIfVisible(blueAnimatronic, currentView);
    }

    private void ScareIfVisible(Animatronic anim, RoomView currentView)
    {
        if (!anim.attacking) return;

        bool correctView =

            anim.attackView == AttackView.Left && currentView == RoomView.Left ||
            anim.attackView == AttackView.Right && currentView == RoomView.Right ||
            anim.attackView == AttackView.Back && currentView == RoomView.Back;

        if (!correctView) return;

        if (audioManager != null)
        {
            PlayFootstepSound(anim.attackView);
        }

        anim.attacking = false;
        anim.attackTimer = 0f;

        anim.routeIndex = 0;

        anim.takingBranch = false;
        anim.branchIndex = 0;
        anim.usingBranchA = false;

        anim.moveTimer = 0f;
    }

    //SONIDO COOLDOWN

    public void PlaySound()
    {
        if (isDying) return;

        if (soundOnCooldown)
        {
            Debug.Log("El sonido esta en cooldown. Espera " + soundCooldownTimer.ToString("F1") + "s");
            return;
        }

        Debug.Log("Sonido activado en Camara " + selectedCamera);

        PullToCamera(greenAnimatronic, selectedCamera);
        PullToCamera(blueAnimatronic, selectedCamera);

        RefreshTabletSensors();

        StartSoundCooldown();
    }

    private void StartSoundCooldown()
    {
        soundOnCooldown = true;
        soundCooldownTimer = soundCooldown;

        if (soundButton != null)
        {
            soundButton.interactable = false;
        }
    }

    private void UpdateSoundCooldown()
    {
        if (!soundOnCooldown) return;

        soundCooldownTimer -= Time.deltaTime;

        if (soundCooldownTimer <= 0f)
        {
            soundOnCooldown = false;
            soundCooldownTimer = 0f;

            if (soundButton != null)
            {
                soundButton.interactable = true;
            }
        }
    }

    public float GetSoundCooldownRemaining()
    {
        return soundOnCooldown ? soundCooldownTimer : 0f;
    }

    //MOVIMIENTO POR SONIDO (con anti-solapamiento)

    private void PullToCamera(Animatronic anim, int cameraNumber)
    {
        if (anim.attacking) return;

        if (IsCameraOccupiedByOther(anim, cameraNumber))
        {
            Debug.Log(anim.name + " no se movio a Camara " + cameraNumber + " porque ya esta ocupada.");
            return;
        }

        for (int i = 0; i < anim.route.Length; i++)
        {
            if (anim.route[i] == cameraNumber)
            {
                anim.routeIndex = i;
                anim.moveTimer = 0f;

                Debug.Log(anim.name + " fue atraido a Camara " + cameraNumber);
                return;
            }
        }

        Debug.Log(anim.name + " no puede ir a Camara " + cameraNumber);
    }

    private bool IsCameraOccupiedByOther(Animatronic mover, int cameraNumber)
    {
        Animatronic other = (mover == greenAnimatronic) ? blueAnimatronic : greenAnimatronic;

        if (other.attacking) return false;
        if (other.route == null || other.route.Length == 0) return false;

        if (other.routeIndex < 0 || other.routeIndex >= other.route.Length)
            return false;

        int otherCamera = other.route[other.routeIndex];
        return otherCamera == cameraNumber;
    }

    // TABLET

    private void RefreshTabletSensors()
    {
        for (int i = 0; i < cameraButtons.Length; i++)
        {
            cameraButtons[i].image.color = normalColor;
        }

        PaintSensor(greenAnimatronic);
        PaintSensor(blueAnimatronic);

        int selectedIndex = selectedCamera - 1;

        if (selectedIndex >= 0 && selectedIndex < cameraButtons.Length)
        {
            cameraButtons[selectedIndex].image.color = selectedColor;
        }
    }

    private void PaintSensor(Animatronic anim)
    {
        if (anim.attacking) return;
        if (anim.route == null || anim.route.Length == 0) return;

        int cameraNumber = -1;

        if (anim.takingBranch)
        {
            int[] branch = anim.usingBranchA ?
                anim.routeContinuationA :
                anim.routeContinuationB;

            if (branch == null || branch.Length == 0) return;

            if (anim.branchIndex < 0 || anim.branchIndex >= branch.Length)
            {
                Debug.LogWarning(anim.name + " tiene branchIndex fuera de rango: " + anim.branchIndex);
                return;
            }

            cameraNumber = branch[anim.branchIndex];
        }
        else
        {
            if (anim.routeIndex < 0 || anim.routeIndex >= anim.route.Length)
            {
                Debug.LogWarning(anim.name + " tiene routeIndex fuera de rango: " + anim.routeIndex);
                return;
            }

            cameraNumber = anim.route[anim.routeIndex];
        }


        int buttonIndex = cameraNumber - 1;

        if (buttonIndex >= 0 && buttonIndex < cameraButtons.Length)
        {
            Color visibleColor = anim.sensorColor;
            visibleColor.a = 1f;

            cameraButtons[buttonIndex].image.color = visibleColor;
        }
    }

    private void SelectCamera(int cameraNumber)
    {
        selectedCamera = cameraNumber;
        Debug.Log("Camara seleccionada: " + selectedCamera);
    }

    public void SelectCam1() { selectedCamera = 1; }
    public void SelectCam2() { selectedCamera = 2; }
    public void SelectCam3() { selectedCamera = 3; }
    public void SelectCam4() { selectedCamera = 4; }
    public void SelectCam5() { selectedCamera = 5; }
    public void SelectCam6() { selectedCamera = 6; }
    public void SelectCam7() { selectedCamera = 7; }
    public void SelectCam8() { selectedCamera = 8; }
    public void SelectCam9() { selectedCamera = 9; }

    // SCREAMER MUERTE

    private void TriggerDeath(Animatronic killer)
    {
        if (isDying) return;

        isDying = true;
        gameEnded = true;

        StartCoroutine(DeathSequence(killer));
    }

    private IEnumerator DeathSequence(Animatronic killer)
    {
        SetActiveSafe(greenAnimatronic.peekVisualLeft, false);
        SetActiveSafe(greenAnimatronic.peekVisualRight, false);
        SetActiveSafe(greenAnimatronic.peekVisualBack, false);
        SetActiveSafe(blueAnimatronic.peekVisualLeft, false);
        SetActiveSafe(blueAnimatronic.peekVisualRight, false);
        SetActiveSafe(blueAnimatronic.peekVisualBack, false);

        if (killer.screamerPanel != null)
        {
            killer.screamerPanel.SetActive(true);
        }

        if (audioManager != null)
        {
            audioManager.Screamer();
        }

        yield return new WaitForSeconds(screamerDuration);

        ButtonSounds.GuardarNivelActual();
        SceneManager.LoadScene(gameOverSceneName);
    }

    private void WinGame()
    {
        gameEnded = true;

        ButtonSounds.GuardarNivelActual();

        SceneManager.LoadScene(winSceneName);
    }
}