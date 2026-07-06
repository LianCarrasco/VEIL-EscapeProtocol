using UnityEngine;

public enum RoomView
{
    Central,
    Left,
    Right,
    Back
}

public class ViewController : MonoBehaviour
{
    [Header("GameObjects de las Vistas")]
    [SerializeField] private GameObject centralView;
    [SerializeField] private GameObject leftView;
    [SerializeField] private GameObject rightView;
    [SerializeField] private GameObject backView;

    [Header("Tablet")]
    [SerializeField] private GameObject tabletPanel;
    [Header("Linterna")]
    [SerializeField] private GameObject flashlightEffect;

    private bool flashlightOn = false;
    private RoomView currentView = RoomView.Central;
    private bool tabletOpen = false;
    private void Start()
    {
        UpdateView(RoomView.Central);

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
        }

        if (flashlightEffect != null)
        {
            flashlightEffect.SetActive(false);
        }
    }

    public void MoveLeft()
    {
        switch (currentView)
        {
            case RoomView.Central:
                UpdateView(RoomView.Left);
                break;

            case RoomView.Right:
                UpdateView(RoomView.Central);
                break;

            case RoomView.Back:
                UpdateView(RoomView.Right);
                break;
        }
    }

    public void MoveRight()
    {
        switch (currentView)
        {
            case RoomView.Central:
                UpdateView(RoomView.Right);
                break;

            case RoomView.Left:
                UpdateView(RoomView.Central);
                break;

            case RoomView.Back:
                UpdateView(RoomView.Left);
                break;
        }
    }

    public void MoveToBack()
    {
        if (currentView == RoomView.Central)
        {
            UpdateView(RoomView.Back);
        }
        else
        {
            UpdateView(RoomView.Central);
        }
    }

    public void OpenTablet()
    {
        if (!CanOpenTablet())
        {
            return;
        }

        tabletOpen = true;
        tabletPanel.SetActive(true);
    }

    private void CloseTablet()
    {
        tabletOpen = false;

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
        }
            
    }

    public void ToggleTablet()
    {

        if (tabletOpen)
        {
            CloseTablet();
            return;
        }
        
        OpenTablet();
        
    }

    private void UpdateView(RoomView newView)
    {
        TurnOffFlashlight();
        //s
        CloseTablet();
        currentView = newView;

        centralView.SetActive(currentView == RoomView.Central);
        leftView.SetActive(currentView == RoomView.Left);
        rightView.SetActive(currentView == RoomView.Right);
        backView.SetActive(currentView == RoomView.Back);
    }

    public void ToggleFlashlight()
    {
        if (!CanUseFlashlight())
        {
            TurnOffFlashlight();
            return;
        }

        flashlightOn = !flashlightOn;
        flashlightEffect.SetActive(flashlightOn);

        if (flashlightOn)
        {
            ScareAnimatronics();
        }
    }

    public void TurnOffFlashlight()
    {
        flashlightOn = false;

        if (flashlightEffect != null)
        {
            flashlightEffect.SetActive(false);
        }
    }

    private bool CanUseFlashlight()
    {
        //Este bool identifica si se puede usar la linterna. En su defecto solo en vistas laterales o trasera
        return currentView == RoomView.Left ||
               currentView == RoomView.Right ||
               currentView == RoomView.Back;
    }
    //s
    private bool CanOpenTablet()
    {
        return currentView == RoomView.Central;
    }

    private void ScareAnimatronics()
    {
        Debug.Log("Linterna activada: aqui se alejaria al animatronico.");
    }
}
