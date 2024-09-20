using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Door : Interactable
{
    [Space]
    [SerializeField] private Transform m_doorRotationPoint;
    [SerializeField] private float m_openingDuration;

    [Space]
    [SerializeField] private Lock m_lockOnDoor;

    private bool isLockUsed;

    [SyncVar] private bool IsDoorClosed = true;
    [SyncVar] private int m_direction = 0;

    private Transform interactionTarget;
    private Player p_interactedPlayer;

    [SyncVar]
    private bool taskInProgress;

    private bool needToCheckTask;

    private void Awake()
    {
        isLockUsed = m_lockOnDoor != null;
    }

    private void Update()
    {
        if (taskInProgress)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                m_lockOnDoor.CancelTask();
                p_interactedPlayer.ChangeMovementInputState(true);
                p_interactedPlayer.ChangeInteractingState(false);
                taskInProgress = false;  // Ensure taskInProgress is set to false after cancelling the task
            }
        }
        if (isLockUsed && needToCheckTask)
        {
            if (!taskInProgress && m_lockOnDoor.IsLockUnlocked)
            {
                m_lockOnDoor.DestroyTask();
                needToCheckTask = false;
            }
        }
    }

    public override void OnInteracted(Player g_playerWhoInteracted)
    {
        p_interactedPlayer = g_playerWhoInteracted;
        p_interactedPlayer.ChangeInteractingState(true);

        if (isLockUsed)
        {
            if (m_lockOnDoor.IsLockUnlocked)
            {
                PerformDoorInteraction(g_playerWhoInteracted);
            }
            else
            {
                HandleLockedDoor(g_playerWhoInteracted);
            }
        }
        else
        {
            PerformDoorInteraction(g_playerWhoInteracted);
        }
    }

    private void HandleLockedDoor(Player g_playerWhoInteracted)
    {
        if (m_lockOnDoor.UsedTypeOfLock == LockType.ItemLock)
        {
            if (g_playerWhoInteracted.PlayerInventory.CurrentItemDataInHand == m_lockOnDoor.ItemToUnlock)
            {
                if (m_lockOnDoor.NeedToDestroyItemAfterUse)
                {
                    g_playerWhoInteracted.PlayerInventory.DestroyCurrentItemInHand();
                }

                m_lockOnDoor.RemoveLockModel();
                PerformDoorInteraction(g_playerWhoInteracted);
            }
            p_interactedPlayer.ChangeInteractingState(false);
        }
        else if (m_lockOnDoor.UsedTypeOfLock == LockType.MiniGameLock)
        {
            taskInProgress = true;
            p_interactedPlayer.ChangeMovementInputState(false);
            m_lockOnDoor.SpawnTask(g_playerWhoInteracted.PlayerCanvas.transform, CompleteTaskAndOpenDoor);
        }
    }

    private void PerformDoorInteraction(Player g_playerWhoInteracted)
    {
        needToCheckTask = true;
        base.OnInteracted(g_playerWhoInteracted);
        interactionTarget = g_playerWhoInteracted.transform;
        p_interactedPlayer.ChangeInteractingState(false);
        CalculationOpeningDirection(interactionTarget);
        CmdDoorOpening();
    }

    private void CompleteTaskAndOpenDoor()
    {
        m_lockOnDoor.RemoveLockModel();
        p_interactedPlayer.ChangeMovementInputState(true);
        p_interactedPlayer.ChangeInteractingState(false);
        taskInProgress = false; // Set taskInProgress to false after the task is completed
        PerformDoorInteraction(p_interactedPlayer);
    }

    [Command(requiresAuthority = false)]
    void CmdDoorOpening()
    {
        RpcDoorOpening();
    }

    [ClientRpc]
    void RpcDoorOpening()
    {
        StartCoroutine(DoorInteractingAnimation());
        IsDoorClosed = !IsDoorClosed;
    }

    IEnumerator DoorInteractingAnimation()
    {
        m_interactable = false;

        Quaternion initialRotation = m_doorRotationPoint.localRotation;

        Quaternion finalDoorRotation;

        if (IsDoorClosed)
        {
            finalDoorRotation = Quaternion.Euler(0, m_direction, 0);
        }
        else
        {
            finalDoorRotation = Quaternion.Euler(0, 0, 0);
        }

        float elapsedTime = 0f;

        while (elapsedTime < m_openingDuration)
        {
            float time = elapsedTime / m_openingDuration;
            m_doorRotationPoint.localRotation = Quaternion.Lerp(initialRotation, finalDoorRotation, time);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_doorRotationPoint.localRotation = finalDoorRotation;
        m_interactable = true;
    }

    [Command(requiresAuthority = false)]
    void CmdCalculationOpeningDirection(Transform player)
    {
        RpcCalculationOpeningDirection(player);
    }

    [ClientRpc]
    void RpcCalculationOpeningDirection(Transform player)
    {
        Vector3 directionToTarget = transform.position - player.position;

        float angle = Vector3.Angle(transform.forward, directionToTarget);

        if (Mathf.Abs(angle) < 90)
        {
            m_direction = -90;
        }
        else
        {
            m_direction = 90;
        }
    }

    void CalculationOpeningDirection(Transform player)
    {
        CmdCalculationOpeningDirection(player);
    }
}
