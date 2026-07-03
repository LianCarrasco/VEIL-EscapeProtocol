using UnityEngine;
using UnityEngine.UI;

public class Level3RoomButton : MonoBehaviour
{
    [SerializeField] private string roomID;
    [SerializeField] private Button button;
    [SerializeField] private Level3Manager level3Manager;

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OpenRoomCamera);
        }
    }

    private void OpenRoomCamera()
    {
        if (level3Manager != null)
        {
            level3Manager.OpenCameraRoom(roomID);
        }
    }
}
