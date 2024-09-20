using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory m_inventory;
    [SerializeField] private GameObject m_slotUIPrefab;
    [SerializeField] private Transform m_UIInventoryParent;

    private List<InventoryUISlot> p_listOfUISlots = new List<InventoryUISlot>();

    private void Awake()
    {
        for(int i = 0; i < m_inventory.InventorySlotCount; i++)
        {
            GameObject spawnedPrefab = Instantiate(m_slotUIPrefab, m_UIInventoryParent);
            spawnedPrefab.GetComponent<InventoryUISlot>().UpdateSlotImage(null);
            p_listOfUISlots.Add(spawnedPrefab.GetComponent<InventoryUISlot>());
        }
        UpdateSelectedUiSlot();
    }

    public void PassItemImageToSlot(ItemParameters itemParametersToPass, int slotIndex)
    {
        p_listOfUISlots[slotIndex].UpdateSlotImage(itemParametersToPass.ItemUISprite);
    }

    public void ClearCurrentSlot()
    {
        p_listOfUISlots[m_inventory.CurrentlySelectedSlotIndex].UpdateSlotImage(null);
    }

    public void UpdateSelectedUiSlot()
    {
        for(int i = 0; i < m_inventory.InventorySlotCount; i++)
        {
            p_listOfUISlots[i].ChangeUsingState(false);
        }

        p_listOfUISlots[m_inventory.CurrentlySelectedSlotIndex].ChangeUsingState(true);
    }
}
