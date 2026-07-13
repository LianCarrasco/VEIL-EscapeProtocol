using System.Collections.Generic;
using UnityEngine;

public enum DoorSide
{
    Left,
    Right,
    None
}

public enum Level3EnemyType
{
    DoorAttacker,
    GeneratorAttacker
}

[System.Serializable]
public class Level3RouteStep
{
    public string roomID;
    public bool isHallway;
    public bool isGeneratorRoom;
    public DoorSide doorSide;
}

public class Level3Enemy : MonoBehaviour
{
    [Header("Datos del enemigo")]
    [SerializeField] private string enemyName;
    [SerializeField] private Level3EnemyType enemyType;

    [Header("Ruta")]
    [SerializeField] private List<Level3RouteStep> route = new List<Level3RouteStep>();

    [Header("Movimiento")]
    [SerializeField] private float minMoveTime = 7f;
    [SerializeField] private float maxMoveTime = 12f;

    [Header("Jumpscare")]
    [SerializeField] private GameObject jumpscareVisual;

    [Header("Visuales")]
    [SerializeField] private GameObject cameraVisual;

    public GameObject CameraVisual => cameraVisual;
    public GameObject JumpscareVisual => jumpscareVisual;

    private Level3Manager levelManager;

    private int currentRouteIndex;
    private float moveTimer;
    private float hallwayAttackTimer;

    private bool isInHallway;
    private bool isAtGenerator;
    private bool isActive;

    public string EnemyName => enemyName;

    public string CurrentRoomID
    {
        get
        {
            if (route == null || route.Count == 0)
            {
                return "";
            }

            return route[currentRouteIndex].roomID;
        }
    }

    public bool IsInHallway => isInHallway;
    public bool IsAtGenerator => isAtGenerator;

    public DoorSide CurrentDoorSide
    {
        get
        {
            if (route == null || route.Count == 0)
            {
                return DoorSide.None;
            }

            return route[currentRouteIndex].doorSide;
        }
    }

    public void Initialize(Level3Manager manager)
    {
        levelManager = manager;

        currentRouteIndex = 0;
        isInHallway = false;
        isAtGenerator = false;
        isActive = true;

        if (cameraVisual != null)
        {
            cameraVisual.SetActive(false);
        }

        if (jumpscareVisual != null)
        {
            jumpscareVisual.SetActive(false);
        }
        ScheduleNextMove();
    }

    public void Tick(float deltaTime)
    {
        if (!isActive)
        {
            return;
        }

        if (isInHallway)
        {
            hallwayAttackTimer -= deltaTime;

            if (hallwayAttackTimer <= 0f)
            {
                isActive = false;
                levelManager.StartEnemyJumpscare(this, enemyName + " entró a la habitación del jugador.");
            }

            return;
        }

        if (isAtGenerator)
        {
            return;
        }

        moveTimer -= deltaTime;

        if (moveTimer <= 0f)
        {
            MoveForward();
            ScheduleNextMove();
        }
    }

    private void ScheduleNextMove()
    {
        moveTimer = Random.Range(minMoveTime, maxMoveTime);
    }

    private void MoveForward()
    {
        if (route == null || route.Count == 0)
        {
            return;
        }

        if (currentRouteIndex >= route.Count - 1)
        {
            return;
        }

        currentRouteIndex++;

        Level3RouteStep currentStep = route[currentRouteIndex];

        Debug.Log(enemyName + " se movió a: " + currentStep.roomID);

        if (currentStep.isHallway)
        {
            EnterHallway(currentStep.doorSide);
        }

        if (currentStep.isGeneratorRoom && enemyType == Level3EnemyType.GeneratorAttacker)
        {
            EnterGenerator();
        }
    }

    public bool IsInRoom(string roomID)
    {
        return CurrentRoomID == roomID && !isInHallway;
    }

    private void EnterHallway(DoorSide side)
    {
        isInHallway = true;
        hallwayAttackTimer = levelManager.HallwayAttackTime;

        Debug.Log(enemyName + " está en el pasillo " + side + ". Tienes " + hallwayAttackTimer + " segundos para cerrar la puerta.");

        levelManager.OnEnemyEnteredHallway(this, side);
    }

    private void EnterGenerator()
    {
        Debug.Log("ENTRO AL GENERADOR: " + enemyName);

        isAtGenerator = true;

        levelManager.OnEnemyDamagedGenerator(this);

        Debug.Log(enemyName + " dañó el generador.");
    }

    public bool IsInDoorHallway(DoorSide side)
    {
        return isInHallway && CurrentDoorSide == side;
    }

    public void RetreatFromHallway()
    {
        if (!isInHallway)
        {
            return;
        }

        ReturnToSpawn();

        Debug.Log(enemyName + " fue detenido por la puerta y volvió al Spawn.");
    }

    private void ReturnToSpawn()
    {
        isInHallway = false;
        isAtGenerator = false;

        currentRouteIndex = 0;

        ScheduleNextMove();

        Debug.Log(enemyName + " volvió a: " + CurrentRoomID);
    }

    public bool CanMoveToRoomBySound(string targetRoomID)
    {
        if (isInHallway || isAtGenerator)
        {
            return false;
        }

        for (int i = 0; i < route.Count; i++)
        {
            if (route[i].roomID == targetRoomID && !route[i].isHallway && i != 0)
            {
                return true;
            }
        }

        return false;
    }

    public void MoveToRoomBySound(string targetRoomID)
    {
        if (!CanMoveToRoomBySound(targetRoomID))
        {
            return;
        }

        for (int i = 0; i < route.Count; i++)
        {
            if (route[i].roomID == targetRoomID)
            {
                currentRouteIndex = i;
                isInHallway = false;
                isAtGenerator = false;

                ScheduleNextMove();

                Debug.Log(enemyName + " escuchó el sonido y fue a: " + CurrentRoomID);
                return;
            }
        }
    }

    public void RetreatAfterGeneratorRepair()
    {
        ReturnToSpawn();

        Debug.Log(enemyName + " salió del generador y volvió al Spawn.");
    }
}
