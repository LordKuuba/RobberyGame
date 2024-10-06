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

    [SyncVar]
    public List<ItemParameters> ListOfAllItemParameters = new List<ItemParameters>();

    private int p_currentSelectedInventorySlot = 0;
    private bool p_useInvetoryUI = false;
    private bool p_isCurrentSlotTaken = false;
    private ItemParameters p_currentItemDataInHand;

    private List<ItemParameters> m_remainingItemsToCheck = new List<ItemParameters>();

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
        for (int i = 0; i < m_listOfItems.Count; i++)
        {
            if (m_listOfItems[i] != null)
            {
                m_listOfItems[i].ChangeDrawState(false);
            }
        }

        if (m_listOfItems[p_currentSelectedInventorySlot] != null)
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
        for (int i = 0; i < amountOfSlots; i++)
        {
            m_listOfItems.Add(null);
            ListOfAllItemParameters.Add(null);
        }

        p_useInvetoryUI = m_inventoryUI == null ? false : true;
    }

    public void DestroyItemInSlot(int slotIndex)
    {
        CmdDestroyCurrentItemInHand(m_listOfItems[slotIndex].gameObject);

        if (p_useInvetoryUI)
        {
            m_inventoryUI.ClearCurrentSlot(slotIndex);
        }

        m_listOfItems[slotIndex] = null;
        ListOfAllItemParameters[slotIndex] = null;

        //UpdateCurrentlySelectedSlot(p_currentSelectedInventorySlot);
    }


    [Command(requiresAuthority = false)]
    void CmdDestroyCurrentItemInHand(GameObject objectToDestroy)
    {
        if (objectToDestroy != null)
        {
            Debug.Log(objectToDestroy);

            NetworkServer.Destroy(objectToDestroy);
        }
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
        ListOfAllItemParameters[slotIndex] = itemToAdd.ItemData;

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
        ListOfAllItemParameters[p_currentSelectedInventorySlot] = null;

        UpdateCurrentlySelectedSlot(p_currentSelectedInventorySlot);

        if (p_useInvetoryUI)
        {
            m_inventoryUI.ClearCurrentSlot(p_currentSelectedInventorySlot);
        }
    }

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

    public void FindAndDestroyItem(ItemParameters g_itemToDestroy)
    {
        if (g_itemToDestroy == null)
        {
            Debug.LogError("g_itemToDestroy is NULL!");
            return;
        }

        for (int i = 0; i < ListOfAllItems.Count; i++)
        {
            if (ListOfAllItems[i] != null && ListOfAllItems[i].ItemData == g_itemToDestroy)
            {
                //Debug.Log(ListOfAllItems[i].ItemData);
                //Debug.Log(g_itemToDestroy);
                DestroyItemInSlot(i);

                return;
            }
        }
    }

    public bool HaveItems(ItemParameters[] listA)
    {
        ItemParameters[] m_arrayA = new ItemParameters[listA.Length];
        ItemParameters[] m_arrayB = new ItemParameters[m_listOfItems.Count];

        //Debug.Log(listA.Length);
        //Debug.Log(m_listOfItems.Count);

        // Copy listA to m_arrayA
        for (int i = 0; i < listA.Length; i++)
        {
            m_arrayA[i] = listA[i];
        }

        // Copy item data from m_listOfItems to m_arrayB, with null checks
        for (int i = 0; i < m_listOfItems.Count; i++)
        {
            if (m_listOfItems[i] != null && m_listOfItems[i].ItemData != null)
            {
                m_arrayB[i] = m_listOfItems[i].ItemData;
            }
            else
            {
                m_arrayB[i] = null;
            }
        }

        // Compare arrays and mark matches as null
        for (int i = 0; i < listA.Length; i++)
        {
            for (int x = 0; x < m_listOfItems.Count; x++)
            {
                if (m_arrayA[i] != null && m_arrayA[i] == m_arrayB[x])
                {
                    m_arrayA[i] = null;
                    m_arrayB[x] = null;
                    break;  // Break inner loop after finding a match
                }
            }
        }

        // Check if all elements in m_arrayA are null
        return allElementsAreNull(m_arrayA);
    }


    bool allElementsAreNull(ItemParameters[] array)
    {
        bool allNull = true;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                allNull = false;
            }
        }

        return allNull;
    }
}
