using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemDisposer : Interactable
{
    [Space]
    [SerializeField] private GameObject m_partToRemove; //Just Visual represntation of item "Disapiring from stand"
    [SerializeField] private GameObject m_itemToDispose; //Prefab of an item, that will player get.

    public override void OnInteracted(Player g_playerWhoInteracted)
    {
        if (!g_playerWhoInteracted.PlayerInventory.IsCurrentSlotTaken)
        {
            base.OnInteracted(g_playerWhoInteracted);
            HidePartToRemove();
            SpawnItem(g_playerWhoInteracted);
        }
        else
        {
            Debug.Log($"Player {g_playerWhoInteracted} has current slot taken!");
        }
    }

    void SpawnItem(Player playerWhoInteracted)
    {
        CmdSpawnItem(playerWhoInteracted);
    }
    [Command(requiresAuthority = false)]
    void CmdSpawnItem(Player playerWhoInteracted)
    {
        var spawnedItemPrefab = Instantiate(m_itemToDispose);
        spawnedItemPrefab.transform.position = playerWhoInteracted.PlayerInventory.PlayerHand.position;

        NetworkServer.Spawn(spawnedItemPrefab, connectionToClient);

        RpcSpawnItem(playerWhoInteracted, spawnedItemPrefab.GetComponent<Item>());
    }

    [ClientRpc]
    void RpcSpawnItem(Player playerWhoInteracted, Item spawnedItem)
    {
        playerWhoInteracted.PlayerInventory.AddItemToCurrentlySelectedSlot(spawnedItem);
    }

    void HidePartToRemove()
    {
        CmdHidePartToRemove();
    }
    [Command(requiresAuthority = false)]
    void CmdHidePartToRemove()
    {
        RpcHidePartToRemove();
    }
    [ClientRpc]
    void RpcHidePartToRemove()
    {
        m_partToRemove.SetActive(false);
        m_interactable = false;
    }
}
