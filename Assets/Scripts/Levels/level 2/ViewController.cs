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

    private RoomView currentView = RoomView.Central;
    private bool tabletOpen = false;
    private void Start()
    {
        UpdateView(RoomView.Central);

        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
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
        tabletOpen = true;
        tabletPanel.SetActive(true);
    }

    private void CloseTablet()
    {
        tabletOpen = false;
        tabletPanel.SetActive(false);
    }

    public void ToggleTablet()
    {
        if (tabletOpen)
        {
            CloseTablet();
        }
        else
        {
            OpenTablet();
        }
    }

    private void UpdateView(RoomView newView)
    {
        currentView = newView;

        centralView.SetActive(currentView == RoomView.Central);
        leftView.SetActive(currentView == RoomView.Left);
        rightView.SetActive(currentView == RoomView.Right);
        backView.SetActive(currentView == RoomView.Back);
    }
}
