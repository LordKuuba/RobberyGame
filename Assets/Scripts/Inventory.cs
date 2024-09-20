using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Inventory : NetworkBehaviour
{
    [SerializeField] private int amountOfSlots;

    [SyncVar]
    [SerializeField] private List<Item> m_listOfItems = new List<Item>();
    [SerializeField] private Transform m_playersHand;

    [Space]
    [SerializeField] private InventoryUI m_inventoryUI;

    public int CurrentlySelectedSlotIndex => p_currentSelectedInventorySlot;
    public int InventorySlotCount => amountOfSlots;
    public Transform PlayerHand => m_playersHand;
    public bool IsCurrentSlotTaken => p_isCurrentSlotTaken;
    public ItemParameters CurrentItemDataInHand => p_currentItemDataInHand;

    public List<Item> ListOfAllItems => m_listOfItems;

    private int p_currentSelectedInventorySlot = 0;
    private bool p_useInvetoryUI = false;
    private bool p_isCurrentSlotTaken = false;
    private ItemParameters p_currentItemDataInHand;

    public void UpdateCurrentSlotInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            p_currentSelectedInventorySlot = 0;
            UpdateCurrentlySelectedSlot(p_currentSelectedInventorySlot);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            p_currentSelectedInventorySlot = 1;
            UpdateCurrentlySelectedSlot(p_currentSelectedInventorySlot);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            p_currentSelectedInventorySlot = 2;
            UpdateCurrentlySelectedSlot(p_currentSelectedInventorySlot);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            p_currentSelectedInventorySlot = 3;
            UpdateCurrentlySelectedSlot(p_currentSelectedInventorySlot);
        }
    }
    void UpdateCurrentlySelectedSlot(int slotIndex)
    {
        for(int i = 0; i < m_listOfItems.Count; i++)
        {
            if(m_listOfItems[i] != null)
            {
                m_listOfItems[i].ChangeDrawState(false);
            }
        }

        if(m_listOfItems[p_currentSelectedInventorySlot] != null)
        {
            m_listOfItems[p_currentSelectedInventorySlot].ChangeDrawState(true);
            m_listOfItems[p_currentSelectedInventorySlot].ChangePhysicsState(false);

            CmdSetFollowTarget(m_listOfItems[p_currentSelectedInventorySlot]);

            p_isCurrentSlotTaken = true;
            p_currentItemDataInHand = m_listOfItems[p_currentSelectedInventorySlot].ItemData;
        }
        else
        {
            p_isCurrentSlotTaken = false;
            p_currentItemDataInHand = null;
        }

        if (p_useInvetoryUI)
        {
            m_inventoryUI.UpdateSelectedUiSlot();
        }
    }
    void Awake()
    {
        for(int i = 0; i < amountOfSlots; i++)
        {
            m_listOfItems.Add(null);
        }

        p_useInvetoryUI = m_inventoryUI == null ? false : true;
    }

    public void DestroyCurrentItemInHand()
    {
        CmdDestroyCurrentItemInHand();
        if (p_useInvetoryUI)
        {
            m_inventoryUI.ClearCurrentSlot();
        }
    }

    [Command(requiresAuthority = false)]
    void CmdDestroyCurrentItemInHand()
    {
        NetworkServer.Destroy(m_listOfItems[p_currentSelectedInventorySlot].gameObject);
        m_listOfItems[p_currentSelectedInventorySlot] = null;
        UpdateCurrentlySelectedSlot(p_currentSelectedInventorySlot);
    }

    public void AddItemToCurrentlySelectedSlot(Item g_itemToAdd)
    {
        if (m_listOfItems[p_currentSelectedInventorySlot] != null)
        {
            return;
        }

        CmdPickupItem(g_itemToAdd.GetComponent<NetworkIdentity>());

        CmdAddCurrentItemInList(g_itemToAdd, p_currentSelectedInventorySlot);

        UpdateCurrentlySelectedSlot(p_currentSelectedInventorySlot);

        if (p_useInvetoryUI)
        {
            m_inventoryUI.PassItemImageToSlot(g_itemToAdd.ItemData, p_currentSelectedInventorySlot);
        }
    }

    public void AddItemToSlot(Item g_itemToAdd, int slotIndex)
    {
        if (m_listOfItems[slotIndex] != null)
        {
            return;
        }

        CmdPickupItem(g_itemToAdd.GetComponent<NetworkIdentity>());

        CmdAddCurrentItemInList(g_itemToAdd, slotIndex);

        UpdateCurrentlySelectedSlot(p_currentSelectedInventorySlot);

        if (p_useInvetoryUI)
        {
            m_inventoryUI.PassItemImageToSlot(g_itemToAdd.ItemData, slotIndex);
        }
    }

    [Command(requiresAuthority = false)]
    void CmdAddCurrentItemInList(Item itemToAdd, int slotIndex)
    {
        RpcAddCurrentItemInList(itemToAdd, slotIndex);
    }
    [ClientRpc]
    void RpcAddCurrentItemInList(Item itemToAdd, int slotIndex)
    {
        m_listOfItems[slotIndex] = itemToAdd;

        UpdateCurrentlySelectedSlot(slotIndex);
    }

    public void ReleaseCurrentlySelectedItem()
    {
        //CmdReleaseCurrentlySelectedItem();
        if (m_listOfItems[p_currentSelectedInventorySlot] == null)
        {
            return;
        }

        m_listOfItems[p_currentSelectedInventorySlot].ClearFollowTarget();

        m_listOfItems[p_currentSelectedInventorySlot].ChangePhysicsState(true);

        CmdRemoveItem(m_listOfItems[p_currentSelectedInventorySlot].GetComponent<NetworkIdentity>());

        m_listOfItems[p_currentSelectedInventorySlot] = null;

        UpdateCurrentlySelectedSlot(p_currentSelectedInventorySlot);

        if (p_useInvetoryUI)
        {
            m_inventoryUI.ClearCurrentSlot();
        }
    }
    /*
    [Command(requiresAuthority = false)]
    void CmdReleaseCurrentlySelectedItem()
    {
        RpcReleaseCurrentlySelectedItem();
    }
    [ClientRpc]
    void RpcReleaseCurrentlySelectedItem()
    {
        if (m_listOfItems[p_currentSelectedInventorySlot] == null)
        {
            return;
        }

        m_listOfItems[p_currentSelectedInventorySlot].ClearFollowTarget();

        m_listOfItems[p_currentSelectedInventorySlot].ChangePhysicsState(true);

        CmdRemoveItem(m_listOfItems[p_currentSelectedInventorySlot].GetComponent<NetworkIdentity>());

        m_listOfItems[p_currentSelectedInventorySlot] = null;

        UpdateCurrentlySelectedSlot();
    }*/

    [Command(requiresAuthority = false)]
    void CmdSetFollowTarget(Item itemToSet)
    {
        RpcSetCurrentTarget(itemToSet);
    }
    [ClientRpc]
    void RpcSetCurrentTarget(Item itemToSet)
    {
        itemToSet.SetFollowTarget(m_playersHand.transform);
    }

    [Command(requiresAuthority = false)]
    void CmdPickupItem(NetworkIdentity identity)
    {
        identity.AssignClientAuthority(connectionToClient);
    }
    [Command(requiresAuthority = false)]
    void CmdRemoveItem(NetworkIdentity identity)
    {
        identity.RemoveClientAuthority();
    }
}
