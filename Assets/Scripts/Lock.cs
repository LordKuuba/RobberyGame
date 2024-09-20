using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public enum LockType
{
    ItemLock,
    MiniGameLock
}

public class Lock : NetworkBehaviour
{
    [SerializeField] private GameObject m_lockModel;
    [SerializeField] private LockType m_typeOfLock;

    [Space]
    [Header("Item Lock")]
    [SerializeField] private ItemParameters m_itemToOpen;
    [SerializeField] private bool m_removeAfterUse;

    [Space]
    [Header("Mini Game Lock")]
    [SerializeField] private GameObject m_miniGamePrefab;
    [SerializeField] private Task m_spawnedMiniGame;
    private Transform m_canvasToDisplay;

    public bool IsLockUnlocked => isUnlocked;
    public ItemParameters ItemToUnlock => m_itemToOpen;
    public bool NeedToDestroyItemAfterUse => m_removeAfterUse;
    public LockType UsedTypeOfLock => m_typeOfLock;
    public bool IsTaskInProgress => taskInProgress;

    [SyncVar]
    private bool isUnlocked;

    [SyncVar] private bool taskInProgress;

    public void SpawnTask(Transform interactedCanvas, UnityAction callActionWhenDone)
    {
        if(m_spawnedMiniGame == null)
        {
            GameObject spawnedTask = Instantiate(m_miniGamePrefab, interactedCanvas);
            m_spawnedMiniGame = spawnedTask.GetComponent<Task>();

            m_spawnedMiniGame.SetCallWhenDone(callActionWhenDone);
        }
    }

    public void CancelTask()
    {
        if(m_spawnedMiniGame != null)
        {
            Destroy(m_spawnedMiniGame.gameObject);
        }
    }

    public void DestroyTask()
    {
        if (m_spawnedMiniGame != null)
        {
            Destroy(m_spawnedMiniGame.gameObject);
            UpdateState(true);
        }
    }

    public void UpdateState(bool newState)
    {
        CmdUpdateState(newState);
    }
    [Command(requiresAuthority = false)]
    void CmdUpdateState(bool newState)
    {
        RpcUpdateState(newState);
    }
    [ClientRpc]
    void RpcUpdateState(bool newState)
    {
        isUnlocked = newState;
    }

    public void RemoveLockModel()
    {
        CmdRemoveLockModel();
    }
    [Command(requiresAuthority = false)]
    void CmdRemoveLockModel()
    {
        RpcRemoveLockModel();
    }
    [ClientRpc]
    void RpcRemoveLockModel()
    {
        isUnlocked = true;
        m_lockModel.SetActive(false);
    }
}
