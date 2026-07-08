using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public int[] route;
    public AttackView attackView;
    public float moveTime = 8f;

    [HideInInspector] public int routeIndex = 0;
    [HideInInspector] public int previousRouteIndex = 0;
    [HideInInspector] public float moveTimer = 0f;
    [HideInInspector] public float attackTimer = 0f;
    [HideInInspector] public bool attacking = false;
}

public class AnimatronicManager : MonoBehaviour
{
    [Header("Animatronicos")]
    [SerializeField] private Animatronic greenAnimatronic;
    [SerializeField] private Animatronic blueAnimatronic;

    [Header("Tablet - botones Camara 1 a 9")]
    [SerializeField] private Button[] cameraButtons;

    [Header("Reglas")]
    [SerializeField] private float timeToReact = 6f;
    [SerializeField] private string gameOverSceneName = "GameOver";

    private int selectedCamera = 1;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color selectedColor = new Color(1f, 1f, 0f, 1f);

    private void Start()
    {
        RefreshTabletSensors();
    }

    private void Update()
    {
        UpdateAnimatronic(greenAnimatronic);
        UpdateAnimatronic(blueAnimatronic);
        RefreshTabletSensors();
    }
    private void UpdateAnimatronic(Animatronic anim)
    {
        if (anim.attacking)
        {
            anim.attackTimer += Time.deltaTime;

            if (anim.attackTimer >= timeToReact)
            {
                SceneManager.LoadScene(gameOverSceneName);
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
        anim.previousRouteIndex = anim.routeIndex;

        if (anim.routeIndex < anim.route.Length - 1)
        {
            anim.routeIndex++;
        }

        if (anim.routeIndex == anim.route.Length - 1)
        {
            anim.attacking = true;
            anim.attackTimer = 0f;
        }
    }
    public void UseFlashlight(RoomView currentView)
    {
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

        anim.attacking = false;
        anim.attackTimer = 0f;
        anim.routeIndex = anim.previousRouteIndex;
        anim.moveTimer = 0f;
    }
    public void PlaySound()
    {
        Debug.Log("Sonido activado en Camara " + selectedCamera);

        PullToCamera(greenAnimatronic, selectedCamera);
        PullToCamera(blueAnimatronic, selectedCamera);

        RefreshTabletSensors();
    }

    private void PullToCamera(Animatronic anim, int cameraNumber)
    {
        if (anim.attacking) return;

        for (int i = 0; i < anim.route.Length; i++)
        {
            if (anim.route[i] == cameraNumber)
            {
                anim.previousRouteIndex = anim.routeIndex;
                anim.routeIndex = i;
                anim.moveTimer = 0f;

                Debug.Log(anim.name + " fue atraido a Camara " + cameraNumber);
                return;
            }
        }

        Debug.Log(anim.name + " no puede ir a Camara " + cameraNumber);
    }
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

        int cameraNumber = anim.route[anim.routeIndex];
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

    
}